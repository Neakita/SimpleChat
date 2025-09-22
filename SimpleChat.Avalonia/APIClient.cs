using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using SimpleChat.API.Contracts;

namespace SimpleChat.Avalonia;

public sealed class APIClient
{
	public string RefreshToken { get; set; } = string.Empty;
	public string AccessToken { get; set; } = string.Empty;
	public DateTime AccessTokenExpirationTimestamp { get; set; }

	public APIClient()
	{
		// Handle SSL certificate validation (for development)
		var handler = new HttpClientHandler
		{
			ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
		};
		_httpClient = new HttpClient(handler);
	}

	public async Task RegisterAsync(AuthenticationRequest request, CancellationToken cancellationToken = default)
	{
		const string uri = "https://localhost:7210/api/auth/register";
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
		const string uri = "https://localhost:7210/api/auth/login";
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
			throw new NullReferenceException("Unable to parse response as JSON");
		RefreshToken = response.RefreshToken;
		AccessToken = response.AccessToken;
		AccessTokenExpirationTimestamp = response.AccessTokenExpirationTimestamp;
		return response;
	}

	private readonly HttpClient _httpClient;
}