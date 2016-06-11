using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using iTextSharp.IO.Util.Collections;

namespace iTextSharp.IO.Util {
    /// <summary>
    /// This file is a helper class for internal usage only.
    /// Be aware that it's API and functionality may be changed in future.
    /// </summary>
    public static class JavaCollectionsUtil
    {
        public static IList<T> EmptyList<T>() {
            return new EmptyList<T>();
        }

        public static IDictionary<TKey, TValue> EmptyMap<TKey, TValue>() {
            return new EmptyDictionary<TKey, TValue>();
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

        public static IList<T> SingletonList<T>(T o) {
            return new SingletonList<T>(o);
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
