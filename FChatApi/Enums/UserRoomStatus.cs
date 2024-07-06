namespace FChatApi.Enums;

/// <summary>moderation actions ?SPECULATION</summary>
public enum UserRoomStatus
{
	/// <summary>empty/invalid, failthrough default</summary>
	Invalid		= 0x00,

	/// <summary>bans user from the channel</summary>
	Banned		= 0x01,

	/// <summary>times out user from the channel</summary>
	Timeout		= 0x02,

	/// <summary>kicks user from the channel</summary>
	Kicked		= 0x03,

	/// <summary>no action</summary>
	None		= 0x04,

	/// <summary>demote moderator to user ?SPECULATION</summary>
	User		= 0x05,

	/// <summary>promote user to operator</summary>
	Moderator	= 0x06,

	/// <summary>transfer channel ownership ?SPECULATION (unused)</summary>
	Owner		= 0x07,
}