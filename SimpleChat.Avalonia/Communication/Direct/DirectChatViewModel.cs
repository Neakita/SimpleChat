using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia.Collections;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SimpleChat.API.Contracts;

namespace SimpleChat.Avalonia.Communication.Direct;

public sealed partial class DirectChatViewModel(APIClient apiClient) : ViewModel, IDirectChatDataContext
{
	public string Name { get; init; } = string.Empty;
	public int UserId { get; init; }
	[ObservableProperty] public partial bool IsOnline { get; set; }
	public IReadOnlyCollection<IDirectMessageDataContext> Messages => _messages;

	[ObservableProperty, NotifyCanExecuteChangedFor(nameof(SendMessageCommand))]
	public partial string Message { get; set; } = string.Empty;

	ICommand IDirectChatDataContext.SendMessageCommand => SendMessageCommand;
	ICommand IDirectChatDataContext.LoadMoreMessagesCommand => LoadMoreMessagesCommand;

	public void AddMessage(DirectMessageNotification notification)
	{
		var message = new DirectMessageViewModel
		{
			Id = notification.Id,
			Content = notification.Content,
			Type = MessageType.Incoming,
			Timestamp = notification.Timestamp
		};
		_messages.Add(message);
	}

	private readonly AvaloniaList<DirectMessageViewModel> _messages = new();
	private bool _isAllMessagesLoaded;
	private bool CanLoadMoreMessages => !_isAllMessagesLoaded;

	private bool CanSendMessage()
	{
		return !string.IsNullOrWhiteSpace(Message);
	}

	[RelayCommand(CanExecute = nameof(CanSendMessage))]
	private async Task SendMessageAsync()
	{
		var timestamp = DateTime.Now;
		var request = new SendDirectMessageRequest
		{
			ReceiverId = UserId,
			Timestamp = timestamp,
			Content = Message
		};
		var messageId = await apiClient.SendMessageAsync(request);
		var messageViewModel = new DirectMessageViewModel
		{
			Id = messageId,
			Content = Message,
			Type = MessageType.Outgoing,
			Timestamp = timestamp
		};
		_messages.Add(messageViewModel);
		Message = string.Empty;
	}

	[RelayCommand(CanExecute = nameof(CanLoadMoreMessages))]
	private async Task LoadMoreMessagesAsync(CancellationToken cancellationToken)
	{
		var messages = await apiClient.GetMessagesAsync(UserId, _messages.FirstOrDefault()?.Id, cancellationToken);
		var viewModels = messages.Select(ToViewModel).ToList();
		if (viewModels.Count == 0)
			_isAllMessagesLoaded = true;
		_messages.InsertRange(0, viewModels);
	}

	private DirectMessageViewModel ToViewModel(DirectMessageResponse response)
	{
		return new DirectMessageViewModel
		{
			Id = response.Id,
			Content = response.Content,
			Type = GetMessageType(response),
			Timestamp = response.Timestamp
		};
	}

	private MessageType GetMessageType(DirectMessageResponse response)
	{
		if (response.SenderId == UserId)
			return MessageType.Incoming;
		return MessageType.Outgoing;
	}
}