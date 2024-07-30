using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FChatApi.Objects;
using Plugins.Tokenizer;

namespace RoleplayingGame;

public partial class FRoleplayMC
{
	public override void HandleRecievedMessage(CommandTokens commandTokens)
	{
		FChatMessageBuilder messageBuilder = new FChatMessageBuilder();
#if DEBUG
		Task.Run(()=>Console.WriteLine(""));
#endif
		FChatApi.EnqueueMessage(messageBuilder);
	}
}