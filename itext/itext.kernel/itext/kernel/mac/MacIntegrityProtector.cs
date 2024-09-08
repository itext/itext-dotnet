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

        private const String ID_KDF_PDF_MAC_WRAP_KDF = "1.0.32004.1.1";

        private const String ID_CT_PDF_MAC_INTEGRITY_INFO = "1.0.32004.1.0";

        private const String ID_CONTENT_TYPE = "1.2.840.113549.1.9.3";

        private const String ID_CMS_ALGORITHM_PROTECTION = "1.2.840.113549.1.9.52";

        private const String ID_MESSAGE_DIGEST = "1.2.840.113549.1.9.4";

        private const String PDF_MAC = "PDFMAC";

        private MacPdfObject macPdfObject;

        private readonly PdfDocument document;

        private readonly MacProperties macProperties;

        private byte[] kdfSalt = null;

        private byte[] fileEncryptionKey = new byte[0];

        /// <summary>
        /// Creates
        /// <see cref="MacIntegrityProtector"/>
        /// instance from the provided
        /// <see cref="MacProperties"/>.
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
        }

        /// <summary>
        /// Creates
        /// <see cref="MacIntegrityProtector"/>
        /// instance from the Auth dictionary.
        /// </summary>
        /// <param name="document">
        /// 
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
        /// , for which integrity protection is required
        /// </param>
        /// <param name="authDictionary">
        /// 
        /// <see cref="iText.Kernel.Pdf.PdfDictionary"/>
        /// , representing Auth dictionary, in which MAC container is stored
        /// </param>
        public MacIntegrityProtector(PdfDocument document, PdfDictionary authDictionary) {
            this.document = document;
            this.macProperties = ParseMacProperties(authDictionary.GetAsString(PdfName.MAC).GetValueBytes());
        }

        /// <summary>Sets file encryption key to be used during MAC calculation.</summary>
        /// <param name="fileEncryptionKey">
        /// 
        /// <c>byte[]</c>
        /// file encryption key bytes
        /// </param>
        public virtual void SetFileEncryptionKey(byte[] fileEncryptionKey) {
            this.fileEncryptionKey = fileEncryptionKey;
        }

        /// <summary>Gets KDF salt bytes, which are used during MAC key encryption.</summary>
        /// <returns>
        /// 
        /// <c>byte[]</c>
        /// KDF salt bytes.
        /// </returns>
        public virtual byte[] GetKdfSalt() {
            if (kdfSalt == null) {
                kdfSalt = GenerateRandomBytes(32);
            }
            return JavaUtil.ArraysCopyOf(kdfSalt, kdfSalt.Length);
        }

        /// <summary>Sets KDF salt bytes, to be used during MAC key encryption.</summary>
        /// <param name="kdfSalt">
        /// 
        /// <c>byte[]</c>
        /// KDF salt bytes.
        /// </param>
        public virtual void SetKdfSalt(byte[] kdfSalt) {
            this.kdfSalt = JavaUtil.ArraysCopyOf(kdfSalt, kdfSalt.Length);
        }

        /// <summary>Validates MAC container integrity.</summary>
        /// <remarks>
        /// Validates MAC container integrity. This method throws
        /// <see cref="iText.Kernel.Exceptions.PdfException"/>
        /// in case of any modifications,
        /// introduced to the document in question, after MAC container is integrated.
        /// </remarks>
        /// <param name="authDictionary">
        /// 
        /// <see cref="iText.Kernel.Pdf.PdfDictionary"/>
        /// which represents AuthCode entry in the trailer and container MAC.
        /// </param>
        public virtual void ValidateMacToken(PdfDictionary authDictionary) {
            byte[] expectedMac;
            byte[] actualMac;
            byte[] expectedMessageDigest;
            byte[] actualMessageDigest;
            try {
                byte[] macContainer = authDictionary.GetAsString(PdfName.MAC).GetValueBytes();
                byte[] macKey = ParseMacKey(macContainer);
                PdfArray byteRange = authDictionary.GetAsArray(PdfName.ByteRange);
                byte[] dataDigest = DigestBytes(document.GetReader().GetSafeFile().CreateSourceView(), byteRange.ToLongArray
                    ());
                byte[] expectedData = ParseAuthAttributes(macContainer).GetEncoded();
                expectedMac = GenerateMac(macKey, expectedData);
                expectedMessageDigest = CreateMessageDigestSequence(CreateMessageBytes(dataDigest)).GetEncoded();
                actualMessageDigest = ParseMessageDigest(macContainer).GetEncoded();
                actualMac = ParseMac(macContainer);
            }
            catch (Exception e) {
                throw new PdfException(KernelExceptionMessageConstant.VALIDATION_EXCEPTION, e);
            }
            if (!JavaUtil.ArraysEquals(expectedMac, actualMac) || !JavaUtil.ArraysEquals(expectedMessageDigest, actualMessageDigest
                )) {
                throw new PdfException(KernelExceptionMessageConstant.MAC_VALIDATION_FAILED);
            }
        }

        /// <summary>Prepare the document for MAC protection.</summary>
        public virtual void PrepareDocument() {
            document.AddEventHandler(PdfDocumentEvent.START_DOCUMENT_CLOSING, new MacIntegrityProtector.MacPdfObjectAdder
                (this));
            document.AddEventHandler(PdfDocumentEvent.END_WRITER_FLUSH, new MacIntegrityProtector.MacContainerEmbedder
                (this));
        }

        private int GetContainerSizeEstimate() {
            try {
                IMessageDigest digest = GetMessageDigest();
                digest.Update(new byte[0]);
                return CreateMacContainer(digest.Digest(), GenerateRandomBytes(32)).Length * 2 + 2;
            }
            catch (AbstractGeneralSecurityException e) {
                throw new PdfException(KernelExceptionMessageConstant.CONTAINER_GENERATION_EXCEPTION, e);
            }
            catch (System.IO.IOException e) {
                throw new PdfException(KernelExceptionMessageConstant.CONTAINER_GENERATION_EXCEPTION, e);
            }
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
            // Here we should create MAC
            byte[] mac;
            try {
                byte[] dataDigest = DigestBytes(ras, byteRange);
                mac = CreateMacContainer(dataDigest, GenerateRandomBytes(32));
            }
            catch (AbstractGeneralSecurityException e) {
                throw new PdfException(KernelExceptionMessageConstant.CONTAINER_GENERATION_EXCEPTION, e);
            }
            PdfString macString = new PdfString(mac).SetHexWriting(true);
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

        private String GetMacDigestOid() {
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
                    throw new PdfException(KernelExceptionMessageConstant.DIGEST_NOT_SUPPORTED);
                }
            }
        }

        private byte[] GenerateMac(byte[] macKey, byte[] data) {
            switch (macProperties.GetMacAlgorithm()) {
                case MacProperties.MacAlgorithm.HMAC_WITH_SHA_256: {
                    return BC_FACTORY.GenerateHMACSHA256Token(macKey, data);
                }

                default: {
                    throw new PdfException(KernelExceptionMessageConstant.MAC_ALGORITHM_NOT_SUPPORTED);
                }
            }
        }

        private byte[] GenerateEncryptedKey(byte[] macKey, byte[] macKek) {
            switch (macProperties.GetKeyWrappingAlgorithm()) {
                case MacProperties.KeyWrappingAlgorithm.AES_256_NO_PADD: {
                    return BC_FACTORY.GenerateEncryptedKeyWithAES256NoPad(macKey, macKek);
                }

                default: {
                    throw new PdfException(KernelExceptionMessageConstant.WRAP_ALGORITHM_NOT_SUPPORTED);
                }
            }
        }

        private byte[] GenerateDecryptedKey(byte[] encryptedMacKey, byte[] macKek) {
            switch (macProperties.GetKeyWrappingAlgorithm()) {
                case MacProperties.KeyWrappingAlgorithm.AES_256_NO_PADD: {
                    return BC_FACTORY.GenerateDecryptedKeyWithAES256NoPad(encryptedMacKey, macKek);
                }

                default: {
                    throw new PdfException(KernelExceptionMessageConstant.WRAP_ALGORITHM_NOT_SUPPORTED);
                }
            }
        }

        private String GetMacAlgorithmOid() {
            switch (macProperties.GetMacAlgorithm()) {
                case MacProperties.MacAlgorithm.HMAC_WITH_SHA_256: {
                    return "1.2.840.113549.2.9";
                }

                default: {
                    throw new PdfException(KernelExceptionMessageConstant.MAC_ALGORITHM_NOT_SUPPORTED);
                }
            }
        }

        private String GetKeyWrappingAlgorithmOid() {
            switch (macProperties.GetKeyWrappingAlgorithm()) {
                case MacProperties.KeyWrappingAlgorithm.AES_256_NO_PADD: {
                    return "2.16.840.1.101.3.4.1.45";
                }

                default: {
                    throw new PdfException(KernelExceptionMessageConstant.WRAP_ALGORITHM_NOT_SUPPORTED);
                }
            }
        }

        private byte[] ParseMacKey(byte[] macContainer) {
            IAsn1Sequence authDataSequence = GetAuthDataSequence(macContainer);
            IAsn1Sequence recInfo = BC_FACTORY.CreateASN1Sequence(BC_FACTORY.CreateASN1TaggedObject(BC_FACTORY.CreateASN1Set
                (authDataSequence.GetObjectAt(1)).GetObjectAt(0)).GetObject());
            IAsn1OctetString encryptedKey = BC_FACTORY.CreateASN1OctetString(recInfo.GetObjectAt(3));
            byte[] macKek = BC_FACTORY.GenerateHKDF(fileEncryptionKey, kdfSalt, PDF_MAC.GetBytes(System.Text.Encoding.
                UTF8));
            return GenerateDecryptedKey(encryptedKey.GetOctets(), macKek);
        }

        private IAsn1Sequence ParseMessageDigest(byte[] macContainer) {
            IAsn1Set authAttributes = ParseAuthAttributes(macContainer);
            return BC_FACTORY.CreateASN1Sequence(authAttributes.GetObjectAt(2));
        }

        private byte[] CreateMacContainer(byte[] dataDigest, byte[] macKey) {
            IAsn1EncodableVector contentInfoV = BC_FACTORY.CreateASN1EncodableVector();
            contentInfoV.Add(BC_FACTORY.CreateASN1ObjectIdentifier(ID_AUTHENTICATED_DATA));
            // Recipient info
            IAsn1EncodableVector recInfoV = BC_FACTORY.CreateASN1EncodableVector();
            recInfoV.Add(BC_FACTORY.CreateASN1Integer(0));
            // version
            recInfoV.Add(BC_FACTORY.CreateDERTaggedObject(0, BC_FACTORY.CreateASN1ObjectIdentifier(ID_KDF_PDF_MAC_WRAP_KDF
                )));
            recInfoV.Add(BC_FACTORY.CreateDERSequence(BC_FACTORY.CreateASN1ObjectIdentifier(GetKeyWrappingAlgorithmOid
                ())));
            ////////////////////// KEK
            byte[] macKek = BC_FACTORY.GenerateHKDF(fileEncryptionKey, kdfSalt, PDF_MAC.GetBytes(System.Text.Encoding.
                UTF8));
            byte[] encryptedKey = GenerateEncryptedKey(macKey, macKek);
            recInfoV.Add(BC_FACTORY.CreateDEROctetString(encryptedKey));
            // Digest info
            byte[] messageBytes = CreateMessageBytes(dataDigest);
            // Encapsulated content info
            IAsn1EncodableVector encapContentInfoV = BC_FACTORY.CreateASN1EncodableVector();
            encapContentInfoV.Add(BC_FACTORY.CreateASN1ObjectIdentifier(ID_CT_PDF_MAC_INTEGRITY_INFO));
            encapContentInfoV.Add(BC_FACTORY.CreateDERTaggedObject(0, BC_FACTORY.CreateDEROctetString(messageBytes)));
            IDerSet authAttrs = CreateAuthAttributes(messageBytes);
            // Create mac
            byte[] data = authAttrs.GetEncoded();
            byte[] mac = GenerateMac(macKey, data);
            // Auth data
            IAsn1EncodableVector authDataV = BC_FACTORY.CreateASN1EncodableVector();
            authDataV.Add(BC_FACTORY.CreateASN1Integer(0));
            // version
            authDataV.Add(BC_FACTORY.CreateDERSet(BC_FACTORY.CreateDERTaggedObject(false, 3, BC_FACTORY.CreateDERSequence
                (recInfoV))));
            authDataV.Add(BC_FACTORY.CreateDERSequence(BC_FACTORY.CreateASN1ObjectIdentifier(GetMacAlgorithmOid())));
            authDataV.Add(BC_FACTORY.CreateDERTaggedObject(false, 1, BC_FACTORY.CreateDERSequence(BC_FACTORY.CreateASN1ObjectIdentifier
                (GetMacDigestOid()))));
            authDataV.Add(BC_FACTORY.CreateDERSequence(encapContentInfoV));
            authDataV.Add(BC_FACTORY.CreateDERTaggedObject(false, 2, authAttrs));
            authDataV.Add(BC_FACTORY.CreateDEROctetString(mac));
            contentInfoV.Add(BC_FACTORY.CreateDERTaggedObject(0, BC_FACTORY.CreateDERSequence(authDataV)));
            return BC_FACTORY.CreateDERSequence(contentInfoV).GetEncoded();
        }

        private IDerSequence CreateMessageDigestSequence(byte[] messageBytes) {
            // Hash messageBytes to get messageDigest attribute
            IMessageDigest digest = GetMessageDigest();
            digest.Update(messageBytes);
            byte[] messageDigest = digest.Digest();
            // Message digest
            IAsn1EncodableVector messageDigestV = BC_FACTORY.CreateASN1EncodableVector();
            messageDigestV.Add(BC_FACTORY.CreateASN1ObjectIdentifier(ID_MESSAGE_DIGEST));
            messageDigestV.Add(BC_FACTORY.CreateDERSet(BC_FACTORY.CreateDEROctetString(messageDigest)));
            return BC_FACTORY.CreateDERSequence(messageDigestV);
        }

        private IDerSet CreateAuthAttributes(byte[] messageBytes) {
            // Content type - mac integrity info
            IAsn1EncodableVector contentTypeInfoV = BC_FACTORY.CreateASN1EncodableVector();
            contentTypeInfoV.Add(BC_FACTORY.CreateASN1ObjectIdentifier(ID_CONTENT_TYPE));
            contentTypeInfoV.Add(BC_FACTORY.CreateDERSet(BC_FACTORY.CreateASN1ObjectIdentifier(ID_CT_PDF_MAC_INTEGRITY_INFO
                )));
            IAsn1EncodableVector algorithmsInfoV = BC_FACTORY.CreateASN1EncodableVector();
            algorithmsInfoV.Add(BC_FACTORY.CreateDERSequence(BC_FACTORY.CreateASN1ObjectIdentifier(GetMacDigestOid()))
                );
            algorithmsInfoV.Add(BC_FACTORY.CreateDERTaggedObject(2, BC_FACTORY.CreateASN1ObjectIdentifier(GetMacAlgorithmOid
                ())));
            // CMS algorithm protection
            IAsn1EncodableVector algoProtectionInfoV = BC_FACTORY.CreateASN1EncodableVector();
            algoProtectionInfoV.Add(BC_FACTORY.CreateASN1ObjectIdentifier(ID_CMS_ALGORITHM_PROTECTION));
            algoProtectionInfoV.Add(BC_FACTORY.CreateDERSet(BC_FACTORY.CreateDERSequence(algorithmsInfoV)));
            IAsn1EncodableVector authAttrsV = BC_FACTORY.CreateASN1EncodableVector();
            authAttrsV.Add(BC_FACTORY.CreateDERSequence(contentTypeInfoV));
            authAttrsV.Add(BC_FACTORY.CreateDERSequence(algoProtectionInfoV));
            authAttrsV.Add(CreateMessageDigestSequence(messageBytes));
            return BC_FACTORY.CreateDERSet(authAttrsV);
        }

        private static byte[] ParseMac(byte[] macContainer) {
            IAsn1Sequence authDataSequence = GetAuthDataSequence(macContainer);
            return BC_FACTORY.CreateASN1OctetString(authDataSequence.GetObjectAt(6)).GetOctets();
        }

        private static IAsn1Set ParseAuthAttributes(byte[] macContainer) {
            IAsn1Sequence authDataSequence = GetAuthDataSequence(macContainer);
            return BC_FACTORY.CreateASN1Set(BC_FACTORY.CreateASN1TaggedObject(authDataSequence.GetObjectAt(5)), false);
        }

        private static byte[] CreateMessageBytes(byte[] dataDigest) {
            IAsn1EncodableVector digestInfoV = BC_FACTORY.CreateASN1EncodableVector();
            digestInfoV.Add(BC_FACTORY.CreateASN1Integer(0));
            digestInfoV.Add(BC_FACTORY.CreateDEROctetString(dataDigest));
            return BC_FACTORY.CreateDERSequence(digestInfoV).GetEncoded();
        }

        private static byte[] GenerateRandomBytes(int length) {
            byte[] randomBytes = new byte[length];
            BC_FACTORY.GetSecureRandom().GetBytes(randomBytes);
            return randomBytes;
        }

        private static MacProperties ParseMacProperties(byte[] macContainer) {
            IAsn1Sequence authDataSequence = GetAuthDataSequence(macContainer);
            IAsn1Object digestAlgorithmContainer = BC_FACTORY.CreateASN1TaggedObject(authDataSequence.GetObjectAt(3)).
                GetObject();
            IDerObjectIdentifier digestAlgorithm;
            if (BC_FACTORY.CreateASN1ObjectIdentifier(digestAlgorithmContainer) != null) {
                digestAlgorithm = BC_FACTORY.CreateASN1ObjectIdentifier(digestAlgorithmContainer);
            }
            else {
                digestAlgorithm = BC_FACTORY.CreateASN1ObjectIdentifier(BC_FACTORY.CreateASN1Sequence(digestAlgorithmContainer
                    ).GetObjectAt(0));
            }
            return new MacProperties(GetMacDigestAlgorithm(digestAlgorithm.GetId()));
        }

        private static MacProperties.MacDigestAlgorithm GetMacDigestAlgorithm(String oid) {
            switch (oid) {
                case "2.16.840.1.101.3.4.2.1": {
                    return MacProperties.MacDigestAlgorithm.SHA_256;
                }

                case "2.16.840.1.101.3.4.2.2": {
                    return MacProperties.MacDigestAlgorithm.SHA_384;
                }

                case "2.16.840.1.101.3.4.2.3": {
                    return MacProperties.MacDigestAlgorithm.SHA_512;
                }

                case "2.16.840.1.101.3.4.2.8": {
                    return MacProperties.MacDigestAlgorithm.SHA3_256;
                }

                case "2.16.840.1.101.3.4.2.9": {
                    return MacProperties.MacDigestAlgorithm.SHA3_384;
                }

                case "2.16.840.1.101.3.4.2.10": {
                    return MacProperties.MacDigestAlgorithm.SHA3_512;
                }

                default: {
                    throw new PdfException(KernelExceptionMessageConstant.DIGEST_NOT_SUPPORTED);
                }
            }
        }

        private static IAsn1Sequence GetAuthDataSequence(byte[] macContainer) {
            IAsn1Sequence contentInfoSequence;
            try {
                using (IAsn1InputStream din = BC_FACTORY.CreateASN1InputStream(new MemoryStream(macContainer))) {
                    contentInfoSequence = BC_FACTORY.CreateASN1Sequence(din.ReadObject());
                }
            }
            catch (System.IO.IOException e) {
                throw new PdfException(KernelExceptionMessageConstant.CONTAINER_PARSING_EXCEPTION, e);
            }
            return BC_FACTORY.CreateASN1Sequence(BC_FACTORY.CreateASN1TaggedObject(contentInfoSequence.GetObjectAt(1))
                .GetObject());
        }

        private class MacPdfObjectAdder : iText.Kernel.Events.IEventHandler {
            public virtual void HandleEvent(Event @event) {
                this._enclosing.macPdfObject = new MacPdfObject(this._enclosing.GetContainerSizeEstimate());
                this._enclosing.document.GetTrailer().Put(PdfName.AuthCode, this._enclosing.macPdfObject.GetPdfObject());
            }

            internal MacPdfObjectAdder(MacIntegrityProtector _enclosing) {
                this._enclosing = _enclosing;
            }

            private readonly MacIntegrityProtector _enclosing;
        }

        private class MacContainerEmbedder : iText.Kernel.Events.IEventHandler {
            public virtual void HandleEvent(Event @event) {
                try {
                    this._enclosing.EmbedMacContainer();
                }
                catch (System.IO.IOException e) {
                    throw new PdfException(KernelExceptionMessageConstant.CONTAINER_EMBEDDING_EXCEPTION, e);
                }
            }

            internal MacContainerEmbedder(MacIntegrityProtector _enclosing) {
                this._enclosing = _enclosing;
            }

            private readonly MacIntegrityProtector _enclosing;
        }
    }
}
