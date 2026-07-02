using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using iText.Commons.Utils;
using iText.Commons.Utils.Collections;

namespace iText.Commons.Internal.Runtime
{
    public static class CommonsRuntime
    {
        public static int JRead(this Stream stream, byte[] buffer)
        {
            int size = stream.Read(buffer, 0, buffer.Length);
            return size == 0 ? -1 : size;
        }

        public static int JRead(this Stream stream, byte[] buffer, int offset, int count)
        {
            int result = stream.Read(buffer, offset, count);
            return result == 0 ? -1 : result;
        }

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

        public static V GetOrDefault<K, V>(this IDictionary<K, V> dict, K key, V defaultValue)
        {
            if (!dict.ContainsKey(key))
            {
                return defaultValue;
            }

            return dict[key];
        }

        public static float NextFloat(this Random random)
        {
            double mantissa = random.NextDouble();
            double exponent = Math.Pow(2.0, random.Next(-126, 128));
            if (mantissa < 0 || exponent < 0)
            {
                int a = 5;
            }

            float val = (float)(mantissa * exponent);
            if (val < 0)
            {
                int b = 6;
            }

            return (float)(mantissa * exponent);
        }

        public static bool NextBoolean(this Random random)
        {
            return random.NextDouble() > 0.5;
        }

        public static bool After(this DateTime date, DateTime when)
        {
            return date.CompareTo(when) > 0;
        }

        public static bool Before(this DateTime date, DateTime when)
        {
            return date.CompareTo(when) < 0;
        }

        public static IEnumerable<T> Sorted<T>(this IEnumerable<T> source, Comparison<T> comp)
        {
            return source.OrderBy(x => x, Comparer<T>.Create(comp));
        }

        public static int LastIndexOf<T>(this IList<T> list, T item)
        {
            if (list is List<T>)
            {
                return ((List<T>)list).LastIndexOf(item);
            }

            for (int index = list.Count - 1; index >= 0; --index)
            {
                if (Equals(list[index], item))
                {
                    return index;
                }
            }

            return -1;
        }


        public static KeyValuePair<K, V>? HigherEntry<K, V>(this SortedDictionary<K, V> dict, K key)
        {
            List<K> list = dict.Keys.ToList();
            int index = list.BinarySearch(key, dict.Comparer);
            if (index < 0)
            {
                index = ~index;
            }
            else
            {
                index++;
            }

            if (index == list.Count)
            {
                return null;
            }
            else
            {
                return new KeyValuePair<K, V>(list[index], dict[list[index]]);
            }
        }

        public static T JRemoveFirst<T>(this LinkedList<T> list)
        {
            T value = list.First.Value;
            list.RemoveFirst();

            return value;
        }

        public static void JGetChars(this String str, int srcBegin, int srcEnd, char[] dst, int dstBegin)
        {
            str.CopyTo(srcBegin, dst, dstBegin, srcEnd - srcBegin);
        }

        public static String JSubstring(this String str, int beginIndex, int endIndex)
        {
            return str.Substring(beginIndex, endIndex - beginIndex);
        }


        public static TValue JRemove<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key)
        {
            TValue value;
            dictionary.TryGetValue(key, out value);
            dictionary.Remove(key);

            return value;
        }

        public static TValue Get<TKey, TValue>(this IDictionary<TKey, TValue> col, TKey key) {
            TValue value = default(TValue);
            if (key != null) {
                col.TryGetValue(key, out value);
            }
            return value;
        }

        public static TValue Put<TKey, TValue>(this IDictionary<TKey, TValue> col, TKey key, TValue value)
        {
            TValue oldVal = col.Get(key);
            col[key] = value;
            return oldVal;
        }

        public static void AddAll<TKey, TValue>(this IDictionary<TKey, TValue> c,
            IDictionary<TKey, TValue> collectionToAdd)
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

        public static bool IsEmpty<T>(this ICollection<T> c)
        {
            return c.Count == 0;
        }

        public static void AddAll<T>(this IList<T> list, IEnumerable<T> c)
        {
            foreach (T item in c)
            {
                list.Add(item);
            }
        }


        public static void AddAll<T>(this Stack<T> c, IEnumerable<T> collectionToAdd)
        {
            foreach (T o in collectionToAdd)
            {
                c.Push(o);
            }
        }


        public static bool Add<T>(this LinkedList<T> list, T elem)
        {
            list.AddLast(elem);
            return true;
        }


        public static bool Contains<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key)
        {
            return dictionary.ContainsKey(key);
        }


        public static TValue Get<TKey, TValue>(this ConditionalWeakTable<TKey, TValue> table, TKey key) where
            TKey : class
            where TValue : class
        {
            TValue value = default(TValue);
            if (key != null)
            {
                table.TryGetValue(key, out value);
            }

            return value;
        }

        public static TValue Put<TKey, TValue>(this ConditionalWeakTable<TKey, TValue> table, TKey key, TValue value)
            where TKey : class where TValue : class
        {
            TValue oldVal = table.Get(key);
            if (oldVal != null)
            {
                table.Remove(key);
            }

            table.Add(key, value);
            return oldVal;
        }


        public static void AddAll<T>(this ICollection<T> c, IEnumerable<T> collectionToAdd)
        {
            foreach (T o in collectionToAdd)
            {
                c.Add(o);
            }
        }

        public static T JRemoveAt<T>(this IList<T> list, int index)
        {
            T value = list[index];
            list.RemoveAt(index);

            return value;
        }


        public static void Add<T>(this IList<T> list, int index, T elem)
        {
            list.Insert(index, elem);
        }

        public static bool ContainsKey<TKey, TValue>(this ConditionalWeakTable<TKey, TValue> table, TKey key)
            where TKey : class where TValue : class
        {
            return table.Get(key) != null;
        }

        public static void Write(this Stream stream, int value)
        {
            stream.WriteByte((byte)value);
        }

        public static void Put(this IDictionary col, Object key, Object value)
        {
            if (key != null)
            {
                col[key] = value;
            }
        }

        public static int JRead(this BinaryReader stream, byte[] buffer, int offset, int count)
        {
            int result = stream.Read(buffer, offset, count);
            return result == 0 ? -1 : result;
        }


        public static ICollection<T> SubList<T>(this ICollection<T> collection, int fromIndex, int toIndex)
        {
            return collection.ToList().GetRange(fromIndex, toIndex - fromIndex);
        }

        public static List<T> SubList<T>(this IList<T> list, int fromIndex, int toIndex)
        {
            if (list is SingletonList<T>)
            {
                if (fromIndex == 0 && toIndex >= 1)
                {
                    return new List<T>(list);
                }
                else
                {
                    return new List<T>();
                }
            }

            if (list is ReadOnlyCollection<T>)
            {
                List<T> copy = new List<T>(list);
                return copy.GetRange(fromIndex, toIndex - fromIndex);
            }

            return ((List<T>)list).GetRange(fromIndex, toIndex - fromIndex);
        }


        public static bool Matches(this String str, String regex)
        {
            return Regex.IsMatch(str, "^" + regex + "$");
        }


        public static long Seek(this FileStream fs, long offset)
        {
            return fs.Seek(offset, SeekOrigin.Begin);
        }


        public static void GetChars(this StringBuilder sb, int srcBegin, int srcEnd, char[] dst, int dstBegin)
        {
            sb.CopyTo(srcBegin, dst, dstBegin, srcEnd - srcBegin);
        }

        public static T[] ToArray<T>(this ICollection<T> col, T[] toArray)
        {
            T[] r;
            int colSize = col.Count;
            if (colSize <= toArray.Length)
            {
                col.CopyTo(toArray, 0);
                if (colSize != toArray.Length)
                {
                    toArray[colSize] = default(T);
                }

                r = toArray;
            }
            else
            {
                r = new T[colSize];
                col.CopyTo(r, 0);
            }

            return r;
        }

        public static String JSubstring(this StringBuilder sb, int beginIndex, int endIndex)
        {
            return sb.ToString(beginIndex, endIndex - beginIndex);
        }

        public static void JReset(this MemoryStream stream)
        {
            // previously we used stream.Position = 0, but this is not the same 
            // as the java ByteArrayOutputStream.reset() method, which also clears the buffer.
            stream.SetLength(0);
        }

        public static Stack<T> Clone<T>(this Stack<T> stack)
        {
            return new Stack<T>(new Stack<T>(stack)); // create stack twice to retain the original order
        }

        public static bool ContainsAll<T>(this ICollection<T> thisC, ICollection<T> otherC)
        {
            foreach (T e in otherC)
            {
                if (!thisC.Contains(e))
                {
                    return false;
                }
            }

            return true;
        }

        public static String ReplaceFirst(this String input, String pattern, String replacement)
        {
            var regex = new Regex(pattern);
            return regex.Replace(input, replacement, 1);
        }


        public static bool IsEmpty<T1, T2>(this ICollection<KeyValuePair<T1, T2>> collection)
        {
            return collection.Count == 0;
        }


        public static bool IsEmpty<T>(this Stack<T> collection)
        {
            return collection.Count == 0;
        }

        public static bool RemoveAll<T>(this IList<T> list, ICollection<T> c)
        {
            return BatchRemove(list, c, false);
        }

        // Removes from this list all of its elements that are not contained in the specified collection.
        public static bool RetainAll<T>(this IList<T> list, ICollection<T> c)
        {
            return BatchRemove(list, c, true);
        }

        private static bool BatchRemove<T>(IList<T> list, ICollection<T> c, bool complement)
        {
            bool modified = false;
            int j = 0;
            for (int i = 0; i < list.Count; ++i)
            {
                if (c.Contains(list[i]) == complement)
                {
                    list[j++] = list[i];
                }
            }

            if (j != list.Count)
            {
                modified = true;
                for (int i = list.Count - 1; i >= j; --i)
                {
                    list.RemoveAt(i);
                }
            }

            return modified;
        }

        public static bool RemoveAll<T>(this ICollection<T> toClean, ICollection<T> c)
        {
            bool modified = false;
            foreach (T element in c)
            {
                bool anythingToRemove;
                do
                {
                    anythingToRemove = toClean.Remove(element);
                    modified |= anythingToRemove;
                } while (anythingToRemove);
            }

            return modified;
        }

        public static bool RetainAll<T>(this ICollection<T> toClean, ICollection<T> c)
        {
            IList<T> toRemove = new List<T>();
            foreach (T element in toClean)
            {
                if (!c.Contains(element))
                {
                    toRemove.Add(element);
                }
            }

            return toClean.RemoveAll(toRemove);
        }

        public static T PollFirst<T>(this SortedSet<T> set)
        {
            T item = set.First();
            set.Remove(item);
            return item;
        }


        public static void SetCharAt(this StringBuilder builder, int index, char ch)
        {
            builder[index] = ch;
        }

        public static T[] ToArray<T>(this ICollection<T> col)
        {
            T[] result = new T[col.Count];
            return col.ToArray<T>(result);
        }

        public static bool EqualsIgnoreCase(this String str, String anotherString)
        {
            return String.Equals(str, anotherString, StringComparison.OrdinalIgnoreCase);
        }


        public static long Skip(this Stream s, long n)
        {
            s.Seek(n, SeekOrigin.Current);
            return n;
        }

        public static byte[] GetBytes(this String str, Encoding encoding)
        {
            return encoding.GetBytes(str);
        }


        public static void Write(this Stream stream, byte[] buffer)
        {
            stream.Write(buffer, 0, buffer.Length);
        }


        public static byte[] GetBytes(this String str, String encoding)
        {
            return EncodingUtil.GetEncoding(encoding).GetBytes(str);
        }


        public static byte[] GetBytes(this String str)
        {
            return System.Text.Encoding.UTF8.GetBytes(str);
        }

        public static Assembly GetAssembly(this Type type)
        {
#if !NETSTANDARD2_0
            return type.Assembly;
#else
            return type.GetTypeInfo().Assembly;
#endif
        }

        public static String Name(this Encoding e)
        {
            return e.WebName.ToUpperInvariant();
        }

        public static String DisplayName(this Encoding e)
        {
            return e.WebName.ToUpperInvariant();
        }

        public static bool RegionMatches(this string s, bool ignoreCase, int toffset, String other, int ooffset,
            int len)
        {
            return 0 == String.Compare(s, toffset, other, ooffset, len,
                ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal);
        }

        public static bool StartsWith(this string s, string prefix, int pos)
        {
            int to = pos;
            int po = 0;
            int pc = prefix.Length;
            if ((pos < 0) || (pos > s.Length - pc))
            {
                return false;
            }

            while (--pc >= 0)
            {
                if (s[to++] != prefix[po++])
                {
                    return false;
                }
            }

            return true;
        }

        public static void ReadFully(this FileStream stream, byte[] bytes)
        {
            stream.Read(bytes, 0, bytes.Length);
        }

        public static T JRemove<T>(this LinkedList<T> list)
        {
            T head = list.First.Value;
            list.RemoveFirst();
            return head;
        }


        public static void ForEach<T>(this IList<T> collection, Action<T> action)
        {
            foreach (T element in collection)
            {
                action(element);
            }
        }
        
        

        public static void RemoveIf<T>(this ICollection<T> collection, Func<T, bool> predicate) {
            T element;
            for (int i = 0; i < collection.Count; i++) {
                element = collection.ElementAt(i);
                if (predicate(element)) {
                    collection.Remove(element);
                    i--;
                }
            }
        }
        
       
        public static bool RemoveIf<T>(this IList<T> list, Func<T, bool> predicate)
        {
            bool elementRemoved = false;
            for (int i = 0; i < list.Count; i++) {
                T element = list.ElementAt(i);
                if (predicate(element))
                {
                    list.RemoveAt(i);
                    elementRemoved = true;
                    i--;
                }
            }
            return elementRemoved;
        } 

        public static T JGetFirst<T>(this LinkedList<T> list)
        {
            return list.First.Value;
        }

        public static T JGetLast<T>(this LinkedList<T> list)
        {
            return list.Last.Value;
        }


        public static StringBuilder JAppend(this StringBuilder sb, String str, int begin, int end)
        {
            return sb.Append(str, begin, end - begin);
        }

        public static String ToExternalForm(this Uri u)
        {
            return u.AbsoluteUri;
        }

        public static bool CanEncode(this Encoding encoding, char c)
        {
            return encoding.CanEncode(c.ToString());
        }

        public static bool CanEncode(this Encoding encoding, String chars)
        {
            byte[] src = Encoding.Unicode.GetBytes(chars);
            return encoding.CanEncode(src);
        }

        public static bool CanEncode(this Encoding encoding, byte[] src)
        {
            try
            {
                byte[] dest = Encoding.Convert(Encoding.Unicode,
                    EncodingUtil.GetEncoding(encoding.CodePage, new EncoderExceptionFallback(),
                        new DecoderExceptionFallback()), src);
            }
            catch (EncoderFallbackException)
            {
                return false;
            }

            return true;
        }


        public static Uri ToUri(this String s)
        {
            return new Uri(s);
        }

        public static FileInfo ToFile(this String s)
        {
            return new FileInfo(s);
        }

        public static Uri ToUrl(this Uri u)
        {
            return u;
        }

        public static int CodePointAt(this String str, int index)
        {
            return char.ConvertToUtf32(str, index);
        }

        public static StringBuilder AppendCodePoint(this StringBuilder sb, int codePoint)
        {
            return sb.Append(char.ConvertFromUtf32(codePoint));
        }


        public static StringBuilder Delete(this StringBuilder sb, int beginIndex, int endIndex)
        {
            return sb.Remove(beginIndex, endIndex - beginIndex);
        }


#if !NETSTANDARD2_0
        public static Attribute GetCustomAttributeRuntime(this Assembly assembly, Type attributeType)
        {
            object[] customAttributes = Assembly.GetExecutingAssembly().GetCustomAttributes(attributeType, false);
            if (customAttributes.Length > 0 && customAttributes[0] is Attribute)
            {
                return customAttributes[0] as Attribute;
            }
            else
            {
                return null;
            }
        }
#endif
    }
}
