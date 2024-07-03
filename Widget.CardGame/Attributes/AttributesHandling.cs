using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Widget.CardGame.Enums;

namespace Widget.CardGame.Attributes;

public static class AttributeEnumExtensions
{
    /// <summary>
    /// A dictionary indexed by EnumType + AttributeType pairs as Keys and
    /// EnumConstValue + AttributeValue Values.
    /// </summary>
    static readonly Dictionary<Tuple<Type, Type>, Dictionary<Enum, Attribute>> _staticEnumAttributeLookup;

    /// <summary>
    /// Gets the slow work out of the way during initialization, going through
    /// the known EnumType + AttributeType pairs and pulling the AttributeValues
    /// from them and storing them as the Value keyed to the Type + Type combination.
    /// </summary>
    static AttributeEnumExtensions()
    {
        _staticEnumAttributeLookup = [];

        // Put known enum types here to speed things up later
        ProcessEnumForAttribute<DescriptionAttribute>(typeof(CharacterStat));
        ProcessEnumForAttribute<StatAliasAttribute  >(typeof(CharacterStat));
        ProcessEnumForAttribute<StatGroupAttribute  >(typeof(CharacterStat));
        ProcessEnumForAttribute<DescriptionAttribute>(typeof(CharacterStatGroup));
        ProcessEnumForAttribute<DescriptionAttribute>(typeof(Command));
    }

    /// <summary>
    /// To ensure that the index is always correctly ordered as EnumType + AttributeType,
    /// this intermediary method forces the AttributeType to be passed as a typal argument,
    /// while receiving the EnumType as an ordinary parameter.
    /// </summary>
    /// <typeparam name="TAttribute">The AttributeType half of the Dictionary Key.</typeparam>
    /// <param name="enumType">The EnumType half of the Dictionary Key.</param>
    static void ProcessEnumForAttribute<TAttribute>(Type enumType) where TAttribute : Attribute
    {
        ProcessEnumForAttribute(Tuple.Create(enumType, typeof(TAttribute)));
    }

    /// <summary>
    /// Populates <c>_staticEnumAttributeLookup</c> using reflection, by iterating through each
    /// member of the passed EnumType + AttributeType Key-pair's EnumType and storing those
    /// Values in the inner Dictionary.
    /// </summary>
    /// <param name="key">An EnumType + AttributeType Key-pair</param>
    static void ProcessEnumForAttribute(Tuple<Type, Type> key)
    {
        if (_staticEnumAttributeLookup.ContainsKey(key))
            return;

        (Type enumType, Type TAttribute) = key;

        _staticEnumAttributeLookup[key] = new Dictionary<Enum, Attribute>();
        foreach (Enum enumValue in Enum.GetValues(enumType))
        {
            _staticEnumAttributeLookup[key][enumValue] = (Attribute)
                enumType.GetMember(enumValue.ToString())
                    .Where(member => member.MemberType == MemberTypes.Field)
                    .FirstOrDefault()
                    ?.GetCustomAttributes(TAttribute, false)
                    ?.SingleOrDefault()!;
        }
    }

    /// <summary>
    /// Retreives an instance of <c>TAttribute</c> from <c>_staticEnumAttributeLookup</c> that matches the specified <c>enumValue</c>.
    /// </summary>
    /// <typeparam name="TEnum">The EnumType being looked at.</typeparam>
    /// <typeparam name="TAttribute">The AttributeType of which we want to retrieve an instance of.</typeparam>
    /// <param name="enumValue">The specific const EnumValue from which we want to retrieve the data.</param>
    /// <returns>The instance of the specified <c>enumValue</c>'s <c>TAttribute</c> as passed in the TypeParameter.</returns>
    public static TAttribute GetEnumAttribute<TEnum,TAttribute>(this TEnum enumValue) where TEnum : Enum where TAttribute : Attribute
    {
        Tuple<Type, Type> key = Tuple.Create(typeof(TEnum), typeof(TAttribute));
        if (!_staticEnumAttributeLookup.TryGetValue(key, out Dictionary<Enum, Attribute> ?attributeLookup))
        {
            ProcessEnumForAttribute(key);
            attributeLookup = _staticEnumAttributeLookup[key];
        }

        return (TAttribute)attributeLookup[enumValue];
    }

    /// <summary>
    /// Retreives an instance of <c>TAttribute</c> that matches the specified <c>enumValue</c>, using reflection.
    /// 
    /// CS: "warning, uses reflection and is very slow"
    /// 
    /// Note-1: "Reflection" is a programming "looking at itself".
    /// Note-2: This should also work generically with non-Enum targets, provided the constraints are adjusted?
    /// </summary>
    /// <typeparam name="TEnum">The EnumType being looked at.</typeparam>
    /// <typeparam name="TAttribute">The AttributeType of which we want to retrieve an instance of.</typeparam>
    /// <param name="enumValue">The specific const EnumValue from which we want to retrieve the data.</param>
    /// <returns>The instance of the specified <c>enumValue</c>'s <c>TAttribute</c> as passed in the TypeParameter.</returns>
    public static TAttribute GetAttribute<TEnum, TAttribute>(this TEnum enumValue) where TEnum : Enum where TAttribute : Attribute
    {
        return (TAttribute)(
            typeof(TEnum)
            .GetMember(enumValue.ToString())
            .FirstOrDefault()
            ?.GetCustomAttributes(typeof(TAttribute), false)
            ?.FirstOrDefault()!
            );
    }
}