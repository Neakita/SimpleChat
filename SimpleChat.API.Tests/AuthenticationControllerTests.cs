using System.Security.Claims;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using NSubstitute;
using SimpleChat.API.Authentication;
using SimpleChat.API.Contracts;
using SimpleChat.API.Services.Authentication;

namespace SimpleChat.API.Tests;

public sealed class AuthenticationControllerTests
{
	[Fact]
	public async Task ShouldRegister()
	{
		var authenticator = Substitute.For<IAuthenticator>();
		var controller = CreateController(authenticator);
		const string name = "doesntmatter";
		const string password = "dontcare";
		var request = new AuthenticationRequest
		{
			Name = name,
			Password = password
		};
		var result = await controller.Register(request);
		result.Should().BeOfType<CreatedResult>();
		_ = authenticator.Received().RegisterAsync(name, password);
	}

	[Fact]
	public async Task ShouldNotRegisterWhenUserNameIsTaken()
	{
		var authenticator = Substitute.For<IAuthenticator>();
		authenticator.GetIsUserNameTakenAsync(Arg.Any<string>()).Returns(Task.FromResult(true));
		var controller = CreateController(authenticator);
		var request = new AuthenticationRequest
		{
			Name = "doesntmatter",
			Password = "dontcare"
		};
		var result = await controller.Register(request);
		result.Should().BeOfType<ConflictResult>();
		_ = authenticator.DidNotReceive().RegisterAsync(Arg.Any<string>(), Arg.Any<string>());
	}

	[Fact]
	public async Task ShouldLogin()
	{
		var authenticator = Substitute.For<IAuthenticator>();
		authenticator
			.LoginAsync(Arg.Any<string>(), Arg.Any<string>())
			.Returns(Task.FromResult(DefaultLoginResult));
		var controller = CreateController(authenticator);
		const string name = "doesntmatter";
		const string password = "dontcare";
		var request = new AuthenticationRequest
		{
			Name = name,
			Password = password
		};
		await controller.Login(request);
		_ = authenticator.Received().LoginAsync(name, password);
	}

	[Fact]
	public async Task LoginShouldReturnAuthenticationResponse()
	{
		var authenticator = Substitute.For<IAuthenticator>();
		authenticator
			.LoginAsync(Arg.Any<string>(), Arg.Any<string>())
			.Returns(Task.FromResult(DefaultLoginResult));
		var controller = CreateController(authenticator);
		var request = new AuthenticationRequest
		{
			Name = "doesntmatter",
			Password = "dontcare"
		};
		var result = await controller.Login(request);
		result.Should().BeOfType<OkObjectResult>().Which.Value.Should().BeOfType<AuthenticationResponse>();
	}

	[Fact]
	public async Task ShouldRefreshAccessToken()
	{
		const int userId = 123;
		const string refreshToken = "some";
		var refreshTokenManager = Substitute.For<IRefreshTokenManager>();
		refreshTokenManager.CanRefreshAccessToken(userId, refreshToken).Returns(true);
		var controller = CreateController(refreshTokenManager: refreshTokenManager);
		var userIdClaim = new Claim(ClaimTypes.NameIdentifier, userId.ToString());
		var identity = new ClaimsIdentity([userIdClaim]);
		var principal = new ClaimsPrincipal(identity);
		controller.ControllerContext = new ControllerContext
		{
			HttpContext = new DefaultHttpContext
			{
				User = principal
			}
		};
		var result = await controller.RefreshAccessToken(refreshToken);
		result.Should().BeOfType<OkObjectResult>().Which.Value.Should().BeOfType<RefreshAccessTokenResponse>();
	}

	private static readonly JWTGeneratorConfiguration JWTGeneratorDefaultConfiguration = new()
	{
		TokenExpiration = TimeSpan.FromMinutes(30),
		SecurityKey = new SymmetricSecurityKey("super-duper-hyper-top-secret-security-key"u8.ToArray())
	};

	private static readonly LoginResult DefaultLoginResult = new()
	{
		UserId = 123,
		RefreshToken = "refresh-token",
		RefreshTokenExpirationTimestamp = DateTime.Now
	};

	private static AuthenticationController CreateController(IAuthenticator? authenticator = null, JWTGenerator? jwtGenerator = null, IRefreshTokenManager? refreshTokenManager = null)
	{
		authenticator ??= Substitute.For<IAuthenticator>();
		jwtGenerator ??= new JWTGenerator(JWTGeneratorDefaultConfiguration);
		refreshTokenManager ??= Substitute.For<IRefreshTokenManager>();
		return new AuthenticationController(authenticator, jwtGenerator, refreshTokenManager);
	}
}