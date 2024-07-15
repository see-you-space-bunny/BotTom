using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FChatApi.Enums;
using FChatApi.Objects;
using Newtonsoft.Json.Linq;

namespace FChatApi.Core;

public partial class ApiConnection
{
#region (-) Handler_CBU
	/// <summary><b>Channel Ban</b><br/>
	/// incoming when a character has been banned from a channel<br/>
	/// (INC) &lt;&lt; CBU {"character": "CharacterName", "channel": "ChannelCode", "operator": "OperatorName"}<br/>
	/// </summary>
	/// <param name="json">the incoming message's contents</param>
	/// <returns>the task we initiated</returns>
	private Task Handler_CBU(JObject json,bool logging = true)
	{
		return Task.CompletedTask;
	}
#endregion


#region (-) Handler_CDS
	/// <summary><b>Channel Description</b><br/>
	/// incoming when a channel's description has updated<br/>
	/// (INC) &lt;&lt; CDS {"channel": "ChannelCode", "description": "A String"}<br/>
	/// </summary>
	/// <param name="json">the incoming message's contents</param>
	/// <returns>the task we initiated</returns>
	private Task Handler_CDS(JObject json,bool logging = true) =>
		Task.Run(() => {
			Channels.CreationSemaphore.Wait();
			Channels.CreationSemaphore.Release();
			Channels.SingleByNameOrCode(json["channel"].ToString()).Description = json["description"].ToString();
		});
#endregion


#region (-) Handler_CIU
	/// <summary><b>Channel Invite</b><br/>
	/// incoming when api-user is being invited to a channel<br/>
	/// (INC) &lt;&lt; CIU {"sender": "CharacterName", "title": "ChannelName", "name": "ChannelCode"}<br/>
	/// </summary>
	/// <param name="json">the incoming message's contents</param>
	/// <returns>the task we initiated</returns>
	private Task Handler_CIU(JObject json,bool logging = true)
	{
		return Task.CompletedTask;
	}
#endregion


#region (-) Handler_CKU
	/// <summary><b>Channel Kick</b><br/>
	/// incoming when a user was kicked from a channel<br/>
	/// (INC) &lt;&lt; CKU {"character": "CharacterName", "channel": "ChannelCode", "operator": "OperatorName"}<br/>
	/// </summary>
	/// <param name="json">the incoming message's contents</param>
	/// <returns>the task we initiated</returns>
	private Task Handler_CKU(JObject json)
	{
		return Task.CompletedTask;
	}
#endregion


#region (-) Handler_COA
	/// <summary><b>Channel Operator Add</b><br/>
	/// incoming when a character was promoted to channel operator<br/>
	/// (INC) &lt;&lt; COA {"character": "CharacterName", "channel": "ChannelCode"}<br/>
	/// </summary>
	/// <param name="json">the incoming message's contents</param>
	/// <returns>the task we initiated</returns>
	private Task Handler_COA(JObject json)
	{
		return Task.CompletedTask;
	}
#endregion


#region (-) Handler_COR
	/// <summary><b>Channel Operator Remove</b><br/>
	/// incoming when a character was demoted from channel operator<br/>
	/// (INC) &lt;&lt; COR {"character": "CharacterName", "channel": "ChannelCode"}<br/>
	/// </summary>
	/// <param name="json">the incoming message's contents</param>
	/// <returns>the task we initiated</returns>
	private Task Handler_COR(JObject json)
	{
		return Task.CompletedTask;
	}
#endregion


#region (-) Handler_CSO
	/// <summary><b>Channel Set Owner</b><br/>
	/// incoming when a character was promoted to channel owner<br/>
	/// (INC) &lt;&lt; CSO {"character": "CharacterName", "channel": "ChannelCode"}<br/>
	/// </summary>
	/// <param name="json">the incoming message's contents</param>
	/// <returns>the task we initiated</returns>
	private Task Handler_CSO(JObject json)
	{
		return Task.CompletedTask;
	}
#endregion


#region (-) Handler_CTU
	/// <summary><b>Channel Timeout</b><br/>
	/// incoming when a character was timed out from a channel<br/>
	/// (INC) &lt;&lt; CTU {"character": "CharacterName", "channel": "ChannelCode", "operator": "OperatorName", "length": 1234}<br/>
	/// <i>length is measured in seconds</i><br/>
	/// </summary>
	/// <param name="json">the incoming message's contents</param>
	/// <returns>the task we initiated</returns>
	private Task Handler_CTU(JObject json)
	{
		return Task.CompletedTask;
	}
#endregion


#region (-) Handler_RMO
	/// <summary><b>Channel Message Mode</b><br/>
	/// incoming with the message mode of a channel<br/>
	/// (INC) &lt;&lt; RMO {"channel": "ChannelCode", "mode": "MessageMode"}<br/>
	/// </summary>
	/// <param name="json">the incoming message's contents</param>
	/// <returns>the task we initiated</returns>
	private Task Handler_RMO(JObject json)
	{
		return Task.CompletedTask;
	}
#endregion
}
