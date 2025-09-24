using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;

namespace SimpleChat.API.Authentication;

public sealed class JWTGenerator(JWTGeneratorConfiguration configuration)
{
	public JwtSecurityToken GenerateToken(params IEnumerable<Claim> claims)
	{
		var token = new JwtSecurityToken(
			claims: claims,
			expires: DateTime.UtcNow.Add(configuration.TokenExpiration),
			signingCredentials: new SigningCredentials(configuration.SecurityKey, SecurityAlgorithms.HmacSha256));
		return token;
	}
}