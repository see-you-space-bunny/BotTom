using FChatApi.Core;
using FChatApi.Objects;
using FChatApi.Enums;
using System;

namespace BotTom.FChat;

public class SystemController
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    private static SystemController _instance;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

    private static readonly object iLocker = new();

    ApiConnection Api;

    private string BaseColor;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    private SystemController()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    {
        BaseColor = "white";
    }

    public static SystemController Instance
    {
        get
        {
            lock (iLocker)
            {
                _instance ??= new SystemController();
                return _instance;
            }
        }
    }

    public void SetBaseColor(string color)
    {
        BaseColor = color;
    }

    public void SetApi(ApiConnection api)
    {
        Api = api;
    }

    /// <summary>
    /// replies via the f-list api
    /// </summary>
    /// <param name="channel">channel to reply to</param>
    /// <param name="message">message to reply with</param>
    /// <param name="recipient">person to reply to</param>
    public void Respond(string channel, string message, string recipient)
    {
        ChatMessageType mt = ChatMessageType.Basic;
        if (string.IsNullOrWhiteSpace(channel))
        {
            mt = ChatMessageType.Whisper;
        }

        Respond(channel, message, recipient, mt);
    }

    /// <summary>
    /// replies via the f-list api
    /// </summary>
    /// <param name="channel">channel to reply to</param>
    /// <param name="message">message to reply with</param>
    /// <param name="recipient">person to reply to</param>
    /// <param name="messagetype">type of message we're sending</param>
    public void Respond(string channel, string message, string recipient, ChatMessageType messagetype)
    {
        if (!string.IsNullOrWhiteSpace(channel))
        {
            recipient = string.Empty;
        }

        if (string.IsNullOrWhiteSpace(channel) && string.IsNullOrWhiteSpace(recipient))
        {
            Console.WriteLine($"Error attempting to send message with no valid channel or recipient.");
            return;
        }

        message = $"{message}";

        Api.EnqueueMessage(channel, message, recipient, messagetype);
    }

    /// <summary>
    /// replies via the f-list api
    /// </summary>
    /// <param name="messageBuilder">the builder we're passing to send-message</param>
    public void Respond(ChatMessageBuilder messageBuilder)
    {
        Api.EnqueueMessage(messageBuilder);
    }
}