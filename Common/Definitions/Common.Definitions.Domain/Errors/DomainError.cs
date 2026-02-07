namespace Common.Definitions.Domain.Errors;

public class DomainErrors
{

    public class AuthErrors
    {
        // Model Validation
        public static string UserIdNotValid { get; set; } = "Kullanıcı ID si geçersiz.";

        // Database Validation
        public static string UserNotExist { get; set; } = "Kullanıcı bulunamadı.";
        public static string UserBlocked { get; set; } = "Kullanıcı engellenmiş.";
        public static string UserBanned { get; set; } = "Kullanıcı yasaklanmış.";
        public static string UserDeleted { get; set; } = "Kullanıcı silinmiş.";
        public static string AdAccountsCanBeDeletedByAdministrator { get; set; } = "Reklam hesapları sadece yöneticiler tarafından silinebilir.";
        public static string UserCanDeleteOnlyOwnAccount { get; set; } = "Kullanıcı sadece kendi hesabını silebilir.";
        public static string EmailOrPasswordIncorrect { get; set; } = "E-posta veya şifre yanlış.";
        public static string UserLoginCodeNotBelongToRequestedUser { get; set; } = "Giriş kodu istenen kullanıcıya ait değil.";
        public static string UserLoginCodeIsExpired { get; set; } = "Giriş kodu süresi dolmuş.";
        public static string UserLoginCodeIsNotExpired { get; set; } = "Giriş kodu süresi dolmamış.";
        public static string UserLoginCodeIsNotCorrect { get; set; } = "Giriş kodu doğru değil.";
        public static string UserLoginCodeNotExist { get; set; } = "Giriş kodu bulunamadı.";
        public static string DeviceIdNotValid { get; set; } = "Cihaz ID si geçersiz.";
        public static string ApprovalExpired { get; set; } = "Onay süresi dolmuş.";
        public static string ApprovalHasOwner { get; set; } = "Onay sahibi var.";
        public static string ApprovalStatusIsInvalid { get; set; } = "Onay durumu geçersiz.";
        public static string UserCanOnlyMakeActionsForSelf { get; set; } = "Kullanıcı sadece kendi işlemlerini yapabilir.";
        public static string ApprovalNotFound { get; set; } = "Onay bulunamadı.";
        public static string QrCounterHasException { get; set; } = "QR sayaç hatası oluştu.";
        public static string QrJobTaskCancelled { get; set; } = "QR görevi iptal edildi.";
        public static string QrJobTaskHasException { get; set; } = "QR görevinde hata oluştu.";
        public static string IdentityNumberCannotBeEmpty { get; set; } = "Kimlik numarası boş olamaz.";
        public static string IdentityNumberIsInvalid { get; set; } = "Kimlik numarası geçersiz.";
        public static string VerificationIdCannotBeEmpty { get; set; } = "Doğrulama ID si boş olamaz.";
        public static string VerificationIdInvalidFormat { get; set; } = "Doğrulama ID si geçersiz format.";
        public static string VerificationEnvironmentInvalidFormat { get; set; } = "Doğrulama ortamı geçersiz format.";
        public static string VerificationMerkezBirligiUserCanLogin { get; set; } = "Merkez Birligi kullanıcıları giriş yapamaz.";
        public static string TenantIdIsNotGuidEmpty { get; set; } = "Tenant ID si geçersiz.";
        public static string RefreshTokenNotFound { get; set; } = "Yenileme tokeni bulunamadı.";
    }
    public class CommonErrors
    {
        public static string ErrorTitle { get; set; } = "Hata";
        // Model Validation
        public static string LocationIdNotValid { get; set; } = "Konum ID si geçersiz.";
        public static string DepartmanIdNotValid { get; set; } = "Departman ID si geçersiz.";

        // Database Validation
        public static string EkoopUserNotExist { get; set; } = "Ekoop kullanıcı bulunamadı.";
        public static string ModuleNotExist { get; set; } = "Modül bulunamadı.";
        public static string UniversityNotExist { get; set; } = "Üniversite bulunamadı.";
        public static string CompanyNotExist { get; set; } = "Şirket bulunamadı.";
        public static string CompanyPartnershipNotExist { get; set; } = "Şirket ortaklığı bulunamadı.";
        public static string CompanyManagementNotExist { get; set; } = "Şirket yönetimi bulunamadı.";
        public static string LocationNotExist { get; set; } = "Konum bulunamadı.";
        public static string DepartmentNotExist { get; set; } = "Departman bulunamadı.";
        public static string DepartmentUnitNotExist { get; set; } = "Departman birimi bulunamadı.";
        public static string TitleNotExist { get; set; } = "Başlık bulunamadı.";
        public static string QualificationNotExist { get; set; } = "Yetkinlik bulunamadı.";
        public static string ProcessNotExist { get; set; } = "İşlem bulunamadı.";
        public static string ProcessStepNotExist { get; set; } = "İşlem adımı bulunamadı.";
        public static string GroupNotExist { get; set; } = "Grup bulunamadı.";
        public static string QualificationHasBeenDeleted { get; set; } = "Yetkinlik silinmiş.";
        public static string FileBucketIdCanNotBeEmpty { get; set; } = "Dosya kütüğü ID si boş olamaz.";
        public static string DescriptionCanNotBeEmpty { get; set; } = "Açıklama boş olamaz.";
    }

    public class CityErrors
    {
        // Model Validation

        // Database Validation
        public static string CityNotExist { get; set; } = "Şehir bulunamadı.";

    }
    public class CompanyErrors
    {
        // Model Validation

        // Database Validation
        public static string UserDoesNotBelongToThisCompany { get; set; } = "Kullanıcı bu şirkete ait değil.";
        public static string NameRequired { get; set; } = "İsim zorunludur.";
        public static string TaxNumberRequired { get; set; } = "Vergi numarası zorunludur.";
        public static string TaxNumberAlreadyExists { get; set; } = "Vergi numarası zaten var.";
        public static string IdRequired { get; set; } = "ID zorunludur.";
        public static string NotFound { get; set; } = "Şirket bulunamadı.";
        public static string PickLimitInvalid { get; set; } = "Seçim limiti geçersiz.";
        public static string CompanyIdRequired { get; set; } = "Şirket ID si zorunludur.";

    }
    public class SystemAdminErrors
    {
        // Model Validation

        // Database Validation
        public static string SystemAdminNotExist { get; set; } = "Sistem yöneticisi bulunamadı.";
        public static string UserAlreadyRegisteredAsSystemAdmin { get; set; } = "Kullanıcı zaten sistem yöneticisi olarak kayıtlı.";
        public static string OnlySystemAdminPermitted { get; set; } = "Sadece sistem yöneticileri izinlidir.";
        public static string OnlyBussinesModuleAccepted { get; set; } = "Sadece iş modülleri kabul edilir.";
    }

    public class CompanyAdminErrors
    {
        // Model Validation

        // Database Validation
        public static string CompanyAdminNotExist { get; set; } = " Şirket yöneticisi bulunamadı.";
        public static string UserAlreadyRegisteredAsCompanyAdmin { get; set; } = "Kullanıcı zaten şirket yöneticisi olarak kayıtlı.";
        public static string OnlySystemAdminOrCompanyAdminPermitted { get; set; } = "Sadece sistem yöneticileri veya şirket yöneticileri izinlidir.";
        public static string UserNotPermitted { get; set; } = "Kullanıcı izinli değil.";
    }

    public class ModuleAdminErrors
    {
        // Model Validation

        // Database Validation
        public static string ModuleAdminNotExist { get; set; } = " Modül yöneticisi bulunamadı.";
        public static string UserAlreadyRegisteredAsModuleAdmin { get; set; } = "Kullanıcı zaten modül yöneticisi olarak kayıtlı.";
        public static string OnlySystemAdminOrModuleAdminPermitted { get; set; } = "Sadece sistem yöneticileri veya modül yöneticileri izinlidir.";
    }

    public class AuthorizedModuleErrors
    {
        // Model Validation

        // Database Validation
        public static string AuthorizedModuleNotExist { get; set; } = " Yetkili modül bulunamadı.";
        public static string AuthorizedModuleAlreadyExist { get; set; } = "Yetkili modül zaten var.";
        // public static string OnlyCompanyAdminPermitted { get; set; } = ;
    }

    public class CompanyResourceErrors
    {
        // Model Validation

        // Database Validation
        public static string CompanyResourceNotExist { get; set; } = " Şirket kaynağı bulunamadı.";

    }
    public class UserProxyErrors
    {
        public static string UserProxyNotExist { get; set; } = " Kullanıcı vekili bulunamadı.";
        public static object EndDateCantBeSmallerThanThisHour { get; set; } = " Bitiş tarihi bu saatten küçük olamaz.";
        public static object EndDateCantBeSmallerThanStartDate { get; set; } = " Bitiş tarihi başlangıç tarihinden küçük olamaz.";
    }
    public class CompanySettingErrors
    {
        public static string CompanySettingNotExist { get; set; } = " Şirket ayarı bulunamadı.";
        public static string CompanyIdNotExist { get; set; } = "Şirket ID'si bulunamadı.";
    }

    public class AuthorizationServiceErrors
    {
        public static string UserDoesNotHaveSufficientPermission { get; set; } = "Kullanıcı yeterli izne sahip değil.";
        public static string UserIsBanned { get; set; } = "Kullanıcı yasaklandı.";
        public static string CrossTenantDataAccessNotPermtted { get; set; } = "Çapraz kiracı veri erişimi izinli değil.";
        public static string NamespaceNotFound { get; set; } = "Ad alanı bulunamadı.";
        public static string TenantRelationError { get; set; } = "Kiracı ilişkisi hatası";
    }
    public class PollErrors
    {
        public static string PollIdNotValid { get; set; } = "Anket ID'si geçerli değil.";
        public static string PollCompanyIdNotValid { get; set; } = "Anket şirket ID'si geçerli değil.";
        public static string PollOptionIdNotValid { get; set; } = "Anket seçeneği ID'si geçerli değil.";
        public static string PollAnswerIdNotValid { get; set; } = "Anket cevabı ID'si geçerli değil.";

    }
    public class AnnouncementErrors
    {
        public static string AnnouncementIdNotValid { get; set; } = "Duyuru ID'si geçerli değil.";
        public static string AnnouncementCompanyIdNotValid { get; set; } = "Duyuru şirket ID'si geçerli değil.";
        public static string AnnouncementNotFound { get; set; } = "Duyuru bulunamadı.";


    }
    public class EventErrors
    {
        public static string EventIdNotValid { get; set; } = "Etkinlik ID'si geçerli değil.";
        public static string EventCompanyIdNotValid { get; set; } = "Etkinlik şirket ID'si geçerli değil.";
        public static string EventNotFound { get; set; } = "Etkinlik bulunamadı.";

    }
    public class InstrumentErrors
    {
        public static string InstrumentIdNotValid { get; set; } = "Alet ID'si geçerli değil.";


    }

    public class RoleErrors
    {
        public static string NameValid { get; set; } = "Geçersiz rol adı.";
        public static string IdValid { get; set; } = "Geçersiz rol ID'si.";
        public static string IncludeDeletedValid { get; set; } = "Geçersiz silinen rol dahil etme parametresi.";
        public static string RoleNotExist { get; set; } = "Rol bulunamadı.";

    }

    public class UserErrors
    {
        public static string UserNameValid { get; set; } = "Geçersiz kullanıcı adı.";
        public static string EmailValid { get; set; } = "Geçersiz e-posta adresi.";

        // Şifre ile ilgili hata kodları
        public static string PasswordRequired { get; set; } = "Şifre zorunludur.";
        public static string PasswordTooShort { get; set; } = "Şifre en az 6 karakter olmalıdır.";
        public static string PasswordIsRequired { get; set; } = "Şifre gereklidir.";
        public static string PasswordTooLong { get; set; } = "Şifre en fazla 20 karakter olmalıdır.";
        public static string PasswordMustContainUppercase { get; set; } = "Şifre en az bir büyük harf içermelidir.";
        public static string PasswordMustContainLowercase { get; set; } = "Şifre en az bir küçük harf içermelidir.";
        public static string PasswordMustContainDigit { get; set; } = "Şifre en az bir rakam içermelidir.";
        public static string UserNotFound { get; set; } = " Kullanıcı bulunamadı.";
        public static string UserIdIsRequired { get; set; } = "Kullanıcı ID si gereklidir.";
        public static string InvalidPassword { get; set; } = "Geçersiz şifre.";
        public static string EmailNotFound { get; set; } = "E-posta adresi bulunamadı.";
        public static string RefreshTokenRequired { get; set; } = "Refresh token gereklidir.";
        public static string RefreshTokenInvalid { get; set; } = "Geçersiz refresh token.";
        public static string RefreshTokenExpired { get; set; } = "Refresh token süresi dolmuş.";
        public static string GoogleTokenInvalid { get; set; } = "Geçersiz Google token.";
        public static string AppleTokenInvalid { get; set; } = "Geçersiz Apple token.";
        public static string ProviderInvalid { get; set; } = "Bilinmeyen giriş sağlayıcısı.";
        public static string TokenRequired { get; set; } = "Token gereklidir.";
        public static string UserNameRequired { get; set; } = "Kullanıcı adı gereklidir.";
        public static string ProviderRequired { get; set; } = "Giriş sağlayıcısı gereklidir.";
        public static string InvalidCredentials { get; set; } = "Geçersiz kimlik bilgileri.";
        public static string UserIdNotFound { get; set; } = "Kullanıcı ID si bulunamadı.";
        public static string PhoneNumberRequired { get; set; } = "Telefon numarası gereklidir.";
        public static string PhoneNumberInvalid { get; set; } = "Telefon numarası formatı geçersiz.";
        public static string MessageRequired { get; set; } = "Mesaj gereklidir.";
        public static string LanguageRequired { get; set; } = "Dil gereklidir.";
        public static string GoogleTokenExpired { get; set; } = "Google token süresi dolmuş.";
        public static string InvalidVisibilityRadius { get; set; } = "Geçersiz görünürlük yarıçapı.";
        public static string GoogleEmailNotVerified { get; set; } = "Google e-posta adresi doğrulanmamış.";
        public static string UserFirstNameRequired { get; set; } = "Ad zorunludur.";
        public static string UserSurnameRequired { get; set; } = "Soyad zorunludur.";
        public static string UserEmailAlreadyExists { get; set; } = "Bu e-posta adresi zaten kullanılıyor.";
        public static string UserUserNameAlreadyExists { get; set; } = "Bu kullanıcı adı zaten kullanılıyor.";
        public static string UserCountryIdRequired { get; set; } = "Ülke seçimi zorunludur.";

    }
}