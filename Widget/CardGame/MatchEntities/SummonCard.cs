using CardGame.Enums;

namespace CardGame.MatchEntities;

public class SummonCard(CharacterStat cardArchetype,ushort power,ushort health,ushort turnSummoned = 0,string? name = null)
{
	private bool _secondAttackTaken = false;
	internal ushort Power = power;
	internal ushort PowerVsSummons => (ushort)(Power * CardArchetype switch {
		CharacterStat.INT => 0.5f,
		CharacterStat.STR => 1.5f,
		_ => 1.0f,
	});
	internal ushort Health = health;
	internal string Name = name ?? string.Empty;
	internal CharacterStat CardArchetype = cardArchetype;
	internal ushort TurnSummoned = turnSummoned;
	internal ushort TurnAttacked = turnSummoned;
	internal bool SecondAttackTaken { get=> CardArchetype != CharacterStat.INT || _secondAttackTaken; set=>_secondAttackTaken=value; }
}