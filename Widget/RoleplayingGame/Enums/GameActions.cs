using RoleplayingGame.Attributes;

namespace RoleplayingGame.Enums;

public enum GameAction
{
	None        = 0x00,

	#region Combat Actions
	[ActionProperties]
	[ActionDefaultValues([Ability.Reflex,Ability.Luck],0,5,3)]
	Ambush      = 0x01,

	[ActionProperties]
	[ActionDefaultValues([Ability.Power,Ability.Body],0,5,3)]
	Brawl       = 0x02,

	[ActionProperties]
	[ActionDefaultValues([Ability.Reflex,Ability.Focus],0,5,3)]
	Duel        = 0x03,

	[ActionProperties]
	[ActionDefaultValues([Ability.Reflex,Ability.Will],0,5,3)]
	GunFight    = 0x04,

	[ActionProperties]
	[ActionDefaultValues([Ability.Reflex,Ability.Wit],0,5,3)]
	Outmaneuver = 0x05,
	#endregion
	

	#region Exploration Actions
	[ActionProperties]
	[ActionDefaultValues([Ability.Reflex,Ability.Wit],0,5,3)]
	Sneak       = 0x06,

	[ActionProperties]
	[ActionDefaultValues([Ability.Reflex,Ability.Luck],0,5,3)]
	Steal       = 0x07,

	[ActionProperties]
	[ActionDefaultValues([Ability.Charm,Ability.Wit],0,5,3)]
	Trick       = 0x08,

	[ActionProperties]
	[ActionDefaultValues([Ability.Charm,Ability.Integrity],0,5,3)]
	Convince    = 0x09,

	[ActionProperties]
	[ActionDefaultValues([Ability.Presence,Ability.Charm],0,5,3)]
	Taunt       = 0x0A,

	[ActionProperties]
	[ActionDefaultValues([Ability.Presence,Ability.Power],0,5,3)]
	Threaten    = 0x0B,
	#endregion

	#region Special/Meta Actions
	[ActionProperties(cooldown:1200.0f)]
	[ActionDefaultValues([Ability.None])]
	BringLow	= 0xFC,

	[ActionProperties(cooldown:1200.0f)]
	[ActionDefaultValues([Ability.None])]
	LimitBreak	= 0xFD,

	[ActionProperties(nameTagged:true,sharesCooldownWith:BadEnd)]
	[ActionDefaultValues([Ability.None])]
	Devour		= 0xFE,
	
	[ActionProperties(cooldown:1200.0f)]
	[ActionDefaultValues([Ability.None])]
	BadEnd		= 0xFF,
	#endregion
}