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
using iText.Bouncycastleconnector;
using iText.Commons.Bouncycastle.Cert;
using iText.Kernel.Mac;

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

        protected internal PdfAConformance addPdfAXmpMetadata = null;

        protected internal PdfUAConformance addPdfUaXmpMetadata = null;

        protected internal PdfVersion pdfVersion;

        protected internal EncryptionProperties encryptionProperties;

        /// <summary>The ID entry that represents the initial identifier.</summary>
        protected internal PdfString initialDocumentId;

        /// <summary>The ID entry that represents a change in a document.</summary>
        protected internal PdfString modifiedDocumentId;

        public WriterProperties() {
            smartMode = false;
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

        /// <summary>Adds PDF/A XMP metadata to the PDF document.</summary>
        /// <remarks>
        /// Adds PDF/A XMP metadata to the PDF document.
        /// <para />
        /// This method calls
        /// <see cref="AddXmpMetadata()"/>
        /// implicitly.
        /// <para />
        /// NOTE: Calling this method only affects the XMP metadata, but doesn't enable any additional checks that the
        /// created document meets all PDF/A requirements. When using this method make sure you are familiar with PDF/A
        /// document requirements. If you are not sure, use dedicated iText PDF/A module to create valid PDF/A documents.
        /// </remarks>
        /// <param name="aConformance">the PDF/A conformance which will be added to XMP metadata</param>
        /// <returns>
        /// this
        /// <see cref="WriterProperties"/>
        /// instance
        /// </returns>
        public virtual iText.Kernel.Pdf.WriterProperties AddPdfAXmpMetadata(PdfAConformance aConformance) {
            this.addPdfAXmpMetadata = aConformance;
            AddXmpMetadata();
            return this;
        }

        /// <summary>Adds PDF/UA XMP metadata to the PDF document.</summary>
        /// <remarks>
        /// Adds PDF/UA XMP metadata to the PDF document.
        /// <para />
        /// This method calls
        /// <see cref="AddXmpMetadata()"/>
        /// implicitly.
        /// <para />
        /// NOTE: Calling this method only affects the XMP metadata, but doesn't enable any additional checks that the
        /// created document meets all PDF/UA requirements. When using this method make sure you are familiar with PDF/UA
        /// document requirements. If you are not sure, use dedicated iText PDF/UA module to create valid PDF/UA documents.
        /// </remarks>
        /// <param name="uaConformance">the PDF/UA conformance which will be added to XMP metadata</param>
        /// <returns>
        /// this
        /// <see cref="WriterProperties"/>
        /// instance
        /// </returns>
        public virtual iText.Kernel.Pdf.WriterProperties AddPdfUaXmpMetadata(PdfUAConformance uaConformance) {
            this.addPdfUaXmpMetadata = uaConformance;
            AddXmpMetadata();
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
            return SetStandardEncryption(userPassword, ownerPassword, permissions, encryptionAlgorithm, EncryptionProperties
                .DEFAULT_MAC_PROPERTIES);
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
        /// <param name="macProperties">
        /// 
        /// <see cref="iText.Kernel.Mac.MacProperties"/>
        /// class to configure MAC integrity protection properties.
        /// Pass
        /// <see langword="null"/>
        /// if you want to disable MAC protection for any reason
        /// </param>
        /// <returns>
        /// this
        /// <see cref="WriterProperties"/>
        /// instance
        /// </returns>
        public virtual iText.Kernel.Pdf.WriterProperties SetStandardEncryption(byte[] userPassword, byte[] ownerPassword
            , int permissions, int encryptionAlgorithm, MacProperties macProperties) {
            encryptionProperties.SetStandardEncryption(userPassword, ownerPassword, permissions, encryptionAlgorithm, 
                macProperties);
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
        public virtual iText.Kernel.Pdf.WriterProperties SetPublicKeyEncryption(IX509Certificate[] certs, int[] permissions
            , int encryptionAlgorithm) {
            return SetPublicKeyEncryption(certs, permissions, encryptionAlgorithm, EncryptionProperties.DEFAULT_MAC_PROPERTIES
                );
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
        /// <param name="macProperties">
        /// 
        /// <see cref="iText.Kernel.Mac.MacProperties"/>
        /// class to configure MAC integrity protection properties.
        /// Pass
        /// <see langword="null"/>
        /// if you want to disable MAC protection for any reason
        /// </param>
        /// <returns>
        /// this
        /// <see cref="WriterProperties"/>
        /// instance
        /// </returns>
        public virtual iText.Kernel.Pdf.WriterProperties SetPublicKeyEncryption(IX509Certificate[] certs, int[] permissions
            , int encryptionAlgorithm, MacProperties macProperties) {
            BouncyCastleFactoryCreator.GetFactory().IsEncryptionFeatureSupported(encryptionAlgorithm, true);
            encryptionProperties.SetPublicKeyEncryption(certs, permissions, encryptionAlgorithm, macProperties);
            return this;
        }

        /// <summary>The /ID entry of a document contains an array with two entries.</summary>
        /// <remarks>
        /// The /ID entry of a document contains an array with two entries.
        /// The first one (initial id) represents the initial document id.
        /// It's a permanent identifier based on the contents of the file at the time it was originally created
        /// and does not change when the file is incrementally updated.
        /// To help ensure the uniqueness of file identifiers,
        /// it is recommended to be computed by means of a message digest algorithm such as MD5.
        /// iText will by default keep the existing initial id.
        /// But if you'd like you can set this id yourself using this setter.
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

//\cond DO_NOT_DOCUMENT
        internal virtual bool IsStandardEncryptionUsed() {
            return encryptionProperties.IsStandardEncryptionUsed();
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal virtual bool IsPublicKeyEncryptionUsed() {
            return encryptionProperties.IsPublicKeyEncryptionUsed();
        }
//\endcond
    }
}
