/*
    This file is part of the iText (R) project.
    Copyright (c) 1998-2023 Apryse Group NV
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
using System.Linq;
using System.Text;

namespace iText.Signatures {
    internal static class SignExtensions {
        public static String JSubstring(this String str, int beginIndex, int endIndex) {
            return str.Substring(beginIndex, endIndex - beginIndex);
        }

        public static byte[] GetBytes(this String str) {
            return System.Text.Encoding.UTF8.GetBytes(str);
        }
        
        public static byte[] GetBytes(this String str, Encoding encoding) {
            return encoding.GetBytes(str);
        }

        public static void AddAll<T>(this ICollection<T> t, IEnumerable<T> newItems) {
            foreach (T item in newItems) {
                t.Add(item);
            }
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

        public static void AddAll<TKey, TValue>(this IDictionary<TKey, TValue> c, IDictionary<TKey, TValue> collectionToAdd) {
            foreach (KeyValuePair<TKey, TValue> pair in collectionToAdd) {
                c[pair.Key] = pair.Value;
            }
        }

        public static T JRemoveAt<T>(this IList<T> list, int index) {
            T value = list[index];
            list.RemoveAt(index);

            return value;
        }

        public static TValue JRemove<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key) {
            TValue value;
            dictionary.TryGetValue(key, out value);
            dictionary.Remove(key);

            return value;
        }

        public static bool IsEmpty<T>(this ICollection<T> collection) {
            return collection.Count == 0;
        }

        public static int Read(this Stream stream, byte[] buffer) {
            return stream.Read(buffer, 0, buffer.Length);
        }

        public static ICollection<T> SubList<T>(this ICollection<T> collection, int fromIndex, int toIndex) {
            return collection.ToList().GetRange(fromIndex, toIndex - fromIndex);
        }

        public static void Write(this Stream stream, byte[] buffer) {
            stream.Write(buffer, 0, buffer.Length);
        }

        public static void JReset(this MemoryStream stream) {
            stream.Position = 0;
        }

        public static int JRead(this Stream stream, byte[] buffer, int offset, int count) {
            int result = stream.Read(buffer, offset, count);
            return result == 0 ? -1 : result;
        }

        public static long Seek(this FileStream fs, long offset) {
            return fs.Seek(offset, SeekOrigin.Begin);
        }

        public static TValue Get<TKey, TValue>(this IDictionary<TKey, TValue> col, TKey key) {
            TValue value;
            col.TryGetValue(key, out value);
            return value;
        }

        public static TValue Put<TKey, TValue>(this IDictionary<TKey, TValue> col, TKey key, TValue value) {
            TValue oldVal = col.Get(key);
            col[key] = value;
            return oldVal;
        }

        public static bool After(this DateTime date, DateTime when) {
            return date.CompareTo(when) > 0;
        }

        public static bool Before(this DateTime date, DateTime when) {
            return date.CompareTo(when) < 0;
        }
    }
}
