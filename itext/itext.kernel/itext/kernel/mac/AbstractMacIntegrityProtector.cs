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
using iText.Commons.Digest;
using iText.Commons.Utils;
using iText.IO.Source;
using iText.Kernel.Crypto;
using iText.Kernel.Exceptions;
using iText.Kernel.Pdf;

namespace iText.Kernel.Mac {
    /// <summary>Class responsible for integrity protection in encrypted documents, which uses MAC container.</summary>
    public abstract class AbstractMacIntegrityProtector {
        private static readonly IBouncyCastleFactory BC_FACTORY = BouncyCastleFactoryCreator.GetFactory();

        private const String PDF_MAC = "PDFMAC";

        protected internal readonly PdfDocument document;

        protected internal readonly MacProperties macProperties;

        protected internal byte[] kdfSalt = null;

        protected internal byte[] fileEncryptionKey = new byte[0];

        private readonly MacContainerReader macContainerReader;

        /// <summary>
        /// Creates
        /// <see cref="AbstractMacIntegrityProtector"/>
        /// instance from the provided
        /// <see cref="MacProperties"/>.
        /// </summary>
        /// <param name="document">
        /// 
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
        /// for which integrity protection is required
        /// </param>
        /// <param name="macProperties">
        /// 
        /// <see cref="MacProperties"/>
        /// used to provide MAC algorithm properties
        /// </param>
        protected internal AbstractMacIntegrityProtector(PdfDocument document, MacProperties macProperties) {
            this.document = document;
            this.macContainerReader = null;
            this.macProperties = macProperties;
        }

        /// <summary>
        /// Creates
        /// <see cref="AbstractMacIntegrityProtector"/>
        /// instance from the Auth dictionary.
        /// </summary>
        /// <param name="document">
        /// 
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
        /// for which integrity protection is required
        /// </param>
        /// <param name="authDictionary">
        /// 
        /// <see cref="iText.Kernel.Pdf.PdfDictionary"/>
        /// representing Auth dictionary in which MAC container is stored
        /// </param>
        protected internal AbstractMacIntegrityProtector(PdfDocument document, PdfDictionary authDictionary) {
            this.document = document;
            this.macContainerReader = MacContainerReader.GetInstance(authDictionary);
            this.macProperties = new MacProperties(GetMacDigestAlgorithm(macContainerReader.ParseDigestAlgorithm()));
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
        public virtual void ValidateMacToken() {
            try {
                byte[] macKey = GenerateDecryptedKey(macContainerReader.ParseMacKey());
                long[] byteRange = macContainerReader.GetByteRange();
                byte[] dataDigest;
                IRandomAccessSource randomAccessSource = document.GetReader().GetSafeFile().CreateSourceView();
                using (Stream rg = new RASInputStream(new RandomAccessSourceFactory().CreateRanged(randomAccessSource, byteRange
                    ))) {
                    dataDigest = DigestBytes(rg);
                }
                byte[] expectedData = macContainerReader.ParseAuthAttributes().GetEncoded();
                byte[] expectedMac = GenerateMac(macKey, expectedData);
                byte[] signatureDigest = DigestBytes(macContainerReader.GetSignature());
                byte[] expectedMessageDigest = CreateMessageDigestSequence(CreatePdfMacIntegrityInfo(dataDigest, signatureDigest
                    )).GetEncoded();
                byte[] actualMessageDigest = macContainerReader.ParseMessageDigest().GetEncoded();
                byte[] actualMac = macContainerReader.ParseMac();
                if (!JavaUtil.ArraysEquals(expectedMac, actualMac) || !JavaUtil.ArraysEquals(expectedMessageDigest, actualMessageDigest
                    )) {
                    throw new PdfException(KernelExceptionMessageConstant.MAC_VALIDATION_FAILED);
                }
            }
            catch (PdfException e) {
                throw;
            }
            catch (Exception e) {
                throw new PdfException(KernelExceptionMessageConstant.MAC_VALIDATION_EXCEPTION, e);
            }
        }

        /// <summary>Digests provided bytes based on hash algorithm, specified for this class instance.</summary>
        /// <param name="bytes">
        /// 
        /// <c>byte[]</c>
        /// to be digested
        /// </param>
        /// <returns>digested bytes.</returns>
        protected internal virtual byte[] DigestBytes(byte[] bytes) {
            return bytes == null ? null : DigestBytes(new MemoryStream(bytes));
        }

        /// <summary>Digests provided input stream based on hash algorithm, specified for this class instance.</summary>
        /// <param name="inputStream">
        /// 
        /// <see cref="System.IO.Stream"/>
        /// to be digested
        /// </param>
        /// <returns>digested bytes.</returns>
        protected internal virtual byte[] DigestBytes(Stream inputStream) {
            if (inputStream == null) {
                return null;
            }
            String algorithm = MacProperties.MacDigestAlgorithmToString(macProperties.GetMacDigestAlgorithm());
            IMessageDigest digest = DigestAlgorithms.GetMessageDigest(algorithm);
            byte[] buf = new byte[8192];
            int rd;
            while ((rd = inputStream.JRead(buf, 0, buf.Length)) > 0) {
                digest.Update(buf, 0, rd);
            }
            return digest.Digest();
        }

        /// <summary>Creates MAC container as ASN1 object based on data digest, MAC key and signature parameters.</summary>
        /// <param name="dataDigest">
        /// data digest as
        /// <c>byte[]</c>
        /// to be used during MAC container creation
        /// </param>
        /// <param name="macKey">
        /// MAC key as
        /// <c>byte[]</c>
        /// to be used during MAC container creation
        /// </param>
        /// <param name="signature">
        /// signature value as
        /// <c>byte[]</c>
        /// to be used during MAC container creation
        /// </param>
        /// <returns>
        /// MAC container as
        /// <see cref="iText.Commons.Bouncycastle.Asn1.IDerSequence"/>.
        /// </returns>
        protected internal virtual IDerSequence CreateMacContainer(byte[] dataDigest, byte[] macKey, byte[] signature
            ) {
            IAsn1EncodableVector contentInfoV = BC_FACTORY.CreateASN1EncodableVector();
            contentInfoV.Add(BC_FACTORY.CreateASN1ObjectIdentifier(OID.AUTHENTICATED_DATA));
            // Recipient info
            IAsn1EncodableVector recInfoV = BC_FACTORY.CreateASN1EncodableVector();
            recInfoV.Add(BC_FACTORY.CreateASN1Integer(0));
            // version
            recInfoV.Add(BC_FACTORY.CreateDERTaggedObject(0, BC_FACTORY.CreateASN1ObjectIdentifier(OID.KDF_PDF_MAC_WRAP_KDF
                )));
            recInfoV.Add(BC_FACTORY.CreateDERSequence(BC_FACTORY.CreateASN1ObjectIdentifier(GetKeyWrappingAlgorithmOid
                ())));
            ////////////////////// KEK
            byte[] macKek = BC_FACTORY.GenerateHKDF(fileEncryptionKey, kdfSalt, PDF_MAC.GetBytes(System.Text.Encoding.
                UTF8));
            byte[] encryptedKey = GenerateEncryptedKey(macKey, macKek);
            recInfoV.Add(BC_FACTORY.CreateDEROctetString(encryptedKey));
            // Digest info
            byte[] messageBytes = CreatePdfMacIntegrityInfo(dataDigest, signature == null ? null : DigestBytes(signature
                ));
            // Encapsulated content info
            IAsn1EncodableVector encapContentInfoV = BC_FACTORY.CreateASN1EncodableVector();
            encapContentInfoV.Add(BC_FACTORY.CreateASN1ObjectIdentifier(OID.CT_PDF_MAC_INTEGRITY_INFO));
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
            String algorithm = MacProperties.MacDigestAlgorithmToString(macProperties.GetMacDigestAlgorithm());
            String macDigestOid = DigestAlgorithms.GetAllowedDigest(algorithm);
            authDataV.Add(BC_FACTORY.CreateDERTaggedObject(false, 1, BC_FACTORY.CreateDERSequence(BC_FACTORY.CreateASN1ObjectIdentifier
                (macDigestOid))));
            authDataV.Add(BC_FACTORY.CreateDERSequence(encapContentInfoV));
            authDataV.Add(BC_FACTORY.CreateDERTaggedObject(false, 2, authAttrs));
            authDataV.Add(BC_FACTORY.CreateDEROctetString(mac));
            contentInfoV.Add(BC_FACTORY.CreateDERTaggedObject(0, BC_FACTORY.CreateDERSequence(authDataV)));
            return BC_FACTORY.CreateDERSequence(contentInfoV);
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

        private byte[] GenerateDecryptedKey(byte[] encryptedMacKey) {
            byte[] macKek = BC_FACTORY.GenerateHKDF(fileEncryptionKey, kdfSalt, PDF_MAC.GetBytes(System.Text.Encoding.
                UTF8));
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

        private IDerSequence CreateMessageDigestSequence(byte[] messageBytes) {
            String algorithm = MacProperties.MacDigestAlgorithmToString(macProperties.GetMacDigestAlgorithm());
            // Hash messageBytes to get messageDigest attribute
            IMessageDigest digest = DigestAlgorithms.GetMessageDigest(algorithm);
            digest.Update(messageBytes);
            byte[] messageDigest = DigestBytes(messageBytes);
            // Message digest
            IAsn1EncodableVector messageDigestV = BC_FACTORY.CreateASN1EncodableVector();
            messageDigestV.Add(BC_FACTORY.CreateASN1ObjectIdentifier(OID.MESSAGE_DIGEST));
            messageDigestV.Add(BC_FACTORY.CreateDERSet(BC_FACTORY.CreateDEROctetString(messageDigest)));
            return BC_FACTORY.CreateDERSequence(messageDigestV);
        }

        private IDerSet CreateAuthAttributes(byte[] messageBytes) {
            // Content type - mac integrity info
            IAsn1EncodableVector contentTypeInfoV = BC_FACTORY.CreateASN1EncodableVector();
            contentTypeInfoV.Add(BC_FACTORY.CreateASN1ObjectIdentifier(OID.CONTENT_TYPE));
            contentTypeInfoV.Add(BC_FACTORY.CreateDERSet(BC_FACTORY.CreateASN1ObjectIdentifier(OID.CT_PDF_MAC_INTEGRITY_INFO
                )));
            IAsn1EncodableVector algorithmsInfoV = BC_FACTORY.CreateASN1EncodableVector();
            String algorithm = MacProperties.MacDigestAlgorithmToString(macProperties.GetMacDigestAlgorithm());
            String macDigestOid = DigestAlgorithms.GetAllowedDigest(algorithm);
            algorithmsInfoV.Add(BC_FACTORY.CreateDERSequence(BC_FACTORY.CreateASN1ObjectIdentifier(macDigestOid)));
            algorithmsInfoV.Add(BC_FACTORY.CreateDERTaggedObject(2, BC_FACTORY.CreateASN1ObjectIdentifier(GetMacAlgorithmOid
                ())));
            // CMS algorithm protection
            IAsn1EncodableVector algoProtectionInfoV = BC_FACTORY.CreateASN1EncodableVector();
            algoProtectionInfoV.Add(BC_FACTORY.CreateASN1ObjectIdentifier(OID.CMS_ALGORITHM_PROTECTION));
            algoProtectionInfoV.Add(BC_FACTORY.CreateDERSet(BC_FACTORY.CreateDERSequence(algorithmsInfoV)));
            IAsn1EncodableVector authAttrsV = BC_FACTORY.CreateASN1EncodableVector();
            authAttrsV.Add(BC_FACTORY.CreateDERSequence(contentTypeInfoV));
            authAttrsV.Add(BC_FACTORY.CreateDERSequence(algoProtectionInfoV));
            authAttrsV.Add(CreateMessageDigestSequence(messageBytes));
            return BC_FACTORY.CreateDERSet(authAttrsV);
        }

        private static byte[] CreatePdfMacIntegrityInfo(byte[] dataDigest, byte[] signatureDigest) {
            IAsn1EncodableVector digestInfoV = BC_FACTORY.CreateASN1EncodableVector();
            digestInfoV.Add(BC_FACTORY.CreateASN1Integer(0));
            digestInfoV.Add(BC_FACTORY.CreateDEROctetString(dataDigest));
            if (signatureDigest != null) {
                digestInfoV.Add(BC_FACTORY.CreateDERTaggedObject(false, 0, BC_FACTORY.CreateDEROctetString(signatureDigest
                    )));
            }
            return BC_FACTORY.CreateDERSequence(digestInfoV).GetEncoded();
        }

        protected internal static byte[] GenerateRandomBytes(int length) {
            byte[] randomBytes = new byte[length];
            BC_FACTORY.GetSecureRandom().GetBytes(randomBytes);
            return randomBytes;
        }

        private static MacProperties.MacDigestAlgorithm GetMacDigestAlgorithm(String oid) {
            switch (oid) {
                case OID.SHA_256: {
                    return MacProperties.MacDigestAlgorithm.SHA_256;
                }

                case OID.SHA_384: {
                    return MacProperties.MacDigestAlgorithm.SHA_384;
                }

                case OID.SHA_512: {
                    return MacProperties.MacDigestAlgorithm.SHA_512;
                }

                case OID.SHA3_256: {
                    return MacProperties.MacDigestAlgorithm.SHA3_256;
                }

                case OID.SHA3_384: {
                    return MacProperties.MacDigestAlgorithm.SHA3_384;
                }

                case OID.SHA3_512: {
                    return MacProperties.MacDigestAlgorithm.SHA3_512;
                }

                default: {
                    throw new PdfException(KernelExceptionMessageConstant.DIGEST_NOT_SUPPORTED);
                }
            }
        }
    }
}
