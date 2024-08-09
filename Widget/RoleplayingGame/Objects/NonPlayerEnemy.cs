using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RoleplayingGame.Enums;

namespace RoleplayingGame.Objects;

public class NonPlayerEnemy : Actor
{
#region (+P)
	public EnvironmentSource SourceType	{ get; }
#endregion


#region Constructor
	public NonPlayerEnemy(string name) : base(name)
	{
		SourceType	=	EnvironmentSource.Foe;
	}
#endregion
}