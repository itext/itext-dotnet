/*
    This file is part of the iText (R) project.
    Copyright (c) 1998-2025 Apryse Group NV
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

namespace iText.Commons.Utils {
    /// <summary>
    /// This file is a helper class for internal usage only.
    /// Be aware that its API and functionality may be changed in future.
    /// </summary>
    public class SortUtil {
        public static void MergeSort<T>(List<T> list, IComparer<T> comparer) {
            if (comparer == null) {
                comparer = Comparer<T>.Default;
            }
            MergeSort(list, comparer, 0, list.Count - 1);
        }

        public static void MergeSort(List<String> list, IComparer<String> comparer) {
            if (comparer == null) {
                comparer = new StringOrdinalComparator();
            }
            MergeSort(list, comparer, 0, list.Count - 1);
        }

        private static void MergeSort<T>(List<T> list, IComparer<T> comparer, int left, int right) {
            if (right > left) {
                int mid = (right + left)/2;
                MergeSort(list, comparer, left, mid);
                MergeSort(list, comparer, (mid + 1), right);

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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="from">the index of the first element, inclusive, to be sorted</param>
        /// <param name="to">the index of the last element, exclusive, to be sorted</param>
        public static void MergeSort<T>(T[] array, int from, int to, IComparer<T> comparer) {
            if (comparer == null) {
                comparer = Comparer<T>.Default;
            }
            MergeSort(array, comparer, from, to - 1);
        }

        public static void MergeSort(String[] array, IComparer<String> comparer) {
            if (comparer == null) {
                comparer = new StringOrdinalComparator();
            }
            MergeSort(array, comparer, 0, array.Length - 1);
        }

        private static void MergeSort<T>(T[] array, IComparer<T> comparer, int left, int right) {
            if (right > left) {
                int mid = (right + left)/2;
                MergeSort(array, comparer, left, mid);
                MergeSort(array, comparer, (mid + 1), right);

                Merge(array, left, (mid + 1), right, comparer);
            }
        }

        private static void Merge<T>(T[] array, int left, int mid, int right, IComparer<T> comparer) {
            int eol = (mid - 1), num = (right - left + 1);

            List<T> temp = new List<T>(num);

            while ((left <= eol) && (mid <= right)) {
                if (comparer.Compare(array[left], array[mid]) <= 0)
                    temp.Add(array[left++]);
                else
                    temp.Add(array[mid++]);
            }

            while (left <= eol)
                temp.Add(array[left++]);

            while (mid <= right)
                temp.Add(array[mid++]);

            for (int i = num - 1; i >= 0; i--) {
                array[right--] = temp[i];
            }
        }

        //\cond DO_NOT_DOCUMENT
        internal class StringOrdinalComparator : IComparer<String> {
            public int Compare(String x, String y) {
                return String.CompareOrdinal(x, y);
            }
        }
        //\endcond
    }
}
