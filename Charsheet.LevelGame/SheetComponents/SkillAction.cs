using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Charsheet.LevelGame.Enums;

namespace Charsheet.LevelGame.SheetComponents;

public class SkillAction : Resource
{
    public Abilities[] Abilities { get; }
    
    public SkillAction(Abilities[] abilities, int current = 0,int hardLimit = -1,int softLimit = -1,bool moreIsBetter = true) : base(current,hardLimit,softLimit,moreIsBetter)
    {
        Abilities = abilities;
    }

    public SkillAction(Abilities ability, int current = 0,int hardLimit = -1,int softLimit = -1,bool moreIsBetter = true) : base(current,hardLimit,softLimit,moreIsBetter)
    {
        Abilities = [ability];
    }
}