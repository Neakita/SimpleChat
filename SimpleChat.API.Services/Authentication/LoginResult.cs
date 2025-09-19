namespace SimpleChat.API.Services.Authentication;

public sealed class LoginResult
{
	public int UserId { get; set; }
	public string RefreshToken { get; set; } = string.Empty;
	public DateTime RefreshTokenExpirationTimestamp { get; set; }
}