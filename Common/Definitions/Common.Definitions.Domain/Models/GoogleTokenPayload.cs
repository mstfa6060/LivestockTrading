namespace Common.Definitions.Domain.Models;

public class GoogleTokenPayload
{
    public string Sub { get; set; }       // Google benzersiz kullanıcı ID'si
    public string Email { get; set; }
    public string Name { get; set; }
    public string Picture { get; set; }
    public string Aud { get; set; }       // Uygulamanın Client ID'si
    public string Exp { get; set; }
}
