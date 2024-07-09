namespace CardGame.MatchEntities
{
    public class PlaySlot
	{
		private const string OutputFormat = "[color={0}]([/color]{1}[color={0}]/[/color]{2}[color={0}])[/color]";
		private const string OutputFormatDamaged = "[color=white][u][color=red][b]{0}[/b][/color][/u][/color]";
		private const string EmptySlot = "(—/—)";
		internal SummonCard? Card;
		internal short _damage = 0;

		public void Clear()
		{
			Card = null;
			_damage = 0;
		}

		public override string ToString()
		{
			if (Card is not null)
			{
				var card = (SummonCard)Card;
				if (_damage > 0)
					return string.Format(OutputFormat,"white",card.Power,string.Format(OutputFormatDamaged,card.Health-_damage));
				else
					return string.Format(OutputFormat,"white",card.Power,card.Health);
			}
			return EmptySlot;
		}
	}
}