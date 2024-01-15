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
