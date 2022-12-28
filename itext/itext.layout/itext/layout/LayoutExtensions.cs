/*

This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
Authors: Bruno Lowagie, Paulo Soares, et al.

This program is free software; you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License version 3
as published by the Free Software Foundation with the addition of the
following permission added to Section 15 as permitted in Section 7(a):
FOR ANY PART OF THE COVERED WORK IN WHICH THE COPYRIGHT IS OWNED BY
ITEXT GROUP. ITEXT GROUP DISCLAIMS THE WARRANTY OF NON INFRINGEMENT
OF THIRD PARTY RIGHTS

This program is distributed in the hope that it will be useful, but
WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
or FITNESS FOR A PARTICULAR PURPOSE.
See the GNU Affero General Public License for more details.
You should have received a copy of the GNU Affero General Public License
along with this program; if not, see http://www.gnu.org/licenses or write to
the Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor,
Boston, MA, 02110-1301 USA, or download the license from the following URL:
http://itextpdf.com/terms-of-use/

The interactive user interfaces in modified source and object code versions
of this program must display Appropriate Legal Notices, as required under
Section 5 of the GNU Affero General Public License.

In accordance with Section 7(b) of the GNU Affero General Public License,
a covered work must retain the producer line in every PDF that is created
or manipulated using iText.

You can be released from the requirements of the license by purchasing
a commercial license. Buying such a license is mandatory as soon as you
develop commercial activities involving the iText software without
disclosing the source code of your own applications.
These activities include: offering paid services to customers as an ASP,
serving PDFs on the fly in a web application, shipping iText with a closed
source product.

For more information, please contact iText Software Corp. at this
address: sales@itextpdf.com
*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using iText.Commons.Utils.Collections;

namespace iText.Layout {
    internal static class LayoutExtensions {
        public static String JSubstring(this String str, int beginIndex, int endIndex) {
            return str.Substring(beginIndex, endIndex - beginIndex);
        }

        public static String JSubstring(this StringBuilder sb, int beginIndex, int endIndex) {
            return sb.ToString(beginIndex, endIndex - beginIndex);
        }

        public static bool EqualsIgnoreCase(this String str, String anotherString) {
            return String.Equals(str, anotherString, StringComparison.OrdinalIgnoreCase);
        }

        public static void JGetChars(this String str, int srcBegin, int srcEnd, char[] dst, int dstBegin) {
            str.CopyTo(srcBegin, dst, dstBegin, srcEnd - srcBegin);
        }

        public static void SetCharAt(this StringBuilder sb, int ind, char ch) {
            sb[ind] = ch;
        }

        public static byte[] GetBytes(this String str, Encoding encoding) {
            return encoding.GetBytes(str);
        }

        public static TValue JRemove<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key) {
            TValue value;
            dictionary.TryGetValue(key, out value);
            dictionary.Remove(key);

            return value;
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

        public static void Add<T>(this IList<T> list, int index, T elem) {
            list.Insert(index, elem);
        }

        public static bool ContainsAll<T>(this ICollection<T> thisC, ICollection<T> otherC) {
            foreach (T e in otherC) {
                if (!thisC.Contains(e)) {
                    return false;
                }
            }
            return true;
        }

        public static bool IsEmpty<T>(this ICollection<T> collection) {
            return 0 == collection.Count;
        }

        public static bool IsEmpty(this ICollection collection) {
            return 0 == collection.Count;
        }

        public static KeyValuePair<K, V>? HigherEntry<K, V>(this SortedDictionary<K, V> dict, K key) {
            List<K> list = dict.Keys.ToList();
            int index = list.BinarySearch(key, dict.Comparer);
            if (index < 0) {
                index = ~index;
            } else {
                index++;
            }
            if (index == list.Count) {
                return null;
            } else {
                return new KeyValuePair<K, V>(list[index], dict[list[index]]);
            }
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

        public static String[] Split(this String str, String regex) {
            return str.Split(regex.ToCharArray());
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

#if NETSTANDARD2_0
        public static MethodInfo GetMethod(this Type type, String methodName, Type[] parameterTypes) {
            return type    .GetTypeInfo().GetMethod(methodName, parameterTypes);
        }

        public static ConstructorInfo GetConstructor(this Type type, Type[] parameterTypes) {
            return type.GetTypeInfo().GetConstructor(parameterTypes);
        }
#endif
    }
}
