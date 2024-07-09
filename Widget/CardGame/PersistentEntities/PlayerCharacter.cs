using CardGame.MatchEntities;
using CardGame.Enums;
using FChatApi.Objects;
using FChatApi.Core;

namespace CardGame.PersistentEntities
{
    public class PlayerCharacter
	{
		internal User User;
		internal string Key => User.Key;
		internal Dictionary<CharacterStat,int> Stats;

		public PlayerCharacter(User user)
		{
			User = user;
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
			if (Enum.TryParse(stat1,true, out CharacterStat deckArchetype1))
			{
				if (Enum.TryParse(stat2,true, out CharacterStat deckArchetype2))
				{
					return new MatchPlayer(this,deckArchetype1,deckArchetype2:deckArchetype2);
				}
				throw new ArgumentException($"The selected '{stat2}' is not a valid choice of stat.");
			}
			if (Enum.TryParse(stat2,true, out CharacterStat __deckArchetype2))
				throw new ArgumentException($"The selected '{stat1}' is not a valid choice of stat.");

			throw new ArgumentException($"Both selected '{stat1}' and '{stat2}' are not valid choices of stats.");
		}
		public MatchPlayer CreateMatchPlayer(string stat1)
		{
			if (Enum.TryParse(stat1,true, out CharacterStat deckArchetype1))
				return new MatchPlayer(this,deckArchetype1);

			throw new ArgumentException($"The selected '{stat1}' is not a valid choice of stat.");
		}

		public static PlayerCharacter Deserialize(BinaryReader reader)
		{
			PlayerCharacter player = new PlayerCharacter(ApiConnection.GetUserByName(reader.ReadString()));
			for(int i = 0;i < reader.ReadInt32();i++)
			{
				player.Stats.Add((CharacterStat)reader.ReadUInt16(),reader.ReadInt32());
			}
			return player;
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
	}
}