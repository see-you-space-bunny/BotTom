namespace CardGame.Interfaces
{
    public interface ICommandIO<TKey>
	{
		internal TKey AlternateKey { get; }
	}
}