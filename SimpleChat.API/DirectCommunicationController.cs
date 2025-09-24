using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using SimpleChat.API.Contracts;
using SimpleChat.API.Services;

namespace SimpleChat.API;

[ApiController]
[Route("api")]
[Authorize]
public sealed class DirectCommunicationController(IDirectMessagesProvider messagesProvider, IHubContext<DirectCommunicationHub> hubContext, IDirectMessagePersister messagePersister) : ControllerBase
{
	[HttpGet]
	[Route("users/{userId:int}/messages")]
	public async Task<IActionResult> GetMessages(int userId, int? cursor)
	{
		var currentUserId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
		if (currentUserId == null)
			throw new NullReferenceException("User id is not set");
		var lastMessages = await messagesProvider.GetLastMessages(int.Parse(currentUserId), userId, cursor);
		var response = lastMessages.Select(ToResponse);
		return Ok(response);
	}

	// This logically actually should be in DirectCommunicationHub, but it is here to be able return message id as a result 
	[HttpPost]
	[Route("send-direct")]
	public async Task<IActionResult> SendMessage(SendDirectMessageRequest request)
	{
		var receiver = GetReceiverGroup(request.ReceiverId);
		var messageId = await PersistMessageAsync(request);
		var notification = CreateNotification(messageId, request);
		await receiver.SendAsync("ReceiveMessage", notification);
		return Ok(messageId);
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

	private DirectMessageNotification CreateNotification(int messageId, SendDirectMessageRequest request)
	{
		var senderId = GetCurrentUserId();
		return new DirectMessageNotification
		{
			Id = messageId,
			SenderId = senderId,
			Timestamp = request.Timestamp,
			Content = request.Content
		};
	}

	private IClientProxy GetReceiverGroup(int receiverId)
	{
		var groupName = DirectCommunicationHub.GetGroupName(receiverId);
		return hubContext.Clients.Group(groupName);
	}

	private int GetCurrentUserId()
	{
		var identifier = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
		if (identifier == null)
			throw new NullReferenceException($"Claim {ClaimTypes.NameIdentifier} is not set");
		return int.Parse(identifier);
	}

	private Task<int> PersistMessageAsync(SendDirectMessageRequest request)
	{
		var dto = new DirectMessageInfo
		{
			SenderId = GetCurrentUserId(),
			ReceiverId = request.ReceiverId,
			Timestamp = request.Timestamp,
			Content = request.Content
		};
		return messagePersister.PersistMessageAsync(dto);
	}
}