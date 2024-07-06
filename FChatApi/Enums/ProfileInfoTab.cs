namespace FChatApi.Enums;

/// <summary>profile info field</summary>
public enum ProfileInfoTab
{
	/// <summary>empty/invalid, failthrough default</summary>
	Invalid				= 0x00,

	/// <summary>user's contact details</summary>
	ContactDetails		= 0x01,

	/// <summary>user's sexual details</summary>
	SexualDetails		= 0x02,

	/// <summary>user's general details</summary>
	GeneralDetails		= 0x03,

	/// <summary>user's roleplaying preferences</summary>
	RoleplayPreferences	= 0x04,
}