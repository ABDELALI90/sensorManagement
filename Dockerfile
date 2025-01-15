# Use the official .NET 9 SDK image for building the application
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

# Set the working directory
WORKDIR /app

# Copy the solution file and all project files
COPY *.sln ./
COPY SensorManagement.API/*.csproj ./SensorManagement.API/
COPY SensorManagement.Application/*.csproj ./SensorManagement.Application/
COPY SensorManagement.Caching/*.csproj ./SensorManagement.Caching/
COPY SensorManagement.Domain/*.csproj ./SensorManagement.Domain/
COPY SensorManagement.ErrorHandlingLibrary/*.csproj ./SensorManagement.ErrorHandlingLibrary/
COPY SensorManagement.Infrastructure/*.csproj ./SensorManagement.Infrastructure/
COPY SensorManagement.Tests/*.csproj ./SensorManagement.Tests/

# Restore dependencies
RUN dotnet restore

# Copy the full source code for all projects
COPY . ./

# Publish the main application (SensorManagement.API)
RUN dotnet publish SensorManagement.API/SensorManagement.API.csproj -c Release -o /out

# Use the official ASP.NET Core runtime image for running the application
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime

# Set the working directory
WORKDIR /app

# Copy the published files from the build stage
COPY --from=build /out .

# Expose the default HTTP port
EXPOSE 5000

# Define the entry point for the application
ENTRYPOINT ["dotnet", "SensorManagement.API.dll"]
