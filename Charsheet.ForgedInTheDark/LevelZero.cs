using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Charsheet.ForgedInTheDark.Attributes;
using Charsheet.ForgedInTheDark.SheetComponents;

namespace Charsheet.ForgedInTheDark;

public class LevelZero
{
    // Abilities always in order: Offense, Defense, Flexile
    public enum Abilities
    {
        [Description("")]
        [ShortForm("NON")]
        None        = 0x00,
        
        #region Physical Abilities
        [Description("")]
        [ShortForm("POW")]
        Power       = 0x01,

        [Description("")]
        [ShortForm("BOD")]
        Body        = 0x02,

        [Description("")]
        [ShortForm("REF")]
        Reflex      = 0x03,
        #endregion
        
        #region Mental Abilities
        [Description("")]
        [ShortForm("FOC")]
        Focus       = 0x04,

        [Description("")]
        [ShortForm("WIL")]
        Will        = 0x05,

        [Description("")]
        [ShortForm("WIT")]
        Wit         = 0x06,
        #endregion
        
        #region Social Abilities
        [Description("")]
        [ShortForm("PRS")]
        Presence    = 0x07,

        [Description("")]
        [ShortForm("INT")]
        Integrity   = 0x08,

        [Description("")]
        [ShortForm("CHA")]
        Charm       = 0x09,
        #endregion

        [Description("")]
        [ShortForm("LUK")]
        Luck        = 0x0A,
    }
    public enum Actions
    {
        None        = 0x00,
        Ambush      = 0x01,
        Brawl       = 0x02,
        Duel        = 0x03,
        GunFight    = 0x04,
        Outmaneuver = 0x05,
        Sneak       = 0x06,
        Steal       = 0x07,
        Trick       = 0x08,
        Convince    = 0x09,
        Taunt       = 0x0A,
        Threaten    = 0x0B,
    }

    #region Fields(-)
    private Dictionary<Actions,SkillAction> _actions;
    private int _level = 0;
    #endregion

    #region Properties (+)
    public int Level => _level;
    #endregion

    public LevelZero()
    {  
        _level = 0;
        _actions = new Dictionary<Actions,SkillAction>{
            [Actions.Ambush     ] = new SkillAction([Abilities.Reflex,      Abilities.Wit       ],0,5,3),
            [Actions.Brawl      ] = new SkillAction([Abilities.Power,       Abilities.Body      ],0,5,3),
            [Actions.Duel       ] = new SkillAction([Abilities.Reflex,      Abilities.Focus     ],0,5,3),
            [Actions.Sneak      ] = new SkillAction([Abilities.Reflex,      Abilities.Wit       ],0,5,3),
            [Actions.Steal      ] = new SkillAction([Abilities.Reflex,      Abilities.Wit       ],0,5,3),
            [Actions.Trick      ] = new SkillAction([Abilities.Charm,       Abilities.Wit       ],0,5,3),
            [Actions.Convince   ] = new SkillAction([Abilities.Charm,       Abilities.Presence  ],0,5,3),
            [Actions.Taunt      ] = new SkillAction([Abilities.Presence,    Abilities.Charm     ],0,5,3),
            [Actions.Threaten   ] = new SkillAction([Abilities.Presence,    Abilities.Power     ],0,5,3),
        };
        Console.WriteLine(DescriptionAttribute.GetEnumDescription(Abilities.Power));
    }
}