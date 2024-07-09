using CardGame.Enums;

namespace CardGame.Exceptions;

[Serializable]
internal class DisallowedCommandException : Exception
{
	internal CardGameCommand Command;
	internal CommandState Reason;

	public DisallowedCommandException(CardGameCommand command,CommandState reason) : base()
	{
		Command = command;
		Reason = reason;
	}

	public DisallowedCommandException(CardGameCommand command,CommandState reason,string? message) : base(message)
	{
		Command = command;
		Reason = reason;
	}

	public DisallowedCommandException(CardGameCommand command,CommandState reason,string? message,Exception? innerException) : base(message, innerException)
	{
		Command = command;
		Reason = reason;
	}
}