using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using SimpleChat.API.Contracts;

namespace SimpleChat.Avalonia;

public sealed class APIClient : IDisposable
{
	public string RefreshToken { get; set; } = string.Empty;

	public APIClient(string serverUri)
	{
		_serverUri = serverUri;
		// Handle SSL certificate validation (for development)
		var handler = new HttpClientHandler
		{
			ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
		};
		_httpClient = new HttpClient(handler);
	}

	public async Task RegisterAsync(AuthenticationRequest request, CancellationToken cancellationToken = default)
	{
		var uri = $"{_serverUri}/api/auth/register";
		var requestTypeInfo = ContractsSourceGenerationContext.Default.AuthenticationRequest;
		var httpResponse = await _httpClient.PostAsJsonAsync(uri, request, requestTypeInfo, cancellationToken);
		if (!httpResponse.IsSuccessStatusCode)
		{
			var responseText = await httpResponse.Content.ReadAsStringAsync(cancellationToken);
			throw new Exception(responseText);
		}
	}

	public async Task<AuthenticationResponse> LoginAsync(AuthenticationRequest request, CancellationToken cancellationToken = default)
	{
		var uri = $"{_serverUri}/api/auth/login";
		var requestTypeInfo = ContractsSourceGenerationContext.Default.AuthenticationRequest;
		var httpResponse = await _httpClient.PostAsJsonAsync(uri, request, requestTypeInfo, cancellationToken);
		if (!httpResponse.IsSuccessStatusCode)
		{
			var responseText = await httpResponse.Content.ReadAsStringAsync(cancellationToken);
			throw new Exception(responseText);
		}
		var responseTypeInfo = ContractsSourceGenerationContext.Default.AuthenticationResponse;
		var response = await httpResponse.Content.ReadFromJsonAsync(responseTypeInfo, cancellationToken);
		if (response == null)
			throw new NullReferenceException("Unable to get authentication response from API");
		RefreshToken = response.RefreshToken;
		_accessToken = response.AccessToken;
		_accessTokenExpirationTimestamp = response.AccessTokenExpirationTimestamp;
		_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", response.AccessToken);
		_currentUserId = response.UserId;
		return response;
	}

	public async Task<IEnumerable<UserInfoResponse>> GetUsersAsync(CancellationToken cancellationToken = default)
	{
		await EnsureAccessAsync(cancellationToken);
		var uri = $"{_serverUri}/api/users";
		var responseTypeInfo = ContractsSourceGenerationContext.Default.IEnumerableUserInfoResponse;
		var users = await _httpClient.GetFromJsonAsync(uri, responseTypeInfo, cancellationToken);
		if (users == null)
			throw new NullReferenceException("Unable to get users from API");
		return users.Where(user => user.Id != _currentUserId);
	}

	public async Task<UserInfoResponse> GetUserAsync(int id, CancellationToken cancellationToken = default)
	{
		await EnsureAccessAsync(cancellationToken);
		var uri = $"{_serverUri}/api/users/{id}";
		var responseTypeInfo = ContractsSourceGenerationContext.Default.UserInfoResponse;
		var response = await _httpClient.GetFromJsonAsync(uri, responseTypeInfo, cancellationToken);
		if (response == null)
			throw new NullReferenceException("Unable to get user from API");
		return response;
	}

	public async Task<IEnumerable<DirectMessageResponse>> GetMessagesAsync(int userId, int? cursor = null, CancellationToken cancellationToken = default)
	{
		await EnsureAccessAsync(cancellationToken);
		var uri = $"{_serverUri}/api/users/{userId}/messages";
		if (cursor.HasValue)
			uri += $"?cursor={cursor}";
		var responseTypeInfo = ContractsSourceGenerationContext.Default.IEnumerableDirectMessageResponse;
		var messages = await _httpClient.GetFromJsonAsync(uri, responseTypeInfo, cancellationToken);
		if (messages == null)
			throw new NullReferenceException("Unable to get messages from API");
		return messages;
	}

	public async Task<string> GetAccessTokenAsync(CancellationToken cancellationToken = default)
	{
		await EnsureAccessAsync(cancellationToken);
		return _accessToken;
	}

	public async Task<int> SendMessageAsync(SendDirectMessageRequest request, CancellationToken cancellationToken = default)
	{
		await EnsureAccessAsync(cancellationToken);
		var uri = $"{_serverUri}/api/send-direct";
		var requestTypeInfo = ContractsSourceGenerationContext.Default.SendDirectMessageRequest;
		var httpResponse = await _httpClient.PostAsJsonAsync(uri, request, requestTypeInfo, cancellationToken);
		httpResponse.EnsureSuccessStatusCode();
		var response = await httpResponse.Content.ReadAsStringAsync(cancellationToken);
		return int.Parse(response);
	}

	public void Dispose()
	{
		_httpClient.Dispose();
	}

	private readonly HttpClient _httpClient;
	private readonly string _serverUri;
	private string _accessToken = string.Empty;
	private DateTime? _accessTokenExpirationTimestamp;
	private int _currentUserId;

	private Task EnsureAccessAsync(CancellationToken cancellationToken)
	{
		var tolerance = TimeSpan.FromMinutes(1);
		var considerAccessTokenExpired = _accessTokenExpirationTimestamp - tolerance;
		if (DateTime.UtcNow > considerAccessTokenExpired)
			return RefreshAccessTokenAsync(cancellationToken);
		return Task.CompletedTask;
	}

	private async Task RefreshAccessTokenAsync(CancellationToken cancellationToken)
	{
		var uri = Path.Combine(_serverUri, "api", "auth", "refresh");
		uri += $"?refreshToken={RefreshToken}";
		var httpResponse = await _httpClient.GetAsync(uri, cancellationToken);
		httpResponse.EnsureSuccessStatusCode();
		var responseTypeInfo = ContractsSourceGenerationContext.Default.RefreshAccessTokenResponse;
		var response = await httpResponse.Content.ReadFromJsonAsync(responseTypeInfo, cancellationToken);
		if (response == null)
			throw new NullReferenceException("Unable to parse response as JSON");
		_accessToken = response.Token;
		_accessTokenExpirationTimestamp = response.ExpirationTimestamp;
		_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", response.Token);
	}
}