using BotTom.DiceRoller;

namespace BotTom.ProcGen.Scumify;

public partial class SCVM
{
  private static (int Agility, int Presence, int Strength, int Toughness) RollAbilities(int agilityMod, int presenceMod, int strengthMod, int toughnessMod) => (
    AbilityScoreToModifier( RollAbility( agilityMod ) ),
    AbilityScoreToModifier( RollAbility( presenceMod ) ), 
    AbilityScoreToModifier( RollAbility( strengthMod ) ),
    AbilityScoreToModifier( RollAbility( toughnessMod ) )
  );

  private static int RollAbility( int abilityMod ) => DiceParser.BasicRoll("3d6").Item2 + abilityMod;

  private static int AbilityScoreToModifier( int abilityScore ) => Math.Max( ( abilityScore - 10 ) / 2, -3 );
}