using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Collections;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using SimpleChat.API.Contracts;
using SimpleChat.Avalonia.Communication.Direct;

namespace SimpleChat.Avalonia.Communication;

public sealed partial class ChatsBrowserViewModel(APIClient apiClient, HubsClient hubsClient) : ViewModel, IChatsBrowserDataContext
{
	public IReadOnlyCollection<IChatItemDataContext> Chats => _chats;
	[ObservableProperty] public partial IChatItemDataContext? SelectedChat { get; set; }

	public async Task InitializeAsync(CancellationToken cancellationToken = default)
	{
		var users = await apiClient.GetUsersAsync(cancellationToken);
		foreach (var userInfo in users)
		{
			var chat = ToChat(userInfo);
			_chats.Add(chat);
			_chatsById.Add(userInfo.Id, chat);
		}
		await hubsClient.InitializeAsync(cancellationToken);
		var synchronizationContext = new AvaloniaSynchronizationContext();
		hubsClient.UserConnected
			.Select(GetOrCreateChatAsync)
			.Concat()
			.ObserveOn(synchronizationContext)
			.Subscribe(chat => chat.IsOnline = true);
		hubsClient.UserDisconnected
			.Select(GetOrCreateChatAsync)
			.Concat()
			.ObserveOn(synchronizationContext)
			.Subscribe(chat => chat.IsOnline = false);
		hubsClient.DirectMessageReceived
			.Select(notification => (notification, chat: _chatsById[notification.SenderId]))
			.ObserveOn(synchronizationContext)
			.Subscribe(tuple => tuple.chat.AddMessage(tuple.notification));
	}

	private readonly AvaloniaList<DirectChatViewModel> _chats = new();
	private readonly Dictionary<int, DirectChatViewModel> _chatsById = new();

	private DirectChatViewModel ToChat(UserInfoResponse userInfo)
	{
		return new DirectChatViewModel(apiClient)
		{
			UserId = userInfo.Id,
			Name = userInfo.Name,
			IsOnline = userInfo.IsOnline
		};
	}

	private async Task<DirectChatViewModel> GetOrCreateChatAsync(int userId)
	{
		if (_chatsById.TryGetValue(userId, out var existingChat))
			return existingChat;
		var userInfo = await apiClient.GetUserAsync(userId);
		var newChat = ToChat(userInfo);
		_chats.Add(newChat);
		_chatsById.Add(userId, newChat);
		return newChat;
	}
}