using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using itextsharp.io.util.Collections;

namespace iTextSharp.IO.Util {
    public static class JavaCollectionsUtil {
        public static IList<T> EmptyList<T>() {
            return new EmptyList<T>();
        }

        public static IDictionary<TKey, TValue> EmptyMap<TKey, TValue>() {
            return new EmptyDictionary<TKey, TValue>();
        }

        public static IEnumerator<T> EmptyIterator<T>() {
            return new EmptyEnumerator<T>();
        }

        public static ICollection<T> UnmodifiableCollection<T>(ICollection<T> collection) {
            return new UnmodifiableCollection<T>(collection);
        }

        public static IList<T> UnmodifiableList<T>(IList<T> list) {
            return new ReadOnlyCollection<T>(list);
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
            if (list is List<T>) {
                ((List<T>) list).Sort();
            } else {
                T[] arr = list.ToArray();
                Array.Sort(arr);
                list.Clear();
                list.AddAll(arr);
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
