using RoleplayingGame.Enums;
using RoleplayingGame.SheetComponents;

namespace RoleplayingGame.Systems;

internal class DieRoller
{
#region Constants
	private const int StandardDie	=	100;
#endregion


#region (-) Static Fields
	private readonly Random Rng;
#endregion


#region (~) RollDie
	private int RollDie(int value) => Rng.Next(1,value+1);
#endregion


#region (~) StandardRoll
	internal DieRoll StandardRoll()	=>
		new ([new Tuple<int,int>(StandardDie,RollDie(StandardDie))]);
	internal DieRoll StandardRoll(AbilityScore[] abilities)	=>
		new ([new Tuple<int,int>(StandardDie,RollDie(StandardDie))],[.. abilities.Select(a=>new Tuple<Ability,int>(a.Key,a.GetDisplayValue()/10))]);
#endregion


#region Constructor
	internal DieRoller()
	{
		Rng = new Random();
	}

	static DieRoller()
	{ }
#endregion
}