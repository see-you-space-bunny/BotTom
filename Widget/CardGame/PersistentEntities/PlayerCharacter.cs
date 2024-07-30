using CardGame.MatchEntities;
using CardGame.Enums;
using FChatApi.Objects;
using FChatApi.Core;
using System.Text;
using FChatApi.Attributes;
using CardGame.Attributes;
using Plugins.Tokenizer;
using FChatApi.Enums;

namespace CardGame.PersistentEntities;

public class PlayerCharacter
{
	private const string FormatStatEntry = "{3}[color={2}]{0}[/color] {1}";
	private const string FormatStatBlock = "[color=white]Level {0} | [b]Core Stats[/b] TOT {7}\n\t{1}    {2}    {3}\n\t\t{4}    {5}    {6}[/color]";
	private const BBCodeColor DefaultStatColor = BBCodeColor.white;

	internal User User;
	internal string _orphanName;
	internal string Key => User?.Name ?? _orphanName;
	internal bool IsOrphan => !string.IsNullOrWhiteSpace(_orphanName);
	internal Dictionary<CharacterStat,int> Stats;

	public PlayerCharacter(User user) : this()
	{
		User = user;
	}

	public PlayerCharacter(string name) : this()
	{
		_orphanName = name;
	}

	private PlayerCharacter()
	{
		User = null!;
		_orphanName = string.Empty;
		Stats = new Dictionary<CharacterStat,int>{
			[CharacterStat.STR] = 0,
			[CharacterStat.VIT] = 0,
			[CharacterStat.DEX] = 0,
			[CharacterStat.INT] = 0,
			[CharacterStat.CHA] = 0,
			[CharacterStat.LUC] = 0,
			[CharacterStat.LVL] = 0,
		};
	}

	public MatchPlayer CreateMatchPlayer(CharacterStat stat1, CharacterStat? stat2 = null)
	{
		if (stat2 is not null)
			return new MatchPlayer(User,this,deckArchetype1: stat1,deckArchetype2: (CharacterStat)stat2);
		else
			return new MatchPlayer(User,this,deckArchetype1: stat1);
	}

	public static PlayerCharacter Deserialize(BinaryReader reader)
	{
		if (ApiConnection.Users.TrySingleByName(reader.ReadString(),out User user))
		{
			var result = new PlayerCharacter(user);
			for(int i = 0;i < reader.ReadInt32();i++)
			{
				result.Stats.AddOrUpdate((CharacterStat)reader.ReadUInt16(),reader.ReadInt32());
			}
			return result;
		}
		//	if no valid user found,
		//		read through the rest of the entry until the next one
		for(int i = 0;i < reader.ReadInt32();i++)
		{
			reader.ReadUInt16();
			reader.ReadInt32();
		}
		return default!;
	}

	public void Serialize(BinaryWriter writer)
	{
		writer.Write(Key);
		writer.Write(Stats.Count);
		foreach ((CharacterStat stat, int value) in Stats)
		{
			writer.Write((ushort)	stat);
			writer.Write((int)		value);
		}
	}

	private string GetFormattedStat(CharacterStat value)
	{
		(string Color,string Emoji) deco;
		if (value.HasEnumAttribute<CharacterStat,StatDecorationAttribute>())
		{
			deco = (
				value.GetEnumAttribute<CharacterStat,StatDecorationAttribute>().Color.ToString(),
				value.GetEnumAttribute<CharacterStat,StatDecorationAttribute>().Emoji
			);
		}
		else
		{
			deco = (DefaultStatColor.ToString(),string.Empty);
		}
		return string.Format(FormatStatEntry,
			value.ToString(),
			Stats[value].ToString(),
			deco.Color,
			deco.Emoji
		);
	}

	public string GetFormattedStatBlock()
	{
		return string.Format(
			FormatStatBlock,
			Stats[CharacterStat.LVL].ToString(),
			GetFormattedStat(CharacterStat.STR),
			GetFormattedStat(CharacterStat.VIT),
			GetFormattedStat(CharacterStat.DEX),
			GetFormattedStat(CharacterStat.INT),
			GetFormattedStat(CharacterStat.CHA),
			GetFormattedStat(CharacterStat.LUC),
			Stats.Values.Sum().ToString()
		);
	}

	public override string ToString()
	{
		return base.ToString() ?? string.Empty;
	}
}