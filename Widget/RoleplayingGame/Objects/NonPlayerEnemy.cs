using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RoleplayingGame.Enums;

namespace RoleplayingGame.Objects;

public class NonPlayerEnemy : Actor
{
	public EnvironmentSource SourceType = EnvironmentSource.Foe;

	public NonPlayerEnemy(string name) : base(name)
	{ }
}