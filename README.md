# AssetLog Microservices

AssetLog is an asset-management backend built with ASP.NET Core and .NET 8. The project was moved from a monolithic design into three focused REST services so that users, assets, and assignments can be developed and deployed as separate application boundaries.

## Architecture

```text
                    +------------------+
                    |   UserService    |  :5182
                    |   user.db        |
                    +------------------+

+------------------+                    +------------------+
|   AssetService   |                    | AssignmentService|  :5135
|   asset.db       |                    | assignment.db    |
+------------------+                    +------------------+
       :5209
```

Each service is an independent ASP.NET Core minimal API with:

- Its own project and endpoint module
- Its own Entity Framework Core `DbContext`
- Its own SQLite database and migrations
- Independent startup and deployment boundaries
- JSON request and response bodies

The services currently exchange identifiers through API payloads (`AssetId` and `UserId`). They do not make synchronous HTTP calls to one another, so referential validation across service databases is not currently enforced by the backend.

## Services

| Service | Responsibility | HTTP base URL | Database |
| --- | --- | --- | --- |
| `AssetService` | Create, search, update, and remove assets | `http://localhost:5209` | `asset.db` |
| `AssignmentService` | Record assignments and return assets | `http://localhost:5135` | `assignment.db` |
| `UserService` | Create, update, list, and remove users | `http://localhost:5182` | `user.db` |

## Technology Stack

- .NET 8 and ASP.NET Core minimal APIs
- Entity Framework Core 8
- SQLite for service-local persistence
- Data annotations for request validation
- REST-style HTTP methods and status codes
- Postman for API testing

## Prerequisites

- .NET 8 SDK
- Postman, if you want to repeat the API test workflow

## Run Locally

From the `backend` directory, start each service in a separate terminal:

```bash
cd AssetService
dotnet run --launch-profile http
```

```bash
cd AssignmentService
dotnet run --launch-profile http
```

```bash
cd UserService
dotnet run --launch-profile http
```

The services use these default development addresses:

- Asset API: `http://localhost:5209`
- Assignment API: `http://localhost:5135`
- User API: `http://localhost:5182`

On first startup, each service creates its SQLite database if it does not already exist. The solution file is `backend/AssetLog Microservices.sln` and can also be opened in Visual Studio or VS Code.

## REST API

### AssetService

| Method | Route | Description |
| --- | --- | --- |
| `POST` | `/assets` | Create an asset |
| `GET` | `/assets` | List all assets |
| `GET` | `/assets?search=laptop` | Search assets by name |
| `GET` | `/assets/{id}` | Get one asset |
| `PUT` | `/assets/{id}` | Update an asset status |
| `DELETE` | `/assets/{id}` | Remove an asset |

Example request:

```json
{
  "assetName": "Dell Latitude 5520",
  "serialNumber": "DL-5520-001",
  "status": "Available"
}
```

`assetName` is required and must contain at least two characters. New assets default to `Available` and receive a UTC creation timestamp.

### UserService

| Method | Route | Description |
| --- | --- | --- |
| `POST` | `/users` | Create a user |
| `GET` | `/users` | List all users |
| `GET` | `/users/{id}` | Get one user |
| `PUT` | `/users/{id}` | Update user details |
| `DELETE` | `/users/{id}` | Remove a user |

Example request:

```json
{
  "userName": "Thabo Lubhele",
  "email": "thabo@example.com",
  "department": "IT",
  "role": "Employee"
}
```

The username, email, department, and role are required. Email format is validated by the API.

### AssignmentService

| Method | Route | Description |
| --- | --- | --- |
| `POST` | `/assignments` | Assign an asset to a user |
| `GET` | `/assignments` | List all assignments |
| `GET` | `/assignments/{id}` | Get one assignment |
| `PUT` | `/assignments/{id}/return` | Mark an assignment as returned |
| `DELETE` | `/assignments/{id}` | Remove an assignment |

Example request:

```json
{
  "assetId": 1,
  "userId": 1,
  "status": "Active"
}
```

New assignments default to `Active`. Returning an assignment sets `returnDate` to the current UTC time and changes `status` to `Returned`.

## Postman Test Workflow

The API can be tested as three separate Postman environments or as one collection with these variables:

```text
assetBaseUrl       http://localhost:5209
assignmentBaseUrl  http://localhost:5135
userBaseUrl        http://localhost:5182
```

Recommended request sequence:

1. `POST {{userBaseUrl}}/users` and save the returned user `id`.
2. `POST {{assetBaseUrl}}/assets` and save the returned asset `id`.
3. `POST {{assignmentBaseUrl}}/assignments` using the saved `userId` and `assetId`.
4. `GET {{assignmentBaseUrl}}/assignments/{id}` to verify the assignment.
5. `PUT {{assignmentBaseUrl}}/assignments/{id}/return` to complete the return workflow.
6. Use the `GET`, `PUT`, and `DELETE` requests for each service to verify read, update, and delete behavior.

Expected response patterns include:

- `201 Created` for successful creates
- `200 OK` for successful reads, updates, returns, and deletes
- `400 Bad Request` for invalid request data
- `404 Not Found` when a requested record does not exist

The `.http` files in each service contain the local request starting points. Postman is useful for persisting IDs between requests, grouping the service endpoints into a collection, and checking response status codes and JSON bodies.

## Project Structure

```text
backend/
├── AssetLog Microservices.sln
├── AssetService/
│   ├── Data/             # AssetDbContext
│   ├── Endpoints/        # Asset routes
│   ├── Migrations/       # EF Core schema history
│   └── Models/           # Asset entity
├── AssignmentService/
│   ├── Data/             # AssignmentDbContext
│   ├── Endpoints/        # Assignment routes
│   ├── Migrations/
│   └── Models/
└── UserService/
    ├── Data/             # UserDbContext
    ├── Endpoints/        # User routes
    ├── Migrations/
    └── Models/
```

## Monolith-to-Microservices Migration

The original monolithic approach would keep users, assets, assignments, persistence, and routes in one deployable application. This implementation separates those responsibilities into bounded services:

- A change to user management can be built and deployed without rebuilding the asset API.
- Each service owns its data and persistence lifecycle.
- Failures and scaling needs can be isolated to the affected service.
- The API surface is easier to test by domain using Postman.

The current design is intentionally lightweight for a development project. Production hardening would typically add authentication and authorization, centralized configuration, structured logging, health checks, service-to-service validation, API versioning, and a gateway or service discovery layer.