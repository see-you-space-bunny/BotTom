using RoleplayingGame.Enums;
using RoleplayingGame.Objects;
using RoleplayingGame.Systems;

namespace RoleplayingGame.SheetComponents;

public class ClassLevels
{
#region (-) Fields
	private Actor _parent;
	public CharacterClass Class;
	private ClassName ClassName	=>	Class.Name;
#endregion


#region (-) Records
	private readonly List<(DateTime When,ulong Id,int Value,int Actual)>				_lifetimeLevelsPvP	= [];
	private readonly List<(DateTime When,EnvironmentSource Id,int Value,int Actual)>	_lifetimeLevelsPvE	= [];
	private readonly List<(DateTime When,ulong Id,int Value,int Actual)>				_currentLevelsPvP	= [];
	private readonly List<(DateTime When,EnvironmentSource Id,int Value,int Actual)>	_currentLevelsPvE	= [];
#endregion


#region (+) Modify
	public void Modify(DateTime when,ulong source,int value)
	{
		int actual = (int)((value + CurrentLevel > 0 ?
				Math.Min(value + CurrentLevel, _parent.LevelCap) :
					Math.Max(value + CurrentLevel, _parent.LevelCap)) - CurrentLevel);
		_currentLevelsPvP.Add((when,source,value,actual));
	}
	public void Modify(DateTime when,EnvironmentSource source,int value)
	{
		int actual = (int)((value + CurrentLevel > 0 ?
				Math.Min(value + CurrentLevel, _parent.LevelCap) :
					Math.Max(value + CurrentLevel, _parent.LevelCap)) - CurrentLevel);
		_currentLevelsPvE.Add((when,source,value,actual));
	}
#endregion


#region (+) Lifetime
	public int LifetimeGain	=> _lifetimeLevelsPvP.Sum(li=>li.Actual > 0 ? li.Actual : 0) + _lifetimeLevelsPvE.Sum(li=>li.Actual > 0 ? li.Actual : 0);
	public int LifetimeLoss	=> _lifetimeLevelsPvP.Sum(li=>li.Actual < 0 ? li.Actual : 0) + _lifetimeLevelsPvE.Sum(li=>li.Actual < 0 ? li.Actual : 0);
	public int LifetimeTotal=> _lifetimeLevelsPvP.Sum(li=>li.Actual) + _lifetimeLevelsPvE.Sum(li=>li.Actual);
#endregion


#region (+) Current
	public int CurrentGain	=> _currentLevelsPvP.Sum(li=>li.Actual > 0 ? li.Actual : 0) + _currentLevelsPvE.Sum(li=>li.Actual > 0 ? li.Actual : 0);
	public int CurrentLoss	=> _currentLevelsPvP.Sum(li=>li.Actual < 0 ? li.Actual : 0) + _currentLevelsPvE.Sum(li=>li.Actual < 0 ? li.Actual : 0);
	public int CurrentLevel	=> _currentLevelsPvP.Sum(li=>li.Actual) + _currentLevelsPvE.Sum(li=>li.Actual);
#endregion


#region Serialization
	private static (DateTime When,ulong Id,int Value,int Actual) DeserializePvPModifier(BinaryReader reader)
		=> (
			new DateTime(reader.ReadInt64()),
			reader.ReadUInt64(),
			reader.ReadInt32(),
			reader.ReadInt32()
		);
	private static (DateTime When,EnvironmentSource Id,int Value,int Actual) DeserializePvEModifier(BinaryReader reader)
		=> (
			new DateTime(reader.ReadInt64()),
			(EnvironmentSource)	reader.ReadUInt16(),
			reader.ReadInt32(),
			reader.ReadInt32()
		);

	internal static ClassLevels Deserialize(BinaryReader reader,CharacterClassTracker CharacterClasses,Actor parent)
	{
		ClassLevels result = new (parent,CharacterClasses.All[(ClassName)reader.ReadUInt16()]);

		for (uint i=0;i<reader.ReadUInt32();i++)
		{
			result._currentLevelsPvE.Add(DeserializePvEModifier(reader));
		}
		for (uint i=0;i<reader.ReadUInt32();i++)
		{
			result._currentLevelsPvP.Add(DeserializePvPModifier(reader));
		}

		for (uint i=0;i<reader.ReadUInt32();i++)
		{
			result._lifetimeLevelsPvE.Add(DeserializePvEModifier(reader));
		}
		for (uint i=0;i<reader.ReadUInt32();i++)
		{
			result._lifetimeLevelsPvP.Add(DeserializePvPModifier(reader));
		}

		return result;
	}

	private static void SerializeModifier(BinaryWriter writer,(DateTime When,ulong Id,int Value,int Actual) modifier)
	{
		writer.Write((long)		modifier.When.Ticks);
		writer.Write((ulong)	modifier.Id);
		writer.Write((int)		modifier.Value);
		writer.Write((int)		modifier.Actual);
	}
	private static void SerializeModifier(BinaryWriter writer,(DateTime When,EnvironmentSource Id,int Value,int Actual) modifier)
	{
		writer.Write((long)		modifier.When.Ticks);
		writer.Write((ushort)	modifier.Id);
		writer.Write((int)		modifier.Value);
		writer.Write((int)		modifier.Actual);
	}

	public void Serialize(BinaryWriter writer)
	{
		writer.Write((uint)		ClassName);
		writer.Write((uint)		_currentLevelsPvE.Count);
		foreach (var modifier in _currentLevelsPvE)
		{
			SerializeModifier(writer,modifier);
		}
		writer.Write((uint)		_currentLevelsPvP.Count);
		foreach (var modifier in _currentLevelsPvP)
		{
			SerializeModifier(writer,modifier);
		}

		writer.Write((uint)		_lifetimeLevelsPvE.Count);
		foreach (var modifier in _lifetimeLevelsPvE)
		{
			SerializeModifier(writer,modifier);
		}
		writer.Write((uint)		_lifetimeLevelsPvP.Count);
		foreach (var modifier in _lifetimeLevelsPvP)
		{
			SerializeModifier(writer,modifier);
		}
	}
#endregion


#region Constructor
	public ClassLevels(Actor parent,CharacterClass @class)
	{
		_parent		=	parent;
		Class		=	@class;
	}
#endregion
}