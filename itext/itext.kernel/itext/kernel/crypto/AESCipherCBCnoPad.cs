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
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Parameters;

namespace iText.Kernel.Crypto {
    /// <summary>Creates an AES Cipher with CBC and no padding.</summary>
    /// <author>Paulo Soares</author>
    public class AESCipherCBCnoPad {
        private IBlockCipher cbc;

        /// <summary>Creates a new instance of AESCipher with CBC and no padding</summary>
        /// <param name="forEncryption">
        /// if true the cipher is initialised for
        /// encryption, if false for decryption
        /// </param>
        /// <param name="key">the key to be used in the cipher</param>
        public AESCipherCBCnoPad(bool forEncryption, byte[] key) {
            IBlockCipher aes = new AesFastEngine();
            cbc = new CbcBlockCipher(aes);
            KeyParameter kp = new KeyParameter(key);
            cbc.Init(forEncryption, kp);
        }

        /// <summary>Creates a new instance of AESCipher with CBC and no padding</summary>
        /// <param name="forEncryption">
        /// if true the cipher is initialised for
        /// encryption, if false for decryption
        /// </param>
        /// <param name="key">the key to be used in the cipher</param>
        /// <param name="initVector">initialization vector to be used in cipher</param>
        public AESCipherCBCnoPad(bool forEncryption, byte[] key, byte[] initVector) {
            IBlockCipher aes = new AesFastEngine();
            cbc = new CbcBlockCipher(aes);
            KeyParameter kp = new KeyParameter(key);
            ParametersWithIV piv = new ParametersWithIV(kp, initVector);
            cbc.Init(forEncryption, piv);
        }

        public virtual byte[] ProcessBlock(byte[] inp, int inpOff, int inpLen) {
            if ((inpLen % cbc.GetBlockSize()) != 0) {
                throw new ArgumentException("Not multiple of block: " + inpLen);
            }
            byte[] outp = new byte[inpLen];
            int baseOffset = 0;
            while (inpLen > 0) {
                cbc.ProcessBlock(inp, inpOff, outp, baseOffset);
                inpLen -= cbc.GetBlockSize();
                baseOffset += cbc.GetBlockSize();
                inpOff += cbc.GetBlockSize();
            }
            return outp;
        }
    }
}
