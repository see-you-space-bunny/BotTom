using System.ComponentModel;

namespace Engine.ModuleHost.Enums;

public enum BotModule
{
	[Description("")]
	None	= 0x00,
	
	[Description("")]
	System	= 0x01,

	[Description("")]
	Clock,
	
	[Description("")]
	XCG,
}