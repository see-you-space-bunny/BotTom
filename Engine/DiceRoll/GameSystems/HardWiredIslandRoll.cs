using System.Text;

namespace Engine.DiceRoll.GameSystems;

/// <summary>
/// Rolling dice for the Hard Wired Island game system.
/// </summary>
/// <param name="diceModifier"></param>
/// <param name="difficultyRating"></param>
/// <param name="boost"></param>
/// <param name="label"></param>
class HardWiredIslandRoll(long diceModifier, long? difficultyRating, long? boost, string? label)
{
  long DiceModifier { get; } = diceModifier;
  long? DifficultyRating { get; } = difficultyRating;
  long? Boost { get; } = boost;
  string? Label { get; } = label;


  public override string ToString()
  {
    StringBuilder sb = new();
    if(Label is not null)
      sb.AppendLine($"> {Label.Replace("\\n","\n> ")}");

    return sb.ToString();
  }
}