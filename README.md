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
- Docker (planned)
- xUnit/NUnit tests (planned)

## Architecture (Simple MVC)
- Controllers and Razor views for UI.
- Service layer for domain logic (planned).
- EF Core `ApplicationDbContext` for persistence.

## Configuration
Update the default connection string if needed:
- `ConnectionStrings:DefaultConnection`

FatSecret API (planned):
- `FATSECRET_CLIENT_ID`
- `FATSECRET_CLIENT_SECRET`

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

## Notes
- XML documentation file generation is enabled.
- Docker, tests, and API integration are planned next steps.

## Requirements Coverage (Checklist)
- C# + ASP.NET UI: covered by MVC + Razor Pages (Identity UI).
- Persistence + EF Core ORM: SQL Server via EF Core.
- Read/write data + collections usage: CRUD + filtering/search (planned).
- Manual input: forms in MVC views (planned).
- Data visualization: tables/lists in MVP; charts planned after MVP.
- Auth + roles: Identity with User/Admin (planned).
- External API + JSON: FatSecret integration (planned).
- Validation + exception handling: DataAnnotations + service-level validation (planned).
- Documentation: XML docs enabled; DocFX planned.
- Containerization: Docker planned.
- Tests: unit tests planned (xUnit/NUnit).
- Cloud: optional deployment to Azure planned.