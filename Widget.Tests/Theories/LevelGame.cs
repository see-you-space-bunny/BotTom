using Xunit.Abstractions;
using LevelGame;
using LevelGame.Enums;
using LevelGame.Objects;

namespace Widget.Tests.Theories;

public class @LevelGame(ITestOutputHelper output)
{
	private readonly ITestOutputHelper _output = output;
	
	[Theory]
	[InlineData("Nobody"		,10		,20		,10		,10		,0	)]
	[InlineData("Adventurer"	,10		,204	,204	,204	,0	)]
	[InlineData("Merchant"		,10		,102	,102	,103	,0	)]
	[InlineData("Nobody"		,100	,110	,10		,10		,0	)]
	[InlineData("Adventurer"	,100	,1154	,1151	,1151	,0	)]
	[InlineData("Merchant"		,100	,580	,581	,587	,0	)]
	[InlineData("Nobody"		,200	,210	,10		,10		,0	)]
	[InlineData("Adventurer"	,200	,2209	,2202	,2202	,0	)]
	[InlineData("Merchant"		,200	,1110	,1112	,1124	,0	)]
	[InlineData("Nobody"		,100	,160	,60		,60		,50	)]
	[InlineData("Adventurer"	,100	,1204	,1201	,1201	,50	)]
	[InlineData("Merchant"		,100	,630	,631	,637	,50	)]
	public void TestResourceScaling(string className,int levels,int health,int protection,int evasion,int abilityAdjustment)
	{
		FRoleplayMC.LoadClasses("CharacterClasses - Export.csv");
		CharacterSheet character = new(0uL,"Testo Telesto");
		character
			.ChangeClass(Enum.Parse<ClassName>(className))
			.LevelUp(levels)
			.AdjustAllAbilities(abilityAdjustment,true)
			.FullRecovery();

		Assert.Equal(health,		character.Health.Current);
		Assert.Equal(protection,	character.Protection.Current);
		Assert.Equal(evasion,		character.Evasion.Current);
	}
	
	[Theory]
	[InlineData("Adventurer"	,AttackType.Basic	,100	,0		,757	,817	,817	,0	)]
	[InlineData("Adventurer"	,AttackType.Basic	,200	,100	,3030	,3270	,3270	,0	)]
	[InlineData("Adventurer"	,AttackType.Basic	,100	,-100	,378	,408	,408	,0	)]
	[InlineData("Adventurer"	,AttackType.Basic	,50		,-50	,262	,283	,283	,0	)]
	public void TestAttackScaling(string className,AttackType attackType,int levels,int levelgap,int accuracy,int impact,int harm,int abilityAdjustment)
	{
		FRoleplayMC.LoadClasses("CharacterClasses - Export.csv");

		CharacterSheet attacker = new(0uL,"Alecto Attacko");
		attacker
			.ChangeClass(Enum.Parse<ClassName>(className))
			.LevelUp(levels)
			.AdjustAllAbilities(abilityAdjustment,true)
			.FullRecovery();

		CharacterSheet defender = new(0uL,"Domino Defendo");
		defender
			.ChangeClass(Enum.Parse<ClassName>(className))
			.LevelUp(levels-levelgap)
			.AdjustAllAbilities(abilityAdjustment,true)
			.FullRecovery();

		var attack = FRoleplayMC
			.AttackPool[attackType]
			.BuildAttack(defender,attacker);

		Assert.Equal(accuracy,	(int)attack.Accuracy);
		Assert.Equal(impact,	(int)attack.Impact);
		Assert.Equal(harm,		(int)attack.Harm);

	}
}