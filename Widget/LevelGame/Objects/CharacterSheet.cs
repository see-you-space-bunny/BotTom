using RoleplayingGame.Enums;
using RoleplayingGame.Statistics;

namespace RoleplayingGame.Objects;

public class CharacterSheet : Actor
{
	#region Fields(-)
	private ulong _userId;
	private string _uniqueCharacterName;
	private string? _characterNickname;
	private bool _useOnlyNickname;
	#endregion

	#region Properties (+)
	public new string CharacterName => _characterNickname != null ? (_useOnlyNickname ? _characterNickname : $"{_characterNickname} ({_characterName})") : _characterName;
	public bool CharacterNameIsIdentifier { get; internal set; }
	#endregion

	#region Private Constructor
	private CharacterSheet() : base(string.Empty)
	{
		_userId                 = 0;
		_uniqueCharacterName    = string.Empty;

		_useOnlyNickname        = false;
	}
	#endregion

	#region Constructor
	public CharacterSheet(string uniqueCharacterName, string? characterName = null) : this()
	{
		_uniqueCharacterName        = uniqueCharacterName;
		_characterName              = characterName ?? uniqueCharacterName;
		CharacterNameIsIdentifier   = true;
	}

	public CharacterSheet(ulong userId, string? characterName = null) : this()
	{
		_userId                     = userId;
		_characterName              = characterName ?? string.Empty;
		CharacterNameIsIdentifier   = false;
	}
	#endregion



	#region Assignment Methods
	public CharacterSheet ClearNickname()
	{
		_characterNickname = null;
		return this;
	}
	public CharacterSheet WithNickname(string? nickname, bool? useOnlyNickname = null)
	{
		_characterNickname = nickname;
		if (useOnlyNickname != null)
			_useOnlyNickname = (bool)useOnlyNickname;
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
		var characterSheet = new CharacterSheet(
			reader.ReadString(),
			reader.ReadBoolean() ? null : reader.ReadString()
		);

		if (reader.ReadBoolean())
			characterSheet
				.WithNickname(useOnlyNickname:reader.ReadBoolean(), nickname:reader.ReadString());

		for (int i=0;i<reader.ReadUInt32();i++)
		{
			characterSheet.ChangeClass((ClassName) reader.ReadUInt32());
			characterSheet.LevelUp((int) reader.ReadInt32());
		}
		characterSheet.ChangeClass((ClassName)	reader.ReadUInt32());
		characterSheet.Level.BaseValue = characterSheet._classLevels.Values.Sum((cl)=>cl.Level);

		for (int i=0;i<reader.ReadUInt32();i++)
		{
			characterSheet._abilities[(Ability) reader.ReadUInt16()].BaseValue = reader.ReadInt32();
		}

		for (int i=0;i<reader.ReadUInt32();i++)
		{
			characterSheet._resources[(Resource) reader.ReadUInt16()].BaseValue = reader.ReadInt32();
		}

		characterSheet.Statistics = ActorStatistics.Deserialize(reader);
		return characterSheet;
	}

	public void Serialize(BinaryWriter writer)
	{
		writer.Write((string)	_uniqueCharacterName);

		writer.Write((bool)		_uniqueCharacterName.Equals(_characterName));
		if (!_uniqueCharacterName.Equals(_characterName))
			writer.Write((string)	_characterName);

		writer.Write((bool)		(_characterNickname is not null));
		if (_characterNickname is not null)
		{
			writer.Write((bool)		_useOnlyNickname);
			writer.Write((string)	_characterNickname);
		}

		writer.Write((uint) 	_classLevels.Count);
		foreach (var classLevel in _classLevels)
		{
			writer.Write((uint)		classLevel.Key);
			writer.Write((int)		classLevel.Value.Level);
		}
		writer.Write((uint)		_activeClass);

		writer.Write((uint) 	Abilities.Count);
		foreach (var ability in Abilities)
		{
			writer.Write((ushort)	ability.Key);
			writer.Write((int)		ability.Value.BaseValue);
		}

		writer.Write((uint) 	_resources.Count);
		foreach (var resource in _resources)
		{
			writer.Write((ushort)	resource.Key);
			writer.Write((int)		resource.Value.CurrentNoSoftLimit);
		}
		Statistics.Serialize(writer);
	}

	#endregion
}