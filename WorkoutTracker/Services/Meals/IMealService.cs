using WorkoutTracker.Models;

namespace WorkoutTracker.Services.Meals;

/// <summary>
/// Provides meal queries scoped to a user.
/// </summary>
public interface IMealService
{
    /// <summary>
    /// Gets meals for a user with optional filters.
    /// </summary>
    /// <param name="userId">Owner user identifier.</param>
    /// <param name="search">Optional name search term.</param>
    /// <param name="fromDate">Optional earliest consumed date.</param>
    /// <param name="toDate">Optional latest consumed date.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Filtered meals with items included.</returns>
    Task<IReadOnlyList<Meal>> GetUserMealsAsync(
        string userId,
        string? search,
        DateTime? fromDate,
        DateTime? toDate,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a meal owned by the specified user.
    /// </summary>
    /// <param name="userId">Owner user identifier.</param>
    /// <param name="mealId">Meal identifier.</param>
    /// <param name="includeItems">Whether to include meal items.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The meal if found; otherwise null.</returns>
    Task<Meal?> GetUserMealAsync(
        string userId,
        int mealId,
        bool includeItems = false,
        CancellationToken cancellationToken = default);
}
