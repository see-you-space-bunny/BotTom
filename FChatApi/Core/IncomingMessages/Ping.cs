using System;
using System.Threading.Tasks;
using FChatApi.Enums;
using Newtonsoft.Json.Linq;

namespace FChatApi.Core;

public partial class ApiConnection
{
#region (-) Handler_PIN
	/// <summary>The keep-alive ping that maintains the api's connection.</summary>
	private Task Handler_PIN()
	{
		return Client.SendAsync(MessageCode.PIN.ToString());
	}
#endregion
}
