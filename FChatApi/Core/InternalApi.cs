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
using FChatApi.EventArguments;

namespace FChatApi.Core;

public partial class ApiConnection
{
#region Constructor
	public ApiConnection()
	{ }
#endregion


///////////////////////////////////////////////////


#region (E) MessageReceived
	async void Client_MessageReceived(object sender, MessageReceivedEventArgs @event)
	{
		string message = Encoding.UTF8.GetString(@event.Data.ToArray());
		//Console.WriteLine($"Message from server: {message}");
		await ParseMessage(Enum.Parse<MessageCode>(message.Split(' ').First()), message.Split(" ".ToCharArray(), 2).Last());
	}
#endregion


#region (E) ChatDisConnected
	void Client_ChatDisconnected(object sender, EventArgs eventArgs)
	{
		Console.WriteLine("Disconnected from F-Chat servers!");
	}
#endregion


#region (E) ChatConnected
	async void Client_ChatConnected(object sender, EventArgs eventArgs)
	{
		Console.WriteLine("Connected to F-Chat servers! Sending identification...");
		await IdentifySelf(UserName, TicketInformation.Ticket, CharacterName, ClientId, ClientVersion);
		StartReplyThread(20);
	}
#endregion


///////////////////////////////////////////////////



#region (-) IdentifySelf
	private async static Task IdentifySelf(string accountName, string ticket, string botName, string botClientID, string botClientVersion)
	{
		string toSend = $"{MessageCode.IDN}  {{ \"method\": \"ticket\", \"account\": \"{accountName}\", \"ticket\": \"{ticket}\", \"character\": \"{botName}\", \"cname\": \"{botClientID}\", \"cversion\": \"{botClientVersion}\" }}";
		await Client.SendAsync(toSend);
	}
#endregion


///////////////////////////////////////////////////


#region (-) ParseToJObject
	private static JObject ParseToJObject(string message, MessageCode hycybh)
	{
		JObject returnCarrier;

		try
		{
			if (message.Split(' ').Length <= 1)
			{
				if (string.Equals(hycybh.ToString(), message))
				{
					return null;
				}

				returnCarrier = JObject.Parse(message);
			}
			else
			{
				returnCarrier = JObject.Parse(message.Replace(hycybh.ToString(), "").TrimStart());
			}
		}
		catch
		{
			throw new Exception($"Failure to parse message: {message}");
		}

		return returnCarrier;
	}
#endregion


	///////////////////////////////////////////////////


#region (-) ParseMessage
	private async Task ParseMessage(MessageCode messageCode, string message)
	{
		JObject json = messageCode switch {
			MessageCode.NON => throw new ArgumentException("Invalid (NON) message code.",nameof(messageCode)),
			MessageCode.PIN => null,
			_ => ParseToJObject(message, messageCode),
		};

		switch (messageCode)
		{
			// keep-alive ping
			case MessageCode.PIN: await Client.SendAsync(MessageCode.PIN.ToString());	break;

			// incoming messages
			case MessageCode.MSG: await Handler_MSG(json,message);	break;
			case MessageCode.PRI: await Handler_PRI(json,message);	break;

			// user activity
			case MessageCode.NLN: await Handler_NLN(json);			break;
			case MessageCode.FLN: await Handler_FLN(json);			break;
			case MessageCode.STA: await Handler_STA(json);			break;

			// channel join/leave, users/operators
			case MessageCode.JCH: await Handler_JCH(json);			break;
			case MessageCode.LCH: await Handler_LCH(json);			break;
			case MessageCode.ICH: await Handler_ICH(json);			break;
			case MessageCode.COL: await Handler_COL(json);			break;

			// info about self
			case MessageCode.LIS: await Handler_LIS(json);			break;
			case MessageCode.FRL: /* friends list				*/	break;
			case MessageCode.IGN: /* ignore list				*/	break;

			// low frequency
			case MessageCode.CDS: await Task.Run(() => ChannelTracker.GetChannelByNameOrCode(json["channel"].ToString()).Description = json["description"].ToString());	break;
			case MessageCode.ORS: await Handler_ORS(json);			break;

			// very low frequency, mostly on login
			case MessageCode.CHA: await Handler_CHA(json);			break;
			case MessageCode.VAR: /* server variable			*/	break;
			case MessageCode.CON: await Task.Run(() => Console.WriteLine($"{json["count"]} connected users sent."));	break;
			case MessageCode.ADL: /* admin list					*/	break;
			case MessageCode.HLO: /* server hello				*/	break;
			case MessageCode.IDN: await Handler_IDN(json);			break;

			default: break;
		}
	}
#endregion
}