using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace WorkoutTracker.Models;

/// <summary>
/// Represents a product fetched from an external nutrition API.
/// </summary>
public class Product
{
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string ExternalId { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    public string Source { get; set; } = "FatSecret";

    [Required]
    [StringLength(150)]
    public string Name { get; set; } = string.Empty;

    [Precision(8, 2)]
    [Range(0.01, 10000)]
    public decimal? ServingSize { get; set; }

    [StringLength(50)]
    public string? ServingUnit { get; set; }

    [Range(0, 10000)]
    public int Calories { get; set; }

    [Precision(6, 2)]
    [Range(0, 1000)]
    public decimal? ProteinG { get; set; }

    [Precision(6, 2)]
    [Range(0, 1000)]
    public decimal? CarbsG { get; set; }

    [Precision(6, 2)]
    [Range(0, 1000)]
    public decimal? FatG { get; set; }

    public DateTime CachedAt { get; set; } = DateTime.UtcNow;

    public ICollection<MealItem> MealItems { get; set; } = new List<MealItem>();
}
