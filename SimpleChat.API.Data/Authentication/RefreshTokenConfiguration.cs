namespace SimpleChat.API.Data.Authentication;

public sealed class RefreshTokenConfiguration
{
	public TimeSpan ExpirationTime { get; set; }
}