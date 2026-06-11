using Microsoft.EntityFrameworkCore;
using WorkoutTracker.Data;
using WorkoutTracker.Models;
using WorkoutTracker.Services.FatSecret;

namespace WorkoutTracker.Services.Products;

/// <summary>
/// Persists and refreshes cached FatSecret products.
/// </summary>
public class ProductCacheService : IProductCacheService
{
    private const string FatSecretSource = "FatSecret";

    private readonly ApplicationDbContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="ProductCacheService"/> class.
    /// </summary>
    /// <param name="context">Application database context.</param>
    public ProductCacheService(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc />
    public async Task<Product> GetOrCreateFromFatSecretAsync(
        FatSecretFoodDetails food,
        FatSecretServing serving,
        CancellationToken cancellationToken = default)
    {
        var externalId = BuildExternalId(food.FoodId, serving.ServingId);
        var product = await _context.Products
            .FirstOrDefaultAsync(
                p => p.ExternalId == externalId && p.Source == FatSecretSource,
                cancellationToken);

        if (product is null)
        {
            product = new Product
            {
                ExternalId = externalId,
                Source = FatSecretSource,
                Name = food.Name,
                ServingSize = serving.MetricServingAmount,
                ServingUnit = serving.MetricServingUnit ?? serving.Description,
                Calories = serving.Calories ?? 0,
                ProteinG = serving.ProteinG,
                CarbsG = serving.CarbsG,
                FatG = serving.FatG,
                CachedAt = DateTime.UtcNow
            };

            _context.Products.Add(product);
        }
        else
        {
            product.Name = food.Name;
            product.ServingSize = serving.MetricServingAmount ?? product.ServingSize;
            product.ServingUnit = serving.MetricServingUnit ?? serving.Description ?? product.ServingUnit;
            product.Calories = serving.Calories ?? product.Calories;
            product.ProteinG = serving.ProteinG ?? product.ProteinG;
            product.CarbsG = serving.CarbsG ?? product.CarbsG;
            product.FatG = serving.FatG ?? product.FatG;
            product.CachedAt = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync(cancellationToken);
        return product;
    }

    internal static string BuildExternalId(string foodId, string? servingId)
    {
        return string.IsNullOrWhiteSpace(servingId) ? foodId : $"{foodId}:{servingId}";
    }
}
