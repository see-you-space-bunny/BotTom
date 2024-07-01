using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using SurlyCardGame;

namespace TomDungeon;

public class UnitTestLevelGame
{
    [Fact]
    public void Test()
    {
        Assert.True(true);
    }

    
    [Theory]
    [InlineData("Daniel","tom!xcg challenge The Cooler Daniel")]
    public void TestCommand(string characterIdentity,string commandString)
    {
        
    }
}