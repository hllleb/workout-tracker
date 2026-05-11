using Microsoft.AspNetCore.Identity;

namespace WorkoutTracker.Models;

/// <summary>
/// Application user with domain data navigation.
/// </summary>
public class ApplicationUser : IdentityUser
{
    public ICollection<Workout> Workouts { get; set; } = new List<Workout>();

    public ICollection<Meal> Meals { get; set; } = new List<Meal>();
}
