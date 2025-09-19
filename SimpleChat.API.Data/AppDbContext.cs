using Microsoft.EntityFrameworkCore;
using SimpleChat.API.Data.Model;

namespace SimpleChat.API.Data;

public sealed class AppDbContext : DbContext
{
	public DbSet<User> Users { get; set; } = null!;
}