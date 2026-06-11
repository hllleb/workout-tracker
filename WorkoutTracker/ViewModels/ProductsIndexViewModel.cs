namespace WorkoutTracker.ViewModels;

/// <summary>
/// View model for the admin products index page.
/// </summary>
public class ProductsIndexViewModel
{
    /// <summary>
    /// Gets or sets the name search term.
    /// </summary>
    public string? Search { get; set; }

    /// <summary>
    /// Gets or sets the source filter.
    /// </summary>
    public string? Source { get; set; }

    /// <summary>
    /// Gets or sets the filtered products.
    /// </summary>
    public IReadOnlyList<ProductListItemViewModel> Products { get; set; } = Array.Empty<ProductListItemViewModel>();

    /// <summary>
    /// Gets or sets available product sources for filtering.
    /// </summary>
    public IReadOnlyList<string> AvailableSources { get; set; } = Array.Empty<string>();
}

/// <summary>
/// Represents a product row on the admin list.
/// </summary>
public class ProductListItemViewModel
{
    /// <summary>
    /// Gets or sets the product identifier.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the external API identifier.
    /// </summary>
    public string ExternalId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the data source.
    /// </summary>
    public string Source { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the product name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets calories per serving.
    /// </summary>
    public int Calories { get; set; }

    /// <summary>
    /// Gets or sets when the product was cached.
    /// </summary>
    public DateTime CachedAt { get; set; }

    /// <summary>
    /// Gets or sets how many meal items reference this product.
    /// </summary>
    public int MealItemCount { get; set; }
}
