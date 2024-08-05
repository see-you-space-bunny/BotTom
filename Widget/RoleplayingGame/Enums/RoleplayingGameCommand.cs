using System.ComponentModel;
using FChatApi.Enums;
using Plugins.Attributes;
using Plugins.Enums;

namespace RoleplayingGame.Enums;

//	Begin / End case-insensitive matching
//		(?i) / (?-i)

//	"Command"
//		\s+(?'Command'[a-zA-Z]{1,24})

//	"Word"
//		\s+(?'Word'[a-zA-Z\-_0-9]{1,24})

//	"WordArray"
//		\s+(?'WordArray'[a-zA-Z\-_0-9]{1,24}([\,\+]{1}\s*[a-zA-Z]{1,24})*)

//	"WordIntegerArray"
//		\s+(?'WordNumberArray'(?'Word'[a-zA-Z\-_0-9]{1,24})\s+(?'Number'[0-9]{1,7})\s*[\,\+]{0,1}\s*)+

//	"Integer"
//		\s+(?'Integer'[0-9]{1,7})

//	"Number"
//		\s+(?'Number'[0-9]{1,7}[\.]{0,1}[0-9]{0,4})

//	"Player"
//		\s+(\[user\]){0,1}(?'Player'[a-zA-Z\-_\s0-9]{1,32})(\[\/user\]){0,1}

//	Command End
//		[\s+\]$]

public enum RoleplayingGameCommand
{
	None,

	[Description("")]
	[CommandPattern(@"(?i)((?'Attack'[a-zA-Z\-_0-9]{1,24})\s+){0,1}(\[user\]){0,1}(?'Target'[a-zA-Z\-_\s0-9]{1,32})(\[\/user\]){0,1}")]
	[MinimumPrivilege(Privilege.RegisteredUser)]
	[UsageScope(CommandScope.Anywhere)]
	[CommandAlias("a")]
	Attack,

	[Description("")]
	[CommandPattern(@"(?i)(?'Class'[a-zA-Z\-_\s]{1,64})")]
	[MinimumPrivilege(Privilege.RegisteredUser)]
	[UsageScope(CommandScope.Anywhere)]
	[CommandAlias("cc")]
	ClassChange,

	[Description("")]
	[MinimumPrivilege(Privilege.RegisteredUser)]
	[UsageScope(CommandScope.Anywhere)]
	[CommandAlias("d")]
	Defend,

	[Description("")]
	[MinimumPrivilege(Privilege.RegisteredUser)]
	[UsageScope(CommandScope.Whisper)]
	[CommandAlias("e")]
	Explore,

	[Description("")]
	[CommandPattern(@"(?i)((\[user\]){0,1}(?'Target'[a-zA-Z\-_\s0-9]{1,32})(\[\/user\]){0,1}){0,1}")]
	[MinimumPrivilege(Privilege.RegisteredUser)]
	[UsageScope(CommandScope.Anywhere)]
	[CommandAlias("i")]
	Inspect,

	[Description("")]
	[MinimumPrivilege(Privilege.RegisteredUser)]
	[UsageScope(CommandScope.Anywhere)]
	[CommandAlias("s")]
	Status,

	[Description("")]
	[MinimumPrivilege(Privilege.RegisteredUser)]
	[UsageScope(CommandScope.Whisper)]
	[CommandAlias("f")]
	Flee,

	[Description("")]
	[MinimumPrivilege(Privilege.RegisteredUser)]
	[UsageScope(CommandScope.Anywhere)]
	[CommandAlias("u")]
	Use,
}