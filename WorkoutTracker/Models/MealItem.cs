using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace WorkoutTracker.Models;

/// <summary>
/// Represents a single item within a meal, optionally linked to a product.
/// </summary>
public class MealItem
{
    /// <summary>
    /// Gets or sets the unique identifier.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the parent meal identifier.
    /// </summary>
    [Required]
    public int MealId { get; set; }

    /// <summary>
    /// Gets or sets the parent meal.
    /// </summary>
    public Meal? Meal { get; set; }

    /// <summary>
    /// Gets or sets the linked cached product identifier, if any.
    /// </summary>
    public int? ProductId { get; set; }

    /// <summary>
    /// Gets or sets the linked cached product.
    /// </summary>
    public Product? Product { get; set; }

    /// <summary>
    /// Gets or sets the item name.
    /// </summary>
    [Required]
    [StringLength(150)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the quantity consumed.
    /// </summary>
    [Precision(8, 2)]
    [Range(0.01, 10000)]
    public decimal Quantity { get; set; } = 1;

    /// <summary>
    /// Gets or sets the unit label (for example g, ml, or serving).
    /// </summary>
    [StringLength(50)]
    public string? Unit { get; set; }

    /// <summary>
    /// Gets or sets calories for this item.
    /// </summary>
    [Range(0, 10000)]
    public int Calories { get; set; }

    /// <summary>
    /// Gets or sets grams of protein.
    /// </summary>
    [Precision(6, 2)]
    [Range(0, 1000)]
    public decimal? ProteinG { get; set; }

    /// <summary>
    /// Gets or sets grams of carbohydrates.
    /// </summary>
    [Precision(6, 2)]
    [Range(0, 1000)]
    public decimal? CarbsG { get; set; }

    /// <summary>
    /// Gets or sets grams of fat.
    /// </summary>
    [Precision(6, 2)]
    [Range(0, 1000)]
    public decimal? FatG { get; set; }
}
