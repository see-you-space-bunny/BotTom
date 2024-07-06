using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Widget.CardGame;
using Widget.CardGame.Attributes;
using Widget.CardGame.Enums;
using Engine.ModuleHost.Attributes;

namespace Widget.Tests.Facts;

public class @CardGame
{

    [Fact]
    public void TestAttributeHandler()
    {
        StatAliasAttribute statAliasLevel = CharacterStat.LVL.GetAttribute<CharacterStat, StatAliasAttribute>();
        Assert.Equal(["Level","Lv"],statAliasLevel.Alias);

        StatAliasAttribute statAliasLuck = CharacterStat.LUC.GetEnumAttribute<CharacterStat, StatAliasAttribute>();
        Assert.Equal(["Luck"],statAliasLuck.Alias);
    }
}