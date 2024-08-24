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
using System.Security.Cryptography;
using iText.Bouncycastleconnector;
using iText.Commons.Bouncycastle;
using iText.Commons.Bouncycastle.Asn1;
using iText.Commons.Bouncycastle.Security;
using iText.Commons.Digest;
using iText.Commons.Utils;
using iText.IO.Source;
using iText.Kernel.Events;
using iText.Kernel.Exceptions;
using iText.Kernel.Pdf;

namespace iText.Kernel.Mac {
    /// <summary>Class responsible for integrity protection in encrypted documents, which uses MAC container.</summary>
    public class MacIntegrityProtector {
        private static readonly IBouncyCastleFactory BC_FACTORY = BouncyCastleFactoryCreator.GetFactory();

        private const String ID_AUTHENTICATED_DATA = "1.2.840.113549.1.9.16.1.2";

        private const String ID_KDF_PDFMACWRAPKDF = "1.0.32004.1.1";

        private const String ID_CT_PDFMACINTEGRITYINFO = "1.0.32004.1.0";

        private const String ID_CONTENT_TYPE = "1.2.840.113549.1.9.3";

        private const String ID_CMS_ALGORITHM_PROTECTION = "1.2.840.113549.1.9.52";

        private const String ID_MESSAGE_DIGEST = "1.2.840.113549.1.9.4";

        private const String DIGEST_NOT_SUPPORTED = "Digest algorithm is not supported.";

        private const String MAC_ALGORITHM_NOT_SUPPORTED = "This MAC algorithm is not supported.";

        private const String WRAP_ALGORITHM_NOT_SUPPORTED = "This wrapping algorithm is not supported.";

        private const String CONTAINER_GENERATION_EXCEPTION = "Exception occurred during MAC container generation.";

        private const String CONTAINER_EMBEDDING_EXCEPTION = "IOException occurred while trying to embed MAC container into document output stream.";

        private readonly MacPdfObject macPdfObject;

        private readonly PdfDocument document;

        private readonly MacProperties macProperties;

        private readonly byte[] kdfSalt = new byte[32];

        private byte[] fileEncryptionKey = new byte[0];

        /// <summary>
        /// Creates
        /// <see cref="MacIntegrityProtector"/>
        /// instance.
        /// </summary>
        /// <param name="document">
        /// 
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
        /// , for which integrity protection is required
        /// </param>
        /// <param name="macProperties">
        /// 
        /// <see cref="MacProperties"/>
        /// used to provide MAC algorithm properties
        /// </param>
        public MacIntegrityProtector(PdfDocument document, MacProperties macProperties) {
            this.document = document;
            this.macProperties = macProperties;
            this.macPdfObject = new MacPdfObject(GetContainerSizeEstimate());
            AddDocumentEvents();
            RNGCryptoServiceProvider sr = BC_FACTORY.GetSecureRandom();
            sr.GetBytes(kdfSalt);
        }

        /// <summary>Sets file encryption key to be used during MAC token calculation.</summary>
        /// <param name="fileEncryptionKey">
        /// 
        /// <c>byte[]</c>
        /// file encryption key bytes
        /// </param>
        public virtual void SetFileEncryptionKey(byte[] fileEncryptionKey) {
            this.fileEncryptionKey = fileEncryptionKey;
        }

        /// <summary>Gets KDF salt bytes, which are used during MAC token calculation.</summary>
        /// <returns>
        /// 
        /// <c>byte[]</c>
        /// KDF salt bytes.
        /// </returns>
        public virtual byte[] GetKdfSalt() {
            return JavaUtil.ArraysCopyOf(kdfSalt, kdfSalt.Length);
        }

        private int GetContainerSizeEstimate() {
            try {
                IMessageDigest digest = GetMessageDigest();
                digest.Update(new byte[0]);
                return CreateMacContainer(digest.Digest()).Length * 2 + 2;
            }
            catch (AbstractGeneralSecurityException e) {
                throw new PdfException(CONTAINER_GENERATION_EXCEPTION, e);
            }
            catch (System.IO.IOException e) {
                throw new PdfException(CONTAINER_GENERATION_EXCEPTION, e);
            }
        }

        private void AddDocumentEvents() {
            document.AddEventHandler(PdfDocumentEvent.START_DOCUMENT_CLOSING, new MacIntegrityProtector.MacPdfObjectAddingEvent
                (document, macPdfObject));
            document.AddEventHandler(PdfDocumentEvent.END_WRITER_FLUSH, new MacIntegrityProtector.MacContainerEmbedder
                (this));
        }

        private void EmbedMacContainer() {
            byte[] documentBytes = GetDocumentByteArrayOutputStream().ToArray();
            long[] byteRange = macPdfObject.ComputeByteRange(documentBytes.Length);
            long byteRangePosition = macPdfObject.GetByteRangePosition();
            MemoryStream localBaos = new MemoryStream();
            PdfOutputStream os = new PdfOutputStream(localBaos);
            os.Write('[');
            foreach (long l in byteRange) {
                os.WriteLong(l).Write(' ');
            }
            os.Write(']');
            Array.Copy(localBaos.ToArray(), 0, documentBytes, (int)byteRangePosition, localBaos.Length);
            IRandomAccessSource ras = new RandomAccessSourceFactory().CreateSource(documentBytes);
            // Here we should create MAC token
            byte[] macToken;
            try {
                byte[] dataDigest = DigestBytes(ras, byteRange);
                macToken = CreateMacContainer(dataDigest);
            }
            catch (AbstractGeneralSecurityException e) {
                throw new PdfException(CONTAINER_GENERATION_EXCEPTION, e);
            }
            PdfString macString = new PdfString(macToken).SetHexWriting(true);
            // fill in the MAC
            localBaos.JReset();
            os.Write(macString);
            Array.Copy(localBaos.ToArray(), 0, documentBytes, (int)byteRange[1], localBaos.Length);
            GetDocumentByteArrayOutputStream().JReset();
            document.GetWriter().GetOutputStream().Write(documentBytes, 0, documentBytes.Length);
        }

        private MemoryStream GetDocumentByteArrayOutputStream() {
            return ((MemoryStream)document.GetWriter().GetOutputStream());
        }

        private byte[] DigestBytes(IRandomAccessSource ras, long[] byteRange) {
            IMessageDigest digest = GetMessageDigest();
            using (Stream rg = new RASInputStream(new RandomAccessSourceFactory().CreateRanged(ras, byteRange))) {
                byte[] buf = new byte[8192];
                int rd;
                while ((rd = rg.JRead(buf, 0, buf.Length)) > 0) {
                    digest.Update(buf, 0, rd);
                }
                return digest.Digest();
            }
        }

        private IMessageDigest GetMessageDigest() {
            switch (macProperties.GetMacDigestAlgorithm()) {
                case MacProperties.MacDigestAlgorithm.SHA_256: {
                    return iText.Bouncycastleconnector.BouncyCastleFactoryCreator.GetFactory().CreateIDigest("SHA256");
                }

                case MacProperties.MacDigestAlgorithm.SHA_384: {
                    return iText.Bouncycastleconnector.BouncyCastleFactoryCreator.GetFactory().CreateIDigest("SHA384");
                }

                case MacProperties.MacDigestAlgorithm.SHA_512: {
                    return iText.Bouncycastleconnector.BouncyCastleFactoryCreator.GetFactory().CreateIDigest("SHA512");
                }

                case MacProperties.MacDigestAlgorithm.SHA3_256: {
                    return iText.Bouncycastleconnector.BouncyCastleFactoryCreator.GetFactory().CreateIDigest("SHA3-256");
                }

                case MacProperties.MacDigestAlgorithm.SHA3_384: {
                    return iText.Bouncycastleconnector.BouncyCastleFactoryCreator.GetFactory().CreateIDigest("SHA3-384");
                }

                case MacProperties.MacDigestAlgorithm.SHA3_512: {
                    return iText.Bouncycastleconnector.BouncyCastleFactoryCreator.GetFactory().CreateIDigest("SHA3-512");
                }

                default: {
                    throw new PdfException("This digest algorithm is not supported by MAC.");
                }
            }
        }

        private String GetMessageDigestOid() {
            switch (macProperties.GetMacDigestAlgorithm()) {
                case MacProperties.MacDigestAlgorithm.SHA_256: {
                    return "2.16.840.1.101.3.4.2.1";
                }

                case MacProperties.MacDigestAlgorithm.SHA_384: {
                    return "2.16.840.1.101.3.4.2.2";
                }

                case MacProperties.MacDigestAlgorithm.SHA_512: {
                    return "2.16.840.1.101.3.4.2.3";
                }

                case MacProperties.MacDigestAlgorithm.SHA3_256: {
                    return "2.16.840.1.101.3.4.2.8";
                }

                case MacProperties.MacDigestAlgorithm.SHA3_384: {
                    return "2.16.840.1.101.3.4.2.9";
                }

                case MacProperties.MacDigestAlgorithm.SHA3_512: {
                    return "2.16.840.1.101.3.4.2.10";
                }

                default: {
                    throw new PdfException(DIGEST_NOT_SUPPORTED);
                }
            }
        }

        private byte[] GenerateMacToken(byte[] macKey, byte[] data) {
            switch (macProperties.GetMacAlgorithm()) {
                case MacProperties.MacAlgorithm.HMAC_WITH_SHA_256: {
                    return BC_FACTORY.GenerateHMACSHA256Token(macKey, data);
                }

                default: {
                    throw new PdfException(MAC_ALGORITHM_NOT_SUPPORTED);
                }
            }
        }

        private byte[] GenerateEncryptedKey(byte[] macKey, byte[] macKek) {
            switch (macProperties.GetKeyWrappingAlgorithm()) {
                case MacProperties.KeyWrappingAlgorithm.AES_256_NO_PADD: {
                    return BC_FACTORY.GenerateEncryptedKeyWithAES256NoPad(macKey, macKek);
                }

                default: {
                    throw new PdfException(WRAP_ALGORITHM_NOT_SUPPORTED);
                }
            }
        }

        private String GetMacAlgorithmOid() {
            switch (macProperties.GetMacAlgorithm()) {
                case MacProperties.MacAlgorithm.HMAC_WITH_SHA_256: {
                    return "1.2.840.113549.2.9";
                }

                default: {
                    throw new PdfException(MAC_ALGORITHM_NOT_SUPPORTED);
                }
            }
        }

        private String GetKeyWrappingAlgorithmOid() {
            switch (macProperties.GetKeyWrappingAlgorithm()) {
                case MacProperties.KeyWrappingAlgorithm.AES_256_NO_PADD: {
                    return "2.16.840.1.101.3.4.1.45";
                }

                default: {
                    throw new PdfException(WRAP_ALGORITHM_NOT_SUPPORTED);
                }
            }
        }

        private byte[] CreateMacContainer(byte[] dataDigest) {
            IAsn1EncodableVector contentInfoV = BC_FACTORY.CreateASN1EncodableVector();
            contentInfoV.Add(BC_FACTORY.CreateASN1ObjectIdentifier(ID_AUTHENTICATED_DATA));
            // Recipient info
            IAsn1EncodableVector recInfoV = BC_FACTORY.CreateASN1EncodableVector();
            recInfoV.Add(BC_FACTORY.CreateASN1Integer(0));
            // version
            recInfoV.Add(BC_FACTORY.CreateDERTaggedObject(0, BC_FACTORY.CreateASN1ObjectIdentifier(ID_KDF_PDFMACWRAPKDF
                )));
            recInfoV.Add(BC_FACTORY.CreateDERSequence(BC_FACTORY.CreateASN1ObjectIdentifier(GetKeyWrappingAlgorithmOid
                ())));
            ////////////////////// KEK
            RNGCryptoServiceProvider sr = BC_FACTORY.GetSecureRandom();
            byte[] macKey = new byte[32];
            sr.GetBytes(macKey);
            byte[] macKek = BC_FACTORY.GenerateHKDF(fileEncryptionKey, kdfSalt, "PDFMAC".GetBytes(System.Text.Encoding
                .UTF8));
            byte[] encryptedKey = GenerateEncryptedKey(macKey, macKek);
            //////////////////////////////
            recInfoV.Add(BC_FACTORY.CreateDEROctetString(encryptedKey));
            // Digest info
            IAsn1EncodableVector digestInfoV = BC_FACTORY.CreateASN1EncodableVector();
            digestInfoV.Add(BC_FACTORY.CreateASN1Integer(0));
            // version
            digestInfoV.Add(BC_FACTORY.CreateDEROctetString(dataDigest));
            byte[] messageBytes = BC_FACTORY.CreateDERSequence(digestInfoV).GetEncoded();
            // Encapsulated content info
            IAsn1EncodableVector encapContentInfoV = BC_FACTORY.CreateASN1EncodableVector();
            encapContentInfoV.Add(BC_FACTORY.CreateASN1ObjectIdentifier(ID_CT_PDFMACINTEGRITYINFO));
            encapContentInfoV.Add(BC_FACTORY.CreateDERTaggedObject(0, BC_FACTORY.CreateDEROctetString(messageBytes)));
            // Hash messageBytes to get messageDigest attribute
            IMessageDigest digest = GetMessageDigest();
            digest.Update(messageBytes);
            byte[] messageDigest = digest.Digest();
            // Content type - mac integrity info
            IAsn1EncodableVector contentTypeInfoV = BC_FACTORY.CreateASN1EncodableVector();
            contentTypeInfoV.Add(BC_FACTORY.CreateASN1ObjectIdentifier(ID_CONTENT_TYPE));
            contentTypeInfoV.Add(BC_FACTORY.CreateDERSet(BC_FACTORY.CreateASN1ObjectIdentifier(ID_CT_PDFMACINTEGRITYINFO
                )));
            IAsn1EncodableVector algorithmsInfoV = BC_FACTORY.CreateASN1EncodableVector();
            algorithmsInfoV.Add(BC_FACTORY.CreateDERSequence(BC_FACTORY.CreateASN1ObjectIdentifier(GetMessageDigestOid
                ())));
            algorithmsInfoV.Add(BC_FACTORY.CreateDERTaggedObject(2, BC_FACTORY.CreateASN1ObjectIdentifier(GetMacAlgorithmOid
                ())));
            // CMS algorithm protection
            IAsn1EncodableVector algoProtectionInfoV = BC_FACTORY.CreateASN1EncodableVector();
            algoProtectionInfoV.Add(BC_FACTORY.CreateASN1ObjectIdentifier(ID_CMS_ALGORITHM_PROTECTION));
            algoProtectionInfoV.Add(BC_FACTORY.CreateDERSet(BC_FACTORY.CreateDERSequence(algorithmsInfoV)));
            // Message digest
            IAsn1EncodableVector messageDigestV = BC_FACTORY.CreateASN1EncodableVector();
            messageDigestV.Add(BC_FACTORY.CreateASN1ObjectIdentifier(ID_MESSAGE_DIGEST));
            messageDigestV.Add(BC_FACTORY.CreateDERSet(BC_FACTORY.CreateDEROctetString(messageDigest)));
            IAsn1EncodableVector authAttrsV = BC_FACTORY.CreateASN1EncodableVector();
            authAttrsV.Add(BC_FACTORY.CreateDERSequence(contentTypeInfoV));
            authAttrsV.Add(BC_FACTORY.CreateDERSequence(algoProtectionInfoV));
            authAttrsV.Add(BC_FACTORY.CreateDERSequence(messageDigestV));
            // Create mac
            byte[] data = BC_FACTORY.CreateDERSet(authAttrsV).GetEncoded();
            byte[] mac = GenerateMacToken(macKey, data);
            // Auth data
            IAsn1EncodableVector authDataV = BC_FACTORY.CreateASN1EncodableVector();
            authDataV.Add(BC_FACTORY.CreateASN1Integer(0));
            // version
            authDataV.Add(BC_FACTORY.CreateDERSet(BC_FACTORY.CreateDERTaggedObject(false, 3, BC_FACTORY.CreateDERSequence
                (recInfoV))));
            authDataV.Add(BC_FACTORY.CreateDERSequence(BC_FACTORY.CreateASN1ObjectIdentifier(GetMacAlgorithmOid())));
            authDataV.Add(BC_FACTORY.CreateDERTaggedObject(1, BC_FACTORY.CreateASN1ObjectIdentifier(GetMessageDigestOid
                ())));
            authDataV.Add(BC_FACTORY.CreateDERSequence(encapContentInfoV));
            authDataV.Add(BC_FACTORY.CreateDERTaggedObject(false, 2, BC_FACTORY.CreateDERSet(authAttrsV)));
            authDataV.Add(BC_FACTORY.CreateDEROctetString(mac));
            contentInfoV.Add(BC_FACTORY.CreateDERTaggedObject(0, BC_FACTORY.CreateDERSequence(authDataV)));
            return BC_FACTORY.CreateDERSequence(contentInfoV).GetEncoded();
        }

        private class MacPdfObjectAddingEvent : iText.Kernel.Events.IEventHandler {
            private readonly PdfDocument document;

            private readonly MacPdfObject macPdfObject;

//\cond DO_NOT_DOCUMENT
            internal MacPdfObjectAddingEvent(PdfDocument document, MacPdfObject macPdfObject) {
                this.document = document;
                this.macPdfObject = macPdfObject;
            }
//\endcond

            public virtual void HandleEvent(Event @event) {
                document.GetTrailer().Put(PdfName.AuthCode, macPdfObject.GetPdfObject());
            }
        }

        private class MacContainerEmbedder : iText.Kernel.Events.IEventHandler {
            public virtual void HandleEvent(Event @event) {
                try {
                    this._enclosing.EmbedMacContainer();
                }
                catch (System.IO.IOException e) {
                    throw new PdfException(MacIntegrityProtector.CONTAINER_EMBEDDING_EXCEPTION, e);
                }
            }

            internal MacContainerEmbedder(MacIntegrityProtector _enclosing) {
                this._enclosing = _enclosing;
            }

            private readonly MacIntegrityProtector _enclosing;
        }
    }
}
