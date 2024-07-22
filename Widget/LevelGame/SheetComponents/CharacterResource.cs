using System.Reflection.Metadata.Ecma335;
using LevelGame.Attributes;
using LevelGame.Enums;

namespace LevelGame.SheetComponents;

public class CharacterResource(int baseValue = 0,int hardLimit = -1,int softLimit = -1,bool moreIsBetter = true)
{
	internal CharacterResource(ResourceDefaultValuesAttribute defaults) :
		this(defaults.BaseValue,defaults.HardLimit,defaults.SoftLimit,defaults.MoreIsBetter)
	{ }

	private int _baseValue = baseValue;

	public int Current {
		get => IsSoftLimited && _baseValue > SoftLimit? SoftLimit : _baseValue;
		set => _baseValue = IsHardLimited && value > HardLimit? HardLimit : value;
	}

	public List<StatusEffect> Modifiers { get; } = [];

	public int CurrentNoSoftLimit { get=>_baseValue; }

	public int SoftLimit { get; set; } = softLimit;
	public int HardLimit { get; set; } = hardLimit;
	
	public bool IsSoftLimited { get; set; } = softLimit >= 0;
	public bool IsHardLimited { get; set; } = hardLimit >= 0;
	
	public bool IsAtSoftLimit => IsSoftLimited && _baseValue >= SoftLimit;
	public bool IsAtHardLimit => IsHardLimited && _baseValue == HardLimit;
	
	public bool IsOverSoftLimit => _baseValue > SoftLimit;

	public bool MoreIsBetter { get; set; } = moreIsBetter;
}