namespace FChatApi.Enums;

/// <summary>user kink preference groups</summary>
public enum KinkPreference
{
	/// <summary>empty/invalid, failthrough default</summary>
	Invalid	= 0x00,

	/// <summary>no kink/s</summary>
	No		= 0x01,

	/// <summary>no preference</summary>
	None	= 0x02,

	/// <summary>maybe kinks/s</summary>
	Maybe	= 0x03,

	/// <summary>yes kink/s</summary>
	Yes		= 0x04,

	/// <summary>favorite kink/s</summary>
	Favorite= 0x05,
}