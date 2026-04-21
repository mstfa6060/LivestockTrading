using System.Linq.Expressions;

namespace Common.Definitions.Base.Entity;
public interface ITenantEntity
{
    Guid GetTenantId();
    object GetTenantEntity();
    string GetTenantPropertyName();
}

public interface ITenantEntity2
{
    Guid GetTenantId();
    Expression<Func<CoreEntity, CoreEntity>> GetExpression();
}
