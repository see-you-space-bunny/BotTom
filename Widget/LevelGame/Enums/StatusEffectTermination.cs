using System.ComponentModel;

namespace LevelGame.Enums;

public enum StatusEffectTermination
{
	[Description]
    None			= 0x00,

	[Description("Will cure itself automatically after a certain duration.")]
    Timed			= 0x11,

	[Description("Will cure itself automatically after a certain number of interactions.")]
    Interact		= 0x22,

	[Description("Has a random chance to cure itself each interaction.")]
    CureProc		= 0x33,

	[Description("Will cure itself automatically once {0} is {1} than {2}.")]
    StatRequirement	= 0x44,

	[Description("Will be cured when resting.")]
    Rest			= 0x55,

	[Description("Will be cured with a full recovery.")]
    FullRecovery	= 0x66,

	[Description("Lasts until cured by an effect of tier {0} or higher. ({1})")]
    CureEffect		= 0x77,

	[Description("Can only be cured by performing a full character reset.")]
    CharacterReset	= 0xAA,

	[Description("Can never be cured or removed.")]
    Never			= 0xFF,
}