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
	private readonly Dictionary<Ability,float> _damageScales;
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

	#region csv-DamageScales
	public float DmgPerLVL	{ set => AddOrUpdate(_damageScales,Ability.Level		,value); }
	public float DmgPerPOW	{ set => AddOrUpdate(_damageScales,Ability.Power		,value); }
	public float DmgPerBOD	{ set => AddOrUpdate(_damageScales,Ability.Body			,value); }
	public float DmgPerREF	{ set => AddOrUpdate(_damageScales,Ability.Reflex		,value); }
	public float DmgPerFOC	{ set => AddOrUpdate(_damageScales,Ability.Focus		,value); }
	public float DmgPerWIL	{ set => AddOrUpdate(_damageScales,Ability.Will			,value); }
	public float DmgPerWIT	{ set => AddOrUpdate(_damageScales,Ability.Wit			,value); }
	public float DmgPerCHA	{ set => AddOrUpdate(_damageScales,Ability.Charm		,value); }
	public float DmgPerINT	{ set => AddOrUpdate(_damageScales,Ability.Integrity	,value); }
	public float DmgPerPRS	{ set => AddOrUpdate(_damageScales,Ability.Presence		,value); }
	public float DmgPerLUK	{ set => AddOrUpdate(_damageScales,Ability.Luck			,value); }
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
			_accuracyScales,
			_damageScales,
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
		_damageScales		= [];
        _carriedEffects		= [];
    }
}