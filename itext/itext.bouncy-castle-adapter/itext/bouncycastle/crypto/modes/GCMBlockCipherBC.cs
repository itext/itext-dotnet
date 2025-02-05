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
using System;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Parameters;
using iText.Commons.Bouncycastle.Crypto.Modes;
using iText.Commons.Utils;

namespace iText.Bouncycastle.Crypto.Modes {
    /// <summary>
    /// This class provides the functionality of a cryptographic cipher of aes-gcm for encryption and decryption via
    /// wrapping the corresponding
    /// <c>GCMBlockCipher</c>
    /// class from bouncy-castle.
    /// </summary>
    public class GCMBlockCipherBC : IGCMBlockCipher {
        private readonly GcmBlockCipher cipher;

        /// <summary>
        /// Creates new wrapper for
        /// <see cref="Org.BouncyCastle.Crypto.Modes.GcmBlockCipher"/>
        /// aes-gcm block cipher class.
        /// </summary>
        /// <param name="cipher">bouncy-castle class to wrap</param>
        public GCMBlockCipherBC(GcmBlockCipher cipher) {
            this.cipher = cipher;
        }

        /// <summary>Gets actual org.bouncycastle object being wrapped.</summary>
        /// <returns>
        /// wrapped
        /// <see cref="Org.BouncyCastle.Crypto.Modes.GcmBlockCipher"/>
        /// </returns>
        public virtual GcmBlockCipher GetCipher() {
            return cipher;
        }

        /// <summary><inheritDoc/></summary>
        public virtual void Init(bool forEncryption, byte[] key, int macSizeBits, byte[] iv) {
            cipher.Init(forEncryption, new AeadParameters(new KeyParameter(key), macSizeBits, iv));
        }

        /// <summary><inheritDoc/></summary>
        public virtual int GetUpdateOutputSize(int len) {
            return cipher.GetUpdateOutputSize(len);
        }

        /// <summary><inheritDoc/></summary>
        public virtual void ProcessBytes(byte[] input, int inputOffset, int len, byte[] output, int outOffset) {
            cipher.ProcessBytes(input, inputOffset, len, output, outOffset);
        }

        /// <summary><inheritDoc/></summary>
        public virtual int GetOutputSize(int len) {
            return cipher.GetOutputSize(len);
        }

        /// <summary><inheritDoc/></summary>
        public virtual void DoFinal(byte[] plainText, int i) {
            try {
                cipher.DoFinal(plainText, i);
            }
            catch (InvalidCipherTextException e) {
                throw new ArgumentException(e.Message, e);
            }
        }

        /// <summary><inheritDoc/></summary>
        public override bool Equals(Object o) {
            if (this == o) {
                return true;
            }
            if (o == null || GetType() != o.GetType()) {
                return false;
            }
            iText.Bouncycastle.Crypto.Modes.GCMBlockCipherBC that = (iText.Bouncycastle.Crypto.Modes.GCMBlockCipherBC)
                o;
            return Object.Equals(cipher, that.cipher);
        }

        /// <summary><inheritDoc/></summary>
        public override int GetHashCode() {
            return JavaUtil.ArraysHashCode(cipher);
        }

        /// <summary><inheritDoc/></summary>
        public override String ToString() {
            return cipher.ToString();
        }
    }
}
