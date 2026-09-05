using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Modules.Common.Domain.Handlers;
using Modules.Common.Domain.Results;
using Modules.Users.Domain.Errors;
using Modules.Users.Infrastructure.Database;
using Modules.Users.PublicApi.Contracts;

namespace Modules.Users.Features.Users.GetUserProfile;

internal interface IGetUserProfileHandler : IHandler
{
	Task<Result<UserProfileResponse>> HandleAsync(string userId, CancellationToken cancellationToken);
}

internal sealed class GetUserProfileHandler(
	UsersDbContext context,
	ILogger<GetUserProfileHandler> logger) : IGetUserProfileHandler
{
	public async Task<Result<UserProfileResponse>> HandleAsync(string userId, CancellationToken cancellationToken)
	{
		var user = await context.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Id == userId, cancellationToken);
		if (user is null)
		{
			logger.LogInformation("User with ID {UserId} not found", userId);
			return UserErrors.NotFound(userId);
		}

		return new UserProfileResponse(user.Id, user.Email!, user.Street, user.City, user.Zip);
	}
}
