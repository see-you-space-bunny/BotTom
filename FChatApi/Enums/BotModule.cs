using System.ComponentModel;

namespace FChatApi.Enums;

public enum BotModule
{
	[Description("")]
	None	= 0x00,
	
	[Description("")]
	System	= 0x01,

	[Description("")]
	Clock	= 0x02,
	
	[Description("")]
	XCG		= 0x03,
}