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
	protected Dictionary<Resource,CharacterResource> _resources;
	protected List<ActiveStatusEffect> _statusEffects;
	protected uint _levelCap;
	protected ulong _actorId;
#endregion

#region (+P)
	public CharacterResource Health							=>	_resources[Resource.Health];
	public CharacterResource Protection						=>	_resources[Resource.Protection];
	public CharacterResource Evasion						=>	_resources[Resource.Evasion];
	public Dictionary<Ability,AbilityScore> Abilities		=>	_abilities.Where(k=>k.Key!=Ability.Level).ToDictionary();
	public new int Level									=>	_classLevels.Values.Sum(cl=>cl.CurrentLevel);
	public Dictionary<ClassName,ClassLevels> ClassLevels	=>	_classLevels;
	public uint LevelCap	=>	_levelCap;
	public ulong ActorId	=>	_actorId;
#endregion

#region (+P) Meta
	public ActorStatistics Statistics;
#endregion

#region Interaction
	public Actor AttackActorTarget(AttackType attackType,Actor target)
	{
		return this;
	}

	public Actor ApplyAttackEffect(AttackEvent attack)
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
		return this;
	}

	public Actor ReCalculateDerivedStatistics()
	{
		ReCalculateResourceLimits();
		return this;
	}

	private void ReCalculateResourceLimits()
	{
		foreach (var resource in _resources.Values.Where(r=>r.HasHardLimit || r.HasSoftLimit))
		{
			resource.ReCalculateLimits();
		}
	}
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
	public Actor LevelUpRoll(ushort @base,ushort dice,EnvironmentSource source = EnvironmentSource.World)
	{
        LevelUp(@base + new int[dice].Sum(d=>FRoleplayMC.Rng.Next(1,7)),source);
		return this;
	}
	
/// <summary>
/// Generates a random number between 18 and 33 and calls the <c>LevelUp</c> method.<br/>
/// The <c>LevelUp</c> method is subject to growth-scales.
/// </summary>
/// <returns>this Actor</returns>
	public Actor LevelUpRoll(ushort @base,ushort dice,Actor source)
	{
        LevelUp(@base + new int[dice].Sum(d=>FRoleplayMC.Rng.Next(1,7)),source);
		return this;
	}

/// <summary>
/// Adds a number of levels to this Actor's currently active class.
/// </summary>
/// <param name="levels">the number of levels to add, subject to growth-scales</param>
/// <returns>this Actor</returns>
	public Actor LevelUp(int levels,EnvironmentSource source)
	{
		if (_classLevels.TryGetValue(_activeClass, out var activeClassLevels))
		{
			levels = (int)Math.Round(activeClassLevels.Class.AbilityGrowth[Ability.Level] * levels);
			levels = levels == 0 ? 1 : levels;
			activeClassLevels.Modify(DateTime.Now,source,levels);
		}
		else
		{
            ClassLevels @class = new (this,FRoleplayMC.CharacterClasses[_activeClass]);
			levels = (int)Math.Round(@class.Class.AbilityGrowth[Ability.Level] * levels);
			levels = levels == 0 ? 1 : levels;
			@class.Modify(DateTime.Now,source,levels);
		    _classLevels.Add(_activeClass,@class);
		}
		return this;
	}

/// <summary>
/// Adds a number of levels to this Actor's currently active class.
/// </summary>
/// <param name="levels">the number of levels to add, subject to growth-scales</param>
/// <returns>this Actor</returns>
	public Actor LevelUp(int levels,Actor source)
	{
		if (_classLevels.TryGetValue(_activeClass, out var activeClassLevels))
		{
			levels = (int)Math.Round(activeClassLevels.Class.AbilityGrowth[Ability.Level] * levels);
			levels = levels == 0 ? 1 : levels;
			activeClassLevels.Modify(DateTime.Now,source.ActorId,levels);
		}
		else
		{
            ClassLevels @class = new (this,FRoleplayMC.CharacterClasses[_activeClass]);
			levels = (int)Math.Round(@class.Class.AbilityGrowth[Ability.Level] * levels);
			levels = levels == 0 ? 1 : levels;
			@class.Modify(DateTime.Now,source.ActorId,levels);
		    _classLevels.Add(_activeClass,@class);
		}
		return this;
	}

/// <summary>
/// 
/// </summary>
/// <param name="value"></param>
/// <returns>this Actor</returns>
	public Actor AdjustAllAbilities(int value,EnvironmentSource source,bool recalculate = true)
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
	public Actor AdjustAbilities(Ability[] abilities,int value,EnvironmentSource source,bool recalculate = true)
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
	public Actor AdjustAbility(Ability ability,int value,EnvironmentSource source,bool recalculate = true)
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
	public Actor RemoveLevels(int levels,EnvironmentSource source)
	{
		if (_classLevels.TryGetValue(_activeClass, out var activeClassLevels))
		{
			activeClassLevels.Modify(DateTime.Now,source,-levels);
		}
		else
		{
			activeClassLevels = new ClassLevels(this,FRoleplayMC.CharacterClasses[_activeClass]);
			activeClassLevels.Modify(DateTime.Now,source,-levels);
            _classLevels.Add(_activeClass, activeClassLevels);
		}
		//Level.Modify(DateTime.Now,source,-levels);
		ReCalculateDerivedStatistics();
		return this;
	}

/// <summary>
/// Remove an amount of levels from this Actor's currently active class.
/// </summary>
/// <param name="levels">the levels to remove, unaffected by growth scales</param>
/// <returns>this Actor</returns>
	public Actor RemoveLevels(int levels,Actor source)
	{
		if (_classLevels.TryGetValue(_activeClass, out var activeClassLevels))
		{
			activeClassLevels.Modify(DateTime.Now,source.ActorId,-levels);
		}
		else
		{
			activeClassLevels = new ClassLevels(this,FRoleplayMC.CharacterClasses[_activeClass]);
			activeClassLevels.Modify(DateTime.Now,source.ActorId,-levels);
            _classLevels.Add(_activeClass, activeClassLevels);
		}
		//Level.Modify(DateTime.Now,source.UserId,-levels);
		ReCalculateDerivedStatistics();
		return this;
	}

	public Actor FullRecovery()
	{
		ReCalculateAll();
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
		_characterName	= name;
		_actorId			= Convert.ToUInt64(new Guid().ToString("N"));
		_levelCap		= 500u;

		_abilities = [];
		foreach(Ability ability in Enum.GetValues(typeof(Ability)).Cast<Ability>().Where(a=>a != Ability.None))
		{
			_abilities.Add(ability,new AbilityScore(this,ability));
		}

		_resources = [];
		foreach(Resource resource in Enum.GetValues(typeof(Resource)).Cast<Resource>().Where(a=>a != Resource.None))
		{
			_resources.Add(resource,new CharacterResource(this,resource,resource.GetEnumAttribute<Resource,ResourceDefaultValuesAttribute>()));
		}

		_statusEffects = [];

		_classLevels = [];

		Statistics = new ActorStatistics();
	}
	public Actor(string name,ulong userId) : base(0)
	{
		_characterName	= name;
		_actorId			= userId;
		_levelCap		= 500u;

		_abilities = [];
		foreach(Ability ability in Enum.GetValues(typeof(Ability)).Cast<Ability>().Where(a=>a != Ability.None && a != Ability.Level))
		{
			_abilities.Add(ability,new AbilityScore(this,ability));
		}

		_resources = [];
		foreach(Resource resource in Enum.GetValues(typeof(Resource)).Cast<Resource>().Where(a=>a != Resource.None))
		{
			_resources.Add(resource,new CharacterResource(this,resource,resource.GetEnumAttribute<Resource,ResourceDefaultValuesAttribute>()));
		}

		_statusEffects = [];

		_classLevels = [];

		Statistics = new ActorStatistics();
	}
#endregion
}