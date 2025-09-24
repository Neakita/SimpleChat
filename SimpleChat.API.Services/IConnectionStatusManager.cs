namespace SimpleChat.API.Services;

public interface IConnectionStatusManager
{
	bool GetConnectionStatus(int userId);
	void SetConnectionStatus(int userId, bool isOnline);
}