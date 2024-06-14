using Xunit.Abstractions;
using Xunit.Sdk;
using System.Text.RegularExpressions;
using BotTom.ProcGen.Scumify;

namespace BotTom;

public class UnitTestMorkBorg(ITestOutputHelper output)
{
  private const string ExpectedRegexPattern_1 = @"\#\ [\w]+";
  private const string ExpectedRegexPattern_2 = @"\>\ \*By\ \[\w+\]\([\w\+\.\:\/]+\)\*";
  private const string ExpectedRegexPattern_3 = @"\nBackground\:\ [\w\d]+";
  private const string ExpectedRegexPattern_4 = @"Strength\ [\+\-0-6]{2}\,\ Agility\ [\+\-0-6]{2}\,\ Presence\ [\+\-0-6]{2}\,\ Toughness\ [\+\-0-6]{2}";
  private const string ExpectedRegexPattern_5 = @"[0-9]{1}\ Omen[s]{0,1}\ \(d[0-9]{1}\)\,\ [0-9]+ Silver";
  private const string ExpectedRegexPattern_6 = @"Weapon\ [0-9]+\,\ Armor\ [0-9]+[\n\w]*";
  private readonly ITestOutputHelper _output = output;

  private readonly string[] _BackgroundResults = [
    "TextTextText1",
    "TextTextText2",
    "TextTextText3",
    "TextTextText4",
    "TextTextText5",
    "TextTextText6"
  ];

  private readonly string[] _SpecialResults = [
    "TextTextTextS1",
    "TextTextTextS2",
    "TextTextTextS3",
    "TextTextTextS4",
    "TextTextTextS5",
    "TextTextTextS6"
  ];

  private readonly string[] _NameResults = [
    "Cursed Skinwalker",
    "TextTextTextName"
  ];

  [Fact]
  public void TestClassName()
  {
    var scvm = new MakeSCVM().Random();
    _output.WriteLine(scvm.Name);
    Assert.Contains(scvm.Name,_NameResults);
  }

  [Fact]
  public void TestRandomAttributes()
  {
    var scvm = new MakeSCVM().Random();
    _output.WriteLine( "A {0}, P {1}, S {2}, T {3}",
      scvm.Abilities.Agility.ToString(),
      scvm.Abilities.Presence.ToString(),
      scvm.Abilities.Strength.ToString(),
      scvm.Abilities.Toughness.ToString()
    );
    Assert.InRange(scvm.Abilities.Agility, -3, 6);
    Assert.InRange(scvm.Abilities.Presence, -3, 6);
    Assert.InRange(scvm.Abilities.Strength, -3, 6);
    Assert.InRange(scvm.Abilities.Toughness, -3, 6);
  }

  [Fact]
  public void TestRandomEquipmentIntegers()
  {
    var scvm = new MakeSCVM().Random();
    _output.WriteLine( "W {0}, A {1}, S {2}",
      scvm.Equipment.Weapon.ToString(),
      scvm.Equipment.Armor.ToString(),
      scvm.Equipment.Silver.ToString()
    );
    Assert.InRange(scvm.Equipment.Weapon, 1, 6);
    Assert.InRange(scvm.Equipment.Armor, 1, 2);
    Assert.InRange(scvm.Equipment.Silver, 20, 120);
  }

  [Fact]
  public void TestRandomStartingOmens()
  {
    var scvm = new MakeSCVM().Random();
    _output.WriteLine( "O {0} (d{1})",
      scvm.Omens.Roll.ToString(),
      scvm.Omens.Die.ToString()
    );
    Assert.InRange(scvm.Omens.Roll, 1, 2);
    Assert.Equal(2, scvm.Omens.Die);
  }

  [Fact]
  public void TestRandomBackground()
  {
    var scvm = new MakeSCVM().Random();
    _output.WriteLine(scvm.Background);
    Assert.Contains(scvm.Background,_BackgroundResults);
  }

  [Fact]
  public void TestRandomSpecial()
  {
    var scvm = new MakeSCVM().Random();
    _output.WriteLine("HasSpecial ? {0}",scvm.HasSpecial);
    Assert.True(scvm.HasSpecial);
    _output.WriteLine("Special of {0}: {1}", scvm.Name, scvm.Special);
    Assert.Contains(scvm.Special,_SpecialResults);
  }

  [Fact]
  public void TestStringOutput()
  {
    var scvm = new MakeSCVM();
    string scvmString = scvm.Random().ToString();
    _output.WriteLine(scvmString);
    Assert.Matches(ExpectedRegexPattern_1, scvmString);
    Assert.Matches(ExpectedRegexPattern_2, scvmString);
    Assert.Matches(ExpectedRegexPattern_3, scvmString);
    Assert.Matches(ExpectedRegexPattern_4, scvmString);
    Assert.Matches(ExpectedRegexPattern_5, scvmString);
    Assert.Matches(ExpectedRegexPattern_6, scvmString);
    //Assert.Fail(scvmString);
  }
}