using LevelGame.Attributes;

namespace LevelGame.Enums;

public enum Resource
{
	[GameFlags]
	[ResourceDefaultValues]
	None			= 0x00,

	[GameFlags(scalesOnLimitChange:true,refilledByFullRecovery:true)]
	[ResourceDefaultValues(hardLimit:1,softLimit:1)]
	Health			= 0x01,

	[GameFlags(scalesOnLimitChange:true,refilledByFullRecovery:true)]
	[ResourceDefaultValues(hardLimit:1,softLimit:1)]
	Protection		= 0x02,

	[GameFlags(scalesOnLimitChange:true,refilledByFullRecovery:true)]
	[ResourceDefaultValues(hardLimit:1,softLimit:1)]
	Evasion			= 0x03,

	[GameFlags]
	[ResourceDefaultValues]
	Gold			= 0x04,

	[GameFlags]
	[ResourceDefaultValues]
	Experience		= 0x05,

	[GameFlags]
	[ResourceDefaultValues(hardLimit:1,softLimit:1)]
	Energy			= 0x06,

	[GameFlags]
	[ResourceDefaultValues]
	Time			= 0x07,
}