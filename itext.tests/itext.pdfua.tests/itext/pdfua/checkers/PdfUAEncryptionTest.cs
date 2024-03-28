using System;
using iText.IO.Font;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Tagging;
using iText.Kernel.Utils;
using iText.Pdfua;
using iText.Pdfua.Exceptions;
using iText.Test;

namespace iText.Pdfua.Checkers {
    [NUnit.Framework.Category("IntegrationTest")]
    public class PdfUAEncryptionTest : ExtendedITextTest {
        private static readonly String DESTINATION_FOLDER = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/pdfua/PdfUAEncryptionTest/";

        private static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/pdfua/PdfUAEncryptionTest/";

        private static readonly String FONT = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/pdfua/font/FreeSans.ttf";

        private static readonly byte[] USER_PASSWORD = "user".GetBytes(System.Text.Encoding.UTF8);

        private static readonly byte[] OWNER_PASSWORD = "owner".GetBytes(System.Text.Encoding.UTF8);

        [NUnit.Framework.OneTimeSetUp]
        public static void Before() {
            CreateOrClearDestinationFolder(DESTINATION_FOLDER);
        }

        [NUnit.Framework.Test]
        public virtual void EncryptWithPassword() {
            String outPdf = DESTINATION_FOLDER + "encryptWithPassword.pdf";
            WriterProperties writerProperties = PdfUATestPdfDocument.CreateWriterProperties().SetStandardEncryption(USER_PASSWORD
                , OWNER_PASSWORD, -1, 3);
            using (PdfWriter writer = new PdfWriter(outPdf, writerProperties)) {
                using (PdfUATestPdfDocument document = new PdfUATestPdfDocument(writer)) {
                    WriteTextToDocument(document);
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, SOURCE_FOLDER + "cmp_" + "encryptWithPassword.pdf"
                , DESTINATION_FOLDER, "diff", USER_PASSWORD, USER_PASSWORD));
        }

        [NUnit.Framework.Test]
        public virtual void EncryptWithPasswordWithInvalidPermissionsTest() {
            String outPdf = DESTINATION_FOLDER + "encryptWithPassword2.pdf";
            WriterProperties writerProperties = PdfUATestPdfDocument.CreateWriterProperties().SetStandardEncryption(USER_PASSWORD
                , OWNER_PASSWORD, 0, 3);
            PdfUATestPdfDocument document = new PdfUATestPdfDocument(new PdfWriter(outPdf, writerProperties));
            WriteTextToDocument(document);
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfUAConformanceException), () => document.Close());
            NUnit.Framework.Assert.AreEqual(PdfUAExceptionMessageConstants.TENTH_BIT_OF_P_VALUE_IN_ENCRYPTION_SHOULD_BE_NON_ZERO
                , e.Message);
        }

        private void WriteTextToDocument(PdfDocument document) {
            PdfPage page = document.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page);
            PdfFont font = PdfFontFactory.CreateFont(FONT, PdfEncodings.WINANSI, PdfFontFactory.EmbeddingStrategy.PREFER_EMBEDDED
                );
            PdfStructElem doc = document.GetStructTreeRoot().AddKid(new PdfStructElem(document, PdfName.Document));
            PdfStructElem paragraph = doc.AddKid(new PdfStructElem(document, PdfName.P, page));
            PdfMcr mcr = paragraph.AddKid(new PdfMcrNumber(page, paragraph));
            canvas.OpenTag(new CanvasTag(mcr)).SaveState().BeginText().SetFontAndSize(font, 12).MoveText(200, 200).ShowText
                ("Hello World!").EndText().RestoreState().CloseTag();
        }
    }
}
