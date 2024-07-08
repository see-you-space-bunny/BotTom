using System;
using System.Threading.Tasks;
using FChatApi.Enums;
using FChatApi.Attributes;
using System.ComponentModel;

namespace FChatApi.Core;

public partial class ApiConnection
{
#region (-) ConnectionCheck
	/// <summary>
	/// <b>System Action</b><br/>
	/// throws an exception if the api-user is not connected<br/>
	/// </summary>
	/// <exception cref="Exception">if the api-user is not connected</exception>
	private static void ConnectionCheck()
	{
		if (!IsConnected())
			throw new Exception("You must be connected to chat to do this.");
	}
#endregion


////////////////////////////////////////////////


#region (+) SetStatus
	/// <summary>
	/// <b>System Action</b><br/>
	/// sets the api-user's status and status message
	/// </summary>
	/// <param name="statusMessage">the status message to set</param>
	/// <param name="status">the status type to set</param>
	public static Task User_SetStatus(string statusMessage, ChatStatus status)
	{
		string toSend = string.Format(MessageCode.STA.GetEnumAttribute<MessageCode,OutgoingMessageFormatAttribute>().Format,statusMessage,status.ToString());
		Console.WriteLine($"{DateTime.Now.ToShortTimeString()} | New status: {status} -- {statusMessage}");
#if DEBUG
		return DebugSendAsync(toSend);
#else
		return Client.SendAsync(toSend);
#endif
	}
#endregion


#region (-) PerformIgnoreAction
	/// <summary>
	/// <b>System Action</b><br/>
	/// performs one of a number of ignore actions
	/// </summary>
	/// <param name="charactername">character name, subject of action</param>
	/// <param name="action">the type of action to be performed</param>
	private static Task PerformIgnoreAction(string charactername, IgnoreAction action)
	{
		string toSend = string.Format(MessageCode.IGN.GetEnumAttribute<MessageCode,OutgoingMessageFormatAttribute>().Format,charactername,action.ToString());
		Console.WriteLine($"{DateTime.Now.ToShortTimeString()} | {string.Format(action.GetEnumAttribute<IgnoreAction,DescriptionAttribute>().Description,charactername)}");
#if DEBUG
		return DebugSendAsync(toSend);
#else
		return Client.SendAsync(toSend);
#endif
	}
#endregion


#region (+) RequestIgnoreList
	/// <summary>
	/// <b>System Action</b><br/>
	/// requests a list of ignored characters ? or requests to know if charactername is on the api-user's ignore list ?
	/// </summary>
	/// <param name="charactername">character name ? should this be the api-user character's name</param>
	public static Task User_RequestIgnoreList(string charactername) =>
		PerformIgnoreAction(charactername, IgnoreAction.list);
#endregion


#region (+) IgnoreCharacter
	/// <summary>
	/// <b>System Action</b><br/>
	/// informs a character that their message has been ignored
	/// </summary>
	/// <param name="charactername">character name</param>
	private static Task User_IgnoreCharacter(string charactername) =>
		PerformIgnoreAction(charactername, IgnoreAction.add);
#endregion


#region (+) UnignoreCharacter
	/// <summary>
	/// <b>System Action</b><br/>
	/// informs a character that their message has been ignored
	/// </summary>
	/// <param name="charactername">character name</param>
	private static Task User_UnignoreCharacter(string charactername) =>
		PerformIgnoreAction(charactername, IgnoreAction.remove);
#endregion


#region (-) SendMessageIgnoredNotification
	/// <summary>
	/// <b>System Action</b><br/>
	/// informs a character that their message has been ignored
	/// </summary>
	/// <param name="charactername">character name</param>
	private static Task SendMessageIgnoredNotification(string charactername) =>
		PerformIgnoreAction(charactername, IgnoreAction.notify);
#endregion
}
