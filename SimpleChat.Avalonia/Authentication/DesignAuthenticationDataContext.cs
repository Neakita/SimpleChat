using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace SimpleChat.Avalonia.Authentication;

public sealed partial class DesignAuthenticationDataContext : ViewModel, IAuthenticationDataContext
{
	public string Name { get; set; } = "Thomas";
	public string Password { get; set; } = "strong!password";
	[ObservableProperty] public partial string ErrorMessage { get; set; } = string.Empty;
	public ICommand LoginCommand => new RelayCommand(ToggleErrorMessage);
	public ICommand RegisterCommand => new RelayCommand(ToggleErrorMessage);

	private void ToggleErrorMessage()
	{
		ErrorMessage = string.IsNullOrEmpty(ErrorMessage) ? "Invalid credentials" : string.Empty;
	}
}