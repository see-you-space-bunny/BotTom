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
    protected CharacterResource _hitPoints;
    #endregion

    #region Properties (+)
    /// <summary>
    /// Hit Points formula: Level * SQRT(POW(Body*0.8+Power*0.15+Finesse*0.05,2) + POW(Will*0.5 + Focus*0.3 + Presence*0.2 - 1,2)) + 128
    /// </summary>
    public CharacterResource HitPoints => _hitPoints;
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

        _hitPoints = new CharacterResource(0);

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
        int previousSoftLimit = _hitPoints.SoftLimit;
        int previousHardLimit = _hitPoints.HardLimit;
        _hitPoints.SoftLimit = 0;
        _hitPoints.HardLimit = 0;
        foreach(ClassLevels classLevels in _classLevels.Values)
        {
            var classLimits = CalcHitPointsLimits(classLevels);
            _hitPoints.SoftLimit += classLimits.SoftLimit;
            _hitPoints.HardLimit += classLimits.HardLimit;
        }
        _hitPoints.Current *= Math.Max(_hitPoints.SoftLimit / previousSoftLimit, _hitPoints.HardLimit / previousHardLimit);
    }

    private (int SoftLimit,int HardLimit) CalcHitPointsLimits(ClassLevels classLevels)
    {
        int softLimit = (int)(classLevels.Class.ResourceModifiers[Resource.HealthPoints][ResourceModifier.SoftLimit] *
            classLevels.Level * Math.Sqrt(
                Math.Pow(
                    (double)(
                        _abilities[Ability.Power    ]*classLevels.Class.ResourceAbilityScales[Resource.HealthPoints][Ability.Power      ]   +
                        _abilities[Ability.Body     ]*classLevels.Class.ResourceAbilityScales[Resource.HealthPoints][Ability.Body       ]   +
                        _abilities[Ability.Reflex   ]*classLevels.Class.ResourceAbilityScales[Resource.HealthPoints][Ability.Reflex     ]   
                ),2d) + Math.Pow(
                    (double)(
                        _abilities[Ability.Focus    ]*classLevels.Class.ResourceAbilityScales[Resource.HealthPoints][Ability.Focus      ]   +
                        _abilities[Ability.Will     ]*classLevels.Class.ResourceAbilityScales[Resource.HealthPoints][Ability.Will       ]   +
                        _abilities[Ability.Wit      ]*classLevels.Class.ResourceAbilityScales[Resource.HealthPoints][Ability.Wit        ]   +
                        _abilities[Ability.Presence ]*classLevels.Class.ResourceAbilityScales[Resource.HealthPoints][Ability.Presence   ]   +
                        _abilities[Ability.Integrity]*classLevels.Class.ResourceAbilityScales[Resource.HealthPoints][Ability.Integrity  ]   +
                        _abilities[Ability.Charm    ]*classLevels.Class.ResourceAbilityScales[Resource.HealthPoints][Ability.Charm      ]   
                ),2d)
            )
        );
        int hardLimit = (int)(softLimit * classLevels.Class.ResourceModifiers[Resource.HealthPoints][ResourceModifier.HardLimit]);
        return (softLimit,hardLimit);
    }
}