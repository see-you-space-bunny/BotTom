using System.ComponentModel;

using LevelGame.Attributes;

namespace LevelGame.Enums;

// Abilities always in order: Offense, Defense, Flexile
public enum DerivedAbility
{
	[Description("")]
	[ShortForm("NON")]
	[AbilityInfo(AbilityGroup.None,AbilityType.None)]
	None		= 0x00,

	[Description("")]
	[ShortForm("FIN")]
	[DerivedAbilityInfo([Ability.Reflex,Ability.Wit],AbilityGroup.None,AbilityType.None)]
	Finesse		= 0x01,
}
