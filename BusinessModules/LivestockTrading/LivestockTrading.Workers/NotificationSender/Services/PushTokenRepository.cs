using LivestockTrading.Domain.Entities;
using LivestockTrading.Infrastructure.RelationalDB;
using Microsoft.EntityFrameworkCore;

namespace LivestockTrading.Workers.NotificationSender.Services;

public class PushTokenRepository
{
    private readonly LivestockTradingModuleDbContext _dbContext;
    private readonly ILogger<PushTokenRepository> _logger;

    public PushTokenRepository(
        LivestockTradingModuleDbContext dbContext,
        ILogger<PushTokenRepository> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    /// <summary>
    /// Kullanicinin aktif push token'larini getirir (revoke edilmemis)
    /// </summary>
    public async Task<List<string>> GetActiveTokensForUser(Guid userId, CancellationToken ct = default)
    {
        return await _dbContext.AppUserPushTokens
            .AsNoTracking()
            .Where(t => t.UserId == userId && t.RevokedAt == null)
            .Select(t => t.PushToken)
            .ToListAsync(ct);
    }

    /// <summary>
    /// Birden fazla kullanicinin aktif push token'larini getirir
    /// </summary>
    public async Task<Dictionary<Guid, List<string>>> GetActiveTokensForUsers(List<Guid> userIds, CancellationToken ct = default)
    {
        var tokens = await _dbContext.AppUserPushTokens
            .AsNoTracking()
            .Where(t => userIds.Contains(t.UserId) && t.RevokedAt == null)
            .Select(t => new { t.UserId, t.PushToken })
            .ToListAsync(ct);

        return tokens
            .GroupBy(t => t.UserId)
            .ToDictionary(g => g.Key, g => g.Select(t => t.PushToken).ToList());
    }

    /// <summary>
    /// Gecersiz token'lari revoke eder (Firebase hatasi sonrasi)
    /// </summary>
    public async Task RevokeTokens(List<string> invalidTokens, CancellationToken ct = default)
    {
        if (invalidTokens == null || invalidTokens.Count == 0) return;

        var tokensToRevoke = await _dbContext.AppUserPushTokens
            .Where(t => invalidTokens.Contains(t.PushToken) && t.RevokedAt == null)
            .ToListAsync(ct);

        foreach (var token in tokensToRevoke)
        {
            token.RevokedAt = DateTime.UtcNow;
        }

        if (tokensToRevoke.Count > 0)
        {
            await _dbContext.SaveChangesAsync(ct);
            _logger.LogInformation("Revoked {Count} invalid push tokens", tokensToRevoke.Count);
        }
    }

    /// <summary>
    /// In-app bildirim kaydeder
    /// </summary>
    public async Task SaveNotification(
        Guid userId,
        string title,
        string message,
        NotificationType type,
        string actionData = null,
        CancellationToken ct = default)
    {
        var notification = new Notification
        {
            UserId = userId,
            Title = title,
            Message = message,
            Type = type,
            ActionData = actionData,
            IsRead = false,
            SentAt = DateTime.UtcNow
        };

        _dbContext.Notifications.Add(notification);
        await _dbContext.SaveChangesAsync(ct);

        _logger.LogInformation(
            "Saved in-app notification for user {UserId}. Type: {Type}, Title: {Title}",
            userId, type, title);
    }

    // Gece sessiz saatler: kullanicinin yerel saatine gore 21:00 ile 08:00 arasinda push gonderilmez.
    private const int QuietHoursStartLocal = 21; // 21:00 sonrasi sessiz
    private const int QuietHoursEndLocal = 8;    // 08:00 oncesi sessiz

    /// <summary>
    /// Push bildirimin simdi gonderilip gonderilemeyecegini dondurur:
    /// - Kullanici push bildirimleri kapatmis mi?
    /// - Kullanicinin yerel saatine gore gece sessiz saatlerinde miyiz (21:00-08:00)?
    /// Sessiz saatlerde in-app bildirim yine kaydedilir; sadece push bastirilir.
    /// </summary>
    public async Task<bool> ShouldSendPushNow(Guid userId, CancellationToken ct = default)
    {
        var preferences = await _dbContext.UserPreferences
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.UserId == userId && !p.IsDeleted, ct);

        if (preferences != null && !preferences.PushNotificationsEnabled)
            return false;

        var timeZoneId = preferences?.TimeZone;
        return !IsInQuietHours(DateTime.UtcNow, timeZoneId);
    }

    private static bool IsInQuietHours(DateTime utcNow, string? timeZoneId)
    {
        TimeZoneInfo tz;
        try
        {
            tz = string.IsNullOrWhiteSpace(timeZoneId)
                ? TimeZoneInfo.Utc
                : TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
        }
        catch (TimeZoneNotFoundException)
        {
            tz = TimeZoneInfo.Utc;
        }
        catch (InvalidTimeZoneException)
        {
            tz = TimeZoneInfo.Utc;
        }

        var localHour = TimeZoneInfo.ConvertTimeFromUtc(utcNow, tz).Hour;
        // 21:00 (inclusive) – 08:00 (exclusive) → sessiz
        return localHour >= QuietHoursStartLocal || localHour < QuietHoursEndLocal;
    }

    /// <summary>
    /// Seller tablosundaki kaydin UserId'sini getirir (IAM User ID)
    /// </summary>
    public async Task<Guid?> GetSellerUserId(Guid sellerId, CancellationToken ct = default)
    {
        return await _dbContext.Sellers
            .AsNoTracking()
            .Where(s => s.Id == sellerId && !s.IsDeleted)
            .Select(s => s.UserId)
            .FirstOrDefaultAsync(ct);
    }
}
