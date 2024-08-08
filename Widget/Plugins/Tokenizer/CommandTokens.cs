using System.Text.RegularExpressions;
using FChatApi.Attributes;
using FChatApi.Objects;
using Plugins.Attributes;

namespace Plugins.Tokenizer;

public class CommandTokens
{
#region (-) Fields
	private readonly string _parameters;
	private readonly string _command;
	private readonly FChatMessage _source;
#endregion


#region (+) Properties
	public FChatMessage Source => _source; 
	public string Command => _command;
	public Dictionary<string,string> Parameters { get; private set; }
#endregion


#region (+) Methods
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
#endregion


#region (#) Methods
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
#endregion


#region Constructor
	public CommandTokens(FChatMessage fChatMessage, string command, string parameters)
	{
		_source		=	fChatMessage;
		_command	=	command;
		_parameters	=	parameters;
		Parameters	=	new (StringComparer.InvariantCultureIgnoreCase);
	}
#endregion
}

