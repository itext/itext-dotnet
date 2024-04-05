using System;
using System.Collections.Generic;

namespace iText.Commons.Utils.Collections
{
    public static class MapExtensions
    {
        public static V ComputeIfAbsent<K, V>(this IDictionary<K, V> dict, K key, Func<K, V> calculator)
        {
            if (!dict.ContainsKey(key))
            {
                var value = calculator(key);
                dict[key] = value;
                return value;
            }

            return dict[key];
        }
    }
}