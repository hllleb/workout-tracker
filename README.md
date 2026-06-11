# Workout Tracker

An ASP.NET Core MVC application for tracking workouts and meals. The MVP focuses on workouts, exercises, meals, and products with Identity-based authentication and persistence in SQL Server.

## MVP Scope
- Workouts and exercise entries with CRUD.
- Meals with manual entries or optional product search from FatSecret.
- Products cached from FatSecret API.
- Authentication with roles (User, Admin).

## Tech Stack
- .NET 8 LTS
- ASP.NET Core MVC + Razor Pages (Identity UI)
- Entity Framework Core + SQL Server
- Docker
- xUnit

## Architecture (Simple MVC)
- Controllers and Razor views for UI.
- Service layer for domain logic (planned).
- EF Core `ApplicationDbContext` for persistence.

## Configuration
Update the default connection string if needed:
- `ConnectionStrings:DefaultConnection`

FatSecret API (OAuth 2.0):
- `FatSecret:ClientId`
- `FatSecret:ClientSecret`
- `FatSecret:Scope` (optional, default: `basic`)

You can set secrets with user-secrets:

```bash
dotnet user-secrets set "FatSecret:ClientId" "<client-id>" --project WorkoutTracker
dotnet user-secrets set "FatSecret:ClientSecret" "<client-secret>" --project WorkoutTracker
```

Optional admin seed (roles are always created):
- `SeedAdmin:Email`
- `SeedAdmin:Password`
- `SeedAdmin:UserName` (optional)

```bash
dotnet user-secrets set "SeedAdmin:Email" "admin@example.com" --project WorkoutTracker
dotnet user-secrets set "SeedAdmin:Password" "<strong-password>" --project WorkoutTracker
```

## Local Development
1. Ensure SQL Server LocalDB or SQL Server is installed.
2. Update the connection string in `appsettings.json` if needed.
3. Install EF Core tools if you do not have them yet:

```bash
dotnet tool install --global dotnet-ef
```

4. Apply migrations:

```bash
dotnet ef database update
```

5. Run the app:

```bash
dotnet run --project WorkoutTracker
```

## Docker (Local)
1. Copy `.env.example` to `.env` and set your values.
2. Build and run containers:

```bash
docker compose up --build
```

3. Open the app at `http://localhost:8080`.

The app applies EF Core migrations on startup.

## Notes
- XML documentation file generation is enabled.
- Docker and unit tests are set up.

## Requirements Coverage (Checklist)
- C# + ASP.NET UI: covered by MVC + Razor Pages (Identity UI).
- Persistence + EF Core ORM: SQL Server via EF Core.
- Read/write data + collections usage: CRUD + filtering/search on workouts and meals.
- Manual input: forms in MVC views.
- Data visualization: tables/lists plus daily nutrition charts on Meals.
- Auth + roles: Identity with User/Admin.
- External API + JSON: FatSecret integration.
- Validation + exception handling: DataAnnotations + service-level validation.
- Documentation: XML docs enabled; DocFX planned.
- Containerization: Docker.
- Tests: unit tests in xUnit.
- Cloud: optional deployment to Azure planned.