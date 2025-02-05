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
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Bouncycastle.Crypto;
using iText.Commons.Utils;

namespace iText.Kernel.Pdf {
    /// <summary>The class representing various properties used to read PDF documents.</summary>
    public class ReaderProperties {
        protected internal byte[] password;

        protected internal IPrivateKey certificateKey;

        protected internal IX509Certificate certificate;

        protected internal MemoryLimitsAwareHandler memoryLimitsAwareHandler;

        /// <summary>
        /// Creates an instance of
        /// <see cref="ReaderProperties"/>.
        /// </summary>
        public ReaderProperties() {
        }

//\cond DO_NOT_DOCUMENT
        // Empty constructor
        internal ReaderProperties(iText.Kernel.Pdf.ReaderProperties readerProperties) {
            this.password = readerProperties.password == null ? null : JavaUtil.ArraysCopyOf(readerProperties.password
                , readerProperties.password.Length);
            this.certificateKey = readerProperties.certificateKey;
            this.certificate = readerProperties.certificate;
            this.memoryLimitsAwareHandler = readerProperties.memoryLimitsAwareHandler == null ? null : readerProperties
                .memoryLimitsAwareHandler.CreateNewInstance();
        }
//\endcond

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
        public virtual iText.Kernel.Pdf.ReaderProperties SetPassword(byte[] password) {
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
        public virtual iText.Kernel.Pdf.ReaderProperties SetPublicKeySecurityParams(IX509Certificate certificate, 
            IPrivateKey certificateKey) {
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
        public virtual iText.Kernel.Pdf.ReaderProperties SetPublicKeySecurityParams(IX509Certificate certificate) {
            ClearEncryptionParams();
            this.certificate = certificate;
            return this;
        }

        /// <summary>Sets the memory handler which will be used to handle decompressed PDF streams.</summary>
        /// <param name="memoryLimitsAwareHandler">the memory handler which will be used to handle decompressed PDF streams
        ///     </param>
        /// <returns>
        /// this
        /// <see cref="ReaderProperties"/>
        /// instance
        /// </returns>
        public virtual iText.Kernel.Pdf.ReaderProperties SetMemoryLimitsAwareHandler(MemoryLimitsAwareHandler memoryLimitsAwareHandler
            ) {
            this.memoryLimitsAwareHandler = memoryLimitsAwareHandler;
            return this;
        }

        private void ClearEncryptionParams() {
            this.password = null;
            this.certificate = null;
            this.certificateKey = null;
        }
    }
}
