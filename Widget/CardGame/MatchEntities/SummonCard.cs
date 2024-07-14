using CardGame.Enums;

namespace CardGame.MatchEntities;

public class SummonCard(CharacterStat cardArchetype,ushort power,ushort health,ushort turnSummoned = 0,string? name = null)
{
	internal ushort Power = power;
	internal ushort PowerVsSummons => CardArchetype == CharacterStat.STR ? (ushort)Math.Ceiling(Power*1.5f) : Power;
	internal ushort Health = health;
	internal string Name = name ?? string.Empty;
	internal CharacterStat CardArchetype = cardArchetype;
	internal ushort TurnSummoned = turnSummoned;
	internal ushort TurnAttacked = turnSummoned;
}