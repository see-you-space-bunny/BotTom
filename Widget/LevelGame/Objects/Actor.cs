using FChatApi.Attributes;
using LevelGame.Attributes;
using LevelGame.Core;
using LevelGame.Enums;
using LevelGame.SheetComponents;

namespace LevelGame.Objects;

public class Actor : GameObject
{
	#region Constant
	public const float HealthPointsScalingFactor = 1.115f;
	public const int MinimumHealthPoints = 10;
	#endregion

	#region Fields(#)
	protected ClassName _activeClass;
	protected Dictionary<ClassName,ClassLevels> _classLevels;
	protected Dictionary<Ability,int> _abilities;
	protected Dictionary<SkillActions,SkillAction> _actions;
	protected Dictionary<Resource,CharacterResource> _resources;
	#endregion

	#region Properties (+)
	/// <summary>
	/// Hit Points formula: (int) BaseValue + Level * SumOfEach(Ability * MOD)
	/// </summary>
	public CharacterResource Health		=>	_resources[Resource.Health];
	public CharacterResource Protection	=>	_resources[Resource.Protection];
	public CharacterResource Evasion	=>	_resources[Resource.Evasion];
	public new int Level				=>	_classLevels.Values.Sum((cl)=>cl.Level);
	#endregion
	
	public Actor() : base(0)
	{
		_actions = new Dictionary<SkillActions,SkillAction>{
			[SkillActions.Ambush     ] = new SkillAction([Ability.Reflex,      Ability.Wit       ],0,5,3),
			[SkillActions.Brawl      ] = new SkillAction([Ability.Power,       Ability.Body      ],0,5,3),
			[SkillActions.Duel       ] = new SkillAction([Ability.Reflex,      Ability.Focus     ],0,5,3),
			[SkillActions.Sneak      ] = new SkillAction([Ability.Reflex,      Ability.Wit       ],0,5,3),
			[SkillActions.Steal      ] = new SkillAction([Ability.Reflex,      Ability.Wit       ],0,5,3),
			[SkillActions.Trick      ] = new SkillAction([Ability.Charm,       Ability.Wit       ],0,5,3),
			[SkillActions.Convince   ] = new SkillAction([Ability.Charm,       Ability.Presence  ],0,5,3),
			[SkillActions.Taunt      ] = new SkillAction([Ability.Presence,    Ability.Charm     ],0,5,3),
			[SkillActions.Threaten   ] = new SkillAction([Ability.Presence,    Ability.Power     ],0,5,3),
		};

		_abilities = [];
		foreach(Ability ability in Enum.GetValues(typeof(Ability)).Cast<Ability>())
		{
			_abilities.Add(ability,0);
		}

		_resources = [];
		foreach(Resource resource in Enum.GetValues(typeof(Resource)).Cast<Resource>())
		{
			_resources.Add(resource,new CharacterResource());
		}

		_classLevels = [];

		// Causes StackOverFlowException!! ???
		// Console.WriteLine(DescriptionAttribute.GetEnumDescription(Ability.Power));
	}

	public Actor ReCalculateDerivedStatistics()
	{
		ReCalculateResourceLimits();
		return this;
	}

	private void ReCalculateResourceLimits()
	{
		foreach (var resource in _resources)
		{
			ReCalculateResourceLimit(resource);
		}
	}

	private void ReCalculateResourceLimit(KeyValuePair<Resource, CharacterResource> resource)
	{
		int previousSoftLimit = resource.Value.SoftLimit;
		int previousHardLimit = resource.Value.HardLimit;
		foreach(ClassLevels classLevels in _classLevels.Values.Where((cl)=>cl.Level>0))
		{
			(resource.Value.SoftLimit, resource.Value.HardLimit) = CalcResourceLimits(classLevels,resource.Key);
		}
		resource.Value.Current *= Math.Max(resource.Value.SoftLimit / previousSoftLimit, resource.Value.HardLimit / previousHardLimit);
	}

	private (int SoftLimit,int HardLimit) CalcResourceLimits(ClassLevels classLevels,Resource resource)
	{
		var resourceScales		= classLevels.Class.ResourceAbilityScales[resource];
		var resourceModifiers	= classLevels.Class.ResourceModifiers[resource];
		int abilitySum			= _abilities.Keys.Sum(k=>(int)(_abilities[k]*resourceScales[k]));

		int softLimit = (int)(resourceModifiers[ResourceModifier.SoftLimit] * (
			resourceModifiers[ResourceModifier.BaseValue] +
			classLevels.Level *
			_abilities.Keys.Sum(k=>(int)(_abilities[k]*resourceScales[k]))
		));

		int hardLimit = (int)(softLimit * classLevels.Class.ResourceModifiers[resource][ResourceModifier.HardLimit]);
		softLimit = Math.Max(softLimit,(int)classLevels.Class.ResourceModifiers[resource][ResourceModifier.MinimumValue]);
		hardLimit = Math.Max(hardLimit,softLimit);
		return (softLimit,hardLimit);
	}

	#region Actor Extensions
	public Actor LevelUp(int levels = 1)
	{
		if (_classLevels.TryGetValue(_activeClass, out var activeClassLevels))
			activeClassLevels.Level += levels;
		else
		    _classLevels.Add(_activeClass,new ClassLevels(World.CharacterClasses[_activeClass],levels));
		ReCalculateDerivedStatistics();
		return this;
	}

	public Actor FullRecovery()
	{
		foreach (var resource in _resources)
		{
			if (resource.Key.GetEnumAttribute<Resource,GameFlagsAttribute>().RefilledByFullRecovery)
				resource.Value.Current = resource.Value.SoftLimit;
		}
		return this;
	}

	public Actor ChangeClass(ClassName className)
	{
		_activeClass = className;
		return this;
	}

	public Actor AddToResource(Resource resource,int value)
	{
		_resources[resource].Current += value;
		return this;
	}

	public Actor SetResource(Resource resource,int value)
	{
		_resources[resource].Current = value;
		return this;
	}
	#endregion
}