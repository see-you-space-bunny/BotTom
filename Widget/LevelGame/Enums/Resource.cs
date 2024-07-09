namespace LevelGame.Enums;

public enum Resource
{
	HealthPoints,
	Gold,
	Experience,
	Energy,
	Time,
}
public enum ResourceModifier
{
	BaseValue   = 0x00,
	SoftLimit   = 0x01,
	HardLimit   = 0x02,
	Recovery    = 0x03,
	Growth      = 0x04,
	Loss        = 0x05,
}