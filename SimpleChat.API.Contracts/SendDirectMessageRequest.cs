using System.ComponentModel.DataAnnotations;

namespace SimpleChat.API.Contracts;

public sealed class SendDirectMessageRequest
{
	public int ReceiverId { get; set; }

	public DateTime Timestamp { get; set; }

	[MinLength(1)]
	public string Content { get; set; } = string.Empty;
}