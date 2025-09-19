namespace SimpleChat.API.Authentication;

public sealed class RefreshAccessTokenResponse
{
	public required string Token { get; init; }
	public required DateTime ExpirationTimestamp { get; init; }
}