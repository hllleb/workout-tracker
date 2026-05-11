using System.ComponentModel.DataAnnotations;

namespace WorkoutTracker.ViewModels;

/// <summary>
/// View model for creating or editing a meal.
/// </summary>
public class MealEditViewModel
{
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [DataType(DataType.DateTime)]
    public DateTime ConsumedAt { get; set; } = DateTime.Now;

    [StringLength(500)]
    public string? Notes { get; set; }
}
