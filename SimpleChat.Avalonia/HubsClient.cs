using System;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using SimpleChat.API.Contracts;

namespace SimpleChat.Avalonia;

public sealed class HubsClient : IAsyncDisposable
{
	public IObservable<int> UserConnected => _userConnected;
	public IObservable<int> UserDisconnected => _userDisconnected;
	public IObservable<DirectMessageNotification> DirectMessageReceived => _directMessageReceived;

	public HubsClient(string serverUrl, APIClient apiClient)
	{
		_apiClient = apiClient;
		_notificationsConnection = ConnectToHub(serverUrl, "notifications");
		_notificationsConnection.On<int>("UserConnected", userId => _userConnected.OnNext(userId));
		_notificationsConnection.On<int>("UserDisconnected", userId => _userDisconnected.OnNext(userId));
		
		_directCommunicationConnection = ConnectToHub(serverUrl, "direct");
		_directCommunicationConnection.On<DirectMessageNotification>("ReceiveMessage", notification => _directMessageReceived.OnNext(notification));
	}

	public async Task InitializeAsync(CancellationToken cancellationToken = default)
	{
		if (_isInitialized)
			return;
		await Task.WhenAll(
			_notificationsConnection.StartAsync(cancellationToken),
			_directCommunicationConnection.StartAsync(cancellationToken));
		_isInitialized = true;
	}

	private HubConnection ConnectToHub(string serverUrl, string hubRoute)
	{
		var url = $"{serverUrl}/hubs/{hubRoute}";
		return new HubConnectionBuilder()
			.WithUrl(url, options => options.AccessTokenProvider = GetAccessToken!)
			.Build();
	}

	private Task<string> GetAccessToken()
	{
		return _apiClient.GetAccessTokenAsync();
	}

	private readonly APIClient _apiClient;
	private readonly Subject<int> _userConnected = new();
	private readonly Subject<int> _userDisconnected = new();
	private readonly Subject<DirectMessageNotification> _directMessageReceived = new();
	private readonly HubConnection _notificationsConnection;
	private readonly HubConnection _directCommunicationConnection;
	private bool _isInitialized;

	public async ValueTask DisposeAsync()
	{
		_userConnected.Dispose();
		_userDisconnected.Dispose();
		_directMessageReceived.Dispose();
		await _notificationsConnection.DisposeAsync();
		await _directCommunicationConnection.DisposeAsync();
	}
}