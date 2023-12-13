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

namespace iText.Signatures {
    /// <summary>
    /// Implementation of the
    /// <see cref="IExternalSignature"/>
    /// interface that
    /// can be used when you have a
    /// <see cref="Org.BouncyCastle.Crypto.ICipherParameters"/>
    /// object.
    /// </summary>
    /// <author>Paulo Soares</author>
    public class PrivateKeySignature : IExternalSignature {
        /// <summary>The private key object.</summary>
        private ICipherParameters pk;

        /// <summary>The hash algorithm.</summary>
        private String hashAlgorithm;

        /// <summary>The encryption algorithm (obtained from the private key)</summary>
        private String encryptionAlgorithm;

        /// <summary>
        /// Creates a
        /// <see cref="PrivateKeySignature"/>
        /// instance.
        /// </summary>
        /// <param name="pk">
        /// A
        /// <see cref="Org.BouncyCastle.Crypto.ICipherParameters"/>
        /// object.
        /// </param>
        /// <param name="hashAlgorithm">A hash algorithm (e.g. "SHA-1", "SHA-256",...).</param>
        /// <param name="provider">A security provider (e.g. "BC").</param>
        public PrivateKeySignature(ICipherParameters pk, String hashAlgorithm) {
            this.pk = pk;
            this.hashAlgorithm = DigestAlgorithms.GetDigest(DigestAlgorithms.GetAllowedDigest(hashAlgorithm));
            this.encryptionAlgorithm = SignUtils.GetPrivateKeyAlgorithm(pk);
        }

        /// <summary><inheritDoc/></summary>
        public virtual String GetHashAlgorithm() {
            return hashAlgorithm;
        }

        /// <summary><inheritDoc/></summary>
        public virtual String GetEncryptionAlgorithm() {
            return encryptionAlgorithm;
        }

        /// <summary><inheritDoc/></summary>
        public virtual byte[] Sign(byte[] message) {
            String algorithm = hashAlgorithm + "with" + encryptionAlgorithm;
            ISigner sig = SignUtils.GetSignatureHelper(algorithm);
            sig.InitSign(pk);
            sig.Update(message);
            return sig.GenerateSignature();
        }
    }
}
