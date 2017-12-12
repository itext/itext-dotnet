using System;
using System.IO;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Filespec;
using iText.Kernel.Utils;
using iText.Test;

namespace iText.Kernel.Crypto {
    public class UnencryptedWrapperTest : ExtendedITextTest {
        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/kernel/crypto/UnencryptedWrapperTest/";

        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/kernel/crypto/UnencryptedWrapperTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateOrClearDestinationFolder(destinationFolder);
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void CreateSimpleWrapperDocumentTest() {
            String inPath = sourceFolder + "cmp_customEncryptedDocument.pdf";
            String cmpPath = sourceFolder + "cmp_simpleUnencryptedWrapper.pdf";
            String outPath = destinationFolder + "simpleUnencryptedWrapper.pdf";
            String diff = "diff_simpleUnencryptedWrapper.pdf_";
            PdfDocument document = new PdfDocument(new PdfWriter(outPath, new WriterProperties().SetPdfVersion(PdfVersion
                .PDF_2_0)));
            PdfFileSpec fs = PdfEncryptedPayloadFileSpecFactory.Create(document, inPath, new PdfEncryptedPayload("iText"
                ));
            document.SetEncryptedPayload(fs);
            PdfFont font = PdfFontFactory.CreateFont();
            PdfCanvas canvas = new PdfCanvas(document.AddNewPage());
            canvas.SaveState().BeginText().MoveText(36, 750).SetFontAndSize(font, 30).ShowText("Hi! I'm wrapper document."
                ).EndText().RestoreState();
            canvas.Release();
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPath, cmpPath, destinationFolder, diff
                ));
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void ExtractCustomEncryptedDocumentTest() {
            String inPath = sourceFolder + "cmp_simpleUnencryptedWrapper.pdf";
            String cmpPath = sourceFolder + "cmp_customEncryptedDocument.pdf";
            String outPath = destinationFolder + "customEncryptedDocument.pdf";
            PdfDocument document = new PdfDocument(new PdfReader(inPath));
            PdfStream stream = document.GetEncryptedPayloadAsStream();
            byte[] encryptedDocumentBytes = stream.GetBytes();
            FileStream fos = new FileStream(outPath, FileMode.Create);
            fos.Write(encryptedDocumentBytes);
            fos.Dispose();
            document.Close();
            //TODO: check files by bytes
            NUnit.Framework.Assert.IsNotNull(encryptedDocumentBytes);
        }
    }
}
