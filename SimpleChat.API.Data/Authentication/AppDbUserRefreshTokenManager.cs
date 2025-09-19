using Microsoft.EntityFrameworkCore;
using SimpleChat.API.Services.Authentication;

namespace SimpleChat.API.Data.Authentication;

public sealed class AppDbUserRefreshTokenManager : IUserRefreshTokenManager
{
	public AppDbUserRefreshTokenManager(IDbContextFactory<AppDbContext> dbContextFactory, RefreshTokenConfiguration refreshTokenConfiguration)
	{
		_dbContextFactory = dbContextFactory;
		_refreshTokenConfiguration = refreshTokenConfiguration;
	}

	public async Task<bool> CanRefreshAccessToken(int userId, string refreshToken, CancellationToken cancellationToken = default)
	{
		await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
		var user = await dbContext.Users
			.AsNoTracking()
			.Include(user => user.Session)
			.SingleAsync(user => user.Id == userId, cancellationToken);
		return user.Session?.RefreshToken == refreshToken;
	}

	public async Task DelayRefreshTokenExpiration(int userId, CancellationToken cancellationToken = default)
	{
		await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
		var user = await dbContext.Users.Include(user => user.Session).SingleAsync(user => user.Id == userId, cancellationToken: cancellationToken);
		if (user.Session == null)
			throw new ArgumentException("User has no active session");
		if (user.Session.RefreshTokenExpirationTimestamp > DateTime.Now)
			throw new ArgumentException("Refresh token has expired");
		user.Session.RefreshTokenExpirationTimestamp = GetRefreshTokenExpirationTimestamp();
	}

	private readonly IDbContextFactory<AppDbContext> _dbContextFactory;
	private readonly RefreshTokenConfiguration _refreshTokenConfiguration;

	private DateTime GetRefreshTokenExpirationTimestamp()
	{
		return DateTime.Now + _refreshTokenConfiguration.ExpirationTime;
	}
}