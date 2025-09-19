using System.ComponentModel.DataAnnotations;

namespace SimpleChat.API.Authentication;

public sealed class UserAuthenticationRequest
{
	[Required]
	[MinLength(8)]
	[MaxLength(20)]
	public string Login { get; set; } = string.Empty;

	[Required]
	[DataType(DataType.Password)]
	[MinLength(8)]
	[MaxLength(20)]
	public string Password { get; set; } = string.Empty;
}