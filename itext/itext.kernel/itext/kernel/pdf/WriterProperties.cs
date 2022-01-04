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
using Org.BouncyCastle.X509;

namespace iText.Kernel.Pdf {
    public class WriterProperties {
        protected internal int compressionLevel;

        /// <summary>Indicates if to use full compression (using object streams).</summary>
        protected internal bool? isFullCompression;

        /// <summary>Indicates if the writer copy objects in a smart mode.</summary>
        /// <remarks>
        /// Indicates if the writer copy objects in a smart mode. If so PdfDictionary and PdfStream will be hashed
        /// and reused if there's an object with the same content later.
        /// </remarks>
        protected internal bool smartMode;

        protected internal bool addXmpMetadata;

        protected internal bool addUAXmpMetadata;

        protected internal PdfVersion pdfVersion;

        protected internal EncryptionProperties encryptionProperties;

        /// <summary>The ID entry that represents the initial identifier.</summary>
        protected internal PdfString initialDocumentId;

        /// <summary>The ID entry that represents a change in a document.</summary>
        protected internal PdfString modifiedDocumentId;

        public WriterProperties() {
            smartMode = false;
            addUAXmpMetadata = false;
            compressionLevel = CompressionConstants.DEFAULT_COMPRESSION;
            isFullCompression = null;
            encryptionProperties = new EncryptionProperties();
        }

        /// <summary>Defines pdf version for the created document.</summary>
        /// <remarks>Defines pdf version for the created document. Default value is PDF_1_7.</remarks>
        /// <param name="version">version for the document.</param>
        /// <returns>
        /// this
        /// <see cref="WriterProperties"/>
        /// instance
        /// </returns>
        public virtual iText.Kernel.Pdf.WriterProperties SetPdfVersion(PdfVersion version) {
            this.pdfVersion = version;
            return this;
        }

        /// <summary>Enables smart mode.</summary>
        /// <remarks>
        /// Enables smart mode.
        /// <br />
        /// In smart mode when resources (such as fonts, images,...) are
        /// encountered, a reference to these resources is saved
        /// in a cache, so that they can be reused.
        /// This requires more memory, but reduces the file size
        /// of the resulting PDF document.
        /// </remarks>
        /// <returns>
        /// this
        /// <see cref="WriterProperties"/>
        /// instance
        /// </returns>
        public virtual iText.Kernel.Pdf.WriterProperties UseSmartMode() {
            this.smartMode = true;
            return this;
        }

        /// <summary>
        /// If true, default XMPMetadata based on
        /// <see cref="PdfDocumentInfo"/>
        /// will be added.
        /// </summary>
        /// <remarks>
        /// If true, default XMPMetadata based on
        /// <see cref="PdfDocumentInfo"/>
        /// will be added.
        /// For PDF 2.0 documents, metadata will be added in any case.
        /// </remarks>
        /// <returns>
        /// this
        /// <see cref="WriterProperties"/>
        /// instance
        /// </returns>
        public virtual iText.Kernel.Pdf.WriterProperties AddXmpMetadata() {
            this.addXmpMetadata = true;
            return this;
        }

        /// <summary>Defines the level of compression for the document.</summary>
        /// <remarks>
        /// Defines the level of compression for the document.
        /// See
        /// <see cref="CompressionConstants"/>
        /// </remarks>
        /// <param name="compressionLevel">
        /// 
        /// <see cref="CompressionConstants"/>
        /// value.
        /// </param>
        /// <returns>
        /// this
        /// <see cref="WriterProperties"/>
        /// instance
        /// </returns>
        public virtual iText.Kernel.Pdf.WriterProperties SetCompressionLevel(int compressionLevel) {
            this.compressionLevel = compressionLevel;
            return this;
        }

        /// <summary>Defines if full compression mode is enabled.</summary>
        /// <remarks>
        /// Defines if full compression mode is enabled. If enabled, not only the content of the pdf document will be
        /// compressed, but also the pdf document inner structure.
        /// </remarks>
        /// <param name="fullCompressionMode">true - to enable full compression mode, false to disable it</param>
        /// <returns>
        /// this
        /// <see cref="WriterProperties"/>
        /// instance
        /// </returns>
        public virtual iText.Kernel.Pdf.WriterProperties SetFullCompressionMode(bool fullCompressionMode) {
            this.isFullCompression = fullCompressionMode;
            return this;
        }

        /// <summary>Sets the encryption options for the document.</summary>
        /// <param name="userPassword">
        /// the user password. Can be null or of zero length, which is equal to
        /// omitting the user password
        /// </param>
        /// <param name="ownerPassword">
        /// the owner password. If it's null or empty, iText will generate
        /// a random string to be used as the owner password
        /// </param>
        /// <param name="permissions">
        /// the user permissions
        /// The open permissions for the document can be
        /// <see cref="EncryptionConstants.ALLOW_PRINTING"/>
        /// ,
        /// <see cref="EncryptionConstants.ALLOW_MODIFY_CONTENTS"/>
        /// ,
        /// <see cref="EncryptionConstants.ALLOW_COPY"/>
        /// ,
        /// <see cref="EncryptionConstants.ALLOW_MODIFY_ANNOTATIONS"/>
        /// ,
        /// <see cref="EncryptionConstants.ALLOW_FILL_IN"/>
        /// ,
        /// <see cref="EncryptionConstants.ALLOW_SCREENREADERS"/>
        /// ,
        /// <see cref="EncryptionConstants.ALLOW_ASSEMBLY"/>
        /// and
        /// <see cref="EncryptionConstants.ALLOW_DEGRADED_PRINTING"/>.
        /// The permissions can be combined by ORing them
        /// </param>
        /// <param name="encryptionAlgorithm">
        /// the type of encryption. It can be one of
        /// <see cref="EncryptionConstants.STANDARD_ENCRYPTION_40"/>
        /// ,
        /// <see cref="EncryptionConstants.STANDARD_ENCRYPTION_128"/>
        /// ,
        /// <see cref="EncryptionConstants.ENCRYPTION_AES_128"/>
        /// or
        /// <see cref="EncryptionConstants.ENCRYPTION_AES_256"/>.
        /// Optionally
        /// <see cref="EncryptionConstants.DO_NOT_ENCRYPT_METADATA"/>
        /// can be ORed
        /// to output the metadata in cleartext.
        /// <see cref="EncryptionConstants.EMBEDDED_FILES_ONLY"/>
        /// can be ORed as well.
        /// Please be aware that the passed encryption types may override permissions:
        /// <see cref="EncryptionConstants.STANDARD_ENCRYPTION_40"/>
        /// implicitly sets
        /// <see cref="EncryptionConstants.DO_NOT_ENCRYPT_METADATA"/>
        /// and
        /// <see cref="EncryptionConstants.EMBEDDED_FILES_ONLY"/>
        /// as false;
        /// <see cref="EncryptionConstants.STANDARD_ENCRYPTION_128"/>
        /// implicitly sets
        /// <see cref="EncryptionConstants.EMBEDDED_FILES_ONLY"/>
        /// as false;
        /// </param>
        /// <returns>
        /// this
        /// <see cref="WriterProperties"/>
        /// instance
        /// </returns>
        public virtual iText.Kernel.Pdf.WriterProperties SetStandardEncryption(byte[] userPassword, byte[] ownerPassword
            , int permissions, int encryptionAlgorithm) {
            encryptionProperties.SetStandardEncryption(userPassword, ownerPassword, permissions, encryptionAlgorithm);
            return this;
        }

        /// <summary>Sets the certificate encryption options for the document.</summary>
        /// <remarks>
        /// Sets the certificate encryption options for the document. An array of one or more public certificates
        /// must be provided together with an array of the same size for the permissions for each certificate.
        /// </remarks>
        /// <param name="certs">the public certificates to be used for the encryption</param>
        /// <param name="permissions">
        /// the user permissions for each of the certificates
        /// The open permissions for the document can be
        /// <see cref="EncryptionConstants.ALLOW_PRINTING"/>
        /// ,
        /// <see cref="EncryptionConstants.ALLOW_MODIFY_CONTENTS"/>
        /// ,
        /// <see cref="EncryptionConstants.ALLOW_COPY"/>
        /// ,
        /// <see cref="EncryptionConstants.ALLOW_MODIFY_ANNOTATIONS"/>
        /// ,
        /// <see cref="EncryptionConstants.ALLOW_FILL_IN"/>
        /// ,
        /// <see cref="EncryptionConstants.ALLOW_SCREENREADERS"/>
        /// ,
        /// <see cref="EncryptionConstants.ALLOW_ASSEMBLY"/>
        /// and
        /// <see cref="EncryptionConstants.ALLOW_DEGRADED_PRINTING"/>.
        /// The permissions can be combined by ORing them
        /// </param>
        /// <param name="encryptionAlgorithm">
        /// the type of encryption. It can be one of
        /// <see cref="EncryptionConstants.STANDARD_ENCRYPTION_40"/>
        /// ,
        /// <see cref="EncryptionConstants.STANDARD_ENCRYPTION_128"/>
        /// ,
        /// <see cref="EncryptionConstants.ENCRYPTION_AES_128"/>
        /// or
        /// <see cref="EncryptionConstants.ENCRYPTION_AES_256"/>.
        /// Optionally
        /// <see cref="EncryptionConstants.DO_NOT_ENCRYPT_METADATA"/>
        /// can be ORed
        /// to output the metadata in cleartext.
        /// <see cref="EncryptionConstants.EMBEDDED_FILES_ONLY"/>
        /// can be ORed as well.
        /// Please be aware that the passed encryption types may override permissions:
        /// <see cref="EncryptionConstants.STANDARD_ENCRYPTION_40"/>
        /// implicitly sets
        /// <see cref="EncryptionConstants.DO_NOT_ENCRYPT_METADATA"/>
        /// and
        /// <see cref="EncryptionConstants.EMBEDDED_FILES_ONLY"/>
        /// as false;
        /// <see cref="EncryptionConstants.STANDARD_ENCRYPTION_128"/>
        /// implicitly sets
        /// <see cref="EncryptionConstants.EMBEDDED_FILES_ONLY"/>
        /// as false;
        /// </param>
        /// <returns>
        /// this
        /// <see cref="WriterProperties"/>
        /// instance
        /// </returns>
        public virtual iText.Kernel.Pdf.WriterProperties SetPublicKeyEncryption(X509Certificate[] certs, int[] permissions
            , int encryptionAlgorithm) {
            encryptionProperties.SetPublicKeyEncryption(certs, permissions, encryptionAlgorithm);
            return this;
        }

        /// <summary>The /ID entry of a document contains an array with two entries.</summary>
        /// <remarks>
        /// The /ID entry of a document contains an array with two entries. The first one (initial id) represents the initial document id.
        /// It's a permanent identifier based on the contents of the file at the time it was originally created
        /// and does not change when the file is incrementally updated.
        /// To help ensure the uniqueness of file identifiers, it is recommend to be computed by means of a message digest algorithm such as MD5.
        /// iText will by default keep the existing initial id. But if you'd like you can set this id yourself using this setter.
        /// </remarks>
        /// <param name="initialDocumentId">the new initial document id</param>
        /// <returns>
        /// this
        /// <see cref="WriterProperties"/>
        /// instance
        /// </returns>
        public virtual iText.Kernel.Pdf.WriterProperties SetInitialDocumentId(PdfString initialDocumentId) {
            this.initialDocumentId = initialDocumentId;
            return this;
        }

        /// <summary>The /ID entry of a document contains an array with two entries.</summary>
        /// <remarks>
        /// The /ID entry of a document contains an array with two entries.
        /// The second one (modified id) should be the same entry, unless the document has been modified. iText will by default generate
        /// a modified id. But if you'd like you can set this id yourself using this setter.
        /// </remarks>
        /// <param name="modifiedDocumentId">the new modified document id</param>
        /// <returns>
        /// this
        /// <see cref="WriterProperties"/>
        /// instance
        /// </returns>
        public virtual iText.Kernel.Pdf.WriterProperties SetModifiedDocumentId(PdfString modifiedDocumentId) {
            this.modifiedDocumentId = modifiedDocumentId;
            return this;
        }

        /// <summary>This method marks the document as PDF/UA and sets related flags is XMPMetaData.</summary>
        /// <remarks>
        /// This method marks the document as PDF/UA and sets related flags is XMPMetaData.
        /// This method calls
        /// <see cref="AddXmpMetadata()"/>
        /// implicitly.
        /// NOTE: iText does not validate PDF/UA, which means we don't check if created PDF meets all PDF/UA requirements.
        /// Don't use this method if you are not familiar with PDF/UA specification in order to avoid creation of non-conformant PDF/UA file.
        /// </remarks>
        /// <returns>
        /// this
        /// <see cref="WriterProperties"/>
        /// instance
        /// </returns>
        public virtual iText.Kernel.Pdf.WriterProperties AddUAXmpMetadata() {
            this.addUAXmpMetadata = true;
            return AddXmpMetadata();
        }

        internal virtual bool IsStandardEncryptionUsed() {
            return encryptionProperties.IsStandardEncryptionUsed();
        }

        internal virtual bool IsPublicKeyEncryptionUsed() {
            return encryptionProperties.IsPublicKeyEncryptionUsed();
        }
    }
}
