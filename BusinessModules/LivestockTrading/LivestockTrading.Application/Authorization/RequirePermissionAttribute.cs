using LivestockTrading.Domain.Enums;

namespace LivestockTrading.Application.Authorization;

/// <summary>
/// Endpoint'e erişim için gerekli izinleri belirten attribute
/// Handler class'larına uygulanır
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public class RequirePermissionAttribute : Attribute
{
    /// <summary>
    /// Gerekli izinler
    /// </summary>
    public Permission[] Permissions { get; }

    /// <summary>
    /// True: Tüm izinler gerekli (AND)
    /// False: Herhangi biri yeterli (OR) - varsayılan
    /// </summary>
    public bool RequireAll { get; set; } = false;

    /// <summary>
    /// Endpoint'e erişim için gerekli izinleri belirtir
    /// </summary>
    /// <param name="permissions">Gerekli izinler</param>
    public RequirePermissionAttribute(params Permission[] permissions)
    {
        Permissions = permissions ?? Array.Empty<Permission>();
    }
}

/// <summary>
/// Endpoint'e erişim için gerekli rolleri belirten attribute
/// Handler class'larına uygulanır
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public class RequireRoleAttribute : Attribute
{
    /// <summary>
    /// Gerekli roller
    /// </summary>
    public string[] Roles { get; }

    /// <summary>
    /// True: Tüm roller gerekli (AND)
    /// False: Herhangi biri yeterli (OR) - varsayılan
    /// </summary>
    public bool RequireAll { get; set; } = false;

    /// <summary>
    /// Endpoint'e erişim için gerekli rolleri belirtir
    /// </summary>
    /// <param name="roles">Gerekli roller (LivestockTradingConstants.Roles değerleri)</param>
    public RequireRoleAttribute(params string[] roles)
    {
        Roles = roles ?? Array.Empty<string>();
    }
}

/// <summary>
/// Endpoint'in public olduğunu belirten attribute (authentication gerektirmez)
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public class AllowAnonymousAttribute : Attribute
{
}

/// <summary>
/// Endpoint'in sadece kaynak sahibi tarafından erişilebileceğini belirten attribute
/// Örn: Kullanıcı sadece kendi siparişlerini görebilir
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public class RequireOwnershipAttribute : Attribute
{
    /// <summary>
    /// Sahiplik kontrolü yapılacak entity tipi
    /// </summary>
    public string EntityType { get; }

    /// <summary>
    /// Request model'deki ID property adı
    /// </summary>
    public string IdPropertyName { get; set; } = "Id";

    public RequireOwnershipAttribute(string entityType)
    {
        EntityType = entityType;
    }
}
