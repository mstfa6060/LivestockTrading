namespace Shared.Abstractions.Endpoints;

/// <summary>
/// Marker interface for FastEndpoints endpoint discovery via Architecture.Tests.
/// FastEndpoints endpoints extend Endpoint[TReq, TRes] directly — this interface
/// is for NetArchTest assertions only.
/// </summary>
public interface IEndpoint;
