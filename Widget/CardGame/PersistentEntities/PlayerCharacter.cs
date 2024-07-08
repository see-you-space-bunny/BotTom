using Widget.CardGame.MatchEntities;
using Widget.CardGame.Enums;

namespace Widget.CardGame.PersistentEntities
{
    public class PlayerCharacter
	{
		internal string _mention;
		internal string Identity;
		internal string Mention { get => _mention; set { _mention = value; MentionIsIdentity = false; } }
		internal bool MentionIsIdentity { get; set; }
		internal string MentionAndIdentity => MentionIsIdentity ? $"[user]{Identity}[/user]" : $"{Mention} ([user]{Identity}[/user])";

		internal Dictionary<CharacterStat,int> Stats;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor.
		public PlayerCharacter(string identity)
#pragma warning restore CS8618 // Consider adding the 'required' modifier or declaring as nullable.
		{
			Stats = new Dictionary<CharacterStat,int>{
				[CharacterStat.STR] = 0,
				[CharacterStat.VIT] = 0,
				[CharacterStat.DEX] = 0,
				[CharacterStat.INT] = 0,
				[CharacterStat.CHA] = 0,
				[CharacterStat.LUC] = 0,
				[CharacterStat.LVL] = 0,
			};
			Mention     = identity;
			Identity    = identity;
			MentionIsIdentity = true;
		}

		public void ResetMention() => _mention = Identity;

		public int GetStatValue(string stat)
		{
			if (Enum.TryParse<CharacterStat>(stat,true, out CharacterStat statKey))
				return Stats[statKey];
			throw new ArgumentException($"The selected '{stat}' is not a valid choice of stat.");
		}
		public int GetStatValue(CharacterStat stat) => Stats[stat];

		public MatchPlayer CreateMatchPlayer(CharacterStat stat1, CharacterStat? stat2 = null)
		{
			if (stat2 is not null)
				return new MatchPlayer(this,deckArchetype1: stat1,deckArchetype2: (CharacterStat)stat2);
			else
				return new MatchPlayer(this,deckArchetype1: stat1);
		}

		public MatchPlayer CreateMatchPlayer(string stat1,string stat2)
		{
			if (Enum.TryParse<CharacterStat>(stat1,true, out CharacterStat deckArchetype1))
			{
				if (Enum.TryParse<CharacterStat>(stat2,true, out CharacterStat deckArchetype2))
				{
					return new MatchPlayer(this,deckArchetype1,deckArchetype2:deckArchetype2);
				}
				throw new ArgumentException($"The selected '{stat2}' is not a valid choice of stat.");
			}
			if (Enum.TryParse<CharacterStat>(stat2,true, out CharacterStat __deckArchetype2))
				throw new ArgumentException($"The selected '{stat1}' is not a valid choice of stat.");

			throw new ArgumentException($"Both selected '{stat1}' and '{stat2}' are not valid choices of stats.");
		}

		public MatchPlayer CreateMatchPlayer(string stat1)
		{
			if (Enum.TryParse<CharacterStat>(stat1,true, out CharacterStat deckArchetype1))
				return new MatchPlayer(this,deckArchetype1);

			throw new ArgumentException($"The selected '{stat1}' is not a valid choice of stat.");
		}
	}
}