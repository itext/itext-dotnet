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
using System;
using System.IO;
using iText.Bouncycastleconnector;
using iText.Commons.Bouncycastle;
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Bouncycastle.Crypto;
using iText.Commons.Utils;
using iText.Kernel.Pdf;
using iText.Signatures;
using iText.Signatures.Testutils;
using iText.Test;

namespace iText.Signatures.Mac {
    [NUnit.Framework.Category("BouncyCastleIntegrationTest")]
    public class TwoStepSigningWithMacTest : ExtendedITextTest {
        private static readonly IBouncyCastleFactory FACTORY = BouncyCastleFactoryCreator.GetFactory();

        private static readonly String CERTS_SRC = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/signatures/certs/";

        private static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/signatures/mac/TwoStepSigningWithMacTest/";

        private static readonly String DESTINATION_FOLDER = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/signatures/mac/TwoStepSigningWithMacTest/";

        private static readonly byte[] ENCRYPTION_PASSWORD = "123".GetBytes();

        private static readonly char[] PRIVATE_KEY_PASSWORD = "testpassphrase".ToCharArray();

        [NUnit.Framework.OneTimeSetUp]
        public static void Before() {
            CreateOrClearDestinationFolder(DESTINATION_FOLDER);
        }

        [NUnit.Framework.Test]
        public virtual void SignDeferredWithReaderTest() {
            String fileName = "signDeferredWithReaderTest.pdf";
            String srcFileName = SOURCE_FOLDER + "macEncryptedDoc.pdf";
            String outputFileName = DESTINATION_FOLDER + fileName;
            String signCertFileName = CERTS_SRC + "signCertRsa01.pem";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName;
            IX509Certificate[] signRsaChain = PemFileHelper.ReadFirstChain(signCertFileName);
            IPrivateKey signRsaPrivateKey = PemFileHelper.ReadFirstKey(signCertFileName, PRIVATE_KEY_PASSWORD);
            ReaderProperties properties = new ReaderProperties().SetPassword(ENCRYPTION_PASSWORD);
            MemoryStream preparedDocument = new MemoryStream();
            using (PdfReader reader = new PdfReader(srcFileName, properties)) {
                using (Stream outputStream = preparedDocument) {
                    PdfSigner pdfSigner = new PdfSigner(reader, outputStream, new StampingProperties());
                    pdfSigner.SignExternalContainer(new ExternalBlankSignatureContainer(PdfName.Adobe_PPKLite, PdfName.Adbe_pkcs7_detached
                        ), 5000);
                }
            }
            using (PdfReader reader_1 = new PdfReader(new MemoryStream(preparedDocument.ToArray()), properties)) {
                using (Stream outputStream_1 = FileUtil.GetFileOutputStream(outputFileName)) {
                    PdfSigner.SignDeferred(reader_1, "Signature1", outputStream_1, new PKCS7ExternalSignatureContainer(signRsaPrivateKey
                        , signRsaChain, "SHA-512"));
                }
            }
            NUnit.Framework.Assert.IsNull(SignaturesCompareTool.CompareSignatures(outputFileName, cmpFileName, properties
                , properties));
        }

        [NUnit.Framework.Test]
        public virtual void SignDeferredWithDocumentTest() {
            String fileName = "signDeferredWithDocumentTest.pdf";
            String srcFileName = SOURCE_FOLDER + "macEncryptedDoc.pdf";
            String outputFileName = DESTINATION_FOLDER + fileName;
            String signCertFileName = CERTS_SRC + "signCertRsa01.pem";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName;
            IX509Certificate[] signRsaChain = PemFileHelper.ReadFirstChain(signCertFileName);
            IPrivateKey signRsaPrivateKey = PemFileHelper.ReadFirstKey(signCertFileName, PRIVATE_KEY_PASSWORD);
            ReaderProperties properties = new ReaderProperties().SetPassword(ENCRYPTION_PASSWORD);
            MemoryStream preparedDocument = new MemoryStream();
            using (PdfReader reader = new PdfReader(srcFileName, properties)) {
                using (Stream outputStream = preparedDocument) {
                    PdfSigner pdfSigner = new PdfSigner(reader, outputStream, new StampingProperties());
                    pdfSigner.SignExternalContainer(new ExternalBlankSignatureContainer(PdfName.Adobe_PPKLite, PdfName.Adbe_pkcs7_detached
                        ), 5000);
                }
            }
            using (PdfDocument document = new PdfDocument(new PdfReader(new MemoryStream(preparedDocument.ToArray()), 
                properties))) {
                using (Stream outputStream_1 = FileUtil.GetFileOutputStream(outputFileName)) {
                    PdfSigner.SignDeferred(document, "Signature1", outputStream_1, new PKCS7ExternalSignatureContainer(signRsaPrivateKey
                        , signRsaChain, "SHA-512"));
                }
            }
            NUnit.Framework.Assert.IsNull(SignaturesCompareTool.CompareSignatures(outputFileName, cmpFileName, properties
                , properties));
        }

        [NUnit.Framework.Test]
        public virtual void TwoPhaseSignerWithReaderTest() {
            String fileName = "twoPhaseSignerWithReaderTest.pdf";
            String srcFileName = SOURCE_FOLDER + "macEncryptedDoc.pdf";
            String outputFileName = DESTINATION_FOLDER + fileName;
            String signCertFileName = CERTS_SRC + "signCertRsa01.pem";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName;
            IX509Certificate[] signRsaChain = PemFileHelper.ReadFirstChain(signCertFileName);
            IPrivateKey signRsaPrivateKey = PemFileHelper.ReadFirstKey(signCertFileName, PRIVATE_KEY_PASSWORD);
            ReaderProperties properties = new ReaderProperties().SetPassword(ENCRYPTION_PASSWORD);
            using (PdfReader reader = new PdfReader(FileUtil.GetInputStreamForFile(srcFileName), properties)) {
                using (MemoryStream outputStream = new MemoryStream()) {
                    PdfTwoPhaseSigner signer = new PdfTwoPhaseSigner(reader, outputStream);
                    SignerProperties signerProperties = new SignerProperties();
                    byte[] digest = signer.PrepareDocumentForSignature(signerProperties, "SHA-512", PdfName.Adobe_PPKLite, PdfName
                        .Adbe_pkcs7_detached, 5000, false);
                    String fieldName = signerProperties.GetFieldName();
                    byte[] signData = SignDigest(digest, signRsaChain, signRsaPrivateKey);
                    using (Stream outputStreamPhase2 = FileUtil.GetFileOutputStream(outputFileName)) {
                        using (PdfReader newReader = new PdfReader(new MemoryStream(outputStream.ToArray()), properties)) {
                            PdfTwoPhaseSigner.AddSignatureToPreparedDocument(newReader, fieldName, outputStreamPhase2, signData);
                        }
                    }
                }
            }
            NUnit.Framework.Assert.IsNull(SignaturesCompareTool.CompareSignatures(outputFileName, cmpFileName, properties
                , properties));
        }

        [NUnit.Framework.Test]
        public virtual void TwoPhaseSignerWithDocumentTest() {
            String fileName = "twoPhaseSignerWithDocumentTest.pdf";
            String srcFileName = SOURCE_FOLDER + "macEncryptedDoc.pdf";
            String outputFileName = DESTINATION_FOLDER + fileName;
            String signCertFileName = CERTS_SRC + "signCertRsa01.pem";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName;
            IX509Certificate[] signRsaChain = PemFileHelper.ReadFirstChain(signCertFileName);
            IPrivateKey signRsaPrivateKey = PemFileHelper.ReadFirstKey(signCertFileName, PRIVATE_KEY_PASSWORD);
            ReaderProperties properties = new ReaderProperties().SetPassword(ENCRYPTION_PASSWORD);
            using (PdfReader reader = new PdfReader(FileUtil.GetInputStreamForFile(srcFileName), properties)) {
                using (MemoryStream outputStream = new MemoryStream()) {
                    PdfTwoPhaseSigner signer = new PdfTwoPhaseSigner(reader, outputStream);
                    SignerProperties signerProperties = new SignerProperties();
                    byte[] digest = signer.PrepareDocumentForSignature(signerProperties, "SHA-512", PdfName.Adobe_PPKLite, PdfName
                        .Adbe_pkcs7_detached, 5000, false);
                    String fieldName = signerProperties.GetFieldName();
                    byte[] signData = SignDigest(digest, signRsaChain, signRsaPrivateKey);
                    using (Stream outputStreamPhase2 = FileUtil.GetFileOutputStream(outputFileName)) {
                        using (PdfDocument document = new PdfDocument(new PdfReader(new MemoryStream(outputStream.ToArray()), properties
                            ))) {
                            PdfTwoPhaseSigner.AddSignatureToPreparedDocument(document, fieldName, outputStreamPhase2, signData);
                        }
                    }
                }
            }
            NUnit.Framework.Assert.IsNull(SignaturesCompareTool.CompareSignatures(outputFileName, cmpFileName, properties
                , properties));
        }

        private byte[] SignDigest(byte[] data, IX509Certificate[] chain, IPrivateKey pk) {
            PdfPKCS7 sgn = new PdfPKCS7((IPrivateKey)null, chain, "SHA-512", new BouncyCastleDigest(), false);
            byte[] sh = sgn.GetAuthenticatedAttributeBytes(data, PdfSigner.CryptoStandard.CMS, null, null);
            PrivateKeySignature pkSign = new PrivateKeySignature(pk, "SHA-512");
            byte[] signData = pkSign.Sign(sh);
            sgn.SetExternalSignatureValue(signData, null, pkSign.GetSignatureAlgorithmName(), pkSign.GetSignatureMechanismParameters
                ());
            return sgn.GetEncodedPKCS7(data, PdfSigner.CryptoStandard.CMS, null, null, null);
        }
    }
}
