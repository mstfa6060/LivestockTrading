namespace LivestockTrading.Application.Constants;

public static class LivestockTradingConstants
{
    public const string ModuleName = "LivestockTrading";

    public static class CacheKeys
    {
        public const string Prefix = "LivestockTrading_";
    }

    public static class Roles
    {
        // Temel roller
        public const string Admin = "Admin";
        public const string Moderator = "Moderator";
        public const string Support = "Support";
        public const string Seller = "Seller";
        public const string Transporter = "Transporter";
        public const string Buyer = "Buyer";
        public const string Veterinarian = "Veterinarian";

        // Rol grupları
        public static readonly string[] AdminRoles = { Admin };
        public static readonly string[] ModeratorRoles = { Admin, Moderator };
        public static readonly string[] SupportRoles = { Admin, Moderator, Support };
        public static readonly string[] ContentManagers = { Admin, Moderator };
        public static readonly string[] SellerRoles = { Admin, Moderator, Seller };
        public static readonly string[] TransporterRoles = { Admin, Moderator, Transporter };
        public static readonly string[] AllRoles = { Admin, Moderator, Support, Seller, Transporter, Buyer, Veterinarian };

        // JWT claim formatı (ModuleName.RoleName)
        public static string GetJwtRole(string roleName) => $"{ModuleName}.{roleName}";
    }
}
