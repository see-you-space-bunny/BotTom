using Xunit.Abstractions;
using RoleplayingGame;
using RoleplayingGame.Enums;
using RoleplayingGame.Objects;

namespace Widget.Tests.Theories;

public class @RoleplayingGame(ITestOutputHelper output)
{
	private readonly ITestOutputHelper _output = output;
	
	[Theory]
	[InlineData("Adventurer"	,100	,1906	,1885	,1885	,150	)]
	[InlineData("Adventurer"	,100	,1294	,1280	,1280	,0		)]
	public void TestResourceScaling(string className,int levels,int health,int protection,int evasion,int abilityAdjustment)
	{
		CharacterSheet character = new("Testo Telesto",0uL);
		character
			.ChangeClass(Enum.Parse<ClassName>(className))
			.LevelUp(levels,EnvironmentSource.World)
			.AdjustAllAbilities(abilityAdjustment,EnvironmentSource.World,true)
			.FullRecovery();

		Assert.Equal(health,		character.Health.Current);
		Assert.Equal(protection,	character.Protection.Current);
		Assert.Equal(evasion,		character.Evasion.Current);
	}
	
	[Theory]
	[InlineData("Adventurer"	,AttackType.Basic	,100	,0		,1035	,1048	,1048	,150)]
	[InlineData("Adventurer"	,AttackType.Basic	,100	,0		,703	,711	,711	,0	)]
	public void TestAttackScaling(string className,AttackType attackType,int levels,int levelgap,int accuracy,int impact,int harm,int abilityAdjustment)
	{
		CharacterSheet attacker = new("Alecto Attacko",0uL);
		attacker
			.ChangeClass(Enum.Parse<ClassName>(className))
			.LevelUp(levels,EnvironmentSource.World)
			.AdjustAllAbilities(abilityAdjustment,EnvironmentSource.World,true)
			.FullRecovery();

		CharacterSheet defender = new("Domino Defendo",1uL);
		defender
			.ChangeClass(Enum.Parse<ClassName>(className))
			.LevelUp(levels-levelgap,EnvironmentSource.World)
			.AdjustAllAbilities(abilityAdjustment,EnvironmentSource.World,true)
			.FullRecovery();

		var attack = FRoleplayMC
			.AttackPool[attackType]
			.BuildAttack(defender,attacker);

		Assert.Equal(accuracy,	(int)attack.Accuracy);
		Assert.Equal(impact,	(int)attack.Impact);
		Assert.Equal(harm,		(int)attack.Harm);

	}
}