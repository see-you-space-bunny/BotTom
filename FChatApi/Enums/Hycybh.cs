namespace FChatApi.Enums;

/// <summary>response codes used by FChat</summary>
public enum Hycybh
{
	/// <summary>empty/invalid default</summary>
	NON	= 0x00,

	/// <summary>user is typing</summary>
	TPN	= 0x01,

	/// <summary>channel message sent/received</summary>
	MSG	= 0x02,

	/// <summary>connected or requesting chat connection</summary>
	IDN	= 0x03,

	/// <summary>private message sent/received</summary>
	PRI	= 0x04,

	/// <summary>public channel list received </summary>
	CHA	= 0x05,

	/// <summary>private channel list received</summary>
	ORS	= 0x06,

	/// <summary>joined or requesting to join a channel</summary>
	JCH	= 0x07,

	/// <summary>left or requesting to leave a channel</summary>
	LCH	= 0x08,

	/// <summary>a ping</summary>
	PIN	= 0x09,

	/// <summary>requests status set</summary>
	STA	= 0x0A,

	/// <summary>advertisement</summary>
	LRP	= 0x0B,

	/// <summary>create(d) a private, invite-only channel</summary>
	CCR	= 0x0C,

	/// <summary>chat system variables</summary>
	VAR	= 0x0D,

	/// <summary>server hello command</summary>
	HLO	= 0x0E,

	/// <summary>returns number of connected users</summary>
	CON	= 0x0F,

	/// <summary>friends list</summary>
	FRL	= 0x10,

	/// <summary>ignore list</summary>
	IGN	= 0x11,

	/// <summary>chat ops list</summary>
	ADL	= 0x12,

	/// <summary>character list and online status</summary>
	LIS	= 0x13,

	/// <summary>a user connected</summary>
	NLN	= 0x14,

	/// <summary>a user disconnected</summary>
	FLN	= 0x15,

	/// <summary>list of channel ops</summary>
	COL	= 0x16,

	/// <summary>initial channel data</summary>
	ICH	= 0x17,

	/// <summary>channel's description has updated</summary>
	CDS	= 0x18,

	/// <summary>basic error response</summary>
	ERR	= 0x19,

	/// <summary>invite user to channel</summary>
	CIU	= 0x1A,

	/// <summary>kicks a user</summary>
	CKU	= 0x1B,

	/// <summary>bans a user</summary>
	CBU	= 0x1C,

	/// <summary>promotes to channel op</summary>
	COA	= 0x1D,

	/// <summary>demotes from channel op to user</summary>
	COR	= 0x1E,

	/// <summary>time a user out</summary>
	CTU	= 0x1F,

	/// <summary>unbans a user</summary>
	CUB	= 0x20,
}
