using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;

namespace SimpleChat.Avalonia.Communication.Direct;

public sealed class DesignDirectChatDataContext : IDirectChatDataContext
{
	public static DesignDirectChatDataContext WithMessages => new()
	{
		Name = "Александр Абдулов",
		IsOnline = true,
		Messages =
		[
			new DesignDirectMessageDataContext
			{
				Content = "What is Lorem Ipsum?",
				Type = MessageType.Outgoing,
				Timestamp = DateTime.Now.AddMinutes(-115)
			},
			new DesignDirectMessageDataContext
			{
				Content = "Lorem Ipsum is simply dummy text of the printing and typesetting industry.",
				Type = MessageType.Incoming,
				Timestamp = DateTime.Now.AddMinutes(-113)
			},
			new DesignDirectMessageDataContext
			{
				Content = "Why do we use it?",
				Type = MessageType.Outgoing,
				Timestamp = DateTime.Now.AddMinutes(-112)
			},
			new DesignDirectMessageDataContext
			{
				Content =
					"It is a long established fact that a reader will be distracted by the readable content of a page when looking at its layout.",
				Type = MessageType.Incoming,
				Timestamp = DateTime.Now.AddMinutes(-110)
			},
			new DesignDirectMessageDataContext
			{
				Content = "Where does it come from?",
				Type = MessageType.Outgoing,
				Timestamp = DateTime.Now.AddMinutes(-108)
			},
			new DesignDirectMessageDataContext
			{
				Content =
					"Contrary to popular belief, Lorem Ipsum is not simply random text. It has roots in a piece of classical Latin literature from 45 BC, making it over 2000 years old.",
				Type = MessageType.Incoming,
				Timestamp = DateTime.Now.AddMinutes(-107)
			}
		]
	};

	public static DesignDirectChatDataContext WithoutMessages => new();

	public string Name { get; set; } = string.Empty;
	public bool IsOnline { get; set; }

	public IReadOnlyCollection<IDirectMessageDataContext> Messages { get; set; } = ReadOnlyCollection<IDirectMessageDataContext>.Empty;

	public string Message { get; set; } = string.Empty;

	public ICommand SendMessageCommand => new RelayCommand(() => { });

	public ICommand LoadMoreMessagesCommand => new RelayCommand(() => { });
}