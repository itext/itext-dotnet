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

namespace iText.IO.Util {
    /// <summary>This file is a helper class for internal usage only.</summary>
    /// <remarks>
    /// This file is a helper class for internal usage only.
    /// Be aware that its API and functionality may be changed in the future.
    /// </remarks>
    public sealed class ArrayUtil {
        private ArrayUtil() {
        }

        /// <summary>Shortens byte array.</summary>
        /// <param name="src">the byte array</param>
        /// <param name="length">the new length of bytes array</param>
        /// <returns>the shortened byte array</returns>
        public static byte[] ShortenArray(byte[] src, int length) {
            if (length < src.Length) {
                byte[] shortened = new byte[length];
                Array.Copy(src, 0, shortened, 0, length);
                return shortened;
            }
            return src;
        }

        /// <summary>Converts a collection to an int array.</summary>
        /// <param name="collection">the collection</param>
        /// <returns>the int array</returns>
        public static int[] ToIntArray(ICollection<int> collection) {
            int[] array = new int[collection.Count];
            int k = 0;
            foreach (int? key in collection) {
                array[k++] = (int)key;
            }
            return array;
        }

        /// <summary>Creates a hash of the given byte array.</summary>
        /// <param name="a">the byte array</param>
        /// <returns>the byte array</returns>
        public static int HashCode(byte[] a) {
            if (a == null) {
                return 0;
            }
            int result = 1;
            foreach (byte element in a) {
                result = 31 * result + element;
            }
            return result;
        }

        /// <summary>Fills an array with the given value.</summary>
        /// <param name="a">the int array</param>
        /// <param name="value">the number of a value</param>
        /// <returns>the int array</returns>
        public static int[] FillWithValue(int[] a, int value) {
            for (int i = 0; i < a.Length; i++) {
                a[i] = value;
            }
            return a;
        }

        /// <summary>Fills an array with the given value.</summary>
        /// <param name="a">the float array</param>
        /// <param name="value">the number of a value</param>
        /// <returns>the float array</returns>
        public static float[] FillWithValue(float[] a, float value) {
            for (int i = 0; i < a.Length; i++) {
                a[i] = value;
            }
            return a;
        }

        /// <summary>Fills an array with the given value.</summary>
        /// <param name="a">the array</param>
        /// <param name="value">the value of type</param>
        /// <typeparam name="T">the type of the implementation</typeparam>
        public static void FillWithValue<T>(T[] a, T value) {
            for (int i = 0; i < a.Length; i++) {
                a[i] = value;
            }
        }

        /// <summary>Clones int array.</summary>
        /// <param name="src">the int array</param>
        /// <returns>the int array</returns>
        public static int[] CloneArray(int[] src) {
            return (int[])src.Clone();
        }

        /// <summary>Gets the index of object.</summary>
        /// <param name="a">the object array</param>
        /// <param name="key">the object key</param>
        /// <returns>the index of object</returns>
        public static int IndexOf(Object[] a, Object key) {
            for (int i = 0; i < a.Length; i++) {
                Object el = a[i];
                if (el.Equals(key)) {
                    return i;
                }
            }
            return -1;
        }
    }
}
