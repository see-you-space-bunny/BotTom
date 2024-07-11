using CardGame.Attributes;
using CardGame.Enums;
using CardGame.PersistentEntities;
using FChatApi.Attributes;
using FChatApi.Enums;
using FChatApi.Objects;

namespace CardGame.MatchEntities;

public class MatchPlayer
{
	private const string OutputFormat = "{0} | 💖 {1} - 🃏 {2} | 1 {3} | 2 {4} | 3 {5} | [color=green]{6}[/color]";
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

	internal PlaySlot[] PlaySlots = new PlaySlot[3];

	private MatchPlayer(User user,PlayerCharacter playerCharacter,short health = 25,short deckSize = 5,short metaPoints = 3)
	{
		User = user;
		PlayerCharacter = playerCharacter;
		Health = health;
		DeckSize = deckSize;
		MetaPoints = metaPoints;
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

	internal (string Result,SummonCard Card) DrawCard(CharacterStat stat)
	{
		int dieRoll			= new Random().Next(1,101);
		int statModifier	= PlayerCharacter.Stats[stat]/10;
		short rollSum		= (short)((dieRoll+statModifier)/10);
		--DeckSize;
		return (
			string.Format("1d100 ({0}) + {1} = {2}",
				dieRoll,
				EncloseWithColor(statModifier.ToString(),stat.GetEnumAttribute<CharacterStat,StatColorAttribute>().Color),
				dieRoll+statModifier
			),
			new SummonCard(rollSum,rollSum,stat)
		);
	}

	internal void PlayCard(SummonCard card)
	{
		if (!IsCardInSlot1)
		{
			Slot1.Card = card;
			return;
		}
		else if (!IsCardInSlot2)
		{
			Slot2.Card = card;
			return;
		}
		else if (!IsCardInSlot3)
		{
			Slot3.Card = card;
			return;
		}
		else
			throw new InvalidOperationException("Cant't play a card while all slots are full!!");
	}

	public override string ToString()
	{
		return string.Format(OutputFormat,
			PlayerCharacter.User.Mention,
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
			activePlayer ? EncloseWithColor(PlayerCharacter.User.Mention,BBCodeColor.green) : PlayerCharacter.User.Mention,
			Health,
			DeckSize,
			Slot1.ToString(),
			Slot2.ToString(),
			Slot3.ToString(),
			new string(MetaPointSymbol,MetaPoints)
		);
	}
}