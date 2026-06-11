using System.ComponentModel.DataAnnotations;

namespace WorkoutTracker.ViewModels;

/// <summary>
/// View model for editing a cached product.
/// </summary>
public class ProductEditViewModel
{
    /// <summary>
    /// Gets or sets the product identifier.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the external API identifier (read-only).
    /// </summary>
    [Display(Name = "External ID")]
    public string ExternalId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the data source (read-only).
    /// </summary>
    public string Source { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the product name.
    /// </summary>
    [Required]
    [StringLength(150)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the serving size.
    /// </summary>
    [Display(Name = "Serving size")]
    [Range(0.01, 10000)]
    public decimal? ServingSize { get; set; }

    /// <summary>
    /// Gets or sets the serving unit.
    /// </summary>
    [Display(Name = "Serving unit")]
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
    [Display(Name = "Protein (g)")]
    [Range(0, 1000)]
    public decimal? ProteinG { get; set; }

    /// <summary>
    /// Gets or sets grams of carbohydrates per serving.
    /// </summary>
    [Display(Name = "Carbs (g)")]
    [Range(0, 1000)]
    public decimal? CarbsG { get; set; }

    /// <summary>
    /// Gets or sets grams of fat per serving.
    /// </summary>
    [Display(Name = "Fat (g)")]
    [Range(0, 1000)]
    public decimal? FatG { get; set; }

    /// <summary>
    /// Gets or sets when the product was cached.
    /// </summary>
    [Display(Name = "Cached at")]
    public DateTime CachedAt { get; set; }
}
