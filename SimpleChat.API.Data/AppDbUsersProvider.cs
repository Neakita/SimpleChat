using Microsoft.EntityFrameworkCore;
using SimpleChat.API.Services;

namespace SimpleChat.API.Data;

public sealed class AppDbUsersProvider(IDbContextFactory<AppDbContext> dbContextFactory) : IUsersProvider
{
	public async Task<IEnumerable<UserInfo>> GetUsers(CancellationToken cancellationToken = default)
	{
		await using var dbContext = await dbContextFactory.CreateDbContextAsync(cancellationToken);
		return await dbContext.Users
			.Select(user => new UserInfo
			{
				Id = user.Id,
				Name = user.Name
			})
			.ToListAsync(cancellationToken);
	}
}