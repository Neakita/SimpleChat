namespace SimpleChat.API.Services.Authentication;

public interface IUserRefreshTokenManager
{
	Task<bool> CanRefreshAccessToken(int userId, string refreshToken, CancellationToken cancellationToken = default);
	Task DelayRefreshTokenExpiration(int userId, CancellationToken cancellationToken = default);
}