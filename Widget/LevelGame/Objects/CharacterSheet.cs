namespace Widget.LevelGame;

public class CharacterSheet : Actor
{
	#region Fields(-)
	private ulong _userId;
	private string _uniqueCharacterName;
	private string _characterName;
	private string? _characterNickname;
	private bool _useOnlyNickname;
	#endregion

	#region Properties (+)
	public string CharacterName => _characterNickname != null ? (_useOnlyNickname ? _characterNickname : $"{_characterNickname} ({_characterName})") : _characterName;
	public bool CharacterNameIsIdentifier { get; internal set; }
	#endregion

	#region Private Constructor
	private CharacterSheet() : base()
	{
		_userId                 = 0;
		_uniqueCharacterName    = string.Empty;
		_characterName          = string.Empty;

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
}