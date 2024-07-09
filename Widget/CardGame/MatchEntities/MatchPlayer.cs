using CardGame.Enums;
using CardGame.PersistentEntities;

namespace CardGame.MatchEntities;

public class MatchPlayer
{
	private const string OutputFormat = "{0} | 💖 {1} - 🃏 {2} | 1 {3} | 2 {4} | 3 {5} | [color=green]{6}[/color]";
	private const string ColorTags = "[color={1}]{0}[/color]";
	private const char MetaPointSymbol = '❚';


	internal PlayerCharacter PlayerCharacter;
	internal (CharacterStat First,CharacterStat Second) DeckArchetype;
	internal short Health;
	internal short DeckSize;
	internal short MetaPoints;

	internal PlaySlot Slot1 => PlaySlots[0];
	internal bool IsCardInSlot1 => PlaySlots[0].Card is not null;

	internal PlaySlot Slot2 => PlaySlots[1];
	internal bool IsCardInSlot2 => PlaySlots[1].Card is not null;

	internal PlaySlot Slot3 => PlaySlots[2];
	internal bool IsCardInSlot3 => PlaySlots[2].Card is not null;

	internal PlaySlot[] PlaySlots = new PlaySlot[3];

	private MatchPlayer(PlayerCharacter playerCharacter,short health = 25,short deckSize = 5,short metaPoints = 3)
	{
			PlayerCharacter = playerCharacter;
			Health = health;
			DeckSize = deckSize;
			MetaPoints = metaPoints;
	}

	public MatchPlayer(PlayerCharacter playerCharacter,CharacterStat deckArchetype1,short health = 25,short deckSize = 5,short metaPoints = 3)
		: this(playerCharacter,health,deckSize,metaPoints)
	{
		DeckArchetype = (deckArchetype1,CharacterStat.NON);
	}
	public MatchPlayer(PlayerCharacter playerCharacter,CharacterStat deckArchetype1,CharacterStat deckArchetype2,short health = 25,short deckSize = 5,short metaPoints = 3)
		: this(playerCharacter,health,deckSize,metaPoints)
	{
		DeckArchetype = (deckArchetype1,deckArchetype2);
	}

	private static string EncloseWithColor(string value, string color) => string.Format(ColorTags,value,color);

	public override string ToString()
	{
		return string.Format(OutputFormat,
			PlayerCharacter.MentionAndIdentity,
			Health,
			DeckSize,
			Slot1.ToString(),
			Slot2.ToString(),
			Slot3.ToString(),
			new string(MetaPointSymbol,MetaPoints)
		);
	}

	public string ToString(bool activePlayer)
	{
		return string.Format(OutputFormat,
			activePlayer ? EncloseWithColor(PlayerCharacter.MentionAndIdentity,"green") : PlayerCharacter.MentionAndIdentity,
			Health,
			DeckSize,
			Slot1.ToString(),
			Slot2.ToString(),
			Slot3.ToString(),
			new string(MetaPointSymbol,MetaPoints)
		);
	}
}