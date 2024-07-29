using System.Text.RegularExpressions;
using FChatApi.Attributes;
using FChatApi.Objects;
using Plugins.Attributes;

namespace Plugins.Tokenizer;

public class CommandTokens(FChatMessage fChatMessage, string command, string parameters)
{
	public FChatMessage Source { get; }		= fChatMessage;
	public string Command { get; }			= command;
	public string _parameters { get; }		= parameters;
	public bool TryGetParameters<TCommand>(out TCommand command, out Dictionary<string,string> parameters) where TCommand : struct, Enum
	{
		parameters = new (StringComparer.InvariantCultureIgnoreCase);
		if (Enum.TryParse(Command,true,out command))
		{
			var pattern	= command.GetEnumAttribute<TCommand,CommandPatternAttribute>().Pattern;
			var groups	= pattern.Match(_parameters).Groups;
			foreach (string groupname in pattern.GetGroupNames())
			{
				parameters.Add(groupname,groups[groupname].Value);
			}
			return true;
		}
		return false;
	}
}

