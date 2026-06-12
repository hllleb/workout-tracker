using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace WorkoutTracker.Models;

/// <summary>
/// Represents a single exercise entry within a workout.
/// </summary>
public class ExerciseEntry
{
    /// <summary>
    /// Gets or sets the unique identifier.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the parent workout identifier.
    /// </summary>
    [Required]
    public int WorkoutId { get; set; }

    /// <summary>
    /// Gets or sets the parent workout.
    /// </summary>
    public Workout? Workout { get; set; }

    /// <summary>
    /// Gets or sets the exercise name.
    /// </summary>
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the number of sets performed.
    /// </summary>
    [Range(1, 50)]
    public int Sets { get; set; } = 1;

    /// <summary>
    /// Gets or sets the number of repetitions per set.
    /// </summary>
    [Range(1, 200)]
    public int Reps { get; set; } = 1;

    /// <summary>
    /// Gets or sets the weight used in kilograms.
    /// </summary>
    [Precision(6, 2)]
    [Range(0, 1000)]
    public decimal? WeightKg { get; set; }

    /// <summary>
    /// Gets or sets optional notes for the exercise entry.
    /// </summary>
    [StringLength(500)]
    public string? Notes { get; set; }
}
