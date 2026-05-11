using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WorkoutTracker.Models;

namespace WorkoutTracker.Data;

/// <summary>
/// EF Core database context for application and Identity data.
/// </summary>
public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ApplicationDbContext"/> class.
    /// </summary>
    /// <param name="options">Context options configured by DI.</param>
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Workout> Workouts => Set<Workout>();

    public DbSet<ExerciseEntry> ExerciseEntries => Set<ExerciseEntry>();

    public DbSet<Meal> Meals => Set<Meal>();

    public DbSet<MealItem> MealItems => Set<MealItem>();

    public DbSet<Product> Products => Set<Product>();
}
