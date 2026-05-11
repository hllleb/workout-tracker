using System.ComponentModel.DataAnnotations;

namespace WorkoutTracker.Models;

/// <summary>
/// Represents a meal logged by a user.
/// </summary>
public class Meal
{
    public int Id { get; set; }

    [Required]
    public string UserId { get; set; } = string.Empty;

    public ApplicationUser? User { get; set; }

    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    public DateTime ConsumedAt { get; set; } = DateTime.UtcNow;

    [StringLength(500)]
    public string? Notes { get; set; }

    public ICollection<MealItem> Items { get; set; } = new List<MealItem>();
}
