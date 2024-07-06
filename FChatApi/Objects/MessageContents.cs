using FChatApi.Enums;

namespace FChatApi.Objects;

/// <summary>
/// just setting some values
/// </summary>
/// <param name="ch">originating channel</param>
/// <param name="msg">full, scrubbed message</param>
/// <param name="user">who sent the message</param>
/// <param name="type">the type of message this is</param>
public class MessageContents(string ch, string msg, string user, ChatMessageType type)
{
	public string channel = ch;

	public string message = msg;

	public string sendinguser = user;

	public ChatMessageType messageType = type;
}
