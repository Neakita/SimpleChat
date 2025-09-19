using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;

namespace SimpleChat.API.Authentication;

public sealed class JWTGenerator(JWTGeneratorConfiguration configuration)
{
	public JwtSecurityToken GenerateToken(params IEnumerable<Claim> claims)
	{
		return GenerateToken(configuration.TokenExpiration, claims);
	}

	private JwtSecurityToken GenerateToken(TimeSpan expiration, IEnumerable<Claim> claims)
	{
		var token = new JwtSecurityToken(
			issuer: configuration.Issuer,
			audience: configuration.Audience,
			claims: claims,
			expires: DateTime.UtcNow.Add(expiration),
			signingCredentials: new SigningCredentials(configuration.SecurityKey, SecurityAlgorithms.HmacSha256));
		return token;
	}
}