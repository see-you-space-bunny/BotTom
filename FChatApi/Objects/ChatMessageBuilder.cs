using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using FChatApi.Enums;

namespace FChatApi.Objects;

public class ChatMessageBuilder
{
    private const string BuildStep = "Build";
    private const string DeriveWhisperChannelStep = "DeriveWhisperChannel";

    private const uint MessageLengthMaxWhisper = 50000;
    private const uint MessageLengthMaxBasic = 4000;


    #region Core Methods
    string Author;
    public ChatMessageBuilder WithAuthor(string value)
    {
        Author = value;
        return this;
    }

    string Recipient;
    public ChatMessageBuilder WithRecipient(string value)
    {
        Recipient = value;
        return this;
    }

    Channel Channel;
    public ChatMessageBuilder WithChannel(Channel value)
    {
        Channel = value;
        return this;
    }

    ChatMessageType MessageType;
    public ChatMessageBuilder WithMessageType(ChatMessageType value)
    {
        MessageType = value;
        return this;
    }

    string Message;
    public ChatMessageBuilder WithMessage(string value)
    {
        Message = value;
        return this;
    }

    public ChatMessage Build()
    {
        if (string.IsNullOrWhiteSpace(Author))
            throw new IncompleteBuilderException("Attempting to send an unsigned (no Author) message.");

        if (string.IsNullOrWhiteSpace(Message))
            throw new IncompleteBuilderException("Attempting to send an empty message.");

        if (string.IsNullOrWhiteSpace(Recipient) && (Channel == null || string.IsNullOrWhiteSpace(Channel.Code)))
            throw new IncompleteBuilderException("Attempting to send an empty user and channel.");

        StringBuilder messageStringBuilder = new();

        if (!string.IsNullOrWhiteSpace(Mention))
            messageStringBuilder
                .Append(Mention)
                .Append(' ');

        if (Channel == null)
        {
            MessageType = ChatMessageType.Whisper;
        }
        else if (!Channel.AdEnabled && MessageType == ChatMessageType.Advertisement)
        {
            throw new IncompleteBuilderException($"Error: Attempting to post an ad in a channel that doesn't support it. ({Channel.Name})");
        }
        
        string encodedMessage = System.Web.HttpUtility.JavaScriptStringEncode(Message);

        return new ChatMessage(
            Author,
            Recipient,
            MessageType,
            Channel != null ? Channel.Code : string.Empty,
            messageStringBuilder.Append(encodedMessage).ToString()
        );
    }
    #endregion

    #region Optional Methods
    string Mention;
    public ChatMessageBuilder WithMention(string value)
    {
        Mention = value;
        return this;
    }
    public ChatMessageBuilder WithoutMention()
    {
        Mention = null;
        return this;
    }
    #endregion
}

[Serializable]
internal class IncompleteBuilderException : Exception
{
    public IncompleteBuilderException()
    {
    }

    public IncompleteBuilderException(string message) : base(message)
    {
    }

    public IncompleteBuilderException(string message, Exception innerException) : base(message, innerException)
    {
    }
}