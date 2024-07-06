using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Widget.LevelGame;
using Widget.LevelGame.Enums;

namespace Widget.Tests;

public class @LevelGame
{
    [Fact]
    public void TestHumanXmlDeserialization()
    {
        string filePath = Path.Combine(Environment.CurrentDirectory,"xml","CharacterClasses.xml");
        var characterClassInfo = HumanXmlDeserializer.GetClasses(filePath);
        Assert.NotEmpty(characterClassInfo);
    }
}