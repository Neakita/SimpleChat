namespace SimpleChat.API.Services;

public interface IUsersProvider
{
	Task<IEnumerable<UserInfo>> GetUsers(CancellationToken cancellationToken = default);
}