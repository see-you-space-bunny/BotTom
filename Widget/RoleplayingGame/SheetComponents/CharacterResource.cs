using System.Reflection.Metadata.Ecma335;
using FChatApi.Attributes;
using RoleplayingGame.Attributes;
using RoleplayingGame.Enums;
using RoleplayingGame.Objects;

namespace RoleplayingGame.SheetComponents;

public class CharacterResource : ResourceBase
{
	private const float OverallResourceScaling		= 1.0f;
	private const float OverallResorceExponentBase	= 1.3851f;
	private const float LevelScaleDivisor			= 1.0f;

	private float _baseValue;
	private Resource _resource;
	public Resource Key => _resource;

#region Recordkeeping
	private readonly List<(DateTime When,ulong Id,int Value,int Actual)> _lifetimeModifiersPvP = [];
	private readonly List<(DateTime When,EnvironmentSource Id,int Value,int Actual)> _lifetimeModifiersPvE = [];
	private readonly List<(DateTime When,ulong Id,int Value,int Actual)> _currentModifiersPvP = [];
	private readonly List<(DateTime When,EnvironmentSource Id,int Value,int Actual)> _currentModifiersPvE = [];

	public void Modify(DateTime when,ulong source,int value)
	{
		int actual = value;
		if (HasHardLimit)
			actual = (value+CurrentTotal > 0 ?
				Math.Min(value+CurrentTotal,HardLimit) : 
					Math.Max(value+CurrentTotal,HardLimit)) - CurrentTotal;
		_currentModifiersPvP.Add((when,source,value,actual));
	}
	public void Modify(DateTime when,EnvironmentSource source,int value)
	{
		int actual = value;
		if (HasHardLimit)
			actual = (value+CurrentTotal > 0 ?
				Math.Min(value+CurrentTotal,HardLimit) : 
					Math.Max(value+CurrentTotal,HardLimit)) - CurrentTotal;
		_currentModifiersPvE.Add((when,source,value,actual));
	}

	public int LifetimeGain	=> _lifetimeModifiersPvP.Sum(li=>li.Actual > 0 ? li.Actual : 0) + _lifetimeModifiersPvE.Sum(li=>li.Actual > 0 ? li.Actual : 0);
	public int LifetimeLoss	=> _lifetimeModifiersPvP.Sum(li=>li.Actual < 0 ? li.Actual : 0) + _lifetimeModifiersPvE.Sum(li=>li.Actual < 0 ? li.Actual : 0);
	public int LifetimeTotal=> _lifetimeModifiersPvP.Sum(li=>li.Actual) + _lifetimeModifiersPvE.Sum(li=>li.Actual);

	public int CurrentGain	=> _currentModifiersPvP.Sum(li=>li.Actual > 0 ? li.Actual : 0) + _currentModifiersPvE.Sum(li=>li.Actual > 0 ? li.Actual : 0);
	public int CurrentLoss	=> _currentModifiersPvP.Sum(li=>li.Actual < 0 ? li.Actual : 0) + _currentModifiersPvE.Sum(li=>li.Actual < 0 ? li.Actual : 0);
	public int CurrentTotal	=> _currentModifiersPvP.Sum(li=>li.Actual) + _currentModifiersPvE.Sum(li=>li.Actual);

	public int GetValue(bool softLimited = true) => softLimited && HasSoftLimit ? Math.Min(SoftLimit,CurrentTotal) : CurrentTotal;
#endregion

#region OutwardFacing
	public float BaseValue {
		get => _baseValue;
		set => _baseValue = HasHardLimit && value > HardLimit ?
			HardLimit : value;
	}

	public int Current {
		get => HasSoftLimit && CurrentNoSoftLimit > SoftLimit ?
			SoftLimit : CurrentNoSoftLimit;
	}

	public int CurrentNoSoftLimit => (int)((_baseValue + SumOfModifiers)*CombinedMultipliers);
#endregion

#region Limits
	public override bool IsAtSoftLimit => HasSoftLimit && Math.Abs(CurrentNoSoftLimit) >= SoftLimit;
	public override bool IsAtHardLimit => HasHardLimit && Math.Abs(CurrentNoSoftLimit) == HardLimit;
	
	public override bool IsOverSoftLimit => Math.Abs(CurrentNoSoftLimit) > SoftLimit;

	public void ReCalculateLimits()
	{
		int previousSoftLimit	= SoftLimit;
		int previousHardLimit	= HardLimit;
		int highestBaseValue	= int.MinValue;
		int highestMinimumValue	= int.MinValue;
		foreach(ClassLevels classLevels in _parent.ClassLevels.Values.Where((cl)=>cl.CurrentLevel>0))
		{
			highestMinimumValue	= Math.Max(highestBaseValue,(int)classLevels.Class.ResourceModifiers[Key][ResourceModifier.MinimumValue]);
			highestBaseValue	= Math.Max(highestBaseValue,(int)classLevels.Class.ResourceModifiers[Key][ResourceModifier.BaseValue]);
			var debugpowval	= (double)(classLevels.Class.ResourceAbilityScales[Key].Keys.Sum(k =>(k == Ability.Level ? 0 : _parent.Abilities[k].GetActualValue()) * classLevels.Class.ResourceAbilityScales[Key][k])/ classLevels.CurrentLevel);
			SoftLimit = HasSoftLimit ?
				(int)(OverallResourceScaling
					* classLevels.Class.ResourceModifiers[Key][ResourceModifier.SoftLimit]
					* (classLevels.Class.ResourceAbilityScales[Key][Ability.Level]
						* classLevels.CurrentLevel
						/ LevelScaleDivisor
				* (float)Math.Pow(
					OverallResorceExponentBase,
					(double)(classLevels.Class.ResourceAbilityScales[Key].Keys.Sum(k =>(k == Ability.Level ? 0 : _parent.Abilities[k].GetActualValue()) * classLevels.Class.ResourceAbilityScales[Key][k])
						/ classLevels.CurrentLevel)
				)))
					: -1;
			HardLimit = HasHardLimit ?
				(int)(OverallResourceScaling
					* classLevels.Class.ResourceModifiers[Key][ResourceModifier.HardLimit]
					* (classLevels.Class.ResourceAbilityScales[Key][Ability.Level]
						* classLevels.CurrentLevel
						/ LevelScaleDivisor
				* (float)Math.Pow(
					OverallResorceExponentBase,
					(double)(classLevels.Class.ResourceAbilityScales[Key].Keys.Sum(k =>(k == Ability.Level ? 0 : _parent.Abilities[k].GetActualValue()) * classLevels.Class.ResourceAbilityScales[Key][k])
						/ classLevels.CurrentLevel)
				)))
					: -1;
		}
		if (HasSoftLimit)
		{
			SoftLimit += highestBaseValue;
			SoftLimit = Math.Max(SoftLimit,highestMinimumValue);
		}
		if (HasHardLimit)
		{
			HardLimit += highestBaseValue;
			HardLimit = Math.Max(HardLimit,highestMinimumValue);
		}
		if (Key.GetEnumAttribute<Resource,GameFlagsAttribute>().ScalesOnLimitChange)
			BaseValue *= Math.Max(Math.Max(SoftLimit / Math.Max(previousSoftLimit,1), HardLimit / Math.Max(previousHardLimit,1)),1);
	}
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

	public static CharacterResource Deserialize(BinaryReader reader,Actor parent)
	{
		CharacterResource result = new (parent,(Resource)reader.ReadUInt16());

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
		writer.Write((ushort)	_resource);
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
	internal CharacterResource(Actor parent,Resource resource,ResourceDefaultValuesAttribute defaults) :
		this(parent,resource,defaults.BaseValue,defaults.HardLimit,defaults.SoftLimit,defaults.MoreIsBetter)
	{ }

	public CharacterResource(Actor parent,Resource resource,int baseValue = 0,int hardLimit = -1,int softLimit = -1,bool moreIsBetter = true)
		: base(parent,hardLimit,softLimit,moreIsBetter)
	{
		_baseValue	= baseValue;
		_resource	= resource;
	}
#endregion
}