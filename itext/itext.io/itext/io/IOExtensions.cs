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
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using iText.Commons.Utils.Collections;

namespace iText.IO {
    internal static class IOExtensions {
        public static String JSubstring(this String str, int beginIndex, int endIndex) {
            return str.Substring(beginIndex, endIndex - beginIndex);
        }

        public static void JReset(this MemoryStream stream) {
            stream.Position = 0;
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

        public static void Write(this Stream stream, byte[] buffer) {
            stream.Write(buffer, 0, buffer.Length);
        }

        public static byte[] GetBytes(this String str) {
            return System.Text.Encoding.UTF8.GetBytes(str);
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

        public static void GetChars(this StringBuilder sb, int srcBegin, int srcEnd, char[] dst, int dstBegin) {
            sb.CopyTo(srcBegin, dst, dstBegin, srcEnd - srcBegin);
        }

        public static String[] Split(this String str, String regex) {
            return str.Split(regex.ToCharArray());
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

        public static void AddAll<T>(this IList<T> list, int index, IList<T> c) {
            for (int i = c.Count - 1; i >= 0; i--) {
                list.Insert(index, c[i]);
            }
        }

        public static void Add<T>(this IList<T> list, int index, T elem) {
            list.Insert(index, elem);
        }

        public static T JRemoveAt<T>(this IList<T> list, int index) {
            T value = list[index];
            list.RemoveAt(index);

            return value;
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

        public static bool Contains<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key) {
            return dictionary.ContainsKey(key);
        }

        public static bool IsEmpty<T>(this ICollection<T> collection) {
            return 0 == collection.Count;
        }

        public static bool EqualsIgnoreCase(this String str, String anotherString) {
            return String.Equals(str, anotherString, StringComparison.OrdinalIgnoreCase);
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

        public static Assembly GetAssembly(this Type type) {
#if !NETSTANDARD2_0
            return type.Assembly;
#else
            return type.GetTypeInfo().Assembly;
#endif
        }
    }
}
