using System;

namespace SimpleChat.Avalonia.Communication.Direct;

public sealed class DesignDirectMessageDataContext : IDirectMessageDataContext
{
	public static DesignDirectMessageDataContext Outgoing => new()
	{
		Content = "Lorem Ipsum is simply dummy text of the printing and typesetting industry.",
		Type = MessageType.Outgoing,
		Timestamp = DateTime.Now.AddMinutes(-113)
	};

	public static DesignDirectMessageDataContext Incoming => new()
	{
		Content = "Lorem Ipsum is simply dummy text of the printing and typesetting industry.",
		Type = MessageType.Incoming,
		Timestamp = DateTime.Now.AddMinutes(-113)
	};

	public string Content { get; set; } = string.Empty;
	public MessageType Type { get; set; }
	public DateTime Timestamp { get; set; }
}