namespace SimpleChat.API.Services.Authentication;

public interface IAuthenticator
{
	Task<bool> GetIsLoginTakenAsync(string login, CancellationToken cancellationToken = default);
	Task<RegistrationResult> RegisterAsync(string login, string password, CancellationToken cancellationToken = default);
	Task<LoginResult> LoginAsync(string login, string password, CancellationToken cancellationToken = default);
}