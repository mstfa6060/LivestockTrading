namespace Iam.Domain.Errors;

public static class IamErrors
{
    public static class Auth
    {
        public const string InvalidCredentials = "Kullanıcı adı veya şifre hatalı.";
        public const string UserNotFound = "Kullanıcı bulunamadı.";
        public const string InvalidRefreshToken = "Refresh token geçersiz veya süresi dolmuş.";
        public const string ProviderRequired = "Login provider zorunludur.";
        public const string InvalidProvider = "Geçersiz login provider. (native, google, apple)";
        public const string OtpInvalid = "OTP kodu hatalı.";
        public const string OtpExpired = "OTP kodunun süresi dolmuş (10 dakika).";
        public const string ResetTokenInvalid = "Şifre sıfırlama bağlantısı geçersiz veya süresi dolmuş.";
    }

    public static class Users
    {
        public const string EmailAlreadyExists = "Bu e-posta adresi zaten kayıtlı.";
        public const string UserNameAlreadyExists = "Bu kullanıcı adı zaten alınmış.";
        public const string NotFound = "Kullanıcı bulunamadı.";
    }
}
