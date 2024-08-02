using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FChatApi.Attributes;
using FChatApi.Enums;
using RoleplayingGame.Attributes;
using RoleplayingGame.Enums;
using RoleplayingGame.SheetComponents;

namespace RoleplayingGame.Systems;

internal class DieRoller
{
#region (-) Constants
	private const string SymbolGameDie				=	"🎲";
	private const string FormatDieRoll				=	"{0}{1}";
	private const string FormatDieRollModified		=	"{0}{1}+{2}";
	private const string FormatBBCodeWithArg		=	"[{1}={2}]{0}[/{1}]";
	private const BBCodeColor ColorNegativeValue	=	BBCodeColor.black;
#endregion

#region (-) Static Fields
	private readonly Random Rng;
#endregion


#region (~) RollD100
	internal int RollD100() => Rng.Next(1,101);
#endregion


#region (~) StandardRoll
	internal (int Value,string Formatted) StandardRoll(AbilityScore[] abilities)
	{
		int value			=	RollD100();
		string formatted	=	string.Format(FormatDieRoll,value,SymbolGameDie);
		foreach (AbilityScore ability in abilities)
		{
			int rawMod		=	ability.GetDisplayValue()/10;
			string formMod	=	rawMod >= 0 ? rawMod.ToString() : string.Format(FormatBBCodeWithArg,Math.Abs(rawMod),"color",ColorNegativeValue);
			var deco		=	ability.Key.GetEnumAttribute<Ability,StatDecorationAttribute>();
			value			+=	rawMod;
			formatted		=	string.Format(FormatDieRollModified,formMod,deco?.Emoji ?? string.Empty,formatted);
		}
		return (value,formatted);
	}
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