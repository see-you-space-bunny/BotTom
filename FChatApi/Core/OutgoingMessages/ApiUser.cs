using System;
using System.Threading.Tasks;
using FChatApi.Enums;
using FChatApi.Attributes;
using System.ComponentModel;

namespace FChatApi.Core;

public partial class ApiConnection
{
#region (+) SetStatus
	/// <summary>
	/// <b>System Action</b><br/>
	/// sets the api-user's status and status message
	/// </summary>
	/// <param name="statusMessage">the status message to set</param>
	/// <param name="status">the status type to set</param>
	public static Task User_SetStatus(ChatStatus status,string statusMessage)
	{
		string toSend = string.Format(MessageCode.STA.GetEnumAttribute<MessageCode,OutgoingMessageFormatAttribute>().Format,status.ToString(),statusMessage);
		Console.WriteLine($"{DateTime.Now.ToShortTimeString()} | New status: {status} -- {statusMessage}");
#if DEBUG
		return DebugSendAsync(toSend);
#else
		return Client.SendAsync(toSend);
#endif
	}
#endregion
}
