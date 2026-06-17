namespace Iam.Domain.Errors;

// Error constants are SYMBOLIC CODES, not user-facing text. The frontend
// (and any other localized client) maps them through its locale dictionary
// (livestock-frontend/common/livestock-api/src/errors/locales/...). Keeping
// the codes in SCREAMING_SNAKE_CASE makes them stable across UI languages
// and grep-able in client codebases.
public static class IamErrors
{
    public static class Auth
    {
        // Codes are aligned with the existing frontend dictionary
        // (livestock-frontend/common/livestock-api/src/errors/locales/...)
        // to avoid duplicate keys for the same concept.
        public const string InvalidCredentials = "EMAIL_OR_PASSWORD_INCORRECT";
        public const string UserNotFound = "USER_NOT_FOUND";
        public const string InvalidRefreshToken = "REFRESH_TOKEN_NOT_FOUND";
        public const string ProviderRequired = "AUTH_PROVIDER_REQUIRED";
        public const string InvalidProvider = "AUTH_INVALID_PROVIDER";
        public const string OtpInvalid = "AUTH_OTP_INVALID";
        public const string OtpExpired = "AUTH_OTP_EXPIRED";
        public const string ResetTokenInvalid = "AUTH_RESET_TOKEN_INVALID";
    }

    public static class Users
    {
        public const string EmailAlreadyExists = "USER_EMAIL_ALREADY_EXISTS";
        public const string UserNameAlreadyExists = "USER_USER_NAME_ALREADY_EXISTS";
        public const string NotFound = "USER_NOT_FOUND";
    }
}
