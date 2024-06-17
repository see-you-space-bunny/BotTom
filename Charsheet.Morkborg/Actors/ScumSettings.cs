using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Charsheet.MorkBorg
{
    public struct ScumSettings(
        string? name              = null,
        string? creditUrl         = null,
        string? creditText        = null,
        int? agilityMod           = null,
        int? presenceMod          = null,
        int? strengthMod          = null,
        int? toughnessMod         = null,
        string? weaponFormula     = null,
        string? armorFormula      = null,
        string? silverFormula     = null,
        string? hitPointsFormula  = null,
        int? omenDie              = null,
        string? backgroundFormula = null,
        string[]? backgroundTable = null,
        string? specialFormula    = null,
        string[]? specialTable    = null
    )
    {
        #region Const
        private const string _defaultName              = "Nameless Scum";
        private const string _defaultCreditUrl         = "https://morkborg.com/";
        private const string _defaultCreditText        = "MÖRK BORG is constructed by Pelle Nilsson (Ockult Örtmästare Games) and Johan Nohr (Stockholm Kartell). The game is published by Free League Publishing and is available in Swedish and English - the English text is edited and cursed by Patrick Stuart (Veins of the Earth, Silent Titans).";
        private const int    _defaultAgilityMod        = 0;
        private const int    _defaultPresenceMod       = 0;
        private const int    _defaultStrengthMod       = 0;
        private const int    _defaultToughnessMod      = 0;
        private const string _defaultWeaponFormula     = "d2";
        private const string _defaultArmorFormula      = "d2";
        private const string _defaultSilverFormula     = "2d6*10";
        private const string _defaultHitPointsFormula  = "d2+T";
        private const int    _defaultOmenDie           = 2;
        private const string _defaultBackgroundFormula = "d6";
        private const string? _defaultSpecialFormula   = null;
        private const string[]? _defaultSpecialTable   = null;
        #endregion

        #region F(+)
        public string Name              = name              ?? _defaultName;
        public string CreditUrl         = creditUrl         ?? _defaultCreditUrl;
        public string CreditText        = creditText        ?? _defaultCreditText;
        public int AgilityMod           = agilityMod        ?? _defaultAgilityMod;
        public int PresenceMod          = presenceMod       ?? _defaultPresenceMod;
        public int StrengthMod          = strengthMod       ?? _defaultStrengthMod;
        public int ToughnessMod         = toughnessMod      ?? _defaultToughnessMod;
        public string WeaponFormula     = weaponFormula     ?? _defaultWeaponFormula;
        public string ArmorFormula      = armorFormula      ?? _defaultArmorFormula;
        public string SilverFormula     = silverFormula     ?? _defaultSilverFormula;
        public string HitPointsFormula  = hitPointsFormula  ?? _defaultHitPointsFormula;
        public int OmenDie              = omenDie           ?? _defaultOmenDie;
        public string BackgroundFormula = backgroundFormula ?? _defaultBackgroundFormula;
        public string[] BackgroundTable = backgroundTable   ?? ["A nobody.","A nobody.","A nobody.","A nobody.","A nobody.","A nobody."];
        public string? SpecialFormula   = specialFormula    ?? _defaultSpecialFormula;
        public string[]? SpecialTable   = specialTable      ?? _defaultSpecialTable;
        #endregion
    }
}