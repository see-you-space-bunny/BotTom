using CardGame.Enums;

namespace CardGame.MatchEntities
{
    public struct SummonCard(short power,short health,CharacterStat cardArchetype)
	{
		internal short Power = power;
		internal short Health = health;
		internal CharacterStat CardArchetype = cardArchetype;
	}
}