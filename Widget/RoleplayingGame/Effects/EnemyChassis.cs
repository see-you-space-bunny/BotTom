using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RoleplayingGame.Enums;
using RoleplayingGame.Objects;
using RoleplayingGame.SheetComponents;

namespace RoleplayingGame.Effects;

public class EnemyChassis
{
#region (-) Fields
	readonly string	_name;
	readonly EnemyGroup		_enemyGroup;
	readonly EncounterZone	_encounterZone;
	readonly CharacterClass	_class;
#endregion


#region (-) Fields
	int		_level;
#endregion


#region (+) Properties
	public EnemyGroup		Group	=> _enemyGroup;
	public EncounterZone	Zone	=> _encounterZone;
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
		NonPlayerEnemy result	=	new (_name);
		result
			.ChangeClass(_class)
			.LevelUp(_level,EnvironmentSource.World)
			.FullRecovery();
		return result;
	}
#endregion


#region Constructor
	internal EnemyChassis(string name,CharacterClass @class,EnemyGroup group,EncounterZone zone)
	{
		_name			= name;
		_enemyGroup		= group;
		_encounterZone	= zone;
		_class			= @class;
	}
#endregion
}