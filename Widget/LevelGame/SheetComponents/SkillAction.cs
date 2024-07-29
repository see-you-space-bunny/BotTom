using RoleplayingGame.Attributes;
using RoleplayingGame.Enums;

namespace RoleplayingGame.SheetComponents;

public class SkillAction : CharacterResource
{
	public Ability[] Abilities { get; }
	
	internal SkillAction(ActionDefaultValuesAttribute defaults) :
		base(defaults.Current,defaults.HardLimit,defaults.SoftLimit,defaults.MoreIsBetter)
	{
		Abilities = defaults.Abilities;
	}

	public SkillAction() :
		base(0,-1,-1,true)
	{
		Abilities = [];
	}

	public SkillAction(Ability[] abilities) :
		base(0,-1,-1,true)
	{
		Abilities = abilities;
	}

	public SkillAction(Ability ability) :
		base(0,-1,-1,true)
	{
		Abilities = [ability];
	}
	
	public SkillAction(Ability[] abilities, int current = 0,int hardLimit = -1,int softLimit = -1,bool moreIsBetter = true) :
		base(current,hardLimit,softLimit,moreIsBetter)
	{
		Abilities = abilities;
	}

	public SkillAction(Ability ability, int current = 0,int hardLimit = -1,int softLimit = -1,bool moreIsBetter = true) :
		base(current,hardLimit,softLimit,moreIsBetter)
	{
		Abilities = [ability];
	}
}