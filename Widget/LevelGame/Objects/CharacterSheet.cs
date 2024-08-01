using RoleplayingGame.Enums;
using RoleplayingGame.SheetComponents;
using RoleplayingGame.Statistics;

namespace RoleplayingGame.Objects;

public class CharacterSheet : Actor
{
#region Fields(-)
	private string _uniqueCharacterName;
	private string _characterNickname;
	private bool _useOnlyNickname;
#endregion

#region Properties (+)
	public new string CharacterName => _characterNickname != null ? (_useOnlyNickname ? _characterNickname : $"{_characterNickname} ({_characterName})") : _characterName;
	public bool CharacterNameIsIdentifier { get; internal set; }
#endregion

#region Private Constructor
	private CharacterSheet() : base(string.Empty)
	{
		_userId                 = 0uL;
		_uniqueCharacterName    = string.Empty;
		_characterNickname		= string.Empty;
		_useOnlyNickname        = false;
	}
#endregion

#region Assignment Methods
	public CharacterSheet ClearNickname()
	{
		_characterNickname	= string.Empty;
		_useOnlyNickname	= false;
		return this;
	}
	public CharacterSheet WithNickname(string nickname)
	{
		_characterNickname = nickname;
		return this;
	}
#endregion

#region Toggle Mothods
	public CharacterSheet ToggleUsingOnlyNickname()
	{
		_useOnlyNickname = !_useOnlyNickname;
		return this;
	}
#endregion

#region Serialization
	public static CharacterSheet Deserialize(BinaryReader reader)
	{
/////	Identifying Information
		var characterSheet = new CharacterSheet(
			reader.ReadString(),
			reader.ReadUInt64(),
			reader.ReadBoolean() ? null : reader.ReadString()
		);

/////	Nickname
		if (reader.ReadBoolean())
		{
			characterSheet._useOnlyNickname = reader.ReadBoolean();
			characterSheet.WithNickname(nickname:reader.ReadString());
		}

/////	Class Levels
		characterSheet._levelCap = reader.ReadUInt32();
		for (int i=0;i<reader.ReadUInt32();i++)
		{
			ClassLevels classLevels = SheetComponents.ClassLevels.Deserialize(reader,characterSheet);
			characterSheet.ClassLevels.Add(classLevels.Class.Name,classLevels);
		}
		characterSheet.ChangeClass((ClassName)	reader.ReadUInt32());

/////	Abilities
		for (int i=0;i<reader.ReadUInt32();i++)
		{
			AbilityScore abilityScore = AbilityScore.Deserialize(reader,characterSheet);
			characterSheet._abilities[abilityScore.Key] = abilityScore;
		}

/////	Resources
		for (int i=0;i<reader.ReadUInt32();i++)
		{
			CharacterResource resource = CharacterResource.Deserialize(reader,characterSheet);
			characterSheet._resources[resource.Key] = resource;
		}

/////	Statistics
		characterSheet.Statistics = ActorStatistics.Deserialize(reader);

/////	End
		return characterSheet;
	}

	public void Serialize(BinaryWriter writer)
	{
/////	Identifying Information
		writer.Write((string)	_uniqueCharacterName);
		writer.Write((ulong)	_userId);
		writer.Write((bool)		_uniqueCharacterName.Equals(_characterName));
		if (!_uniqueCharacterName.Equals(_characterName))
			writer.Write((string)	_characterName);

/////	Nickname
		writer.Write((bool)		!string.IsNullOrWhiteSpace(_characterNickname));
		if (!string.IsNullOrWhiteSpace(_characterNickname))
		{
			writer.Write((bool)		_useOnlyNickname);
			writer.Write((string)	_characterNickname);
		}

/////	Class Levels
		writer.Write((uint) 	LevelCap);
		writer.Write((uint) 	_classLevels.Count);
		foreach (var classLevel in _classLevels)
		{
			classLevel.Value.Serialize(writer);
		}
		writer.Write((uint)		_activeClass);

/////	Abilities
		writer.Write((uint) 	Abilities.Count);
		foreach (var ability in Abilities)
		{
			ability.Value.Serialize(writer);
		}

/////	Resources
		writer.Write((uint) 	_resources.Count);
		foreach (var resource in _resources)
		{
			resource.Value.Serialize(writer);
		}

/////	Statistics
		Statistics.Serialize(writer);
	}
#endregion

#region Constructor
	public CharacterSheet(string uniqueCharacterName, string? characterName = null)
		: this()
	{
		_uniqueCharacterName        = uniqueCharacterName;
		_characterName              = characterName ?? uniqueCharacterName;
	}

	public CharacterSheet(string uniqueCharacterName, ulong userId, string? characterName = null)
		: this(uniqueCharacterName,characterName)
	{
		_userId                     = userId;
	}
#endregion
}