namespace FChatApi.Enums;

/// <summary>user's publicly visible status</summary>
public enum ChatStatus
{
	/// <summary>empty/invalid, failthrough default</summary>
	Invalid		= 0x00,

	/// <summary>any status</summary>
	Any			= 0x01,

	/// <summary>offline</summary>
	Offline		= 0x05,

	/// <summary>connected (but not online yet?) ?SPECULATION</summary>
	Connected	= 0x06,

	/// <summary>all 'online' user status</summary>
	AnyOnline	= 0x0A,

	/// <summary>idle by timer</summary>
	Idle		= 0x13,

	/// <summary>away</summary>
	Away		= 0x14,

	/// <summary>do not disturb</summary>
	DND			= 0x15,

	/// <summary>busy</summary>
	Busy		= 0x16,

	/// <summary>online</summary>
	Online		= 0x17,

	/// <summary>looking for roleplay</summary>
	Looking		= 0x18,

	/// <summary>unknown</summary>
	Crown		= 0x19,
}