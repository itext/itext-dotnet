using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace iTextSharp.Layout
{
    internal static class LayoutExtensions
    {
        public static String JSubstring(this String str, int beginIndex, int endIndex)
        {
            return str.Substring(beginIndex, endIndex - beginIndex);
        }

        public static void JGetChars(this String str, int srcBegin, int srcEnd, char[] dst, int dstBegin)
        {
            str.CopyTo(srcBegin, dst, dstBegin, srcEnd - srcBegin);
        }

        public static void SetCharAt(this StringBuilder sb, int ind, char ch)
        {
            sb[ind] = ch;
        }

        public static TValue JRemove<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key)
        {
            TValue value;
            dictionary.TryGetValue(key, out value);
            dictionary.Remove(key);

            return value;
        }

        public static void AddAll<T>(this ICollection<T> c, IEnumerable<T> collectionToAdd)
        {
            foreach (T o in collectionToAdd)
            {
                c.Add(o);
            }
        }

        public static void AddAll<T>(this IList<T> list, int index, IList<T> c)
        {
            for (int i = c.Count - 1; i >= 0; i--)
            {
                list.Insert(index, c[i]);
            }
        }

        public static bool IsEmpty<T>(this IList<T> list)
        {
            return 0 == list.Count();
        }

        public static List<T> SubList<T>(this IList<T> list, int fromIndex, int toIndex)
        {
            return ((List<T>)list).GetRange(fromIndex, toIndex - fromIndex);
        }

        public static T JRemoveAt<T>(this IList<T> list, int index)
        {
            T value = list[index];
            list.RemoveAt(index);

            return value;
        }
    }
}
