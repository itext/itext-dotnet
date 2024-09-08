using System;
using NUnit.Framework;
using iText.Bouncycastleconnector;
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Bouncycastle.Crypto;
using iText.Commons.Utils;
using iText.Kernel.Crypto;
using iText.Kernel.Exceptions;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Annot;
using iText.Kernel.Utils;
using iText.Test;

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
            NUnit.Framework.Assume.That("BC".Equals(PROVIDER_NAME));
            CreateOrClearDestinationFolder(DESTINATION_FOLDER);
        }

        [NUnit.Framework.OneTimeTearDown]
        public static void AfterClass() {
            CompareTool.Cleanup(DESTINATION_FOLDER);
        }

        [NUnit.Framework.Test]
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
        public virtual void InvalidPublicKeyMacProtectedDocumentTest() {
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
        public virtual void ReadThirdPartyMacProtectedDocumentTest() {
            NUnit.Framework.Assert.DoesNotThrow(() => {
                using (PdfDocument pdfDoc = new PdfDocument(new PdfReader(SOURCE_FOLDER + "thirdPartyMacProtectedDocument.pdf"
                    , new ReaderProperties().SetPassword(PASSWORD)))) {
                }
            }
            );
        }

        [NUnit.Framework.Test]
        public virtual void ReadThirdPartyPublicKeyMacProtectedDocumentTest() {
            IPrivateKey privateKey = MacIntegrityProtectorCreationTest.GetPrivateKey(CERTS_SRC + "keyForEncryption.pem"
                );
            IX509Certificate certificate = CryptoUtil.ReadPublicCertificate(FileUtil.GetInputStreamForFile(CERTS_SRC +
                 "certForEncryption.crt"));
            NUnit.Framework.Assert.DoesNotThrow(() => {
                using (PdfDocument pdfDoc = new PdfDocument(new PdfReader(SOURCE_FOLDER + "thirdPartyPublicKeyMacProtectedDocument.pdf"
                    , new ReaderProperties().SetPublicKeySecurityParams(certificate, privateKey)))) {
                }
            }
            );
        }

        [NUnit.Framework.Test]
        public virtual void ReadMacProtectedPdf1_7() {
            NUnit.Framework.Assert.DoesNotThrow(() => {
                using (PdfDocument pdfDoc = new PdfDocument(new PdfReader(SOURCE_FOLDER + "macProtectedDocumentPdf1_7.pdf"
                    , new ReaderProperties().SetPassword(PASSWORD)))) {
                }
            }
            );
        }
    }
}
