namespace FChatApi.Enums;

/// <summary>the bot's relationship with this user</summary>
public enum UserStatus
{
	/// <summary>empty/invalid, failthrough default</summary>
	Invalid	= 0x00,
	
	/// <summary>user has been globally blocked ?SPECULATION</summary>
	Blocked	= 0x01,
	
	/// <summary>user has been ignored in chat ?SPECULATION</summary>
	Ignored	= 0x02,

	/// <summary>no defined relationship to user</summary>
	None	= 0x03,
	
	/// <summary>bookmarked? as in "see when Online" ?SPECULATION</summary>
	Online	= 0x04,
	
	/// <summary>user is a friend (of this account, but not this character?) ?SPECULATION</summary>
	Friended= 0x05,
	
	/// <summary>user is a friend (of this character?) ?SPECULATION</summary>
	Married	= 0x06,
}