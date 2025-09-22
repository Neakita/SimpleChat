using Microsoft.IdentityModel.Tokens;

namespace SimpleChat.API.Authentication;

public sealed class JWTGeneratorConfiguration
{
	public TimeSpan TokenExpiration { get; set; }
	public required SecurityKey SecurityKey { get; set; }
}