using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FChatApi.Core;
using FChatApi.Objects;

namespace Plugins.Tokenizer
{
    public static class ParameterExtensions
    {
        public static bool TryGetAs<TKey>(this Dictionary<TKey,string> dict,TKey key,out int result) where TKey : notnull
		{
			if (dict.TryGetValue(key,out string value))
			{
				if (!string.IsNullOrWhiteSpace(value) && int.TryParse(value,out result))
					return true;
			}
			result = int.MinValue;
			return false;
		}

        public static bool TryGetAs<TKey>(this Dictionary<TKey,string> dict,TKey key,out float result) where TKey : notnull
		{
			if (dict.TryGetValue(key,out string value))
			{
				if (!string.IsNullOrWhiteSpace(value) && float.TryParse(value,out result))
					return true;
			}
			result = float.MinValue;
			return false;
		}

        public static bool TryGetAs<TKey>(this Dictionary<TKey,string> dict,TKey key,out User result) where TKey : notnull
		{
			if (dict.TryGetValue(key,out string value))
			{
				if (!string.IsNullOrWhiteSpace(value) && ApiConnection.Users.TrySingleByName(value,out result))
					return true;
			}
			result = null!;
			return false;
		}
    }
}