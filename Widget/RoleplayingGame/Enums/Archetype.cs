using System.ComponentModel;
using RoleplayingGame.Attributes;

namespace RoleplayingGame.Enums;

public enum Archetype
{
	[Description("none. This text should never be displayed.")]
	None		= 0x00,

	[Description(".")]
	Bystander	= 0x10,

	[Description(".")]
	Trickster	= 0x20,

	[Description(".")]
	Hero		= 0x30,

	[Description(".")]
	Villain		= 0x40,

}