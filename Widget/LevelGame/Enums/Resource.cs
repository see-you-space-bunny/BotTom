using LevelGame.Attributes;

namespace LevelGame.Enums;

public enum Resource
{
	[XmlKey("")]
	[GameFlags]
	None				= 0x00,

	[XmlKey("descendant::Resources/Health")]
	[GameFlags(scalesOnLimitChange:true,refilledByFullRecovery:true)]
	Health				= 0x01,

	[XmlKey("descendant::Resources/Protection")]
	[GameFlags(scalesOnLimitChange:true,refilledByFullRecovery:true)]
	Protection			= 0x02,

	[XmlKey("descendant::Resources/Evasion")]
	[GameFlags(scalesOnLimitChange:true,refilledByFullRecovery:true)]
	Evasion				= 0x03,

	[XmlKey("descendant::Resources/Gold")]
	[GameFlags]
	Gold				= 0x04,

	[XmlKey("descendant::Resources/Experience")]
	[GameFlags]
	Experience			= 0x05,

	[XmlKey("descendant::Resources/Energy")]
	[GameFlags]
	Energy				= 0x06,

	[XmlKey("descendant::Resources/Time")]
	[GameFlags]
	Time				= 0x07,
}
public enum ResourceModifier
{
	[DefaultModifier(0.0f)]
	None			= 0x00,
	
	[DefaultModifier(100.0f)]
	BaseValue		= 0x01,
	
	[DefaultModifier(0.0f)]
	MinimumValue	= 0x02,
	
	[DefaultModifier(1.0f)]
	SoftLimit		= 0x03,
	
	[DefaultModifier(1.0f)]
	HardLimit		= 0x04,
	
	[DefaultModifier(1.0f)]
	Recovery		= 0x05,
	
	[DefaultModifier(1.0f)]
	Growth			= 0x06,
	
	[DefaultModifier(1.0f)]
	Loss			= 0x07,
}