namespace FChatApi.Enums;

/// <summary>user's publicly visible status</summary>
public enum ChatStatus
{
	/// <summary>empty/invalid, failthrough default</summary>
	Invalid		= 0x00,

	/// <summary>offline</summary>
	Offline		= 0x01,

	/// <summary>connected (but not online yet?) ?SPECULATION</summary>
	Connected	= 0x02,

	/// <summary>idle by timer</summary>
	Idle		= 0x03,

	/// <summary>away</summary>
	Away		= 0x04,

	/// <summary>do not disturb</summary>
	DND			= 0x05,

	/// <summary>busy</summary>
	Busy		= 0x06,

	/// <summary>online</summary>
	Online		= 0x07,

	/// <summary>looking for roleplay</summary>
	Looking		= 0x08,

	/// <summary>unknown</summary>
	Crown		= 0x09,
}