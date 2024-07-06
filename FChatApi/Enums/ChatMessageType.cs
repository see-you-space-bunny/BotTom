namespace FChatApi.Enums;

/// <summary>the type of message being sent/recieved</summary>
public enum ChatMessageType
{
	/// <summary>empty/invalid, failthrough default</summary>
	Invalid			= 0x00,

	/// <summary>roleplay advert message</summary>
	Advertisement	= 0x01,

	/// <summary>basic channel message</summary>
	Basic			= 0x02,

	/// <summary>channel alert message</summary>
	Yell			= 0x03,

	/// <summary>direct message</summary>
	Whisper			= 0x04,
}