namespace WorkoutTracker.ViewModels;

/// <summary>
/// View model for the meals index page.
/// </summary>
public class MealsIndexViewModel
{
    /// <summary>
    /// Gets or sets the list of meals.
    /// </summary>
    public IReadOnlyList<Models.Meal> Meals { get; set; } = Array.Empty<Models.Meal>();

    /// <summary>
    /// Gets or sets daily nutrition totals.
    /// </summary>
    public IReadOnlyList<DailyNutritionSummaryViewModel> DailyTotals { get; set; } = Array.Empty<DailyNutritionSummaryViewModel>();
}

/// <summary>
/// Represents daily nutrition totals.
/// </summary>
public class DailyNutritionSummaryViewModel
{
    /// <summary>
    /// Gets or sets the date for the summary.
    /// </summary>
    public DateTime Date { get; set; }

    /// <summary>
    /// Gets or sets total calories for the day.
    /// </summary>
    public int Calories { get; set; }

    /// <summary>
    /// Gets or sets total grams of protein.
    /// </summary>
    public decimal ProteinG { get; set; }

    /// <summary>
    /// Gets or sets total grams of carbohydrates.
    /// </summary>
    public decimal CarbsG { get; set; }

    /// <summary>
    /// Gets or sets total grams of fat.
    /// </summary>
    public decimal FatG { get; set; }
}
