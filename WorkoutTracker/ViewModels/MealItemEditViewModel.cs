using System.ComponentModel.DataAnnotations;

namespace WorkoutTracker.ViewModels;

/// <summary>
/// View model for creating or editing a meal item.
/// </summary>
public class MealItemEditViewModel
{
    /// <summary>
    /// Gets or sets the item identifier.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the linked product identifier, if any.
    /// </summary>
    public int? ProductId { get; set; }

    /// <summary>
    /// Gets or sets the meal identifier.
    /// </summary>
    [Required]
    public int MealId { get; set; }

    /// <summary>
    /// Gets or sets the item name.
    /// </summary>
    [Required]
    [StringLength(150)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the quantity consumed.
    /// </summary>
    [Range(0.01, 10000)]
    public decimal Quantity { get; set; } = 1;

    /// <summary>
    /// Gets or sets the unit label (e.g. "1 slice (28g)", "cup", "100g").
    /// </summary>
    [StringLength(100)]
    public string? Unit { get; set; }

    /// <summary>
    /// Gets or sets calories for the item (Quantity × CaloriesPerUnit when from FatSecret).
    /// </summary>
    [Range(0, 10000)]
    public int Calories { get; set; }

    /// <summary>
    /// Gets or sets calories per one serving unit (populated when linked to a FatSecret product).
    /// Used by client-side JS to recalculate Calories when Quantity changes.
    /// </summary>
    public int? CaloriesPerUnit { get; set; }

    /// <summary>
    /// Gets or sets grams of protein per one serving unit.
    /// </summary>
    public decimal? ProteinPerUnit { get; set; }

    /// <summary>
    /// Gets or sets grams of carbs per one serving unit.
    /// </summary>
    public decimal? CarbsPerUnit { get; set; }

    /// <summary>
    /// Gets or sets grams of fat per one serving unit.
    /// </summary>
    public decimal? FatPerUnit { get; set; }

    /// <summary>
    /// Gets or sets grams of protein.
    /// </summary>
    [Range(0, 1000)]
    public decimal? ProteinG { get; set; }

    /// <summary>
    /// Gets or sets grams of carbohydrates.
    /// </summary>
    [Range(0, 1000)]
    public decimal? CarbsG { get; set; }

    /// <summary>
    /// Gets or sets grams of fat.
    /// </summary>
    [Range(0, 1000)]
    public decimal? FatG { get; set; }
}
