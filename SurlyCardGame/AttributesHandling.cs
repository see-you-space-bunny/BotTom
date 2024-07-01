using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace SurlyCardGame.Attributes;

internal static class AttributeEnumExtensions
{
    static readonly Dictionary<Tuple<Type, Type>, Dictionary<Enum, Attribute>> _staticEnumAttributeLookup;

    static AttributeEnumExtensions()
    {
        _staticEnumAttributeLookup = [];

        // Put known enum types here to speed things up later
        ProcessEnumForAttribute<TAttribute>(typeof(CharacterStat));
        /// The type or namespace name 'TAttribute' could not be found (are you missing a using directive or an assembly reference?) (CS0246)
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
        foreach (var enumValue in Enum.GetValues(enumType))
        {
            _staticEnumAttributeLookup[key][enumValue] = (Attribute)
            /// Argument 1: cannot convert from 'object' to 'System.Enum' (CS1503)
                enumType.GetMember(enumValue.ToString())
                    .Where(member => member.MemberType == MemberTypes.Field)
                    .FirstOrDefault()
                    ?.GetCustomAttributes(typeof(TAttribute), false)
                    /// 'TAttribute' is a variable but is used like a type (CS0118)
                    ?.SingleOrDefault();
        }
    }

    public static TAttribute GetEnumAttribute<TAttribute,TEnum>(this TEnum @enum) where TAttribute : Attribute
    {
        var key = Tuple.Create(typeof(TEnum), typeof(TAttribute));
        if (!_staticEnumAttributeStrings.TryGetValue(key, out Dictionary<TEnum, Attribute> attributeLookup))
        /// The name '_staticEnumAttributeStrings' does not exist in the current context (CS0103)
        {
            ProcessEnumForAttribute(key);
            attributeLookup = _staticEnumAttributeStrings[key];
        }

        return (TAttribute)attributeLookup[enum];
        /// There is no argument given that corresponds to the required parameter 'key' of 'Dictionary<TEnum, Attribute>.this[TEnum]' (CS7036)
    }
}
/// Type or namespace definition, or end-of-file expected (CS1022)