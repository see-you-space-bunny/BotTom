using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Charsheet.ForgedInTheDark.SheetComponents;

public class SkillAction : Resource
{
    public LevelZero.Abilities[] Abilities { get; }
    
    public SkillAction(LevelZero.Abilities[] abilities, int current = 0,int hardLimit = -1,int softLimit = -1,bool moreIsBetter = true) : base(current,hardLimit,softLimit,moreIsBetter)
    {
        Abilities = abilities;
    }

    public SkillAction(LevelZero.Abilities ability, int current = 0,int hardLimit = -1,int softLimit = -1,bool moreIsBetter = true) : base(current,hardLimit,softLimit,moreIsBetter)
    {
        Abilities = [ability];
    }
}