namespace SimpleChat.API.Services;

public interface IConnectionStatusManager
{
	Task SetConnectionStatusAsync(int userId, bool isOnline, CancellationToken cancellationToken = default);
}