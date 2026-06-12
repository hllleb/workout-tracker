using System.ComponentModel.DataAnnotations;

namespace WorkoutTracker.ViewModels;

/// <summary>
/// View model for creating or editing a meal.
/// </summary>
public class MealEditViewModel
{
    /// <summary>
    /// Gets or sets the meal identifier.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the meal name.
    /// </summary>
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets when the meal was consumed.
    /// </summary>
    [DataType(DataType.DateTime)]
    public DateTime ConsumedAt { get; set; } = DateTime.Now;

    /// <summary>
    /// Gets or sets optional notes about the meal.
    /// </summary>
    [StringLength(500)]
    public string? Notes { get; set; }
}
