using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimpleChat.API.Services;
using SimpleChat.API.Services.Authentication;

namespace SimpleChat.API.Authentication;

[ApiController]
[Route("api/[controller]")]
public class AuthenticationController(IAuthenticator authenticator, JWTGenerator jwtGenerator, IUserRefreshTokenManager refreshTokenManager) : ControllerBase
{
	[HttpPost]
	public async Task<IActionResult> Register(UserAuthenticationRequest request)
	{
		var isNameTaken = await authenticator.GetIsLoginTakenAsync(request.Login);
		if (isNameTaken)
			return Conflict();
		await authenticator.RegisterAsync(request.Login, request.Password);
		return Created();
	}

	[HttpGet]
	public async Task<IActionResult> Login(UserAuthenticationRequest request)
	{
		var result = await authenticator.LoginAsync(request.Login, request.Password);
		var idClaim = new Claim(ClaimTypes.NameIdentifier, result.UserId.ToString(CultureInfo.InvariantCulture));
		var accessToken = jwtGenerator.GenerateToken(idClaim);
		var tokenHandler = new JwtSecurityTokenHandler();
		var response = new AuthenticationResponse
		{
			AccessToken = tokenHandler.WriteToken(accessToken),
			RefreshToken = result.RefreshToken,
			AccessTokenExpirationTimestamp = accessToken.ValidTo,
		};
		return Ok(response);
	}

	[HttpPost]
	public async Task<IActionResult> RefreshAccessToken(string refreshToken)
	{
		var idClaim = HttpContext.User.Claims.Single(claim => claim.Type == ClaimTypes.NameIdentifier);
		var userId = int.Parse(idClaim.Value);
		var canRefresh = await refreshTokenManager.CanRefreshAccessToken(userId, refreshToken);
		if (!canRefresh)
			return Unauthorized();
		await refreshTokenManager.DelayRefreshTokenExpiration(userId);
		var accessToken = jwtGenerator.GenerateToken(idClaim);
		var tokenHandler = new JwtSecurityTokenHandler();
		var response = new RefreshAccessTokenResponse
		{
			Token = tokenHandler.WriteToken(accessToken),
			ExpirationTimestamp = accessToken.ValidTo,
		};
		return Ok(response);
	}
}