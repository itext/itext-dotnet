/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 Apryse Group NV
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
    /// Be aware that its API and functionality may be changed in future.
    /// </remarks>
    public sealed class ArrayUtil {
        private ArrayUtil() {
        }

        public static byte[] ShortenArray(byte[] src, int length) {
            if (length < src.Length) {
                byte[] shortened = new byte[length];
                Array.Copy(src, 0, shortened, 0, length);
                return shortened;
            }
            return src;
        }

        public static int[] ToIntArray(ICollection<int> collection) {
            int[] array = new int[collection.Count];
            int k = 0;
            foreach (int? key in collection) {
                array[k++] = (int)key;
            }
            return array;
        }

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

        public static int[] FillWithValue(int[] a, int value) {
            for (int i = 0; i < a.Length; i++) {
                a[i] = value;
            }
            return a;
        }

        public static float[] FillWithValue(float[] a, float value) {
            for (int i = 0; i < a.Length; i++) {
                a[i] = value;
            }
            return a;
        }

        public static void FillWithValue<T>(T[] a, T value) {
            for (int i = 0; i < a.Length; i++) {
                a[i] = value;
            }
        }

        public static int[] CloneArray(int[] src) {
            return (int[])src.Clone();
        }
    }
}
