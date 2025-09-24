using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using SimpleChat.API.Contracts;
using SimpleChat.API.Services.Authentication;

namespace SimpleChat.API.Authentication;

[ApiController]
[Route("api/auth")]
public class AuthenticationController(IAuthenticator authenticator, JWTGenerator jwtGenerator, IRefreshTokenManager refreshTokenManager) : ControllerBase
{
	[Route("register")]
	public async Task<IActionResult> Register(AuthenticationRequest request)
	{
		var isNameTaken = await authenticator.GetIsUserNameTakenAsync(request.Name);
		if (isNameTaken)
			return Conflict("The name is already taken");
		await authenticator.RegisterAsync(request.Name, request.Password);
		return Created();
	}

	[Route("login")]
	public async Task<IActionResult> Login(AuthenticationRequest request)
	{
		var result = await authenticator.LoginAsync(request.Name, request.Password);
		if (result == null)
			return BadRequest("Invalid credentials");
		var idClaim = new Claim(ClaimTypes.NameIdentifier, result.UserId.ToString(CultureInfo.InvariantCulture));
		var accessToken = jwtGenerator.GenerateToken(idClaim);
		var tokenHandler = new JwtSecurityTokenHandler();
		var response = new AuthenticationResponse
		{
			UserId = result.UserId,
			AccessToken = tokenHandler.WriteToken(accessToken),
			RefreshToken = result.RefreshToken,
			AccessTokenExpirationTimestamp = accessToken.ValidTo,
		};
		return Ok(response);
	}

	[Route("refresh")]
	public async Task<IActionResult> RefreshAccessToken(string refreshToken)
	{
		var idClaim = HttpContext.User.Claims.SingleOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier);
		if (idClaim == null)
			return Unauthorized("Unable to get user id");
		var userId = int.Parse(idClaim.Value, CultureInfo.InvariantCulture);
		var canRefresh = await refreshTokenManager.CanRefreshAccessToken(userId, refreshToken);
		if (!canRefresh)
			return Unauthorized();
		await refreshTokenManager.DelayRefreshTokenExpiration(userId);
		var accessToken = jwtGenerator.GenerateToken(idClaim);
		var tokenHandler = new JwtSecurityTokenHandler();
		var response = new RefreshAccessTokenResponse
		{
			Token = tokenHandler.WriteToken(accessToken),
			ExpirationTimestamp = accessToken.ValidTo
		};
		return Ok(response);
	}
}