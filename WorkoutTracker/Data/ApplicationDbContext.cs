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

    /// <summary>
    /// Gets the workouts data set.
    /// </summary>
    public DbSet<Workout> Workouts => Set<Workout>();

    /// <summary>
    /// Gets the exercise entries data set.
    /// </summary>
    public DbSet<ExerciseEntry> ExerciseEntries => Set<ExerciseEntry>();

    /// <summary>
    /// Gets the meals data set.
    /// </summary>
    public DbSet<Meal> Meals => Set<Meal>();

    /// <summary>
    /// Gets the meal items data set.
    /// </summary>
    public DbSet<MealItem> MealItems => Set<MealItem>();

    /// <summary>
    /// Gets the cached products data set.
    /// </summary>
    public DbSet<Product> Products => Set<Product>();
}
