using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FChatApi.Core;
using FChatApi.Objects;
using RoleplayingGame.Interfaces;
using RoleplayingGame.Objects;

namespace RoleplayingGame.Contexts;

internal class CombatContext : BaseContext, IContext<CombatContext>
{
#region 
	
#endregion

#region IContext
	public new CombatContext WithParticipant(Actor value)
	{
		base.WithParticipant(value);
		return this;
	}

	public new CombatContext WithParticipants(params Actor[] values)
	{
		base.WithParticipants(values);
		return this;
	}

	public new CombatContext EnqueueMessage(ApiConnection api)
	{
		var message = new FChatMessageBuilder();
		
		api.EnqueueMessage(message);
		return this;
	}
#endregion

#region Constructor
	internal CombatContext() : base()
	{ }
#endregion
}