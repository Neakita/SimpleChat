namespace SimpleChat.API.Services;

public interface IDirectMessagePersister
{
	Task<int> PersistMessageAsync(DirectMessageInfo messageInfo, CancellationToken cancellationToken = default);
}