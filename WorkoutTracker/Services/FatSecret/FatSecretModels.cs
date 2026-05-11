namespace WorkoutTracker.Services.FatSecret;

/// <summary>
/// Represents a simplified food search result from FatSecret.
/// </summary>
public sealed class FatSecretFoodSearchResult
{
    /// <summary>
    /// Gets or sets the FatSecret food identifier.
    /// </summary>
    public string FoodId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the food name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the description provided by FatSecret.
    /// </summary>
    public string? Description { get; set; }
}

/// <summary>
/// Represents detailed information about a food.
/// </summary>
public sealed class FatSecretFoodDetails
{
    /// <summary>
    /// Gets or sets the FatSecret food identifier.
    /// </summary>
    public string FoodId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the food name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the available servings.
    /// </summary>
    public IReadOnlyList<FatSecretServing> Servings { get; set; } = Array.Empty<FatSecretServing>();
}

/// <summary>
/// Represents a single serving option for a food.
/// </summary>
public sealed class FatSecretServing
{
    /// <summary>
    /// Gets or sets the serving identifier.
    /// </summary>
    public string? ServingId { get; set; }

    /// <summary>
    /// Gets or sets the serving description.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the metric serving amount.
    /// </summary>
    public decimal? MetricServingAmount { get; set; }

    /// <summary>
    /// Gets or sets the metric serving unit.
    /// </summary>
    public string? MetricServingUnit { get; set; }

    /// <summary>
    /// Gets or sets calories for the serving.
    /// </summary>
    public int? Calories { get; set; }

    /// <summary>
    /// Gets or sets grams of protein.
    /// </summary>
    public decimal? ProteinG { get; set; }

    /// <summary>
    /// Gets or sets grams of carbohydrates.
    /// </summary>
    public decimal? CarbsG { get; set; }

    /// <summary>
    /// Gets or sets grams of fat.
    /// </summary>
    public decimal? FatG { get; set; }
}
