using Charsheet.LevelGame.Attributes;

namespace Charsheet.LevelGame.Enums;

// Abilities always in order: Offense, Defense, Flexile
public enum Ability
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
