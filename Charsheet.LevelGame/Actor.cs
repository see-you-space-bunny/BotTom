using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Charsheet.LevelGame.Attributes;
using Charsheet.LevelGame.Enums;
using Charsheet.LevelGame.SheetComponents;

namespace Charsheet.LevelGame;

public class Actor : GameObject
{
    #region Constant
    public const float HitPointsScalingFactor = 1.115f;
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
    /// Hit Points formula: (int) BaseValue + Level * SQRT(POW(Body*MOD+Power*MOD+Finesse*MOD,2) + POW(Focus*MOD+Will*MOD+Wit*MOD+Presence*MOD+Integrity*MOD+Charm*MOD-1,2))
    /// </summary>
    public CharacterResource HealthPoints => _resources[Resource.HealthPoints];
    public new int Level => _classLevels.Values.Sum((cl)=>cl.Level);
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

        Console.WriteLine(DescriptionAttribute.GetEnumDescription(Ability.Power));
    }

    public Actor ReCalculateDerivedStatistics()
    {
        ReCalculateHitPoints();
        return this;
    }

    private void ReCalculateHitPoints()
    {
        int previousSoftLimit = HealthPoints.SoftLimit;
        int previousHardLimit = HealthPoints.HardLimit;
        foreach(ClassLevels classLevels in _classLevels.Values.Where((cl)=>cl.Level>0))
        {
            (HealthPoints.SoftLimit, HealthPoints.HardLimit) = CalcHealthPointsLimits(classLevels);
        }
        HealthPoints.Current *= Math.Max(HealthPoints.SoftLimit / previousSoftLimit, HealthPoints.HardLimit / previousHardLimit);
    }

    private (int SoftLimit,int HardLimit) CalcHealthPointsLimits(ClassLevels classLevels)
    {
        var healthPointScales = classLevels.Class.ResourceAbilityScales[Resource.HealthPoints];
        var healthPointModifiers = classLevels.Class.ResourceModifiers[Resource.HealthPoints];
        int softLimit = (int)(healthPointModifiers[ResourceModifier.SoftLimit] * (
            healthPointModifiers[ResourceModifier.BaseValue] +
            classLevels.Level * Math.Sqrt(
                Math.Pow(
                    (double)(
                        _abilities[Ability.Power    ]*healthPointScales[Ability.Power    ] +
                        _abilities[Ability.Body     ]*healthPointScales[Ability.Body     ] +
                        _abilities[Ability.Reflex   ]*healthPointScales[Ability.Reflex   ]
                ),2d) + Math.Pow(
                    (double)(
                        _abilities[Ability.Focus    ]*healthPointScales[Ability.Focus    ] +
                        _abilities[Ability.Will     ]*healthPointScales[Ability.Will     ] +
                        _abilities[Ability.Wit      ]*healthPointScales[Ability.Wit      ] +
                        _abilities[Ability.Presence ]*healthPointScales[Ability.Presence ] +
                        _abilities[Ability.Integrity]*healthPointScales[Ability.Integrity] +
                        _abilities[Ability.Charm    ]*healthPointScales[Ability.Charm    ]
                ),2d)
            )
        ));
        int hardLimit = (int)(softLimit * classLevels.Class.ResourceModifiers[Resource.HealthPoints][ResourceModifier.HardLimit]);
        return (softLimit,hardLimit);
    }

    #region Actor Extensions
    public Actor LevelUp(int levels = 1)
    {
        _classLevels[_activeClass].Level += levels;
        ReCalculateDerivedStatistics();
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