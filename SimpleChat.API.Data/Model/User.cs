using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace SimpleChat.API.Data.Model;

[Index(nameof(Login), IsUnique = true)]
public sealed class User
{
	[Key]
	public int Id { get; set; }

	[Required]
	[MinLength(8)]
	[MaxLength(20)]
	public string Login { get; set; } = string.Empty;

	[MinLength(8)]
	[MaxLength(20)]
	public string PasswordHash { get; set; } = string.Empty;

	[MinLength(8)]
	[MaxLength(20)]
	public string DisplayName { get; set; } = string.Empty;

	public bool IsAdministrator { get; set; }
	
	public UserSession? Session { get; set; }
}