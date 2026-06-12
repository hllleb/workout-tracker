using System.ComponentModel.DataAnnotations;

namespace WorkoutTracker.Models;

/// <summary>
/// Represents a workout session logged by a user.
/// </summary>
public class Workout
{
    /// <summary>
    /// Gets or sets the unique identifier.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the owning user identifier.
    /// </summary>
    [Required]
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the owning user.
    /// </summary>
    public ApplicationUser? User { get; set; }

    /// <summary>
    /// Gets or sets the workout name.
    /// </summary>
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the date the workout was performed.
    /// </summary>
    public DateTime WorkoutDate { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets or sets optional notes about the workout.
    /// </summary>
    [StringLength(500)]
    public string? Notes { get; set; }

    /// <summary>
    /// Gets or sets the exercise entries recorded in this workout.
    /// </summary>
    public ICollection<ExerciseEntry> ExerciseEntries { get; set; } = new List<ExerciseEntry>();
}
