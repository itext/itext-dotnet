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
using iText.Commons.Bouncycastle.Crypto;
using iText.Commons.Utils;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Parameters;

namespace iText.Bouncycastle.Crypto {
    public class CipherCBCnoPadBC : ICipherCBCnoPad {
        private readonly IBlockCipher blockCipher;

        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="IBlockCipher"/>.
        /// </summary>
        /// <param name="forEncryption">
        /// Defines whether this wrapper will be used for encryption or decryption.
        /// </param>
        /// <param name="key">
        /// Key bytes to be used during block cipher creation.
        /// </param>
        /// <param name="initVector">
        /// Initialization vector to be used during block cipher creation.
        /// </param>
        public CipherCBCnoPadBC(bool forEncryption, byte[] key, byte[] initVector = null) {
            IBlockCipher aes = new AesEngine();
            blockCipher = new CbcBlockCipher(aes);
            KeyParameter kp = new KeyParameter(key);
            if (initVector == null) {
                blockCipher.Init(forEncryption, kp);
            }
            else {
                ParametersWithIV piv = new ParametersWithIV(kp, initVector);
                blockCipher.Init(forEncryption, piv);   
            }
        }

        /// <summary><inheritDoc/></summary>
        public virtual byte[] ProcessBlock(byte[] inp, int inpOff, int inpLen) {
            if ((inpLen % blockCipher.GetBlockSize()) != 0) {
                throw new ArgumentException("Not multiple of block: " + inpLen);
            }
            byte[] outp = new byte[inpLen];
            int baseOffset = 0;
            while (inpLen > 0) {
                blockCipher.ProcessBlock(inp, inpOff, outp, baseOffset);
                inpLen -= blockCipher.GetBlockSize();
                baseOffset += blockCipher.GetBlockSize();
                inpOff += blockCipher.GetBlockSize();
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
            CipherCBCnoPadBC that = (CipherCBCnoPadBC)o;
            return Object.Equals(blockCipher, that.blockCipher);
        }

        /// <summary>Returns a hash code value based on the wrapped object.</summary>
        public override int GetHashCode() {
            return JavaUtil.ArraysHashCode(blockCipher);
        }

        /// <summary>
        /// Delegates
        /// <c>toString</c>
        /// method call to the wrapped object.
        /// </summary>
        public override String ToString() {
            return blockCipher.ToString();
        }
    }
}
