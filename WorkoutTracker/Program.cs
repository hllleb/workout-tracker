using System.Globalization;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Localization;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using WorkoutTracker.Data;
using WorkoutTracker.Models;
using WorkoutTracker.Services.Email;
using WorkoutTracker.Services.FatSecret;
using WorkoutTracker.Services.Meals;
using WorkoutTracker.Services.Nutrition;
using WorkoutTracker.Services.Products;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    options.KnownNetworks.Clear();
    options.KnownProxies.Clear();
});

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.Configure<EmailOptions>(builder.Configuration.GetSection(EmailOptions.SectionName));
builder.Services.AddTransient<IEmailSender, SmtpEmailSender>();
builder.Services.AddMemoryCache();
builder.Services.Configure<FatSecretOptions>(builder.Configuration.GetSection(FatSecretOptions.SectionName));
builder.Services.AddHttpClient<IFatSecretClient, FatSecretClient>();
builder.Services.AddScoped<IMealService, MealService>();
builder.Services.AddScoped<INutritionSummaryService, NutritionSummaryService>();
builder.Services.AddScoped<IProductCacheService, ProductCacheService>();
builder.Services.AddControllersWithViews();

var invariantCulture = CultureInfo.GetCultureInfo("en-US");
builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    options.DefaultRequestCulture = new RequestCulture(invariantCulture);
    options.SupportedCultures = [invariantCulture];
    options.SupportedUICultures = [invariantCulture];
});

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseForwardedHeaders();

if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRequestLocalization();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    var migrationLogger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>()
        .CreateLogger("DatabaseMigration");

    await MigrateDatabaseWithRetryAsync(dbContext, migrationLogger);

    var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>()
        .CreateLogger("IdentitySeeder");
    await IdentitySeeder.SeedAsync(scope.ServiceProvider, app.Configuration, logger);
}

app.Run();

static async Task MigrateDatabaseWithRetryAsync(
    ApplicationDbContext dbContext,
    ILogger logger,
    int maxAttempts = 30)
{
    for (var attempt = 1; attempt <= maxAttempts; attempt++)
    {
        try
        {
            await dbContext.Database.MigrateAsync();
            return;
        }
        catch (SqlException ex) when (attempt < maxAttempts && IsTransientSqlError(ex))
        {
            var delay = TimeSpan.FromSeconds(Math.Min(attempt * 2, 10));
            logger.LogWarning(
                ex,
                "Database not ready (attempt {Attempt}/{MaxAttempts}). Retrying in {DelaySeconds}s...",
                attempt,
                maxAttempts,
                delay.TotalSeconds);
            await Task.Delay(delay);
        }
    }
}

static bool IsTransientSqlError(SqlException ex)
{
    // Login failures mean a config/password mismatch and should fail fast.
    if (ex.Number is 18456)
    {
        return false;
    }

    return ex.Number is -2 or 0 or 4060 or 40197 or 40501 or 49918 or 49919 or 49920;
}
