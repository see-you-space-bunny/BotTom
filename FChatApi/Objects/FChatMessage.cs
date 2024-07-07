using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FChatApi.Enums;

namespace FChatApi.Objects;

/// <summary>
/// a chat message
/// </summary>
/// <param name="author">the user that created the message</param>
/// <param name="recipient"></param>
/// <param name="messageType"></param>
/// <param name="channel"></param>
/// <param name="message"></param>
public class FChatMessage(User author, User recipient, FChatMessageType messageType, Channel channel, string message)
{
	public readonly User Author = author;
	public readonly User Recipient = recipient;
	public readonly FChatMessageType MessageType = messageType;
	public readonly Channel Channel = channel;
	public readonly string Message = message;
}