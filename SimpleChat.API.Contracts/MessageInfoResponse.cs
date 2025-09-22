using System.ComponentModel.DataAnnotations;

namespace SimpleChat.API.Contracts;

public sealed class MessageInfoResponse
{
	public int SenderId { get; set; }

	public DateTime Timestamp { get; set; }

	[MinLength(1), MaxLength(1000)]
	public string Content { get; set; } = string.Empty;
}