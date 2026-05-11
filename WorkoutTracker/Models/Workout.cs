using System.ComponentModel.DataAnnotations;

namespace WorkoutTracker.Models;

/// <summary>
/// Represents a workout session logged by a user.
/// </summary>
public class Workout
{
    public int Id { get; set; }

    [Required]
    public string UserId { get; set; } = string.Empty;

    public ApplicationUser? User { get; set; }

    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    public DateTime WorkoutDate { get; set; } = DateTime.UtcNow;

    [StringLength(500)]
    public string? Notes { get; set; }

    public ICollection<ExerciseEntry> ExerciseEntries { get; set; } = new List<ExerciseEntry>();
}
