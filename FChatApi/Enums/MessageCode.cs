using FChatApi.Attributes;

namespace FChatApi.Enums;

/// <summary>response codes used by FChat</summary>
public enum MessageCode
{
	/// <summary>empty/invalid default</summary>
	[OutgoingMessageFormat("NON")]
	NON,

	/// <summary><b>Global Account Ban</b><br/>
	/// sent to ban a character and the entire attached account<br/>
	/// (OUT) &gt;&gt; ACB {"character": "CharacterName"}<br/>
	/// <i>"CharacterName" is case sensitive!<br/></i>
	/// </summary>
	[OutgoingMessageFormat("ACB {\"character\": \"{0}\"}")]
	ABC,

	/// <summary><b>Admin List</b><br/>
	/// when incoming provides a list of global ops<br/>
	/// (INC) &lt;&lt; ADL {"ops": Array["OperatorName"]}
	/// </summary>
	[OutgoingMessageFormat("ADL")]
	ADL,

	/// <summary><b>Add Global Operator</b><br/>
	/// incoming when a new character is added to the global ops list<br/>
	/// (INC) &lt;&lt; AOP {"character": "OperatorName"}<br/><br/>
	/// 
	/// sent to add a new character to the global ops list<br/>
	/// (OUT) &gt;&gt; ACB {"character": "OperatorName"}<br/>
	/// <i>"OperatorName" is case sensitive!<br/>
	/// requires you to be a global (super?) operator</i>
	/// </summary>
	[OutgoingMessageFormat("AOP {\"character\": \"{0}\"}")]
	AOP,

	/// <summary><b>Alt-Watch(Unused)</b><br/>
	/// sent to ??<br/>
	/// (OUT) &gt;&gt; AWC {"character": "CharacterName"}<br/>
	/// <i>"CharacterName" is case sensitive!<br/>
	/// returns a "Not Implemented" error if used</i>
	/// </summary>
	[OutgoingMessageFormat("ACB {\"character\": \"{0}\"}")]
	AWC,

	/// <summary><b>Broadcast</b><br/>
	/// incoming when a global broadcast is recieved<br/>
	/// (INC) &lt;&lt; BRO {"character": "OperatorName", "message": "A String"}<br/><br/>
	/// 
	/// sends a a global broadcast to the server<br/>
	/// (OUT) &gt;&gt; BRO {"message": "A String"}
	/// </summary>
	[OutgoingMessageFormat("BRO {\"message\": \"{0}\"}")]
	BRO,

	/// <summary><b>Channel Ban List</b><br/>
	/// send to request a list of banned characters<br/>
	/// (OUT) &gt;&gt; CBL {"channel": "ChannelCode"}<br/>
	/// <i>the ban list itself is returned in a <c>SYS</c> message with the format:</i> <c>Channel bans for $channel: $name1, $name2, ...</c>
	/// </summary>
	[OutgoingMessageFormat("CBL {\"channel\": \"{0}\"}")]
	CBL,

	/// <summary><b>Channel Ban</b><br/>
	/// incoming when a character has been banned from a channel<br/>
	/// (INC) &lt;&lt; CBU {"character": "CharacterName", "channel": "ChannelCode", "operator": "OperatorName"}<br/><br/>
	/// 
	/// send to ban a character from a channel<br/>
	/// (OUT) &gt;&gt; CBU {"character": "CharacterName", "channel": "ChannelCode"}<br/>
	/// <i>"CharacterName" is case sensitive!<br/>
	/// returns <c>ALREADY_CHANNEL_BANNED</c> error if the character is already on the ban list.<br/>
	/// returns <c>DENIED_ON_OP</c> error if used on a global operator</i>
	/// </summary>
	[OutgoingMessageFormat("CBU {\"character\": \"{0}\", \"channel\": \"{1}\"}")]
	CBU,

	/// <summary><b>Create Private Channel</b><br/>
	/// send initiate the channel creation process<br/>
	/// (OUT) &gt;&gt; CCR {"channel": "ChannelName"}<br/>
	/// <i>private channels are created with access mode <c>closed</c> (invite only)<br/>
	/// <c>MAXLENGTH</c> 64 characters<br/>
	/// <c>ENCODING</c> UTF-8<br/>
	/// the name must be a HTML-escaped, but escaped characters each count as a single character respectively<br/>
	/// when successful the api-user is forcibly added to the channel (also triggering a <c>JCH</c> message ?)</i>
	/// </summary>
	[OutgoingMessageFormat("CCR {\"channel\": \"{0}\"}")]
	CCR,

	/// <summary><b>Channel Description</b><br/>
	/// incoming when a channel's description has updated<br/>
	/// (INC) &lt;&lt; CDS {"channel": "ChannelCode", "description": "A String"}<br/><br/>
	/// 
	/// send to update a channel's description<br/>
	/// (OUT) &gt;&gt; CDS {"channel": "ChannelCode", "description": "A String"}<br/>
	/// <i><c>MAXLENGTH</c> ?? characters<br/>
	/// returns a <c>DESCRIPTION_TOO_LONG</c> error if description exceeds max <c>MAXLENGTH</c></i>
	/// </summary>
	[OutgoingMessageFormat("CDS {\"channel\": \"{0}\", \"description\": \"{1}\"}")]
	CDS,

	/// <summary><b>Official Channel List</b><br/>
	/// incoming with a list of official channels<br/>
	/// (INC) &lt;&lt; CHA {"channels": Array[OfficialChannel]}<br/><br/>
	/// 
	/// send to request a list of official channels<br/>
	/// (OUT) &gt;&gt; CHA<br/>
	/// <i><c>cha_flood</c> variable throttles this command</i>
	/// </summary>
	[OutgoingMessageFormat("CHA")]
	CHA,

	/// <summary><b>Channel Invite</b><br/>
	/// incoming when api-user is being invited to a channel<br/>
	/// (INC) &lt;&lt; CIU {"sender": "CharacterName", "title": "ChannelName", "name": "ChannelCode"}<br/><br/>
	/// 
	/// send invite a character to a channel<br/>
	/// (OUT) &gt;&gt; CIU {"character": "CharacterName", "channel": "ChannelCode"}<br/>
	/// <i>"CharacterName" is case sensitive!<br/>
	/// <c>SYS</c> message indicating success will be returned<br/>
	/// cannot be used for official channels<br/>
	/// api-user must be channel operator to perform this action</i>
	/// </summary>
	[OutgoingMessageFormat("CIU {\"character\": \"{0}\", \"channel\": \"{1}\"}")]
	CIU,

	/// <summary><b>Channel Kick</b><br/>
	/// incoming when a user was kicked from a channel<br/>
	/// (INC) &lt;&lt; CKU {"character": "CharacterName", "channel": "ChannelCode", "operator": "OperatorName"}<br/><br/>
	/// 
	/// send invite a character to a channel<br/>
	/// (OUT) &gt;&gt; CKU {"character": "CharacterName", "channel": "ChannelCode"}<br/>
	/// <i>"CharacterName" is case sensitive!<br/>
	/// returns <c>DENIED_ON_OP</c> error if used on a global operator<br/>
	/// the target character must be online and present in the channel<br/>
	/// api-user must be channel operator to perform this action<br/>
	/// if the channel is invite only, this also removes the target's invite</i>
	/// </summary>
	[OutgoingMessageFormat("CKU {\"character\": \"{0}\", \"channel\": \"{1}\"}")]
	CKU,

	/// <summary><b>Channel Operator Add</b><br/>
	/// incoming when a character was promoted to channel operator<br/>
	/// (INC) &lt;&lt; COA {"character": "CharacterName", "channel": "ChannelCode"}<br/><br/>
	/// 
	/// send invite a character to a channel<br/>
	/// (OUT) &gt;&gt; COA {"character": "CharacterName", "channel": "ChannelCode"}<br/>
	/// <i>"CharacterName" is case sensitive!<br/>
	/// api-user must be channel operator to perform this action<br/>
	/// returns <c>COL</c> & <c>COA</c><br/>
	/// also returns <c>SYS</c> unless the silent property is set</i>
	/// </summary>
	[OutgoingMessageFormat("COA {\"character\": \"{0}\", \"channel\": \"{1}\"}")]
	COA,

	/// <summary><b>Channel Operator List</b><br/>
	/// incoming with a list of channel operators<br/>
	/// (INC) &lt;&lt; COL {"channel": "ChannelCode", "oplist": Array["OperatorName"]}<br/><br/>
	/// 
	/// <i>first entry is ChannelOwner may be string.Empty</i>
	/// send to request a list of channel operators<br/>
	/// (OUT) &gt;&gt; COL {"channel": "ChannelCode"}<br/>
	/// <i>api-user need not be in a channel to make this request<br/>
	/// returns <c>COL</c> & <c>SYS</c></i>
	/// </summary>
	[OutgoingMessageFormat("COL {\"channel\": \"{0}\"}")]
	COL,

	/// <summary><b>Connection Count</b><br/>
	/// incoming with count of characters connected to chat<br/>
	/// (INC) &lt;&lt; CON {"count": 1234}
	/// </summary>
	[OutgoingMessageFormat("CON")]
	CON,

	/// <summary><b>Channel Operator Remove</b><br/>
	/// incoming when a character was demoted from channel operator<br/>
	/// (INC) &lt;&lt; COR {"character": "CharacterName", "channel": "ChannelCode"}<br/><br/>
	/// 
	/// send to demote a character from channel operator<br/>
	/// (OUT) &gt;&gt; COR {"character": "CharacterName", "channel": "ChannelCode"}<br/>
	/// <i>"CharacterName" is case sensitive!<br/>
	/// channel operators cannot be removed from the channel operator list and must be reassigned with <c>CSO</c> instead<br/>
	/// returns <c>COL</c> & <c>COR</c><br/>
	/// also returns <c>SYS</c> unless the silent property is set</i>
	/// </summary>
	[OutgoingMessageFormat("COR {\"character\": \"{0}\", \"channel\": \"{1}\"}")]
	COR,

	/// <summary><b>Create Public Channel</b><br/>
	/// send to request the creation of a public channel<br/>
	/// (OUT) &gt;&gt; DOP {"CRC": "ChannelCode"}<br/>
	/// <i><b>requires global operator rights</b><br/>
	/// returns <c>SYS</c> on success</i>
	/// </summary>
	[OutgoingMessageFormat("CRC {\"channel\": \"{1}\"}")]
	CRC,

	/// <summary><b>Channel Set Owner</b><br/>CSO {"character": "Kira", "channel": "Development"}
	/// incoming when a character was promoted to channel owner<br/>
	/// (INC) &lt;&lt; CSO {"character": "CharacterName", "channel": "ChannelCode"}<br/><br/>
	/// 
	/// send to reassign channel owner<br/>
	/// (OUT) &gt;&gt; CSO {"character": "CharacterName", "channel": "ChannelCode"}<br/>
	/// <i>"CharacterName" is case sensitive!<br/>
	/// requires you to be the channel owner</i>
	/// </summary>
	[OutgoingMessageFormat("CSO {\"character\": \"{0}\", \"channel\": \"{1}\"}")]
	CSO,

	/// <summary><b>Channel Timeout</b><br/>
	/// incoming when a character was timed out from a channel<br/>
	/// (INC) &lt;&lt; CTU {"character": "CharacterName", "channel": "ChannelCode", "operator": "OperatorName", "length": 1234}<br/>
	/// <i>length is measured in seconds ?</i><br/><br/>
	/// 
	/// send to time out a character from a channel<br/>
	/// (OUT) &gt;&gt; CTU {"character": "CharacterName", "channel": "ChannelCode", "length": 1234}<br/>
	/// <i>"CharacterName" is case sensitive!<br/>
	/// length is measured in seconds ?<br/>
	/// requires you to be the channel operator</i><br/>
	/// &gt; 300 = 5 minutes, 900 = 15 minutes, 1800 = 30 minutes,<br/>
	/// &gt; 3600 = 1 hour, 7200 = 1 hours, 28800 = 8 hours,<br/>
	/// &gt; 86400 = 1 day, 604800 = 1 week, 1209600 = 2 weeks<br/>
	/// </summary>
	[OutgoingMessageFormat("CTU {\"character\": \"{0}\", \"channel\": \"{1}\", \"length\": {2}}")]
	CTU,

	/// <summary><b>Channel Remove Ban/Timeout</b><br/>
	/// send to end a character's ban/timeout<br/>
	/// (OUT) &gt;&gt; CUB {"character": "CharacterName", "channel": "ChannelCode"}<br/>
	/// <i>"CharacterName" is case sensitive!<br/>
	/// length is measured in seconds ?<br/>
	/// requires you to be a channel operator</i>
	/// </summary>
	[OutgoingMessageFormat("CUB {\"character\": \"{0}\", \"channel\": \"{1}\"}")]
	CUB,

	/// <summary><b>Remove Global Operator</b><br/>
	/// incoming when a character was demoted from global operator<br/>
	/// (INC) &lt;&lt; DOP {"character": "OperatorName"}<br/><br/>
	/// 
	/// send to demote a character from global operator<br/>
	/// (OUT) &gt;&gt; CUB {"character": "CharacterName"}<br/>
	/// <i>"CharacterName" is case sensitive!<br/>
	/// requires you to be global (super?) operator</i>
	/// </summary>
	[OutgoingMessageFormat("DOP {\"character\": \"{0}\"}")]
	DOP,

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
	[OutgoingMessageFormat("ERR")]
	ERR,

	/// <summary><b>Offline User Notification</b><br/>
	/// incoming when a character has gone offline<br/>
	/// (INC) &lt;&lt; FLN {"character": "CharacterName"}
	/// </summary>
	[OutgoingMessageFormat("FLN")]
	FLN,

	/// <summary><b>Friends List</b><br/>
	/// incoming with a list of api-user's friends<br/>
	/// (INC) &lt;&lt; FRL {"characters": Array["FriendName"]}
	/// </summary>
	[OutgoingMessageFormat("FRL")]
	FRL,

	/// <summary><b>Server Welcome Message</b><br/>
	/// incoming when initially connecting to the server<br/>
	/// (INC) &lt;&lt; HLO {"message": "A String"}<br/>
	/// <i>the message contains the server version</i>
	/// </summary>
	[OutgoingMessageFormat("HLO")]
	HLO,

	/// <summary><b>Channel User List</b><br/>
	/// incoming with a list of characters in this channel<br/>
	/// (INC) &lt;&lt; ICH {"channel": "ChannelName", "users": Array[ChannelCharacter], "mode": "A String"}
	/// </summary>
	[OutgoingMessageFormat("ICH")]
	ICH,

	/// <summary><b>Chat Login</b><br/>
	/// incoming when api-user has successfully identified with the server<br/>
	/// (INC) &lt;&lt; IDN {"character": "ApiUserName"}<br/>
	/// <i><b>DO NOT</b> issue chat commands until you have also recieved the NLN command with the current character's name!!<br/>
	/// Doing so will cause desynchronization with the server.</i><br/><br/>
	/// 
	/// send to demote a character from global operator<br/>
	/// (OUT) &gt;&gt; IDN {"character": "CharacterName", "cversion": "ClientVersion", "method": "AuthorizationMethod", "account": "AccountName", "ticket": "TicketInformation", "cname": "ClientName"}<br/>
	/// <i>"CharacterName" is case sensitive!<br/>
	/// method should be <c>ticket</c><br/>
	/// returns <c>IDENT_FAILED</c> if login details or ticket were not valid. clients should not use the same ticket again.<br/>
	/// returns <c>TOO_MANY_FROM_IP</c> if too many clients connected from this IP-Address. Do not reconnect.<br/>
	/// returns <c>BANNED_FROM_SERVER</c> if you are banned from the server. Do not reconnect.<br/>
	/// returns <c>LOGGED_IN_AGAIN</c> if the active connection is being terminated and replaced. Do not automatically reconnect.<br/>
	/// returns <c>UNKNOWN_AUTH_METHOD</c> if <c>ticket</c> method was not used. Do not automatically reconnect.<br/>
	/// returns <c>NO_LOGIN_SLOTS</c> if already connected with too many characters. Automatically reconnection should be staggered/delayed.<br/>
	/// returns <c>SERVER_FULL</c> if the chat server is full. Do not automatically reconnect.<br/>
	/// returns <c>ALREADY_IDENT</c> if you are already identified with the server or you have a pending IDN request.</i>
	/// </summary>
	[OutgoingMessageFormat("IDN {\"account\": : \"{0}\", \"character\": \"{1}\", \"method\": \"{2}\", \"ticket\": \"{3}\", \"cname\": \"{4}\", \"cversion\": \"{5}\"}")]
	IDN,

	/// <summary><b>Ignore</b><br/>
	/// send to perform various ignore functionality actions<br/>
	/// (OUT) &gt;&gt; IGN {"character": "CharacterName", "action": "IgnoreAction"}<br/>
	/// <i>"CharacterName" is case sensitive!</i><br/>
	/// &gt; action <c>list</c> requests a list of api-user's ignored characters ?<br/>
	/// &gt; action <c>add</c> adds a character to api-user's ignore list<br/>
	/// &gt; action <c>remove</c> removes a character from api-user's ignore list. removing <c>*</c> clears the api-user's ignore list<br/>
	/// &gt; action <c>notify</c> notifies a character that their private (<c>PRI</c>) message was ignored.
	/// <br/><i>Client should notify for each recieved private message from character's on api-user's ignore list.</i>
	/// </summary>
	[OutgoingMessageFormat("IGN {\"channel\": \"{0}\", \"action\": \"{1}\"}")]
	IGN,

	/// <summary><b>Join Channel</b><br/>
	/// incoming when api-user has joined a channel<br/>
	/// (INC) &lt;&lt; JCH {"character": ChannelCharacter, "channel": "ChannelCode", "title": "ChannelName"}<br/><br/>
	/// 
	/// send to atempt to join a channel<br/>
	/// (OUT) &gt;&gt; JCH {"channel": "ChannelCode"}
	/// </summary>
	[OutgoingMessageFormat("JCH {\"channel\": \"{0}\"}")]
	JCH,

	/// <summary><b>Destroy Channel</b><br/>
	/// send to atempt to destroy a channel<br/>
	/// (OUT) &gt;&gt; KIC {"channel": "ChannelCode"}<br/>
	/// <i>api-user must be channel owner to do this</i>
	/// </summary>
	[OutgoingMessageFormat("KIC {\"channel\": \"{0}\"}")]
	KIC,

	/// <summary><b>Character Kinks Data</b><br/>
	/// incoming with the kink data of a character<br/>
	/// (INC) &lt;&lt; KID {"type": "A String", "character": "CharacterName", "value": "A String", "message": "A String", "key": "A String"}
	/// </summary>
	[OutgoingMessageFormat("KID")]
	KID,

	/// <summary><b>Global Chat Kick</b><br/>
	/// send to atempt to kick a character and their client from the server<br/>
	/// (OUT) &gt;&gt; KIK {"character": "CharacterName"}<br/>
	/// <i>"CharacterName" is case sensitive!<br/>
	/// api-user must be channel owner to do this</i>
	[OutgoingMessageFormat("KIK {\"character\": \"{0}\"}")]
	KIK,

	/// <summary><b>Leave Channel</b><br/>
	/// incoming when a character leaves a channel<br/>
	/// (INC) &lt;&lt; LCH {"character": "CharacterName", "channel": "ChannelCode"}<br/><br/>
	/// 
	/// send to atempt to leave a channel<br/>
	/// (OUT) &gt;&gt; LCH {"channel": "ChannelCode"}
	/// </summary>
	[OutgoingMessageFormat("LCH {\"channel\": \"{0}\"}")]
	LCH,

	/// <summary><b>Character List</b><br/>
	/// api-user's character list and online status ?
	/// </summary>
	[OutgoingMessageFormat("LIS")]
	LIS,

	/// <summary><b>Channel Advertisement Message</b><br/>
	/// incoming when a character posts an ad in a channel<br/>
	/// (INC) &lt;&lt; LRP {"character": "CharacterName", "channel": "ChannelCode", "message": "A String"}<br/><br/>
	/// 
	/// send to post an ad in a channel<br/>
	/// (OUT) &gt;&gt; LRP {"channel": "ChannelCode", "message": "A String"}
	/// </summary>
	[OutgoingMessageFormat("LRP {\"channel\": \"{0}\", \"message\": \"{1}\"}")]
	LRP,

	/// <summary><b>Channel Message</b><br/>
	/// incoming when a character posts a chat message in a channel<br/>
	/// (INC) &lt;&lt; MSG {"character": "CharacterName", "channel": "ChannelCode", "message": "A String"}<br/><br/>
	/// 
	/// send to post a chat message in a channel<br/>
	/// (OUT) &gt;&gt; LRP {"channel": "ChannelCode", "message": "A String"}<br/>
	/// &gt; messages are throttled based on <c>msg_flood</c> variable<br/>
	/// &gt; message length determined by <c>chat_max</c> variable
	/// </summary>
	[OutgoingMessageFormat("MSG {\"channel\": \"{0}\", \"message\": \"{1}\"}")]
	MSG,

	/// <summary><b>Channel Message</b><br/>
	/// incoming when a character has come online<br/>
	/// (INC) &lt;&lt; NLN {"gender": "A String", "status": "A String", "identity": "CharacterName"}<br/>
	/// <i>This includes the api-user! Wait to recieve this message before issuing any chat commands!!</i>
	/// </summary>
	[OutgoingMessageFormat("NLN")]
	NLN,

	/// <summary><b>Private Channel List</b><br/>
	/// incoming with a list of private chanels<br/>
	/// (INC) &lt;&lt; ORS {"channels": Array[PrivateChannel]}
	/// </summary>
	[OutgoingMessageFormat("ORS")]
	ORS,

	/// <summary>a ping</summary>
	[OutgoingMessageFormat("PIN")]
	PIN,

	/// <summary><b>Private Message</b><br/>
	/// incoming with a private message from another character<br/>
	/// (INC) &lt;&lt; PRI {"character": "CharacterName", "message": "A String", "recipient": "ApiUserCharacterName"}<br/><br/>
	/// 
	/// send to post a chat message in a channel<br/>
	/// (OUT) &gt;&gt; PRI {"message": "A String", "recipient": "CharacterName"}<br/><br/>
	/// <i>"CharacterName" is case sensitive!</i><br/>
	/// &gt; messages are throttled based on <c>msg_flood</c> variable<br/>
	/// &gt; messages length determined by <c>priv_max</c> variable
	/// </summary>
	[OutgoingMessageFormat("PRI {\"message\": \"{0}\", \"recipient\": \"{1}\"}")]
	PRI,

	/// <summary><b>Reload Serzer State</b><br/>
	/// ??? <br/>
	/// (OUT) &gt;&gt; RLD {"save": "A String"}
	/// </summary>
	[OutgoingMessageFormat("RLD {\"save\": \"{0}\"}")]
	RLD,

	/// <summary><b>Dice Roll/Bottle Spin</b><br/>
	/// incoming when a character rolls dice or spins the bottle<br/>
	/// (INC) &lt;&lt; RLL {"character": "CharacterName", "channel": "ChannelCode", "message": "A String", "type": "CommandType", "results": Array[1234], "endresult": 1234, "rolls": Array["RollResults"], "target": "TargetCharacterName"}<br/><br/>
	/// 
	/// send to roll a dice string or spin the bottle<br/>
	/// (OUT) &gt;&gt; RLL {"channel": "ChannelCode", "dice": "A String"}
	/// </summary>
	[OutgoingMessageFormat("RLL {\"channel\": \"{0}\", \"dice\": \"{1}\"}")]
	RLL,

	/// <summary><b>Channel Message Mode</b><br/>
	/// incoming with the message mode of a channel<br/>
	/// (INC) &lt;&lt; RMO {"channel": "ChannelCode", "mode": "MessageMode"}<br/><br/>
	/// 
	/// send to request a channel's message mode<br/>
	/// (OUT) &gt;&gt; RMO {"channel": "ChannelCode", "mode": "A String"}
	/// </summary>
	[OutgoingMessageFormat("RMO {\"channel\": \"{0}\", \"mode\": \"{1}\"}")]
	RMO,

	/// <summary><b>Channel Visibility</b><br/>
	/// send to ??<br/>
	/// (OUT) &gt;&gt; RST {"channel": "ChannelCode", "mode": "VisibilityMode"}
	/// </summary>
	[OutgoingMessageFormat("RST {\"channel\": \"{0}\", \"mode\": \"{1}\"}")]
	RST,

	/// <summary><b>Reward User</b><br/>
	/// send to ??<br/>
	/// (OUT) &gt;&gt; RWD {"character": "CharacterName"}<br/>
	/// <i>"CharacterName" is case sensitive!</i>
	/// </summary>
	[OutgoingMessageFormat("RWD {\"character\": \"{0}\"}")]
	RWD,

	/// <summary><b>Staff Alert</b><br/>
	/// incoming with a staff alert ?<br/>
	/// (INC) &lt;&lt; SFC {"character": "CharacterName", "moderator": "OperatorName", "logid": 1234, "action": "A String(??)", "timestamp": 1234, "report": "A String(??)", "callid": 1234}<br/><br/>
	/// 
	/// send to issue a staff alert ?<br/>
	/// (OUT) &gt;&gt; SFC {"report": "A String", "callid": 1234, "action": "A String", "logid": "A String"}
	/// </summary>
	[OutgoingMessageFormat("SFC {\"report\": \"{0}\", \"callid\": \"{1}\", \"action\": \"{2}\", \"logid\": \"{3}\"}")]
	SFC,

	/// <summary><b>Status Update</b><br/>
	/// incoming with a staff alert ?<br/>
	/// (INC) &lt;&lt; STA {"character": "CharacterName", "status": "UserStatusType", "statusmsg": "A String"}<br/><br/>
	/// 
	/// when outgoing requests status set<br/>
	/// (OUT) &lt;&lt; STA {"status": "UserStatusType", "statusmsg": "A String"}
	/// </summary>
	[OutgoingMessageFormat("STA {\"status\": \"{0}\", \"statusmsg\": \"{1}\"}")]
	STA,

	/// <summary><b>System Message</b><br/>
	/// incoming with a system message<br/>
	/// (INC) &lt;&lt; SYS {"message": "A String"}
	/// </summary>
	[OutgoingMessageFormat("SYS")]
	SYS,

	/// <summary><b>Global Chat Timeout</b><br/>
	/// send to globally time out a character and their client<br/>
	/// (OUT) &lt;&lt; TMO {"character": "CharacterName", "reason": "A String", "time": "A String"}<br/>
	/// <i>"CharacterName" is case sensitive!<br/>
	/// requires you to be a global operator<br/>
	/// length is measured in seconds.</i><br/>
	/// &gt; 300 = 5 minutes, 900 = 15 minutes, 1800 = 30 minutes,<br/>
	/// &gt; 3600 = 1 hour, 7200 = 1 hours, 28800 = 8 hours,<br/>
	/// &gt; 86400 = 1 day, 604800 = 1 week, 1209600 = 2 weeks<br/>
	/// </summary>
	[OutgoingMessageFormat("STA {\"character\": \"{0}\", \"reason\": \"{1}\", \"time\": \"{2}\"}")]
	TMO,

	/// <summary><b>Typing Notification</b><br/>
	/// incoming with a notification that a character is typing<br/>
	/// (INC) &lt;&lt; TPN {"character": "CharacterName", "status": "TypingStatusType"}<br/>
	/// <i>"status" must indicate 'has entered text' or 'active typing' ?</i><br/><br/>
	/// 
	/// send to indicate that the api-user is typing or has entered text<br/>
	/// (OUT) &lt;&lt; TPN {"character": "CharacterName", "status": "A String"}<br/>
	/// <i>"CharacterName" is case sensitive!</i>
	/// </summary>
	[OutgoingMessageFormat("STA {\"character\": \"{0}\", \"status\": \"{1}\"}")]
	TPN,

	/// <summary><b>Uptime Information</b><br/>
	/// incoming with 'uptime information'<br/>
	/// (INC) &lt;&lt; UPT {"startstring": "A String", "starttime": 1234, "channels": 1234, "maxusers": 1234, "users": 1234, "accepted": 1234, "time": 1234}<br/><br/>
	/// 
	/// send to indicate that the api-user is typing or has entered text<br/>
	/// (OUT) &lt;&lt; UPT {"character": "CharacterName"}<br/>
	/// <i>"CharacterName" is case sensitive!</i>
	/// </summary>
	[OutgoingMessageFormat("STA {\"character\": \"{0}\"}")]
	UPT,

	/// <summary><b>Remove Global Ban</b><br/>
	/// send to remove a global ban from a character and their account<br/>
	/// (OUT) &lt;&lt; UNB {"character": "CharacterName"}<br/>
	/// <i>"CharacterName" is case sensitive!<br/>
	/// requires you to be a global operator</i>
	/// </summary>
	[OutgoingMessageFormat("UNB {\"character\": \"{0}\"}")]
	UNB,

	/// <summary><b>Server Variable</b><br/>
	/// incoming with a server variable<br/>
	/// (INC) &lt;&lt; VAR {"value": 1.234, "variable": "A String"}
	/// </summary>
	[OutgoingMessageFormat("VAR")]
	VAR,
}