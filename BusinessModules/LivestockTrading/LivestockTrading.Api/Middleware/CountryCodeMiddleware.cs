namespace LivestockTrading.Api.Middleware;

public class CountryCodeMiddleware
{
    private readonly RequestDelegate _next;

    public CountryCodeMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Read X-Country-Code header
        var countryCode = context.Request.Headers["X-Country-Code"].FirstOrDefault();

        if (!string.IsNullOrWhiteSpace(countryCode))
        {
            context.Items["CountryCode"] = countryCode.ToUpperInvariant();
        }

        await _next(context);
    }
}

public static class CountryCodeMiddlewareExtensions
{
    public static IApplicationBuilder UseCountryCode(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<CountryCodeMiddleware>();
    }
}
