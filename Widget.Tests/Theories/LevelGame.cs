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
	[InlineData("Adventurer","Testo Telesto",100)]
	[InlineData("Merchant","Besto Balesto",50)]
	public void TestNewCharacter(string className,string characterName,int healthPoints)
	{
		World.LoadClasses("CharacterClasses.xml");
		CharacterSheet character = new(0uL,characterName);
		character.ChangeClass(Enum.Parse<ClassName>(className)).LevelUp().FullRecovery();
		Assert.Equal<int>(healthPoints,character.Health.Current);
		Assert.Equal<int>(healthPoints,character.Protection.Current);
		Assert.Equal<int>(healthPoints,character.Evasion.Current);
	}
}