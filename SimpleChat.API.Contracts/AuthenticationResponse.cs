namespace SimpleChat.API.Contracts;

public sealed class AuthenticationResponse
{
	public int UserId { get; set; }
	public string AccessToken { get; set; } = string.Empty;
	public string RefreshToken { get; set; } = string.Empty;
	public DateTime AccessTokenExpirationTimestamp { get; set; }
}