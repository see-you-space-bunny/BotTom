using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Timers;
using FChatApi.Objects;
using FChatApi.Enums;

namespace FChatApi.Core;

public partial class ApiConnection
{
    private const string GenericBanner  = "//////////////////////////////";
    private const string ErrorBanner    = "////////////// ERROR   ///////"; 
    private const string WarningBanner  = "////////////// WARNING ///////"; 

    private readonly object SocketLocker = new();
    static readonly Queue<ChatMessageBuilder> MessageQueue = new();

    private static async Task SendMessage(string channel, string message)
    {
        string toSend = $"{Hycybh.MSG} {{ \"channel\": \"{channel}\", \"message\": \"{message}\" }}";
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

    private static async Task SendAd(string channel, string message)
    {
        string toSend = $"{Hycybh.LRP} {{ \"channel\": \"{channel}\", \"message\": \"{message}\" }}";
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

    public static async Task SetStatus(string statusMessage, ChatStatus status, string sendingUser)
    {
        string toSend = $"{Hycybh.STA} {{ \"status\": \"{status}\", \"statusmsg\": \"{statusMessage}\", \"character\": \"{sendingUser}\" }}";
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

    private static async Task SendWhisper(string targetUser, string message)
    {
        string toSend = $"{Hycybh.PRI} {{ \"recipient\": \"{targetUser}\", \"message\": \"{message}\" }}";
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
                if (MessageQueue.Count == 0)
                    return;

                var messageBuilder = MessageQueue.Dequeue();
                message = messageBuilder.Build();
            }
        }
        catch (IncompleteBuilderException ibe)
        {
            Console.WriteLine(ErrorBanner);
            Console.WriteLine(ibe.Message);
            Console.WriteLine(ibe.StackTrace);
            Console.WriteLine(GenericBanner);
            return;
        }
        catch (NullReferenceException nre)
        {
            Console.WriteLine(ErrorBanner);
            Console.WriteLine(nre.Message);
            Console.WriteLine(nre.StackTrace);
            Console.WriteLine(GenericBanner);
            return;
        }
        catch
        {
            Console.WriteLine(ErrorBanner);
            Console.WriteLine("Anonymous Error dequeueing message when queue not empty.");
            Console.WriteLine(GenericBanner);
            return;
        }

        if (message.MessageType == ChatMessageType.Whisper)
        {
            await SendWhisper(message.Recipient, message.Message);
        }
        else if (message.MessageType == ChatMessageType.Advertisement)
        {
            await SendAd(message.Channel, message.Message);
        }
        else if (message.MessageType == ChatMessageType.Basic)
        {
            await SendMessage(message.Channel, message.Message);
        }
        else
        {
            Console.WriteLine("Bad reply: " + message.Channel + " / " + message.Recipient + " / " + message);
        }
    }
}
