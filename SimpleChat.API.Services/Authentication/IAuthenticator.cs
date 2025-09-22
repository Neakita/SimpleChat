namespace SimpleChat.API.Services.Authentication;

public interface IAuthenticator
{
	Task<bool> GetIsUserNameTakenAsync(string name, CancellationToken cancellationToken = default);
	Task<RegistrationResult> RegisterAsync(string name, string password, CancellationToken cancellationToken = default);
	Task<LoginResult> LoginAsync(string name, string password, CancellationToken cancellationToken = default);
}