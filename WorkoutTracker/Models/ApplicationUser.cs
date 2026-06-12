using Microsoft.AspNetCore.Identity;

namespace WorkoutTracker.Models;

/// <summary>
/// Application user with domain data navigation.
/// </summary>
public class ApplicationUser : IdentityUser
{
    /// <summary>
    /// Gets or sets workouts logged by this user.
    /// </summary>
    public ICollection<Workout> Workouts { get; set; } = new List<Workout>();

    /// <summary>
    /// Gets or sets meals logged by this user.
    /// </summary>
    public ICollection<Meal> Meals { get; set; } = new List<Meal>();
}
