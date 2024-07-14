using CardGame.Attributes;
using CardGame.Enums;
using CardGame.PersistentEntities;
using FChatApi.Attributes;
using FChatApi.Enums;
using FChatApi.Objects;

namespace CardGame.MatchEntities;

public class MatchPlayer
{
	private const string OutputFormat = "{0} | 💖 {1} - 🃏 {2} | [color={4}]1[/color] {3} | [color={6}]2[/color] {5} | [color={8}]3[/color] {7} | [color=green]{9}[/color]";
	private const string ColorTags = "[color={1}]{0}[/color]";
	private const char MetaPointSymbol = '❚';


	internal User User;
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

	internal PlaySlot[] PlaySlots;

	internal bool HasDefender => PlaySlots.Any(s => s.Card != null && s.Card.CardArchetype == CharacterStat.VIT);

	private MatchPlayer(User user,PlayerCharacter playerCharacter,short health = 25,short deckSize = 5,short metaPoints = 3)
	{
		User = user;
		PlayerCharacter = playerCharacter;
		Health = health;
		DeckSize = deckSize;
		MetaPoints = metaPoints;
		PlaySlots = [new PlaySlot(),new PlaySlot(),new PlaySlot()];
	}

	public MatchPlayer(User user,PlayerCharacter playerCharacter,CharacterStat deckArchetype1,short health = 25,short deckSize = 5,short metaPoints = 3)
		: this(user,playerCharacter,health,deckSize,metaPoints)
	{
		DeckArchetype = (deckArchetype1,CharacterStat.NON);
	}
	public MatchPlayer(User user,PlayerCharacter playerCharacter,CharacterStat deckArchetype1,CharacterStat deckArchetype2,short health = 25,short deckSize = 5,short metaPoints = 3)
		: this(user,playerCharacter,health,deckSize,metaPoints)
	{
		DeckArchetype = (deckArchetype1,deckArchetype2);
	}

	private static string EncloseWithColor(string value, BBCodeColor color) => string.Format(ColorTags,value,color);

	public override string ToString()
	{
		return string.Format(OutputFormat,
			PlayerCharacter.User.Mention,
			Health,
			DeckSize,
			Slot1.ToString(),
			Slot1.Color,
			Slot2.ToString(),
			Slot2.Color,
			Slot3.ToString(),
			Slot3.Color,
			new string(MetaPointSymbol,MetaPoints)
		);
	}

	public string ToString(bool activePlayer)
	{
		return string.Format(OutputFormat,
			EncloseWithColor("[b]➤[/b] ",activePlayer ? BBCodeColor.green : BBCodeColor.black) + PlayerCharacter.User.Mention,
			Health,
			DeckSize,
			Slot1.ToString(),
			Slot1.Color,
			Slot2.ToString(),
			Slot2.Color,
			Slot3.ToString(),
			Slot3.Color,
			new string(MetaPointSymbol,MetaPoints)
		);
	}
}