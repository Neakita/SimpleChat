namespace SimpleChat.API.Services;

public sealed class DirectMessageInfo
{
	public int Id { get; set; }
	public int SenderId { get; set; }
	public int ReceiverId { get; set; }
	public DateTime Timestamp { get; set; }
	public string Content { get; set; } = string.Empty;
}