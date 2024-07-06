using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FChatApi.Enums;

namespace FChatApi.Objects;

public readonly struct ChatMessage(string author, string recipient, ChatMessageType messageType, string channel, string message)
{
    public readonly string Author = author;
    public readonly string Recipient = recipient;
    public readonly ChatMessageType MessageType = messageType;
    public readonly string Channel = channel;
    public readonly string Message = message;
}