using FChatApi.Core;
using FChatApi.Objects;
using RoleplayingGame.Enums;
using RoleplayingGame.SheetComponents;
using RoleplayingGame.Statistics;
using RoleplayingGame.Systems;

namespace RoleplayingGame.Objects;

public class CharacterSheet : Actor
{
#region Fields(-)
	private User _user;
	private string _uniqueCharacterName;
	private string _characterNickname;
	private bool _useOnlyNickname;
#endregion


#region (-) Fields
	protected static readonly TimeSpan ClassChangeCooldown;
	public DateTime _nextClassChange;
#endregion


#region Properties (+)
	public User User						=> _user;
	public new string CharacterName			=> _characterNickname != null ? (_useOnlyNickname ? _characterNickname : $"{_characterNickname} ({_characterName})") : _characterName;
	public bool CharacterNameIsIdentifier	{ get; internal set; }
#endregion

#region Properties (+)
	public bool CanChangeClass				=>	DateTime.Now >= _nextClassChange;
	public int RemainingClassChangeCooldown	=>	(int)Math.Max((_nextClassChange-DateTime.Now).TotalMinutes+1,0);
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


#region BeAdopted
	public CharacterSheet BecomeAdopted(User user)
	{
		_user = user;
		return this;
	}
#endregion


#region ChangeClass
	public CharacterSheet ChangeClass(CharacterClass @class,bool triggerCooldown = true)
	{
		if (triggerCooldown)
		{
			_nextClassChange = DateTime.Now + ClassChangeCooldown;
		}
		base.ChangeClass(@class);
		return this;
	}
#endregion


#region Serialization
	internal static CharacterSheet Deserialize(BinaryReader reader,CharacterClassTracker CharacterClasses)
	{
/////	Identifying Information
		var characterSheet = new CharacterSheet(
			reader.ReadString(),
			reader.ReadUInt64(),
			reader.ReadBoolean() ? null : reader.ReadString()
		);
			
		ApiConnection.Users.TrySingleByName(characterSheet._uniqueCharacterName,out characterSheet._user);

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
			ClassLevels classLevels = SheetComponents.ClassLevels.Deserialize(reader,CharacterClasses,characterSheet);
			characterSheet.ClassLevels.Add(classLevels.Class.Name,classLevels);
		}
		characterSheet.ChangeClass(CharacterClasses.All[(ClassName)	reader.ReadUInt32()]);

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
		writer.Write((ulong)	_actorId);
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
		ApiConnection.Users.TrySingleByName(_uniqueCharacterName,out _user);
	}

	public CharacterSheet(string uniqueCharacterName, ulong userId, string? characterName = null)
		: this(uniqueCharacterName,characterName)
	{
		_actorId                     = userId;
	}

	public CharacterSheet(User user, string? characterName = null)
		: this(user.Name,characterName)
	{
		_user = user;
	}
#endregion

#region Private Constructor
	private CharacterSheet() : base(string.Empty)
	{
		_actorId				= 0uL;
		_uniqueCharacterName	= string.Empty;
		_characterNickname		= string.Empty;
		_useOnlyNickname		= false;
		_user					= default!;
		_nextClassChange		= DateTime.Now;
	}
#endregion


#region Static Constructor
	static CharacterSheet()
	{
		ClassChangeCooldown = new TimeSpan(0,29,0);
	}
#endregion
}