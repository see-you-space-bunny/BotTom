using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Widget.CardGame.Enums;

namespace Widget.CardGame.Attributes;

internal static class AttributeEnumExtensions
{
    static readonly Dictionary<Tuple<Type, Type>, Dictionary<Enum, Attribute>> _staticEnumAttributeLookup;

    static AttributeEnumExtensions()
    {
        _staticEnumAttributeLookup = [];

        // Put known enum types here to speed things up later
        ProcessEnumForAttribute<DescriptionAttribute>(typeof(CharacterStat));
    }

    static void ProcessEnumForAttribute<TAttribute>(Type enumType) where TAttribute : Attribute
    {
        ProcessEnumForAttribute(Tuple.Create(enumType, typeof(TAttribute)));
    }

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

    /**
    public static TAttribute GetEnumAttribute<TAttribute,TEnum>(this TEnum @enum) where TAttribute : Attribute where TEnum : Enum
    {
        Tuple<Type, Type> key = Tuple.Create(typeof(TEnum), typeof(TAttribute));
        if (!_staticEnumAttributeLookup.TryGetValue(key, out Dictionary<Enum, Attribute> attributeLookup))
        /// Converting null literal or possible null value to non-nullable type.(CS8600)
        {
            ProcessEnumForAttribute(key);
            attributeLookup = _staticEnumAttributeLookup[key];
        }

        return (TAttribute)attributeLookup[@enum];
        /// Argument 1: cannot convert from 'TEnum' to 'System.Enum'(CS1503)
    }
    */
}
/// Type or namespace definition, or end-of-file expected (CS1022)