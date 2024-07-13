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
#region (-) Handler_LRP
	/// <summary><b>Channel Advertisement Message</b><br/>
	/// incoming when a character posts an ad in a channel<br/>
	/// (INC) &lt;&lt; LRP {"character": "CharacterName", "channel": "ChannelCode", "message": "A String"}<br/>
	/// </summary>
	/// <param name="json">the incoming message's contents</param>
	/// <returns>the task we initiated</returns>
	private Task Handler_LRP(JObject json)
	{
		Task t = Task.Run(() => MessageHandler?.Invoke(
			MessageCode.LRP,
			new FChatMessageBuilder()
				.WithRecipient(ApiUser)
				.WithAuthor(Users.SingleByName(json["character"].ToString()))
				.WithChannel(Channels.SingleByNameOrCode(json["channel"].ToString()))
				.WithMessage(json["message"].ToString())
				.Build()
		));
		Console.WriteLine(json["message"].ToString());
		return t;
	}
#endregion


#region (-) Handler_MSG
	/// <summary><b>Channel Message</b><br/>
	/// incoming when a character posts a chat message in a channel<br/>
	/// (INC) &lt;&lt; MSG {"character": "CharacterName", "channel": "ChannelCode", "message": "A String"}<br/>
	/// </summary>
	/// <param name="json">the incoming message's contents</param>
	/// <returns>the task we initiated</returns>
	private Task Handler_MSG(JObject json)
	{
		Task t = Task.Run(() => MessageHandler?.Invoke(
			MessageCode.MSG,
			new FChatMessageBuilder()
				.WithRecipient(ApiUser)
				.WithAuthor(Users.SingleByName(json["character"].ToString()))
				.WithChannel(Channels.SingleByNameOrCode(json["channel"].ToString()))
				.WithMessage(json["message"].ToString())
				.Build()
		));
		Console.WriteLine(json["message"].ToString());
		return t;
	}
#endregion


#region (-) Handler_PRI
	/// <summary><b>Private Message</b><br/>
	/// incoming with a private message from another character<br/>
	/// (INC) &lt;&lt; PRI {"character": "CharacterName", "message": "A String", "recipient": "ApiUserCharacterName"}<br/>
	/// </summary>
	/// <param name="json">the incoming message's contents</param>
	/// <returns>the task we initiated</returns>
	private Task Handler_PRI(JObject json)
	{
		Task t = Task.Run(() => MessageHandler?.Invoke(
			MessageCode.PRI,
			new FChatMessageBuilder()
				.WithRecipient(ApiUser)
				.WithAuthor(Users.SingleByName(json["character"].ToString()))
				.WithMessage(json["message"].ToString())
				.Build()
		));
		Console.WriteLine(json["message"].ToString());
		return t;
	}
#endregion


#region (-) Handler_RLL
	/// <summary><b>Dice Roll/Bottle Spin</b><br/>
	/// incoming when a character rolls dice or spins the bottle<br/>
	/// (INC) &lt;&lt; RLL {"character": "CharacterName", "channel": "ChannelCode", "message": "A String", "type": "CommandType", "results": Array[1234], "endresult": 1234, "rolls": Array["RollResults"], "target": "TargetCharacterName"}<br/>
	/// </summary>
	/// <param name="json">the incoming message's contents</param>
	/// <returns>the task we initiated</returns>
	private Task Handler_RLL(JObject json)
	{
		return Task.CompletedTask;
	}
#endregion


#region (-) Handler_TPN
	/// <summary><b>Typing Notification</b><br/>
	/// incoming with a notification that a character is typing<br/>
	/// (INC) &lt;&lt; TPN {"character": "CharacterName", "status": "TypingStatusType"}<br/>
	/// <i>"status" must indicate 'has entered text' or 'active typing' ?</i><br/>
	/// </summary>
	/// <param name="json">the incoming message's contents</param>
	/// <returns>the task we initiated</returns>
	private Task Handler_TPN(JObject json)
	{
		return Task.CompletedTask;
	}
#endregion
}
