using System.Globalization;
using System.Reflection.Metadata.Ecma335;
using RoleplayingGame.Attributes;
using RoleplayingGame.Enums;
using RoleplayingGame.Objects;

namespace RoleplayingGame.SheetComponents;

public class AbilityScore : ResourceBase
{
#region Constants
	private const float StatusEffectMultiplierPositive	= 0.85f;
	private const float StatusEffectMultiplierNegative	= 1.35f;
	private const float ModifiersMultiplierPositive		= 0.85f;
	private const float ModifiersMultiplierNegative		= 1.35f;
#endregion


	private readonly Ability _ability;
	public Ability Key => _ability;


#region Recordkeeping
	private readonly List<(DateTime When,ulong Id,int Value,int Actual)> _lifetimeModifiersPvP = [];
	private readonly List<(DateTime When,EnvironmentSource Id,int Value,int Actual)> _lifetimeModifiersPvE = [];
	private readonly List<(DateTime When,ulong Id,int Value,int Actual)> _currentModifiersPvP = [];
	private readonly List<(DateTime When,EnvironmentSource Id,int Value,int Actual)> _currentModifiersPvE = [];

	public void Modify(DateTime when,ulong source,int value)
	{
		int actual = value;
		if (HasHardLimit)
			actual = (value + CurrentTotal > 0 ?
				Math.Min(value + CurrentTotal, ModifierHardLimitHigh) :
					Math.Max(value + CurrentTotal, ModifierHardLimitLow)) - CurrentTotal;
		_currentModifiersPvP.Add((when,source,value,actual));
	}
	public void Modify(DateTime when,EnvironmentSource source,int value)
	{
		int actual = value;
		if (HasHardLimit)
			actual = (value + CurrentTotal > 0 ?
				Math.Min(value + CurrentTotal, ModifierHardLimitHigh) :
					Math.Max(value + CurrentTotal, ModifierHardLimitLow)) - CurrentTotal;
		_currentModifiersPvE.Add((when,source,value,actual));
	}

	public int LifetimeGain	=> _lifetimeModifiersPvP.Sum(li=>li.Actual > 0 ? li.Actual : 0) + _lifetimeModifiersPvE.Sum(li=>li.Actual > 0 ? li.Actual : 0);
	public int LifetimeLoss	=> _lifetimeModifiersPvP.Sum(li=>li.Actual < 0 ? li.Actual : 0) + _lifetimeModifiersPvE.Sum(li=>li.Actual < 0 ? li.Actual : 0);
	public int LifetimeTotal=> _lifetimeModifiersPvP.Sum(li=>li.Actual) + _lifetimeModifiersPvE.Sum(li=>li.Actual);

	public int CurrentGain	=> _currentModifiersPvP.Sum(li=>li.Actual > 0 ? li.Actual : 0) + _currentModifiersPvE.Sum(li=>li.Actual > 0 ? li.Actual : 0);
	public int CurrentLoss	=> _currentModifiersPvP.Sum(li=>li.Actual < 0 ? li.Actual : 0) + _currentModifiersPvE.Sum(li=>li.Actual < 0 ? li.Actual : 0);
	public int CurrentTotal	=> _currentModifiersPvP.Sum(li=>li.Actual) + _currentModifiersPvE.Sum(li=>li.Actual);
#endregion


#region Partial Values
	public float ClassValue => _parent.ClassLevels.Values.Sum(cl=>cl.Class.AbilityGrowth[_ability] * cl.CurrentLevel);

	public float Modifiers => CurrentTotal * (CurrentTotal > 0 ? ModifiersMultiplierPositive : ModifiersMultiplierNegative);

	public float StatusAdjustment {
		get {
			float statusAdjustment = _parent.SumStatusAdjustmentByAbility(_ability);
			return statusAdjustment * (statusAdjustment > 0 ? StatusEffectMultiplierPositive : StatusEffectMultiplierNegative);
		}
	}
#endregion


#region Values
	private int ActualValue => Math.Min(SoftLimit,ActualValueNoSoftLimit);
	private int ActualValueNoSoftLimit => HasHardLimit ? Math.Min(HardLimit,ActualValueRaw) : ActualValueRaw;
	private int ActualValueRaw => (int)(ClassValue+Modifiers+StatusAdjustment);
	public int GetActualValue(bool softLimited = true) => softLimited && HasSoftLimit ? ActualValue : ActualValueNoSoftLimit;

	private int DisplayValue => Math.Min(SoftLimit,DisplayValueNoSoftLimit);
	private int DisplayValueNoSoftLimit => HasHardLimit ? Math.Min(HardLimit,DisplayValueRaw) : ActualValueRaw;
	private int DisplayValueRaw => (int)(ClassValue+CurrentTotal+_parent.SumStatusAdjustmentByAbility(_ability));
	public int GetDisplayValue(bool softLimited = true) => softLimited && HasSoftLimit ? DisplayValue : DisplayValueNoSoftLimit;
#endregion


#region Limits
	private int ModifierSoftLimit		=> (CurrentTotal > 0 ? ModifierSoftLimitHigh : -ModifierSoftLimitLow);
	private int ModifierSoftLimitHigh	=> (int)(SoftLimit - ClassValue);
	private int ModifierSoftLimitLow	=> (int)-(SoftLimit + ClassValue);
	private int ModifierHardLimit		=> (CurrentTotal > 0 ? ModifierHardLimitHigh : -ModifierHardLimitLow);
	private int ModifierHardLimitHigh	=> (int)(HardLimit - ClassValue);
	private int ModifierHardLimitLow	=> (int)-(HardLimit + ClassValue);
	
	public override bool IsAtSoftLimit => HasSoftLimit && DisplayValue >= SoftLimit;
	public override bool IsAtHardLimit => HasHardLimit && DisplayValue == HardLimit;
	
	public override bool IsOverSoftLimit => DisplayValue > SoftLimit;
#endregion


#region Serialization
	private static (DateTime When,ulong Id,int Value,int Actual) DeserializePvPModifier(BinaryReader reader)
		=> (
			new DateTime(
				year:	reader.ReadInt32(),
				month:	reader.ReadUInt16(),
				day:	reader.ReadUInt16(),
				hour:	reader.ReadUInt16(),
				minute:	reader.ReadUInt16(),
				second:	0
			),
			reader.ReadUInt64(),
			reader.ReadInt32(),
			reader.ReadInt32()
		);
	private static (DateTime When,EnvironmentSource Id,int Value,int Actual) DeserializePvEModifier(BinaryReader reader)
		=> (
			new DateTime(
				year:	reader.ReadInt32(),
				month:	reader.ReadUInt16(),
				day:	reader.ReadUInt16(),
				hour:	reader.ReadUInt16(),
				minute:	reader.ReadUInt16(),
				second:	0
			),
			(EnvironmentSource)	reader.ReadUInt16(),
			reader.ReadInt32(),
			reader.ReadInt32()
		);

	public static AbilityScore Deserialize(BinaryReader reader,Actor parent)
	{
		AbilityScore result = new (parent,(Ability)reader.ReadUInt16());

		for (uint i=0;i<reader.ReadUInt32();i++)
		{
			result._currentModifiersPvE.Add(DeserializePvEModifier(reader));
		}
		for (uint i=0;i<reader.ReadUInt32();i++)
		{
			result._currentModifiersPvP.Add(DeserializePvPModifier(reader));
		}

		for (uint i=0;i<reader.ReadUInt32();i++)
		{
			result._lifetimeModifiersPvE.Add(DeserializePvEModifier(reader));
		}
		for (uint i=0;i<reader.ReadUInt32();i++)
		{
			result._lifetimeModifiersPvP.Add(DeserializePvPModifier(reader));
		}

		return result;
	}

	private static void SerializeModifier(BinaryWriter writer,(DateTime When,ulong Id,int Value,int Actual) modifier)
	{
		writer.Write((int)		modifier.When.Year);
		writer.Write((ushort)	modifier.When.Month);
		writer.Write((ushort)	modifier.When.Day);
		writer.Write((ushort)	modifier.When.Hour);
		writer.Write((ushort)	modifier.When.Minute);
		writer.Write((ulong)	modifier.Id);
		writer.Write((int)		modifier.Value);
		writer.Write((int)		modifier.Actual);
	}
	private static void SerializeModifier(BinaryWriter writer,(DateTime When,EnvironmentSource Id,int Value,int Actual) modifier)
	{
		writer.Write((int)		modifier.When.Year);
		writer.Write((ushort)	modifier.When.Month);
		writer.Write((ushort)	modifier.When.Day);
		writer.Write((ushort)	modifier.When.Hour);
		writer.Write((ushort)	modifier.When.Minute);
		writer.Write((ushort)	modifier.Id);
		writer.Write((int)		modifier.Value);
		writer.Write((int)		modifier.Actual);
	}

	public void Serialize(BinaryWriter writer)
	{
		writer.Write((ushort)	_ability);
		writer.Write((uint)		_currentModifiersPvE.Count);
		foreach (var modifier in _currentModifiersPvE)
		{
			SerializeModifier(writer,modifier);
		}
		writer.Write((uint)		_currentModifiersPvP.Count);
		foreach (var modifier in _currentModifiersPvP)
		{
			SerializeModifier(writer,modifier);
		}

		writer.Write((uint)		_lifetimeModifiersPvE.Count);
		foreach (var modifier in _lifetimeModifiersPvE)
		{
			SerializeModifier(writer,modifier);
		}
		writer.Write((uint)		_lifetimeModifiersPvP.Count);
		foreach (var modifier in _lifetimeModifiersPvP)
		{
			SerializeModifier(writer,modifier);
		}
	}
#endregion


#region Constructor
	internal AbilityScore(Actor parent,Ability ability,ResourceDefaultValuesAttribute defaults) :
		this(parent,ability,defaults.HardLimit,defaults.SoftLimit,defaults.MoreIsBetter)
	{ }

	public AbilityScore(Actor parent,Ability ability,int hardLimit = -1,int softLimit = -1,bool moreIsBetter = true)
		: base(parent,hardLimit,softLimit,moreIsBetter)
	{
		_ability	= ability;
	}
#endregion
}