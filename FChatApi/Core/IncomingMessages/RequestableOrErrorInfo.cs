using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FChatApi.Enums;
using FChatApi.Objects;
using Newtonsoft.Json.Linq;

namespace FChatApi.Core;

public partial class ApiConnection
{
#region (-) Handler_ERR
	/// <summary><b>Error Response</b><br/>
	/// Error Codes:<br/>
	/// &gt; 2: The server is full and not able to accept additional connections at this time.<br/>
	/// &gt; 9: The api-user is banned.<br/>
	/// &gt; 30: The api-user is already connecting from too many clients at this time.<br/>
	/// &gt; 31: The character is logging in from another location and this connection is being terminated.<br/>
	/// &gt; 33: Invalid authentication method used.<br/>
	/// &gt; 39: The api-user is being timed out from the server.<br/>
	/// &gt; 40: The api-user is being kicked from the server.
	/// </summary>
	/// <param name="json">the incoming message's contents</param>
	/// <returns>the task we initiated</returns>
	private Task Handler_ERR(JObject json)
	{
		return Task.Run(() => Console.WriteLine(string.Format("Server reported an error: {0}",json.ToString())));
	}
#endregion


#region (-) Handler_KID
	/// <summary><b>Character Kinks Data</b><br/>
	/// incoming with the kink data of a character<br/>
	/// (INC) &lt;&lt; KID {"type": "A String", "character": "CharacterName", "value": "A String", "message": "A String", "key": "A String"}
	/// </summary>
	/// <param name="json">the incoming message's contents</param>
	/// <returns>the task we initiated</returns>
	private Task Handler_KID(JObject json)
	{
		return Task.CompletedTask;
	}
#endregion


#region (-) Handler_UPT
	/// <summary><b>Uptime Information</b><br/>
	/// incoming with 'uptime information'<br/>
	/// (INC) &lt;&lt; UPT {"startstring": "A String", "starttime": 1234, "channels": 1234, "maxusers": 1234, "users": 1234, "accepted": 1234, "time": 1234}<br/>
	/// </summary>
	/// <param name="json">the incoming message's contents</param>
	/// <returns>the task we initiated</returns>
	private Task Handler_UPT(JObject json)
	{
		return Task.CompletedTask;
	}
#endregion


#region (-) Handler_VAR
	/// </summary>
	/// <param name="json">the incoming message's contents</param>
	/// <returns>the task we initiated</returns>
	private Task Handler_VAR(JObject json)
	{
		return Task.Run(() => Console.WriteLine(string.Format("Recieved server variable: {0} = {1}",json["variable"].ToString(),json["value"].ToString())));
	}
#endregion
}