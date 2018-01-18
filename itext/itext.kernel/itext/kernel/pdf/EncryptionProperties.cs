/*

This file is part of the iText (R) project.
Copyright (c) 1998-2018 iText Group NV
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
using System.Security.Cryptography;
using Org.BouncyCastle.X509;

namespace iText.Kernel.Pdf {
    public class EncryptionProperties {
        protected internal int encryptionAlgorithm;

        /// <summary>StandardEncryption properties</summary>
        protected internal byte[] userPassword;

        protected internal byte[] ownerPassword;

        protected internal int standardEncryptPermissions;

        /// <summary>PublicKeyEncryption properties</summary>
        protected internal X509Certificate[] publicCertificates;

        protected internal int[] publicKeyEncryptPermissions;

        /// <summary>Sets the encryption options for the document.</summary>
        /// <remarks>
        /// Sets the encryption options for the document. The userPassword and the
        /// ownerPassword can be null or have zero length. In this case the ownerPassword
        /// is replaced by a random string. The open permissions for the document can be
        /// ALLOW_PRINTING, ALLOW_MODIFY_CONTENTS, ALLOW_COPY, ALLOW_MODIFY_ANNOTATIONS,
        /// ALLOW_FILL_IN, ALLOW_SCREENREADERS, ALLOW_ASSEMBLY and ALLOW_DEGRADED_PRINTING.
        /// The permissions can be combined by ORing them.
        /// See
        /// <see cref="EncryptionConstants"/>
        /// .
        /// </remarks>
        /// <param name="userPassword">the user password. Can be null or empty</param>
        /// <param name="ownerPassword">the owner password. Can be null or empty</param>
        /// <param name="permissions">the user permissions</param>
        /// <param name="encryptionAlgorithm">
        /// the type of encryption. It can be one of STANDARD_ENCRYPTION_40, STANDARD_ENCRYPTION_128,
        /// ENCRYPTION_AES128 or ENCRYPTION_AES256
        /// Optionally DO_NOT_ENCRYPT_METADATA can be ored to output the metadata in cleartext
        /// </param>
        public virtual EncryptionProperties SetStandardEncryption(byte[] userPassword, byte[] ownerPassword, int permissions
            , int encryptionAlgorithm) {
            ClearEncryption();
            this.userPassword = userPassword;
            if (ownerPassword != null) {
                this.ownerPassword = ownerPassword;
            }
            else {
                this.ownerPassword = new byte[16];
                RandomBytes(this.ownerPassword);
            }
            this.standardEncryptPermissions = permissions;
            this.encryptionAlgorithm = encryptionAlgorithm;
            return this;
        }

        /// <summary>Sets the certificate encryption options for the document.</summary>
        /// <remarks>
        /// Sets the certificate encryption options for the document. An array of one or more public certificates
        /// must be provided together with an array of the same size for the permissions for each certificate.
        /// The open permissions for the document can be
        /// AllowPrinting, AllowModifyContents, AllowCopy, AllowModifyAnnotations,
        /// AllowFillIn, AllowScreenReaders, AllowAssembly and AllowDegradedPrinting.
        /// The permissions can be combined by ORing them.
        /// Optionally DO_NOT_ENCRYPT_METADATA can be ORed to output the metadata in cleartext
        /// See
        /// <see cref="EncryptionConstants"/>
        /// .
        /// </remarks>
        /// <param name="certs">the public certificates to be used for the encryption</param>
        /// <param name="permissions">the user permissions for each of the certificates</param>
        /// <param name="encryptionAlgorithm">
        /// the type of encryption. It can be one of STANDARD_ENCRYPTION_40, STANDARD_ENCRYPTION_128,
        /// ENCRYPTION_AES128 or ENCRYPTION_AES256.
        /// </param>
        public virtual EncryptionProperties SetPublicKeyEncryption(X509Certificate[] certs, int[] permissions, int
             encryptionAlgorithm) {
            ClearEncryption();
            this.publicCertificates = certs;
            this.publicKeyEncryptPermissions = permissions;
            this.encryptionAlgorithm = encryptionAlgorithm;
            return this;
        }

        internal virtual bool IsStandardEncryptionUsed() {
            return ownerPassword != null;
        }

        internal virtual bool IsPublicKeyEncryptionUsed() {
            return publicCertificates != null;
        }

        private void ClearEncryption() {
            this.publicCertificates = null;
            this.publicKeyEncryptPermissions = null;
            this.userPassword = null;
            this.ownerPassword = null;
        }

        private static void RandomBytes(byte[] bytes) {
            RandomNumberGenerator.Create().GetBytes(bytes);
        }
    }
}
