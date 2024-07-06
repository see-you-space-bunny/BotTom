namespace FChatApi.Enums;

/// <summary>the type of this channel</summary>
public enum ChannelType
{
	/// <summary>empty/invalid, failthrough default</summary>
	Invalid		= 0x00,

	/// <summary>any kind of channel</summary>
	All			= 0x01,

	/// <summary>public 'official' channel</summary>
	Public		= 0x02,

	/// <summary>user-created channel</summary>
	Private		= 0x03,

	/// <summary>user-created channel that is not visible to others</summary>
	Hidden		= 0x04,
}