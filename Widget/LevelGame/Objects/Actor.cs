using FChatApi.Attributes;
using LevelGame.Attributes;
using LevelGame.Core;
using LevelGame.Enums;
using LevelGame.SheetComponents;

namespace LevelGame.Objects;

public class Actor : GameObject
{
	#region (+) Constants
	public const float HealthPointsScalingFactor = 1.115f;
	#endregion

	#region (#) Fields
	protected ClassName _activeClass;
	protected Dictionary<ClassName,ClassLevels> _classLevels;
	protected Dictionary<Ability,CharacterResource> _abilities;
	protected Dictionary<GameAction,SkillAction> _actions;
	protected Dictionary<Resource,CharacterResource> _resources;
	protected List<ActiveStatusEffect> _statusEffects;
	#endregion

	#region (+) Properties
	public CharacterResource Health		=>	_resources[Resource.Health];
	public CharacterResource Protection	=>	_resources[Resource.Protection];
	public CharacterResource Evasion	=>	_resources[Resource.Evasion];
	public new int Level				=>	LevelSanityCheck();
	#endregion
	
	public Actor() : base(0)
	{
		_actions = [];
		foreach(GameAction gameAction in Enum.GetValues(typeof(GameAction)).Cast<GameAction>().Where(a=>a != GameAction.None))
		{
			_actions.Add(gameAction,new SkillAction(gameAction.GetEnumAttribute<GameAction,ActionDefaultValuesAttribute>()));
		}

		_abilities = [];
		foreach(Ability ability in Enum.GetValues(typeof(Ability)).Cast<Ability>().Where(a=>a != Ability.None))
		{
			_abilities.Add(ability,new CharacterResource());
		}

		_resources = [];
		foreach(Resource resource in Enum.GetValues(typeof(Resource)).Cast<Resource>().Where(a=>a != Resource.None))
		{
			_resources.Add(resource,new CharacterResource(resource.GetEnumAttribute<Resource,ResourceDefaultValuesAttribute>()));
		}

		_statusEffects = [];

		_classLevels = [];
	}

	#region Interaction
	public void ApplyStatusEffect(StatusEffect statusEffect,float intensity,Actor? source)
	{
		_statusEffects.Add(new ActiveStatusEffect(){ EffectType = statusEffect, Target = this, Intensity = intensity, Source = source});
	}
	#endregion

	#region Calculation
	public Actor ReCalculateAll()
	{
		ReCalculateDerivedStatistics();
		ReCalculateStatusEffects();
		return this;
	}

	public Actor ReCalculateStatusEffects()
	{
		foreach (ActiveStatusEffect effect in _statusEffects)
		{
			foreach ((Ability ability,float baseValue) in effect.AffectsAbilities)
			{
				_abilities[ability].SumOfModifiers += (int)(baseValue * effect.Intensity);
			}
		}
		return this;
	}

	public Actor ReCalculateDerivedStatistics()
	{
		ReCalculateResourceLimits();
		return this;
	}

	private void ReCalculateResourceLimits()
	{
		foreach (var resource in _resources.Where(r=>r.Value.IsHardLimited || r.Value.IsSoftLimited))
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
			CalcResourceLimits(classLevels,resource);
		}
		if (resource.Key.GetEnumAttribute<Resource,GameFlagsAttribute>().ScalesOnLimitChange)
			resource.Value.BaseValue *= Math.Max(Math.Max(resource.Value.SoftLimit / Math.Max(previousSoftLimit,1), resource.Value.HardLimit / Math.Max(previousHardLimit,1)),1);
	}

	private void CalcResourceLimits(ClassLevels classLevels,KeyValuePair<Resource, CharacterResource> resource)
	{
		resource.Value.SoftLimit = resource.Value.IsSoftLimited ? CalcResourceLimit(classLevels, resource.Key,ResourceModifier.SoftLimit) : -1;
		resource.Value.HardLimit = resource.Value.IsHardLimited ? CalcResourceLimit(classLevels, resource.Key,ResourceModifier.HardLimit) : -1;
	}

	/// <summary>
	/// SoftLimit formula: (int) ClassBaseValue + ClassLevel * ClassLevelScales + SumOfEach(Ability * ClassAbilityScales)
	/// </summary>
	private int CalcResourceLimit(ClassLevels classLevels, Resource resource,ResourceModifier limit) =>
		(int)Math.Max(
			classLevels.Class.ResourceModifiers[resource][limit] *
			(
				classLevels.Class.ResourceModifiers[resource][ResourceModifier.BaseValue] +
				classLevels.Level * classLevels.Class.ResourceAbilityScales[resource][Ability.Level] +
				_abilities.Keys.Where(k=>k!=Ability.Level).Sum(k =>(int)(_abilities[k].Current * classLevels.Class.ResourceAbilityScales[resource][k]))
			),
			classLevels.Class.ResourceModifiers[resource][ResourceModifier.MinimumValue]
		);
	
	protected int LevelSanityCheck()
	{
		int level = _classLevels.Values.Sum((cl)=>cl.Level);
		if (level != _abilities[Ability.Level].BaseValue)
			_abilities[Ability.Level].BaseValue = level;
		return level;
	}
	#endregion

	#region Actor Extensions
	public Actor LevelUpRoll()
	{
		LevelUp(11 + World.Rng.Next(1,11) + World.Rng.Next(1,11));
		return this;
	}

	public Actor LevelUp(int levels = 1)
	{
		if (_classLevels.TryGetValue(_activeClass, out var activeClassLevels))
		{
			activeClassLevels.Level += levels;
		}
		else
		{
		    _classLevels.Add(_activeClass,new ClassLevels(World.CharacterClasses[_activeClass],levels));
		}
		_abilities[Ability.Level].BaseValue += levels;
		ReCalculateDerivedStatistics();
		return this;
	}

	public Actor FullRecovery()
	{
		ReCalculateResourceLimits();
		foreach (var resource in _resources)
		{
			if (resource.Key.GetEnumAttribute<Resource,GameFlagsAttribute>().RefilledByFullRecovery)
			{
				if (resource.Value.IsSoftLimited)
				{
					resource.Value.BaseValue = resource.Value.SoftLimit;
				}
				else if (resource.Value.IsHardLimited)
				{
					resource.Value.BaseValue = resource.Value.HardLimit;
				}
			}
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
		_resources[resource].BaseValue += value;
		return this;
	}

	public Actor SetResource(Resource resource,int value)
	{
		_resources[resource].BaseValue = value;
		return this;
	}
	#endregion
}