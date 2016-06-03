using System;
using System.Collections.Generic;

namespace iTextSharp.IO.Util {
    public class SortUtil {
        public static void MergeSort<T>(List<T> list, IComparer<T> comparer) {
            if (comparer == null) {
                comparer = Comparer<T>.Default;
            }
            MergeSort(list, 0, list.Count - 1, comparer);
        }

        public static void MergeSort<T>(List<T> list) {
            MergeSort(list, null);
        }

        public static void MergeSort<T>(List<String> list) {
            MergeSort(list, new StringOrdinalComparator());
        }

        private static void MergeSort<T>(List<T> list, int left, int right, IComparer<T> comparer) {
            if (right > left) {
                int mid = (right + left) / 2;
                MergeSort(list, left, mid, comparer);
                MergeSort(list, (mid + 1), right, comparer);

                Merge(list, left, (mid + 1), right, comparer);
            }
        }

        private static void Merge<T>(List<T> list, int left, int mid, int right, IComparer<T> comparer) {
            int eol = (mid - 1), num = (right - left + 1);

            List<T> temp = new List<T>(num);

            while ((left <= eol) && (mid <= right)) {
                if (comparer.Compare(list[left], list[mid]) <= 0)
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

        public static void MergeSort<T>(T[] array, IComparer<T> comparer)
        {
            if (comparer == null) {
                comparer = Comparer<T>.Default;
            }
            MergeSort(array, 0, array.Length - 1, comparer);
        }

        public static void MergeSort<T>(T[] array) 
        {
            MergeSort(array, null);
        }

        public static void MergeSort(String[] array) 
        {
            MergeSort(array, null);
        }

        public static void MergeSort(String[] array, IComparer<String> comparer) {
            if (comparer == null) {
                comparer = new StringOrdinalComparator();
            }
            MergeSort(array, 0, array.Length - 1, comparer);
        }

        private static void MergeSort<T>(T[] array, int left, int right, IComparer<T> comparer)
        {
            if (right > left)
            {
                int mid = (right + left) / 2;
                MergeSort(array, left, mid, comparer);
                MergeSort(array, (mid + 1), right, comparer);

                Merge(array, left, (mid + 1), right, comparer);
            }
        }

        private static void Merge<T>(T[] array, int left, int mid, int right, IComparer<T> comparer)
        {
            int eol = (mid - 1), num = (right - left + 1);

            List<T> temp = new List<T>(num);

            while ((left <= eol) && (mid <= right))
            {
                if (comparer.Compare(array[left], array[mid]) <= 0)
                    temp.Add(array[left++]);
                else
                    temp.Add(array[mid++]);
            }

            while (left <= eol)
                temp.Add(array[left++]);

            while (mid <= right)
                temp.Add(array[mid++]);

            for (int i = num - 1; i >= 0; i--)
            {
                array[right--] = temp[i];
            }
        }

        private class StringOrdinalComparator : IComparer<String> {
            public int Compare(String x, String y) {
                return String.CompareOrdinal(x, y);
            }
        }
    }
}
