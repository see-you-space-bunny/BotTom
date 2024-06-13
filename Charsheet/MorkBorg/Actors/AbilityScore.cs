using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Charsheet.MorkBorg.Enum;

namespace Charsheet.MorkBorg.Actors;

internal readonly struct AbilityScore(AbilityScores abilityScore, int value)
{
  #region F[-]
  private readonly AbilityScores _info = abilityScore;
  private readonly int _value = value;
  #endregion

  #region P[~]
  internal readonly AbilityScores EnumReference => _info;
  internal readonly string Name => _info.ToString();
  internal readonly int Value => _value;
  #endregion

  #region Operator ++
  public static AbilityScore operator ++(AbilityScore abilityScore) =>
    new (abilityScore.EnumReference,abilityScore.Value+1);
  #endregion

  #region Operator --
  public static AbilityScore operator --(AbilityScore abilityScore) =>
    new (abilityScore.EnumReference,abilityScore.Value+1);
  #endregion

  #region Operator +
  public static AbilityScore operator +(AbilityScore abilityScore,int value) =>
    new (abilityScore.EnumReference,abilityScore.Value+value);
  public static AbilityScore operator +(int value,AbilityScore abilityScore) =>
    new (abilityScore.EnumReference,abilityScore.Value+value);
  #endregion

  #region Operator -
  public static AbilityScore operator -(AbilityScore abilityScore,int value) =>
    new (abilityScore.EnumReference,abilityScore.Value-value);
  public static AbilityScore operator -(int value,AbilityScore abilityScore) =>
    new (abilityScore.EnumReference,value-abilityScore.Value);
  #endregion

  #region Operator *
  public static AbilityScore operator *(AbilityScore abilityScore,int value) =>
    new (abilityScore.EnumReference,abilityScore.Value*value);
  public static AbilityScore operator *(int value,AbilityScore abilityScore) =>
    new (abilityScore.EnumReference,abilityScore.Value*value);
  #endregion

  #region Operator /
  public static AbilityScore operator /(AbilityScore abilityScore,int value) =>
    new (abilityScore.EnumReference,abilityScore.Value/value);
  public static AbilityScore operator /(int value,AbilityScore abilityScore) =>
    new (abilityScore.EnumReference,value/abilityScore.Value);
  #endregion

  #region Override
  public override string ToString()
  {
      return $"{Name} {(_value<0?string.Empty:'+')}{_value}";
  }
  #endregion
}
