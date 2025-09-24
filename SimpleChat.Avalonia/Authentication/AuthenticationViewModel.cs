using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SimpleChat.API.Contracts;
using SimpleChat.Avalonia.Communication;

namespace SimpleChat.Avalonia.Authentication;

public sealed partial class AuthenticationViewModel(APIClient apiClient, HubsClient hubsClient, IPresentationManager presentationManager) : ViewModel, IAuthenticationDataContext
{
	[ObservableProperty, NotifyCanExecuteChangedFor(nameof(RegisterCommand), nameof(LoginCommand))]
	public partial string Name { get; set; } = string.Empty;

	[ObservableProperty, NotifyCanExecuteChangedFor(nameof(RegisterCommand), nameof(LoginCommand))]
	public partial string Password { get; set; } = string.Empty;

	[ObservableProperty] public partial string ErrorMessage { get; set; } = string.Empty;

	ICommand IAuthenticationDataContext.LoginCommand => LoginCommand;
	ICommand IAuthenticationDataContext.RegisterCommand => RegisterCommand;

	private bool IsCredentialsNotEmpty => !string.IsNullOrEmpty(Name) && !string.IsNullOrEmpty(Password);

	[RelayCommand(CanExecute = nameof(IsCredentialsNotEmpty))]
	private async Task Register()
	{
		ErrorMessage = string.Empty;
		var request = new AuthenticationRequest
		{
			Name = Name,
			Password = Password
		};
		try
		{
			await apiClient.RegisterAsync(request);
			await apiClient.LoginAsync(request);
		}
		catch (Exception exception)
		{
			ErrorMessage = exception.Message;
			return;
		}
		await PresentChatsBrowserAsync();
	}

	[RelayCommand(CanExecute = nameof(IsCredentialsNotEmpty))]
	private async Task Login()
	{
		ErrorMessage = string.Empty;
		var request = new AuthenticationRequest
		{
			Name = Name,
			Password = Password
		};
		try
		{
			await apiClient.LoginAsync(request);
		}
		catch (Exception exception)
		{
			ErrorMessage = exception.Message;
			return;
		}
		await PresentChatsBrowserAsync();
	}

	private async Task PresentChatsBrowserAsync(CancellationToken cancellationToken = default)
	{
		var chatsBrowser = new ChatsBrowserViewModel(apiClient, hubsClient);
		await chatsBrowser.InitializeAsync(cancellationToken);
		presentationManager.DataContext = chatsBrowser;
	}
}