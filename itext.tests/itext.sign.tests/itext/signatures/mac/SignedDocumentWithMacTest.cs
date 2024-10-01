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
using NUnit.Framework;
using iText.Bouncycastleconnector;
using iText.Commons.Bouncycastle;
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Bouncycastle.Crypto;
using iText.Commons.Utils;
using iText.Kernel.Crypto;
using iText.Kernel.Exceptions;
using iText.Kernel.Logs;
using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using iText.Signatures;
using iText.Signatures.Testutils;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Signatures.Mac {
    [NUnit.Framework.Category("BouncyCastleIntegrationTest")]
    public class SignedDocumentWithMacTest : ExtendedITextTest {
        private static readonly IBouncyCastleFactory FACTORY = BouncyCastleFactoryCreator.GetFactory();

        private static readonly String CERTS_SRC = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/signatures/mac/SignedDocumentWithMacTest/certs/";

        private static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/signatures/mac/SignedDocumentWithMacTest/";

        private static readonly String DESTINATION_FOLDER = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/signatures/mac/SignedDocumentWithMacTest/";

        private static readonly byte[] ENCRYPTION_PASSWORD = "123".GetBytes();

        private static readonly char[] PRIVATE_KEY_PASSWORD = "testpassphrase".ToCharArray();

        [NUnit.Framework.OneTimeSetUp]
        public static void Before() {
            CreateOrClearDestinationFolder(DESTINATION_FOLDER);
        }

        [NUnit.Framework.Test]
        [LogMessage(KernelLogMessageConstant.MD5_IS_NOT_FIPS_COMPLIANT, Ignore = true)]
        public virtual void SignMacProtectedDocTest() {
            String fileName = "signMacProtectedDocTest.pdf";
            String srcFileName = SOURCE_FOLDER + "macEncryptedDoc.pdf";
            String outputFileName = DESTINATION_FOLDER + fileName;
            String signCertFileName = CERTS_SRC + "signCertRsa01.pem";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName;
            IX509Certificate[] signRsaChain = PemFileHelper.ReadFirstChain(signCertFileName);
            IPrivateKey signRsaPrivateKey = PemFileHelper.ReadFirstKey(signCertFileName, PRIVATE_KEY_PASSWORD);
            using (PdfReader reader = new PdfReader(srcFileName, new ReaderProperties().SetPassword(ENCRYPTION_PASSWORD
                ))) {
                using (Stream outputStream = FileUtil.GetFileOutputStream(outputFileName)) {
                    PdfSigner pdfSigner = new PdfSigner(reader, outputStream, new StampingProperties());
                    PerformSignDetached(pdfSigner, signRsaPrivateKey, signRsaChain);
                }
            }
            ReaderProperties properties = new ReaderProperties().SetPassword(ENCRYPTION_PASSWORD);
            NUnit.Framework.Assert.IsNull(SignaturesCompareTool.CompareSignatures(outputFileName, cmpFileName, properties
                , properties));
        }

        [NUnit.Framework.Test]
        [LogMessage(KernelLogMessageConstant.MD5_IS_NOT_FIPS_COMPLIANT, Ignore = true)]
        public virtual void SignNotMacProtectedDocTest() {
            String fileName = "signNotMacProtectedDocTest.pdf";
            String srcFileName = SOURCE_FOLDER + "noMacProtectionDocument.pdf";
            String outputFileName = DESTINATION_FOLDER + fileName;
            String signCertFileName = CERTS_SRC + "signCertRsa01.pem";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName;
            IX509Certificate[] signRsaChain = PemFileHelper.ReadFirstChain(signCertFileName);
            IPrivateKey signRsaPrivateKey = PemFileHelper.ReadFirstKey(signCertFileName, PRIVATE_KEY_PASSWORD);
            using (PdfReader reader = new PdfReader(srcFileName, new ReaderProperties().SetPassword(ENCRYPTION_PASSWORD
                ))) {
                using (Stream outputStream = FileUtil.GetFileOutputStream(outputFileName)) {
                    PdfSigner pdfSigner = new PdfSigner(reader, outputStream, new StampingProperties());
                    PerformSignDetached(pdfSigner, signRsaPrivateKey, signRsaChain);
                }
            }
            ReaderProperties properties = new ReaderProperties().SetPassword(ENCRYPTION_PASSWORD);
            NUnit.Framework.Assert.IsNull(SignaturesCompareTool.CompareSignatures(outputFileName, cmpFileName, properties
                , properties));
        }

        [NUnit.Framework.Test]
        [LogMessage(KernelLogMessageConstant.MD5_IS_NOT_FIPS_COMPLIANT, Ignore = true)]
        public virtual void SignNotMacProtectedDoc17Test() {
            String fileName = "signNotMacProtectedDoc17Test.pdf";
            String srcFileName = SOURCE_FOLDER + "noMacProtectionDocument_1_7.pdf";
            String outputFileName = DESTINATION_FOLDER + fileName;
            String signCertFileName = CERTS_SRC + "signCertRsa01.pem";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName;
            IX509Certificate[] signRsaChain = PemFileHelper.ReadFirstChain(signCertFileName);
            IPrivateKey signRsaPrivateKey = PemFileHelper.ReadFirstKey(signCertFileName, PRIVATE_KEY_PASSWORD);
            using (PdfReader reader = new PdfReader(srcFileName, new ReaderProperties().SetPassword(ENCRYPTION_PASSWORD
                ))) {
                using (Stream outputStream = FileUtil.GetFileOutputStream(outputFileName)) {
                    PdfSigner pdfSigner = new PdfSigner(reader, outputStream, new StampingProperties());
                    PerformSignDetached(pdfSigner, signRsaPrivateKey, signRsaChain);
                }
            }
            ReaderProperties properties = new ReaderProperties().SetPassword(ENCRYPTION_PASSWORD);
            NUnit.Framework.Assert.IsNull(SignaturesCompareTool.CompareSignatures(outputFileName, cmpFileName, properties
                , properties));
        }

        [NUnit.Framework.Test]
        [LogMessage(KernelLogMessageConstant.MD5_IS_NOT_FIPS_COMPLIANT, Ignore = true)]
        public virtual void SignNotMacProtectedDocInAppendModeTest() {
            // MAC should not be added in append mode
            String fileName = "signNotMacProtectedDocInAppendModeTest.pdf";
            String srcFileName = SOURCE_FOLDER + "noMacProtectionDocument.pdf";
            String outputFileName = DESTINATION_FOLDER + fileName;
            String signCertFileName = CERTS_SRC + "signCertRsa01.pem";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName;
            IX509Certificate[] signRsaChain = PemFileHelper.ReadFirstChain(signCertFileName);
            IPrivateKey signRsaPrivateKey = PemFileHelper.ReadFirstKey(signCertFileName, PRIVATE_KEY_PASSWORD);
            using (PdfReader reader = new PdfReader(srcFileName, new ReaderProperties().SetPassword(ENCRYPTION_PASSWORD
                ))) {
                using (Stream outputStream = FileUtil.GetFileOutputStream(outputFileName)) {
                    PdfSigner pdfSigner = new PdfSigner(reader, outputStream, new StampingProperties().UseAppendMode());
                    PerformSignDetached(pdfSigner, signRsaPrivateKey, signRsaChain);
                }
            }
            ReaderProperties properties = new ReaderProperties().SetPassword(ENCRYPTION_PASSWORD);
            NUnit.Framework.Assert.IsNull(SignaturesCompareTool.CompareSignatures(outputFileName, cmpFileName, properties
                , properties));
        }

        [NUnit.Framework.Test]
        [LogMessage(KernelLogMessageConstant.MD5_IS_NOT_FIPS_COMPLIANT, Ignore = true)]
        public virtual void SignMacProtectedDocInAppendModeTest() {
            String fileName = "signMacProtectedDocInAppendModeTest.pdf";
            String srcFileName = SOURCE_FOLDER + "macEncryptedDoc.pdf";
            String outputFileName = DESTINATION_FOLDER + fileName;
            String signCertFileName = CERTS_SRC + "signCertRsa01.pem";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName;
            IX509Certificate[] signRsaChain = PemFileHelper.ReadFirstChain(signCertFileName);
            IPrivateKey signRsaPrivateKey = PemFileHelper.ReadFirstKey(signCertFileName, PRIVATE_KEY_PASSWORD);
            using (PdfReader reader = new PdfReader(srcFileName, new ReaderProperties().SetPassword(ENCRYPTION_PASSWORD
                ))) {
                using (Stream outputStream = FileUtil.GetFileOutputStream(outputFileName)) {
                    PdfSigner pdfSigner = new PdfSigner(reader, outputStream, new StampingProperties().UseAppendMode());
                    PerformSignDetached(pdfSigner, signRsaPrivateKey, signRsaChain);
                }
            }
            ReaderProperties properties = new ReaderProperties().SetPassword(ENCRYPTION_PASSWORD);
            NUnit.Framework.Assert.IsNull(SignaturesCompareTool.CompareSignatures(outputFileName, cmpFileName, properties
                , properties));
        }

        [NUnit.Framework.Test]
        [LogMessage(KernelLogMessageConstant.MD5_IS_NOT_FIPS_COMPLIANT, Ignore = true)]
        public virtual void SignMacProtectedDocWithSHA3_384Test() {
            String fileName = "signMacProtectedDocWithSHA3_384Test.pdf";
            String srcFileName = SOURCE_FOLDER + "macEncryptedDocSHA3_384.pdf";
            String outputFileName = DESTINATION_FOLDER + fileName;
            String signCertFileName = CERTS_SRC + "signCertRsa01.pem";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName;
            IX509Certificate[] signRsaChain = PemFileHelper.ReadFirstChain(signCertFileName);
            IPrivateKey signRsaPrivateKey = PemFileHelper.ReadFirstKey(signCertFileName, PRIVATE_KEY_PASSWORD);
            using (PdfReader reader = new PdfReader(srcFileName, new ReaderProperties().SetPassword(ENCRYPTION_PASSWORD
                ))) {
                using (Stream outputStream = FileUtil.GetFileOutputStream(outputFileName)) {
                    PdfSigner pdfSigner = new PdfSigner(reader, outputStream, new StampingProperties());
                    PerformSignDetached(pdfSigner, signRsaPrivateKey, signRsaChain);
                }
            }
            ReaderProperties properties = new ReaderProperties().SetPassword(ENCRYPTION_PASSWORD);
            NUnit.Framework.Assert.IsNull(SignaturesCompareTool.CompareSignatures(outputFileName, cmpFileName, properties
                , properties));
        }

        [NUnit.Framework.Test]
        [LogMessage(KernelLogMessageConstant.MD5_IS_NOT_FIPS_COMPLIANT, Ignore = true)]
        public virtual void SignMacPublicEncryptionDocTest() {
            try {
                BouncyCastleFactoryCreator.GetFactory().IsEncryptionFeatureSupported(0, true);
            }
            catch (Exception) {
                NUnit.Framework.Assume.That(false);
            }
            String fileName = "signMacPublicEncryptionDocTest.pdf";
            String srcFileName = SOURCE_FOLDER + "macEncryptedWithPublicHandlerDoc.pdf";
            String outputFileName = DESTINATION_FOLDER + fileName;
            String signCertFileName = CERTS_SRC + "signCertRsa01.pem";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName;
            IX509Certificate[] signRsaChain = PemFileHelper.ReadFirstChain(signCertFileName);
            IPrivateKey signRsaPrivateKey = PemFileHelper.ReadFirstKey(signCertFileName, PRIVATE_KEY_PASSWORD);
            IX509Certificate certificate = CryptoUtil.ReadPublicCertificate(FileUtil.GetInputStreamForFile(CERTS_SRC +
                 "SHA256withRSA.cer"));
            IPrivateKey privateKey = PemFileHelper.ReadFirstKey(CERTS_SRC + "SHA256withRSA.key", PRIVATE_KEY_PASSWORD);
            ReaderProperties properties = new ReaderProperties().SetPublicKeySecurityParams(certificate, privateKey);
            using (PdfReader reader = new PdfReader(srcFileName, properties)) {
                using (Stream outputStream = FileUtil.GetFileOutputStream(outputFileName)) {
                    PdfSigner pdfSigner = new PdfSigner(reader, outputStream, new StampingProperties());
                    PerformSignDetached(pdfSigner, signRsaPrivateKey, signRsaChain);
                }
            }
            NUnit.Framework.Assert.IsNull(SignaturesCompareTool.CompareSignatures(outputFileName, cmpFileName, properties
                , properties));
        }

        [NUnit.Framework.Test]
        [LogMessage(KernelLogMessageConstant.MD5_IS_NOT_FIPS_COMPLIANT, Ignore = true)]
        public virtual void ReadSignedMacProtectedInvalidDocTest() {
            String srcFileName = SOURCE_FOLDER + "signedMacProtectedInvalidDoc.pdf";
            String exceptionMessage = NUnit.Framework.Assert.Catch(typeof(PdfException), () => {
                using (PdfDocument ignored = new PdfDocument(new PdfReader(srcFileName, new ReaderProperties().SetPassword
                    (ENCRYPTION_PASSWORD)))) {
                }
            }
            ).Message;
            // Do nothing.
            NUnit.Framework.Assert.AreEqual(KernelExceptionMessageConstant.MAC_VALIDATION_FAILED, exceptionMessage);
        }

        [NUnit.Framework.Test]
        [LogMessage(KernelLogMessageConstant.MD5_IS_NOT_FIPS_COMPLIANT, Ignore = true)]
        public virtual void UpdateSignedMacProtectedDocumentTest() {
            String fileName = "updateSignedMacProtectedDocumentTest.pdf";
            String srcFileName = SOURCE_FOLDER + "thirdPartyMacProtectedAndSignedDocument.pdf";
            String outputFileName = DESTINATION_FOLDER + fileName;
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName;
            using (PdfDocument ignored = new PdfDocument(new PdfReader(srcFileName, new ReaderProperties().SetPassword
                (ENCRYPTION_PASSWORD)), new PdfWriter(FileUtil.GetFileOutputStream(outputFileName)), new StampingProperties
                ().UseAppendMode())) {
            }
            // Do nothing.
            // This call produces INFO log from AESCipher caused by exception while decrypting. The reason is that,
            // while comparing encrypted signed documents, CompareTool needs to mark signature value as unencrypted.
            // Instead, it tries to decrypt not encrypted value which results in exception.
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outputFileName, cmpFileName, DESTINATION_FOLDER
                , "diff", ENCRYPTION_PASSWORD, ENCRYPTION_PASSWORD));
        }

        private static void PerformSignDetached(PdfSigner pdfSigner, IPrivateKey privateKey, IX509Certificate[] chain
            ) {
            pdfSigner.SignDetached(new PrivateKeySignature(privateKey, DigestAlgorithms.SHA256), chain, null, null, null
                , 0, PdfSigner.CryptoStandard.CADES);
        }
    }
}
