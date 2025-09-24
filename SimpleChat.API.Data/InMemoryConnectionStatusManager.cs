using Microsoft.EntityFrameworkCore;
using SimpleChat.API.Services;

namespace SimpleChat.API.Data;

public sealed class InMemoryConnectionStatusManager : IConnectionStatusManager
{
	public bool GetConnectionStatus(int userId)
	{
		return _onlineUsersIds.Contains(userId);
	}

	public void SetConnectionStatus(int userId, bool isOnline)
	{
		if (isOnline)
			_onlineUsersIds.Add(userId);
		else
			_onlineUsersIds.Remove(userId);
	}

	private readonly HashSet<int> _onlineUsersIds = new();
}