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
	[InlineData("Adventurer","Testo Telesto",110,105,105)]
	[InlineData("Merchant","Besto Balesto",55,50,50)]
	[InlineData("Nobody","Bronto Balonto",11,10,10)]
	public void TestNewCharacter(string className,string characterName,int health,int protection,int evasion)
	{
		World.LoadClasses("CharacterClasses.csv");
		CharacterSheet character = new(0uL,characterName);
		character.ChangeClass(Enum.Parse<ClassName>(className)).LevelUp().FullRecovery();
		Assert.InRange<int>(character.Health.Current,health,health*2);
		Assert.InRange<int>(character.Protection.Current,protection,protection*2);
		Assert.InRange<int>(character.Evasion.Current,evasion,evasion*2);
	}
}