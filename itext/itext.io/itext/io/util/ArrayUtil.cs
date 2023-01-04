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
