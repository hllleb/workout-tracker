using WorkoutTracker.Models;
using WorkoutTracker.Services.Nutrition;

namespace WorkoutTracker.Tests;

/// <summary>
/// Tests for nutrition summary calculations.
/// </summary>
public class NutritionSummaryServiceTests
{
    private readonly NutritionSummaryService _service = new();

    /// <summary>
    /// Verifies empty input returns no daily totals.
    /// </summary>
    [Fact]
    public void CalculateDailyTotals_ReturnsEmpty_WhenNoMeals()
    {
        var totals = _service.CalculateDailyTotals(Array.Empty<Meal>());

        Assert.Empty(totals);
    }

    /// <summary>
    /// Verifies meals on the same day are aggregated.
    /// </summary>
    [Fact]
    public void CalculateDailyTotals_AggregatesMealsOnSameDay()
    {
        var date = new DateTime(2026, 6, 10, 8, 0, 0);
        var meals = new[]
        {
            CreateMeal(date, ("Breakfast", 300, 20m, 30m, 10m)),
            CreateMeal(date.AddHours(6), ("Dinner", 500, 30m, 40m, 15m))
        };

        var totals = _service.CalculateDailyTotals(meals);

        Assert.Single(totals);
        Assert.Equal(date.Date, totals[0].Date);
        Assert.Equal(800, totals[0].Calories);
        Assert.Equal(50m, totals[0].ProteinG);
        Assert.Equal(70m, totals[0].CarbsG);
        Assert.Equal(25m, totals[0].FatG);
    }

    /// <summary>
    /// Verifies null macro values are treated as zero.
    /// </summary>
    [Fact]
    public void CalculateDailyTotals_TreatsNullMacrosAsZero()
    {
        var meal = new Meal
        {
            ConsumedAt = new DateTime(2026, 6, 10),
            Items =
            {
                new MealItem { Calories = 100, ProteinG = null, CarbsG = null, FatG = null }
            }
        };

        var totals = _service.CalculateDailyTotals(new[] { meal });

        Assert.Equal(100, totals[0].Calories);
        Assert.Equal(0m, totals[0].ProteinG);
        Assert.Equal(0m, totals[0].CarbsG);
        Assert.Equal(0m, totals[0].FatG);
    }

    /// <summary>
    /// Verifies meal calorie totals sum item calories.
    /// </summary>
    [Fact]
    public void CalculateMealCalories_SumsItemCalories()
    {
        var meal = new Meal
        {
            Items =
            {
                new MealItem { Calories = 120 },
                new MealItem { Calories = 80 }
            }
        };

        Assert.Equal(200, _service.CalculateMealCalories(meal));
    }

    private static Meal CreateMeal(
        DateTime consumedAt,
        (string Name, int Calories, decimal ProteinG, decimal CarbsG, decimal FatG) item)
    {
        return new Meal
        {
            ConsumedAt = consumedAt,
            Items =
            {
                new MealItem
                {
                    Name = item.Name,
                    Calories = item.Calories,
                    ProteinG = item.ProteinG,
                    CarbsG = item.CarbsG,
                    FatG = item.FatG
                }
            }
        };
    }
}
