using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BotTom.CardiApi;
using org.mariuszgromada.math.mxparser;

namespace Bot_Tom.CardiApi
{
    public class ChatMessageBuilder
    {
        private const string BuildStep = "Build";
        private const string DeriveWhisperChannelStep = "DeriveWhisperChannel";

        private const uint MessageLengthMaxWhisper = 50000;
        private const uint MessageLengthMaxBasic = 4000;


        #region Core Methods
        string? Author;
        public ChatMessageBuilder WithAuthor(string value)
        {
            Author = value;
            return this;
        }
        
        string? Recipient;
        public ChatMessageBuilder WithRecipient(string value)
        {
            Recipient = value;
            return this;
        }
        
        string? Channel;
        public ChatMessageBuilder WithChannel(string value)
        {
            Channel = value;
            return this;
        }
        
        MessageType? MessageType;
        public ChatMessageBuilder WithMessageType(MessageType value)
        {
            MessageType = value;
            return this;
        }
        
        string? Message;
        public ChatMessageBuilder WithMessage(string value)
        {
            Message = value;
            return this;
        }

        public ChatMessage Build()
        {
            switch(MessageType)
            {
                case BotTom.CardiApi.MessageType.Basic:
                    if (Channel is null)
                        throw new IncompleteBuilderException(nameof(Channel),Channel,BuildStep);
                    break;
                    
                case BotTom.CardiApi.MessageType.Whisper:
                    if (Recipient is null)
                        throw new IncompleteBuilderException(nameof(Recipient),Recipient,BuildStep);
                    Channel = DeriveWhisperChannel();
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(MessageType),
                        $"{BuildStep} step encountered unexpected {typeof(MessageType)} value: {MessageType}");
            }

            if (Author is null)
                throw new IncompleteBuilderException(nameof(Author),Author,BuildStep);

            if (Message is null)
                throw new IncompleteBuilderException(nameof(Message),Message,BuildStep);

            StringBuilder messageStringBuilder = new StringBuilder();
            if (Mention is not null)
            {
                messageStringBuilder.Append(Mention);
                messageStringBuilder.Append(' ');
            }
            messageStringBuilder.Append(Message ?? throw new IncompleteBuilderException(nameof(Message),Message,BuildStep));

            return new ChatMessage(
                Author,
                Recipient ?? string.Empty,
                (MessageType)MessageType,
                Channel,
                messageStringBuilder.ToString()
            );
        }

        private string DeriveWhisperChannel() =>
            Recipient ?? throw new IncompleteBuilderException(nameof(Recipient),Recipient,DeriveWhisperChannelStep);
        #endregion


        #region Optional Methods
        string? Mention;
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

        [Serializable]
        private class IncompleteBuilderException : Exception
        {
            public IncompleteBuilderException()
            {
            }

            public IncompleteBuilderException(string propertyName,object? value,string step)
                : base($"Value of {propertyName} cannot be {value} during {step} step.")
            {
            }

            public IncompleteBuilderException(string? message) : base(message)
            {
            }

            public IncompleteBuilderException(string? message, Exception? innerException) : base(message, innerException)
            {
            }
        }
    }
}