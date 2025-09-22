namespace SimpleChat.API.Services;

public interface IDirectMessagePersister
{
	Task PersistMessageAsync(DirectMessageInfo messageInfo, CancellationToken cancellationToken = default);
}