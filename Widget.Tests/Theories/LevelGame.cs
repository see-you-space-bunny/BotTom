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
	[InlineData("Adventurer"	,10		,204	,203	,203	,0	)]
	[InlineData("Merchant"		,10		,102	,101	,103	,0	)] // Evasion off by 1: (actual 102)
	[InlineData("Nobody"		,100	,110	,10		,10		,0	)]
	[InlineData("Adventurer"	,100	,1153	,1149	,1151	,0	)]
	[InlineData("Merchant"		,100	,579	,579	,586	,0	)]
	[InlineData("Nobody"		,200	,210	,10		,10		,0	)]
	[InlineData("Adventurer"	,200	,2208	,2200	,2202	,0	)]
	[InlineData("Merchant"		,200	,1109	,1110	,1123	,0	)]
	[InlineData("Nobody"		,100	,159	,58		,59		,50	)] // Health off by N: (actual 110)
	[InlineData("Adventurer"	,100	,1203	,1199	,1201	,50	)]
	[InlineData("Merchant"		,100	,629	,629	,636	,50	)]
	public void TestResourceScaling(string className,int levels,int health,int protection,int evasion,int abilityAdjustment)
	{
		World.LoadClasses("CharacterClasses - Export.csv");
		CharacterSheet character = new(0uL,"Testo Telesto");
		character.ChangeClass(Enum.Parse<ClassName>(className)).LevelUp(levels).FullRecovery();

		if (abilityAdjustment > 0)
		{
			foreach (Ability ability in Enum.GetValues<Ability>().Cast<Ability>().Where(a=>a!=Ability.None&&a!=Ability.Level))
				character.AdjustAbility(ability,abilityAdjustment);
		}

		Assert.Equal(health,		character.Health.Current);
		Assert.Equal(protection,	character.Protection.Current);
		Assert.Equal(evasion,		character.Evasion.Current);
	}
}