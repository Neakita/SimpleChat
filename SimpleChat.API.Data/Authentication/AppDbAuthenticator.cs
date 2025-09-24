using Microsoft.EntityFrameworkCore;
using SimpleChat.API.Data.Model;
using SimpleChat.API.Services.Authentication;

namespace SimpleChat.API.Data.Authentication;

public sealed class AppDbAuthenticator : IAuthenticator
{
	public AppDbAuthenticator(IDbContextFactory<AppDbContext> dbContextFactory, IPasswordHasher passwordHasher, RefreshTokenConfiguration refreshTokenConfiguration)
	{
		_dbContextFactory = dbContextFactory;
		_passwordHasher = passwordHasher;
		_refreshTokenConfiguration = refreshTokenConfiguration;
	}

	public async Task<bool> GetIsUserNameTakenAsync(string name, CancellationToken cancellationToken)
	{
		await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
		return await dbContext.Users.AnyAsync(user => user.Name == name, cancellationToken);
	}

	public async Task<RegistrationResult> RegisterAsync(string name, string password, CancellationToken cancellationToken)
	{
		var user = new User
		{
			Name = name,
			PasswordHash = _passwordHasher.GetPasswordHash(password)
		};
		await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
		await dbContext.Users.AddAsync(user, cancellationToken);
		await dbContext.SaveChangesAsync(cancellationToken);
		return new RegistrationResult
		{
			UserId = user.Id
		};
	}

	public async Task<LoginResult?> LoginAsync(string name, string password, CancellationToken cancellationToken)
	{
		await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
		var user = await GetUserAsync(dbContext, name, cancellationToken);
		if (user == null || !IsPasswordCorrect(user, password))
			return null;
		var session = CreateSession();
		user.Session = session;
		return new LoginResult
		{
			UserId = user.Id,
			RefreshToken = session.RefreshToken,
			RefreshTokenExpirationTimestamp = session.RefreshTokenExpirationTimestamp
		};
	}

	private readonly IDbContextFactory<AppDbContext> _dbContextFactory;
	private readonly IPasswordHasher _passwordHasher;
	private readonly RefreshTokenConfiguration _refreshTokenConfiguration;

	private static async Task<User?> GetUserAsync(AppDbContext dbContext, string login, CancellationToken cancellationToken)
	{
		var user = await dbContext.Users
			.Include(user => user.Session)
			.SingleOrDefaultAsync(user => user.Name == login, cancellationToken);
		return user;
	}

	private bool IsPasswordCorrect(User user, string password)
	{
		var providedPasswordHash = _passwordHasher.GetPasswordHash(password);
		return providedPasswordHash == user.PasswordHash;
	}

	private UserSession CreateSession()
	{
		return new UserSession
		{
			RefreshToken = RefreshTokenGenerator.GenerateToken(),
			RefreshTokenExpirationTimestamp = GetRefreshTokenExpirationTimestamp()
		};
	}

	private DateTime GetRefreshTokenExpirationTimestamp()
	{
		return DateTime.Now + _refreshTokenConfiguration.ExpirationTime;
	}
}