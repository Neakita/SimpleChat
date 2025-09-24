using System.Collections.Generic;
using System.Windows.Input;

namespace SimpleChat.Avalonia.Communication.Direct;

public interface IDirectChatDataContext : IChatItemDataContext
{
	IReadOnlyCollection<IDirectMessageDataContext> Messages { get; }
	string Message { get; set; }

	ICommand SendMessageCommand { get; }
	ICommand LoadMoreMessagesCommand { get; }
}