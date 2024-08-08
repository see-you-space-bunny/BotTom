using System.ComponentModel;
using RoleplayingGame.Attributes;

namespace RoleplayingGame.Enums;

public enum SpecificItem
{
	Custom,

	[Description("")]
	[SpecificItemCategory(ItemCategory.Consumable)]
	ExplorationSupplies,

	
}