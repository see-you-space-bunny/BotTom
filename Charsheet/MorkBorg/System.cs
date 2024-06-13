
using System.Xml;
using System.Xml.Serialization;
using Charsheet.MorkBorg.Actors;

namespace Charsheet.MorkBorg.System;

public static class MorkBorg
{
  static MorkBorg() { }
  
  public const int CurrentVersion = 0;
  public const int MaxAbilityScores = 69;
  public const string DefaultName = "Nameless Scum";
  public const int DefaultOmensMaximum = 2;
  public const int DefaultOmensCurrent = 0;
  public const int DefaultHitPointsMaximum = 1;
  public const int DefaultHitPointsCurrent = 1;
  public const int DefaultAbilityScore = -3;
}