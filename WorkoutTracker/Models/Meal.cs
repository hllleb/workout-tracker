using System.ComponentModel.DataAnnotations;

namespace WorkoutTracker.Models;

/// <summary>
/// Represents a meal logged by a user.
/// </summary>
public class Meal
{
    /// <summary>
    /// Gets or sets the unique identifier.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the owning user identifier.
    /// </summary>
    [Required]
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the owning user.
    /// </summary>
    public ApplicationUser? User { get; set; }

    /// <summary>
    /// Gets or sets the meal name.
    /// </summary>
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets when the meal was consumed.
    /// </summary>
    public DateTime ConsumedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets or sets optional notes about the meal.
    /// </summary>
    [StringLength(500)]
    public string? Notes { get; set; }

    /// <summary>
    /// Gets or sets the items that make up this meal.
    /// </summary>
    public ICollection<MealItem> Items { get; set; } = new List<MealItem>();
}
