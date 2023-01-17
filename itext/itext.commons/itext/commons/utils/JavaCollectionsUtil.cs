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
using System.Collections.ObjectModel;
using System.Linq;
using iText.Commons.Utils.Collections;

namespace iText.Commons.Utils {
    /// <summary>
    /// This file is a helper class for internal usage only.
    /// Be aware that its API and functionality may be changed in future.
    /// </summary>
    public static class JavaCollectionsUtil {
        public static IList<T> EmptyList<T>() {
            return new EmptyList<T>();
        }

        public static IDictionary<TKey, TValue> EmptyMap<TKey, TValue>() {
            return new EmptyDictionary<TKey, TValue>();
        }

        public static ISet<T> EmptySet<T>() {
            return new EmptySet<T>();
        }

        public static IEnumerator<T> EmptyIterator<T>() {
            return new EmptyEnumerator<T>();
        }

        public static IEnumerator EmptyIterator() {
            return new EmptyEnumerator();
        }

        public static ICollection<T> UnmodifiableCollection<T>(ICollection<T> collection) {
            return new UnmodifiableCollection<T>(collection);
        }

        public static IList<T> UnmodifiableList<T>(IList<T> list) {
            return new ReadOnlyCollection<T>(list);
        }

        public static IList UnmodifiableList(IList list) {
            return new UnmodifiableList(list);
        }

        public static IDictionary<TKey, TValue> UnmodifiableMap<TKey, TValue>(IDictionary<TKey, TValue> dict) {
            return new UnmodifiableDictionary<TKey, TValue>(dict);
        }

        public static ISet<T> UnmodifiableSet<T>(ISet<T> set) {
            return new UnmodifiableSet<T>(set);
        }

        public static ISet<T> Singleton<T>(T o) {
            return new SingletonSet<T>(o);
        }

        public static IList<T> SingletonList<T>(T o) {
            return new SingletonList<T>(o);
        }

        public static IDictionary<TKey, TValue> SingletonMap<TKey, TValue>(TKey key, TValue value)
        {
            return new SingletonDictionary<TKey,TValue>(key, value);
        }

        public static void Sort<T>(IList<T> list) {
            Sort(list, null);
        }

        public static void Sort<T>(IList<T> list, IComparer<T> comparer) {
            if (list is List<T>) {
                SortUtil.MergeSort((List<T>) list, comparer);
            } else {
                IEnumerable<T> sorted = list.ToArray().OrderBy(x => x, comparer ?? Comparer<T>.Default);
                list.Clear();
                list.AddAll(sorted);
            }
        }

        public static void Sort(IList<String> list) {
            Sort(list, null);
        }

        public static void Sort(IList<String> list, IComparer<String> comparer) {
            if (list is List<String>) {
                SortUtil.MergeSort((List<String>) list, comparer);
            } else {
                IEnumerable<String> sorted = list.ToArray().OrderBy(x => x, comparer ?? new SortUtil.StringOrdinalComparator());
                list.Clear();
                list.AddAll(sorted);
            }
        }

        public static void Reverse<T>(IList<T> list) {
            if (list is List<T>) {
                ((List<T>) list).Reverse();
            } else {
                IEnumerable<T> rev = list.Reverse();
                list.Clear();
                list.AddAll(rev);
            }
        }
    }
}
