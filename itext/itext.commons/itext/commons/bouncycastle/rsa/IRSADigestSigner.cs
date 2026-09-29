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
using iText.Commons.Bouncycastle.Cert;

namespace iText.Commons.Bouncycastle.Rsa {
    /// <summary>Wrapper for BouncyCastle RSADigestSigner.</summary>
    public interface IRSADigestSigner {
        /// <summary>Initialize the signer for signing or verification.</summary>
        /// <param name="forSigning">
        /// 
        /// <see langword="true"/>
        /// if for signing,
        /// <see langword="false"/>
        /// otherwise
        /// </param>
        /// <param name="parameters">necessary parameters</param>
        void Init(bool forSigning, ICipherParams parameters);

        /// <summary>Update the internal digest with the byte array.</summary>
        /// <param name="input">the byte array to update the digest with</param>
        /// <param name="inOff">the offset in the byte array to start from</param>
        /// <param name="length">the number of bytes to update the digest with</param>
        void Update(byte[] input, int inOff, int length);

        /// <summary>
        /// Return
        /// <see langword="true"/>
        /// if the internal state represents the signature described in the updated array.
        /// </summary>
        /// <param name="signature">the signature to verify</param>
        /// <returns>
        /// 
        /// <see langword="true"/>
        /// if the signature is valid,
        /// <see langword="false"/>
        /// otherwise
        /// </returns>
        bool VerifySignature(byte[] signature);
    }
}
