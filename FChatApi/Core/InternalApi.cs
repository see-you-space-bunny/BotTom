using System;
using System.Text;
using WatsonWebsocket;
using System.Linq;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using FChatApi.Systems;
using FChatApi.Objects;
using FChatApi.Enums;
using FChatApi.EventArguments;
using System.Net.WebSockets;

namespace FChatApi.Core;

public partial class ApiConnection
{
#region Constructor
	public ApiConnection()
	{ }
#endregion


///////////////////////////////////////////////////


#region (-) ConnectionCheck
	/// <summary>
	/// throws an exception if the api-user is not connected<br/>
	/// </summary>
	/// <exception cref="Exception">if the api-user is not connected</exception>
	private static void ConnectionCheck()
	{
		if (!IsConnected())
			throw new WebSocketException("You must be connected to chat to do this.");
	}
#endregion


///////////////////////////////////////////////////


#region (-) ParseToJObject
	private static JObject ParseToJObject(string message, MessageCode value)
	{
		JObject json;

		try
		{
			if (message.Split(' ').Length <= 1)
			{
				if (string.Equals(value.ToString(), message))
				{
					return null;
				}

				json = JObject.Parse(message);
			}
			else
			{
				json = JObject.Parse(message.Replace(value.ToString(), "").TrimStart());
			}
		}
		catch
		{
			throw new Exception($"Failure to parse message: {message}");
		}

		return json;
	}
#endregion


///////////////////////////////////////////////////


#region (-) ParseMessage
	public async Task ParseMessage(MessageCode messageCode, string message)
	{
		JObject json = messageCode switch {
			MessageCode.NON => throw new ArgumentException("Invalid (NON) message code.",nameof(messageCode)),
			MessageCode.PIN => null,
			_ => ParseToJObject(message, messageCode),
		};

		switch (messageCode)
		{
#region /// ping
			case MessageCode.PIN: await Handler_PIN();		break;
#endregion


#region /// messages
			case MessageCode.MSG: await Handler_MSG(json);	break;
			case MessageCode.PRI: await Handler_PRI(json);	break;
			case MessageCode.LRP: await Handler_LRP(json);	break;
			case MessageCode.RLL: await Handler_RLL(json);	break;
			case MessageCode.TPN: await Handler_TPN(json);	break;
#endregion


#region /// user activity
			case MessageCode.NLN: await Handler_NLN(json);	break;
			case MessageCode.FLN: await Handler_FLN(json);	break;
			case MessageCode.STA: await Handler_STA(json);	break;
#endregion


#region /// channel activity
			case MessageCode.JCH: await Handler_JCH(json);	break;
			case MessageCode.LCH: await Handler_LCH(json);	break;
			case MessageCode.ICH: await Handler_ICH(json);	break;
			case MessageCode.COL: await Handler_COL(json);	break;
			case MessageCode.CIU: await Handler_CIU(json);	break;
			case MessageCode.CKU: await Handler_CKU(json);	break;
			case MessageCode.CTU: await Handler_CTU(json);	break;
			case MessageCode.CBU: await Handler_CBU(json);	break;
			case MessageCode.COA: await Handler_COA(json);	break;
			case MessageCode.COR: await Handler_COR(json);	break;
			case MessageCode.CSO: await Handler_CSO(json);	break;
			case MessageCode.RMO: await Handler_RMO(json);	break;
			case MessageCode.CDS: await Handler_CDS(json);	break;
#endregion


#region /// about api-user
			case MessageCode.LIS: await Handler_LIS(json);	break;
			case MessageCode.FRL: await Handler_FRL(json);	break;
#endregion


#region /// info / error
			case MessageCode.KID: await Handler_KID(json);	break;
			case MessageCode.UPT: await Handler_UPT(json);	break;
			case MessageCode.VAR: await Handler_VAR(json);	break;
			case MessageCode.ERR: await Handler_ERR(json);	break;
#endregion


#region /// global op alerts
			case MessageCode.AOP: await Handler_AOP(json);	break;
			case MessageCode.DOP: await Handler_DOP(json);	break;
			case MessageCode.BRO: await Handler_BRO(json);	break;
#endregion


#region /// on-login events
			case MessageCode.ORS: await Handler_ORS(json);	break;
			case MessageCode.CHA: await Handler_CHA(json);	break;
			case MessageCode.ADL: await Handler_ADL(json);	break;
			case MessageCode.CON: await Handler_CON(json);	break;
			case MessageCode.IDN: await Handler_IDN(json);	break;
			case MessageCode.HLO: await Handler_HLO(json);	break;
#endregion


#region /// do-not-handle
			// should not be handled by bot
			// MessageCode.SFC		// staff notification

			// was on the switch for incoming messages initially,
			// but IGN seems to have no known (INC) message format
			// MessageCode.IGN		// ignore list
#endregion

			default: break;
		}
	}
#endregion
}