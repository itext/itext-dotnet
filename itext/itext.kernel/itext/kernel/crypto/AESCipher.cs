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
using Org.BouncyCastle.Crypto.Paddings;
using Org.BouncyCastle.Crypto.Parameters;

namespace iText.Kernel.Crypto {
    /// <summary>Creates an AES Cipher with CBC and padding PKCS5/7.</summary>
    /// <author>Paulo Soares</author>
    public class AESCipher {
        private PaddedBufferedBlockCipher bp;

        /// <summary>Creates a new instance of AESCipher</summary>
        /// <param name="forEncryption">
        /// if true the cipher is initialised for
        /// encryption, if false for decryption
        /// </param>
        /// <param name="key">the key to be used in the cipher</param>
        /// <param name="iv">initialization vector to be used in cipher</param>
        public AESCipher(bool forEncryption, byte[] key, byte[] iv) {
            IBlockCipher aes = new AesFastEngine();
            IBlockCipher cbc = new CbcBlockCipher(aes);
            bp = new PaddedBufferedBlockCipher(cbc);
            KeyParameter kp = new KeyParameter(key);
            ParametersWithIV piv = new ParametersWithIV(kp, iv);
            bp.Init(forEncryption, piv);
        }

        public virtual byte[] Update(byte[] inp, int inpOff, int inpLen) {
            int neededLen = bp.GetUpdateOutputSize(inpLen);
            byte[] outp;
            if (neededLen > 0) {
                outp = new byte[neededLen];
            }
            else {
                outp = new byte[0];
            }
            bp.ProcessBytes(inp, inpOff, inpLen, outp, 0);
            return outp;
        }

        public virtual byte[] DoFinal() {
            int neededLen = bp.GetOutputSize(0);
            byte[] outp = new byte[neededLen];
            int n;
            try {
                n = bp.DoFinal(outp, 0);
            }
            catch (Exception) {
                return outp;
            }
            if (n != outp.Length) {
                byte[] outp2 = new byte[n];
                Array.Copy(outp, 0, outp2, 0, n);
                return outp2;
            }
            else {
                return outp;
            }
        }
    }
}
