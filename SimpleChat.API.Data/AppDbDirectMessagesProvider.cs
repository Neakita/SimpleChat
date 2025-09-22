using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using SimpleChat.API.Data.Model;
using SimpleChat.API.Services;

namespace SimpleChat.API.Data;

public sealed class AppDbDirectMessagesProvider(IDbContextFactory<AppDbContext> dbContextFactory) : IDirectMessagesProvider
{
	private const int PageSize = 20;

	public async Task<IEnumerable<DirectMessageInfo>> GetLastMessages(int firstUserId, int secondUserId, int? paginationCursor)
	{
		await using var dbContext = await dbContextFactory.CreateDbContextAsync();
		var query = dbContext.DirectMessages
			.AsNoTracking()
			.Where(IsConversationBetween(firstUserId, secondUserId));

		if (paginationCursor.HasValue)
			query = query.Where(message => message.Id < paginationCursor);

		query = query
			.OrderBy(message => message.Id)
			.TakeLast(PageSize);

		return await query
			.Select(message => new DirectMessageInfo
			{
				SenderId = message.SenderId,
				ReceiverId = message.ReceiverId,
				Timestamp = message.Timestamp,
				Content = message.Content
			}).ToListAsync();
	}

	private static Expression<Func<DirectMessage, bool>> IsConversationBetween(int firstUserId, int secondUserId) =>
		message => (message.SenderId == firstUserId && message.ReceiverId == secondUserId) ||
		           (message.SenderId == secondUserId && message.ReceiverId == firstUserId);
}