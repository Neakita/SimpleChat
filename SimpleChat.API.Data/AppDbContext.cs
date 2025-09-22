using Microsoft.EntityFrameworkCore;
using SimpleChat.API.Data.Model;

namespace SimpleChat.API.Data;

public sealed class AppDbContext : DbContext
{
	public AppDbContext(DbContextOptions options) : base(options)
	{
		Database.EnsureCreated();
	}

	public DbSet<User> Users { get; set; }
	public DbSet<DirectMessage> DirectMessages { get; set; }
}