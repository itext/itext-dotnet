/*
*
* This file is part of the iText (R) project.
Copyright (c) 1998-2022 iText Group NV
* Authors: Bruno Lowagie, Paulo Soares, et al.
*
* This program is free software; you can redistribute it and/or modify
* it under the terms of the GNU Affero General Public License version 3
* as published by the Free Software Foundation with the addition of the
* following permission added to Section 15 as permitted in Section 7(a):
* FOR ANY PART OF THE COVERED WORK IN WHICH THE COPYRIGHT IS OWNED BY
* ITEXT GROUP. ITEXT GROUP DISCLAIMS THE WARRANTY OF NON INFRINGEMENT
* OF THIRD PARTY RIGHTS
*
* This program is distributed in the hope that it will be useful, but
* WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
* or FITNESS FOR A PARTICULAR PURPOSE.
* See the GNU Affero General Public License for more details.
* You should have received a copy of the GNU Affero General Public License
* along with this program; if not, see http://www.gnu.org/licenses or write to
* the Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor,
* Boston, MA, 02110-1301 USA, or download the license from the following URL:
* http://itextpdf.com/terms-of-use/
*
* The interactive user interfaces in modified source and object code versions
* of this program must display Appropriate Legal Notices, as required under
* Section 5 of the GNU Affero General Public License.
*
* In accordance with Section 7(b) of the GNU Affero General Public License,
* a covered work must retain the producer line in every PDF that is created
* or manipulated using iText.
*
* You can be released from the requirements of the license by purchasing
* a commercial license. Buying such a license is mandatory as soon as you
* develop commercial activities involving the iText software without
* disclosing the source code of your own applications.
* These activities include: offering paid services to customers as an ASP,
* serving PDFs on the fly in a web application, shipping iText with a closed
* source product.
*
* For more information, please contact iText Software Corp. at this
* address: sales@itextpdf.com
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
