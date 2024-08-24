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
using System;
using System.IO;
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Bouncycastle.Crypto;
using iText.Commons.Digest;
using iText.Commons.Utils;
using iText.IO.Source;
using iText.Kernel.Crypto;
using iText.Kernel.Crypto.Securityhandler;
using iText.Kernel.Exceptions;
using iText.Kernel.Mac;

namespace iText.Kernel.Pdf {
    public class PdfEncryption : PdfObjectWrapper<PdfDictionary> {
        private const String MAC_FOR_PDF_2 = "MAC integrity protection is only supported for PDF 2.0 or higher.";

        private const String MAC_FOR_ENCRYPTION_5 = "MAC integrity protection is only supported for encryption algorithms of version 5 or higher.";

        private const int STANDARD_ENCRYPTION_40 = 2;

        private const int STANDARD_ENCRYPTION_128 = 3;

        private const int AES_128 = 4;

        private const int AES_256 = 5;

        private const int DEFAULT_KEY_LENGTH = 40;

        private static long seq = SystemUtil.GetTimeBasedSeed();

        private int cryptoMode;

        private long? permissions;

        private bool encryptMetadata;

        private bool embeddedFilesOnly;

        private byte[] documentId;

        private SecurityHandler securityHandler;

        private MacIntegrityProtector mac;

        /// <summary>Creates the encryption.</summary>
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
        /// <param name="encryptionType">
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
        /// can be
        /// ORed to output the metadata in cleartext.
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
        /// <param name="documentId">document id which will be used for encryption</param>
        /// <param name="version">
        /// the
        /// <see cref="PdfVersion"/>
        /// of the target document for encryption
        /// </param>
        public PdfEncryption(byte[] userPassword, byte[] ownerPassword, int permissions, int encryptionType, byte[]
             documentId, PdfVersion version)
            : this(userPassword, ownerPassword, permissions, encryptionType, documentId, version, null) {
        }

        /// <summary>Creates the encryption.</summary>
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
        /// <param name="encryptionType">
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
        /// can be
        /// ORed to output the metadata in cleartext.
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
        /// <param name="documentId">document id which will be used for encryption</param>
        /// <param name="version">
        /// the
        /// <see cref="PdfVersion"/>
        /// of the target document for encryption
        /// </param>
        /// <param name="mac">
        /// 
        /// <see cref="iText.Kernel.Mac.MacIntegrityProtector"/>
        /// class for MAC integrity protection
        /// </param>
        public PdfEncryption(byte[] userPassword, byte[] ownerPassword, int permissions, int encryptionType, byte[]
             documentId, PdfVersion version, MacIntegrityProtector mac)
            : base(new PdfDictionary()) {
            this.mac = mac;
            this.documentId = documentId;
            if (version != null && version.CompareTo(PdfVersion.PDF_2_0) >= 0) {
                permissions = FixAccessibilityPermissionPdf20(permissions);
                if (mac != null) {
                    permissions = ConfigureAccessibilityPermissionsForMac(permissions);
                }
            }
            int revision = SetCryptoMode(encryptionType);
            switch (revision) {
                case STANDARD_ENCRYPTION_40: {
                    StandardHandlerUsingStandard40 handlerStd40 = new StandardHandlerUsingStandard40(this.GetPdfObject(), userPassword
                        , ownerPassword, permissions, encryptMetadata, embeddedFilesOnly, documentId);
                    this.permissions = handlerStd40.GetPermissions();
                    securityHandler = handlerStd40;
                    break;
                }

                case STANDARD_ENCRYPTION_128: {
                    StandardHandlerUsingStandard128 handlerStd128 = new StandardHandlerUsingStandard128(this.GetPdfObject(), userPassword
                        , ownerPassword, permissions, encryptMetadata, embeddedFilesOnly, documentId);
                    this.permissions = handlerStd128.GetPermissions();
                    securityHandler = handlerStd128;
                    break;
                }

                case AES_128: {
                    StandardHandlerUsingAes128 handlerAes128 = new StandardHandlerUsingAes128(this.GetPdfObject(), userPassword
                        , ownerPassword, permissions, encryptMetadata, embeddedFilesOnly, documentId);
                    this.permissions = handlerAes128.GetPermissions();
                    securityHandler = handlerAes128;
                    break;
                }

                case AES_256: {
                    StandardHandlerUsingAes256 handlerAes256 = new StandardHandlerUsingAes256(this.GetPdfObject(), userPassword
                        , ownerPassword, permissions, encryptMetadata, embeddedFilesOnly, version);
                    this.permissions = handlerAes256.GetPermissions();
                    securityHandler = handlerAes256;
                    break;
                }
            }
        }

        /// <summary>Creates the certificate encryption.</summary>
        /// <remarks>
        /// Creates the certificate encryption.
        /// <para />
        /// An array of one or more public certificates must be provided together with
        /// an array of the same size for the permissions for each certificate.
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
        /// <param name="encryptionType">
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
        /// <param name="version">
        /// the
        /// <see cref="PdfVersion"/>
        /// of the target document for encryption
        /// </param>
        public PdfEncryption(IX509Certificate[] certs, int[] permissions, int encryptionType, PdfVersion version)
            : this(certs, permissions, encryptionType, version, null) {
        }

        /// <summary>Creates the certificate encryption.</summary>
        /// <remarks>
        /// Creates the certificate encryption.
        /// <para />
        /// An array of one or more public certificates must be provided together with
        /// an array of the same size for the permissions for each certificate.
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
        /// <param name="encryptionType">
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
        /// <param name="version">
        /// the
        /// <see cref="PdfVersion"/>
        /// of the target document for encryption
        /// </param>
        /// <param name="mac">
        /// 
        /// <see cref="iText.Kernel.Mac.MacIntegrityProtector"/>
        /// class for MAC integrity protection
        /// </param>
        public PdfEncryption(IX509Certificate[] certs, int[] permissions, int encryptionType, PdfVersion version, 
            MacIntegrityProtector mac)
            : base(new PdfDictionary()) {
            this.mac = mac;
            if (version != null && version.CompareTo(PdfVersion.PDF_2_0) >= 0) {
                for (int i = 0; i < permissions.Length; i++) {
                    permissions[i] = FixAccessibilityPermissionPdf20(permissions[i]);
                    if (mac != null) {
                        permissions[i] = ConfigureAccessibilityPermissionsForMac(permissions[i]);
                    }
                }
            }
            int revision = SetCryptoMode(encryptionType);
            switch (revision) {
                case STANDARD_ENCRYPTION_40: {
                    securityHandler = new PubSecHandlerUsingStandard40(this.GetPdfObject(), certs, permissions, encryptMetadata
                        , embeddedFilesOnly);
                    break;
                }

                case STANDARD_ENCRYPTION_128: {
                    securityHandler = new PubSecHandlerUsingStandard128(this.GetPdfObject(), certs, permissions, encryptMetadata
                        , embeddedFilesOnly);
                    break;
                }

                case AES_128: {
                    securityHandler = new PubSecHandlerUsingAes128(this.GetPdfObject(), certs, permissions, encryptMetadata, embeddedFilesOnly
                        );
                    break;
                }

                case AES_256: {
                    securityHandler = new PubSecHandlerUsingAes256(this.GetPdfObject(), certs, permissions, encryptMetadata, embeddedFilesOnly
                        );
                    break;
                }
            }
        }

        public PdfEncryption(PdfDictionary pdfDict, byte[] password, byte[] documentId)
            : base(pdfDict) {
            SetForbidRelease();
            this.documentId = documentId;
            int revision = ReadAndSetCryptoModeForStdHandler(pdfDict);
            switch (revision) {
                case STANDARD_ENCRYPTION_40: {
                    StandardHandlerUsingStandard40 handlerStd40 = new StandardHandlerUsingStandard40(this.GetPdfObject(), password
                        , documentId, encryptMetadata);
                    permissions = handlerStd40.GetPermissions();
                    securityHandler = handlerStd40;
                    break;
                }

                case STANDARD_ENCRYPTION_128: {
                    StandardHandlerUsingStandard128 handlerStd128 = new StandardHandlerUsingStandard128(this.GetPdfObject(), password
                        , documentId, encryptMetadata);
                    permissions = handlerStd128.GetPermissions();
                    securityHandler = handlerStd128;
                    break;
                }

                case AES_128: {
                    StandardHandlerUsingAes128 handlerAes128 = new StandardHandlerUsingAes128(this.GetPdfObject(), password, documentId
                        , encryptMetadata);
                    permissions = handlerAes128.GetPermissions();
                    securityHandler = handlerAes128;
                    break;
                }

                case AES_256: {
                    StandardHandlerUsingAes256 aes256Handler = new StandardHandlerUsingAes256(this.GetPdfObject(), password);
                    permissions = aes256Handler.GetPermissions();
                    encryptMetadata = aes256Handler.IsEncryptMetadata();
                    securityHandler = aes256Handler;
                    break;
                }
            }
        }

        public PdfEncryption(PdfDictionary pdfDict, IPrivateKey certificateKey, IX509Certificate certificate)
            : base(pdfDict) {
            SetForbidRelease();
            int revision = ReadAndSetCryptoModeForPubSecHandler(pdfDict);
            switch (revision) {
                case STANDARD_ENCRYPTION_40: {
                    securityHandler = new PubSecHandlerUsingStandard40(this.GetPdfObject(), certificateKey, certificate, encryptMetadata
                        );
                    break;
                }

                case STANDARD_ENCRYPTION_128: {
                    securityHandler = new PubSecHandlerUsingStandard128(this.GetPdfObject(), certificateKey, certificate, encryptMetadata
                        );
                    break;
                }

                case AES_128: {
                    securityHandler = new PubSecHandlerUsingAes128(this.GetPdfObject(), certificateKey, certificate, encryptMetadata
                        );
                    break;
                }

                case AES_256: {
                    securityHandler = new PubSecHandlerUsingAes256(this.GetPdfObject(), certificateKey, certificate, encryptMetadata
                        );
                    break;
                }
            }
        }

        public static byte[] GenerateNewDocumentId() {
            IMessageDigest sha512;
            try {
                sha512 = iText.Bouncycastleconnector.BouncyCastleFactoryCreator.GetFactory().CreateIDigest("SHA-512");
            }
            catch (Exception e) {
                throw new PdfException(KernelExceptionMessageConstant.PDF_ENCRYPTION, e);
            }
            long time = SystemUtil.GetTimeBasedSeed();
            long mem = SystemUtil.GetFreeMemory();
            String s = time + "+" + mem + "+" + (seq++);
            return sha512.Digest(s.GetBytes(iText.Commons.Utils.EncodingUtil.ISO_8859_1));
        }

        /// <summary>Creates a PdfLiteral that contains an array of two id entries.</summary>
        /// <remarks>
        /// Creates a PdfLiteral that contains an array of two id entries. These entries are both hexadecimal
        /// strings containing 16 hex characters. The first entry is the original id, the second entry
        /// should be different from the first one if the document has changed.
        /// </remarks>
        /// <param name="id">the first id</param>
        /// <param name="modified">whether the document has been changed or not</param>
        /// <returns>PdfObject containing the two entries</returns>
        public static PdfObject CreateInfoId(byte[] id, bool modified) {
            if (modified) {
                return CreateInfoId(id, GenerateNewDocumentId(), false);
            }
            else {
                return CreateInfoId(id, id, false);
            }
        }

        /// <summary>Creates a PdfLiteral that contains an array of two id entries.</summary>
        /// <remarks>
        /// Creates a PdfLiteral that contains an array of two id entries. These entries are both hexadecimal
        /// strings containing up to 16 hex characters. The first entry is the original id, the second entry
        /// should be different from the first one if the document has changed.
        /// </remarks>
        /// <param name="firstId">the first id</param>
        /// <param name="secondId">the second id</param>
        /// <param name="preserveEncryption">the encryption preserve</param>
        /// <returns>PdfObject containing the two entries.</returns>
        public static PdfObject CreateInfoId(byte[] firstId, byte[] secondId, bool preserveEncryption) {
            if (!preserveEncryption) {
                if (firstId.Length < 16) {
                    firstId = PadByteArrayTo16(firstId);
                }
                if (secondId.Length < 16) {
                    secondId = PadByteArrayTo16(secondId);
                }
            }
            ByteBuffer buf = new ByteBuffer(90);
            buf.Append('[').Append('<');
            for (int k = 0; k < firstId.Length; ++k) {
                buf.AppendHex(firstId[k]);
            }
            buf.Append('>').Append('<');
            for (int k = 0; k < secondId.Length; ++k) {
                buf.AppendHex(secondId[k]);
            }
            buf.Append('>').Append(']');
            return new PdfLiteral(buf.ToByteArray());
        }

        private static byte[] PadByteArrayTo16(byte[] documentId) {
            byte[] paddingBytes = new byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 };
            Array.Copy(documentId, 0, paddingBytes, 0, documentId.Length);
            return paddingBytes;
        }

        /// <summary>Gets the encryption permissions.</summary>
        /// <remarks>
        /// Gets the encryption permissions. It can be used directly in
        /// <see cref="WriterProperties.SetStandardEncryption(byte[], byte[], int, int)"/>.
        /// See ISO 32000-1, Table 22 for more details.
        /// </remarks>
        /// <returns>the encryption permissions, an unsigned 32-bit quantity.</returns>
        public virtual long? GetPermissions() {
            return permissions;
        }

        /// <summary>Gets encryption algorithm and access permissions.</summary>
        /// <returns>the crypto mode value</returns>
        /// <seealso cref="EncryptionConstants"/>
        public virtual int GetCryptoMode() {
            return cryptoMode;
        }

        public virtual bool IsMetadataEncrypted() {
            return encryptMetadata;
        }

        public virtual bool IsEmbeddedFilesOnly() {
            return embeddedFilesOnly;
        }

        /// <returns>document id which was used for encryption. Could be null, if encryption doesn't rely on document id.
        ///     </returns>
        public virtual byte[] GetDocumentId() {
            return documentId;
        }

        public virtual void SetHashKeyForNextObject(int objNumber, int objGeneration) {
            securityHandler.SetHashKeyForNextObject(objNumber, objGeneration);
        }

        public virtual OutputStreamEncryption GetEncryptionStream(Stream os) {
            return securityHandler.GetEncryptionStream(os);
        }

        public virtual byte[] EncryptByteArray(byte[] b) {
            MemoryStream ba = new MemoryStream();
            OutputStreamEncryption ose = GetEncryptionStream(ba);
            try {
                ose.Write(b);
            }
            catch (System.IO.IOException e) {
                throw new PdfException(KernelExceptionMessageConstant.PDF_ENCRYPTION, e);
            }
            ose.Finish();
            return ba.ToArray();
        }

        public virtual byte[] DecryptByteArray(byte[] b) {
            try {
                MemoryStream ba = new MemoryStream();
                IDecryptor dec = securityHandler.GetDecryptor();
                byte[] b2 = dec.Update(b, 0, b.Length);
                if (b2 != null) {
                    ba.Write(b2);
                }
                b2 = dec.Finish();
                if (b2 != null) {
                    ba.Write(b2);
                }
                return ba.ToArray();
            }
            catch (System.IO.IOException e) {
                throw new PdfException(KernelExceptionMessageConstant.PDF_ENCRYPTION, e);
            }
        }

        public virtual bool IsOpenedWithFullPermission() {
            if (securityHandler is PubKeySecurityHandler) {
                return true;
            }
            else {
                if (securityHandler is StandardSecurityHandler) {
                    return ((StandardSecurityHandler)securityHandler).IsUsedOwnerPassword();
                }
            }
            return true;
        }

        /// <summary>Computes user password if standard encryption handler is used with Standard40, Standard128 or AES128 algorithm.
        ///     </summary>
        /// <param name="ownerPassword">owner password of the encrypted document.</param>
        /// <returns>user password, or null if not a standard encryption handler was used.</returns>
        public virtual byte[] ComputeUserPassword(byte[] ownerPassword) {
            byte[] userPassword = null;
            if (securityHandler is StandardHandlerUsingStandard40) {
                userPassword = ((StandardHandlerUsingStandard40)securityHandler).ComputeUserPassword(ownerPassword, GetPdfObject
                    ());
            }
            return userPassword;
        }

        /// <summary>
        /// To manually flush a
        /// <c>PdfObject</c>
        /// behind this wrapper, you have to ensure
        /// that this object is added to the document, i.e. it has an indirect reference.
        /// </summary>
        /// <remarks>
        /// To manually flush a
        /// <c>PdfObject</c>
        /// behind this wrapper, you have to ensure
        /// that this object is added to the document, i.e. it has an indirect reference.
        /// Basically this means that before flushing you need to explicitly call
        /// <see cref="PdfObjectWrapper{T}.MakeIndirect(PdfDocument)"/>.
        /// For example: wrapperInstance.makeIndirect(document).flush();
        /// Note that not every wrapper require this, only those that have such warning in documentation.
        /// </remarks>
        public override void Flush() {
            base.Flush();
        }

        protected internal override bool IsWrappedObjectMustBeIndirect() {
            return true;
        }

        private void SetKeyLength(int keyLength) {
            if (keyLength != DEFAULT_KEY_LENGTH) {
                GetPdfObject().Put(PdfName.Length, new PdfNumber(keyLength));
            }
        }

        private int SetCryptoMode(int mode) {
            return SetCryptoMode(mode, 0);
        }

        private int SetCryptoMode(int mode, int length) {
            int revision;
            cryptoMode = mode;
            encryptMetadata = (mode & EncryptionConstants.DO_NOT_ENCRYPT_METADATA) != EncryptionConstants.DO_NOT_ENCRYPT_METADATA;
            embeddedFilesOnly = (mode & EncryptionConstants.EMBEDDED_FILES_ONLY) == EncryptionConstants.EMBEDDED_FILES_ONLY;
            mode &= EncryptionConstants.ENCRYPTION_MASK;
            switch (mode) {
                case EncryptionConstants.STANDARD_ENCRYPTION_40: {
                    encryptMetadata = true;
                    embeddedFilesOnly = false;
                    SetKeyLength(40);
                    revision = STANDARD_ENCRYPTION_40;
                    break;
                }

                case EncryptionConstants.STANDARD_ENCRYPTION_128: {
                    if (length > 0) {
                        SetKeyLength(length);
                    }
                    else {
                        SetKeyLength(128);
                    }
                    revision = STANDARD_ENCRYPTION_128;
                    break;
                }

                case EncryptionConstants.ENCRYPTION_AES_128: {
                    SetKeyLength(128);
                    revision = AES_128;
                    break;
                }

                case EncryptionConstants.ENCRYPTION_AES_256: {
                    SetKeyLength(256);
                    revision = AES_256;
                    break;
                }

                default: {
                    throw new PdfException(KernelExceptionMessageConstant.NO_VALID_ENCRYPTION_MODE);
                }
            }
            return revision;
        }

        private int ReadAndSetCryptoModeForStdHandler(PdfDictionary encDict) {
            int cryptoMode;
            int length = 0;
            PdfNumber rValue = encDict.GetAsNumber(PdfName.R);
            if (rValue == null) {
                throw new PdfException(KernelExceptionMessageConstant.ILLEGAL_R_VALUE);
            }
            int revision = rValue.IntValue();
            bool embeddedFilesOnlyMode = ReadEmbeddedFilesOnlyFromEncryptDictionary(encDict);
            switch (revision) {
                case 2: {
                    cryptoMode = EncryptionConstants.STANDARD_ENCRYPTION_40;
                    break;
                }

                case 3: {
                    PdfNumber lengthValue = encDict.GetAsNumber(PdfName.Length);
                    length = lengthValue == null ? DEFAULT_KEY_LENGTH : lengthValue.IntValue();
                    if (length > 128 || length < 40 || length % 8 != 0) {
                        throw new PdfException(KernelExceptionMessageConstant.ILLEGAL_LENGTH_VALUE);
                    }
                    cryptoMode = EncryptionConstants.STANDARD_ENCRYPTION_128;
                    break;
                }

                case 4: {
                    PdfDictionary dic = (PdfDictionary)encDict.Get(PdfName.CF);
                    if (dic == null) {
                        throw new PdfException(KernelExceptionMessageConstant.CF_NOT_FOUND_ENCRYPTION);
                    }
                    dic = (PdfDictionary)dic.Get(PdfName.StdCF);
                    if (dic == null) {
                        throw new PdfException(KernelExceptionMessageConstant.STDCF_NOT_FOUND_ENCRYPTION);
                    }
                    if (PdfName.V2.Equals(dic.Get(PdfName.CFM))) {
                        cryptoMode = EncryptionConstants.STANDARD_ENCRYPTION_128;
                    }
                    else {
                        if (PdfName.AESV2.Equals(dic.Get(PdfName.CFM))) {
                            cryptoMode = EncryptionConstants.ENCRYPTION_AES_128;
                        }
                        else {
                            throw new PdfException(KernelExceptionMessageConstant.NO_COMPATIBLE_ENCRYPTION_FOUND);
                        }
                    }
                    PdfBoolean em = encDict.GetAsBoolean(PdfName.EncryptMetadata);
                    if (em != null && !em.GetValue()) {
                        cryptoMode |= EncryptionConstants.DO_NOT_ENCRYPT_METADATA;
                    }
                    if (embeddedFilesOnlyMode) {
                        cryptoMode |= EncryptionConstants.EMBEDDED_FILES_ONLY;
                    }
                    break;
                }

                case 5:
                case 6: {
                    cryptoMode = EncryptionConstants.ENCRYPTION_AES_256;
                    PdfBoolean em5 = encDict.GetAsBoolean(PdfName.EncryptMetadata);
                    if (em5 != null && !em5.GetValue()) {
                        cryptoMode |= EncryptionConstants.DO_NOT_ENCRYPT_METADATA;
                    }
                    if (embeddedFilesOnlyMode) {
                        cryptoMode |= EncryptionConstants.EMBEDDED_FILES_ONLY;
                    }
                    break;
                }

                default: {
                    throw new PdfException(KernelExceptionMessageConstant.UNKNOWN_ENCRYPTION_TYPE_R).SetMessageParams(rValue);
                }
            }
            revision = SetCryptoMode(cryptoMode, length);
            return revision;
        }

        private int ReadAndSetCryptoModeForPubSecHandler(PdfDictionary encDict) {
            int cryptoMode;
            int length = 0;
            PdfNumber vValue = encDict.GetAsNumber(PdfName.V);
            if (vValue == null) {
                throw new PdfException(KernelExceptionMessageConstant.ILLEGAL_V_VALUE);
            }
            int v = vValue.IntValue();
            bool embeddedFilesOnlyMode = ReadEmbeddedFilesOnlyFromEncryptDictionary(encDict);
            switch (v) {
                case 1: {
                    cryptoMode = EncryptionConstants.STANDARD_ENCRYPTION_40;
                    length = 40;
                    break;
                }

                case 2: {
                    PdfNumber lengthValue = encDict.GetAsNumber(PdfName.Length);
                    length = lengthValue == null ? DEFAULT_KEY_LENGTH : lengthValue.IntValue();
                    if (length > 128 || length < 40 || length % 8 != 0) {
                        throw new PdfException(KernelExceptionMessageConstant.ILLEGAL_LENGTH_VALUE);
                    }
                    cryptoMode = EncryptionConstants.STANDARD_ENCRYPTION_128;
                    break;
                }

                case 4:
                case 5: {
                    PdfDictionary dic = encDict.GetAsDictionary(PdfName.CF);
                    if (dic == null) {
                        throw new PdfException(KernelExceptionMessageConstant.CF_NOT_FOUND_ENCRYPTION);
                    }
                    dic = (PdfDictionary)dic.Get(PdfName.DefaultCryptFilter);
                    if (dic == null) {
                        throw new PdfException(KernelExceptionMessageConstant.DEFAULT_CRYPT_FILTER_NOT_FOUND_ENCRYPTION);
                    }
                    if (PdfName.V2.Equals(dic.Get(PdfName.CFM))) {
                        cryptoMode = EncryptionConstants.STANDARD_ENCRYPTION_128;
                        length = 128;
                    }
                    else {
                        if (PdfName.AESV2.Equals(dic.Get(PdfName.CFM))) {
                            cryptoMode = EncryptionConstants.ENCRYPTION_AES_128;
                            length = 128;
                        }
                        else {
                            if (PdfName.AESV3.Equals(dic.Get(PdfName.CFM))) {
                                cryptoMode = EncryptionConstants.ENCRYPTION_AES_256;
                                length = 256;
                            }
                            else {
                                throw new PdfException(KernelExceptionMessageConstant.NO_COMPATIBLE_ENCRYPTION_FOUND);
                            }
                        }
                    }
                    PdfBoolean em = dic.GetAsBoolean(PdfName.EncryptMetadata);
                    if (em != null && !em.GetValue()) {
                        cryptoMode |= EncryptionConstants.DO_NOT_ENCRYPT_METADATA;
                    }
                    if (embeddedFilesOnlyMode) {
                        cryptoMode |= EncryptionConstants.EMBEDDED_FILES_ONLY;
                    }
                    break;
                }

                default: {
                    throw new PdfException(KernelExceptionMessageConstant.UNKNOWN_ENCRYPTION_TYPE_V, vValue);
                }
            }
            return SetCryptoMode(cryptoMode, length);
        }

//\cond DO_NOT_DOCUMENT
        internal static bool ReadEmbeddedFilesOnlyFromEncryptDictionary(PdfDictionary encDict) {
            PdfName embeddedFilesFilter = encDict.GetAsName(PdfName.EFF);
            bool encryptEmbeddedFiles = !PdfName.Identity.Equals(embeddedFilesFilter) && embeddedFilesFilter != null;
            bool encryptStreams = !PdfName.Identity.Equals(encDict.GetAsName(PdfName.StmF));
            bool encryptStrings = !PdfName.Identity.Equals(encDict.GetAsName(PdfName.StrF));
            if (encryptStreams || encryptStrings || !encryptEmbeddedFiles) {
                return false;
            }
            PdfDictionary cfDictionary = encDict.GetAsDictionary(PdfName.CF);
            if (cfDictionary != null) {
                // Here we check if the crypt filter for embedded files and the filter in the CF dictionary are the same
                return cfDictionary.GetAsDictionary(embeddedFilesFilter) != null;
            }
            return false;
        }
//\endcond

        private static int ConfigureAccessibilityPermissionsForMac(int permissions) {
            // 13 bit but pdf writers start counting from 1
            return permissions & ~(1 << 12);
        }

        private static int FixAccessibilityPermissionPdf20(int permissions) {
            // This bit was previously used to determine whether
            // content could be extracted for the purposes of accessibility,
            // however, that restriction has been deprecated in PDF 2.0. PDF
            // readers shall ignore this bit and PDF writers shall always set this
            // bit to 1 to ensure compatibility with PDF readers following
            // earlier specifications.
            return permissions | EncryptionConstants.ALLOW_SCREENREADERS;
        }

//\cond DO_NOT_DOCUMENT
        internal virtual void CheckEncryptionRequirements(PdfDocument document) {
            if (mac != null) {
                if (document.GetPdfVersion() == null || document.GetPdfVersion().CompareTo(PdfVersion.PDF_2_0) < 0) {
                    throw new PdfException(MAC_FOR_PDF_2);
                }
                if (this.GetPdfObject().GetAsNumber(PdfName.V).IntValue() < 5) {
                    throw new PdfException(MAC_FOR_ENCRYPTION_5);
                }
            }
            if (GetCryptoMode() < EncryptionConstants.ENCRYPTION_AES_256) {
                VersionConforming.ValidatePdfVersionForDeprecatedFeatureLogWarn(document, PdfVersion.PDF_2_0, VersionConforming
                    .DEPRECATED_ENCRYPTION_ALGORITHMS);
            }
            else {
                if (GetCryptoMode() == EncryptionConstants.ENCRYPTION_AES_256) {
                    PdfNumber r = GetPdfObject().GetAsNumber(PdfName.R);
                    if (r != null && r.IntValue() == 5) {
                        VersionConforming.ValidatePdfVersionForDeprecatedFeatureLogWarn(document, PdfVersion.PDF_2_0, VersionConforming
                            .DEPRECATED_AES256_REVISION);
                    }
                }
            }
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal virtual void ConfigureEncryptionParameters(PdfDocument document) {
            if (mac != null) {
                document.GetCatalog().AddDeveloperExtension(PdfDeveloperExtension.ISO_32004);
                mac.SetFileEncryptionKey(securityHandler.GetMkey().Length == 0 ? securityHandler.GetNextObjectKey() : securityHandler
                    .GetMkey());
                GetPdfObject().Put(PdfName.KDFSalt, new PdfString(mac.GetKdfSalt()).SetHexWriting(true));
            }
        }
//\endcond
    }
}
