using WorkoutTracker.Services.FatSecret;

namespace WorkoutTracker.ViewModels;

/// <summary>
/// View model for FatSecret food details.
/// </summary>
public class FoodDetailsViewModel
{
    /// <summary>
    /// Gets or sets the meal identifier.
    /// </summary>
    public int MealId { get; set; }

    /// <summary>
    /// Gets or sets the food details.
    /// </summary>
    public FatSecretFoodDetails? Food { get; set; }
}
