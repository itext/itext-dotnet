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
using NUnit.Framework;
using iText.Bouncycastleconnector;
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Bouncycastle.Crypto;
using iText.Commons.Utils;
using iText.IO.Util;
using iText.Kernel.Crypto;
using iText.Kernel.Exceptions;
using iText.Kernel.Geom;
using iText.Kernel.Logs;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Annot;
using iText.Kernel.Utils;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Kernel.Mac {
    [NUnit.Framework.Category("BouncyCastleIntegrationTest")]
    public class MacIntegrityProtectorCreationTest : ExtendedITextTest {
        private static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/kernel/mac/MacIntegrityProtectorCreationTest/";

        private static readonly String DESTINATION_FOLDER = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/kernel/mac/MacIntegrityProtectorCreationTest/";

        private static readonly String CERTS_SRC = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/kernel/mac/MacIntegrityProtectorCreationTest/certs/";

        private static readonly byte[] PASSWORD = "123".GetBytes();

        private static readonly String PROVIDER_NAME = BouncyCastleFactoryCreator.GetFactory().GetProviderName();

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateOrClearDestinationFolder(DESTINATION_FOLDER);
        }

        [NUnit.Framework.OneTimeTearDown]
        public static void AfterClass() {
            CompareTool.Cleanup(DESTINATION_FOLDER);
        }

        [NUnit.Framework.Test]
        public virtual void StandaloneMacStandardEncryptionTest() {
            String fileName = "standaloneMacStandardEncryptionTest.pdf";
            String outputFileName = DESTINATION_FOLDER + fileName;
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName;
            WriterProperties writerProperties = new WriterProperties().SetPdfVersion(PdfVersion.PDF_2_0).SetStandardEncryption
                (PASSWORD, PASSWORD, 0, EncryptionConstants.ENCRYPTION_AES_256, new MacProperties(MacProperties.MacDigestAlgorithm
                .SHA_256));
            using (PdfDocument pdfDoc = new PdfDocument(CompareTool.CreateTestPdfWriter(outputFileName, writerProperties
                ))) {
                pdfDoc.AddNewPage().AddAnnotation(new PdfTextAnnotation(new Rectangle(100, 100, 100, 100)));
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().EnableEncryptionCompare(false).CompareByContent(outputFileName
                , cmpFileName, DESTINATION_FOLDER, "diff", PASSWORD, PASSWORD));
        }

        [NUnit.Framework.Test]
        public virtual void NoMacProtectionTest() {
            String fileName = "noMacProtectionTest.pdf";
            String outputFileName = DESTINATION_FOLDER + fileName;
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName;
            WriterProperties writerProperties = new WriterProperties().SetPdfVersion(PdfVersion.PDF_2_0).SetStandardEncryption
                (PASSWORD, PASSWORD, 0, EncryptionConstants.ENCRYPTION_AES_256, null);
            using (PdfDocument pdfDoc = new PdfDocument(CompareTool.CreateTestPdfWriter(outputFileName, writerProperties
                ))) {
                pdfDoc.AddNewPage().AddAnnotation(new PdfTextAnnotation(new Rectangle(100, 100, 100, 100)));
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().EnableEncryptionCompare().CompareByContent(outputFileName, 
                cmpFileName, DESTINATION_FOLDER, "diff", PASSWORD, PASSWORD));
        }

        [NUnit.Framework.Test]
        [LogMessage(KernelLogMessageConstant.MD5_IS_NOT_FIPS_COMPLIANT, Ignore = true)]
        public virtual void MacEncryptionWithAesGcmTest() {
            String fileName = "macEncryptionWithAesGsmTest.pdf";
            String outputFileName = DESTINATION_FOLDER + fileName;
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName;
            WriterProperties writerProperties = new WriterProperties().SetPdfVersion(PdfVersion.PDF_2_0).SetStandardEncryption
                (PASSWORD, PASSWORD, 0, EncryptionConstants.ENCRYPTION_AES_GCM, new MacProperties(MacProperties.MacDigestAlgorithm
                .SHA_256));
            using (PdfDocument pdfDoc = new PdfDocument(CompareTool.CreateTestPdfWriter(outputFileName, writerProperties
                ))) {
                pdfDoc.AddNewPage().AddAnnotation(new PdfTextAnnotation(new Rectangle(100, 100, 100, 100)));
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().EnableEncryptionCompare(false).CompareByContent(outputFileName
                , cmpFileName, DESTINATION_FOLDER, "diff", PASSWORD, PASSWORD));
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.PDF_WRITER_CLOSING_FAILED)]
        public virtual void StandaloneMacUnwritableStreamTest() {
            WriterProperties writerProperties = new WriterProperties().SetPdfVersion(PdfVersion.PDF_2_0).SetStandardEncryption
                (PASSWORD, PASSWORD, 0, EncryptionConstants.ENCRYPTION_AES_256, new MacProperties(MacProperties.MacDigestAlgorithm
                .SHA_256));
            MemoryStream unwritableStream = new _MemoryStream_147();
            using (PdfDocument pdfDoc = new PdfDocument(new PdfWriter(unwritableStream, writerProperties))) {
                pdfDoc.AddNewPage().AddAnnotation(new PdfTextAnnotation(new Rectangle(100, 100, 100, 100)));
            }
            unwritableStream.Dispose();
        }

        private sealed class _MemoryStream_147 : MemoryStream {
            public _MemoryStream_147() {
            }

            public override void Write(byte[] b, int off, int len) {
                throw new Exception("expected");
            }
        }

        [NUnit.Framework.Test]
        public virtual void StandaloneMacWithAllHashAlgorithmsTest() {
            for (int i = 0; i < EnumUtil.GetAllValuesOfEnum<MacProperties.MacDigestAlgorithm>().Count; i++) {
                String fileName = "standaloneMacWithAllHashAlgorithmsTest" + (i + 1) + ".pdf";
                String outputFileName = DESTINATION_FOLDER + fileName;
                String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName;
                MacProperties macProperties = new MacProperties(EnumUtil.GetAllValuesOfEnum<MacProperties.MacDigestAlgorithm
                    >()[i], MacProperties.MacAlgorithm.HMAC_WITH_SHA_256, MacProperties.KeyWrappingAlgorithm.AES_256_NO_PADD
                    );
                WriterProperties writerProperties = new WriterProperties().SetPdfVersion(PdfVersion.PDF_2_0).SetStandardEncryption
                    (PASSWORD, PASSWORD, 0, EncryptionConstants.ENCRYPTION_AES_256, macProperties);
                using (PdfDocument pdfDoc = new PdfDocument(CompareTool.CreateTestPdfWriter(outputFileName, writerProperties
                    ))) {
                    pdfDoc.AddNewPage().AddAnnotation(new PdfTextAnnotation(new Rectangle(100, 100, 100, 100)));
                }
                NUnit.Framework.Assert.IsNull(new CompareTool().EnableEncryptionCompare(false).CompareByContent(outputFileName
                    , cmpFileName, DESTINATION_FOLDER, "diff", PASSWORD, PASSWORD));
            }
        }

        [NUnit.Framework.Test]
        public virtual void StandaloneMacPdfVersionNotSetTest() {
            String fileName = "standaloneMacPdfVersionNotSetTest.pdf";
            String outputFileName = DESTINATION_FOLDER + fileName;
            MacProperties macProperties = new MacProperties(MacProperties.MacDigestAlgorithm.SHA_256, MacProperties.MacAlgorithm
                .HMAC_WITH_SHA_256, MacProperties.KeyWrappingAlgorithm.AES_256_NO_PADD);
            WriterProperties writerProperties = new WriterProperties().SetStandardEncryption(PASSWORD, PASSWORD, 0, EncryptionConstants
                .ENCRYPTION_AES_256, macProperties);
            String exceptionMessage = NUnit.Framework.Assert.Catch(typeof(PdfException), () => {
                using (PdfDocument pdfDoc = new PdfDocument(CompareTool.CreateTestPdfWriter(outputFileName, writerProperties
                    ))) {
                    pdfDoc.AddNewPage().AddAnnotation(new PdfTextAnnotation(new Rectangle(100, 100, 100, 100)));
                }
            }
            ).Message;
            NUnit.Framework.Assert.AreEqual(KernelExceptionMessageConstant.MAC_FOR_PDF_2, exceptionMessage);
        }

        [NUnit.Framework.Test]
        [LogMessage(KernelLogMessageConstant.MD5_IS_NOT_FIPS_COMPLIANT, Ignore = true)]
        public virtual void AddMacOnPreserveEncryptionTest() {
            String fileName = "addMacOnPreserveEncryptionTest.pdf";
            String outputFileName = DESTINATION_FOLDER + fileName;
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName;
            using (PdfDocument pdfDoc = new PdfDocument(new PdfReader(SOURCE_FOLDER + "noMacProtectionDocument.pdf", new 
                ReaderProperties().SetPassword(PASSWORD)), CompareTool.CreateTestPdfWriter(outputFileName, new WriterProperties
                ().SetPdfVersion(PdfVersion.PDF_2_0)), new StampingProperties().PreserveEncryption())) {
                pdfDoc.AddNewPage().AddAnnotation(new PdfTextAnnotation(new Rectangle(100, 100, 100, 100)));
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().EnableEncryptionCompare(false).CompareByContent(outputFileName
                , cmpFileName, DESTINATION_FOLDER, "diff", PASSWORD, PASSWORD));
        }

        [NUnit.Framework.Test]
        [LogMessage(KernelLogMessageConstant.MD5_IS_NOT_FIPS_COMPLIANT, Ignore = true)]
        public virtual void AddMacOnAppendModeTest() {
            // MAC should not be added in append mode
            String fileName = "addMacOnAppendModeTest.pdf";
            String outputFileName = DESTINATION_FOLDER + fileName;
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName;
            using (PdfDocument pdfDoc = new PdfDocument(new PdfReader(SOURCE_FOLDER + "noMacProtectionDocument.pdf", new 
                ReaderProperties().SetPassword(PASSWORD)), CompareTool.CreateTestPdfWriter(outputFileName, new WriterProperties
                ().SetPdfVersion(PdfVersion.PDF_2_0)), new StampingProperties().UseAppendMode())) {
                pdfDoc.AddNewPage().AddAnnotation(new PdfTextAnnotation(new Rectangle(100, 100, 100, 100)));
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().EnableEncryptionCompare().CompareByContent(outputFileName, 
                cmpFileName, DESTINATION_FOLDER, "diff", PASSWORD, PASSWORD));
        }

        [NUnit.Framework.Test]
        public virtual void AddMacWithDisableMacPropertyTest() {
            // MAC should not be added in disable MAC mode even if it was provided with writer properties
            String fileName = "addMacWithDisableMacPropertyTest.pdf";
            String outputFileName = DESTINATION_FOLDER + fileName;
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName;
            MacProperties macProperties = new MacProperties(MacProperties.MacDigestAlgorithm.SHA_384);
            WriterProperties writerProperties = new WriterProperties().SetPdfVersion(PdfVersion.PDF_2_0).SetStandardEncryption
                (PASSWORD, PASSWORD, 0, EncryptionConstants.ENCRYPTION_AES_256, macProperties);
            using (PdfDocument pdfDoc = new PdfDocument(new PdfReader(SOURCE_FOLDER + "noMacProtectionDocument.pdf", new 
                ReaderProperties().SetPassword(PASSWORD)), new PdfWriter(outputFileName, writerProperties), new StampingProperties
                ().DisableMac())) {
                pdfDoc.AddNewPage().AddAnnotation(new PdfTextAnnotation(new Rectangle(100, 100, 100, 100)));
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().EnableEncryptionCompare().CompareByContent(outputFileName, 
                cmpFileName, DESTINATION_FOLDER, "diff", PASSWORD, PASSWORD));
        }

        [NUnit.Framework.Test]
        [LogMessage(KernelLogMessageConstant.MD5_IS_NOT_FIPS_COMPLIANT, Ignore = true)]
        public virtual void AddMacOnPreserveEncryptionWhileDowngradingTest() {
            String fileName = "addMacOnPreserveEncryptionWhileDowngradingTest.pdf";
            String outputFileName = DESTINATION_FOLDER + fileName;
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName;
            using (PdfDocument pdfDoc = new PdfDocument(new PdfReader(SOURCE_FOLDER + "noMacProtectionDocument.pdf", new 
                ReaderProperties().SetPassword(PASSWORD)), CompareTool.CreateTestPdfWriter(outputFileName, new WriterProperties
                ().SetPdfVersion(PdfVersion.PDF_1_7)), new StampingProperties().PreserveEncryption())) {
                pdfDoc.AddNewPage().AddAnnotation(new PdfTextAnnotation(new Rectangle(100, 100, 100, 100)));
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().EnableEncryptionCompare().CompareByContent(outputFileName, 
                cmpFileName, DESTINATION_FOLDER, "diff", PASSWORD, PASSWORD));
        }

        [NUnit.Framework.Test]
        [LogMessage(VersionConforming.DEPRECATED_AES256_REVISION)]
        [LogMessage(KernelLogMessageConstant.MD5_IS_NOT_FIPS_COMPLIANT, Ignore = true)]
        public virtual void AddMacOnPreserveEncryptionFor17DocTest() {
            // We can't embed MAC into encrypted documents during the conversion from earlier PDF version
            // because their encryption does not support this. So WriterProperties should be used iso preserveEncryption
            String fileName = "addMacOnPreserveEncryptionFor17DocTest.pdf";
            String outputFileName = DESTINATION_FOLDER + fileName;
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName;
            using (PdfDocument pdfDoc = new PdfDocument(new PdfReader(SOURCE_FOLDER + "noMacProtectionDocument_1_7.pdf"
                , new ReaderProperties().SetPassword(PASSWORD)), CompareTool.CreateTestPdfWriter(outputFileName, new WriterProperties
                ().SetPdfVersion(PdfVersion.PDF_2_0)), new StampingProperties().PreserveEncryption())) {
                pdfDoc.AddNewPage().AddAnnotation(new PdfTextAnnotation(new Rectangle(100, 100, 100, 100)));
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().EnableEncryptionCompare().CompareByContent(outputFileName, 
                cmpFileName, DESTINATION_FOLDER, "diff", PASSWORD, PASSWORD));
        }

        [NUnit.Framework.Test]
        [LogMessage(KernelLogMessageConstant.MD5_IS_NOT_FIPS_COMPLIANT, Ignore = true)]
        public virtual void StandaloneMacOldEncryptionAlgorithmTest() {
            String fileName = "standaloneMacOldEncryptionAlgorithmTest.pdf";
            String outputFileName = DESTINATION_FOLDER + fileName;
            MacProperties macProperties = new MacProperties(MacProperties.MacDigestAlgorithm.SHA_256, MacProperties.MacAlgorithm
                .HMAC_WITH_SHA_256, MacProperties.KeyWrappingAlgorithm.AES_256_NO_PADD);
            WriterProperties writerProperties = new WriterProperties().SetPdfVersion(PdfVersion.PDF_2_0).SetStandardEncryption
                (PASSWORD, PASSWORD, 0, EncryptionConstants.ENCRYPTION_AES_128, macProperties);
            String exceptionMessage = NUnit.Framework.Assert.Catch(typeof(PdfException), () => {
                using (PdfDocument pdfDoc = new PdfDocument(CompareTool.CreateTestPdfWriter(outputFileName, writerProperties
                    ))) {
                    pdfDoc.AddNewPage().AddAnnotation(new PdfTextAnnotation(new Rectangle(100, 100, 100, 100)));
                }
            }
            ).Message;
            NUnit.Framework.Assert.AreEqual(KernelExceptionMessageConstant.MAC_FOR_ENCRYPTION_5, exceptionMessage);
        }

        [NUnit.Framework.Test]
        public virtual void StandaloneMacPublicKeyEncryptionTest() {
            try {
                BouncyCastleFactoryCreator.GetFactory().IsEncryptionFeatureSupported(0, true);
            }
            catch (Exception) {
                NUnit.Framework.Assume.That(false);
            }
            NUnit.Framework.Assume.That(!BouncyCastleFactoryCreator.GetFactory().IsInApprovedOnlyMode());
            String fileName = "standaloneMacPublicKeyEncryptionTest.pdf";
            String outputFileName = DESTINATION_FOLDER + fileName;
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName;
            IX509Certificate certificate = CryptoUtil.ReadPublicCertificate(FileUtil.GetInputStreamForFile(CERTS_SRC +
                 "SHA256withRSA.cer"));
            WriterProperties writerProperties = new WriterProperties().SetPdfVersion(PdfVersion.PDF_2_0).SetPublicKeyEncryption
                (new IX509Certificate[] { certificate }, new int[] { -1 }, EncryptionConstants.ENCRYPTION_AES_256, new 
                MacProperties(MacProperties.MacDigestAlgorithm.SHA_256));
            using (PdfDocument pdfDoc = new PdfDocument(CompareTool.CreateTestPdfWriter(outputFileName, writerProperties
                ))) {
                pdfDoc.AddNewPage().AddAnnotation(new PdfTextAnnotation(new Rectangle(100, 100, 100, 100)));
            }
            IPrivateKey privateKey = GetPrivateKey(CERTS_SRC + "SHA256withRSA.key");
            CompareTool compareTool = new CompareTool();
            compareTool.GetCmpReaderProperties().SetPublicKeySecurityParams(certificate, privateKey);
            compareTool.GetOutReaderProperties().SetPublicKeySecurityParams(certificate, privateKey);
            NUnit.Framework.Assert.IsNull(compareTool.CompareByContent(outputFileName, cmpFileName, DESTINATION_FOLDER
                , "diff"));
        }

        [NUnit.Framework.Test]
        [LogMessage(KernelLogMessageConstant.MD5_IS_NOT_FIPS_COMPLIANT, Ignore = true)]
        public virtual void AddMacOnPreservePublicKeyEncryptionTest() {
            // TODO DEVSIX-8635 - Verify MAC permission and embed MAC in stamping mode for public key encryption
            try {
                BouncyCastleFactoryCreator.GetFactory().IsEncryptionFeatureSupported(0, true);
            }
            catch (Exception) {
                NUnit.Framework.Assume.That(false);
            }
            String fileName = "addMacOnPreservePublicKeyEncryptionTest.pdf";
            String outputFileName = DESTINATION_FOLDER + fileName;
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName;
            IX509Certificate certificate = CryptoUtil.ReadPublicCertificate(FileUtil.GetInputStreamForFile(CERTS_SRC +
                 "SHA256withRSA.cer"));
            IPrivateKey privateKey = GetPrivateKey(CERTS_SRC + "SHA256withRSA.key");
            ReaderProperties readerProperties = new ReaderProperties();
            readerProperties.SetPublicKeySecurityParams(certificate, privateKey);
            using (PdfDocument pdfDoc = new PdfDocument(new PdfReader(SOURCE_FOLDER + "noMacProtectionPublicKeyEncryptionDocument.pdf"
                , readerProperties), CompareTool.CreateTestPdfWriter(outputFileName), new StampingProperties().PreserveEncryption
                ())) {
                pdfDoc.AddNewPage().AddAnnotation(new PdfTextAnnotation(new Rectangle(100, 100, 100, 100)));
            }
            CompareTool compareTool = new CompareTool();
            compareTool.GetCmpReaderProperties().SetPublicKeySecurityParams(certificate, privateKey);
            compareTool.GetOutReaderProperties().SetPublicKeySecurityParams(certificate, privateKey);
            NUnit.Framework.Assert.IsNull(compareTool.CompareByContent(outputFileName, cmpFileName, DESTINATION_FOLDER
                , "diff"));
        }

        public static IPrivateKey GetPrivateKey(String keyName) {
            return PemFileHelper.ReadPrivateKeyFromPemFile(FileUtil.GetInputStreamForFile(keyName), "testpassphrase".ToCharArray
                ());
        }
    }
}
