namespace CardGame.Enums
{
	public enum CommandState
	{
		SufficientPermission,
		InsufficientPermission,
		RateLimited,
		ActionLocked,
		AwaitingResponse,
		ResponseRequired,
	}
}