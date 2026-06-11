using WorkoutTracker.Models;
using WorkoutTracker.ViewModels;

namespace WorkoutTracker.Services.Nutrition;

/// <summary>
/// Default implementation for nutrition summary calculations.
/// </summary>
public class NutritionSummaryService : INutritionSummaryService
{
    /// <inheritdoc />
    public IReadOnlyList<DailyNutritionSummaryViewModel> CalculateDailyTotals(IEnumerable<Meal> meals)
    {
        return meals
            .GroupBy(meal => meal.ConsumedAt.Date)
            .Select(group => new DailyNutritionSummaryViewModel
            {
                Date = group.Key,
                Calories = group.Sum(meal => meal.Items.Sum(item => item.Calories)),
                ProteinG = group.Sum(meal => meal.Items.Sum(item => item.ProteinG ?? 0m)),
                CarbsG = group.Sum(meal => meal.Items.Sum(item => item.CarbsG ?? 0m)),
                FatG = group.Sum(meal => meal.Items.Sum(item => item.FatG ?? 0m))
            })
            .OrderByDescending(summary => summary.Date)
            .ToList();
    }

    /// <inheritdoc />
    public int CalculateMealCalories(Meal meal)
    {
        return meal.Items.Sum(item => item.Calories);
    }
}
