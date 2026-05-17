using Shared.Results;
using Microsoft.AspNetCore.Http;

namespace LivestockTrading.Catalog.Application.Common;

/// <summary>
/// Maps Shared.Results.Result/Result&lt;T&gt; to ASP.NET Core Minimal API IResult.
/// Status code matrix derived from Error.Code prefix:
///   INVALID_*    → 400 BadRequest
///   NOT_FOUND*   → 404 NotFound
///   CONFLICT*    → 409 Conflict
///   UNAUTHORIZED → 401 Unauthorized
///   FORBIDDEN    → 403 Forbidden
///   (default)    → 400 BadRequest (catch-all)
/// Success → 200 OK (no body for Result, with Value for Result&lt;T&gt;).
/// </summary>
public static class ResultExtensions
{
    public static IResult ToApiResult(this Result result)
        => result.IsSuccess
            ? Results.Ok()
            : MapError(result.Error!);

    public static IResult ToApiResult<T>(this Result<T> result)
        => result.IsSuccess
            ? Results.Ok(result.Value)
            : MapError(result.Error!);

    private static IResult MapError(Error error)
    {
        var payload = new { code = error.Code, message = error.Message };
        return error.Code switch
        {
            var c when c.StartsWith("INVALID_") => Results.BadRequest(payload),
            var c when c.StartsWith("NOT_FOUND") => Results.NotFound(payload),
            var c when c.StartsWith("CONFLICT") => Results.Conflict(payload),
            "UNAUTHORIZED" => Results.Unauthorized(),
            "FORBIDDEN" => Results.Forbid(),
            _ => Results.BadRequest(payload)
        };
    }
}
