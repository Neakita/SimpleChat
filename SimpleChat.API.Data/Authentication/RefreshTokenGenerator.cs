using System.Security.Cryptography;

namespace SimpleChat.API.Data.Authentication;

public static class RefreshTokenGenerator
{
	public static string GenerateToken()
	{
		var data = RandomNumberGenerator.GetBytes(64);
		return Convert.ToBase64String(data);
	}
}