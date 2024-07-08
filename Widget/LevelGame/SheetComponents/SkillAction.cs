using Widget.LevelGame.Enums;

namespace Widget.LevelGame.SheetComponents;

public class SkillAction : CharacterResource
{
	public Ability[] Abilities { get; }
	
	public SkillAction(Ability[] abilities, int current = 0,int hardLimit = -1,int softLimit = -1,bool moreIsBetter = true) : base(current,hardLimit,softLimit,moreIsBetter)
	{
		Abilities = abilities;
	}

	public SkillAction(Ability ability, int current = 0,int hardLimit = -1,int softLimit = -1,bool moreIsBetter = true) : base(current,hardLimit,softLimit,moreIsBetter)
	{
		Abilities = [ability];
	}
}