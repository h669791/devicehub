# DeviceHub

A backend for managing and monitoring VR headsets. Inspired by my work on a VR dashboard for Helse Vest IKT, but written from scratch on my own time, using simulated data only.

The goal is a small but realistic web backend: a REST API, a relational database, and later live status updates and a simulator that behaves like real headsets.

`C#` `.NET 10` `ASP.NET Core Web API` `Entity Framework Core` `PostgreSQL` `Docker`

## API

| Method | Route | What it does |
| --- | --- | --- |
| `GET` | `/api/headsets` | Lists all headsets |
| `GET` | `/api/headsets/{id}` | Gets one headset, or `404 Not Found` |
| `POST` | `/api/headsets` | Registers a headset and returns `201 Created` with a `Location` header |

Register a headset:

```http
POST /api/headsets
Content-Type: application/json

{
  "name": "Quest 3 #1",
  "serialNumber": "Q3-0001"
}
```

Response:

```json
{
  "id": 1,
  "name": "Quest 3 #1",
  "serialNumber": "Q3-0001",
  "status": "Offline",
  "batteryLevel": 0,
  "lastSeenAt": null
}
```

## Design decisions

- **DTOs separate the API from the database.** Clients only send a name and a serial number. Id, status, battery level and last-seen time are owned by the server.
- **Serial numbers are unique in the database**, enforced by a unique index.
- **Status is a string, not a number**, both in the JSON and in the database. The data stays readable, and reordering the enum can't silently change stored values.
- **PostgreSQL naming conventions.** Tables and columns use snake_case.
- **No secrets in the repository.** The connection string is read from configuration, set locally with user secrets.

## Running it locally

Requires the .NET 10 SDK, Docker and the EF Core tools (`dotnet tool install --global dotnet-ef`).

1. Start PostgreSQL:

   ```bash
   docker compose -f DeviceHub.Api/docker-compose.yml up -d
   ```

2. Set the connection string (matches the development database in `docker-compose.yml`):

   ```bash
   dotnet user-secrets set "ConnectionStrings:DeviceHub" "Host=localhost;Port=5432;Database=devicehub;Username=devicehub;Password=devpassord" --project DeviceHub.Api
   ```

3. Create the database schema:

   ```bash
   dotnet ef database update --project DeviceHub.Api
   ```

4. Run the API:

   ```bash
   dotnet run --project DeviceHub.Api
   ```

The API runs on http://localhost:5164. Example requests are in `DeviceHub.Api/DeviceHub.Api.http`, and the OpenAPI document is at `/openapi/v1.json` in development.

## Project structure

```
DeviceHub.Api/
  Controllers/   HeadsetsController
  Data/          AppDbContext (EF Core)
  Dtos/          API contracts: HeadsetDto, CreateHeadsetDto
  Models/        Headset entity and HeadsetStatus
  Migrations/    EF Core migrations
  docker-compose.yml   PostgreSQL for development
```

## Roadmap

- [ ] Update and delete endpoints
- [ ] `409 Conflict` when a serial number is already registered
- [ ] Sessions and experiences with business rules (e.g. a headset must be online to start a session)
- [ ] Live status updates with SignalR and a React/TypeScript frontend
- [ ] xUnit tests and CI with GitHub Actions
- [ ] A headset simulator in Kotlin/Spring Boot that sends status messages to the API through a message queue
