using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Charsheet.LevelGame;

public class GameObject(int level)
{
    #region Fields(#)
    protected int _level = level;
    #endregion

    #region Properties (+)
    public int Level => _level;
    #endregion
    
}