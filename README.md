# SensorManagement Solution

## Overview

The **SensorManagement** solution is a comprehensive system designed for managing sensors, including operations such as creating, updating, retrieving, and deleting sensors. The solution follows modern software engineering principles and is built using .NET 9.0 with MediatR for CQRS, Entity Framework Core for data persistence, and Redis for caching. It also includes centralized error handling through a custom library.

---

## Projects Structure
## plaintext
## Projects Structure

SensorManagement/
- .github/
  - workflows/               # CI/CD pipeline configurations
- SensorManagement.API/        # Main API project
- SensorManagement.Application/ # Application logic with CQRS
- SensorManagement.Caching/    # Redis caching functionalities
- SensorManagement.Domain/     # Core domain models and entities
- SensorManagement.ErrorHandlingLibrary/ # Centralized error handling utilities
- SensorManagement.Infrastructure/ # Data persistence using EF Core
- SensorManagement.Tests/      # Unit tests for API and application logic
- .gitignore                   # Git ignore rules
- docker-compose.yml           # Docker Compose configuration
- Dockerfile                   # Dockerfile for API
- README.md                    # Documentation
- SensorManagement.sln         # Solution file



### 1. **SensorManagement.API**

- **Purpose**: The main API project that exposes endpoints for managing sensors.
- **Technologies**: ASP.NET Core, MediatR.
- **Endpoints**:
  - `POST /api/v1/sensors`: Create a new sensor.
  - `GET /api/v1/sensors/{id}`: Retrieve sensor details by ID.
  - `PUT /api/v1/sensors/{id}`: Update sensor details.
  - `DELETE /api/v1/sensors/{id}`: Delete a sensor.
- **Features**:
  - API versioning.
  - Centralized error handling via `GlobalExceptionFilter`.

### 2. **SensorManagement.Application**

- **Purpose**: Contains the application logic, including commands, queries, and handlers for the CQRS pattern.
- **Key Components**:
  - **Commands**: Handlers for create, update, and delete operations.
  - **Queries**: Handlers for retrieving data.
- **Dependencies**: MediatR, IUnitOfWork interface.

### 3. **SensorManagement.Caching**

- **Purpose**: Provides caching functionality using Redis.
- **Key Features**:
  - Get, set, and remove operations for cached data.
  - Retry policies for robust Redis interactions.
- **Technologies**: StackExchange.Redis.

### 4. **SensorManagement.Domain**

- **Purpose**: Contains core domain models and entities.
- **Key Components**:
  - `Sensor`: Represents the sensor entity.

### 5. **SensorManagement.ErrorHandlingLibrary**

- **Purpose**: Provides a centralized error handling mechanism for the entire solution.
- **Key Features**:
  - `GlobalExceptionFilter`: Captures and handles all unhandled exceptions.
  - `AddGlobalErrorHandling`: Extension method for easy integration.

### 6. **SensorManagement.Infrastructure**

- **Purpose**: Handles data persistence and provides the implementation of the `IUnitOfWork` and repositories.
- **Technologies**:
  - Entity Framework Core.
  - PostgreSQL.
- **Key Components**:
  - `ApplicationDbContext`: Configures and manages database connections.
  - Repositories for interacting with database entities.

### 7. **SensorManagement.Tests**

- **Purpose**: Contains unit tests for all critical components in the solution.
- **Technologies**:
  - NUnit.
  - Moq for mocking dependencies.
  - FluentAssertions for test assertions.
- **Coverage**:
  - Controllers (e.g., `SensorsControllerTests`).
  - Command Handlers (e.g., `CreateSensorCommandHandlerTests`, `UpdateSensorCommandHandlerTests`, `DeleteSensorCommandHandlerTests`).

---

## Getting Started

### Prerequisites

- [.NET 9.0 SDK](https://dotnet.microsoft.com/)
- Docker (for PostgreSQL and Redis)
- Visual Studio 2022 or VS Code

### Setup Instructions

1. Clone the repository:
   ```bash
   git clone https://github.com/your-repo/SensorManagement.git
   cd SensorManagement
   ```
2. Build the solution:
   ```bash
   dotnet build
   ```
3. Set up the database and cache:
   - Start PostgreSQL and Redis using Docker:
     ```bash
     docker-compose up -d
     ```
   - Apply migrations:
     ```bash
     dotnet ef database update --project SensorManagement.Infrastructure
     ```
4. Run the application:
   ```bash
   dotnet run --project SensorManagement.API
   ```
5. Access the API documentation:
   - Swagger UI: [http://localhost:5000/swagger](http://localhost:5000/swagger)

---

## Managing Docker Containers

### Updated Docker Configuration

Ensure your `docker-compose.yml` includes the following configuration:

```yaml
version: '3'

services:
  sensor-api:
    build:
      context: .
      dockerfile: SensorManagement.API/Dockerfile
    container_name: sensor-api
    ports:
      - "5000:5000"
      - "5001:5001"

  postgres-db:
    image: postgres:latest
    container_name: postgres-db
    environment:
      POSTGRES_USER: admin
      POSTGRES_PASSWORD: admin
      POSTGRES_DB: SensorDB
    ports:
      - "5432:5432"
    volumes:
      - postgres-data:/var/lib/postgresql/data

  redis:
    image: redis:latest
    container_name: redis
    ports:
      - "6379:6379"

volumes:
  postgres-data:
```

### Start PostgreSQL and Redis Containers

To start PostgreSQL and Redis containers:

```bash
docker-compose up -d
```

### Check Running Containers

To verify that the containers are running:

```bash
docker ps
```

### Access PostgreSQL Database

To open a PostgreSQL interactive terminal inside the container:

```bash
docker exec -it postgres-db psql -U admin -d SensorDB
```

### Query Data in PostgreSQL

Once inside the PostgreSQL terminal, you can query data:

```sql
SELECT * FROM sensors;
```

### Access Redis Cache

To access the Redis CLI inside the container:

```bash
docker exec -it redis redis-cli
```

### Check Keys in Redis

List all keys in Redis:

```bash
KEYS *
```

Get the value of a specific key:

```bash
GET <key>
```

---

## Testing

Run the unit tests using the following command:

```bash
dotnet test
```

Ensure all tests pass successfully.

---

## Key Features

- **CQRS Pattern**: Separation of commands and queries for better scalability.
- **Caching**: Optimized performance using Redis for frequently accessed data.
- **Centralized Error Handling**: Consistent and clean error responses across all endpoints.
- **Unit Tests**: Comprehensive test coverage to ensure robustness.
- **API Versioning**: Supports multiple API versions.

---

## Future Enhancements

- Add authentication and authorization.
- Implement additional query endpoints (e.g., get all sensors).
- Extend caching to support more complex scenarios.
- Add integration tests.

---

## Contributors

- **Your Name** (Lead Developer)

---

## Docker Hub Credentials

For Docker integration, use the following credentials:
## Docker Hub Integration
Configure the following secrets in your GitHub repository:
- **DOCKER_USERNAME**: Your Docker Hub username.
- **DOCKER_PASSWORD**: Your Docker Hub access token.


---

## License

This project is licensed under the MIT License.

