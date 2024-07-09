using System;
using FChatApi.Enums;
using FChatApi.Objects;

namespace FChatApi.EventArguments;

/// <summary>
/// custom event arg as easy way to send channel info up
/// </summary>
/// no i don't know why I'm doing it this way
public class MessageEventArgs : EventArgs
{
	/// <summary>
	/// the message body
	/// </summary>
	public string Message;

	/// <summary>
	/// the channel
	/// </summary>
	public Channel Channel;

	/// <summary>
	/// sending user
	/// </summary>
	public User User;

	/// <summary>
	/// advanced message type
	/// </summary>
	public FChatMessageType MessageType;
}
