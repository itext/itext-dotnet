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
using NUnit.Framework;
using iText.Bouncycastleconnector;
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Bouncycastle.Crypto;
using iText.Commons.Utils;
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
    public class MacIntegrityProtectorReadingAndRewritingTest : ExtendedITextTest {
        private static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/kernel/mac/MacIntegrityProtectorReadingAndRewritingTest/";

        private static readonly String DESTINATION_FOLDER = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/kernel/mac/MacIntegrityProtectorReadingAndRewritingTest/";

        private static readonly String CERTS_SRC = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/kernel/mac/MacIntegrityProtectorReadingAndRewritingTest/certs/";

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
        [LogMessage(KernelLogMessageConstant.MD5_IS_NOT_FIPS_COMPLIANT, Ignore = true)]
        public virtual void AppendModeTest() {
            String fileName = "appendModeTest.pdf";
            String outputFileName = DESTINATION_FOLDER + fileName;
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName;
            using (PdfDocument pdfDoc = new PdfDocument(new PdfReader(SOURCE_FOLDER + "macProtectedDocument.pdf", new 
                ReaderProperties().SetPassword(PASSWORD)), CompareTool.CreateTestPdfWriter(outputFileName), new StampingProperties
                ().UseAppendMode())) {
                pdfDoc.AddNewPage().AddAnnotation(new PdfTextAnnotation(new Rectangle(100, 100, 100, 100)));
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().EnableEncryptionCompare().CompareByContent(outputFileName, 
                cmpFileName, DESTINATION_FOLDER, "diff", PASSWORD, PASSWORD));
        }

        [NUnit.Framework.Test]
        [LogMessage(KernelLogMessageConstant.MD5_IS_NOT_FIPS_COMPLIANT, Ignore = true)]
        public virtual void PreserveEncryptionTest() {
            String fileName = "preserveEncryptionTest.pdf";
            String outputFileName = DESTINATION_FOLDER + fileName;
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName;
            using (PdfDocument pdfDoc = new PdfDocument(new PdfReader(SOURCE_FOLDER + "macProtectedDocument.pdf", new 
                ReaderProperties().SetPassword(PASSWORD)), CompareTool.CreateTestPdfWriter(outputFileName), new StampingProperties
                ().PreserveEncryption())) {
                pdfDoc.AddNewPage().AddAnnotation(new PdfTextAnnotation(new Rectangle(100, 100, 100, 100)));
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().EnableEncryptionCompare().CompareByContent(outputFileName, 
                cmpFileName, DESTINATION_FOLDER, "diff", PASSWORD, PASSWORD));
        }

        [NUnit.Framework.Test]
        [LogMessage(KernelLogMessageConstant.MD5_IS_NOT_FIPS_COMPLIANT, Ignore = true)]
        public virtual void DisableMacTest() {
            String fileName = "disableMacTest.pdf";
            String outputFileName = DESTINATION_FOLDER + fileName;
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName;
            using (PdfDocument pdfDoc = new PdfDocument(new PdfReader(SOURCE_FOLDER + "macProtectedDocument.pdf", new 
                ReaderProperties().SetPassword(PASSWORD)), CompareTool.CreateTestPdfWriter(outputFileName), new StampingProperties
                ().PreserveEncryption().DisableMac())) {
                pdfDoc.AddNewPage().AddAnnotation(new PdfTextAnnotation(new Rectangle(100, 100, 100, 100)));
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().EnableEncryptionCompare().CompareByContent(outputFileName, 
                cmpFileName, DESTINATION_FOLDER, "diff", PASSWORD, PASSWORD));
        }

        [NUnit.Framework.Test]
        [LogMessage(KernelLogMessageConstant.MD5_IS_NOT_FIPS_COMPLIANT, Ignore = true)]
        public virtual void DisableMacInAppendModeTest() {
            // We do not disable MAC in append mode if it was there
            String fileName = "disableMacInAppendModeTest.pdf";
            String outputFileName = DESTINATION_FOLDER + fileName;
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName;
            using (PdfDocument pdfDoc = new PdfDocument(new PdfReader(SOURCE_FOLDER + "macProtectedDocument.pdf", new 
                ReaderProperties().SetPassword(PASSWORD)), CompareTool.CreateTestPdfWriter(outputFileName), new StampingProperties
                ().UseAppendMode().DisableMac())) {
                pdfDoc.AddNewPage().AddAnnotation(new PdfTextAnnotation(new Rectangle(100, 100, 100, 100)));
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().EnableEncryptionCompare().CompareByContent(outputFileName, 
                cmpFileName, DESTINATION_FOLDER, "diff", PASSWORD, PASSWORD));
        }

        [NUnit.Framework.Test]
        [LogMessage(KernelLogMessageConstant.MD5_IS_NOT_FIPS_COMPLIANT, Ignore = true)]
        public virtual void WriterPropertiesTest() {
            String fileName = "writerPropertiesTest.pdf";
            String outputFileName = DESTINATION_FOLDER + fileName;
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName;
            MacProperties macProperties = new MacProperties(MacProperties.MacDigestAlgorithm.SHA_512);
            WriterProperties writerProperties = new WriterProperties().SetPdfVersion(PdfVersion.PDF_2_0).SetStandardEncryption
                (PASSWORD, PASSWORD, 0, EncryptionConstants.ENCRYPTION_AES_256, macProperties);
            using (PdfDocument pdfDoc = new PdfDocument(new PdfReader(SOURCE_FOLDER + "macProtectedDocument.pdf", new 
                ReaderProperties().SetPassword(PASSWORD)), CompareTool.CreateTestPdfWriter(outputFileName, writerProperties
                ))) {
                pdfDoc.AddNewPage().AddAnnotation(new PdfTextAnnotation(new Rectangle(100, 100, 100, 100)));
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outputFileName, cmpFileName, DESTINATION_FOLDER
                , "diff", PASSWORD, PASSWORD));
        }

        [NUnit.Framework.Test]
        [LogMessage(KernelLogMessageConstant.MD5_IS_NOT_FIPS_COMPLIANT, Ignore = true)]
        public virtual void MacShouldNotBePreservedWithEncryptionTest() {
            String fileName = "macShouldNotBePreservedWithEncryptionTest.pdf";
            String outputFileName = DESTINATION_FOLDER + fileName;
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName;
            WriterProperties writerProperties = new WriterProperties().SetPdfVersion(PdfVersion.PDF_1_7).SetStandardEncryption
                (PASSWORD, PASSWORD, 0, EncryptionConstants.ENCRYPTION_AES_128);
            using (PdfDocument pdfDoc = new PdfDocument(new PdfReader(SOURCE_FOLDER + "macProtectedDocument.pdf", new 
                ReaderProperties().SetPassword(PASSWORD)), CompareTool.CreateTestPdfWriter(outputFileName, writerProperties
                ))) {
                pdfDoc.AddNewPage().AddAnnotation(new PdfTextAnnotation(new Rectangle(100, 100, 100, 100)));
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().EnableEncryptionCompare().CompareByContent(outputFileName, 
                cmpFileName, DESTINATION_FOLDER, "diff", PASSWORD, PASSWORD));
        }

        [NUnit.Framework.Test]
        [LogMessage(KernelLogMessageConstant.MD5_IS_NOT_FIPS_COMPLIANT, Ignore = true)]
        public virtual void MacShouldNotBePreservedTest() {
            String fileName = "macShouldNotBePreservedTest.pdf";
            String outputFileName = DESTINATION_FOLDER + fileName;
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName;
            using (PdfDocument pdfDoc = new PdfDocument(new PdfReader(SOURCE_FOLDER + "macProtectedDocument.pdf", new 
                ReaderProperties().SetPassword(PASSWORD)), CompareTool.CreateTestPdfWriter(outputFileName))) {
                pdfDoc.AddNewPage().AddAnnotation(new PdfTextAnnotation(new Rectangle(100, 100, 100, 100)));
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outputFileName, cmpFileName, DESTINATION_FOLDER
                , "diff", PASSWORD, PASSWORD));
        }

        [NUnit.Framework.Test]
        [LogMessage(KernelLogMessageConstant.MD5_IS_NOT_FIPS_COMPLIANT, Ignore = true)]
        public virtual void InvalidMacTokenTest() {
            String fileName = "invalidMacTokenTest.pdf";
            String outputFileName = DESTINATION_FOLDER + fileName;
            String exceptionMessage = NUnit.Framework.Assert.Catch(typeof(PdfException), () => {
                using (PdfDocument pdfDoc = new PdfDocument(new PdfReader(SOURCE_FOLDER + "invalidMacProtectedDocument.pdf"
                    , new ReaderProperties().SetPassword(PASSWORD)), CompareTool.CreateTestPdfWriter(outputFileName))) {
                    pdfDoc.AddNewPage().AddAnnotation(new PdfTextAnnotation(new Rectangle(100, 100, 100, 100)));
                }
            }
            ).Message;
            NUnit.Framework.Assert.AreEqual(KernelExceptionMessageConstant.MAC_VALIDATION_FAILED, exceptionMessage);
        }

        [NUnit.Framework.Test]
        [LogMessage(KernelLogMessageConstant.MD5_IS_NOT_FIPS_COMPLIANT, Ignore = true)]
        public virtual void InvalidPublicKeyMacProtectedDocumentTest() {
            try {
                BouncyCastleFactoryCreator.GetFactory().IsEncryptionFeatureSupported(0, true);
            }
            catch (Exception) {
                NUnit.Framework.Assume.That(false);
            }
            String fileName = "invalidPublicKeyMacProtectedDocumentTest.pdf";
            String outputFileName = DESTINATION_FOLDER + fileName;
            IX509Certificate certificate = CryptoUtil.ReadPublicCertificate(FileUtil.GetInputStreamForFile(CERTS_SRC +
                 "SHA256withRSA.cer"));
            IPrivateKey privateKey = MacIntegrityProtectorCreationTest.GetPrivateKey(CERTS_SRC + "SHA256withRSA.key");
            String exceptionMessage = NUnit.Framework.Assert.Catch(typeof(PdfException), () => {
                using (PdfDocument pdfDoc = new PdfDocument(new PdfReader(SOURCE_FOLDER + "invalidPublicKeyMacProtectedDocument.pdf"
                    , new ReaderProperties().SetPublicKeySecurityParams(certificate, privateKey)), CompareTool.CreateTestPdfWriter
                    (outputFileName))) {
                    pdfDoc.AddNewPage().AddAnnotation(new PdfTextAnnotation(new Rectangle(100, 100, 100, 100)));
                }
            }
            ).Message;
            NUnit.Framework.Assert.AreEqual(KernelExceptionMessageConstant.MAC_VALIDATION_FAILED, exceptionMessage);
        }

        [NUnit.Framework.Test]
        [LogMessage(KernelLogMessageConstant.MD5_IS_NOT_FIPS_COMPLIANT, Ignore = true)]
        public virtual void ReadSignedMacProtectedDocumentWithoutAttributeTest() {
            String message = NUnit.Framework.Assert.Catch(typeof(PdfException), () => {
                using (PdfDocument ignored = new PdfDocument(new PdfReader(SOURCE_FOLDER + "signedMacProtectedDocWithoutAttribute.pdf"
                    , new ReaderProperties().SetPassword(PASSWORD)))) {
                }
            }
            ).Message;
            NUnit.Framework.Assert.AreEqual(KernelExceptionMessageConstant.MAC_ATTRIBUTE_NOT_SPECIFIED, message);
        }

        [NUnit.Framework.Test]
        [LogMessage(KernelLogMessageConstant.MD5_IS_NOT_FIPS_COMPLIANT, Ignore = true)]
        public virtual void MacProtectionStrippedTest() {
            String message = NUnit.Framework.Assert.Catch(typeof(PdfException), () => {
                using (PdfDocument ignored = new PdfDocument(new PdfReader(SOURCE_FOLDER + "macProtectionStrippedTest.pdf"
                    , new ReaderProperties().SetPassword(PASSWORD)))) {
                }
            }
            ).Message;
            NUnit.Framework.Assert.AreEqual(KernelExceptionMessageConstant.MAC_PERMS_WITHOUT_MAC, message);
        }

        [NUnit.Framework.Test]
        [LogMessage(KernelLogMessageConstant.MD5_IS_NOT_FIPS_COMPLIANT, Ignore = true)]
        public virtual void ReadSignedMacProtectedDocumentTest() {
            NUnit.Framework.Assert.DoesNotThrow(() => {
                using (PdfDocument ignored = new PdfDocument(new PdfReader(SOURCE_FOLDER + "signedMacProtectedDocument.pdf"
                    , new ReaderProperties().SetPassword(PASSWORD)))) {
                }
            }
            );
        }

        [NUnit.Framework.Test]
        [LogMessage(KernelLogMessageConstant.MD5_IS_NOT_FIPS_COMPLIANT, Ignore = true)]
        public virtual void ReadThirdPartyMacProtectedDocumentTest() {
            NUnit.Framework.Assert.DoesNotThrow(() => {
                using (PdfDocument ignored = new PdfDocument(new PdfReader(SOURCE_FOLDER + "thirdPartyMacProtectedDocument.pdf"
                    , new ReaderProperties().SetPassword(PASSWORD)))) {
                }
            }
            );
        }

        [NUnit.Framework.Test]
        [LogMessage(KernelLogMessageConstant.MD5_IS_NOT_FIPS_COMPLIANT, Ignore = true)]
        public virtual void ReadThirdPartyPublicKeyMacProtectedDocumentTest() {
            try {
                BouncyCastleFactoryCreator.GetFactory().IsEncryptionFeatureSupported(0, true);
            }
            catch (Exception) {
                NUnit.Framework.Assume.That(false);
            }
            IPrivateKey privateKey = MacIntegrityProtectorCreationTest.GetPrivateKey(CERTS_SRC + "keyForEncryption.pem"
                );
            IX509Certificate certificate = CryptoUtil.ReadPublicCertificate(FileUtil.GetInputStreamForFile(CERTS_SRC +
                 "certForEncryption.crt"));
            NUnit.Framework.Assert.DoesNotThrow(() => {
                using (PdfDocument ignored = new PdfDocument(new PdfReader(SOURCE_FOLDER + "thirdPartyPublicKeyMacProtectedDocument.pdf"
                    , new ReaderProperties().SetPublicKeySecurityParams(certificate, privateKey)))) {
                }
            }
            );
        }

        [NUnit.Framework.Test]
        [LogMessage(KernelLogMessageConstant.MD5_IS_NOT_FIPS_COMPLIANT, Ignore = true)]
        public virtual void ReadMacProtectedPdf1_7() {
            NUnit.Framework.Assert.DoesNotThrow(() => {
                using (PdfDocument ignored = new PdfDocument(new PdfReader(SOURCE_FOLDER + "macProtectedDocumentPdf1_7.pdf"
                    , new ReaderProperties().SetPassword(PASSWORD)))) {
                }
            }
            );
        }

        [NUnit.Framework.Test]
        [LogMessage(KernelLogMessageConstant.MD5_IS_NOT_FIPS_COMPLIANT, Ignore = true)]
        public virtual void NoSaltTest() {
            String message = NUnit.Framework.Assert.Catch(typeof(PdfException), () => {
                using (PdfDocument pdfDoc = new PdfDocument(new PdfReader(SOURCE_FOLDER + "noSaltTest.pdf", new ReaderProperties
                    ().SetPassword(PASSWORD)))) {
                }
            }
            ).Message;
            NUnit.Framework.Assert.AreEqual(KernelExceptionMessageConstant.MAC_VALIDATION_NO_SALT, message);
        }

        [NUnit.Framework.Test]
        [LogMessage(KernelLogMessageConstant.MD5_IS_NOT_FIPS_COMPLIANT, Ignore = true)]
        public virtual void ReadTamperedMacProtectedDocumentTest() {
            String message = NUnit.Framework.Assert.Catch(typeof(PdfException), () => {
                using (PdfDocument ignored = new PdfDocument(new PdfReader(SOURCE_FOLDER + "thirdPartyMacProtectedDocumentTampered.pdf"
                    , new ReaderProperties().SetPassword(PASSWORD)))) {
                }
            }
            ).Message;
            NUnit.Framework.Assert.AreEqual(KernelExceptionMessageConstant.MAC_VALIDATION_FAILED, message);
        }

        [NUnit.Framework.Test]
        [LogMessage(KernelLogMessageConstant.MD5_IS_NOT_FIPS_COMPLIANT, Ignore = true)]
        public virtual void DoNotThrowOnValidationTest1() {
            String fileName = "doNotThrowOnValidationTest1.pdf";
            String outputFileName = DESTINATION_FOLDER + fileName;
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName;
            StampingProperties stampingProperties = new StampingProperties();
            stampingProperties.RegisterDependency(typeof(IMacContainerLocator), new _StandaloneMacContainerLocator_360
                ());
            // do nothing
            using (PdfDocument pdfDoc = new PdfDocument(new PdfReader(SOURCE_FOLDER + "macProtectionStrippedTest.pdf", 
                new ReaderProperties().SetPassword(PASSWORD)), CompareTool.CreateTestPdfWriter(outputFileName), stampingProperties
                )) {
            }
            new CompareTool().CompareByContent(outputFileName, cmpFileName, DESTINATION_FOLDER, "diff");
        }

        private sealed class _StandaloneMacContainerLocator_360 : StandaloneMacContainerLocator {
            public _StandaloneMacContainerLocator_360() {
            }

            public override void HandleMacValidationError(MacValidationException exception) {
            }
        }

        [NUnit.Framework.Test]
        [LogMessage(KernelLogMessageConstant.MD5_IS_NOT_FIPS_COMPLIANT, Ignore = true)]
        public virtual void DoNotThrowOnValidationTest2() {
            String fileName = "doNotThrowOnValidationTest2.pdf";
            String outputFileName = DESTINATION_FOLDER + fileName;
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName;
            StampingProperties stampingProperties = new StampingProperties();
            stampingProperties.RegisterDependency(typeof(IMacContainerLocator), new _StandaloneMacContainerLocator_384
                ());
            // do nothing
            using (PdfDocument pdfDoc = new PdfDocument(new PdfReader(SOURCE_FOLDER + "thirdPartyMacProtectedDocumentTampered.pdf"
                , new ReaderProperties().SetPassword(PASSWORD)), CompareTool.CreateTestPdfWriter(outputFileName), stampingProperties
                )) {
            }
            new CompareTool().CompareByContent(outputFileName, cmpFileName, DESTINATION_FOLDER, "diff");
        }

        private sealed class _StandaloneMacContainerLocator_384 : StandaloneMacContainerLocator {
            public _StandaloneMacContainerLocator_384() {
            }

            public override void HandleMacValidationError(MacValidationException exception) {
            }
        }
    }
}
