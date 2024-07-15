using System;
using System.Text;
using WatsonWebsocket;
using System.Linq;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using FChatApi.Systems;
using FChatApi.Objects;
using FChatApi.Enums;
using System.Net.WebSockets;

namespace FChatApi.Core;

public partial class ApiConnection
{
#region (E) MessageReceived
	private void Client_MessageReceived(object sender, MessageReceivedEventArgs @event)
	{
		string message = Encoding.UTF8.GetString(@event.Data.ToArray());
		//Console.WriteLine($"Message from server: {message}");
		ParseMessage(Enum.Parse<MessageCode>(message.Split(' ').First()), message.Split(" ".ToCharArray(), 2).Last());
	}
#endregion


#region (E) ChatDisConnected
	private void Client_ChatDisconnected(object sender, EventArgs eventArgs)
	{
		Console.WriteLine("Disconnected from F-Chat servers!");
	}
#endregion


#region (E) ChatConnected
	private async void Client_ChatConnected(object sender, EventArgs eventArgs)
	{
		Console.WriteLine("Connected to F-Chat servers! Sending identification...");
		await IdentifySelf(UserName, TicketInformation.Ticket, CharacterName, ClientId, ClientVersion);
		StartReplyThread(20);
	}
#endregion


///////////////////////////////////////////////////
///////////////////////////////////////////////////
///////////////////////////////////////////////////


#region (-) IdentifySelf
	private async static Task IdentifySelf(string accountName, string ticket, string botName, string botClientID, string botClientVersion)
	{
		string toSend = $"{MessageCode.IDN}  {{ \"method\": \"ticket\", \"account\": \"{accountName}\", \"ticket\": \"{ticket}\", \"character\": \"{botName}\", \"cname\": \"{botClientID}\", \"cversion\": \"{botClientVersion}\" }}";
		await Client.SendAsync(toSend);
	}
#endregion
}