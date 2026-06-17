using NSwag.Generation.Processors;
using NSwag.Generation.Processors.Contexts;

namespace Shared.Infrastructure.Swagger;

/// FastEndpoints emits long namespace-flattened operationIds like
/// "IamFeaturesAuthLoginLoginEndpoint". NSwag turns those into ugly TS method
/// names. Replace them with `{Tag}_{LastRouteSegment}` so the generator
/// produces `AuthClient.login()`, `SellersClient.nearby()`, etc.
public sealed class RouteBasedOperationIdProcessor : IOperationProcessor
{
    public bool Process(OperationProcessorContext ctx)
    {
        var op = ctx.OperationDescription.Operation;
        var path = ctx.OperationDescription.Path ?? "/";

        var segments = path.Split('/', StringSplitOptions.RemoveEmptyEntries);
        if (segments.Length == 0)
        {
            return true;
        }

        var lastSegment = segments[^1];
        // Strip any path parameter braces (defensive — should not happen with our POST /Entity/Action pattern).
        if (lastSegment.StartsWith('{') && lastSegment.EndsWith('}'))
        {
            lastSegment = lastSegment.Trim('{', '}');
        }

        var tag = op.Tags?.FirstOrDefault() ?? "Default";
        op.OperationId = $"{tag}_{lastSegment}";
        return true;
    }
}
