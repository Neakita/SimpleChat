using System.ComponentModel.DataAnnotations;

namespace SimpleChat.API.Data.Model;

public sealed class UserSession
{
	[MaxLength(64)]
	public string RefreshToken { get; set; } = string.Empty;

	public DateTime RefreshTokenExpirationTimestamp { get; set; }
}