/*

This file is part of the iText (R) project.
Copyright (c) 1998-2022 iText Group NV
Authors: Bruno Lowagie, Paulo Soares, et al.

This program is free software; you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License version 3
as published by the Free Software Foundation with the addition of the
following permission added to Section 15 as permitted in Section 7(a):
FOR ANY PART OF THE COVERED WORK IN WHICH THE COPYRIGHT IS OWNED BY
ITEXT GROUP. ITEXT GROUP DISCLAIMS THE WARRANTY OF NON INFRINGEMENT
OF THIRD PARTY RIGHTS

This program is distributed in the hope that it will be useful, but
WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
or FITNESS FOR A PARTICULAR PURPOSE.
See the GNU Affero General Public License for more details.
You should have received a copy of the GNU Affero General Public License
along with this program; if not, see http://www.gnu.org/licenses or write to
the Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor,
Boston, MA, 02110-1301 USA, or download the license from the following URL:
http://itextpdf.com/terms-of-use/

The interactive user interfaces in modified source and object code versions
of this program must display Appropriate Legal Notices, as required under
Section 5 of the GNU Affero General Public License.

In accordance with Section 7(b) of the GNU Affero General Public License,
a covered work must retain the producer line in every PDF that is created
or manipulated using iText.

You can be released from the requirements of the license by purchasing
a commercial license. Buying such a license is mandatory as soon as you
develop commercial activities involving the iText software without
disclosing the source code of your own applications.
These activities include: offering paid services to customers as an ASP,
serving PDFs on the fly in a web application, shipping iText with a closed
source product.

For more information, please contact iText Software Corp. at this
address: sales@itextpdf.com
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
