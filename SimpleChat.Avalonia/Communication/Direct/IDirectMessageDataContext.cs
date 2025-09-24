using System;

namespace SimpleChat.Avalonia.Communication.Direct;

public interface IDirectMessageDataContext
{
	string Content { get; }
	MessageType Type { get; }
	DateTime Timestamp { get; }
}