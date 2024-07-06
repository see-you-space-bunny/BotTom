using System.ComponentModel;

namespace Engine.ModuleHost.Enums;

public enum Privilege
{
	[Description("")]
	None = 0x00,
	
	[Description("")]
	BannedUser = 0x10,
	
	[Description("")]
	TimedOutUser = 0x20,
	
	[Description("")]
	UnregisteredUser = 0x80,
	
	[Description("")]
	RegisteredUser = 0xA0,
	
	[Description("")]
	TrustedUser = 0xC0,
	
	[Description("")]
	CeremonyOperator = 0xD0,
	
	[Description("")]
	ChannelOperator = 0xE0,
	
	[Description("")]
	GlobalOperator = 0xF0,
	
	[Description("")]
	OwnerOperator = 0xFF,
}