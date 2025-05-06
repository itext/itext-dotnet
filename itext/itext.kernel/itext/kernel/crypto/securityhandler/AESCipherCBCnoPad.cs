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
namespace iText.Kernel.Crypto.Securityhandler {
//\cond DO_NOT_DOCUMENT
    /// <summary>Creates an AES Cipher with CBC and no padding.</summary>
    internal class AESCipherCBCnoPad {
//\cond DO_NOT_DOCUMENT
        internal iText.Kernel.Crypto.AESCipherCBCnoPad aESCipherCBCnoPad;
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>Creates a new instance of AESCipher with CBC and no padding</summary>
        /// <param name="forEncryption">
        /// if true the cipher is initialised for
        /// encryption, if false for decryption
        /// </param>
        /// <param name="key">the key to be used in the cipher</param>
        internal AESCipherCBCnoPad(bool forEncryption, byte[] key) {
            aESCipherCBCnoPad = new iText.Kernel.Crypto.AESCipherCBCnoPad(forEncryption, key);
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>Creates a new instance of AESCipher with CBC and no padding</summary>
        /// <param name="forEncryption">
        /// if true the cipher is initialised for
        /// encryption, if false for decryption
        /// </param>
        /// <param name="key">the key to be used in the cipher</param>
        /// <param name="initVector">initialization vector to be used in cipher</param>
        internal AESCipherCBCnoPad(bool forEncryption, byte[] key, byte[] initVector) {
            aESCipherCBCnoPad = new iText.Kernel.Crypto.AESCipherCBCnoPad(forEncryption, key, initVector);
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal virtual byte[] ProcessBlock(byte[] inp, int inpOff, int inpLen) {
            return aESCipherCBCnoPad.ProcessBlock(inp, inpOff, inpLen);
        }
//\endcond
    }
//\endcond
}
