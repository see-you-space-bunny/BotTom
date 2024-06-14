using System.Xml;
using System.Xml.Serialization;

namespace Charsheet.MorkBorg;

internal static class DefaultValues
{
  static DefaultValues() { }
  
  internal const int CurrentVersion = 0;
  internal const string ScumName = "Nameless Scum";
  internal const int OmensMaximum = 2;
  internal const int OmensCurrent = 0;
  internal const int HitPointsMaximum = 1;
  internal const int HitPointsCurrent = 1;
  internal const int AbilityScore = -3;
}