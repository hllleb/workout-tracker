namespace WorkoutTracker.ViewModels;

/// <summary>
/// View model for the workouts index page with optional filters.
/// </summary>
public class WorkoutsIndexViewModel
{
    /// <summary>
    /// Gets or sets the name search term.
    /// </summary>
    public string? Search { get; set; }

    /// <summary>
    /// Gets or sets the earliest workout date to include.
    /// </summary>
    public DateTime? FromDate { get; set; }

    /// <summary>
    /// Gets or sets the latest workout date to include.
    /// </summary>
    public DateTime? ToDate { get; set; }

    /// <summary>
    /// Gets or sets the filtered list of workouts.
    /// </summary>
    public IReadOnlyList<Models.Workout> Workouts { get; set; } = Array.Empty<Models.Workout>();
}
