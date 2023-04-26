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
using iText.Commons.Utils;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Paddings;
using Org.BouncyCastle.Crypto.Parameters;
using ICipher = iText.Commons.Bouncycastle.Crypto.ICipher;

namespace iText.Bouncycastle.Crypto {
    /// <summary>
    /// Wrapper class for IBufferedCipher.
    /// </summary>
    public class CipherBC : ICipher {
        private readonly IBufferedCipher cipher;
        
        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="IBufferedCipher"/>.
        /// </summary>
        /// <param name="cipher">
        /// <see cref="IBufferedCipher"/> to be wrapped
        /// </param>
        public CipherBC(IBufferedCipher cipher) {
            this.cipher = cipher;
        }
        
        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="IBufferedCipher"/>.
        /// </summary>
        /// <param name="forEncryption">boolean value</param>
        /// <param name="key">byte array</param>
        /// <param name="iv">init vector</param>
        public CipherBC(bool forEncryption, byte[] key, byte[] iv) {
            IBlockCipher aes = new AesFastEngine();
            IBlockCipher cbc = new CbcBlockCipher(aes);
            cipher = new PaddedBufferedBlockCipher(cbc);
            KeyParameter kp = new KeyParameter(key);
            ParametersWithIV piv = new ParametersWithIV(kp, iv);
            cipher.Init(forEncryption, piv);
        }

        /// <summary>Gets actual org.bouncycastle object being wrapped.</summary>
        /// <returns>
        /// wrapped IBufferedCipher<IBlockResult>.
        /// </returns>
        public virtual IBufferedCipher GetICipher() {
            return cipher;
        }

        /// <summary><inheritDoc/></summary>
        public byte[] Update(byte[] inp, int inpOff, int inpLen) {
            int neededLen = cipher.GetUpdateOutputSize(inpLen);
            byte[] outp;
            if (neededLen > 0) {
                outp = new byte[neededLen];
            }
            else {
                outp = new byte[0];
            }
            cipher.ProcessBytes(inp, inpOff, inpLen, outp, 0);
            return outp;
        }

        /// <summary><inheritDoc/></summary>
        public byte[] DoFinal() {
            int neededLen = cipher.GetOutputSize(0);
            byte[] outp = new byte[neededLen];
            int n;
            try {
                n = cipher.DoFinal(outp, 0);
            } catch (Exception) {
                return outp;
            }
            if (n != outp.Length) {
                byte[] outp2 = new byte[n];
                Array.Copy(outp, 0, outp2, 0, n);
                return outp2;
            }
            return outp;
        }
        
        /// <summary>Indicates whether some other object is "equal to" this one. Compares wrapped objects.</summary>
        public override bool Equals(Object o) {
            if (this == o) {
                return true;
            }
            if (o == null || GetType() != o.GetType()) {
                return false;
            }
            CipherBC that = (CipherBC)o;
            return Object.Equals(cipher, that.cipher);
        }

        /// <summary>Returns a hash code value based on the wrapped object.</summary>
        public override int GetHashCode() {
            return JavaUtil.ArraysHashCode(cipher);
        }

        /// <summary>
        /// Delegates
        /// <c>toString</c>
        /// method call to the wrapped object.
        /// </summary>
        public override String ToString() {
            return cipher.ToString();
        }
    }
}
