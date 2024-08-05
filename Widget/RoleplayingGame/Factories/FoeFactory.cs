using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RoleplayingGame.Effects;
using RoleplayingGame.Enums;
using RoleplayingGame.Objects;

namespace RoleplayingGame.Factories;

public class FoeFactory
{
#region (-) Fields
	private readonly List<EnemyChassis> _preDefinedEnemies;
#endregion

#region (+) CreateNonPlayerEnemy
	public NonPlayerEnemy CreateNonPlayerEnemy(EncounterZone encounterZone,int level) =>
		SelectRandomEnemyBuilder(encounterZone).WithLevel(level).Build();
	
	public NonPlayerEnemy CreateNonPlayerEnemy(EnemyGroup enemyGroup,int level) =>
		(enemyGroup switch
			{
				EnemyGroup.Random	=>	SelectRandomEnemyBuilder(),
				_					=>	SelectRandomEnemyBuilder(enemyGroup),
			}
		).WithLevel(level).Build();

	public NonPlayerEnemy CreateNonPlayerEnemy(string name,int level,EnemyChassis enemyBuilder = null!)
	{
		enemyBuilder ??= new EnemyChassis(name,EnemyGroup.Custom,EncounterZone.None);

		return enemyBuilder.WithLevel(level).Build();
	}
#endregion


#region (-) SelectRandomEnemyBuilder
	private EnemyChassis SelectRandomEnemyBuilder() =>
		_preDefinedEnemies
			.Skip(FRoleplayMC.Rng.Next(0, _preDefinedEnemies.Count))
			.First();

	private EnemyChassis SelectRandomEnemyBuilder(EnemyGroup value)
	{
		var builderGroup = _preDefinedEnemies.Where(eb => eb.Group == value);
		return builderGroup
			.Skip(FRoleplayMC.Rng.Next(0,builderGroup.Count()))
			.First();
	}

	private EnemyChassis SelectRandomEnemyBuilder(EncounterZone value)
	{
		var builderGroup = _preDefinedEnemies.Where(eb=>eb.Zone == value);
		return builderGroup
			.Skip(FRoleplayMC.Rng.Next(0,builderGroup.Count()))
			.First();
	}
#endregion


#region (+) AddChassis
	public void AddChassis(EnemyChassis value)
	{
		_preDefinedEnemies.Add(value);
	}
#endregion

	internal FoeFactory()
	{
		_preDefinedEnemies = [];
	}
}