using Modules.Common.Domain.Results;
using Modules.Users.Features.Users.GetUserProfile;
using Modules.Users.PublicApi;
using Modules.Users.PublicApi.Contracts;

namespace Modules.Users.Features.InternalApi;

internal sealed class UsersModuleApi(IGetUserProfileHandler getUserProfileHandler) : IUsersModuleApi
{
	public async Task<Result<UserProfileResponse>> GetProfileAsync(string userId, CancellationToken cancellationToken = default)
	{
		return await getUserProfileHandler.HandleAsync(userId, cancellationToken);
	}
}
