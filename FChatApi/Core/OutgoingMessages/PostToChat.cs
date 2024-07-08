using System;
using System.Threading.Tasks;
using FChatApi.Objects;
using FChatApi.Enums;
using FChatApi.Attributes;

namespace FChatApi.Core;

public partial class ApiConnection
{
#region (+) SendWhisper
	/// <summary>
	/// <b>Chat Post</b><br/>
	/// sends a direct message to the selected character
	/// </summary>
	/// <param name="recipient">character name</param>
	/// <param name="message">message as <c>JavaScript</c> Encoded String</param>
	private static Task User_SendWhisper(string recipient, string message)
	{
		string toSend = string.Format(MessageCode.PRI.GetEnumAttribute<MessageCode,OutgoingMessageFormatAttribute>().Format,recipient,message);
		Console.WriteLine($"{DateTime.Now.ToShortTimeString()} | @ {recipient}: {message}");
#if DEBUG
		return DebugSendAsync(toSend);
#else
		return Client.SendAsync(toSend);
#endif
	}

	/// <summary>
	/// <b>Chat Post</b><br/>
	/// sends a direct message to a user
	/// </summary>
	/// <param name="value">message being sent</param>
	private static Task User_SendWhisper(FChatMessage value) =>
		User_SendWhisper(value.Recipient.Name, value.Message);
#endregion
////////////////////////////////////////////////


#region (+) SendChannelMessage
	/// <summary>
	/// <b>Chat Post</b><br/>
	/// posts a chat message to the selected channel
	/// </summary>
	/// <param name="channel">channel code</param>
	/// <param name="message">message as <c>JavaScript</c> Encoded String</param>
	private static Task User_SendChannelMessage(string channel, string message)
	{
		ConnectionCheck();
		string toSend = string.Format(MessageCode.MSG.GetEnumAttribute<MessageCode,OutgoingMessageFormatAttribute>().Format,channel,message);
		Console.WriteLine($"{DateTime.Now.ToShortTimeString()} | @ {channel}: {message}");
#if DEBUG
		return DebugSendAsync(toSend);
#else
		return Client.SendAsync(toSend);
#endif
	}

	/// <summary>
	/// <b>Chat Post</b><br/>
	/// posts a chat message in a channel
	/// </summary>
	/// <param name="value">message being sent</param>
	private static Task User_SendChannelMessage(FChatMessage value) =>
		User_SendChannelMessage(value.Channel.Code, value.Message);
#endregion


////////////////////////////////////////////////


#region (+) SendChannelAd
	/// <summary>
	/// <b>Chat Post</b><br/>
	/// posts an ad to the selected channel
	/// </summary>
	/// <param name="channel">channel code</param>
	/// <param name="message">message as <c>JavaScript</c> Encoded String</param>
	private static Task User_SendChannelAd(string channel, string message)
	{
		ConnectionCheck();
		string toSend = string.Format(MessageCode.LRP.GetEnumAttribute<MessageCode,OutgoingMessageFormatAttribute>().Format,channel,message);
		Console.WriteLine($"{DateTime.Now.ToShortTimeString()} | @ {channel}: {message}");
#if DEBUG
		return DebugSendAsync(toSend);
#else
		return Client.SendAsync(toSend);
#endif
	}

	/// <summary>
	/// <b>Chat Post</b><br/>
	/// posts an ad in a channel
	/// </summary>
	/// <param name="value">message being sent</param>
	private static Task User_SendChannelAd(FChatMessage value) =>
		User_SendChannelAd(value.Channel.Code, value.Message);
#endregion
}
