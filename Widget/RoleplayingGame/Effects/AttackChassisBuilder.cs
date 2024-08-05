using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RoleplayingGame.Enums;
using RoleplayingGame.Objects;

namespace RoleplayingGame.Effects;

public class AttackChassisBuilder
{
	private AttackType _attackType;
	private readonly Dictionary<Ability,float> _accuracyScales;
	private readonly Dictionary<Ability,float> _impactScales;
	private readonly Dictionary<Ability,float> _harmScales;
    private readonly List<StatusEffect> _carriedEffects;

	#region csv-Name
	private string _textName;
    public string Name		{
		private get => _textName;
		set {
			_textName	= value;
			_attackType	= Enum.Parse<AttackType>(value.Trim().Replace(" ","").Replace("_",""));
		}
	}
	#endregion

	#region csv-BaseValues
    public float Harm		{ private get; set; }
    public float Impact		{ private get; set; }
    public float Accuracy	{ private get; set; }
	#endregion

	#region csv-HarmScales
	public float HrmPerLVL	{ set => AddOrUpdate(_harmScales,Ability.Level			,value); }
	public float HrmPerPOW	{ set => AddOrUpdate(_harmScales,Ability.Power			,value); }
	public float HrmPerBOD	{ set => AddOrUpdate(_harmScales,Ability.Body			,value); }
	public float HrmPerREF	{ set => AddOrUpdate(_harmScales,Ability.Reflex			,value); }
	public float HrmPerFOC	{ set => AddOrUpdate(_harmScales,Ability.Focus			,value); }
	public float HrmPerWIL	{ set => AddOrUpdate(_harmScales,Ability.Will			,value); }
	public float HrmPerWIT	{ set => AddOrUpdate(_harmScales,Ability.Wit			,value); }
	public float HrmPerCHA	{ set => AddOrUpdate(_harmScales,Ability.Charm			,value); }
	public float HrmPerINT	{ set => AddOrUpdate(_harmScales,Ability.Integrity		,value); }
	public float HrmPerPRS	{ set => AddOrUpdate(_harmScales,Ability.Presence		,value); }
	public float HrmPerLUK	{ set => AddOrUpdate(_harmScales,Ability.Luck			,value); }
	#endregion

	#region csv-ImpactScales
	public float ImpPerLVL	{ set => AddOrUpdate(_impactScales,Ability.Level		,value); }
	public float ImpPerPOW	{ set => AddOrUpdate(_impactScales,Ability.Power		,value); }
	public float ImpPerBOD	{ set => AddOrUpdate(_impactScales,Ability.Body			,value); }
	public float ImpPerREF	{ set => AddOrUpdate(_impactScales,Ability.Reflex		,value); }
	public float ImpPerFOC	{ set => AddOrUpdate(_impactScales,Ability.Focus		,value); }
	public float ImpPerWIL	{ set => AddOrUpdate(_impactScales,Ability.Will			,value); }
	public float ImpPerWIT	{ set => AddOrUpdate(_impactScales,Ability.Wit			,value); }
	public float ImpPerCHA	{ set => AddOrUpdate(_impactScales,Ability.Charm		,value); }
	public float ImpPerINT	{ set => AddOrUpdate(_impactScales,Ability.Integrity	,value); }
	public float ImpPerPRS	{ set => AddOrUpdate(_impactScales,Ability.Presence		,value); }
	public float ImpPerLUK	{ set => AddOrUpdate(_impactScales,Ability.Luck			,value); }
	#endregion

	#region csv-AccuracyScales
	public float AccPerLVL	{ set => AddOrUpdate(_accuracyScales,Ability.Level		,value); }
	public float AccPerPOW	{ set => AddOrUpdate(_accuracyScales,Ability.Power		,value); }
	public float AccPerBOD	{ set => AddOrUpdate(_accuracyScales,Ability.Body		,value); }
	public float AccPerREF	{ set => AddOrUpdate(_accuracyScales,Ability.Reflex		,value); }
	public float AccPerFOC	{ set => AddOrUpdate(_accuracyScales,Ability.Focus		,value); }
	public float AccPerWIL	{ set => AddOrUpdate(_accuracyScales,Ability.Will		,value); }
	public float AccPerWIT	{ set => AddOrUpdate(_accuracyScales,Ability.Wit		,value); }
	public float AccPerCHA	{ set => AddOrUpdate(_accuracyScales,Ability.Charm		,value); }
	public float AccPerINT	{ set => AddOrUpdate(_accuracyScales,Ability.Integrity	,value); }
	public float AccPerPRS	{ set => AddOrUpdate(_accuracyScales,Ability.Presence	,value); }
	public float AccPerLUK	{ set => AddOrUpdate(_accuracyScales,Ability.Luck		,value); }
	#endregion


	private static void AddOrUpdate<TKey,TValue>(Dictionary<TKey,TValue> dictionary,TKey key,TValue value) where TKey : struct
	{
		if (!dictionary.TryAdd(key,value))
			dictionary[key] = value;
	}

    public AttackChassis Build()
    {
        return new AttackChassis(
			_attackType,
			_textName,
			Harm,
			Impact,
			Accuracy,
			_harmScales,
			_impactScales,
			_accuracyScales,
            [.. _carriedEffects]
        );
    }

    public AttackChassisBuilder()
    {
		_attackType			= AttackType.None;
		_textName			= default!;
        Harm				= 0.0f;
        Impact				= 0.0f;
        Accuracy			= 0.0f;
		_accuracyScales		= [];
		_impactScales		= [];
		_harmScales			= [];
        _carriedEffects		= [];
    }
}