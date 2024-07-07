using FChatApi.Attributes;

namespace FChatApi.Enums;

/// <summary>the type of message being sent/recieved</summary>
public enum FChatMessageType
{
	/// <summary>empty/invalid, failthrough default</summary>
	[MaximumLength(0)]
	Invalid			= 0x00,

	/// <summary>status message</summary>
	[MaximumLength(255)]
	Status			= 0x01,

	/// <summary>direct message</summary>
	[MaximumLength(50000)]
	Whisper			= 0x02,

	/// <summary>basic channel message</summary>
	[MaximumLength(4096)]
	Basic			= 0x03,

	/// <summary>roleplay advert message</summary>
	[MaximumLength(4096)]
	Advertisement	= 0x04,

	/// <summary>channel alert message</summary>
	[MaximumLength(4096)]
	Yell			= 0x05,
}