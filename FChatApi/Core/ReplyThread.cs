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
#region Queue & Lock F(-)
	/// <summary>
	/// lock object to prevent access conflicts<br/>
	/// ensures messages wait to be sent
	/// </summary>
	private readonly object SocketLocker = new();

	/// <summary>the message queue, keyed to recipient</summary>
	private static readonly Dictionary<IMessageRecipient,Queue<FChatMessageBuilder>> MessageQueue = [];

	/// <summary>the reply thread that periodically checks and handles the message queue</summary>
	private static Timer ReplyThread;
#endregion


////////////////////////////////////////////////


#region (+) EnqueueMessage
	/// <summary>
	/// enqueues a message by its IMessageRecipient
	/// </summary>
	/// <param name="value">chat message builder</param>
	public void EnqueueMessage(FChatMessageBuilder value)
	{
		lock (SocketLocker)
		{
			if (value.HasRecipient)
			{
				MessageQueue.TryAdd(value.MessageRecipient,new Queue<FChatMessageBuilder>());
				MessageQueue[value.MessageRecipient].Enqueue(value);
			}
			else
			{
				throw new InvalidOperationException("The message you are trying to send has no valid MessageRecipient.");
			}
		}
	}
#endregion


////////////////////////////////////////////////


#region (+) EnqueueMessage
	/// <summary>
	/// enqueues a message
	/// </summary>
	/// <param name="channelcode">channel code, if any</param>
	/// <param name="message">the message to be sent</param>
	/// <param name="recipient">the recipient character, if any</param>
	/// <param name="type">the type of message</param>
	public void EnqueueMessage(string channelcode, string message, string recipient, FChatMessageType type = FChatMessageType.Basic)
	{
		EnqueueMessage(new FChatMessageBuilder()
				.WithRecipient(recipient)
				.WithChannel(GetChannelByCode(channelcode))
				.WithMessage(message)
				.WithMessageType(type)
		);
	}
#endregion


////////////////////////////////////////////////
////////////////////////////////////////////////


#region (-) StartReplyThread
	/// <summary>
	/// assigns a new <c>ReplyThread</c> that calls <c>ReplyTicker</c><br/>disposes of the old thread if nessecary 
	/// </summary>
	/// <param name="interval">miliseconds between calls</param>
	private void StartReplyThread(int interval)
	{
		ReplyThread?.Dispose();
		ReplyThread = new Timer(ReplyTicker,new AutoResetEvent(true),0,interval);
		Console.WriteLine($"Starting ReplyTicker with a base interval of {interval} ms.");
	}
#endregion


////////////////////////////////////////////////
////////////////////////////////////////////////


#region (-) ReplyTicker
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

				message = currentQueue.Dequeue().Build();

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

		await (message.MessageType switch
		{
			FChatMessageType.Whisper		=> User_SendWhisper(message.Recipient.Name, message.Message),
			FChatMessageType.Basic			=> User_SendChannelMessage(message.Channel.Code, message.Message),
			FChatMessageType.Advertisement	=> User_SendChannelAd(message.Channel.Code, message.Message),
			_	=> Task.Run(() => Console.WriteLine("Bad reply: " + message.Channel + " / " + message.Recipient + " / " + message)),
		});
	}
#endregion
}
