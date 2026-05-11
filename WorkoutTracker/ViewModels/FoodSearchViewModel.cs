using WorkoutTracker.Services.FatSecret;

namespace WorkoutTracker.ViewModels;

/// <summary>
/// View model for FatSecret food search results.
/// </summary>
public class FoodSearchViewModel
{
    /// <summary>
    /// Gets or sets the meal identifier.
    /// </summary>
    public int MealId { get; set; }

    /// <summary>
    /// Gets or sets the search query.
    /// </summary>
    public string? Query { get; set; }

    /// <summary>
    /// Gets or sets the search results.
    /// </summary>
    public IReadOnlyList<FatSecretFoodSearchResult> Results { get; set; } = Array.Empty<FatSecretFoodSearchResult>();
}
