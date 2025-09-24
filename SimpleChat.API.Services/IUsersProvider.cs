namespace SimpleChat.API.Services;

public interface IUsersProvider
{
	Task<IEnumerable<UserInfo>> GetUsers(CancellationToken cancellationToken = default);
	Task<UserInfo?> GetUser(int id, CancellationToken cancellationToken = default);
}