using WorkoutTracker.Models;
using WorkoutTracker.ViewModels;

namespace WorkoutTracker.Services.Nutrition;

/// <summary>
/// Calculates nutrition summaries from meal data.
/// </summary>
public interface INutritionSummaryService
{
    /// <summary>
    /// Calculates daily nutrition totals grouped by consumed date.
    /// </summary>
    /// <param name="meals">Meals with items loaded.</param>
    /// <returns>Daily totals ordered from newest to oldest.</returns>
    IReadOnlyList<DailyNutritionSummaryViewModel> CalculateDailyTotals(IEnumerable<Meal> meals);

    /// <summary>
    /// Calculates total calories for a single meal.
    /// </summary>
    /// <param name="meal">Meal with items loaded.</param>
    /// <returns>Total calories across all items.</returns>
    int CalculateMealCalories(Meal meal);
}
