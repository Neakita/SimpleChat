using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace SimpleChat.Avalonia.Communication;

internal sealed class DesignChatsBrowserDataContext : IChatsBrowserDataContext
{
	public static DesignChatsBrowserDataContext WithChats => new()
	{
		Chats =
		[
			new DesignChatDataContext("Sam", true),
			new DesignChatDataContext("Александр Абдулов", true),
			new DesignChatDataContext("Thomas", false)
		]
	};

	public static DesignChatsBrowserDataContext WithoutChats => new();

	public IReadOnlyCollection<IChatItemDataContext> Chats { get; set; } = ReadOnlyCollection<IChatItemDataContext>.Empty;

	public IChatItemDataContext? SelectedChat { get; set; }
}