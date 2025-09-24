using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using SimpleChat.API.Services;

namespace SimpleChat.API;

[Authorize]
public sealed class ConnectionStatusHub(IConnectionStatusManager connectionStatusManager) : Hub
{
	public override Task OnConnectedAsync()
	{
		var userId = GetCurrentClientUserId();
		connectionStatusManager.SetConnectionStatus(userId, true);
		return Clients.Others.SendAsync("UserConnected", userId);
	}

	public override Task OnDisconnectedAsync(Exception? exception)
	{
		var userId = GetCurrentClientUserId();
		connectionStatusManager.SetConnectionStatus(userId, false);
		return Clients.Others.SendAsync("UserDisconnected", userId);
	}

	private int GetCurrentClientUserId()
	{
		if (Context.UserIdentifier == null)
			throw new NullReferenceException($"{nameof(Context.UserIdentifier)} is not set");
		return int.Parse(Context.UserIdentifier);
	}
}