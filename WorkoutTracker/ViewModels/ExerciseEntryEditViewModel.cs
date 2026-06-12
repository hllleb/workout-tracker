using System.ComponentModel.DataAnnotations;

namespace WorkoutTracker.ViewModels;

/// <summary>
/// View model for creating or editing an exercise entry.
/// </summary>
public class ExerciseEntryEditViewModel
{
    /// <summary>
    /// Gets or sets the entry identifier.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the workout identifier.
    /// </summary>
    [Required]
    public int WorkoutId { get; set; }

    /// <summary>
    /// Gets or sets the exercise name.
    /// </summary>
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the exercise type: Strength or Cardio.
    /// </summary>
    public string ExerciseType { get; set; } = "Strength";

    /// <summary>
    /// Gets or sets the number of sets (strength).
    /// </summary>
    [Range(1, 50)]
    public int Sets { get; set; } = 1;

    /// <summary>
    /// Gets or sets per-set repetition counts bound from form fields.
    /// </summary>
    public List<int> RepsPerSetValues { get; set; } = new();

    /// <summary>
    /// Gets or sets the weight in kilograms (strength).
    /// </summary>
    [Range(0, 1000)]
    public decimal? WeightKg { get; set; }

    /// <summary>
    /// Gets or sets duration in minutes (cardio).
    /// </summary>
    [Range(0, 1440)]
    public decimal? DurationMinutes { get; set; }

    /// <summary>
    /// Gets or sets distance in kilometres (cardio).
    /// </summary>
    [Range(0, 1000)]
    public decimal? DistanceKm { get; set; }

    /// <summary>
    /// Gets or sets estimated calories burned (computed by controller).
    /// </summary>
    public int? CaloriesBurned { get; set; }

    /// <summary>
    /// Gets or sets optional notes for the entry.
    /// </summary>
    [StringLength(500)]
    public string? Notes { get; set; }
}
