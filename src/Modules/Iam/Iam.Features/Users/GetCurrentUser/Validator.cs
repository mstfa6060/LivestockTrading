using FastEndpoints;
using FluentValidation;

namespace Iam.Features.Users.GetCurrentUser;

public sealed class GetCurrentUserValidator : Validator<GetCurrentUserRequest>
{
    public GetCurrentUserValidator()
    {
        // No validation needed — identity comes from JWT
    }
}
