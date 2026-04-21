using System.Linq.Expressions;

namespace Common.Services.ErrorCodeGenerator;

public static class ErrorCodeGenerator
{
    public static string GetFrontendName(string name)
    {
        var frontendName = "" + name[0];

        for (int i = 1; i < name.Length; i++)
        {
            var ch = name[i];

            // if (ch >= 'A' & ch <= 'Z')
            if (ch >= 65 & ch <= 90)
            {
                frontendName += '_';
                frontendName += name[i];
            }
            else
            {
                frontendName += char.ToUpperInvariant(name[i]);
            }
        }

        return frontendName;
    }

    public static string GetMemberName<T>(Expression<Func<T>> memberExpression)
    {
        MemberExpression expressionBody = (MemberExpression)memberExpression.Body;
        return expressionBody.Member.Name;
    }

    public static string GetErrorCode<T>(Expression<Func<T>> memberExpression)
    {
        var originalName = GetMemberName(memberExpression);
        var code = GetFrontendName(originalName);
        return code;
    }
}