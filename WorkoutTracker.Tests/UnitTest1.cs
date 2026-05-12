using System.Net;
using System.Text;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WorkoutTracker.Services.FatSecret;

namespace WorkoutTracker.Tests;

/// <summary>
/// Tests for the FatSecret API client parsing.
/// </summary>
public class FatSecretClientTests
{
    /// <summary>
    /// Verifies search responses are parsed into results.
    /// </summary>
    [Fact]
    public async Task SearchFoodsAsync_ReturnsResults()
    {
        var client = CreateClient();

        var results = await client.SearchFoodsAsync("apple");

        Assert.Single(results);
        Assert.Equal("123", results[0].FoodId);
        Assert.Equal("Apple", results[0].Name);
        Assert.Contains("Calories", results[0].Description, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Verifies details responses include serving data.
    /// </summary>
    [Fact]
    public async Task GetFoodAsync_ReturnsDetails()
    {
        var client = CreateClient();

        var details = await client.GetFoodAsync("123");

        Assert.NotNull(details);
        Assert.Equal("123", details!.FoodId);
        Assert.Equal("Apple", details.Name);
        Assert.Single(details.Servings);
        Assert.Equal("1", details.Servings[0].ServingId);
        Assert.Equal(52, details.Servings[0].Calories);
        Assert.Equal(0.3m, details.Servings[0].ProteinG);
    }

    private static IFatSecretClient CreateClient()
    {
        var options = Options.Create(new FatSecretOptions
        {
            ApiBaseUrl = "https://example.test/api",
            TokenUrl = "https://example.test/token",
            ClientId = "client",
            ClientSecret = "secret",
            Scope = "basic"
        });

        var cache = new MemoryCache(new MemoryCacheOptions());
        var logger = LoggerFactory.Create(builder => builder.AddDebug()).CreateLogger<FatSecretClient>();
        var handler = new FakeHttpMessageHandler(options.Value);
        var httpClient = new HttpClient(handler);

        return new FatSecretClient(httpClient, cache, options, logger);
    }

    private sealed class FakeHttpMessageHandler : HttpMessageHandler
    {
        private readonly FatSecretOptions _options;

        public FakeHttpMessageHandler(FatSecretOptions options)
        {
            _options = options;
        }

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            var url = request.RequestUri?.ToString() ?? string.Empty;

            if (url.StartsWith(_options.TokenUrl, StringComparison.OrdinalIgnoreCase))
            {
                return Task.FromResult(BuildResponse(TokenJson()));
            }

            if (url.StartsWith(_options.ApiBaseUrl, StringComparison.OrdinalIgnoreCase))
            {
                var query = QueryHelpers.ParseQuery(request.RequestUri?.Query ?? string.Empty);
                var method = query.TryGetValue("method", out var value) ? value.ToString() : string.Empty;

                if (string.Equals(method, "foods.search", StringComparison.OrdinalIgnoreCase))
                {
                    return Task.FromResult(BuildResponse(SearchJson()));
                }

                if (string.Equals(method, "food.get", StringComparison.OrdinalIgnoreCase))
                {
                    return Task.FromResult(BuildResponse(DetailsJson()));
                }
            }

            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.NotFound));
        }

        private static HttpResponseMessage BuildResponse(string json)
        {
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };
        }

        private static string TokenJson()
        {
            return "{\"access_token\":\"token\",\"expires_in\":3600}";
        }

        private static string SearchJson()
        {
            return "{\"foods\":{\"food\":[{\"food_id\":\"123\",\"food_name\":\"Apple\",\"food_description\":\"Per 100g - Calories: 52kcal\"}]}}";
        }

        private static string DetailsJson()
        {
            return "{\"food\":{\"food_id\":\"123\",\"food_name\":\"Apple\",\"servings\":{\"serving\":{\"serving_id\":\"1\",\"serving_description\":\"100 g\",\"metric_serving_amount\":\"100\",\"metric_serving_unit\":\"g\",\"calories\":\"52\",\"protein\":\"0.3\",\"carbohydrate\":\"14\",\"fat\":\"0.2\"}}}}";
        }
    }
}
