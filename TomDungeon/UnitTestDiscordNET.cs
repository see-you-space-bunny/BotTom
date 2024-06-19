using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using BotTom.DiceRoller;
using BotTom.DiceRoller.GameSystems;
using BotTom.Commands;

namespace TomDungeon;

public class UnitTestDiscordNET
{
    [Fact]
    public void TestCommandOption()
    {
        CommandOption<long> TargetNumber = new ("dice","Your aptitude plus discipline.",null,isRequired: true);
    }
}