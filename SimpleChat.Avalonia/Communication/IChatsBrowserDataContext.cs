using System.Collections.Generic;

namespace SimpleChat.Avalonia.Communication;

public interface IChatsBrowserDataContext
{
	IReadOnlyCollection<IChatItemDataContext> Chats { get; }
	IChatItemDataContext? SelectedChat { get; set; }
}