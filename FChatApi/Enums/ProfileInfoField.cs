using FChatApi.Attributes;

namespace FChatApi.Enums;

/// <summary>profile info field</summary>
public enum ProfileInfoField
{
	/// <summary>empty/invalid, failthrough default</summary>
	[InfoTab(ProfileInfoTab.Invalid)]
	Invalid			= 0x00,

	[InfoTab(ProfileInfoTab.ContactDetails)]
	DeviantArt		= 0x01,

	[InfoTab(ProfileInfoTab.ContactDetails)]
	FFXIV			= 0x02,

	[InfoTab(ProfileInfoTab.ContactDetails)]
	Website			= 0x03,

	[InfoTab(ProfileInfoTab.ContactDetails)]
	Skype			= 0x04,

	[InfoTab(ProfileInfoTab.ContactDetails)]
	Tapestries		= 0x05,

	[InfoTab(ProfileInfoTab.ContactDetails)]
	Furcadia		= 0x06,

	[InfoTab(ProfileInfoTab.ContactDetails)]
	Email			= 0x07,

	[InfoTab(ProfileInfoTab.ContactDetails)]
	SecondLife		= 0x08,

	[InfoTab(ProfileInfoTab.ContactDetails)]
	Steam			= 0x09,

	[InfoTab(ProfileInfoTab.ContactDetails)]
	Twitter			= 0x0A,

	[InfoTab(ProfileInfoTab.ContactDetails)]
	Shangrila		= 0x1B,

	[InfoTab(ProfileInfoTab.ContactDetails)]
	Furaffinity		= 0x0C,

	[InfoTab(ProfileInfoTab.ContactDetails)]
	Inkbunny		= 0x0D,

	[InfoTab(ProfileInfoTab.ContactDetails)]
	AIM				= 0x0E,

	[InfoTab(ProfileInfoTab.ContactDetails)]
	Trillian		= 0x0F,

	[InfoTab(ProfileInfoTab.ContactDetails)]
	HouseEros		= 0x10,

	[InfoTab(ProfileInfoTab.ContactDetails)]
	YIM				= 0x11,

	[InfoTab(ProfileInfoTab.ContactDetails)]
	XboxLive		= 0x12,

	[InfoTab(ProfileInfoTab.ContactDetails)]
	Discord			= 0x13,

	[InfoTab(ProfileInfoTab.ContactDetails)]
	PSN				= 0x14,

	[InfoTab(ProfileInfoTab.ContactDetails)]
	WoW				= 0x15,

	[InfoTab(ProfileInfoTab.ContactDetails)]
	ICQ				= 0x16,

	[InfoTab(ProfileInfoTab.ContactDetails)]
	IRC				= 0x17,

	[InfoTab(ProfileInfoTab.ContactDetails)]
	TelegramUse		= 0x18,

	[InfoTab(ProfileInfoTab.ContactDetails)]
	IMVU			= 0x19,

	[InfoTab(ProfileInfoTab.ContactDetails)]
	XMPP_Jabbe		= 0x1A,


	[InfoTab(ProfileInfoTab.SexualDetails)]
	NippleColor				= 0x50,

	[InfoTab(ProfileInfoTab.SexualDetails)]
	VulvaType				= 0x51,

	[InfoTab(ProfileInfoTab.SexualDetails)]
	KnotDiameterInInches	= 0x52,

	[InfoTab(ProfileInfoTab.SexualDetails)]
	Uncut					= 0x53,

	[InfoTab(ProfileInfoTab.SexualDetails)]
	DomSubRole				= 0x54,

	[InfoTab(ProfileInfoTab.SexualDetails)]
	CockLengthInInches		= 0x55,

	[InfoTab(ProfileInfoTab.SexualDetails)]
	PubicHair				= 0x56,

	[InfoTab(ProfileInfoTab.SexualDetails)]
	CumshotSize				= 0x57,

	[InfoTab(ProfileInfoTab.SexualDetails)]
	CockColor				= 0x58,

	[InfoTab(ProfileInfoTab.SexualDetails)]
	Flesh					= 0x59,

	[InfoTab(ProfileInfoTab.SexualDetails)]
	Sheath					= 0x5A,

	[InfoTab(ProfileInfoTab.SexualDetails)]
	Barbed					= 0x5B,

	[InfoTab(ProfileInfoTab.SexualDetails)]
	BreastSize				= 0x5C,

	[InfoTab(ProfileInfoTab.SexualDetails)]
	BallSize				= 0x5D,

	[InfoTab(ProfileInfoTab.SexualDetails)]
	Knotted					= 0x5E,

	[InfoTab(ProfileInfoTab.SexualDetails)]
	CockDiameterInches		= 0x5F,

	[InfoTab(ProfileInfoTab.SexualDetails)]
	CockShape				= 0x61,

	[InfoTab(ProfileInfoTab.SexualDetails)]
	Position				= 0x62,

	[InfoTab(ProfileInfoTab.SexualDetails)]
	Measurements			= 0x63,


	[InfoTab(ProfileInfoTab.GeneralDetails)]
	FurScaleSkinColor	= 0x90,

	[InfoTab(ProfileInfoTab.GeneralDetails)]
	Orientation			= 0x91,

	[InfoTab(ProfileInfoTab.GeneralDetails)]
	Species				= 0x92,

	[InfoTab(ProfileInfoTab.GeneralDetails)]
	HeightLength		= 0x93,

	[InfoTab(ProfileInfoTab.GeneralDetails)]
	Build				= 0x94,

	[InfoTab(ProfileInfoTab.GeneralDetails)]
	Occupation			= 0x95,

	[InfoTab(ProfileInfoTab.GeneralDetails)]
	EyeColor			= 0x96,

	[InfoTab(ProfileInfoTab.GeneralDetails)]
	Hair				= 0x97,

	[InfoTab(ProfileInfoTab.GeneralDetails)]
	Weight				= 0x98,

	[InfoTab(ProfileInfoTab.GeneralDetails)]
	Location			= 0x99,

	[InfoTab(ProfileInfoTab.GeneralDetails)]
	BodyType			= 0x9A,

	[InfoTab(ProfileInfoTab.GeneralDetails)]
	Personality			= 0x9B,

	[InfoTab(ProfileInfoTab.GeneralDetails)]
	PartnerMateLover	= 0x9C,

	[InfoTab(ProfileInfoTab.GeneralDetails)]
	PetsSlaves			= 0x9D,

	[InfoTab(ProfileInfoTab.GeneralDetails)]
	MasterMistressOwner	= 0x9E,

	[InfoTab(ProfileInfoTab.GeneralDetails)]
	Relationship		= 0x9F,

	[InfoTab(ProfileInfoTab.GeneralDetails)]
	Gender				= 0xA0,

	[InfoTab(ProfileInfoTab.GeneralDetails)]
	BodyModifications	= 0xA1,

	[InfoTab(ProfileInfoTab.GeneralDetails)]
	Age					= 0xA2,

	[InfoTab(ProfileInfoTab.GeneralDetails)]
	ApparentAge			= 0xA3,


	[InfoTab(ProfileInfoTab.RoleplayPreferences)]
	GrammarCompetence			= 0xD0,

	[InfoTab(ProfileInfoTab.RoleplayPreferences)]
	DesiredPostLength			= 0xD1,

	[InfoTab(ProfileInfoTab.RoleplayPreferences)]
	DesiredRPMethod				= 0xD2,

	[InfoTab(ProfileInfoTab.RoleplayPreferences)]
	DesiredRPLength				= 0xD3,

	[InfoTab(ProfileInfoTab.RoleplayPreferences)]
	PostPerspective				= 0xD4,

	[InfoTab(ProfileInfoTab.RoleplayPreferences)]
	FurryPreference				= 0xD5,

	[InfoTab(ProfileInfoTab.RoleplayPreferences)]
	GrammarCompetenceRequired	= 0xD6,

	[InfoTab(ProfileInfoTab.RoleplayPreferences)]
	LanguagePreference			= 0xD7,

	[InfoTab(ProfileInfoTab.RoleplayPreferences)]
	CurrentlyÖookingFor			= 0xD8,
}