using System.Linq.Expressions;

namespace Common.Definitions.Base.Entity;

public static class CustomExtensions
{
    public static Guid GetTenantId<TEntity>(this TEntity entity) where TEntity : ITenantEntity
    {
        return entity.GetTenantId();
    }

    public static string GetMemberName<T>(Expression<Func<T>> memberExpression)
    {
        MemberExpression expressionBody = (MemberExpression)memberExpression.Body;
        return expressionBody.Member.Name;
    }

    public static string GetPropertyName<T>(Expression<Func<T>> memberExpression)
    {
        var originalName = GetMemberName(memberExpression);
        return originalName;
    }
}
