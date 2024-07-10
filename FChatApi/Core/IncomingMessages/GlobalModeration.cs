using System;
using System.Threading.Tasks;
using FChatApi.Enums;
using Newtonsoft.Json.Linq;

namespace FChatApi.Core;

public partial class ApiConnection
{
#region (-) Handler_AOP
	/// <summary><b>Global Operator Added</b><br/>
	/// incoming when a new character is added to the global ops list<br/>
	/// (INC) &lt;&lt; AOP {"character": "OperatorName"}<br/>
	/// </summary>
	/// <param name="json">the incoming message's contents</param>
	/// <returns>the task we initiated</returns>
	private Task Handler_AOP(JObject json)
	{
		return Task.CompletedTask;
	}
#endregion


#region (-) Handler_BRO
	/// <summary><b>Global Broadcast Recieved</b><br/>
	/// incoming when a global broadcast is recieved<br/>
	/// (INC) &lt;&lt; BRO {"character": "OperatorName", "message": "A String"}<br/>
	/// </summary>
	/// <param name="json">the incoming message's contents</param>
	/// <returns>the task we initiated</returns>
	private Task Handler_BRO(JObject json)
	{
		return Task.CompletedTask;
	}
#endregion


#region (-) Handler_DOP
	/// <summary><b>Global Operator Removed</b><br/>
	/// incoming when a character was demoted from global operator<br/>
	/// (INC) &lt;&lt; DOP {"character": "OperatorName"}<br/><br/>
	/// </summary>
	/// <param name="json">the incoming message's contents</param>
	/// <returns>the task we initiated</returns>
	private Task Handler_DOP(JObject json)
	{
		return Task.CompletedTask;
	}
#endregion
}
