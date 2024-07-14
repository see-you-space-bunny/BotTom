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
	private static Task User_SendWhisper(User recipient, string message)
	{
		string toSend = string.Format(MessageCode.PRI.GetEnumAttribute<MessageCode,OutgoingMessageFormatAttribute>().Format,message,recipient.Name);
		Task.Run(() => Console.WriteLine($"{DateTime.Now.ToShortTimeString()} | @ {recipient.Name}: {message}"));
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
		User_SendWhisper(value.Recipient, value.Message);
#endregion


////////////////////////////////////////////////


#region (+) SendChannelMessage
	/// <summary>
	/// <b>Chat Post</b><br/>
	/// posts a chat message to the selected channel
	/// </summary>
	/// <param name="channel">channel</param>
	/// <param name="message">message as <c>JavaScript</c> Encoded String</param>
	private static Task User_SendChannelMessage(Channel channel, string message)
	{
		ConnectionCheck();
		string toSend = string.Format(MessageCode.MSG.GetEnumAttribute<MessageCode,OutgoingMessageFormatAttribute>().Format,channel.Code,message);
		Task.Run(() => Console.WriteLine($"{DateTime.Now.ToShortTimeString()} | @ {channel.Name}: {message}"));
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
		User_SendChannelMessage(value.Channel, value.Message);
#endregion


////////////////////////////////////////////////


#region (+) SendChannelAd
	/// <summary>
	/// <b>Chat Post</b><br/>
	/// posts an ad to the selected channel
	/// </summary>
	/// <param name="channel">channel</param>
	/// <param name="message">message as <c>JavaScript</c> Encoded String</param>
	private static Task User_SendChannelAd(Channel channel, string message)
	{
		ConnectionCheck();
		string toSend = string.Format(MessageCode.LRP.GetEnumAttribute<MessageCode,OutgoingMessageFormatAttribute>().Format,channel.Code,message);
		Task.Run(() => Console.WriteLine($"{DateTime.Now.ToShortTimeString()} | @ {channel.Name}: {message}"));
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
		User_SendChannelAd(value.Channel, value.Message);
#endregion
}
