using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Widget.CardGame.Exceptions;

[Serializable]
internal class InvalidCommandSyntaxException : Exception
{
	public InvalidCommandSyntaxException()
	{
	}

	public InvalidCommandSyntaxException(string? message) : base(message)
	{
	}

	public InvalidCommandSyntaxException(string? message, Exception? innerException) : base(message, innerException)
	{
	}
}