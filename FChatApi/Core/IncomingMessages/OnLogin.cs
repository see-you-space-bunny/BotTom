using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FChatApi.Enums;
using FChatApi.EventArguments;
using FChatApi.Objects;
using Newtonsoft.Json.Linq;

namespace FChatApi.Core;

public partial class ApiConnection
{
#region (-) Handler_ADL
	/// <summary><b>Admin List</b><br/>
	/// when incoming provides a list of global ops<br/>
	/// (INC) &lt;&lt; ADL {"ops": Array["OperatorName"]}
	/// </summary>
	/// <param name="json">the incoming message's contents</param>
	/// <returns>the task we initiated</returns>
	private Task Handler_ADL(JObject json,bool logging = true)
	{
		return Task.CompletedTask;
	}
#endregion


#region (-) Handler_CHA
	/// <summary><b>Official Channel List</b><br/>
	/// incoming with a list of official channels<br/>
	/// (INC) &lt;&lt; CHA {"channels": Array[OfficialChannel]}<br/>
	/// </summary>
	/// <param name="json">the incoming message's contents</param>
	/// <returns>the task we initiated</returns>
	private Task Handler_CHA(JObject json,bool logging = true)
	{
		List<Channel> publicChannelList = [];
		foreach (string channelName in json["channels"].Select(ch => ch["name"].ToString()))
		{					
			publicChannelList.Add(new Channel(channelName,channelName,ChannelType.Public));
		}

		return Task.Run(() =>
		{
			Channels.RefreshAvailableChannels(publicChannelList, ChannelType.Public);
			PublicChannelsReceivedHandler?.Invoke(this, new ChannelEventArgs() { });
			Console.WriteLine($"Public Channels Recieved... {publicChannelList.Count} total Public Channels.");
		});
	}
#endregion


#region (-) Handler_CON
	/// <summary><b>Connection Count</b><br/>
	/// incoming with count of characters connected to chat<br/>
	/// (INC) &lt;&lt; CON {"count": 1234}
	/// </summary>
	/// <param name="json">the incoming message's contents</param>
	/// <returns>the task we initiated</returns>
	private Task Handler_CON(JObject json,bool logging = true) =>
		Task.Run(() => Console.WriteLine($"{json["count"]} connected users sent."));
#endregion


#region (-) Handler_HLO
	/// <summary><b>Server Welcome Message</b><br/>
	/// incoming when initially connecting to the server<br/>
	/// (INC) &lt;&lt; HLO {"message": "A String"}<br/>
	/// <i>the message contains the server version</i>
	/// </summary>
	/// <param name="json">the incoming message's contents</param>
	/// <returns>the task we initiated</returns>
	private Task Handler_HLO(JObject json,bool logging = true)
	{
		return Task.CompletedTask;
	}
#endregion


#region (-) Handler_IDN
	/// <summary><b>Chat Login</b><br/>
	/// incoming when api-user has successfully identified with the server<br/>
	/// (INC) &lt;&lt; IDN {"character": "ApiUserName"}<br/>
	/// <i><b>DO NOT</b> issue chat commands until you have also recieved the NLN command with the current character's name!!<br/>
	/// Doing so will cause desynchronization with the server.</i><br/>
	/// </summary>
	/// <param name="json">the incoming message's contents</param>
	/// <returns>the task we initiated</returns>
	private Task Handler_IDN(JObject json,bool logging = true)
	{
		Task[] tasks = new Task[2];
#if !UNIT_TEST
		tasks[0] = RequestChannelListFromServer(ChannelType.Private);
		tasks[1] = RequestChannelListFromServer(ChannelType.Public);
#endif
		return Task.Run(() => Task.WaitAll([.. tasks]));
	}
#endregion


#region (-) Handler_ORS
	/// <summary><b>Private Channel List</b><br/>
	/// incoming with a list of private chanels<br/>
	/// (INC) &lt;&lt; ORS {"channels": Array[PrivateChannel]}
	/// </summary>
	/// <param name="json">the incoming message's contents</param>
	/// <returns>the task we initiated</returns>
	private Task Handler_ORS(JObject json,bool logging = true)
	{
		List<Channel> privateChannelList = [];
		foreach (var channel in json["channels"])
		{
			privateChannelList.Add(new Channel(channel["title"].ToString(), channel["name"].ToString(), ChannelType.Private));
		}

		return Task.Run(() =>
		{
			Channels.RefreshAvailableChannels(privateChannelList, ChannelType.Private);
			PrivateChannelsReceivedHandler?.Invoke(this, new ChannelEventArgs() { });
			Console.WriteLine($"Private Channels Recieved... {privateChannelList.Count} total Private Channels.");
		});
	}
#endregion
}
