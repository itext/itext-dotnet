/*

This file is part of the iText (R) project.
Copyright (c) 1998-2016 iText Group NV
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
using System;
using System.IO;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.X509;
using iText.IO.Source;
using iText.IO.Util;
using iText.Kernel;
using iText.Kernel.Crypto;
using iText.Kernel.Crypto.Securityhandler;

namespace iText.Kernel.Pdf {
    /// <author>Paulo Soares</author>
    /// <author>Kazuya Ujihara</author>
    public class PdfEncryption : PdfObjectWrapper<PdfDictionary> {
        private const int STANDARD_ENCRYPTION_40 = 2;

        private const int STANDARD_ENCRYPTION_128 = 3;

        private const int AES_128 = 4;

        private const int AES_256 = 5;

        private static long seq = SystemUtil.GetSystemTimeTicks();

        private int cryptoMode;

        private long? permissions;

        private bool encryptMetadata;

        private bool embeddedFilesOnly;

        private byte[] documentId;

        private SecurityHandler securityHandler;

        /// <summary>Creates the encryption.</summary>
        /// <remarks>
        /// Creates the encryption. The userPassword and the
        /// ownerPassword can be null or have zero length. In this case the ownerPassword
        /// is replaced by a random string. The open permissions for the document can be
        /// AllowPrinting, AllowModifyContents, AllowCopy, AllowModifyAnnotations,
        /// AllowFillIn, AllowScreenReaders, AllowAssembly and AllowDegradedPrinting.
        /// The permissions can be combined by ORing them.
        /// </remarks>
        /// <param name="userPassword">the user password. Can be null or empty</param>
        /// <param name="ownerPassword">the owner password. Can be null or empty</param>
        /// <param name="permissions">the user permissions</param>
        /// <param name="encryptionType">
        /// the type of encryption. It can be one of STANDARD_ENCRYPTION_40, STANDARD_ENCRYPTION_128 or ENCRYPTION_AES128.
        /// Optionally DO_NOT_ENCRYPT_METADATA can be ored to output the metadata in cleartext
        /// </param>
        /// <exception cref="iText.Kernel.PdfException">if the document is already open</exception>
        public PdfEncryption(byte[] userPassword, byte[] ownerPassword, int permissions, int encryptionType, byte[]
             documentId)
            : base(new PdfDictionary()) {
            this.documentId = documentId;
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
                        , ownerPassword, permissions, encryptMetadata, embeddedFilesOnly);
                    this.permissions = handlerAes256.GetPermissions();
                    securityHandler = handlerAes256;
                    break;
                }
            }
        }

        /// <summary>Creates the certificate encryption.</summary>
        /// <remarks>
        /// Creates the certificate encryption. An array of one or more public certificates
        /// must be provided together with an array of the same size for the permissions for each certificate.
        /// The open permissions for the document can be
        /// AllowPrinting, AllowModifyContents, AllowCopy, AllowModifyAnnotations,
        /// AllowFillIn, AllowScreenReaders, AllowAssembly and AllowDegradedPrinting.
        /// The permissions can be combined by ORing them.
        /// Optionally DO_NOT_ENCRYPT_METADATA can be ored to output the metadata in cleartext
        /// </remarks>
        /// <param name="certs">the public certificates to be used for the encryption</param>
        /// <param name="permissions">the user permissions for each of the certificates</param>
        /// <param name="encryptionType">the type of encryption. It can be one of STANDARD_ENCRYPTION_40, STANDARD_ENCRYPTION_128 or ENCRYPTION_AES128.
        ///     </param>
        /// <exception cref="iText.Kernel.PdfException">if the document is already open</exception>
        public PdfEncryption(X509Certificate[] certs, int[] permissions, int encryptionType)
            : base(new PdfDictionary()) {
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

        public PdfEncryption(PdfDictionary pdfDict, ICipherParameters certificateKey, X509Certificate certificate)
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
            IDigest md5;
            try {
                md5 = Org.BouncyCastle.Security.DigestUtilities.GetDigest("MD5");
            }
            catch (Exception e) {
                throw new PdfException(PdfException.PdfEncryption, e);
            }
            long time = SystemUtil.GetSystemTimeTicks();
            long mem = SystemUtil.GetFreeMemory();
            String s = time + "+" + mem + "+" + (seq++);
            return md5.Digest(s.GetBytes(iText.IO.Util.EncodingUtil.ISO_8859_1));
        }

        /// <summary>Creates a PdfLiteral that contains an array of two id entries.</summary>
        /// <remarks>
        /// Creates a PdfLiteral that contains an array of two id entries. These entries are both hexadecimal
        /// strings containing 16 hex characters. The first entry is the original id, the second entry
        /// should be different from the first one if the document has changed.
        /// </remarks>
        /// <param name="id">the first id</param>
        /// <param name="modified">whether the document has been changed or not</param>
        /// <returns>PdfObject containing the two entries.</returns>
        public static PdfObject CreateInfoId(byte[] id, bool modified) {
            if (modified) {
                return CreateInfoId(id, GenerateNewDocumentId());
            }
            else {
                return CreateInfoId(id, id);
            }
        }

        /// <summary>Creates a PdfLiteral that contains an array of two id entries.</summary>
        /// <remarks>
        /// Creates a PdfLiteral that contains an array of two id entries. These entries are both hexadecimal
        /// strings containing 16 hex characters. The first entry is the original id, the second entry
        /// should be different from the first one if the document has changed.
        /// </remarks>
        /// <param name="firstId">the first id</param>
        /// <param name="secondId">the second id</param>
        /// <returns>PdfObject containing the two entries.</returns>
        public static PdfObject CreateInfoId(byte[] firstId, byte[] secondId) {
            if (firstId.Length < 16) {
                firstId = GenerateNewDocumentId();
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

        /// <summary>Gets the encryption permissions.</summary>
        /// <remarks>
        /// Gets the encryption permissions. It can be used directly in
        /// <see cref="WriterProperties.SetStandardEncryption(byte[], byte[], int, int)"/>
        /// .
        /// See ISO 32000-1, Table 22 for more details.
        /// </remarks>
        /// <returns>the encryption permissions, an unsigned 32-bit quantity.</returns>
        public virtual long? GetPermissions() {
            return permissions;
        }

        /// <summary>Gets encryption algorithm and access permissions.</summary>
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
                throw new PdfException(PdfException.PdfEncryption, e);
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
                throw new PdfException(PdfException.PdfEncryption, e);
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
        /// Basically this means that before flushing you need to explicitly call
        /// <see cref="PdfObjectWrapper{T}.MakeIndirect(PdfDocument)"/>
        /// .
        /// For example: wrapperInstance.makeIndirect(document).flush();
        /// Note that not every wrapper require this, only those that have such warning in documentation.
        /// </summary>
        public override void Flush() {
            base.Flush();
        }

        protected internal override bool IsWrappedObjectMustBeIndirect() {
            return true;
        }

        private void SetKeyLength(int keyLength) {
            // 40 - is default value;
            if (keyLength != 40) {
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
                    embeddedFilesOnly = false;
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
                    throw new PdfException(PdfException.NoValidEncryptionMode);
                }
            }
            return revision;
        }

        private int ReadAndSetCryptoModeForStdHandler(PdfDictionary encDict) {
            int cryptoMode;
            int length = 0;
            PdfNumber rValue = encDict.GetAsNumber(PdfName.R);
            if (rValue == null) {
                throw new PdfException(PdfException.IllegalRValue);
            }
            int revision = rValue.IntValue();
            switch (revision) {
                case 2: {
                    cryptoMode = EncryptionConstants.STANDARD_ENCRYPTION_40;
                    break;
                }

                case 3: {
                    PdfNumber lengthValue = encDict.GetAsNumber(PdfName.Length);
                    if (lengthValue == null) {
                        throw new PdfException(PdfException.IllegalLengthValue);
                    }
                    length = lengthValue.IntValue();
                    if (length > 128 || length < 40 || length % 8 != 0) {
                        throw new PdfException(PdfException.IllegalLengthValue);
                    }
                    cryptoMode = EncryptionConstants.STANDARD_ENCRYPTION_128;
                    break;
                }

                case 4: {
                    PdfDictionary dic = (PdfDictionary)encDict.Get(PdfName.CF);
                    if (dic == null) {
                        throw new PdfException(PdfException.CfNotFoundEncryption);
                    }
                    dic = (PdfDictionary)dic.Get(PdfName.StdCF);
                    if (dic == null) {
                        throw new PdfException(PdfException.StdcfNotFoundEncryption);
                    }
                    if (PdfName.V2.Equals(dic.Get(PdfName.CFM))) {
                        cryptoMode = EncryptionConstants.STANDARD_ENCRYPTION_128;
                    }
                    else {
                        if (PdfName.AESV2.Equals(dic.Get(PdfName.CFM))) {
                            cryptoMode = EncryptionConstants.ENCRYPTION_AES_128;
                        }
                        else {
                            throw new PdfException(PdfException.NoCompatibleEncryptionFound);
                        }
                    }
                    PdfBoolean em = encDict.GetAsBoolean(PdfName.EncryptMetadata);
                    if (em != null && !em.GetValue()) {
                        cryptoMode |= EncryptionConstants.DO_NOT_ENCRYPT_METADATA;
                    }
                    break;
                }

                case 5: {
                    cryptoMode = EncryptionConstants.ENCRYPTION_AES_256;
                    PdfBoolean em5 = encDict.GetAsBoolean(PdfName.EncryptMetadata);
                    if (em5 != null && !em5.GetValue()) {
                        cryptoMode |= EncryptionConstants.DO_NOT_ENCRYPT_METADATA;
                    }
                    break;
                }

                default: {
                    throw new PdfException(PdfException.UnknownEncryptionTypeREq1).SetMessageParams(rValue);
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
                throw new PdfException(PdfException.IllegalVValue);
            }
            int v = vValue.IntValue();
            switch (v) {
                case 1: {
                    cryptoMode = EncryptionConstants.STANDARD_ENCRYPTION_40;
                    length = 40;
                    break;
                }

                case 2: {
                    PdfNumber lengthValue = encDict.GetAsNumber(PdfName.Length);
                    if (lengthValue == null) {
                        throw new PdfException(PdfException.IllegalLengthValue);
                    }
                    length = lengthValue.IntValue();
                    if (length > 128 || length < 40 || length % 8 != 0) {
                        throw new PdfException(PdfException.IllegalLengthValue);
                    }
                    cryptoMode = EncryptionConstants.STANDARD_ENCRYPTION_128;
                    break;
                }

                case 4:
                case 5: {
                    PdfDictionary dic = encDict.GetAsDictionary(PdfName.CF);
                    if (dic == null) {
                        throw new PdfException(PdfException.CfNotFoundEncryption);
                    }
                    dic = (PdfDictionary)dic.Get(PdfName.DefaultCryptFilter);
                    if (dic == null) {
                        throw new PdfException(PdfException.DefaultcryptfilterNotFoundEncryption);
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
                                throw new PdfException(PdfException.NoCompatibleEncryptionFound);
                            }
                        }
                    }
                    PdfBoolean em = dic.GetAsBoolean(PdfName.EncryptMetadata);
                    if (em != null && !em.GetValue()) {
                        cryptoMode |= EncryptionConstants.DO_NOT_ENCRYPT_METADATA;
                    }
                    break;
                }

                default: {
                    throw new PdfException(PdfException.UnknownEncryptionTypeVEq1, vValue);
                }
            }
            return SetCryptoMode(cryptoMode, length);
        }
    }
}
