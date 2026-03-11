using System.Text.Json;

namespace LivestockTrading.Workers.SocialMediaPoster.Services;

public class InstagramService : IInstagramService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;
    private readonly ILogger<InstagramService> _logger;

    // Mutable token - updated on refresh
    private string _currentAccessToken;

    private string AccessToken => _currentAccessToken;
    private string UserId => _configuration["Instagram:UserId"]
        ?? Environment.GetEnvironmentVariable("INSTAGRAM_USER_ID") ?? "";
    private string BaseUrl => _configuration["Instagram:BaseUrl"]
        ?? "https://graph.instagram.com/v21.0";

    public InstagramService(
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration,
        ILogger<InstagramService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
        _logger = logger;
        _currentAccessToken = _configuration["Instagram:AccessToken"]
            ?? Environment.GetEnvironmentVariable("INSTAGRAM_ACCESS_TOKEN") ?? "";
    }

    public async Task<string?> PostImageAsync(string imageUrl, string caption)
    {
        try
        {
            var client = _httpClientFactory.CreateClient("Instagram");

            // Step 1: Create media container
            var containerId = await CreateMediaContainer(client, imageUrl, caption);
            if (containerId == null) return null;

            // Step 2: Wait for processing
            await WaitForMediaProcessing(client, containerId);

            // Step 3: Publish the container
            return await PublishMedia(client, containerId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to post image to Instagram. ImageUrl: {ImageUrl}", imageUrl);
            return null;
        }
    }

    public async Task<string?> PostCarouselAsync(List<string> imageUrls, string caption)
    {
        try
        {
            if (imageUrls.Count < 2)
            {
                _logger.LogWarning("Carousel requires at least 2 images, falling back to single post");
                return await PostImageAsync(imageUrls.First(), caption);
            }

            if (imageUrls.Count > 10)
                imageUrls = imageUrls.Take(10).ToList();

            var client = _httpClientFactory.CreateClient("Instagram");

            // Step 1: Create individual item containers
            var childContainerIds = new List<string>();
            foreach (var imageUrl in imageUrls)
            {
                var childId = await CreateCarouselItemContainer(client, imageUrl);
                if (childId != null)
                    childContainerIds.Add(childId);
            }

            if (childContainerIds.Count < 2)
            {
                _logger.LogWarning("Not enough carousel items created, falling back to single post");
                return await PostImageAsync(imageUrls.First(), caption);
            }

            // Step 2: Wait for all items to process
            foreach (var childId in childContainerIds)
                await WaitForMediaProcessing(client, childId);

            // Step 3: Create carousel container
            var carouselId = await CreateCarouselContainer(client, childContainerIds, caption);
            if (carouselId == null) return null;

            // Step 4: Wait for carousel processing
            await WaitForMediaProcessing(client, carouselId);

            // Step 5: Publish
            return await PublishMedia(client, carouselId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to post carousel to Instagram");
            return null;
        }
    }

    public async Task<bool> IsTokenValidAsync()
    {
        try
        {
            var client = _httpClientFactory.CreateClient("Instagram");
            var response = await client.GetAsync($"{BaseUrl}/me?fields=id,username&access_token={AccessToken}");
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    public async Task<int> GetTokenExpiryDaysAsync()
    {
        try
        {
            var client = _httpClientFactory.CreateClient("Instagram");
            var response = await client.GetAsync(
                $"https://graph.instagram.com/access_token?grant_type=ig_exchange_token&client_secret=placeholder&access_token={AccessToken}");

            // Use debug_token endpoint instead
            var debugResponse = await client.GetAsync(
                $"{BaseUrl}/debug_token?input_token={AccessToken}&access_token={AccessToken}");
            var content = await debugResponse.Content.ReadAsStringAsync();

            if (debugResponse.IsSuccessStatusCode)
            {
                var json = JsonSerializer.Deserialize<JsonElement>(content);
                if (json.TryGetProperty("data", out var data) &&
                    data.TryGetProperty("expires_at", out var expiresAt))
                {
                    var expiryUnix = expiresAt.GetInt64();
                    if (expiryUnix == 0) return 999; // Never expires
                    var expiryDate = DateTimeOffset.FromUnixTimeSeconds(expiryUnix);
                    return (int)(expiryDate - DateTimeOffset.UtcNow).TotalDays;
                }
            }

            // Fallback: try token info endpoint
            var infoResponse = await client.GetAsync(
                $"{BaseUrl}/me?fields=id&access_token={AccessToken}");
            return infoResponse.IsSuccessStatusCode ? -1 : -1;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to check token expiry");
            return -1;
        }
    }

    public async Task<bool> RefreshTokenAsync()
    {
        try
        {
            var client = _httpClientFactory.CreateClient("Instagram");
            var response = await client.GetAsync(
                $"https://graph.instagram.com/refresh_access_token?grant_type=ig_refresh_token&access_token={AccessToken}");
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Failed to refresh Instagram token. Status: {Status}, Response: {Response}",
                    response.StatusCode, content);
                return false;
            }

            var json = JsonSerializer.Deserialize<JsonElement>(content);
            if (json.TryGetProperty("access_token", out var newToken))
            {
                var newTokenValue = newToken.GetString();
                if (!string.IsNullOrEmpty(newTokenValue))
                {
                    _currentAccessToken = newTokenValue;
                    Environment.SetEnvironmentVariable("INSTAGRAM_ACCESS_TOKEN", newTokenValue);

                    var expiresIn = json.TryGetProperty("expires_in", out var exp) ? exp.GetInt64() : 0;
                    _logger.LogInformation(
                        "Instagram token refreshed successfully. New token expires in {Days} days",
                        expiresIn / 86400);
                    return true;
                }
            }

            _logger.LogError("Token refresh response did not contain access_token");
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to refresh Instagram token");
            return false;
        }
    }

    private async Task<string?> CreateMediaContainer(HttpClient client, string imageUrl, string caption)
    {
        var url = $"{BaseUrl}/{UserId}/media?image_url={Uri.EscapeDataString(imageUrl)}&caption={Uri.EscapeDataString(caption)}&access_token={AccessToken}";
        var response = await client.PostAsync(url, null);
        var content = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("Failed to create media container. Status: {Status}, Response: {Response}", response.StatusCode, content);
            return null;
        }

        var json = JsonSerializer.Deserialize<JsonElement>(content);
        return json.GetProperty("id").GetString();
    }

    private async Task<string?> CreateCarouselItemContainer(HttpClient client, string imageUrl)
    {
        var url = $"{BaseUrl}/{UserId}/media?image_url={Uri.EscapeDataString(imageUrl)}&is_carousel_item=true&access_token={AccessToken}";
        var response = await client.PostAsync(url, null);
        var content = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("Failed to create carousel item. Response: {Response}", content);
            return null;
        }

        var json = JsonSerializer.Deserialize<JsonElement>(content);
        return json.GetProperty("id").GetString();
    }

    private async Task<string?> CreateCarouselContainer(HttpClient client, List<string> childIds, string caption)
    {
        var children = string.Join(",", childIds);
        var url = $"{BaseUrl}/{UserId}/media?media_type=CAROUSEL&children={children}&caption={Uri.EscapeDataString(caption)}&access_token={AccessToken}";
        var response = await client.PostAsync(url, null);
        var content = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("Failed to create carousel container. Response: {Response}", content);
            return null;
        }

        var json = JsonSerializer.Deserialize<JsonElement>(content);
        return json.GetProperty("id").GetString();
    }

    private async Task WaitForMediaProcessing(HttpClient client, string containerId, int maxRetries = 10)
    {
        for (int i = 0; i < maxRetries; i++)
        {
            await Task.Delay(2000); // Wait 2 seconds between checks

            var url = $"{BaseUrl}/{containerId}?fields=status_code&access_token={AccessToken}";
            var response = await client.GetAsync(url);
            var content = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var json = JsonSerializer.Deserialize<JsonElement>(content);
                if (json.TryGetProperty("status_code", out var status))
                {
                    var statusCode = status.GetString();
                    if (statusCode == "FINISHED") return;
                    if (statusCode == "ERROR")
                    {
                        _logger.LogError("Media processing failed for container: {ContainerId}", containerId);
                        return;
                    }
                }
            }
        }

        _logger.LogWarning("Media processing timeout for container: {ContainerId}", containerId);
    }

    private async Task<string?> PublishMedia(HttpClient client, string containerId)
    {
        var url = $"{BaseUrl}/{UserId}/media_publish?creation_id={containerId}&access_token={AccessToken}";
        var response = await client.PostAsync(url, null);
        var content = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("Failed to publish media. Response: {Response}", content);
            return null;
        }

        var json = JsonSerializer.Deserialize<JsonElement>(content);
        var mediaId = json.GetProperty("id").GetString();
        _logger.LogInformation("Successfully published Instagram post. MediaId: {MediaId}", mediaId);
        return mediaId;
    }
}
