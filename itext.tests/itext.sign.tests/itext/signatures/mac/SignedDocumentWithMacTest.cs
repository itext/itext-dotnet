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
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using iText.Bouncycastleconnector;
using iText.Commons.Bouncycastle;
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Bouncycastle.Crypto;
using iText.Commons.Utils;
using iText.Kernel.Crypto;
using iText.Kernel.Logs;
using iText.Kernel.Pdf;
using iText.Signatures;
using iText.Signatures.Testutils;
using iText.Signatures.Testutils.Client;
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

        public static IEnumerable<Object[]> CreateParameters() {
            return JavaUtil.ArraysAsList(new Object[] { "signCertRsa01.pem", "signDetached" }, new Object[] { "tsaCert.pem"
                , "timestamping" }, new Object[] { "signCertRsa01.pem", "signExternalContainerReal" }, new Object[] { 
                "signCertRsa01.pem", "signExternalContainerBlank" });
        }

        [NUnit.Framework.TestCaseSource("CreateParameters")]
        [LogMessage(KernelLogMessageConstant.MD5_IS_NOT_FIPS_COMPLIANT, Ignore = true)]
        public virtual void SignMacProtectedDocTest(String certName, String signingOperation) {
            String fileName = "signMacProtectedDocTest_" + signingOperation + ".pdf";
            String srcFileName = SOURCE_FOLDER + "macEncryptedDoc.pdf";
            String outputFileName = DESTINATION_FOLDER + fileName;
            String signCertFileName = CERTS_SRC + certName;
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName;
            IX509Certificate[] signRsaChain = PemFileHelper.ReadFirstChain(signCertFileName);
            IPrivateKey signRsaPrivateKey = PemFileHelper.ReadFirstKey(signCertFileName, PRIVATE_KEY_PASSWORD);
            using (PdfReader reader = new PdfReader(srcFileName, new ReaderProperties().SetPassword(ENCRYPTION_PASSWORD
                ))) {
                using (Stream outputStream = FileUtil.GetFileOutputStream(outputFileName)) {
                    PdfSigner pdfSigner = new PdfSigner(reader, outputStream, new StampingProperties());
                    PerformSigningOperation(signingOperation, pdfSigner, signRsaPrivateKey, signRsaChain);
                }
            }
            ReaderProperties properties = new ReaderProperties().SetPassword(ENCRYPTION_PASSWORD);
            NUnit.Framework.Assert.IsNull(SignaturesCompareTool.CompareSignatures(outputFileName, cmpFileName, properties
                , properties));
        }

        [NUnit.Framework.TestCaseSource("CreateParameters")]
        [LogMessage(KernelLogMessageConstant.MD5_IS_NOT_FIPS_COMPLIANT, Ignore = true)]
        public virtual void SignNotMacProtectedDocTest(String certName, String signingOperation) {
            String fileName = "signNotMacProtectedDocTest_" + signingOperation + ".pdf";
            String srcFileName = SOURCE_FOLDER + "noMacProtectionDocument.pdf";
            String outputFileName = DESTINATION_FOLDER + fileName;
            String signCertFileName = CERTS_SRC + certName;
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName;
            IX509Certificate[] signRsaChain = PemFileHelper.ReadFirstChain(signCertFileName);
            IPrivateKey signRsaPrivateKey = PemFileHelper.ReadFirstKey(signCertFileName, PRIVATE_KEY_PASSWORD);
            using (PdfReader reader = new PdfReader(srcFileName, new ReaderProperties().SetPassword(ENCRYPTION_PASSWORD
                ))) {
                using (Stream outputStream = FileUtil.GetFileOutputStream(outputFileName)) {
                    PdfSigner pdfSigner = new PdfSigner(reader, outputStream, new StampingProperties());
                    PerformSigningOperation(signingOperation, pdfSigner, signRsaPrivateKey, signRsaChain);
                }
            }
            ReaderProperties properties = new ReaderProperties().SetPassword(ENCRYPTION_PASSWORD);
            NUnit.Framework.Assert.IsNull(SignaturesCompareTool.CompareSignatures(outputFileName, cmpFileName, properties
                , properties));
        }

        [NUnit.Framework.TestCaseSource("CreateParameters")]
        [LogMessage(KernelLogMessageConstant.MD5_IS_NOT_FIPS_COMPLIANT, Ignore = true)]
        public virtual void SignNotMacProtectedDoc17Test(String certName, String signingOperation) {
            String fileName = "signNotMacProtectedDoc17Test_" + signingOperation + ".pdf";
            String srcFileName = SOURCE_FOLDER + "noMacProtectionDocument_1_7.pdf";
            String outputFileName = DESTINATION_FOLDER + fileName;
            String signCertFileName = CERTS_SRC + certName;
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName;
            IX509Certificate[] signRsaChain = PemFileHelper.ReadFirstChain(signCertFileName);
            IPrivateKey signRsaPrivateKey = PemFileHelper.ReadFirstKey(signCertFileName, PRIVATE_KEY_PASSWORD);
            using (PdfReader reader = new PdfReader(srcFileName, new ReaderProperties().SetPassword(ENCRYPTION_PASSWORD
                ))) {
                using (Stream outputStream = FileUtil.GetFileOutputStream(outputFileName)) {
                    PdfSigner pdfSigner = new PdfSigner(reader, outputStream, new StampingProperties());
                    PerformSigningOperation(signingOperation, pdfSigner, signRsaPrivateKey, signRsaChain);
                }
            }
            if (!signingOperation.Equals("signExternalContainerBlank")) {
                ReaderProperties properties = new ReaderProperties().SetPassword(ENCRYPTION_PASSWORD);
                NUnit.Framework.Assert.IsNull(SignaturesCompareTool.CompareSignatures(outputFileName, cmpFileName, properties
                    , properties));
            }
        }

        [NUnit.Framework.TestCaseSource("CreateParameters")]
        [LogMessage(KernelLogMessageConstant.MD5_IS_NOT_FIPS_COMPLIANT, Ignore = true)]
        public virtual void SignNotMacProtectedDocInAppendModeTest(String certName, String signingOperation) {
            // MAC should not be added in append mode
            String fileName = "signNotMacProtectedDocInAppendModeTest_" + signingOperation + ".pdf";
            String srcFileName = SOURCE_FOLDER + "noMacProtectionDocument.pdf";
            String outputFileName = DESTINATION_FOLDER + fileName;
            String signCertFileName = CERTS_SRC + certName;
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName;
            IX509Certificate[] signRsaChain = PemFileHelper.ReadFirstChain(signCertFileName);
            IPrivateKey signRsaPrivateKey = PemFileHelper.ReadFirstKey(signCertFileName, PRIVATE_KEY_PASSWORD);
            using (PdfReader reader = new PdfReader(srcFileName, new ReaderProperties().SetPassword(ENCRYPTION_PASSWORD
                ))) {
                using (Stream outputStream = FileUtil.GetFileOutputStream(outputFileName)) {
                    PdfSigner pdfSigner = new PdfSigner(reader, outputStream, new StampingProperties().UseAppendMode());
                    PerformSigningOperation(signingOperation, pdfSigner, signRsaPrivateKey, signRsaChain);
                }
            }
            if (!signingOperation.Equals("signExternalContainerBlank")) {
                ReaderProperties properties = new ReaderProperties().SetPassword(ENCRYPTION_PASSWORD);
                NUnit.Framework.Assert.IsNull(SignaturesCompareTool.CompareSignatures(outputFileName, cmpFileName, properties
                    , properties));
            }
        }

        [NUnit.Framework.TestCaseSource("CreateParameters")]
        [LogMessage(KernelLogMessageConstant.MD5_IS_NOT_FIPS_COMPLIANT, Ignore = true)]
        public virtual void SignMacProtectedDocInAppendModeTest(String certName, String signingOperation) {
            String fileName = "signMacProtectedDocInAppendModeTest_" + signingOperation + ".pdf";
            String srcFileName = SOURCE_FOLDER + "macEncryptedDoc.pdf";
            String outputFileName = DESTINATION_FOLDER + fileName;
            String signCertFileName = CERTS_SRC + certName;
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName;
            IX509Certificate[] signRsaChain = PemFileHelper.ReadFirstChain(signCertFileName);
            IPrivateKey signRsaPrivateKey = PemFileHelper.ReadFirstKey(signCertFileName, PRIVATE_KEY_PASSWORD);
            using (PdfReader reader = new PdfReader(srcFileName, new ReaderProperties().SetPassword(ENCRYPTION_PASSWORD
                ))) {
                using (Stream outputStream = FileUtil.GetFileOutputStream(outputFileName)) {
                    PdfSigner pdfSigner = new PdfSigner(reader, outputStream, new StampingProperties().UseAppendMode());
                    PerformSigningOperation(signingOperation, pdfSigner, signRsaPrivateKey, signRsaChain);
                }
            }
            ReaderProperties properties = new ReaderProperties().SetPassword(ENCRYPTION_PASSWORD);
            NUnit.Framework.Assert.IsNull(SignaturesCompareTool.CompareSignatures(outputFileName, cmpFileName, properties
                , properties));
        }

        [NUnit.Framework.TestCaseSource("CreateParameters")]
        [LogMessage(KernelLogMessageConstant.MD5_IS_NOT_FIPS_COMPLIANT, Ignore = true)]
        public virtual void SignMacProtectedDocWithSHA3_384Test(String certName, String signingOperation) {
            String fileName = "signMacProtectedDocWithSHA3_384Test_" + signingOperation + ".pdf";
            String srcFileName = SOURCE_FOLDER + "macEncryptedDocSHA3_384.pdf";
            String outputFileName = DESTINATION_FOLDER + fileName;
            String signCertFileName = CERTS_SRC + certName;
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName;
            IX509Certificate[] signRsaChain = PemFileHelper.ReadFirstChain(signCertFileName);
            IPrivateKey signRsaPrivateKey = PemFileHelper.ReadFirstKey(signCertFileName, PRIVATE_KEY_PASSWORD);
            using (PdfReader reader = new PdfReader(srcFileName, new ReaderProperties().SetPassword(ENCRYPTION_PASSWORD
                ))) {
                using (Stream outputStream = FileUtil.GetFileOutputStream(outputFileName)) {
                    PdfSigner pdfSigner = new PdfSigner(reader, outputStream, new StampingProperties());
                    PerformSigningOperation(signingOperation, pdfSigner, signRsaPrivateKey, signRsaChain);
                }
            }
            ReaderProperties properties = new ReaderProperties().SetPassword(ENCRYPTION_PASSWORD);
            NUnit.Framework.Assert.IsNull(SignaturesCompareTool.CompareSignatures(outputFileName, cmpFileName, properties
                , properties));
        }

        [NUnit.Framework.TestCaseSource("CreateParameters")]
        [LogMessage(KernelLogMessageConstant.MD5_IS_NOT_FIPS_COMPLIANT, Ignore = true)]
        public virtual void SignMacPublicEncryptionDocTest(String certName, String signingOperation) {
            try {
                BouncyCastleFactoryCreator.GetFactory().IsEncryptionFeatureSupported(0, true);
            }
            catch (Exception) {
                NUnit.Framework.Assume.That(false);
            }
            String fileName = "signMacPublicEncryptionDocTest_" + signingOperation + ".pdf";
            String srcFileName = SOURCE_FOLDER + "macEncryptedWithPublicHandlerDoc.pdf";
            String outputFileName = DESTINATION_FOLDER + fileName;
            String signCertFileName = CERTS_SRC + certName;
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
                    PerformSigningOperation(signingOperation, pdfSigner, signRsaPrivateKey, signRsaChain);
                }
            }
            NUnit.Framework.Assert.IsNull(SignaturesCompareTool.CompareSignatures(outputFileName, cmpFileName, properties
                , properties));
        }

        private static void PerformSigningOperation(String signingOperation, PdfSigner pdfSigner, IPrivateKey privateKey
            , IX509Certificate[] chain) {
            switch (signingOperation) {
                case "signDetached": {
                    PerformSignDetached(pdfSigner, privateKey, chain);
                    break;
                }

                case "timestamping": {
                    PerformTimestamping(pdfSigner, privateKey, chain);
                    break;
                }

                case "signExternalContainerReal": {
                    PerformSignExternalContainerReal(pdfSigner, privateKey, chain);
                    break;
                }

                case "signExternalContainerBlank": {
                    PerformSignExternalContainerBlank(pdfSigner);
                    break;
                }
            }
        }

        private static void PerformSignDetached(PdfSigner pdfSigner, IPrivateKey privateKey, IX509Certificate[] chain
            ) {
            pdfSigner.SignDetached(new PrivateKeySignature(privateKey, DigestAlgorithms.SHA256), chain, null, null, null
                , 0, PdfSigner.CryptoStandard.CADES);
        }

        private static void PerformSignExternalContainerReal(PdfSigner pdfSigner, IPrivateKey privateKey, IX509Certificate
            [] chain) {
            pdfSigner.SignExternalContainer(new PKCS7ExternalSignatureContainer(privateKey, chain, "SHA-512"), 5000);
        }

        private static void PerformSignExternalContainerBlank(PdfSigner pdfSigner) {
            pdfSigner.SignExternalContainer(new ExternalBlankSignatureContainer(PdfName.Adobe_PPKLite, PdfName.Adbe_pkcs7_detached
                ), 5000);
        }

        private static void PerformTimestamping(PdfSigner pdfSigner, IPrivateKey privateKey, IX509Certificate[] chain
            ) {
            pdfSigner.Timestamp(new TestTsaClient(JavaUtil.ArraysAsList(chain), privateKey), "timestamp1");
        }
    }
}
