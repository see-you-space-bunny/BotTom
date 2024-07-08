namespace Widget.CardGame.Interfaces
{
    public interface ICommandIO<TKey>
	{
		internal TKey AlternateKey { get; }
	}
}