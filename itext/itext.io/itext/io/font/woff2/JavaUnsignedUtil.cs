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

namespace iText.IO.Font.Woff2 {
    /// <summary>Helper class to deal with unsigned primitives in java</summary>
    internal class JavaUnsignedUtil {
        public static int AsU16(short number) {
            return number & 0xffff;
        }

        public static int AsU8(byte number) {
            return number & 0xff;
        }

        public static byte ToU8(int number) {
            return (byte)(number & 0xff);
        }

        public static short ToU16(int number) {
            return (short)(number & 0xffff);
        }

        public static int CompareAsUnsigned(int left, int right) {
            return Convert.ToInt64(left & unchecked((long)(0xffffffffL))).CompareTo(right & unchecked((long)(0xffffffffL
                )));
        }
    }
}
