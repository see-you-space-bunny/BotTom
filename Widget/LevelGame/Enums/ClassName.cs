using System.ComponentModel;
using RoleplayingGame.Attributes;

namespace RoleplayingGame.Enums;

public enum ClassName
{
	[Description]
	Nobody		= 0x00,
	
	[Description("")]
	Adventurer	= 0x01,
	
	[Description("")]
	Merchant	= 0x02,
}