# Operations Knowledge Management System

This is a backend-focused ASP.NET Core Web API for managing operational systems, their owners, status, and related organizational information. This project was built to demonstrate practical backend engineering skills including RESTful API design, Entity Framework Core, SQL Server, authentication and authorization, DTOs, service-layer architecture, error handling, and automated testing.

## Overview

The Operations Knowledge Management System provides endpoints for maintaining information about operational systems and the people responsible for them. For example, Susan might manage a software library. Give me details about the software library, or about Susan, or about other systems that Susan manages.

The API supports:

- Managing operational systems and their status and descriptions
- Managing people and their organizational information
- Assigning operational systems to owners
- Viewing the systems owned by a specific person
- Creating, updating, and deleting records
- Securing write operations with JWT authentication
- Restricting administrative operations with role-based authorization

## Architecture

The application follows a layered architecture that separates HTTP handling, business logic, data access, and API models.

- Controllers handle incoming API requests and return HTTP responses. They receive request Data Transfer Objects (DTOs), pass the data to the appropriate service, and convert the resulting entities into response DTOs.
- Services contain business logic and coordinate database operations through Entity Framework Core. This keeps business rules out of the controllers and makes the logic independently testable.
- DTOs define the API's request and response contracts without exposing the Entity Framework entities directly to API clients.
- Mappers handle conversion between Entity Framework entities and response DTOs. Mapping is kept explicit rather than relying on a mapping framework.
- Entity Framework Core provides the data-access layer and maps the application's entities to the SQL Server database.

## API

The API exposes endpoints for managing various entities such as people and operational systems. The list of entity types can grow as needed.

### People

| Method | Endpoint | Description |
|---|---|---|
| GET | `/people` | Get all people |
| GET | `/people/{id}` | Get a person |
| GET | `/people/{id}/owned-systems` | Get the operational systems owned by a person |
| POST | `/people` | Create a person |
| PUT | `/people/{id}` | Update a person |
| DELETE | `/people/{id}` | Delete a person |

### Operational Systems

| Method | Endpoint | Description |
|---|---|---|
| GET | `/operational-systems` | Get all operational systems |
| GET | `/operational-systems/{id}` | Get an operational system |
| POST | `/operational-systems` | Create an operational system |
| PUT | `/operational-systems/{id}` | Update an operational system |
| DELETE | `/operational-systems/{id}` | Delete an operational system |

The API uses standard HTTP status codes to communicate the outcome of requests, including `200 OK`, `201 Created`, `204 No Content`, `400 Bad Request`, `401 Unauthorized`, `403 Forbidden`, and `404 Not Found`. Interactive API documentation is provided through Swagger/OpenAPI when running the application in the Development environment.

## Authentication & Authorization

Some API endpoints require authentication. The API uses JSON Web Token (JWT) bearer authentication for these endpoints. Administrative operations, such as deleting people or operational systems, require the `Administrator` role. For local development, authentication configuration is stored using ASP.NET Core User Secrets rather than being committed to source control. A separate token-generator project is included for generating access tokens.

## Data Access

Entity Framework Core is used for data access, along with a SQL Server database. Database schema changes are managed through Entity Framework Core migrations. The service layer uses Entity Framework Core for querying and saving data, keeping database operations separate from the controllers.

## Testing

The project includes automated unit and integration tests using xUnit. Unit tests cover the service layer and its business logic. Integration tests exercise the API through HTTP requests, including authentication, authorization, validation, and error handling. Tests use an isolated in-memory SQLite database so they can run without requiring a separate database server.

## Running Locally

The application is configured to run locally using SQL Server LocalDB. A separate token-generator project is included in the repository for generating access tokens (JWTs). The generated token can be entered into Swagger's authorization dialog to access protected endpoints.

Follow these steps to run the project on your local machine. Commands listed below are run using Powershell.

1. Clone the repository.
1. Install SQL Server LocalDB.
1. Apply the Entity Framework Core migrations to the database schema.
   1. Install the Entity Framework Core CLI tool with `dotnet tool install --global dotnet-ef`.
   2. From the OperationsKnowledge project directory, run `dotnet ef database update`.
1. Configure the JWT signing key.
   1. Generate a signing key using command `[Convert]::ToBase64String((1..32 | ForEach-Object { Get-Random -Maximum 256 }))`
   2. From the OperationsKnowledge project directory, run `dotnet user-secrets set "Jwt:Key" "<generated signing key>"`
   3. From the OperationsKnowledge.TokenGenerator project directory, run `dotnet user-secrets set "Jwt:Key" "<generated signing key>"`
   4. Use the same key for both projects.
1. Generate an access token.
   1. Run the project `OperationsKnowledge.TokenGenerator` from Visual Studio.
   2. The generated tokens are displayed in the console.
   3. Copy the token that you'd like to use. One token provides Administrator access and the other provides standard User access.
1. Run the project and authorize Swagger.
   1. Run the project `OperationsKnowledge` from Visual Studio.
   2. The Swagger UI launches in a web browser.
   3. Click the `Authorize` button and paste your access token.
