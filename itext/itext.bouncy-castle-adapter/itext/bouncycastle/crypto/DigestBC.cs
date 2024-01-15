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
using iText.Commons.Bouncycastle.Crypto;
using iText.Commons.Utils;
using Org.BouncyCastle.Crypto;
using IDigest = iText.Commons.Bouncycastle.Crypto.IDigest;

namespace iText.Bouncycastle.Crypto {
    /// <summary>
    /// Wrapper class for
    /// <see cref="Org.BouncyCastle.Crypto.IDigest"/>.
    /// </summary>
    public class DigestBC : IDigest {
        private readonly Org.BouncyCastle.Crypto.IDigest iDigest;

        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Crypto.IDigest"/>.
        /// </summary>
        /// <param name="iDigest">
        /// 
        /// <see cref="Org.BouncyCastle.Crypto.IDigest"/>
        /// to be wrapped
        /// </param>
        public DigestBC(Org.BouncyCastle.Crypto.IDigest iDigest) {
            this.iDigest = iDigest;
        }

        /// <summary>Gets actual org.bouncycastle object being wrapped.</summary>
        /// <returns>
        /// wrapped IDigest<IBlockResult>.
        /// </returns>
        public virtual Org.BouncyCastle.Crypto.IDigest GetIDigest() {
            return iDigest;
        }

        /// <summary><inheritDoc/></summary>
        public byte[] Digest(byte[] enc2) {
            iDigest.BlockUpdate(enc2, 0, enc2.Length);
            return Digest();
        }

        /// <summary><inheritDoc/></summary>
        public byte[] Digest() {
            byte[] output = new byte[iDigest.GetDigestSize()];
            iDigest.DoFinal(output, 0);
            return output;
        }

        /// <summary><inheritDoc/></summary>
        public void Update(byte[] buf, int off, int len) {
            iDigest.BlockUpdate(buf, off, len);
        }

        /// <summary><inheritDoc/></summary>
        public int GetDigestLength() {
            return iDigest.GetByteLength();
        }

        /// <summary><inheritDoc/></summary>
        public void Update(byte[] buf) {
            Update(buf, 0, buf.Length);
        }

        /// <summary><inheritDoc/></summary>
        public void Reset() {
            iDigest.Reset();
        }

        /// <summary><inheritDoc/></summary>
        public string GetAlgorithmName() {
            return iDigest.AlgorithmName;
        }

        /// <summary>Indicates whether some other object is "equal to" this one. Compares wrapped objects.</summary>
        public override bool Equals(Object o) {
            if (this == o) {
                return true;
            }
            if (o == null || GetType() != o.GetType()) {
                return false;
            }
            DigestBC that = (DigestBC)o;
            return Object.Equals(iDigest, that.iDigest);
        }

        /// <summary>Returns a hash code value based on the wrapped object.</summary>
        public override int GetHashCode() {
            return JavaUtil.ArraysHashCode(iDigest);
        }

        /// <summary>
        /// Delegates
        /// <c>toString</c>
        /// method call to the wrapped object.
        /// </summary>
        public override String ToString() {
            return iDigest.ToString();
        }
    }
}
