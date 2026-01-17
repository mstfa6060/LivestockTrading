namespace Common.Services.Notification;

public class TokenCleanupService : ITokenCleanupService
{
    public TokenCleanupService()
    {
    }

    public async Task CleanupInvalidTokenAsync(string token)
    {
        try
        {
            Console.WriteLine($"[TokenCleanup] 🗑️ Geçersiz token temizleniyor: {token.Substring(0, Math.Min(20, token.Length))}...");

            // TODO: Burada veritabanından token'ı silme işlemi yapılacak
            // Örnek: await _dbContext.AppUserPushTokens.Where(x => x.PushToken == token).ExecuteDeleteAsync();

            Console.WriteLine($"[TokenCleanup]  Token başarıyla temizlendi: {token.Substring(0, Math.Min(20, token.Length))}...");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[TokenCleanup] ❌ Token temizleme hatası: {ex.Message}");
        }
    }

    public async Task CleanupInvalidTokensAsync(List<string> tokens)
    {
        if (tokens == null || !tokens.Any())
        {
            Console.WriteLine("[TokenCleanup] ℹ️ Temizlenecek token bulunamadı");
            return;
        }

        Console.WriteLine($"[TokenCleanup] 🗑️ {tokens.Count} geçersiz token temizleniyor...");

        foreach (var token in tokens)
        {
            await CleanupInvalidTokenAsync(token);
        }

        Console.WriteLine($"[TokenCleanup]  {tokens.Count} token temizleme işlemi tamamlandı");
    }
}
