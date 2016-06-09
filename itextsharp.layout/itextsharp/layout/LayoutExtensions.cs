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

        public static void AddAll<TKey, TValue>(this IDictionary<TKey, TValue> c, IDictionary<TKey, TValue> collectionToAdd)
        {
            foreach (KeyValuePair<TKey, TValue> pair in collectionToAdd)
            {
                c[pair.Key] = pair.Value;
            }
        }

        public static void AddAll<T>(this IList<T> list, int index, IList<T> c)
        {
            for (int i = c.Count - 1; i >= 0; i--)
            {
                list.Insert(index, c[i]);
            }
        }

        public static void Add<T>(this IList<T> list, int index, T elem) {
            list.Insert(index, elem);
        }

        public static bool IsEmpty<T>(this IList<T> list)
        {
            return 0 == list.Count();
        }

        public static bool IsEmpty<T>(this Queue<T> queue)
        {
            return 0 == queue.Count();
        }

        public static KeyValuePair<K, V> HigherEntry<K, V>(this SortedDictionary<K, V> dict, K key) 
        {
            List<K> list = dict.Keys.ToList();
            int index = list.BinarySearch(key, dict.Comparer);
            if (index < 0) {
                index = ~index;
            }
            else {
                index++;
            }
            if (index == list.Count) 
            {
                return default(KeyValuePair<K, V>);
            }
            else 
            {
                return new KeyValuePair<K, V>(list[index], dict[list[index]]);
            }
        } 

        public static T JRemoveAt<T>(this IList<T> list, int index)
        {
            T value = list[index];
            list.RemoveAt(index);

            return value;
        }

        public static TValue Get<TKey, TValue>(this IDictionary<TKey, TValue> col, TKey key)
        {
            TValue value = default(TValue);
            if (key != null)
            {
                col.TryGetValue(key, out value);
            }

            return value;
        }

        public static List<T> SubList<T>(this IList<T> list, int fromIndex, int toIndex) {
            return ((List<T>) list).GetRange(fromIndex, toIndex - fromIndex);
        }

        public static String[] Split(this String str, String regex)
        {
            return str.Split(regex.ToCharArray());
        }
    }
}
