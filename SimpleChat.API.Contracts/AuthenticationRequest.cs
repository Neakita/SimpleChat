using System.ComponentModel.DataAnnotations;

namespace SimpleChat.API.Contracts;

public sealed class AuthenticationRequest
{
	[Required]
	[MinLength(8)]
	[MaxLength(20)]
	public string Name { get; set; } = string.Empty;

	[Required]
	[DataType(DataType.Password)]
	[MinLength(8)]
	[MaxLength(20)]
	public string Password { get; set; } = string.Empty;
}