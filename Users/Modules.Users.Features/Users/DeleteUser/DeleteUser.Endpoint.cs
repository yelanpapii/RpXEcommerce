using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Modules.Common.API.Abstractions;
using Modules.Common.API.Extensions;
using Modules.Users.Domain.Policies;
using Modules.Users.Features.Users.Shared.Routes;

namespace Modules.Users.Features.Users.DeleteUser;

public class DeleteUserEndpoint : IApiEndpoint
{
    public Asp.Versioning.ApiVersion Version => new(1.0);
    public void MapEndpoint(Microsoft.AspNetCore.Routing.IEndpointRouteBuilder app)
    {
		app.MapDelete(RouteConsts.DeleteUser, Handle)
			.RequireAuthorization(UserPolicyConsts.DeletePolicy)
			.WithTags("Users")
			.WithDescription("Deletes a user by their ID.");

	}

    private static async Task<IResult> Handle(
        string userId,
        IDeleteUserHandler handler,
        CancellationToken cancellationToken)
    {
        var response = await handler.HandleAsync(userId, cancellationToken);
        if (response.IsError)
        {
            return response.Errors.ToProblem();
        }

        return Results.NoContent();
    }
}
