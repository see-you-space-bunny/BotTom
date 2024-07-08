using Widget.CardGame.Enums;

namespace Widget.CardGame.Exceptions;


[Serializable]
internal class DisallowedCommandException : Exception
{
	internal Command Command;
	internal CommandPermission Reason;

	public DisallowedCommandException(Command command,CommandPermission reason) : base()
	{
		Command = command;
		Reason = reason;
	}

	public DisallowedCommandException(Command command,CommandPermission reason,string? message) : base(message)
	{
		Command = command;
		Reason = reason;
	}

	public DisallowedCommandException(Command command,CommandPermission reason,string? message,Exception? innerException) : base(message, innerException)
	{
		Command = command;
		Reason = reason;
	}
}