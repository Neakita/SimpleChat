using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace SimpleChat.API;

[Authorize]
public sealed class ConnectionNotificationsHub : Hub
{
	public override Task OnConnectedAsync()
	{
		var userId = GetCurrentClientUserId();
		return Clients.Others.SendAsync("UserConnected", userId);
	}

	public override Task OnDisconnectedAsync(Exception? exception)
	{
		var userId = GetCurrentClientUserId();
		return Clients.Others.SendAsync("UserDisconnected", userId);
	}

	private int GetCurrentClientUserId()
	{
		if (Context.UserIdentifier == null)
			throw new NullReferenceException($"{nameof(Context.UserIdentifier)} is not set");
		return int.Parse(Context.UserIdentifier);
	}
}