using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace WorkoutTracker.Models;

/// <summary>
/// Represents a product fetched from an external nutrition API.
/// </summary>
public class Product
{
    /// <summary>
    /// Gets or sets the unique identifier.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the external API identifier for the product.
    /// </summary>
    [Required]
    [StringLength(100)]
    public string ExternalId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the data source name (for example FatSecret).
    /// </summary>
    [Required]
    [StringLength(50)]
    public string Source { get; set; } = "FatSecret";

    /// <summary>
    /// Gets or sets the product name.
    /// </summary>
    [Required]
    [StringLength(150)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the serving size amount.
    /// </summary>
    [Precision(8, 2)]
    [Range(0.01, 10000)]
    public decimal? ServingSize { get; set; }

    /// <summary>
    /// Gets or sets the serving size unit.
    /// </summary>
    [StringLength(50)]
    public string? ServingUnit { get; set; }

    /// <summary>
    /// Gets or sets calories per serving.
    /// </summary>
    [Range(0, 10000)]
    public int Calories { get; set; }

    /// <summary>
    /// Gets or sets grams of protein per serving.
    /// </summary>
    [Precision(6, 2)]
    [Range(0, 1000)]
    public decimal? ProteinG { get; set; }

    /// <summary>
    /// Gets or sets grams of carbohydrates per serving.
    /// </summary>
    [Precision(6, 2)]
    [Range(0, 1000)]
    public decimal? CarbsG { get; set; }

    /// <summary>
    /// Gets or sets grams of fat per serving.
    /// </summary>
    [Precision(6, 2)]
    [Range(0, 1000)]
    public decimal? FatG { get; set; }

    /// <summary>
    /// Gets or sets when the product was cached locally.
    /// </summary>
    public DateTime CachedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets or sets meal items that reference this product.
    /// </summary>
    public ICollection<MealItem> MealItems { get; set; } = new List<MealItem>();
}
