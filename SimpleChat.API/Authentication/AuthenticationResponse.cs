namespace SimpleChat.API.Authentication;

public sealed class AuthenticationResponse
{
	public required string AccessToken { get; init; }
	public required string RefreshToken { get; init; }
	public required DateTime AccessTokenExpirationTimestamp { get; init; }
}