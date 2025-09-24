namespace SimpleChat.Avalonia.Communication;

internal sealed class DesignChatDataContext(string name, bool isOnline) : IChatItemDataContext
{
	public static DesignChatDataContext OnlineInstance => new("Александр Абдулов", true);
	public static DesignChatDataContext OfflineInstance => new("Александр Абдулов", false);

	public string Name => name;
	public bool IsOnline => isOnline;
}