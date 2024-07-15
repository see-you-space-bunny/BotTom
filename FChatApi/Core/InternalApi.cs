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
	/// <summary>
	/// 
	/// </summary>
	/// <param name="code"></param>
	/// <param name="message"></param>
	/// <returns></returns>
	/// <exception cref="ArgumentException">if an invalid message code was provided</exception>
	public void ParseMessage(MessageCode code, string message)
	{
		JObject json = code switch {
			MessageCode.NON => throw new ArgumentException("Invalid (NON) message code.",nameof(code)),
			MessageCode.PIN => null,
			_ => ParseToJObject(message, code),
		};

		switch (code)
		{
#region /// ping
			case MessageCode.PIN: _ = Handler_PIN();		break;
#endregion


#region /// messages
			case MessageCode.MSG: Handler_MSG(json);	break;
			case MessageCode.PRI: Handler_PRI(json);	break;
			case MessageCode.LRP: Handler_LRP(json);	break;
			case MessageCode.RLL: Handler_RLL(json);	break;
			case MessageCode.TPN: Handler_TPN(json);	break;
#endregion


#region /// user activity
			case MessageCode.NLN: Handler_NLN(json);	break;
			case MessageCode.FLN: Handler_FLN(json);	break;
			case MessageCode.STA: Handler_STA(json);	break;
#endregion


#region /// channel activity
			case MessageCode.JCH: Handler_JCH(json);	break;
			case MessageCode.LCH: Handler_LCH(json);	break;
			case MessageCode.ICH: Handler_ICH(json);	break;
			case MessageCode.COL: Handler_COL(json);	break;
			case MessageCode.CIU: Handler_CIU(json);	break;
			case MessageCode.CKU: Handler_CKU(json);	break;
			case MessageCode.CTU: Handler_CTU(json);	break;
			case MessageCode.CBU: Handler_CBU(json);	break;
			case MessageCode.COA: Handler_COA(json);	break;
			case MessageCode.COR: Handler_COR(json);	break;
			case MessageCode.CSO: Handler_CSO(json);	break;
			case MessageCode.RMO: Handler_RMO(json);	break;
			case MessageCode.CDS: Handler_CDS(json);	break;
#endregion


#region /// about api-user
			case MessageCode.LIS: Handler_LIS(json);	break;
			case MessageCode.FRL: Handler_FRL(json);	break;
#endregion


#region /// info / error
			case MessageCode.KID: Handler_KID(json);	break;
			case MessageCode.UPT: Handler_UPT(json);	break;
			case MessageCode.VAR: Handler_VAR(json);	break;
			case MessageCode.ERR: Handler_ERR(json);	break;
#endregion


#region /// global op alerts
			case MessageCode.AOP: Handler_AOP(json);	break;
			case MessageCode.DOP: Handler_DOP(json);	break;
			case MessageCode.BRO: Handler_BRO(json);	break;
#endregion


#region /// on-login events
			case MessageCode.ORS: Handler_ORS(json);	break;
			case MessageCode.CHA: Handler_CHA(json);	break;
			case MessageCode.ADL: Handler_ADL(json);	break;
			case MessageCode.CON: Handler_CON(json);	break;
			case MessageCode.IDN: Handler_IDN(json);	break;
			case MessageCode.HLO: Handler_HLO(json);	break;
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