using System.ComponentModel.DataAnnotations;

namespace WorkoutTracker.ViewModels;

/// <summary>
/// View model for creating or editing a workout.
/// </summary>
public class WorkoutEditViewModel
{
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [DataType(DataType.Date)]
    public DateTime WorkoutDate { get; set; } = DateTime.Today;

    [StringLength(500)]
    public string? Notes { get; set; }
}
