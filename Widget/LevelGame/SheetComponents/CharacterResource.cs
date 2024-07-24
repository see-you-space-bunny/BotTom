using System.Reflection.Metadata.Ecma335;
using LevelGame.Attributes;
using LevelGame.Enums;

namespace LevelGame.SheetComponents;

public class CharacterResource(int baseValue = 0,int hardLimit = -1,int softLimit = -1,bool moreIsBetter = true)
{
	internal CharacterResource(ResourceDefaultValuesAttribute defaults) :
		this(defaults.BaseValue,defaults.HardLimit,defaults.SoftLimit,defaults.MoreIsBetter)
	{ }

	private float _baseValue = baseValue;
	private int _softLimit = softLimit;
	private int _hardLimit = hardLimit;

	public float BaseValue {
		get => _baseValue;
		set => _baseValue = IsHardLimited && value > HardLimit ?
			HardLimit : value;
	}

	public int Current {
		get => IsSoftLimited && CurrentNoSoftLimit > SoftLimit ?
			SoftLimit : CurrentNoSoftLimit;
	}

	public float SumOfModifiers { get; set; }
	public float CombinedMultipliers { get; set; } = 1.0f;

	public float SumOfLimitModifiers { get; set; }
	public float CombinedLimitMultipliers { get; set; } = 1.0f;

	public int CurrentNoSoftLimit { get=>(int)((_baseValue + SumOfModifiers)*CombinedMultipliers); }

	public int SoftLimit { get=>(int)((_softLimit + SumOfLimitModifiers)*CombinedLimitMultipliers); set=> _softLimit=value; }
	public int HardLimit { get=>(int)((_hardLimit + SumOfLimitModifiers)*CombinedLimitMultipliers); set=> _hardLimit=value; }
	
	public bool IsSoftLimited { get; set; } = softLimit >= 0;
	public bool IsHardLimited { get; set; } = hardLimit >= 0;
	
	public bool IsAtSoftLimit => IsSoftLimited && _baseValue >= SoftLimit;
	public bool IsAtHardLimit => IsHardLimited && _baseValue == HardLimit;
	
	public bool IsOverSoftLimit => _baseValue > SoftLimit;

	public bool MoreIsBetter { get; set; } = moreIsBetter;
}