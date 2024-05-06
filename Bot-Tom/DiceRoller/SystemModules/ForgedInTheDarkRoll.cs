<<<<<<< HEAD
using System.Text;
=======
>>>>>>> 0f5439c7388993abed3552eea7789a3a2aaa63a5

namespace BotTom;

/// <summary>
/// Rolling dice for Forged In The Dark game systems.
/// </summary>
/// <param name="dicePool"></param>
/// <param name="label"></param>
class ForgedInTheDarkRoll(long dicePool, string? label)
{
  long DicePool { get; } = dicePool;
  string? Label { get; } = label;


  public override string ToString()
  {
      StringBuilder sb = new();
      if(Label is not null)
          sb.AppendLine($"> {Label.Replace("\\n","\n> ")}");
<<<<<<< HEAD

=======
          
>>>>>>> 0f5439c7388993abed3552eea7789a3a2aaa63a5
      return sb.ToString();
  }
}