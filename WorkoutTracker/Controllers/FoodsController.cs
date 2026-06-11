using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WorkoutTracker.Services.FatSecret;
using WorkoutTracker.Services.Meals;
using WorkoutTracker.ViewModels;

namespace WorkoutTracker.Controllers;

/// <summary>
/// Provides FatSecret search and details pages for meal item creation.
/// </summary>
[Authorize]
public class FoodsController : Controller
{
    private readonly IFatSecretClient _fatSecret;
    private readonly IMealService _mealService;

    /// <summary>
    /// Initializes a new instance of the <see cref="FoodsController"/> class.
    /// </summary>
    /// <param name="fatSecret">FatSecret API client.</param>
    /// <param name="mealService">Meal query service.</param>
    public FoodsController(IFatSecretClient fatSecret, IMealService mealService)
    {
        _fatSecret = fatSecret;
        _mealService = mealService;
    }

    /// <summary>
    /// Displays a food search page for a meal.
    /// </summary>
    /// <param name="mealId">Meal identifier.</param>
    /// <param name="query">Search query.</param>
    /// <returns>The search view.</returns>
    public async Task<IActionResult> Search(int? mealId, string? query)
    {
        if (mealId is null)
        {
            return NotFound();
        }

        var userId = GetUserId();
        if (string.IsNullOrEmpty(userId))
        {
            return Challenge();
        }

        var meal = await GetMealAsync(mealId.Value, userId);
        if (meal is null)
        {
            return NotFound();
        }

        ViewData["MealName"] = meal.Name;

        var results = string.IsNullOrWhiteSpace(query)
            ? Array.Empty<FatSecretFoodSearchResult>()
            : await _fatSecret.SearchFoodsAsync(query, cancellationToken: HttpContext.RequestAborted);

        var model = new FoodSearchViewModel
        {
            MealId = meal.Id,
            Query = query,
            Results = results
        };

        return View(model);
    }

    /// <summary>
    /// Displays food details and servings for selection.
    /// </summary>
    /// <param name="mealId">Meal identifier.</param>
    /// <param name="foodId">FatSecret food identifier.</param>
    /// <returns>The details view.</returns>
    public async Task<IActionResult> Details(int? mealId, string? foodId)
    {
        if (mealId is null || string.IsNullOrWhiteSpace(foodId))
        {
            return NotFound();
        }

        var userId = GetUserId();
        if (string.IsNullOrEmpty(userId))
        {
            return Challenge();
        }

        var meal = await GetMealAsync(mealId.Value, userId);
        if (meal is null)
        {
            return NotFound();
        }

        ViewData["MealName"] = meal.Name;

        var food = await _fatSecret.GetFoodAsync(foodId, HttpContext.RequestAborted);
        var model = new FoodDetailsViewModel
        {
            MealId = meal.Id,
            Food = food
        };

        return View(model);
    }

    private string? GetUserId()
    {
        return User.FindFirstValue(ClaimTypes.NameIdentifier);
    }

    private Task<Models.Meal?> GetMealAsync(int mealId, string userId)
    {
        return _mealService.GetUserMealAsync(userId, mealId, cancellationToken: HttpContext.RequestAborted);
    }
}
