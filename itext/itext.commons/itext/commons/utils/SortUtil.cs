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

        internal class StringOrdinalComparator : IComparer<String> {
            public int Compare(String x, String y) {
                return String.CompareOrdinal(x, y);
            }
        }
    }
}
