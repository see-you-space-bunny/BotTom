using Xunit.Abstractions;
using FChatApi.Enums;
using FChatApi.Objects;
using RoleplayingGame.Effects;
using RoleplayingGame;
using RoleplayingGame.Enums;
using RoleplayingGame.Objects;
using Plugins.Tokenizer;
using RoleplayingGame.SheetComponents;

namespace Widget.Tests.Theories;

public class @RoleplayingGame(ITestOutputHelper output)
{
	private readonly ITestOutputHelper	_output			=	output;
	private static readonly FRoleplayMC	_fRoleplayMC;
	private static readonly User		_player1;
	private static readonly User		_player2;
	
	[Theory]
	[InlineData("Adventurer"	,100	,1906	,1885	,1885	,150	)]
	[InlineData("Adventurer"	,100	,1294	,1280	,1280	,0		)]
	public void TestResourceScaling(string className,int levels,int health,int protection,int evasion,int abilityAdjustment)
	{
////////////
		FRoleplayMC fRoleplayMC = new (null!,new TimeSpan(0,0,5));
		fRoleplayMC.Initialize();
////////////
		CharacterSheet character = new("Testo Telesto",0uL);
		character
			.ChangeClass(fRoleplayMC.CharacterClasses.All[Enum.Parse<ClassName>(className)])
			.LevelUp(levels,EnvironmentSource.World)
			.AdjustAllAbilities(abilityAdjustment,EnvironmentSource.World,true)
			.FullRecovery();
////////////
		Assert.Equal(health,		character.Health.Current);
		Assert.Equal(protection,	character.Protection.Current);
		Assert.Equal(evasion,		character.Evasion.Current);
////////////
	}
	
	[Theory]
	[InlineData("Adventurer"	,AttackType.Basic	,100	,0		,1035	,1048	,1048	,150)]
	[InlineData("Adventurer"	,AttackType.Basic	,100	,0		,703	,711	,711	,0	)]
	public void TestAttackScaling(string className,AttackType attackType,int levels,int levelgap,int accuracy,int impact,int harm,int abilityAdjustment)
	{
////////////
		FRoleplayMC fRoleplayMC = new (null!,new TimeSpan(0,0,5));
		fRoleplayMC.Initialize();
////////////
		CharacterSheet attacker = new("Alecto Attacko",0uL);
		attacker
			.ChangeClass(fRoleplayMC.CharacterClasses.All[Enum.Parse<ClassName>(className)])
			.LevelUp(levels,EnvironmentSource.World)
			.AdjustAllAbilities(abilityAdjustment,EnvironmentSource.World,true)
			.FullRecovery();
////////////
		CharacterSheet defender = new("Domino Defendo",1uL);
		defender
			.ChangeClass(fRoleplayMC.CharacterClasses.All[Enum.Parse<ClassName>(className)])
			.LevelUp(levels-levelgap,EnvironmentSource.World)
			.AdjustAllAbilities(abilityAdjustment,EnvironmentSource.World,true)
			.FullRecovery();
////////////
		var attack = FRoleplayMC
			.AttackPool[attackType]
			.BuildAttack(defender,attacker);
////////////
		Assert.Equal(accuracy,	(int)attack.Accuracy);
		Assert.Equal(impact,	(int)attack.Impact);
		Assert.Equal(harm,		(int)attack.Harm);
////////////
	}
	
	[Theory]
	[InlineData("?explore beginnertraining","?attack")]
	public void TestExploration(params string[] messages)
	{
////////////
		foreach (string message in messages)
		{
			CommandTokens commandTokens	=	ChatMessageAssistant.NewDummyMessage(_player1,message);
////////////
			_fRoleplayMC.FoeFactory.AddChassis(new EnemyChassis(
				"Goblin",
				_fRoleplayMC.CharacterClasses.All[ClassName.Nobody],
				EnemyGroup.Generic,
				EncounterZone.BeginnerTraining
			));
////////////
			_fRoleplayMC.Characters.SingleByUser(commandTokens.Source.Author).Inventory
				.Add(new InventoryItem(SpecificItem.ExplorationSupplies));
////////////
			try
			{
				_fRoleplayMC.HandleRecievedMessage(commandTokens);
			}
			catch (NullReferenceException)
			{ }
		}
////////////
	}

	static @RoleplayingGame()
	{
////////////
		const string	TestClass	=	"Adventurer";
		const int		TestLevel	=	100;
////////////
		_fRoleplayMC	=	new (null!,new TimeSpan(0,5,0));
		_fRoleplayMC.Initialize();
////////////
		_player1		=	new User(){ Name = "Testo Telesto"	, PrivilegeLevel = Privilege.RegisteredUser };
		_fRoleplayMC.Characters.TryCreateCharacter(_player1);
		_fRoleplayMC.Characters.SingleByUser(_player1)
			.ChangeClass(_fRoleplayMC.CharacterClasses.All[Enum.Parse<ClassName>(TestClass)])
			.LevelUp(TestLevel,EnvironmentSource.World)
			.FullRecovery();
////////////
		_player2		=	new User(){ Name = "Alecto Attacko"	, PrivilegeLevel = Privilege.RegisteredUser };
		_fRoleplayMC.Characters.TryCreateCharacter(_player2);
		_fRoleplayMC.Characters.SingleByUser(_player2)
			.ChangeClass(_fRoleplayMC.CharacterClasses.All[Enum.Parse<ClassName>(TestClass)])
			.LevelUp(TestLevel,EnvironmentSource.World)
			.FullRecovery();
////////////
	}
}