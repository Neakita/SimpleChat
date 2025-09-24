namespace SimpleChat.Avalonia.Communication;

public interface IChatItemDataContext
{
	string Name { get; }
	bool IsOnline { get; }
}