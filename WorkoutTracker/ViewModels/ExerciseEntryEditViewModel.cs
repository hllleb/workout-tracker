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
    /// Gets or sets the number of sets.
    /// </summary>
    [Range(1, 50)]
    public int Sets { get; set; } = 1;

    /// <summary>
    /// Gets or sets the number of repetitions.
    /// </summary>
    [Range(1, 200)]
    public int Reps { get; set; } = 1;

    /// <summary>
    /// Gets or sets the weight in kilograms.
    /// </summary>
    [Range(0, 1000)]
    public decimal? WeightKg { get; set; }

    /// <summary>
    /// Gets or sets optional notes for the entry.
    /// </summary>
    [StringLength(500)]
    public string? Notes { get; set; }
}
