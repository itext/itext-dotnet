using System;
using System.Collections.Generic;

namespace com.itextpdf.io.util
{
    public class SortUtil {
        /// <summary>
        /// A stable sort
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="comparison"></param>
        public static void InsertionSort<T>(IList<T> list, Comparison<T> comparison) {
            if (list == null)
                throw new ArgumentNullException("list");
            if (comparison == null)
                throw new ArgumentNullException("comparison");
            
            int count = list.Count;
            for (int j = 1; j < count; j++) {
                T key = list[j];

                int i = j - 1;
                for (; i >= 0 && comparison(list[i], key) > 0; i--) {
                    list[i + 1] = list[i];
                }
                list[i + 1] = key;
            }
        }

        public static void InsertionSort<T>(IList<T> list) where T : IComparable<T> {
            InsertionSort(list, delegate(T o1, T o2) { return o1.CompareTo(o2); });
        }

        public static void MergeSort<T>(List<T> list, Comparison<T> comparison) {
            MergeSort(list, 0, list.Count - 1, comparison);
        }

        public static void MergeSort<T>(List<T> list) where T : IComparable<T> {
            MergeSort(list, delegate(T o1, T o2) { return o1.CompareTo(o2); });
        }

        private static void MergeSort<T>(List<T> list, int left, int right, Comparison<T> comparison) {
            if (right > left) {
                int mid = (right + left) / 2;
                MergeSort(list, left, mid, comparison);
                MergeSort(list, (mid + 1), right, comparison);

                Merge(list, left, (mid + 1), right, comparison);
            }
        }

        private static void Merge<T>(List<T> list, int left, int mid, int right, Comparison<T> comparison) {
            int eol = (mid - 1), num = (right - left + 1);

            List<T> temp = new List<T>(num);

            while ((left <= eol) && (mid <= right)) {
                if (comparison(list[left], list[mid]) <= 0)
                    temp.Add(list[left++]);
                else
                    temp.Add(list[mid++]);
            }

            while (left <= eol)
                temp.Add(list[left++]);

            while (mid <= right)
                temp.Add(list[mid++]);

            for (int i = num - 1; i >= 0; i--) {
                list[right--] = temp[i];
            }
        }
    }
}
