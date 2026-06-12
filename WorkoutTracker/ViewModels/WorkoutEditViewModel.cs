using System.ComponentModel.DataAnnotations;

namespace WorkoutTracker.ViewModels;

/// <summary>
/// View model for creating or editing a workout.
/// </summary>
public class WorkoutEditViewModel
{
    /// <summary>
    /// Gets or sets the workout identifier.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the workout name.
    /// </summary>
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the date the workout was performed.
    /// </summary>
    [DataType(DataType.Date)]
    public DateTime WorkoutDate { get; set; } = DateTime.Today;

    /// <summary>
    /// Gets or sets optional notes about the workout.
    /// </summary>
    [StringLength(500)]
    public string? Notes { get; set; }
}
