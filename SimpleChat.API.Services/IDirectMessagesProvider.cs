namespace SimpleChat.API.Services;

public interface IDirectMessagesProvider
{
	Task<IEnumerable<DirectMessageInfo>> GetLastMessages(int firstUserId, int secondUserId, int? paginationCursor);
}