using System;
using System.Threading.Tasks;
using FChatApi.Enums;
using FChatApi.Attributes;
using System.ComponentModel;

namespace FChatApi.Core;

public partial class ApiConnection
{
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
