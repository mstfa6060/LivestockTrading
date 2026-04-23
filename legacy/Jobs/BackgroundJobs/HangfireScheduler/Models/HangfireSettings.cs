namespace Jobs.BackgroundJobs.HangfireScheduler.Models;

/// <summary>
/// Hangfire ayarları
/// </summary>
public class HangfireSettings
{
    /// <summary>
    /// Hangfire dashboard kullanıcı bilgileri
    /// </summary>
    public UserModel User { get; set; }

    /// <summary>
    /// Hangfire'dan API'ye yapılan çağrılar için sistem secret
    /// Bu secret hem Hangfire hem de API tarafında aynı olmalı
    /// </summary>
    public string SystemSecret { get; set; }

    public class UserModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}