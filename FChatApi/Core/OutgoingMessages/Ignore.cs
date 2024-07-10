using System;
using System.Threading.Tasks;
using FChatApi.Enums;
using FChatApi.Attributes;
using System.ComponentModel;
using FChatApi.Objects;

namespace FChatApi.Core;

public partial class ApiConnection
{
#region (+) PerformIgnoreAction
	/// <summary>
	/// <b>System Action</b><br/>
	/// performs one of a number of ignore actions
	/// </summary>
	/// <param name="user">subject of action</param>
	/// <param name="action">the type of action to be performed</param>
	public static Task PerformIgnoreAction(User user, IgnoreAction action)
	{
		string toSend = string.Format(MessageCode.IGN.GetEnumAttribute<MessageCode,OutgoingMessageFormatAttribute>().Format,user.Name,action.ToString());
		Console.WriteLine($"{DateTime.Now.ToShortTimeString()} | {string.Format(action.GetEnumAttribute<IgnoreAction,DescriptionAttribute>().Description,user.Name)}");
#if DEBUG
		return DebugSendAsync(toSend);
#else
		return Client.SendAsync(toSend);
#endif
	}
#endregion
}
