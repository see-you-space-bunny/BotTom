using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Charsheet.LevelGame.SheetComponents;

public class ClassLevels(CharacterClass @class,int level=0)
{
    public CharacterClass Class = @class;
    public int Level = level;
}