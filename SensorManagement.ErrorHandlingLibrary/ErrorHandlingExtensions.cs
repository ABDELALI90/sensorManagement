using Microsoft.Extensions.DependencyInjection;

namespace SensorManagement.ErrorHandlingLibrary
{
    public static class ErrorHandlingExtensions
    {
        public static IServiceCollection AddGlobalErrorHandling(this IServiceCollection services)
        {
            // Ensure GlobalExceptionFilter is added to the ASP.NET Core app
            services.Configure<Microsoft.AspNetCore.Mvc.MvcOptions>(options =>
            {
                options.Filters.Add<GlobalExceptionFilter>();
            });

            return services;
        }
    }
}
