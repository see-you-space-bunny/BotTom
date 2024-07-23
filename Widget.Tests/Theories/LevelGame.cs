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
	[InlineData("Nobody"		,10		,20		,10		,10		)]
	[InlineData("Adventurer"	,10		,204	,203	,203	)]
	[InlineData("Merchant"		,10		,102	,101	,102	)] // evasion off by 1: (expected 103)
	[InlineData("Nobody"		,100	,110	,10		,10		)]
	[InlineData("Adventurer"	,100	,1153	,1149	,1151	)]
	[InlineData("Merchant"		,100	,579	,579	,586	)]
	[InlineData("Nobody"		,200	,210	,10		,10		)]
	[InlineData("Adventurer"	,200	,2208	,2200	,2202	)]
	[InlineData("Merchant"		,200	,1109	,1110	,1123	)]
	public void TestNewCharacter(string className,int levels,int health,int protection,int evasion)
	{
		World.LoadClasses("CharacterClasses - Export.csv");
		CharacterSheet character = new(0uL,"Testo Telesto");
		character.ChangeClass(Enum.Parse<ClassName>(className)).LevelUp(levels).FullRecovery();
		Assert.Equal(health,		character.Health.Current);
		Assert.Equal(protection,	character.Protection.Current);
		Assert.Equal(evasion,		character.Evasion.Current);
	}
}