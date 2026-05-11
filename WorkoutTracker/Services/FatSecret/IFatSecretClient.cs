namespace WorkoutTracker.Services.FatSecret;

/// <summary>
/// Provides access to FatSecret food search and details endpoints.
/// </summary>
public interface IFatSecretClient
{
    /// <summary>
    /// Searches for foods by name.
    /// </summary>
    /// <param name="query">Search phrase.</param>
    /// <param name="pageNumber">Zero-based page number.</param>
    /// <param name="pageSize">Maximum results per page.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Matching foods.</returns>
    Task<IReadOnlyList<FatSecretFoodSearchResult>> SearchFoodsAsync(
        string query,
        int pageNumber = 0,
        int pageSize = 20,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets detailed information for a single food.
    /// </summary>
    /// <param name="foodId">Food identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Food details or null if not found.</returns>
    Task<FatSecretFoodDetails?> GetFoodAsync(string foodId, CancellationToken cancellationToken = default);
}
