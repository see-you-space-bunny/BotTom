using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using Widget.LevelGame;
using Widget.LevelGame.Enums;

namespace Widget.Tests.Theories;

public class @LevelGame(ITestOutputHelper output)
{
  private readonly ITestOutputHelper _output = output;
  
    [Theory]
    [InlineData("Adventurer","Testo Telesto",180)]
    public void TestNewCharacter(string className,string characterName,int healthPoints)
    {
        string filePath = Path.Combine(Environment.CurrentDirectory,"xml","CharacterClasses.xml");
        var characterClassInfo = HumanXmlDeserializer.GetClasses(filePath).Where((cci)=>cci.TextName==className);
        CharacterSheet character = new(0uL,characterName);
        character.ChangeClass(ClassName.Adventurer);
        character.LevelUp();
        Assert.Equal<int>(healthPoints,character.HealthPoints.Current);
    }
}