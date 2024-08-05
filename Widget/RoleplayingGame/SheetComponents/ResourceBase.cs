using System.Reflection.Metadata.Ecma335;
using RoleplayingGame.Attributes;
using RoleplayingGame.Enums;
using RoleplayingGame.Objects;

namespace RoleplayingGame.SheetComponents;

public class ResourceBase
{
	protected Actor _parent;
	protected int _softLimit;
	protected int _hardLimit;

	public float SumOfModifiers { get; set; }
	public float CombinedMultipliers { get; set; } = 1.0f;

	public float SumOfLimitModifiers { get; set; }
	public float CombinedLimitMultipliers { get; set; } = 1.0f;

	public int SoftLimit { get=>(int)((_softLimit + SumOfLimitModifiers)*CombinedLimitMultipliers); set=> _softLimit=value; }
	public int HardLimit { get=>(int)((_hardLimit + SumOfLimitModifiers)*CombinedLimitMultipliers); set=> _hardLimit=value; }
	
	public bool HasSoftLimit { get; set; }
	public bool HasHardLimit { get; set; }
	
	public virtual bool IsAtSoftLimit => false;
	public virtual bool IsAtHardLimit => false;
	public virtual bool IsOverSoftLimit => false;

	public bool MoreIsBetter { get; set; }

	public ResourceBase(Actor parent,int hardLimit = -1,int softLimit = -1,bool moreIsBetter = true)
	{
		_parent			= parent;
		_softLimit		= softLimit;
		_hardLimit		= hardLimit;
		HasSoftLimit	= softLimit >= 0;
		HasHardLimit	= hardLimit >= 0;
		MoreIsBetter	= moreIsBetter;
	}
}