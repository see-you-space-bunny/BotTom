using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;

namespace SurlyCardGame
{
    public class PlayerCharacter(string identity)
    {
        internal string? _mention;
        internal string Identity = identity;
        internal string Mention => _mention ?? Identity;
        internal bool MentionIsNotIdentity => 0 == string.Compare(Mention,Identity,true);
        internal string MentionAndIdentity => MentionIsNotIdentity ? $"{Mention} ({Identity})" : Identity;

        internal Dictionary<CharacterStat,int> Stats = new Dictionary<CharacterStat,int>{
            [CharacterStat.STR] = 0,
            [CharacterStat.VIT] = 0,
            [CharacterStat.DEX] = 0,
            [CharacterStat.INT] = 0,
            [CharacterStat.CHA] = 0,
            [CharacterStat.LUC] = 0,
        };

        public int GetStatValue(string stat)
        {
            if (Enum.TryParse<CharacterStat>(stat,true, out CharacterStat statKey))
                return Stats[statKey];
            throw new ArgumentException($"The selected '{stat}' is not a valid choice of stat.");
        }

        public MatchPlayer CreateMatchPlayer(string stat1,string stat2)
        {
            if (Enum.TryParse<CharacterStat>(stat1,true, out CharacterStat deckArchetype1))
            {
                if (Enum.TryParse<CharacterStat>(stat2,true, out CharacterStat deckArchetype2))
                {
                    return new MatchPlayer(this,deckArchetype1,deckArchetype2);
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