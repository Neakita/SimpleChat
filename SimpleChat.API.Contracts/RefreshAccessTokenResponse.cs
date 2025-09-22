namespace SimpleChat.API.Contracts;

public sealed class RefreshAccessTokenResponse
{
	public required string Token { get; init; }
	public required DateTime ExpirationTimestamp { get; init; }
}