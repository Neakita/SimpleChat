using System.Windows.Input;

namespace SimpleChat.Avalonia.Authentication;

public interface IAuthenticationDataContext
{
	string Name { get; set; }
	string Password { get; set; }
	string ErrorMessage { get; }
	
	ICommand LoginCommand { get; }
	ICommand RegisterCommand { get; }
}