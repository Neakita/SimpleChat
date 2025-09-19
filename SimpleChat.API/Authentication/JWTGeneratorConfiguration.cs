using Microsoft.IdentityModel.Tokens;

namespace SimpleChat.API.Authentication;

public sealed class JWTGeneratorConfiguration
{
	public string Issuer { get; set; } = string.Empty;
	public string Audience { get; set; } = string.Empty;
	public TimeSpan TokenExpiration { get; set; }
	public required SecurityKey SecurityKey { get; set; }
}