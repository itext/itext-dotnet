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
namespace iText.Kernel.Crypto {
    public class StandardDecryptor : IDecryptor {
        protected internal ARCFOUREncryption arcfour;

        /// <summary>Creates a new instance of StandardDecryption</summary>
        /// <param name="key">data to be written</param>
        /// <param name="off">the start offset in data</param>
        /// <param name="len">number of bytes to write</param>
        public StandardDecryptor(byte[] key, int off, int len) {
            arcfour = new ARCFOUREncryption();
            arcfour.PrepareARCFOURKey(key, off, len);
        }

        public virtual byte[] Update(byte[] b, int off, int len) {
            byte[] b2 = new byte[len];
            arcfour.EncryptARCFOUR(b, off, len, b2, 0);
            return b2;
        }

        public virtual byte[] Finish() {
            return null;
        }
    }
}
