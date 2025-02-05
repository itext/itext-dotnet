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
using System.IO;
using iText.Commons.Bouncycastle.Crypto.Modes;
using iText.Commons.Utils;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Fips;
using Org.BouncyCastle.Utilities.IO;

namespace iText.Bouncycastlefips.Crypto.Modes {
    /// <summary>
    /// Wrapper class for IAeadCipher.
    /// </summary>
    public class GCMBlockCipherBCFips : IGCMBlockCipher {
        private IAeadCipher cipher;
        private MemoryOutputStream memoryStream = new MemoryOutputStream();
        private long lastPos = 0;

        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Crypto.IAeadCipher"/>.
        /// </summary>
        public GCMBlockCipherBCFips() {
        }

        /// <summary>Gets actual org.bouncycastle object being wrapped.</summary>
        /// <returns>Wrapped IAeadCipher.</returns>
        public virtual IAeadCipher GetCipher() {
            return cipher;
        }

        /// <summary><inheritDoc/></summary>
        public virtual void Init(bool forEncryption, byte[] key, int macSizeBits, byte[] iv) {
            FipsAes.Key aesKey = new FipsAes.Key(key);
            IAeadCipherService provider = CryptoServicesRegistrar.CreateService<IAeadCipherService>(aesKey);

            IAeadCipherBuilder<IParameters<FipsAlgorithm>> aeadEncryptorBldr = forEncryption ?
                provider.CreateAeadEncryptorBuilder(FipsAes.Gcm.WithIV(iv).WithMacSize(macSizeBits)) :
                provider.CreateAeadDecryptorBuilder(FipsAes.Gcm.WithIV(iv).WithMacSize(macSizeBits));

            cipher = (IAeadCipher)aeadEncryptorBldr.BuildCipher(memoryStream);
        }

        /// <summary><inheritDoc/></summary>
        public virtual int GetUpdateOutputSize(int len) {
            return cipher.GetUpdateOutputSize(len);
        }

        /// <summary><inheritDoc/></summary>
        public virtual void ProcessBytes(byte[] inputBuff, int inOff, int len, byte[] outBuff, int outOff) {
            cipher.Stream.Write(inputBuff, inOff, len);
            byte[] output = GetBytes();
            Array.Copy(output, 0, outBuff, outOff, output.Length);
        }

        /// <summary><inheritDoc/></summary>
        public virtual int GetOutputSize(int i) {
            return cipher.GetMaxOutputSize(i);
        }

        /// <summary><inheritDoc/></summary>
        public virtual void DoFinal(byte[] plainText, int i) {
            cipher.Stream.Close();
            byte[] encMac = GetBytes();
            Array.Copy(encMac, 0, plainText, i, encMac.Length);
        }

        /// <summary>Indicates whether some other object is "equal to" this one. Compares wrapped objects.</summary>
        public override bool Equals(Object o) {
            if (this == o) {
                return true;
            }
            if (o == null || GetType() != o.GetType()) {
                return false;
            }
            iText.Bouncycastlefips.Crypto.Modes.GCMBlockCipherBCFips that = (iText.Bouncycastlefips.Crypto.Modes.GCMBlockCipherBCFips
                )o;
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

        private byte[] GetBytes() {
            byte[] bytes = memoryStream.ToArray();
            long len = bytes.Length - lastPos;
            byte[] res = new byte[len];
            Array.Copy(bytes, lastPos, res, 0, len);
            lastPos = bytes.Length;
            return res;
        }
    }
}
