using Microsoft.EntityFrameworkCore;
using SimpleChat.API.Data.Model;
using SimpleChat.API.Services;

namespace SimpleChat.API.Data;

public sealed class AppDbDirectMessagePersister(IDbContextFactory<AppDbContext> dbContextFactory) : IDirectMessagePersister
{
	public async Task<int> PersistMessageAsync(DirectMessageInfo messageInfo, CancellationToken cancellationToken = default)
	{
		var messageEntity = new DirectMessage
		{
			SenderId = messageInfo.SenderId,
			ReceiverId = messageInfo.ReceiverId,
			Timestamp = messageInfo.Timestamp,
			Content = messageInfo.Content
		};
		await using var dbContext = await dbContextFactory.CreateDbContextAsync(cancellationToken);
		await dbContext.DirectMessages.AddAsync(messageEntity, cancellationToken);
		await dbContext.SaveChangesAsync(cancellationToken);
		return messageEntity.Id;
	}
}