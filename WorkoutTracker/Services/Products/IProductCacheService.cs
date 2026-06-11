using WorkoutTracker.Models;
using WorkoutTracker.Services.FatSecret;

namespace WorkoutTracker.Services.Products;

/// <summary>
/// Caches external nutrition products in the database.
/// </summary>
public interface IProductCacheService
{
    /// <summary>
    /// Gets an existing cached product or creates one from FatSecret data.
    /// </summary>
    /// <param name="food">FatSecret food details.</param>
    /// <param name="serving">Selected serving data.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The cached product.</returns>
    Task<Product> GetOrCreateFromFatSecretAsync(
        FatSecretFoodDetails food,
        FatSecretServing serving,
        CancellationToken cancellationToken = default);
}
