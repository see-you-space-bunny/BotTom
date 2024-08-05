using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RoleplayingGame.Enums;
using RoleplayingGame.Objects;

namespace RoleplayingGame.Effects;

public class EnemyChassis
{
#region (-) Fields
	readonly EnemyGroup		_enemyGroup;
	readonly EncounterZone	_encounterZone;
#endregion


#region (-) Fields
	string	_name;
	int		_level;
#endregion


#region (+) Properties
	public EnemyGroup Group		=> _enemyGroup;
	public EncounterZone Zone	=> _encounterZone;
#endregion


#region (+) With
	public EnemyChassis WithLevel(int value)
	{
		_level = value;
		return this;
	}
#endregion


#region (+) Build
	public NonPlayerEnemy Build()
	{
		return new NonPlayerEnemy(_name);
	}
#endregion


#region Constructor
	internal EnemyChassis(string name,EnemyGroup group,EncounterZone zone)
	{
		_name			= name;
		_enemyGroup		= group;
		_encounterZone	= zone;
	}
#endregion
}