using System.Globalization;
using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace WorkoutTracker.Services.FatSecret;

/// <summary>
/// FatSecret API client using OAuth 2.0 client credentials flow.
/// </summary>
public sealed class FatSecretClient : IFatSecretClient
{
    private const string TokenCacheKey = "fatsecret:token";
    private static readonly TimeSpan SearchCacheDuration = TimeSpan.FromMinutes(5);
    private static readonly TimeSpan DetailsCacheDuration = TimeSpan.FromMinutes(15);

    private readonly HttpClient _httpClient;
    private readonly IMemoryCache _cache;
    private readonly FatSecretOptions _options;
    private readonly ILogger<FatSecretClient> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="FatSecretClient"/> class.
    /// </summary>
    /// <param name="httpClient">HTTP client instance.</param>
    /// <param name="cache">Memory cache for tokens and responses.</param>
    /// <param name="options">FatSecret configuration options.</param>
    /// <param name="logger">Logger instance.</param>
    public FatSecretClient(
        HttpClient httpClient,
        IMemoryCache cache,
        IOptions<FatSecretOptions> options,
        ILogger<FatSecretClient> logger)
    {
        _httpClient = httpClient;
        _cache = cache;
        _options = options.Value;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<FatSecretFoodSearchResult>> SearchFoodsAsync(
        string query,
        int pageNumber = 0,
        int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            return Array.Empty<FatSecretFoodSearchResult>();
        }

        var cacheKey = $"fatsecret:search:{query}:{pageNumber}:{pageSize}";
        if (_cache.TryGetValue(cacheKey, out IReadOnlyList<FatSecretFoodSearchResult> cached))
        {
            return cached;
        }

        var parameters = new Dictionary<string, string?>
        {
            ["method"] = "foods.search",
            ["search_expression"] = query,
            ["page_number"] = pageNumber.ToString(CultureInfo.InvariantCulture),
            ["max_results"] = pageSize.ToString(CultureInfo.InvariantCulture),
            ["format"] = "json"
        };

        using var json = await SendRequestAsync(parameters, cancellationToken);
        var results = ParseSearchResults(json);

        _cache.Set(cacheKey, results, SearchCacheDuration);
        return results;
    }

    /// <inheritdoc />
    public async Task<FatSecretFoodDetails?> GetFoodAsync(string foodId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(foodId))
        {
            return null;
        }

        var cacheKey = $"fatsecret:food:{foodId}";
        if (_cache.TryGetValue(cacheKey, out FatSecretFoodDetails cached))
        {
            return cached;
        }

        var parameters = new Dictionary<string, string?>
        {
            ["method"] = "food.get",
            ["food_id"] = foodId,
            ["format"] = "json"
        };

        using var json = await SendRequestAsync(parameters, cancellationToken);
        var details = ParseFoodDetails(json);

        if (details is not null)
        {
            _cache.Set(cacheKey, details, DetailsCacheDuration);
        }

        return details;
    }

    private async Task<JsonDocument> SendRequestAsync(
        IReadOnlyDictionary<string, string?> parameters,
        CancellationToken cancellationToken)
    {
        var token = await GetAccessTokenAsync(cancellationToken);
        var url = QueryHelpers.AddQueryString(_options.ApiBaseUrl, parameters);

        using var request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        using var response = await _httpClient.SendAsync(request, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            var payload = await response.Content.ReadAsStringAsync(cancellationToken);
            _logger.LogWarning("FatSecret API error: {StatusCode} {Payload}", response.StatusCode, payload);
            response.EnsureSuccessStatusCode();
        }

        var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
        return await JsonDocument.ParseAsync(stream, cancellationToken: cancellationToken);
    }

    private async Task<string> GetAccessTokenAsync(CancellationToken cancellationToken)
    {
        if (_cache.TryGetValue(TokenCacheKey, out string cachedToken))
        {
            return cachedToken;
        }

        if (string.IsNullOrWhiteSpace(_options.ClientId) || string.IsNullOrWhiteSpace(_options.ClientSecret))
        {
            throw new InvalidOperationException("FatSecret client credentials are missing.");
        }

        using var request = new HttpRequestMessage(HttpMethod.Post, _options.TokenUrl)
        {
            Content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["grant_type"] = "client_credentials",
                ["client_id"] = _options.ClientId,
                ["client_secret"] = _options.ClientSecret,
                ["scope"] = _options.Scope
            })
        };

        using var response = await _httpClient.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();

        await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
        using var json = await JsonDocument.ParseAsync(stream, cancellationToken: cancellationToken);

        if (!json.RootElement.TryGetProperty("access_token", out var tokenElement))
        {
            throw new InvalidOperationException("FatSecret token response missing access_token.");
        }

        var token = tokenElement.GetString();
        if (string.IsNullOrWhiteSpace(token))
        {
            throw new InvalidOperationException("FatSecret access token is empty.");
        }

        var expiresInSeconds = json.RootElement.TryGetProperty("expires_in", out var expiresElement)
            ? expiresElement.GetInt32()
            : 3600;

        var cacheDuration = TimeSpan.FromSeconds(Math.Max(expiresInSeconds - 60, 60));
        _cache.Set(TokenCacheKey, token, cacheDuration);

        return token;
    }

    private static IReadOnlyList<FatSecretFoodSearchResult> ParseSearchResults(JsonDocument json)
    {
        if (!json.RootElement.TryGetProperty("foods", out var foodsElement))
        {
            return Array.Empty<FatSecretFoodSearchResult>();
        }

        if (!foodsElement.TryGetProperty("food", out var foodElement))
        {
            return Array.Empty<FatSecretFoodSearchResult>();
        }

        var results = new List<FatSecretFoodSearchResult>();

        if (foodElement.ValueKind == JsonValueKind.Array)
        {
            foreach (var element in foodElement.EnumerateArray())
            {
                var result = ParseSearchItem(element);
                if (result is not null)
                {
                    results.Add(result);
                }
            }
        }
        else if (foodElement.ValueKind == JsonValueKind.Object)
        {
            var result = ParseSearchItem(foodElement);
            if (result is not null)
            {
                results.Add(result);
            }
        }

        return results;
    }

    private static FatSecretFoodDetails? ParseFoodDetails(JsonDocument json)
    {
        if (!json.RootElement.TryGetProperty("food", out var foodElement))
        {
            return null;
        }

        var details = new FatSecretFoodDetails
        {
            FoodId = GetString(foodElement, "food_id") ?? string.Empty,
            Name = GetString(foodElement, "food_name") ?? string.Empty
        };

        if (foodElement.TryGetProperty("servings", out var servingsElement)
            && servingsElement.TryGetProperty("serving", out var servingElement))
        {
            var servings = new List<FatSecretServing>();

            if (servingElement.ValueKind == JsonValueKind.Array)
            {
                foreach (var element in servingElement.EnumerateArray())
                {
                    var serving = ParseServing(element);
                    if (serving is not null)
                    {
                        servings.Add(serving);
                    }
                }
            }
            else if (servingElement.ValueKind == JsonValueKind.Object)
            {
                var serving = ParseServing(servingElement);
                if (serving is not null)
                {
                    servings.Add(serving);
                }
            }

            details.Servings = servings;
        }

        return details;
    }

    private static FatSecretFoodSearchResult? ParseSearchItem(JsonElement element)
    {
        var foodId = GetString(element, "food_id");
        var name = GetString(element, "food_name");

        if (string.IsNullOrWhiteSpace(foodId) || string.IsNullOrWhiteSpace(name))
        {
            return null;
        }

        return new FatSecretFoodSearchResult
        {
            FoodId = foodId,
            Name = name,
            Description = GetString(element, "food_description")
        };
    }

    private static FatSecretServing? ParseServing(JsonElement element)
    {
        return new FatSecretServing
        {
            ServingId = GetString(element, "serving_id"),
            Description = GetString(element, "serving_description"),
            MetricServingAmount = GetDecimal(element, "metric_serving_amount"),
            MetricServingUnit = GetString(element, "metric_serving_unit"),
            Calories = GetInt(element, "calories"),
            ProteinG = GetDecimal(element, "protein"),
            CarbsG = GetDecimal(element, "carbohydrate"),
            FatG = GetDecimal(element, "fat")
        };
    }

    private static string? GetString(JsonElement element, string propertyName)
    {
        return element.TryGetProperty(propertyName, out var property)
            ? property.GetString()
            : null;
    }

    private static decimal? GetDecimal(JsonElement element, string propertyName)
    {
        if (!element.TryGetProperty(propertyName, out var property))
        {
            return null;
        }

        if (property.ValueKind == JsonValueKind.Number && property.TryGetDecimal(out var number))
        {
            return number;
        }

        if (property.ValueKind == JsonValueKind.String
            && decimal.TryParse(property.GetString(), NumberStyles.Any, CultureInfo.InvariantCulture, out var parsed))
        {
            return parsed;
        }

        return null;
    }

    private static int? GetInt(JsonElement element, string propertyName)
    {
        if (!element.TryGetProperty(propertyName, out var property))
        {
            return null;
        }

        if (property.ValueKind == JsonValueKind.Number && property.TryGetInt32(out var number))
        {
            return number;
        }

        if (property.ValueKind == JsonValueKind.String
            && int.TryParse(property.GetString(), NumberStyles.Any, CultureInfo.InvariantCulture, out var parsed))
        {
            return parsed;
        }

        return null;
    }
}
