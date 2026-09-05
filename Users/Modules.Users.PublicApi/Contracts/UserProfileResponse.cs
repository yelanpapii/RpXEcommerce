namespace Modules.Users.PublicApi.Contracts;

public sealed record UserProfileResponse(
	string Id,
	string Email,
	string? Street,
	string? City,
	string? Zip);
