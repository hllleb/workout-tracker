# Workout Tracker

An ASP.NET Core MVC application for tracking workouts and meals. The MVP focuses on workouts, exercises, meals, and products with Identity-based authentication and persistence in SQL Server.

### Deployed application can be accessed at: https://workout-tracker-eragdqbsbkbmhdb3.germanywestcentral-01.azurewebsites.net/

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
- Service layer for domain logic (meals, nutrition summaries, product cache).
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

Email (SMTP, required for registration confirmation):
- `Email:SmtpHost`
- `Email:SmtpPort` (default: 587)
- `Email:SmtpUsername`
- `Email:SmtpPassword`
- `Email:FromAddress`
- `Email:FromName` (optional)

SendGrid SMTP example:
- Host: `smtp.sendgrid.net`
- Port: `587`
- Username: `apikey`
- Password: your SendGrid API key
- FromAddress: a verified sender address

```bash
dotnet user-secrets set "Email:SmtpHost" "smtp.sendgrid.net" --project WorkoutTracker
dotnet user-secrets set "Email:SmtpUsername" "apikey" --project WorkoutTracker
dotnet user-secrets set "Email:SmtpPassword" "<api-key>" --project WorkoutTracker
dotnet user-secrets set "Email:FromAddress" "noreply@yourdomain.com" --project WorkoutTracker
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

If you see `Login failed for user 'sa'`, the SQL Server volume was likely created with a different password. Reset containers and the database volume, then start again:

```bash
docker compose down -v
docker compose up --build
```

## Documentation (DocFX)

XML comments are enabled on public APIs. Generate the documentation site:

```bash
dotnet tool install -g docfx
docfx metadata docfx/docfx.json
docfx build docfx/docfx.json
```

Open `docfx/_site/index.html` in a browser.

## CI/CD (GitHub Actions → Azure)

The workflow in `.github/workflows/ci-cd.yml` runs on every push to `main`:
1. Restore, build, and run tests
2. Publish the app
3. Deploy to Azure App Service (production environment)

### One-time Azure setup

1. Create an **Azure App Service** (Linux, .NET 8) and **Azure SQL Database**.
2. In App Service → **Configuration**, set application settings:
   - `ConnectionStrings__DefaultConnection`
   - `Email__SmtpHost`, `Email__SmtpPort`, `Email__SmtpUsername`, `Email__SmtpPassword`, `Email__FromAddress`
   - `FatSecret__ClientId`, `FatSecret__ClientSecret`
   - `SeedAdmin__Email`, `SeedAdmin__Password` (optional)
   - `ASPNETCORE_ENVIRONMENT` = `Production`
3. Download the **publish profile** from App Service → Overview → Download publish profile.
4. In GitHub → **Settings → Secrets and variables → Actions**, add:
   - `AZURE_WEBAPP_NAME` — App Service name (e.g. `workout-tracker-app`)
   - `AZURE_WEBAPP_PUBLISH_PROFILE` — full contents of the publish profile XML
5. In GitHub → **Settings → Environments**, create a `production` environment (optional approval gate).

Push to `main` to trigger build, test, and deploy.

## Notes
- XML documentation file generation is enabled.
- Docker and unit tests are set up.
- EF Core migrations run automatically on app startup (local Docker and Azure).

## Requirements Coverage (Checklist)
- C# + ASP.NET UI: covered by MVC + Razor Pages (Identity UI).
- Persistence + EF Core ORM: SQL Server via EF Core.
- Read/write data + collections usage: CRUD + filtering/search on workouts and meals.
- Manual input: forms in MVC views.
- Data visualization: tables/lists plus daily nutrition charts on Meals.
- Auth + roles: Identity with User/Admin; admin product management panel.
- External API + JSON: FatSecret integration.
- Validation + exception handling: DataAnnotations + domain services.
- Documentation: XML docs + DocFX site in `docfx/`.
- Containerization: Docker.
- Tests: unit tests in xUnit.
- Cloud: Azure App Service deployment via GitHub Actions CI/CD.
