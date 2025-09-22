using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using SimpleChat.API.Contracts;
using SimpleChat.API.Services;

namespace SimpleChat.API;

[Authorize]
public sealed class DirectCommunicationHub(IDirectMessagePersister messagePersister) : Hub
{
	public override Task OnConnectedAsync()
	{
		// User could possibly have multiple connections via multiple app instances
		// https://code-maze.com/how-to-send-client-specific-messages-using-signalr/
		return AddCurrentClientToItsOwnGroupAsync();
	}

	public Task SendMessage(SendDirectMessageRequest request)
	{
		var receiver = GetReceiverGroup(request.ReceiverId);
		var notification = CreateNotification(request);
		return Task.WhenAll(
			PersistMessageAsync(request),
			receiver.SendAsync("ReceiveMessage", notification));
	}

	private Task AddCurrentClientToItsOwnGroupAsync()
	{
		var groupId = GetCurrentUserGroup();
		return Groups.AddToGroupAsync(Context.ConnectionId, groupId);
	}

	private IClientProxy GetReceiverGroup(int receiverId)
	{
		var groupName = GetGroupName(receiverId);
		return Clients.Group(groupName);
	}

	private string GetCurrentUserGroup()
	{
		var groupId = GetGroupName(GetCurrentUserId());
		return groupId;
	}

	private static string GetGroupName(int userId)
	{
		return $"user{userId}";
	}

	private DirectMessageNotification CreateNotification(SendDirectMessageRequest request)
	{
		var senderId = GetCurrentUserId();
		return new DirectMessageNotification
		{
			SenderId = senderId,
			Timestamp = request.Timestamp,
			Content = request.Content
		};
	}

	private int GetCurrentUserId()
	{
		if (Context.UserIdentifier == null)
			throw new NullReferenceException($"{nameof(Context.UserIdentifier)} is not set");
		return int.Parse(Context.UserIdentifier);
	}

	private Task PersistMessageAsync(SendDirectMessageRequest request)
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