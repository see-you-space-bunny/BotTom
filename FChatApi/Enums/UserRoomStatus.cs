namespace FChatApi.Enums;

/// <summary>moderation actions ?SPECULATION</summary>
public enum UserRoomStatus
{
	/// <summary>empty/invalid, failthrough default</summary>
	Invalid			= 0x00,

	/// <summary>values greater than this have an associated (ACTION) or (STATE)</summary>
	AllValid		= 0x01,

	/// <summary>values greater than this have a valid (ACTION)</summary>
	AllValidActions	= 0x10,

	/// <summary>
	/// (STATE)		do not set a character's status to this, the action makes them a <c>User</c> instead<br/>
	/// (ACTION)	unbans/untimeouts a character from the channel
	/// </summary>
	UnBanned		= 0x11,

	/// <summary>
	/// (STATE)		do not set a character's status to this, the action makes them a <c>User</c> instead<br/>
	/// (ACTION)	invite character to the channel
	/// </summary>
	Invited			= 0x12,

	/// <summary>
	/// (STATE)		do not set a character's status to this, the action makes them a <c>User</c> instead<br/>
	/// (ACTION)	invite character to the channel
	/// </summary>
	Demoted			= 0x13,

	/// <summary>
	/// (STATE)		do not set a character's status to this, the action makes them a <c>User</c> instead (<i>it may not demote channel operators ?SPECULATION</i>)<br/>
	/// (ACTION)	kicks character from the channel
	/// </summary>
	Kicked			= 0x14,

	/// <summary>values greater than this have a valid (STATE)</summary>
	AllValidStates	= 0x30,

	/// <summary>
	/// (STATE)		character is timed out from the channel<br/>
	/// (ACTION)	times out character from the channel
	/// </summary>
	Timeout			= 0x40,

	/// <summary>
	/// (STATE)		character is banned from this channel<br/>
	/// (ACTION)	bans a character from the channel
	/// </summary>
	Banned			= 0x41,


	/// <summary>
	/// (STATE)		character is a basic user of the channel<br/>
	/// (ACTION)	resets the character's relationship with the channel<br/>
	/// <i>this will make them a basic user, taking whatever actions required, such as unbanning them or demoting them</i>
	/// </summary>
	User			= 0x77,


	/// <summary>
	/// (STATE)		character is a trusted user of the channel<br/>
	/// (ACTION)	makes the character a trusted user in the channel<br/>
	/// <i>this is a purely internal state and does not send out any Api requests</i>
	/// </summary>
	TrustedUser		= 0xAA,


	/// <summary>
	/// (STATE)		character is an operator of the channel<br/>
	/// (ACTION)	promotes a character to channel operator
	/// </summary>
	/// <summary>promote user to operator</summary>
	Moderator	= 0xDD,


	/// <summary>
	/// (STATE)		character is channel owner<br/>
	/// (ACTION)	transfers channel ownership to a character
	/// </summary>
	Owner		= 0xFF,
}