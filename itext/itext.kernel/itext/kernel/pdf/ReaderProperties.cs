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
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Bouncycastle.Crypto;

namespace iText.Kernel.Pdf {
    public class ReaderProperties {
        //added by ujihara for decryption
        protected internal byte[] password;

        //added by Aiken Sam for certificate decryption
        protected internal IPrivateKey certificateKey;

        //added by Aiken Sam for certificate decryption
        protected internal IX509Certificate certificate;

        //added by Aiken Sam for certificate decryption
        protected internal MemoryLimitsAwareHandler memoryLimitsAwareHandler;

        /// <summary>Defines the password which will be used if the document is encrypted with standard encryption.</summary>
        /// <remarks>
        /// Defines the password which will be used if the document is encrypted with standard encryption.
        /// This could be either user or owner password.
        /// </remarks>
        /// <param name="password">the password to use in order to open the document</param>
        /// <returns>
        /// this
        /// <see cref="ReaderProperties"/>
        /// instance
        /// </returns>
        public virtual ReaderProperties SetPassword(byte[] password) {
            ClearEncryptionParams();
            this.password = password;
            return this;
        }

        /// <summary>
        /// Defines the certificate which will be used if the document is encrypted with public key
        /// encryption (see Pdf 1.7 specification, 7.6.4. Public-Key Security Handlers)
        /// </summary>
        /// <param name="certificate">
        /// the recipient
        /// <see cref="iText.Commons.Bouncycastle.Cert.IX509Certificate"/>
        /// ,
        /// serves as recipient identifier
        /// </param>
        /// <param name="certificateKey">
        /// the recipient private
        /// <see cref="iText.Commons.Bouncycastle.Crypto.IPrivateKey"/>
        /// to the certificate
        /// </param>
        /// <returns>
        /// this
        /// <see cref="ReaderProperties"/>
        /// instance
        /// </returns>
        public virtual ReaderProperties SetPublicKeySecurityParams(IX509Certificate certificate, IPrivateKey certificateKey
            ) {
            ClearEncryptionParams();
            this.certificate = certificate;
            this.certificateKey = certificateKey;
            return this;
        }

        /// <summary>
        /// Defines the certificate which will be used if the document is encrypted with public key
        /// encryption (see Pdf 1.7 specification, 7.6.4. Public-Key Security Handlers)
        /// </summary>
        /// <param name="certificate">
        /// the recipient
        /// <see cref="iText.Commons.Bouncycastle.Cert.IX509Certificate"/>
        /// ,
        /// serves as recipient identifier
        /// </param>
        /// <returns>
        /// this
        /// <see cref="ReaderProperties"/>
        /// instance
        /// </returns>
        public virtual ReaderProperties SetPublicKeySecurityParams(IX509Certificate certificate) {
            ClearEncryptionParams();
            this.certificate = certificate;
            return this;
        }

        private void ClearEncryptionParams() {
            this.password = null;
            this.certificate = null;
            this.certificateKey = null;
        }

        /// <summary>Sets the memory handler which will be used to handle decompressed PDF streams.</summary>
        /// <param name="memoryLimitsAwareHandler">the memory handler which will be used to handle decompressed PDF streams
        ///     </param>
        /// <returns>
        /// this
        /// <see cref="ReaderProperties"/>
        /// instance
        /// </returns>
        public virtual ReaderProperties SetMemoryLimitsAwareHandler(MemoryLimitsAwareHandler memoryLimitsAwareHandler
            ) {
            this.memoryLimitsAwareHandler = memoryLimitsAwareHandler;
            return this;
        }
    }
}
