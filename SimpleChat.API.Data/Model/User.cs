using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace SimpleChat.API.Data.Model;

[Index(nameof(Name), IsUnique = true)]
public sealed class User
{
	[Key]
	public int Id { get; set; }

	[Required]
	[MinLength(8)]
	[MaxLength(20)]
	public string Name { get; set; } = string.Empty;

	[MinLength(8)]
	[MaxLength(20)]
	public string PasswordHash { get; set; } = string.Empty;

	public bool IsAdministrator { get; set; }
	
	public UserSession? Session { get; set; }
}