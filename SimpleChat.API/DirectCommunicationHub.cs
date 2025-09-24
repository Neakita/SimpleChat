using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace SimpleChat.API;

[Authorize]
public sealed class DirectCommunicationHub : Hub
{
	public static string GetGroupName(int userId)
	{
		return $"user{userId}";
	}

	public override Task OnConnectedAsync()
	{
		// User could possibly have multiple connections via multiple app instances
		// https://code-maze.com/how-to-send-client-specific-messages-using-signalr/
		return AddCurrentClientToItsOwnGroupAsync();
	}

	private Task AddCurrentClientToItsOwnGroupAsync()
	{
		var groupId = GetCurrentUserGroup();
		return Groups.AddToGroupAsync(Context.ConnectionId, groupId);
	}

	private string GetCurrentUserGroup()
	{
		var groupId = GetGroupName(GetCurrentUserId());
		return groupId;
	}

	private int GetCurrentUserId()
	{
		if (Context.UserIdentifier == null)
			throw new NullReferenceException($"{nameof(Context.UserIdentifier)} is not set");
		return int.Parse(Context.UserIdentifier);
	}
}