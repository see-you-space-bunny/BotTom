using System.Collections.Immutable;
using FChatApi.Attributes;
using RoleplayingGame.Attributes;
using RoleplayingGame.Enums;

namespace RoleplayingGame.SheetComponents;

public class CharacterClassBuilder
{
	#region Properties
	ClassName _name;
	Dictionary<Resource,Dictionary<ResourceModifier,float>> _resourceModifiers { get; }
	Dictionary<Resource,Dictionary<Ability,float>> _resourceAbilityScales { get; }
	Dictionary<Ability,float> _abilityGrowth { get; }
    #endregion

	#region csv-TextName
	private string _textName;
	public string TextName {
		set {
			_textName = value;
			_name = Enum.Parse<ClassName>(value.Trim().Replace(" ","").Replace("_",""));
		}
	}	
    #endregion

	#region csv-Health
	public float HealthMinimumValue		{ set => AddOrUpdate(_resourceModifiers		[Resource.Health],ResourceModifier.MinimumValue	,value); }
	public float HealthBaseValue		{ set => AddOrUpdate(_resourceModifiers		[Resource.Health],ResourceModifier.BaseValue	,value); }
	public float HealthHardLimit		{ set => AddOrUpdate(_resourceModifiers		[Resource.Health],ResourceModifier.HardLimit	,value); }
	public float HealthScaleLevel		{ set => AddOrUpdate(_resourceAbilityScales	[Resource.Health],Ability.Level					,value); }
	public float HealthScalePower		{ set => AddOrUpdate(_resourceAbilityScales	[Resource.Health],Ability.Power					,value); }
	public float HealthScaleBody		{ set => AddOrUpdate(_resourceAbilityScales	[Resource.Health],Ability.Body					,value); }
	public float HealthScaleReflex		{ set => AddOrUpdate(_resourceAbilityScales	[Resource.Health],Ability.Reflex				,value); }
	public float HealthScaleFocus		{ set => AddOrUpdate(_resourceAbilityScales	[Resource.Health],Ability.Focus					,value); }
	public float HealthScaleWill		{ set => AddOrUpdate(_resourceAbilityScales	[Resource.Health],Ability.Will					,value); }
	public float HealthScaleWit			{ set => AddOrUpdate(_resourceAbilityScales	[Resource.Health],Ability.Wit					,value); }
	public float HealthScaleCharm		{ set => AddOrUpdate(_resourceAbilityScales	[Resource.Health],Ability.Charm					,value); }
	public float HealthScaleIntegrity	{ set => AddOrUpdate(_resourceAbilityScales	[Resource.Health],Ability.Integrity				,value); }
	public float HealthScalePresence	{ set => AddOrUpdate(_resourceAbilityScales	[Resource.Health],Ability.Presence				,value); }
	public float HealthScaleLuck		{ set => AddOrUpdate(_resourceAbilityScales	[Resource.Health],Ability.Luck					,value); }
	#endregion

	#region csv-Protection
	public float ProtectionMinimumValue		{ set => AddOrUpdate(_resourceModifiers		[Resource.Protection],ResourceModifier.MinimumValue	,value); }
	public float ProtectionBaseValue		{ set => AddOrUpdate(_resourceModifiers		[Resource.Protection],ResourceModifier.BaseValue	,value); }
	public float ProtectionHardLimit		{ set => AddOrUpdate(_resourceModifiers		[Resource.Protection],ResourceModifier.HardLimit	,value); }
	public float ProtectionScaleLevel		{ set => AddOrUpdate(_resourceAbilityScales	[Resource.Protection],Ability.Level					,value); }
	public float ProtectionScalePower		{ set => AddOrUpdate(_resourceAbilityScales	[Resource.Protection],Ability.Power					,value); }
	public float ProtectionScaleBody		{ set => AddOrUpdate(_resourceAbilityScales	[Resource.Protection],Ability.Body					,value); }
	public float ProtectionScaleReflex		{ set => AddOrUpdate(_resourceAbilityScales	[Resource.Protection],Ability.Reflex				,value); }
	public float ProtectionScaleFocus		{ set => AddOrUpdate(_resourceAbilityScales	[Resource.Protection],Ability.Focus					,value); }
	public float ProtectionScaleWill		{ set => AddOrUpdate(_resourceAbilityScales	[Resource.Protection],Ability.Will					,value); }
	public float ProtectionScaleWit			{ set => AddOrUpdate(_resourceAbilityScales	[Resource.Protection],Ability.Wit					,value); }
	public float ProtectionScaleCharm		{ set => AddOrUpdate(_resourceAbilityScales	[Resource.Protection],Ability.Charm					,value); }
	public float ProtectionScaleIntegrity	{ set => AddOrUpdate(_resourceAbilityScales	[Resource.Protection],Ability.Integrity				,value); }
	public float ProtectionScalePresence	{ set => AddOrUpdate(_resourceAbilityScales	[Resource.Protection],Ability.Presence				,value); }
	public float ProtectionScaleLuck		{ set => AddOrUpdate(_resourceAbilityScales	[Resource.Protection],Ability.Luck					,value); }
	#endregion

	#region csv-Evasion
	public float EvasionMinimumValue	{ set => AddOrUpdate(_resourceModifiers		[Resource.Evasion],ResourceModifier.MinimumValue	,value); }
	public float EvasionBaseValue		{ set => AddOrUpdate(_resourceModifiers		[Resource.Evasion],ResourceModifier.BaseValue		,value); }
	public float EvasionHardLimit		{ set => AddOrUpdate(_resourceModifiers		[Resource.Evasion],ResourceModifier.HardLimit		,value); }
	public float EvasionScaleLevel		{ set => AddOrUpdate(_resourceAbilityScales	[Resource.Evasion],Ability.Level					,value); }
	public float EvasionScalePower		{ set => AddOrUpdate(_resourceAbilityScales	[Resource.Evasion],Ability.Power					,value); }
	public float EvasionScaleBody		{ set => AddOrUpdate(_resourceAbilityScales	[Resource.Evasion],Ability.Body						,value); }
	public float EvasionScaleReflex		{ set => AddOrUpdate(_resourceAbilityScales	[Resource.Evasion],Ability.Reflex					,value); }
	public float EvasionScaleFocus		{ set => AddOrUpdate(_resourceAbilityScales	[Resource.Evasion],Ability.Focus					,value); }
	public float EvasionScaleWill		{ set => AddOrUpdate(_resourceAbilityScales	[Resource.Evasion],Ability.Will						,value); }
	public float EvasionScaleWit		{ set => AddOrUpdate(_resourceAbilityScales	[Resource.Evasion],Ability.Wit						,value); }
	public float EvasionScaleCharm		{ set => AddOrUpdate(_resourceAbilityScales	[Resource.Evasion],Ability.Charm					,value); }
	public float EvasionScaleIntegrity	{ set => AddOrUpdate(_resourceAbilityScales	[Resource.Evasion],Ability.Integrity				,value); }
	public float EvasionScalePresence	{ set => AddOrUpdate(_resourceAbilityScales	[Resource.Evasion],Ability.Presence					,value); }
	public float EvasionScaleLuck		{ set => AddOrUpdate(_resourceAbilityScales	[Resource.Evasion],Ability.Luck						,value); }
	#endregion

	#region csv-AbilityGrowth
	public float AbilityGrowthLevel		{ set => AddOrUpdate(_abilityGrowth,Ability.Level		,value); }
	public float AbilityGrowthPower		{ set => AddOrUpdate(_abilityGrowth,Ability.Power		,value); }
	public float AbilityGrowthBody		{ set => AddOrUpdate(_abilityGrowth,Ability.Body		,value); }
	public float AbilityGrowthReflex	{ set => AddOrUpdate(_abilityGrowth,Ability.Reflex		,value); }
	public float AbilityGrowthFocus		{ set => AddOrUpdate(_abilityGrowth,Ability.Focus		,value); }
	public float AbilityGrowthWill		{ set => AddOrUpdate(_abilityGrowth,Ability.Will		,value); }
	public float AbilityGrowthWit		{ set => AddOrUpdate(_abilityGrowth,Ability.Wit			,value); }
	public float AbilityGrowthCharm		{ set => AddOrUpdate(_abilityGrowth,Ability.Charm		,value); }
	public float AbilityGrowthIntegrity	{ set => AddOrUpdate(_abilityGrowth,Ability.Integrity	,value); }
	public float AbilityGrowthPresence	{ set => AddOrUpdate(_abilityGrowth,Ability.Presence	,value); }
	public float AbilityGrowthLuck		{ set => AddOrUpdate(_abilityGrowth,Ability.Luck		,value); }
	#endregion

	private static void AddOrUpdate<TKey,TValue>(Dictionary<TKey,TValue> dictionary,TKey key,TValue value) where TKey : struct
	{
		if (!dictionary.TryAdd(key,value))
			dictionary[key] = value;
	}
	public CharacterClass Build() => new(
		_name,
		_resourceModifiers,
		_resourceAbilityScales,
		_abilityGrowth
	);

	public CharacterClassBuilder WithName(string name)
	{
		_name = Enum.Parse<ClassName>(name,true);
		return this;
	}

	public CharacterClassBuilder WithName(ClassName name)
	{
		_name = name;
		return this;
	}

	public CharacterClassBuilder WithResourceModifier(Resource resource,ResourceModifier resourceModifier,float value)
	{
		_resourceModifiers[resource][resourceModifier]	= value;
		return this;
	}

	public CharacterClassBuilder WithResourceAbilityScales(Resource resource,Ability ability,float value)
	{
		_resourceAbilityScales[resource][ability]	= value;
		return this;
	}

	public CharacterClassBuilder()
	{
		_name = ClassName.Nobody;
		_textName = string.Empty;
		_abilityGrowth			= Enum.GetValues(typeof(Ability)).Cast<Ability>().Where(a=>a != Ability.None).ToDictionary(a=>a,a=>0.0f);
		_resourceAbilityScales	= Enum.GetValues(typeof(Resource)).Cast<Resource>().Where(r=>r != Resource.None).ToDictionary(li=>li,li=>new Dictionary<Ability, float>());
		foreach (var abilityScalesDictionary in _resourceAbilityScales.Values)
		{
			foreach (Ability ability in Enum.GetValues(typeof(Ability)).Cast<Ability>().Where(a=>a != Ability.None))
			{
				abilityScalesDictionary.Add(ability,0.0f);
			}
		}

		_resourceModifiers		= Enum.GetValues(typeof(Resource)).Cast<Resource>().Where(r=>r != Resource.None).ToDictionary(li=>li,li=>new Dictionary<ResourceModifier, float>());
		foreach (var modifierDictionary in _resourceModifiers.Values)
		{
			foreach (ResourceModifier modifier in Enum.GetValues(typeof(ResourceModifier)).Cast<ResourceModifier>().Where(a=>a != ResourceModifier.None))
			{
				modifierDictionary.Add(modifier,modifier.GetEnumAttribute<ResourceModifier,DefaultModifierAttribute>().Value);
			}
		}
	}
}
