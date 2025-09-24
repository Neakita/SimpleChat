using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimpleChat.API.Contracts;
using SimpleChat.API.Services;

namespace SimpleChat.API;

[ApiController]
[Route("api")]
[Authorize]
public sealed class UsersController(IUsersProvider provider) : ControllerBase
{
	[HttpGet]
	[Route("users")]
	public async Task<IActionResult> GetUsers()
	{
		var users = await provider.GetUsers();
		var response = users.Select(ToResponse);
		return Ok(response);
	}

	[HttpGet]
	[Route("users/{userId:int}")]
	public async Task<IActionResult> GetUsers(int userId)
	{
		var user = await provider.GetUser(userId);
		if (user == null)
			return BadRequest();
		var response = ToResponse(user);
		return Ok(response);
	}

	private static UserInfoResponse ToResponse(UserInfo user)
	{
		return new UserInfoResponse
		{
			Id = user.Id,
			Name = user.Name,
			IsOnline = user.IsOnline
		};
	}
}