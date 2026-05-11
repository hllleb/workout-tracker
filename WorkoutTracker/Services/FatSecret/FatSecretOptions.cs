namespace WorkoutTracker.Services.FatSecret;

/// <summary>
/// Configuration options for the FatSecret API integration.
/// </summary>
public class FatSecretOptions
{
    /// <summary>
    /// Gets the configuration section name.
    /// </summary>
    public const string SectionName = "FatSecret";

    /// <summary>
    /// Gets or sets the API base URL for FatSecret REST endpoints.
    /// </summary>
    public string ApiBaseUrl { get; set; } = "https://platform.fatsecret.com/rest/server.api";

    /// <summary>
    /// Gets or sets the OAuth 2.0 token endpoint URL.
    /// </summary>
    public string TokenUrl { get; set; } = "https://oauth.fatsecret.com/connect/token";

    /// <summary>
    /// Gets or sets the OAuth 2.0 scope.
    /// </summary>
    public string Scope { get; set; } = "basic";

    /// <summary>
    /// Gets or sets the OAuth 2.0 client identifier.
    /// </summary>
    public string? ClientId { get; set; }

    /// <summary>
    /// Gets or sets the OAuth 2.0 client secret.
    /// </summary>
    public string? ClientSecret { get; set; }
}
