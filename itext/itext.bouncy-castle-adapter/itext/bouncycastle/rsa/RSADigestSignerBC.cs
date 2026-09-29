/*
This file is part of the iText (R) project.
Copyright (c) 1998-2026 Apryse Group NV
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
using Org.BouncyCastle.Crypto.Signers;
using iText.Bouncycastle.Cert;
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Bouncycastle.Rsa;
using iText.Commons.Utils;

namespace iText.Bouncycastle.Rsa {
    /// <summary>
    /// Wrapper class for
    /// <see cref="Org.BouncyCastle.Crypto.Signers.RSADigestSigner"/>.
    /// </summary>
    public class RSADigestSignerBC : IRSADigestSigner {
        private readonly RsaDigestSigner rsaDigestSigner;

        /// <summary>
        /// Creates a new instance of
        /// <see cref="RSADigestSignerBC"/>
        /// with the specified BouncyCastle RsaDigestSigner.
        /// </summary>
        /// <param name="rsaDigestSigner">the BouncyCastle RsaDigestSigner to wrap</param>
        public RSADigestSignerBC(RsaDigestSigner rsaDigestSigner) {
            this.rsaDigestSigner = rsaDigestSigner;
        }

        /// <summary>Returns the wrapped BouncyCastle RsaDigestSigner.</summary>
        /// <returns>the wrapped BouncyCastle RsaDigestSigner</returns>
        public virtual RsaDigestSigner GetRsaDigestSigner() {
            return rsaDigestSigner;
        }

        /// <summary><inheritDoc/></summary>
        public virtual void Init(bool forSigning, ICipherParams parameters) {
            rsaDigestSigner.Init(forSigning, ((CipherParamsBC)parameters).GetCipherParameters());
        }

        /// <summary><inheritDoc/></summary>
        public virtual void Update(byte[] input, int inOff, int length) {
            rsaDigestSigner.BlockUpdate(input, inOff, length);
        }

        /// <summary><inheritDoc/></summary>
        public virtual bool VerifySignature(byte[] signature) {
            return rsaDigestSigner.VerifySignature(signature);
        }

        /// <summary>Indicates whether some other object is "equal to" this one.</summary>
        /// <remarks>Indicates whether some other object is "equal to" this one. Compares wrapped objects.</remarks>
        public override bool Equals(Object o) {
            if (o == null || GetType() != o.GetType()) {
                return false;
            }
            iText.Bouncycastle.Rsa.RSADigestSignerBC that = (iText.Bouncycastle.Rsa.RSADigestSignerBC)o;
            return Object.Equals(rsaDigestSigner, that.rsaDigestSigner);
        }

        /// <summary>Returns a hash code value based on the wrapped object.</summary>
        public override int GetHashCode() {
            return JavaUtil.ArraysHashCode(rsaDigestSigner);
        }

        /// <summary>
        /// Delegates
        /// <c>toString</c>
        /// method call to the wrapped object.
        /// </summary>
        public override String ToString() {
            return rsaDigestSigner.ToString();
        }
    }
}
