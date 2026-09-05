using Modules.Common.Domain.Results;
using Modules.Users.PublicApi.Contracts;

namespace Modules.Users.PublicApi;

public interface IUsersModuleApi
{
	Task<Result<UserProfileResponse>> GetProfileAsync(string userId, CancellationToken cancellationToken = default);
}
