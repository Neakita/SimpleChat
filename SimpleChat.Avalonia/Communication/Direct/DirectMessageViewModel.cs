using System;

namespace SimpleChat.Avalonia.Communication.Direct;

public sealed class DirectMessageViewModel : IDirectMessageDataContext
{
	public int Id { get; set; }
	public string Content { get; set; } = string.Empty;
	public MessageType Type { get; set; }
	public DateTime Timestamp { get; set; }
}