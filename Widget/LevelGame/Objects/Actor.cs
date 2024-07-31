using FChatApi.Attributes;
using RoleplayingGame.Attributes;
using RoleplayingGame.Enums;
using RoleplayingGame.Effects;
using RoleplayingGame.SheetComponents;
using RoleplayingGame.Statistics;
using RoleplayingGame.Interfaces;

namespace RoleplayingGame.Objects;

public class Actor : GameObject
{
#region (+) Constants
	public const float HealthPointsScalingFactor = 1.115f;
#endregion

#region Properties (+)
	protected string _characterName;
	public string CharacterName => _characterName;
#endregion

#region (#) Fields
	protected ClassName _activeClass;
	protected Dictionary<ClassName,ClassLevels> _classLevels;
	protected Dictionary<Ability,AbilityScore> _abilities;
	protected Dictionary<GameAction,SkillAction> _actions;
	protected Dictionary<Resource,CharacterResource> _resources;
	protected List<ActiveStatusEffect> _statusEffects;
#endregion

#region (+P)
	public CharacterResource Health		=>	_resources[Resource.Health];
	public CharacterResource Protection	=>	_resources[Resource.Protection];
	public CharacterResource Evasion	=>	_resources[Resource.Evasion];
	public Dictionary<Ability,AbilityScore> Abilities => _abilities.Where(k=>k.Key!=Ability.Level).ToDictionary();
	public new AbilityScore Level	=>	_abilities[Ability.Level];
	public Dictionary<ClassName,ClassLevels> ClassLevels => _classLevels;
#endregion

#region (+P) Meta
	public ActorStatistics Statistics;
#endregion

#region Interaction
	public Actor AttackActorTarget(AttackType attackType,Actor target)
	{
		return this;
	}

	public Actor ApplyAttackEffect(AttackEffect attack)
	{
		if (!attack.TryToHit(Evasion,Health))
		{ }

		if (!attack.TryToImpact(Protection))
		{ }

		if (!attack.TryToHarm(Health))
		{
            FRoleplayMC.ApplyStatusEffect(StatusEffect.Defeated,this,1.0f,null);
		}

		(ulong EvasionLoss,bool Hit,ulong ProtectionLoss,bool ProtBreak,ulong HealthLoss,bool Kill,ulong Overkill) = attack.AttackInfo();
		if (attack.EnvironmentSource != EnvironmentSource.None)
		{
			Statistics.RecordIncomingAttackResults(attack.Source,EvasionLoss,Hit,ProtectionLoss,ProtBreak,HealthLoss,Kill,Overkill);
			Statistics.RecordOutgoingAttackResults(attack.Source,EvasionLoss,Hit,ProtectionLoss,ProtBreak,HealthLoss,Kill,Overkill);
		}
		else
		{
			Statistics.RecordIncomingAttackResults(attack.EnvironmentSource,EvasionLoss,Hit,ProtectionLoss,ProtBreak,HealthLoss,Kill,Overkill);
			Statistics.RecordOutgoingAttackResults(attack.EnvironmentSource,EvasionLoss,Hit,ProtectionLoss,ProtBreak,HealthLoss,Kill,Overkill);
		}
		return this;
	}

	public Actor ApplyStatusEffect(ActiveStatusEffect statusEffect)
	{
		_statusEffects.Add(statusEffect);
		return this;
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
		foreach (var resource in _resources.Where(r=>r.Value.HasHardLimit || r.Value.HasSoftLimit))
		{
			ReCalculateResourceLimit(resource);
		}
	}

	private void ReCalculateResourceLimit(KeyValuePair<Resource, CharacterResource> resource)
	{
		int previousSoftLimit	= resource.Value.SoftLimit;
		int previousHardLimit	= resource.Value.HardLimit;
		int highestBaseValue	= int.MinValue;
		int highestMinimumValue	= int.MinValue;
		foreach(ClassLevels classLevels in _classLevels.Values.Where((cl)=>cl.Level>0))
		{
			highestMinimumValue	= Math.Max(highestBaseValue,(int)classLevels.Class.ResourceModifiers[resource.Key][ResourceModifier.MinimumValue]);
			highestBaseValue	= Math.Max(highestBaseValue,(int)classLevels.Class.ResourceModifiers[resource.Key][ResourceModifier.BaseValue]);
			CalcResourceLimits(classLevels,resource);
		}
		if (resource.Value.HasSoftLimit)
		{
			resource.Value.SoftLimit += highestBaseValue;
			resource.Value.SoftLimit = Math.Max(resource.Value.SoftLimit,highestMinimumValue);
		}
		if (resource.Value.HasHardLimit)
		{
			resource.Value.HardLimit += highestBaseValue;
			resource.Value.HardLimit = Math.Max(resource.Value.HardLimit,highestMinimumValue);
		}
		if (resource.Key.GetEnumAttribute<Resource,GameFlagsAttribute>().ScalesOnLimitChange)
			resource.Value.BaseValue *= Math.Max(Math.Max(resource.Value.SoftLimit / Math.Max(previousSoftLimit,1), resource.Value.HardLimit / Math.Max(previousHardLimit,1)),1);
	}

	private void CalcResourceLimits(ClassLevels classLevels,KeyValuePair<Resource, CharacterResource> resource)
	{
		resource.Value.SoftLimit = resource.Value.HasSoftLimit ? (int)CalcResourceLimit(classLevels, resource.Key,ResourceModifier.SoftLimit) : -1;
		resource.Value.HardLimit = resource.Value.HasHardLimit ? (int)CalcResourceLimit(classLevels, resource.Key,ResourceModifier.HardLimit) : -1;
	}
	
/// <summary>
/// SoftLimit formula: (int) ClassLevel * ClassLevelScales + SumOfEach(Ability * ClassAbilityScales)
/// </summary>
/// <param name="classLevels">the class for which we are calculating the limit</param>
/// <param name="resource">the resource whose limit is being calculated</param>
/// <param name="limit">the soft of hard limit being calculated</param>
/// <returns>the resource's calculated limit</returns>
	private float CalcResourceLimit(ClassLevels classLevels, Resource resource,ResourceModifier limit) =>
		classLevels.Class.ResourceModifiers[resource][limit] * (
			classLevels.Level * classLevels.Class.ResourceAbilityScales[resource][Ability.Level] +
			Abilities.Keys
				.Sum(k =>Abilities[k].GetActualValue() * classLevels.Class.ResourceAbilityScales[resource][k])
		);
#endregion

#region Status Effects

	internal float SumStatusAdjustmentByAbility(Ability value)
	{
		float result = 0.0f;
		foreach (ActiveStatusEffect statusEffect in _statusEffects)
		{
			foreach ((Ability ability,float adjustment) in statusEffect.AffectsAbilities)
			{
				if (ability == value)
					result += adjustment;
			}
		}
		return result;
	}

#endregion

#region Actor Extensions
/// <summary>
/// Generates a random number between 18 and 33 and calls the <c>LevelUp</c> method.<br/>
/// The <c>LevelUp</c> method is subject to growth-scales.
/// </summary>
/// <returns>this Actor</returns>
	public Actor LevelUpRoll()
	{
        LevelUp(15 + FRoleplayMC.Rng.Next(1,7) + FRoleplayMC.Rng.Next(1,7) + FRoleplayMC.Rng.Next(1,7));
		return this;
	}

/// <summary>
/// Adds a number of levels to this Actor's currently active class.
/// </summary>
/// <param name="levels">the number of levels to add, subject to growth-scales</param>
/// <returns>this Actor</returns>
	public Actor LevelUp(int levels = 1)
	{
		if (_classLevels.TryGetValue(_activeClass, out var activeClassLevels))
		{
			levels = (int)Math.Round(activeClassLevels.Class.AbilityGrowth[Ability.Level] * levels);
			levels = levels == 0 ? 1 : levels;
			activeClassLevels.Level += levels;
		}
		else
		{
            ClassLevels @class = new (FRoleplayMC.CharacterClasses[_activeClass],0);
			levels = (int)Math.Round(@class.Class.AbilityGrowth[Ability.Level] * levels);
			levels = levels == 0 ? 1 : levels;
			@class.Level += levels;
		    _classLevels.Add(_activeClass,@class);
		}
		Level.Modify(DateTime.Now,EnvironmentSource.World,levels);
		return this;
	}

/// <summary>
/// 
/// </summary>
/// <param name="value"></param>
/// <returns>this Actor</returns>
	public Actor AdjustAllAbilities(int value,EnvironmentSource source = EnvironmentSource.World,bool recalculate = true)
	{
		AdjustAbilities([.. Abilities.Keys],value,source,recalculate: false);
		if (recalculate)
			ReCalculateDerivedStatistics();
		return this;
	}

/// <summary>
/// 
/// </summary>
/// <param name="abilities"></param>
/// <param name="value"></param>
/// <returns>this Actor</returns>
	public Actor AdjustAbilities(Ability[] abilities,int value,EnvironmentSource source = EnvironmentSource.World,bool recalculate = true)
	{
		foreach (Ability ability in abilities)
		{
			AdjustAbility(ability,value,source,recalculate: false);
		}
		if (recalculate)
			ReCalculateDerivedStatistics();
		return this;
	}

/// <summary>
/// 
/// </summary>
/// <param name="ability"></param>
/// <param name="value"></param>
/// <returns>this Actor</returns>
	public Actor AdjustAbility(Ability ability,int value,EnvironmentSource source = EnvironmentSource.World,bool recalculate = true)
	{
		Abilities[ability].Modify(DateTime.Now,source,value);
		if (recalculate)
			ReCalculateDerivedStatistics();
		return this;
	}

/// <summary>
/// Remove an amount of levels from this Actor's currently active class.
/// </summary>
/// <param name="levels">the levels to remove, unaffected by growth scales</param>
/// <returns>this Actor</returns>
	public Actor RemoveLevels(int levels)
	{
		if (_classLevels.TryGetValue(_activeClass, out var activeClassLevels))
		{
			activeClassLevels.Level -= levels;
		}
		else
		{
            _classLevels.Add(_activeClass, new ClassLevels(FRoleplayMC.CharacterClasses[_activeClass], -levels));
		}
		Level.Modify(DateTime.Now,EnvironmentSource.World,-levels);
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
				if (resource.Value.HasSoftLimit)
				{
					resource.Value.BaseValue = resource.Value.SoftLimit;
				}
				else if (resource.Value.HasHardLimit)
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

#region (+) Constructor
	public Actor(string name) : base(0)
	{
		_characterName = name;
		_actions = [];
		foreach(GameAction gameAction in Enum.GetValues(typeof(GameAction)).Cast<GameAction>().Where(a=>a != GameAction.None))
		{
			_actions.Add(gameAction,new SkillAction(gameAction.GetEnumAttribute<GameAction,ActionDefaultValuesAttribute>()));
		}

		_abilities = [];
		foreach(Ability ability in Enum.GetValues(typeof(Ability)).Cast<Ability>().Where(a=>a != Ability.None))
		{
			_abilities.Add(ability,new AbilityScore(this,ability));
		}

		_resources = [];
		foreach(Resource resource in Enum.GetValues(typeof(Resource)).Cast<Resource>().Where(a=>a != Resource.None))
		{
			_resources.Add(resource,new CharacterResource(this,resource.GetEnumAttribute<Resource,ResourceDefaultValuesAttribute>()));
		}

		_statusEffects = [];

		_classLevels = [];

		Statistics = new ActorStatistics();
	}
#endregion
}