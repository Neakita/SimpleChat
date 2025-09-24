using Microsoft.EntityFrameworkCore;
using SimpleChat.API.Services;

namespace SimpleChat.API.Data;

public sealed class AppDbConnectionStatusManager(IDbContextFactory<AppDbContext> dbContextFactory) : IConnectionStatusManager
{
	public async Task SetConnectionStatusAsync(int userId, bool isOnline, CancellationToken cancellationToken = default)
	{
		await using var dbContext = await dbContextFactory.CreateDbContextAsync(cancellationToken);
		var user = dbContext.Users.Single(user => user.Id == userId);
		user.IsOnline = isOnline;
		await dbContext.SaveChangesAsync(cancellationToken);
	}
}