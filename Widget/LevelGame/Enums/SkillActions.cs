using Widget.LevelGame.Attributes;

namespace Widget.LevelGame.Enums;

public enum SkillActions
{
	None        = 0x00,

	#region Combat Actions
	Ambush      = 0x01,
	Brawl       = 0x02,
	Duel        = 0x03,
	GunFight    = 0x04,
	Outmaneuver = 0x05,
	#endregion
	

	#region Exploration Actions
	#endregion
	Sneak       = 0x06,
	Steal       = 0x07,
	Trick       = 0x08,
	Convince    = 0x09,
	Taunt       = 0x0A,
	Threaten    = 0x0B,
}