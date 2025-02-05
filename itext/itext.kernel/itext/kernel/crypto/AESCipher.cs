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
using iText.Bouncycastleconnector;
using iText.Commons.Bouncycastle.Crypto;

namespace iText.Kernel.Crypto {
    /// <summary>Creates an AES Cipher with CBC and padding PKCS5/7.</summary>
    public class AESCipher {
        private ICipher cipher;

        /// <summary>Creates a new instance of AESCipher</summary>
        /// <param name="forEncryption">
        /// if true the cipher is initialised for
        /// encryption, if false for decryption
        /// </param>
        /// <param name="key">the key to be used in the cipher</param>
        /// <param name="iv">initialization vector to be used in cipher</param>
        public AESCipher(bool forEncryption, byte[] key, byte[] iv) {
            cipher = BouncyCastleFactoryCreator.GetFactory().CreateCipher(forEncryption, key, iv);
        }

        public virtual byte[] Update(byte[] inp, int inpOff, int inpLen) {
            return cipher.Update(inp, inpOff, inpLen);
        }

        public virtual byte[] DoFinal() {
            return cipher.DoFinal();
        }
    }
}
