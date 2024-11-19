/*
    This file is part of the iText (R) project.
    Copyright (c) 1998-2024 Apryse Group NV
    Authors: Apryse Software.

    This program is offered under a commercial and under the AGPL license.
    For commercial licensing, contact us at https://itextpdf.com/sales.  For AGPL licensing, see below.

    AGPL licensing:
    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU Affero General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU Affero General Public License for more details.

    You should have received a copy of the GNU Affero General Public License
    along with this program.  If not, see <https://www.gnu.org/licenses/>.
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using iText.Commons.Utils;
using iText.Commons.Utils.Collections;
//\cond DO_NOT_DOCUMENT
internal static class KernelExtensions {
    public static String JSubstring(this String str, int beginIndex, int endIndex) {
        return str.Substring(beginIndex, endIndex - beginIndex);
    }

    public static String JSubstring(this StringBuilder sb, int beginIndex, int endIndex) {
        return sb.ToString(beginIndex, endIndex - beginIndex);
    }

    public static bool EqualsIgnoreCase(this String str, String anotherString) {
        return String.Equals(str, anotherString, StringComparison.OrdinalIgnoreCase);
    }

    public static void JReset(this MemoryStream stream) {
        stream.SetLength(0);
    }

    public static void Write(this Stream stream, int value) {
        stream.WriteByte((byte) value);
    }

    public static int Read(this Stream stream) {
        return stream.ReadByte();
    }

    public static int Read(this Stream stream, byte[] buffer) {
        int size = stream.Read(buffer, 0, buffer.Length);
        return size == 0 ? -1 : size;
    }

    public static int JRead(this Stream stream, byte[] buffer, int offset, int count) {
        int result = stream.Read(buffer, offset, count);
        return result == 0 ? -1 : result;
    }

    public static int JRead(this BinaryReader stream, byte[] buffer, int offset, int count) {
        int result = stream.Read(buffer, offset, count);
        return result == 0 ? -1 : result;
    }

    public static void Write(this Stream stream, byte[] buffer) {
        stream.Write(buffer, 0, buffer.Length);
    }

    public static byte[] GetBytes(this String str) {
        return System.Text.Encoding.UTF8.GetBytes(str);
    }

    public static byte[] GetBytes(this String str, String encoding) {
        return EncodingUtil.GetEncoding(encoding).GetBytes(str);
    }

    public static byte[] GetBytes(this String str, Encoding encoding) {
        return encoding.GetBytes(str);
    }

    public static long Seek(this FileStream fs, long offset) {
        return fs.Seek(offset, SeekOrigin.Begin);
    }

    public static long Skip(this Stream s, long n) {
        s.Seek(n, SeekOrigin.Current);
        return n;
    }

	public static List<T> SubList<T>(this IList<T> list, int fromIndex, int toIndex) {
		if (list is SingletonList<T>) {
			if (fromIndex == 0 && toIndex >= 1) {
				return new List<T>(list);
			} else {
				return new List<T>();
			}
		}
		return ((List<T>) list).GetRange(fromIndex, toIndex - fromIndex);
	}


    public static void AddAll<T>(this IList<T> list, IEnumerable<T> c) {
        ((List<T>) list).AddRange(c);
    }

    public static void AddAll<T>(this IList<T> list, int index, IList<T> c) {
        for (int i = c.Count - 1; i >= 0; i--) {
            list.Insert(index, c[i]);
        }
    }

    public static void Add<T>(this IList<T> list, int index, T elem) {
        list.Insert(index, elem);
    }

    public static void AddAll<T>(this ICollection<T> c, IEnumerable<T> collectionToAdd) {
        foreach (T o in collectionToAdd) {
            c.Add(o);
        }
    }

    public static void AddAll<TKey, TValue>(this IDictionary<TKey, TValue> c, IDictionary<TKey, TValue> collectionToAdd) {
        foreach (KeyValuePair<TKey, TValue> pair in collectionToAdd) {
            c[pair.Key] = pair.Value;
        }
    }

    public static void GetChars(this StringBuilder sb, int srcBegin, int srcEnd, char[] dst, int dstBegin) {
        sb.CopyTo(srcBegin, dst, dstBegin, srcEnd - srcBegin);
    }

    public static String[] Split(this String str, String regex) {
        return str.Split(str.ToCharArray());
    }

    public static bool Matches(this String str, String regex) {
        return Regex.IsMatch(str, regex);
    }

    public static T[] ToArray<T>(this ICollection<T> col, T[] toArray) {
        T[] r;
        int colSize = col.Count;
        if (colSize <= toArray.Length) {
            col.CopyTo(toArray, 0);
            if (colSize != toArray.Length) {
                toArray[colSize] = default(T);
            }
            r = toArray;
        } else {
            r = new T[colSize];
            col.CopyTo(r, 0);
        }

        return r;
    }
    
    public static T[] ToArray<T>(this ICollection<T> col) {
        T[] r = new T[col.Count];
        col.CopyTo(r, 0);
        return r;
    }

    public static void ReadFully(this BinaryReader input, byte[] b, int off, int len) {
        if (len < 0) {
            throw new IndexOutOfRangeException();
        }
        int n = 0;
        while (n < len) {
            int count = input.Read(b, off + n, len - n);
            if (count <= 0) {
                throw new EndOfStreamException();
            }
            n += count;
        }
    }

    public static TValue JRemove<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key) {
        TValue value;
        dictionary.TryGetValue(key, out value);
        dictionary.Remove(key);

        return value;
    }

    public static T JRemoveAt<T>(this IList<T> list, int index) {
        T value = list[index];
        list.RemoveAt(index);

        return value;
    }

    public static bool RemoveAll<T>(this IList<T> list, ICollection<T> c) {
        return BatchRemove(list, c, false);
    }

    // Removes from this list all of its elements that are not contained in the specified collection.
    public static bool RetainAll<T>(this IList<T> list, ICollection<T> c) {
        return BatchRemove(list, c, true);
    }

    private static bool BatchRemove<T>(IList<T> list, ICollection<T> c, bool complement) {
        bool modified = false;
        int j = 0;
        for (int i = 0; i < list.Count; ++i) {
            if (c.Contains(list[i]) == complement) {
                list[j++] = list[i];
            }
        }
        if (j != list.Count) {
            modified = true;
            for (int i = list.Count - 1; i >= j; --i) {
                list.RemoveAt(i);
            }
        }
        return modified;
    }
    
    public static bool RemoveAll<T>(this ICollection<T> toClean, ICollection<T> c) {
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
    
    public static bool RetainAll<T>(this ICollection<T> toClean, ICollection<T> c) {
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

    public static T PollFirst<T>(this SortedSet<T> set) {
        T item = set.First();
        set.Remove(item);

        return item;
    }

    public static bool IsEmpty<T1, T2>(this ICollection<KeyValuePair<T1, T2>> collection) {
        return collection.Count == 0;
    }

    public static bool IsEmpty<T>(this ICollection<T> collection) {
        return collection.Count == 0;
    }

    public static bool IsEmpty<T>(this Stack<T> collection) {
        return collection.Count == 0;
    }

    public static void SetCharAt(this StringBuilder builder, int index, char ch) {
        builder[index] = ch;
    }

    public static float NextFloat(this Random random) {
        double mantissa = random.NextDouble();
        double exponent = Math.Pow(2.0, random.Next(-126, 128));
        if (mantissa < 0 || exponent < 0) {
            int a = 5;
        }
        float val = (float) (mantissa*exponent);
        if (val < 0) {
            int b = 6;
        }
        return (float) (mantissa*exponent);
    }

    public static bool NextBoolean(this Random random) {
        return random.NextDouble() > 0.5;
    }

    public static TValue Get<TKey, TValue>(this IDictionary<TKey, TValue> col, TKey key) {
        TValue value = default(TValue);
        if (key != null) {
            col.TryGetValue(key, out value);
        }

        return value;
    }

    public static TValue Put<TKey, TValue>(this IDictionary<TKey, TValue> col, TKey key, TValue value) {
        TValue oldVal = col.Get(key);
        col[key] = value;
        return oldVal;
    }

    public static Object Get(this IDictionary col, Object key) {
        Object value = null;
        if (key != null) {
            value = col[key];
        }

        return value;
    }

    public static void Put(this IDictionary col, Object key, Object value) {
        if (key != null) {
            col[key] = value;
        }
    }

    public static TValue Get<TKey, TValue>(this ConditionalWeakTable<TKey, TValue > table, TKey key) where
        TKey: class where TValue: class
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

    public static TValue JRemove<TKey, TValue>(this ConditionalWeakTable<TKey, TValue> table, TKey key)
        where TKey : class where TValue : class
    {
        TValue value;
        table.TryGetValue(key, out value);
        table.Remove(key);

        return value;
    }

    public static bool ContainsKey<TKey, TValue>(this ConditionalWeakTable<TKey, TValue> table, TKey key)
        where TKey : class where TValue : class
    {
        return table.Get(key) != null;
    }

    public static bool Contains<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key) {
        return dictionary.ContainsKey(key);
    }

    public static Stack<T> Clone<T>(this Stack<T> stack) {
        return new Stack<T>(new Stack<T>(stack)); // create stack twice to retain the original order
    }

    public static bool CanExecute(this FileInfo fileInfo)
    {
        return fileInfo.Exists;
    }

    public static T PollFirst<T>(this LinkedList<T> list) {
        T result = list.First.Value;
        list.RemoveFirst();
        return result;
    }

#if !NETSTANDARD2_0
    public static Attribute GetCustomAttribute(this Assembly assembly, Type attributeType) {
        object[] customAttributes = assembly.GetCustomAttributes(attributeType, false);
        if (customAttributes.Length > 0 && customAttributes[0] is Attribute) {
            return customAttributes[0] as Attribute;
        } else {
            return null;
        }
    }
#endif

    public static Assembly GetAssembly(this Type type) {
#if !NETSTANDARD2_0
        return type.Assembly;
#else
        return type.GetTypeInfo().Assembly;
#endif
    }

#if NETSTANDARD2_0
    public static MethodInfo GetMethod(this Type type, String methodName, Type[] parameterTypes) {
        return type.GetTypeInfo().GetMethod(methodName, parameterTypes);
    }

    public static MethodInfo GetMethod(this Type type, String methodName) {
        return type.GetTypeInfo().GetMethod(methodName);
    }

    public static ConstructorInfo GetConstructor(this Type type, Type[] parameterTypes) {
        return type.GetTypeInfo().GetConstructor(parameterTypes);
    }

    public static bool IsInstanceOfType(this Type type, object objToCheck) {
        return type.GetTypeInfo().IsInstanceOfType(objToCheck);
    }

    public static FieldInfo[] GetFields(this Type type, BindingFlags flags) {
        return type.GetTypeInfo().GetFields(flags);
    }

    public static byte[] GetBuffer(this MemoryStream memoryStream) {
        ArraySegment<byte> buf;
        memoryStream.TryGetBuffer(out buf);
        return buf.Array;
    }
#endif
}
//\endcond