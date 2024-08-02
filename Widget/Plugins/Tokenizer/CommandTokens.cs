using System.Text.RegularExpressions;
using FChatApi.Attributes;
using FChatApi.Objects;
using Plugins.Attributes;

namespace Plugins.Tokenizer;

public class CommandTokens(FChatMessage fChatMessage, string command, string parameters)
{
	public FChatMessage Source { get; }		= fChatMessage;
	public string Command { get; }			= command;
	private readonly string _parameters		= parameters;
	public Dictionary<string,string> Parameters { get; private set; } = new (StringComparer.InvariantCultureIgnoreCase);
	public bool TryGetParameters<TCommand>(out TCommand command) where TCommand : struct, Enum
	{
		Parameters.Clear();
		if (Enum.TryParse(Command,true,out command) || TryParseByAlias(Command,out command))
		{
			if (!command.HasEnumAttribute<TCommand,CommandPatternAttribute>())
				return true;
			var pattern	= command.GetEnumAttribute<TCommand,CommandPatternAttribute>().Pattern;
			var groups	= pattern.Match(_parameters).Groups;
			foreach (string groupname in pattern.GetGroupNames())
			{
				Parameters.Add(groupname,groups[groupname].Value);
			}
			return true;
		}
		return false;
	}

	protected static bool TryParseByAlias<TCommand>(string value, out TCommand result) where TCommand : struct, Enum =>
		AssembleAliasReference<TCommand>().TryGetValue(value,out result);

	protected static Dictionary<string,TCommand> AssembleAliasReference<TCommand>() where TCommand : struct, Enum
	{
		Dictionary<string,TCommand> result = new (StringComparer.InvariantCultureIgnoreCase);
		foreach (TCommand command in Enum.GetValues<TCommand>().Cast<TCommand>())
		{
			if (command.HasEnumAttribute<TCommand,CommandAliasAttribute>())
			{
				foreach (string alias in command.GetEnumAttribute<TCommand,CommandAliasAttribute>().Values)
				{
					result.AddOrUpdate(alias,command);
				}
			}
		}
		return result;
	}
}

