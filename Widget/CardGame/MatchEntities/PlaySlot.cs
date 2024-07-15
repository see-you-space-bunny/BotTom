using CardGame.Attributes;
using CardGame.Enums;
using FChatApi.Attributes;
using FChatApi.Enums;

namespace CardGame.MatchEntities
{
    public class PlaySlot
	{
		private const string OutputFormat = "([color={0}]{1}[/color]/[b]{2}[/b])";
		//private const string OutputFormatDamaged = "[color=white][u][color=red][b]{0}[/b][/color][/u][/color]";
		private const string EmptySlot = "(—/—)";
		internal SummonCard? Card;


		internal BBCodeColor Color => 
			Card != null ? 
				Card.CardArchetype.GetEnumAttribute<CharacterStat,StatDecorationAttribute>().Color :
				BBCodeColor.white;

		internal short _damage = 0;

		public void Clear()
		{
			Card = null!;
			_damage = 0;
		}

		public bool CanAttackSummons(ushort turn)
		{
			if (Card is null)
				return false;

			if (Card.CardArchetype == CharacterStat.DEX)
				return true;

			return Card.TurnAttacked < turn;
		}

		public bool CanAttackPlayer(ushort turn)
		{
			if (Card is null)
				return false;

			return Card.TurnAttacked < turn;
		}

		public override string ToString()
		{
			if (Card is not null)
			{
				return string.Format(OutputFormat,BBCodeColor.pink,Card.Power,Card.Health-_damage);
			}
			return EmptySlot;
		}
	}
}