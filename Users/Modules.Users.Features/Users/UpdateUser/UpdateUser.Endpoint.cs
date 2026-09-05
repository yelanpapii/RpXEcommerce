using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Modules.Common.API.Abstractions;
using Modules.Common.API.Extensions;
using Modules.Users.Domain.Policies;
using Modules.Users.Features.Users.Shared.Routes;

namespace Modules.Users.Features.Users.UpdateUser;

public sealed record UpdateUserRequest(
    string Email,
    string? Role,
    string? Street = null,
    string? City = null,
    string? Zip = null);

public class UpdateUserEndpoint : IApiEndpoint
{
    public Asp.Versioning.ApiVersion Version => new(1.0);
    public void MapEndpoint(Microsoft.AspNetCore.Routing.IEndpointRouteBuilder app)
    {
        app.MapPut(RouteConsts.UpdateUser, Handle)
            .RequireAuthorization(UserPolicyConsts.UpdatePolicy)
			.WithTags("Users")
			.WithDescription("Updates a user's information.");

	}

    private static async Task<IResult> Handle(
        string userId,
        [FromBody] UpdateUserRequest request,
        IValidator<UpdateUserRequest> validator,
        IUpdateUserHandler handler,
        CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Results.ValidationProblem(validationResult.ToDictionary());
        }

        var response = await handler.HandleAsync(userId, request, cancellationToken);
        if (response.IsError)
        {
            return response.Errors.ToProblem();
        }

        return Results.Ok(response.Value);
    }
}
