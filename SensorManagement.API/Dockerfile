# Use the official .NET 9.0 runtime as a base image for production
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 5000
EXPOSE 5001

# Use the .NET 9.0 SDK as a base image for building the application
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy the project files and restore dependencies
COPY ["SensorManagement.API/SensorManagement.API.csproj", "SensorManagement.API/"]
COPY ["SensorManagement.Infrastructure/SensorManagement.Infrastructure.csproj", "SensorManagement.Infrastructure/"]
COPY ["SensorManagement.Application/SensorManagement.Application.csproj", "SensorManagement.Application/"]
COPY ["SensorManagement.Domain/SensorManagement.Domain.csproj", "SensorManagement.Domain/"]
RUN dotnet restore "SensorManagement.API/SensorManagement.API.csproj"

# Copy all project files and build the app
COPY . .
WORKDIR "/src/SensorManagement.API"
RUN dotnet build -c Release -o /app/build

# Publish the application
FROM build AS publish
RUN dotnet publish -c Release -o /app/publish

# Use the runtime image to run the application
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SensorManagement.API.dll"]
