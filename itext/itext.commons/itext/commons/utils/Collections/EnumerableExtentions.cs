using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace iText.Commons.Utils.Collections
{
    internal static class EnumerableExtentions
    {
        public static IEnumerable<T> Sorted<T>(this  IEnumerable<T> source, Comparison<T> comp)
        {
            return source.OrderBy(x => x, Comparer<T>.Create(comp));
        }
        
        public static IEnumerable<T> Sorted<T>(this  IEnumerable<T> source)
        {
            return source.OrderBy(x => x);
        }
    }
}
