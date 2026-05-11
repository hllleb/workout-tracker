using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace WorkoutTracker.Models;

/// <summary>
/// Represents a single item within a meal, optionally linked to a product.
/// </summary>
public class MealItem
{
    public int Id { get; set; }

    [Required]
    public int MealId { get; set; }

    public Meal? Meal { get; set; }

    public int? ProductId { get; set; }

    public Product? Product { get; set; }

    [Required]
    [StringLength(150)]
    public string Name { get; set; } = string.Empty;

    [Precision(8, 2)]
    [Range(0.01, 10000)]
    public decimal Quantity { get; set; } = 1;

    [StringLength(50)]
    public string? Unit { get; set; }

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
}
