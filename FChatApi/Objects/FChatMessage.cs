using FChatApi.Enums;

namespace FChatApi.Objects;

/// <summary>
/// a chat message
/// </summary>
/// <param name="messageType">the message's type<br/>types correspond to MessageCode <c>MSG</c>, <c>LRP</c>, <c>PRI</c> and <c>STA</c></param>
/// <param name="author">the user that created the message</param>
/// <param name="recipient">the user that recieved the message (api-user)</param>
/// <param name="channel">the channel (if any) that contains the message</param>
/// <param name="message">the message itself</param>
public class FChatMessage(FChatMessageType messageType, User author, User recipient, Channel channel, string message)
{
	public readonly FChatMessageType MessageType = messageType;
	public readonly User Author = author;
	public readonly User Recipient = recipient;
	public readonly Channel Channel = channel;
	public readonly string Message = message;

	public FChatMessage(MessageCode messageCode, User author, User recipient, Channel channel, string message) :
		this( messageCode switch {
			MessageCode.MSG	=>	FChatMessageType.Basic,
			MessageCode.LRP	=>	FChatMessageType.Advertisement,
			MessageCode.PRI	=>	FChatMessageType.Whisper,
			MessageCode.STA	=>	FChatMessageType.Status,
			_				=>	FChatMessageType.Invalid
		}, author, recipient, channel, message)
	{ }
}