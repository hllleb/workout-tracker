using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace WorkoutTracker.Models;

/// <summary>
/// Represents a single exercise entry within a workout.
/// </summary>
public class ExerciseEntry
{
    public int Id { get; set; }

    [Required]
    public int WorkoutId { get; set; }

    public Workout? Workout { get; set; }

    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [Range(1, 50)]
    public int Sets { get; set; } = 1;

    [Range(1, 200)]
    public int Reps { get; set; } = 1;

    [Precision(6, 2)]
    [Range(0, 1000)]
    public decimal? WeightKg { get; set; }

    [StringLength(500)]
    public string? Notes { get; set; }
}
