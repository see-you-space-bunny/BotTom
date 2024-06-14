using System.Text;

namespace BotTom.DiceRoller.GameSystems;

/// <summary>
/// Rolling dice for Forged In The Dark game systems.
/// </summary>
/// <param name="dicePool"></param>
/// <param name="label"></param>
public class ForgedInTheDarkRoll(long dicePool, string? label)
{
  long DicePool { get; } = dicePool;
  string? Label { get; } = label;


  public override string ToString()
  {
      StringBuilder sb = new();
      if(Label is not null)
          sb.AppendLine($"> {Label.Replace("\\n","\n> ")}");
          
      return sb.ToString();
  }
}