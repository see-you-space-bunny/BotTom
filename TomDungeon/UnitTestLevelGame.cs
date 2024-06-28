using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Charsheet.LevelGame;
using Charsheet.LevelGame.Enums;

namespace TomDungeon;

public class UnitTestLevelGame
{
    [Fact]
    public void TestHumanXmlDeserialization()
    {
        string filePath = Path.Combine(Environment.CurrentDirectory,"xml","CharacterClasses.xml");
        var characterClassInfo = HumanXmlDeserializer.GetClasses(filePath);
        Assert.NotEmpty(characterClassInfo);
    }

    
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