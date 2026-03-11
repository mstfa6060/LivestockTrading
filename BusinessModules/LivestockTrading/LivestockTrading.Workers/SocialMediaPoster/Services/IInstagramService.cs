namespace LivestockTrading.Workers.SocialMediaPoster.Services;

public interface IInstagramService
{
    /// <summary>
    /// Posts a single image with caption to Instagram
    /// </summary>
    /// <param name="imageUrl">Publicly accessible image URL</param>
    /// <param name="caption">Post caption text</param>
    /// <returns>Instagram media ID if successful, null if failed</returns>
    Task<string?> PostImageAsync(string imageUrl, string caption);

    /// <summary>
    /// Posts a carousel (multiple images) with caption to Instagram
    /// </summary>
    /// <param name="imageUrls">List of publicly accessible image URLs (2-10)</param>
    /// <param name="caption">Post caption text</param>
    /// <returns>Instagram media ID if successful, null if failed</returns>
    Task<string?> PostCarouselAsync(List<string> imageUrls, string caption);

    /// <summary>
    /// Checks if the Instagram access token is valid
    /// </summary>
    Task<bool> IsTokenValidAsync();

    /// <summary>
    /// Gets the number of days until the access token expires.
    /// Returns -1 if token is invalid or check fails.
    /// </summary>
    Task<int> GetTokenExpiryDaysAsync();

    /// <summary>
    /// Refreshes the long-lived access token.
    /// Instagram long-lived tokens can be refreshed if they are at least 24 hours old and not expired.
    /// </summary>
    /// <returns>True if token was refreshed successfully</returns>
    Task<bool> RefreshTokenAsync();
}
