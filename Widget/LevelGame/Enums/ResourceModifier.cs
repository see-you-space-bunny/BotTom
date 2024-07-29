using RoleplayingGame.Attributes;

namespace RoleplayingGame.Enums;
public enum ResourceModifier
{
	[DefaultModifier(0.0f)]
	None			= 0x00,
	
	[DefaultModifier(100.0f)]
	BaseValue		= 0x10,
	
	[DefaultModifier(0.0f)]
	BaseGrowth		= 0x11,
	
	[DefaultModifier(0.0f)]
	MinimumValue	= 0x20,
	
	[DefaultModifier(1.0f)]
	SoftLimit		= 0x30,
	
	[DefaultModifier(1.0f)]
	HardLimit		= 0x31,
	
	[DefaultModifier(1.0f)]
	Recovery		= 0x40,
	
	[DefaultModifier(1.0f)]
	Growth			= 0x41,
	
	[DefaultModifier(1.0f)]
	Loss			= 0x42,
}