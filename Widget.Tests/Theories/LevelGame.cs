using Xunit.Abstractions;

using LevelGame;
using LevelGame.Enums;
using LevelGame.Objects;
using LevelGame.Serialization;
using System.Runtime.Serialization;
using System.Xml;
using LevelGame.Core;

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
		World.LoadClasses("CharacterClasses - Export.csv");
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
}