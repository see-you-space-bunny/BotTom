using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using FChatApi.Objects;
using FChatApi.Enums;
using FChatApi.Interfaces;
using System.Linq;

namespace FChatApi.Core;

public partial class ApiConnection
{
#region Queue & Lock
	private readonly object SocketLocker = new();
	static readonly Dictionary<IMessageRecipient,Queue<FChatMessageBuilder>> MessageQueue = [];
	static Timer ReplyThread;
#endregion


////////////////////////////////////////////////


#region SendMessage
	private static async Task SendMessage(string channel, string message)
	{
		string toSend = $"{MessageCode.MSG} {{ \"channel\": \"{channel}\", \"message\": \"{message}\" }}";
		Console.WriteLine($"{DateTime.Now.ToShortTimeString()} | @ {channel}: {message}");
#if DEBUG
		if (!await Client.SendAsync(toSend))
		{
			Console.WriteLine(WarningBanner);
			Console.WriteLine($"Unable to send message: {toSend}");
			Console.WriteLine(GenericBanner);
		}
#else
		Client.SendAsync(toSend);
#endif
	}
#endregion


////////////////////////////////////////////////


#region SendAd
	private static async Task SendAd(string channel, string message)
	{
		string toSend = $"{MessageCode.LRP} {{ \"channel\": \"{channel}\", \"message\": \"{message}\" }}";
		Console.WriteLine($"{DateTime.Now.ToShortTimeString()} | @ {channel}: {message}");
#if DEBUG
		if (!await Client.SendAsync(toSend))
		{
			Console.WriteLine(WarningBanner);
			Console.WriteLine($"Unable to send message: {toSend}");
			Console.WriteLine(GenericBanner);
		}
#else
		Client.SendAsync(toSend);
#endif
	}
#endregion


////////////////////////////////////////////////


#region SetStatus
	public static async Task SetStatus(string statusMessage, ChatStatus status, string sendingUser)
	{
		string toSend = $"{MessageCode.STA} {{ \"status\": \"{status}\", \"statusmsg\": \"{statusMessage}\", \"character\": \"{sendingUser}\" }}";
		Console.WriteLine($"{DateTime.Now.ToShortTimeString()} | @ {status}: {statusMessage}");
#if DEBUG
		if (!await Client.SendAsync(toSend))
		{
			Console.WriteLine(WarningBanner);
			Console.WriteLine($"Unable to send message: {toSend}");
			Console.WriteLine(GenericBanner);
		}
#else
		Client.SendAsync(toSend);
#endif
	}
#endregion

////////////////////////////////////////////////


#region SendWhisper
	private static async Task SendWhisper(string targetUser, string message)
	{
		string toSend = $"{MessageCode.PRI} {{ \"recipient\": \"{targetUser}\", \"message\": \"{message}\" }}";
		Console.WriteLine($"{DateTime.Now.ToShortTimeString()} | @ {targetUser}: {message}");
#if DEBUG
		if (!await Client.SendAsync(toSend))
		{
			Console.WriteLine(WarningBanner);
			Console.WriteLine($"Unable to send message: {toSend}");
			Console.WriteLine(GenericBanner);
		}
#else
		Client.SendAsync(toSend);
#endif
	}
#endregion


////////////////////////////////////////////////


#region StartReplyThread
	/// <summary>
	/// assigns a new <c>ReplyThread</c> that calls <c>ReplyTicker</c><br/>disposes of the old thread if nessecary 
	/// </summary>
	/// <param name="interval">miliseconds between calls</param>
	private void StartReplyThread(int interval)
	{
		ReplyThread?.Dispose();
		ReplyThread = new Timer(ReplyTicker,new AutoResetEvent(true),0,interval);
		Console.WriteLine($"Starting ReplyTicker with an interval of {interval}");
	}
    #endregion


    ////////////////////////////////////////////////


    #region ReplyTicker
    /// <summary>
    /// dequeues and sends out messages 
    /// </summary>
    private async void ReplyTicker(object state)
	{
		FChatMessage message;
		try
		{
			lock (SocketLocker)
			{
				if (MessageQueue.Count == 0)
					return;
				
				(var currentRecipient, var currentQueue) = MessageQueue
					.FirstOrDefault(mq=>DateTime.Now > mq.Key.Next && mq.Value.Count > 0);

				if (currentQueue == default)
					return;

				message = currentQueue
					.Dequeue()
					.Build();

				currentRecipient.MessageSent();

				if (currentQueue.Count == 0)
					MessageQueue.Remove(currentRecipient);
			}
		}
		catch (InvalidOperationException e)
		{
			Console.WriteLine(ErrorBanner);
			Console.WriteLine(e.Message);
			Console.WriteLine(e.StackTrace);
			Console.WriteLine(GenericBanner);
			return;
		}
		catch (NullReferenceException e)
		{
			Console.WriteLine(ErrorBanner);
			Console.WriteLine(e.Message);
			Console.WriteLine(e.StackTrace);
			Console.WriteLine(GenericBanner);
			return;
		}
		catch (Exception e)
		{
			Console.WriteLine(GenericBanner);
			Console.WriteLine($"///// Unhandled {e.GetType()} exception when dequeueing message when queue was not empty!");
			Console.WriteLine(ErrorBanner);
			Console.WriteLine(e.Message);
			Console.WriteLine(e.StackTrace);
			Console.WriteLine(GenericBanner);
			return;
		}

		Task t = null;
		switch (message.MessageType)
		{
			case FChatMessageType.Whisper:
				t = SendWhisper(message.Recipient.Name, message.Message);
				break;

			case FChatMessageType.Basic:
				t = SendMessage(message.Channel.Code, message.Message);
				break;

			case FChatMessageType.Advertisement:
				t = SendAd(message.Channel.Code, message.Message);
				break;

			default:
				Console.WriteLine("Bad reply: " + message.Channel + " / " + message.Recipient + " / " + message);
				break;
		}
		if (t is not null)
			await t;
	}
#endregion
}
