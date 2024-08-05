using System.ComponentModel;
using RoleplayingGame.Attributes;

namespace RoleplayingGame.Enums;

public enum Echelon
{
	[Description("none. This text should never be displayed.")]
	[EchelonProperties(tier:-1,signatureAction:GameAction.None)]
	None		= 0x00,
	
	[Description("ordinary people, without special powers or abilities.")]
	[EchelonProperties(tier:0,signatureAction:GameAction.BringLow)]
	Nobody		= 0x10,
	
	[Description("those more capable than your average person, but still at the bottom of their respective hierarchy.")]
	[EchelonProperties(tier:1,signatureAction:GameAction.BringLow)]
	Minion		= 0x20,

	[Description("adventurers and glory seekers. Those that aspire to greatness and have proven themselves a cut above.")]
	[EchelonProperties(tier:2,signatureAction:GameAction.LimitBreak)]
	Aspirant	= 0x30,
	
	[Description("powerful magical (and mundane) creatures that would challenge an entire party of adventurers.")]
	[EchelonProperties(tier:3,signatureAction:GameAction.BadEnd)]
	Monster		= 0x40,
	
	[Description("minor gods and greater spirits who embody their domain, but which are still small or narrow.")]
	[EchelonProperties(tier:4,signatureAction:GameAction.BadEnd)]
	Divinity	= 0x50,

	[Description("mortal legends and demigods.")]
	[EchelonProperties(tier:4,signatureAction:GameAction.LimitBreak)]
	Legend		= 0x60,
	
	[Description("powerful creatures 'not of this world', that fracture mortal minds by mere witness of their true form.")]
	[EchelonProperties(tier:5,signatureAction:GameAction.BadEnd)]
	Horror		= 0x70,
	
	[Description("the highest embodiments of powers beyond mortal beinds that are themselves not gods.")]
	[EchelonProperties(tier:6,signatureAction:GameAction.BringLow)]
	Herald		= 0x80,
	
	[Description("literal gods, whose domain encompass worlds, concepts or even entire realities.")]
	[EchelonProperties(tier:7,signatureAction:GameAction.BringLow)]
	Deity		= 0x90,

}