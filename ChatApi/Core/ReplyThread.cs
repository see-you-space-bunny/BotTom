using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Timers;
using ChatApi.Objects;

namespace ChatApi.Core;

public partial class ApiConnection
{
    private readonly object SocketLocker = new();
    static readonly Queue<ChatMessageBuilder> MessageQueue = new();

    private static async Task SendMessage(string channel, string message)
    {
        string toSend = $"{Hycybh.MSG} {{ \"channel\": \"{channel}\", \"message\": \"{message}\" }}";
        Console.WriteLine($"{DateTime.Now.ToShortTimeString()} | {channel}: {message}");
        await Client.SendAsync(toSend);
    }

    private static async Task SendAd(string channel, string message)
    {
        string toSend = $"{Hycybh.LRP} {{ \"channel\": \"{channel}\", \"message\": \"{message}\" }}";
        Console.WriteLine($"{DateTime.Now.ToShortTimeString()} | {channel}: {message}");
        await Client.SendAsync(toSend);
    }

    public static async Task SetStatus(string statusMessage, ChatStatus status, string sendingUser)
    {
        string toSend = $"{Hycybh.STA} {{ \"status\": \"{status}\", \"statusmsg\": \"{statusMessage}\", \"character\": \"{sendingUser}\" }}";
        Console.WriteLine($"{DateTime.Now.ToShortTimeString()} | {status}: {statusMessage}");
        await Client.SendAsync(toSend);
    }

    private static async Task SendWhisper(string targetUser, string message)
    {
        string toSend = $"{Hycybh.PRI} {{ \"recipient\": \"{targetUser}\", \"message\": \"{message}\" }}";
        Console.WriteLine($"{DateTime.Now.ToShortTimeString()} | {targetUser}: {message}");
        await Client.SendAsync(toSend);
    }

    private void StartReplyThread()
    {
        Timer timer = new(){Interval = 1001};
        timer.Elapsed += new ElapsedEventHandler(ReplyTicker);
        timer.Start();
        Console.WriteLine("Starting Reply-ticket");
    }

    private async void ReplyTicker(object source, ElapsedEventArgs e)
    {
        ChatMessage message;
        try
        {
            lock (SocketLocker)
            {
                if (MessageQueue.Count == 0) return;
                message = MessageQueue.Dequeue().Build();
            }
        }
        catch
        {
            Console.WriteLine("Error dequeueing message when queue not empty.");
            return;
        }

        if (message.MessageType == MessageType.Whisper)
        {
            await SendWhisper(message.Recipient, message.Message);
        }
        else if (message.MessageType == MessageType.Advertisement)
        {
            await SendAd(message.Channel, message.Message);
        }
        else if (message.MessageType == MessageType.Basic)
        {
            await SendMessage(message.Channel, message.Message);
        }
        else
        {
            Console.WriteLine("Bad reply: " + message.Channel + " / " + message.Recipient + " / " + message);
        }
    }
}
