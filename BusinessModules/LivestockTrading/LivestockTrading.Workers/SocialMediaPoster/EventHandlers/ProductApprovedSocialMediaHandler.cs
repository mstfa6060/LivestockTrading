using LivestockTrading.Domain.Events;
using LivestockTrading.Workers.SocialMediaPoster.Services;

namespace LivestockTrading.Workers.SocialMediaPoster.EventHandlers;

public class ProductApprovedSocialMediaHandler
{
    private readonly IInstagramService _instagramService;
    private readonly IConfiguration _configuration;
    private readonly ILogger<ProductApprovedSocialMediaHandler> _logger;

    public ProductApprovedSocialMediaHandler(
        IInstagramService instagramService,
        IConfiguration configuration,
        ILogger<ProductApprovedSocialMediaHandler> logger)
    {
        _instagramService = instagramService;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task HandleAsync(ProductApprovedEvent evt)
    {
        _logger.LogInformation(
            "Processing social media post for approved product: {ProductId} - {Title}",
            evt.ProductId, evt.Title);

        // Check if Instagram token is valid
        if (!await _instagramService.IsTokenValidAsync())
        {
            _logger.LogWarning("Instagram access token is invalid or expired. Skipping post for product: {ProductId}", evt.ProductId);
            return;
        }

        // Build caption
        var caption = BuildCaption(evt);

        // Build image URL
        var imageUrl = BuildImageUrl(evt);

        if (string.IsNullOrWhiteSpace(imageUrl))
        {
            _logger.LogWarning("No cover image available for product: {ProductId}. Skipping Instagram post.", evt.ProductId);
            return;
        }

        // Post to Instagram
        var mediaId = await _instagramService.PostImageAsync(imageUrl, caption);

        if (mediaId != null)
        {
            _logger.LogInformation(
                "Successfully posted product {ProductId} to Instagram. MediaId: {MediaId}",
                evt.ProductId, mediaId);
        }
        else
        {
            _logger.LogWarning("Failed to post product {ProductId} to Instagram", evt.ProductId);
        }
    }

    private string BuildCaption(ProductApprovedEvent evt)
    {
        var webUrl = _configuration["Instagram:WebBaseUrl"] ?? _configuration["Instagram:SiteBaseUrl"] ?? "https://livestock-trading.com";
        var parts = new List<string>();

        // Title
        parts.Add(evt.Title);

        // Short description or truncated description
        var desc = evt.ShortDescription ?? evt.Description;
        if (!string.IsNullOrWhiteSpace(desc))
        {
            if (desc.Length > 300)
                desc = desc.Substring(0, 297) + "...";
            parts.Add(desc);
        }

        // Price
        if (evt.BasePrice > 0)
            parts.Add($"Price: {evt.BasePrice:N0} {evt.Currency}");

        // Location
        var location = new List<string>();
        if (!string.IsNullOrWhiteSpace(evt.City)) location.Add(evt.City);
        if (!string.IsNullOrWhiteSpace(evt.CountryCode)) location.Add(evt.CountryCode);
        if (location.Any())
            parts.Add($"Location: {string.Join(", ", location)}");

        // Link
        if (!string.IsNullOrWhiteSpace(evt.Slug))
            parts.Add($"View: {webUrl}/products/{evt.Slug}");

        // Hashtags
        var hashtags = new List<string> { "#livestocktrading", "#agriculture", "#farming" };
        if (!string.IsNullOrWhiteSpace(evt.CategoryName))
            hashtags.Add($"#{evt.CategoryName.Replace(" ", "").ToLowerInvariant()}");
        if (!string.IsNullOrWhiteSpace(evt.CountryCode))
            hashtags.Add($"#{evt.CountryCode.ToLowerInvariant()}");
        parts.Add(string.Join(" ", hashtags));

        var caption = string.Join("\n\n", parts);

        // Instagram caption limit is 2200 characters
        if (caption.Length > 2200)
            caption = caption.Substring(0, 2197) + "...";

        return caption;
    }

    private string BuildImageUrl(ProductApprovedEvent evt)
    {
        if (string.IsNullOrWhiteSpace(evt.CoverImageUrl))
            return null;

        // If it's already a full URL, use as-is
        if (evt.CoverImageUrl.StartsWith("http"))
            return evt.CoverImageUrl;

        // Build URL from FileProvider/MinIO path
        var siteUrl = _configuration["Instagram:SiteBaseUrl"] ?? "https://livestock-trading.com";
        return $"{siteUrl}/file-storage/{evt.CoverImageUrl}";
    }
}
