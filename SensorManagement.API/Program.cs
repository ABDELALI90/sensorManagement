using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MediatR;
using System.Text;
using SensorManagement.Application.Interfaces;
using SensorManagement.Infrastructure.UnitOfWork.SensorManagement.Infrastructure;
using SensorManagement.Infrastructure.Repositories;
using SensorManagement.Application.Commands.CreateSensor;
using SensorManagement.Infrastructure.Persistence.SensorManagement.Infrastructure.Persistence;
using Npgsql;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using SensorManagement.Caching;
using Microsoft.Extensions.Configuration;
using Polly;
using StackExchange.Redis;
using SensorManagement.ErrorHandlingLibrary;
using SensorManagement.Application.Services;

var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    ContentRootPath = Directory.GetCurrentDirectory(),
    ApplicationName = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name
});

// Print the paths to help debug
Console.WriteLine($"ContentRootPath: {builder.Environment.ContentRootPath}");
Console.WriteLine($"Application Name: {builder.Environment.ApplicationName}");
Console.WriteLine($"Environment Name: {builder.Environment.EnvironmentName}");

try
{
    // Database connection test
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    if (string.IsNullOrEmpty(connectionString))
    {
        throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
    }

    using var connection = new NpgsqlConnection(connectionString);
    await connection.OpenAsync();
    Console.WriteLine("Connection successful!");
}
catch (Exception ex)
{
    Console.WriteLine($"Database connection failed: {ex.Message}");
    Console.WriteLine($"Stack trace: {ex.StackTrace}");
}
// Add DbContext for PostgreSQL
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));


builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddSingleton<IAsyncPolicy>(provider =>
    Policy.Handle<Exception>().RetryAsync(3)
);
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<ISensorRepository, SensorRepository>();

builder.Services.AddMemoryCache();
builder.Services.AddScoped<ICacheService, InMemoryCacheService>();
builder.Services.AddTransient<RedisCacheService>();

builder.Services.Configure<RedisCacheOptions>(options =>
{
    options.ConnectionString = builder.Configuration.GetConnectionString("RedisConnection");
});

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = "localhost:6379"; // Use your Redis server's address
    options.InstanceName = "RedisCacheInstance";
});

// Add Redis configuration
builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    var configuration = builder.Configuration.GetConnectionString("RedisConnection");
    return ConnectionMultiplexer.Connect(configuration);
});
builder.Services.AddScoped<ICacheService, RedisCacheService>();
//No need to write try-catch blocks in every controller action.

builder.Services.AddGlobalErrorHandling();
builder.Services.AddControllers(options =>
{
    options.Filters.Add<GlobalExceptionFilter>();
});

builder.Services.AddRedisCacheService(options =>
{
    options.ConnectionString = builder.Configuration.GetConnectionString("RedisConnection");
    options.ConnectTimeout = 5000;
    options.ConnectRetry = 5;
    options.KeepAlive = 30;
    options.DefaultDatabase = 0;
});
// Register repositories if needed (optional)
// builder.Services.AddScoped<ISensorRepository, SensorRepository>();

// Add MediatR for CQRS
builder.Services.AddMediatR(typeof(CreateSensorCommand).Assembly);

// Add Authentication with JWT
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))


        };
    });

// Add Controllers
builder.Services.AddControllers();

// Add Swagger for API documentation
// Add services to the container.

// Add API versioning
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0); // Set default version to 1.0
    options.AssumeDefaultVersionWhenUnspecified = true; // Use default version if none is specified
    options.ReportApiVersions = true; // Include API version information in the response headers
});
builder.Services.AddEndpointsApiExplorer();
// Add API version explorer for Swagger
builder.Services.AddVersionedApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV"; // Format API versions as "v1", "v2", etc.
    options.SubstituteApiVersionInUrl = true;
});
builder.Services.AddScoped<JwtTokenService>();

builder.Services.AddSwaggerGen(options =>
{
    var provider = builder.Services.BuildServiceProvider()
        .GetRequiredService<IApiVersionDescriptionProvider>();
    //options.SwaggerDoc("v1", new OpenApiInfo
    //{
    //    Version = "v1",
    //    Title = "Sensor Management API",
    //    Description = "API for managing sensors",
    //    TermsOfService = new Uri("https://example.com/terms"),
    //    Contact = new OpenApiContact
    //    {
    //        Name = "Yahya Abdelali",
    //        Email = "contact@example.com",
    //        Url = new Uri("https://example.com")
    //    },
    //    License = new OpenApiLicense
    //    {
    //        Name = "MIT License",
    //        Url = new Uri("https://opensource.org/licenses/MIT")
    //    }
    //});

    // Add security definitions for JWT
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Enter 'Bearer' [space] and your token in the text input below."
    });
    // Add security requirement for all endpoints
    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
    foreach (var description in provider.ApiVersionDescriptions)
    {
        options.SwaggerDoc(description.GroupName, new Microsoft.OpenApi.Models.OpenApiInfo
        {
            Title = $"Sensor Management API {description.GroupName}",
            Version = description.ApiVersion.ToString()
        });
    }
});

// Add CORS if required (optional)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    //app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Sensor Management API v1"));
    app.UseSwaggerUI(c =>
    {
        // Display API versions in Swagger UI
        var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();
        foreach (var description in provider.ApiVersionDescriptions)
        {
            c.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", description.GroupName.ToUpperInvariant());
        }
    });
}

app.UseHttpsRedirection();

app.UseAuthentication(); // Enable JWT Authentication
app.UseAuthorization();

app.MapControllers(); // Map controllers to the pipeline

// Enable CORS if required
app.UseCors("AllowAll");

app.Run();
