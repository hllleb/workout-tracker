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
    /// Gets or sets the exercise type: Strength or Cardio.
    /// </summary>
    [StringLength(20)]
    public string ExerciseType { get; set; } = "Strength";

    /// <summary>
    /// Gets or sets the number of sets performed (strength exercises).
    /// </summary>
    [Range(1, 50)]
    public int Sets { get; set; } = 1;

    /// <summary>
    /// Gets or sets the average number of repetitions per set.
    /// </summary>
    [Range(0, 200)]
    public int Reps { get; set; } = 1;

    /// <summary>
    /// Gets or sets per-set repetitions as a JSON array, e.g. [10,8,6].
    /// </summary>
    [StringLength(300)]
    public string? RepsPerSet { get; set; }

    /// <summary>
    /// Gets or sets the weight used in kilograms (strength exercises).
    /// </summary>
    [Precision(6, 2)]
    [Range(0, 1000)]
    public decimal? WeightKg { get; set; }

    /// <summary>
    /// Gets or sets duration in minutes (cardio exercises).
    /// </summary>
    [Precision(6, 2)]
    [Range(0, 1440)]
    public decimal? DurationMinutes { get; set; }

    /// <summary>
    /// Gets or sets distance in kilometres (cardio exercises).
    /// </summary>
    [Precision(6, 2)]
    [Range(0, 1000)]
    public decimal? DistanceKm { get; set; }

    /// <summary>
    /// Gets or sets estimated calories burned for this entry.
    /// </summary>
    [Range(0, 10000)]
    public int? CaloriesBurned { get; set; }

    /// <summary>
    /// Gets or sets optional notes for the exercise entry.
    /// </summary>
    [StringLength(500)]
    public string? Notes { get; set; }
}
