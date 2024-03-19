using System;
using iText.IO.Font;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Filespec;
using iText.Kernel.Pdf.Tagging;
using iText.Kernel.Pdf.Tagutils;
using iText.Pdfua;
using iText.Pdfua.Exceptions;
using iText.Test;

namespace iText.Pdfua.Checkers {
    [NUnit.Framework.Category("IntegrationTest")]
    public class PdfUAEmbeddedFilesCheckTest : ExtendedITextTest {
        private static readonly String DESTINATION_FOLDER = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/pdfua/PdfUAFormulaTest/";

        private static readonly String FONT = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/pdfua/font/FreeSans.ttf";

        private UaValidationTestFramework framework;

        [NUnit.Framework.OneTimeSetUp]
        public static void Before() {
            CreateOrClearDestinationFolder(DESTINATION_FOLDER);
        }

        [NUnit.Framework.SetUp]
        public virtual void InitializeFramework() {
            framework = new UaValidationTestFramework(DESTINATION_FOLDER);
        }

        [NUnit.Framework.Test]
        public virtual void PdfuaWithEmbeddedFilesWithoutFTest() {
            framework.AddBeforeGenerationHook((pdfDocument) => {
                PdfFileSpec fs = PdfFileSpec.CreateEmbeddedFileSpec(pdfDocument, "file".GetBytes(), "description", "file.txt"
                    , null, null, null);
                PdfDictionary fsDict = (PdfDictionary)fs.GetPdfObject();
                fsDict.Remove(PdfName.F);
                pdfDocument.AddFileAttachment("file.txt", fs);
            }
            );
            framework.AssertBothFail("pdfuaWithEmbeddedFilesWithoutF", PdfUAExceptionMessageConstants.FILE_SPECIFICATION_DICTIONARY_SHALL_CONTAIN_F_KEY_AND_UF_KEY
                );
        }

        [NUnit.Framework.Test]
        public virtual void PdfuaWithEmbeddedFilesWithoutUFTest() {
            framework.AddBeforeGenerationHook((pdfDocument) => {
                pdfDocument.AddNewPage();
                PdfFileSpec fs = PdfFileSpec.CreateEmbeddedFileSpec(pdfDocument, "file".GetBytes(), "description", "file.txt"
                    , null, null, null);
                PdfDictionary fsDict = (PdfDictionary)fs.GetPdfObject();
                fsDict.Remove(PdfName.UF);
                pdfDocument.AddFileAttachment("file.txt", fs);
            }
            );
            framework.AssertBothFail("pdfuaWithEmbeddedFilesWithoutUF", PdfUAExceptionMessageConstants.FILE_SPECIFICATION_DICTIONARY_SHALL_CONTAIN_F_KEY_AND_UF_KEY
                );
        }

        [NUnit.Framework.Test]
        public virtual void PdfuaWithValidEmbeddedFileTest() {
            framework.AddBeforeGenerationHook(((pdfDocument) => {
                PdfFont font;
                try {
                    font = PdfFontFactory.CreateFont(FONT, PdfEncodings.WINANSI, PdfFontFactory.EmbeddingStrategy.FORCE_EMBEDDED
                        );
                }
                catch (System.IO.IOException) {
                    //rethrow as unchecked to fail the test
                    throw new Exception();
                }
                PdfPage page1 = pdfDocument.AddNewPage();
                PdfCanvas canvas = new PdfCanvas(page1);
                TagTreePointer tagPointer = new TagTreePointer(pdfDocument).SetPageForTagging(page1).AddTag(StandardRoles.
                    P);
                canvas.OpenTag(tagPointer.GetTagReference()).SaveState().BeginText().SetFontAndSize(font, 12).MoveText(100
                    , 100).ShowText("Test text.").EndText().RestoreState().CloseTag();
                byte[] somePdf = new byte[35];
                pdfDocument.AddAssociatedFile("some test pdf file", PdfFileSpec.CreateEmbeddedFileSpec(pdfDocument, somePdf
                    , "some test pdf file", "foo.pdf", PdfName.ApplicationPdf, null, new PdfName("Data")));
            }
            ));
            framework.AssertBothValid("pdfuaWithValidEmbeddedFile");
        }
    }
}
