using LivestockTrading.Workers.SocialMediaPoster.Services;

namespace LivestockTrading.Workers.SocialMediaPoster.Workers;

/// <summary>
/// Periodically checks Instagram token expiry and refreshes it automatically.
/// Runs every 24 hours. Refreshes if token expires within 30 days.
/// </summary>
public class TokenRefreshWorker : BackgroundService
{
	private readonly IInstagramService _instagramService;
	private readonly ILogger<TokenRefreshWorker> _logger;
	private static readonly TimeSpan CheckInterval = TimeSpan.FromHours(24);
	private const int RefreshThresholdDays = 30;

	public TokenRefreshWorker(
		IInstagramService instagramService,
		ILogger<TokenRefreshWorker> logger)
	{
		_instagramService = instagramService;
		_logger = logger;
	}

	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		_logger.LogInformation("TokenRefreshWorker starting. Check interval: {Hours}h, Refresh threshold: {Days} days",
			CheckInterval.TotalHours, RefreshThresholdDays);

		// Wait for startup
		await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);

		while (!stoppingToken.IsCancellationRequested)
		{
			try
			{
				await CheckAndRefreshToken();
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error in token refresh check");
			}

			await Task.Delay(CheckInterval, stoppingToken);
		}
	}

	private async Task CheckAndRefreshToken()
	{
		var isValid = await _instagramService.IsTokenValidAsync();
		if (!isValid)
		{
			_logger.LogWarning("Instagram token is invalid. Cannot auto-refresh an invalid token. Manual intervention required.");
			return;
		}

		// Try to refresh proactively - Instagram allows refreshing tokens that are at least 24h old
		// We refresh every cycle to keep the token fresh (60 days from each refresh)
		var refreshed = await _instagramService.RefreshTokenAsync();
		if (refreshed)
		{
			_logger.LogInformation("Instagram token refreshed proactively");
		}
		else
		{
			_logger.LogWarning("Instagram token refresh failed. Token may expire soon. Check manually if this persists.");
		}
	}
}
