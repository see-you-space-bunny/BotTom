using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Charsheet.ForgedInTheDark.SheetComponents;

public class Resource(int current = 0,int hardLimit = -1,int softLimit = -1,bool moreIsBetter = true)
{
    private int _current = current;

    public int Current {
        get => IsSoftLimited && _current > SoftLimit? SoftLimit : _current;
        set => _current = IsHardLimited && value > HardLimit? HardLimit : value;
    }
    public int CurrentNoSoftLimit { get=>_current; }

    public int SoftLimit { get; set; } = softLimit;
    public int HardLimit { get; set; } = hardLimit;
    
    public bool IsSoftLimited { get; set; } = softLimit >= 0;
    public bool IsHardLimited { get; set; } = hardLimit >= 0;
    
    public bool IsAtSoftLimit => IsSoftLimited && _current >= SoftLimit;
    public bool IsAtHardLimit => IsHardLimited && _current == HardLimit;
    
    public bool IsOverSoftLimit => _current > SoftLimit;

    public bool MoreIsBetter { get; set; } = moreIsBetter;
}