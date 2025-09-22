using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimpleChat.API.Contracts;
using SimpleChat.API.Services;

namespace SimpleChat.API;

[ApiController]
[Route("api")]
[Authorize]
public sealed class DirectMessagesController(IDirectMessagesProvider messagesProvider) : ControllerBase
{
	[HttpGet]
	[Route("/users/{userId:int}/messages")]
	public async Task<IActionResult> GetMessages(int userId, int? cursor)
	{
		var currentUserId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
		if (currentUserId == null)
			throw new NullReferenceException("User id is not set");
		var lastMessages = await messagesProvider.GetLastMessages(int.Parse(currentUserId), userId, cursor);
		var response = lastMessages.Select(ToResponse);
		return Ok(response);
	}

	private static DirectMessageResponse ToResponse(DirectMessageInfo info)
	{
		return new DirectMessageResponse
		{
			Id = info.Id,
			SenderId = info.SenderId,
			ReceiverId = info.ReceiverId,
			Timestamp = info.Timestamp,
			Content = info.Content
		};
	}
}