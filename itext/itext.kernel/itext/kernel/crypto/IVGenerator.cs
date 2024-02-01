/*
This file is part of the iText (R) project.
Copyright (c) 1998-2024 Apryse Group NV
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
using iText.Commons.Utils;

namespace iText.Kernel.Crypto {
    /// <summary>An initialization vector generator for a CBC block encryption.</summary>
    /// <remarks>An initialization vector generator for a CBC block encryption. It's a random generator based on ARCFOUR.
    ///     </remarks>
    public sealed class IVGenerator {
        private static readonly ARCFOUREncryption arcfour;

        static IVGenerator() {
            arcfour = new ARCFOUREncryption();
            long time = SystemUtil.GetTimeBasedSeed();
            long mem = SystemUtil.GetFreeMemory();
            String s = time + "+" + mem;
            arcfour.PrepareARCFOURKey(s.GetBytes(iText.Commons.Utils.EncodingUtil.ISO_8859_1));
        }

        /// <summary>Creates a new instance of IVGenerator</summary>
        private IVGenerator() {
        }

        /// <summary>Gets a 16 byte random initialization vector.</summary>
        /// <returns>a 16 byte random initialization vector</returns>
        public static byte[] GetIV() {
            return GetIV(16);
        }

        /// <summary>Gets a random initialization vector.</summary>
        /// <param name="len">the length of the initialization vector</param>
        /// <returns>a random initialization vector</returns>
        public static byte[] GetIV(int len) {
            byte[] b = new byte[len];
            lock (arcfour) {
                arcfour.EncryptARCFOUR(b);
            }
            return b;
        }
    }
}
