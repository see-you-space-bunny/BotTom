using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChatApi;

namespace BotTom.CardiApi;

public readonly struct ChatMessage(string author,string recipient,MessageType messageType,string channel,string message)
{
    public readonly string Author => author;
    public readonly string Recipient => recipient;
    public readonly MessageType MessageType => messageType;
    public readonly string Channel => channel;
    public readonly string Message => message;
}