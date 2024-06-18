using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BotTom.DiceRoller;
using BotTom.SessionManager;
using Charsheet.MorkBorg;
using Xunit;

namespace TomDungeon;

public class UnitTestCommands
{
    string classesDirName = Path.Combine(Environment.CurrentDirectory, "data", "morkborg", "classes");

    Scum? Scum;
    SessionLibrary? SessionLibrary;

    //string InteractionContextUserUsername = "Username";
    ulong InteractionContextUserId = 79875985986786;

    [Fact]
    public void TestCreateNewScum()
    {
        string mbClassFilePath = Directory.GetFiles(classesDirName).Where((s)=>!s.Contains("blank-")).ToArray()[0];
        Scum = Scum.NewFromSettings(HumanXmlDeserializer.GetClassSettings(mbClassFilePath));
    }

    [Fact]
    public void TestSaveNewScum()
    {
        TestCreateNewScum();
        var guid = System.Guid.NewGuid().ToString();
        SessionLibrary = new SessionLibrary();
        SessionLibrary.RegisterAndSave(Scum!,InteractionContextUserId,guid,Scum!.Name);
    }

    [Fact]
    public void TestMakeRollFromActive()
    {
        TestSaveNewScum();
        var currentUser = SessionLibrary!.UserInfo[InteractionContextUserId];
        var mbScumModule = currentUser.ModuleInfo[ValidSerialTypes.Scum];
        var slotInfo = currentUser.Index[mbScumModule.ActiveSaveFile];
        Scum loadScum = (Scum)slotInfo.Object;

        int rollResult = DiceParser.BasicRoll($"1d20{(loadScum.AbilityScores[AbilityScores.Agility].Value>=0?'+':string.Empty)}{loadScum.AbilityScores[AbilityScores.Agility].Value}").Item2;
        Assert.InRange(rollResult,-5,30);
    }

    [Theory]
    [InlineData("agi","Text")]
    [InlineData("pres","Text")]
    [InlineData("Toughness","Test")]
    [InlineData("Toughness","Text")]
    [InlineData("STR","Text")]
    [InlineData("dEX","Text")]
    public void TestMBRollFromCommand(string abilityScore, string charName)
    {
        TestSaveNewScum();
        int badScore = -100;
        
        var currentUser = SessionLibrary!.UserInfo[InteractionContextUserId];
        var mbScumModule = currentUser.ModuleInfo[ValidSerialTypes.Scum];

        /** "/character system: mb charname: test action: attack stat: agi target: 12" */

        Scum loadScum;
        try{
            var slotInfo = currentUser.Index.First((t)=>t.Value.SearchableName.Contains(charName, StringComparison.CurrentCultureIgnoreCase)).Value;
            loadScum = (Scum)slotInfo.Object;
        }
        catch (Exception e)
        {
            loadScum = (Scum)currentUser.Index[mbScumModule.ActiveSaveFile].Object;
        }
        
        int score = badScore;
        var abilityScores = Enum.GetValues(typeof(AbilityScores)).GetEnumerator();
        while(abilityScores.MoveNext())
        {
            if(abilityScores.Current.ToString()!.Contains(abilityScore, StringComparison.CurrentCultureIgnoreCase))
            {
                score = loadScum.AbilityScores[(AbilityScores)abilityScores.Current].Value;
                break;
            }
        }
        if(score == badScore && abilityScore.Equals("dex", StringComparison.CurrentCultureIgnoreCase))
        { }
        else
        {
            int rollResult = 10+score;
            Assert.InRange(rollResult,7,16);
        }
    }
}