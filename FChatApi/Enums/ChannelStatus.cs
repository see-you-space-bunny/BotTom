namespace FChatApi.Enums;

/// <summary>a user's relationship with a channel</summary>
public enum UserRelationshipWithChannel
{
	/// <summary>empty/invalid, failthrough default</summary>
	Invalid		= 0x00,

	/// <summary>all relationships</summary>
	All			= 0x01,

	/// <summary>unknown relationship to this channel</summary>
	Unknown		= 0x02,

	/// <summary>user is/was banned from this channel</summary>
	Banned		= 0x03,

	/// <summary>the user is pending joining this channel ?SPECULATION</summary>
	Pending		= 0x04,

	/// <summary>the user is creating this channel ?SPECULATION</summary>
	Creating	= 0x05,

	/// <summary>all 'valid' relationsips</summary>
	AllValid	= 0x06,

	/// <summary>user is/was kicked from this channel</summary>
	Kicked		= 0x07,

	/// <summary>user has left this channel</summary>
	Left		= 0x08,

	/// <summary>the user is the owner/creator of this channel ? the channel has just been created ?SPECULATION</summary>
	Created		= 0x09,

	/// <summary>channel is available to be joined ?SPECULATION</summary>
	Available	= 0x1A,

	/// <summary>user has been invited to this channel</summary>
	Invited		= 0x10,

	/// <summary>the user has joined this channel ?SPECULATION</summary>
	Joined		= 0x11,
}
