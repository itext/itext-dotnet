using System;
using System.IO;
using iText.IO.Source;
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
            CreateWrapper("customEncryptedDocument.pdf", "simpleUnencryptedWrapper.pdf", "iText");
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void ExtractCustomEncryptedDocumentTest() {
            ExtractEncrypted("customEncryptedDocument.pdf", "simpleUnencryptedWrapper.pdf", null);
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void CreateWrapperForStandardEncryptedTest() {
            CreateWrapper("standardEncryptedDocument.pdf", "standardUnencryptedWrapper.pdf", "Standard");
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void ExtractStandardEncryptedDocumentTest() {
            ExtractEncrypted("standardEncryptedDocument.pdf", "standardUnencryptedWrapper.pdf", "World".GetBytes(iText.IO.Util.EncodingUtil.ISO_8859_1
                ));
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        private void CreateWrapper(String encryptedName, String wrapperName, String cryptoFilter) {
            String inPath = sourceFolder + "cmp_" + encryptedName;
            String cmpPath = sourceFolder + "cmp_" + wrapperName;
            String outPath = destinationFolder + wrapperName;
            String diff = "diff_" + wrapperName + "_";
            PdfDocument document = new PdfDocument(new PdfWriter(outPath, new WriterProperties().SetPdfVersion(PdfVersion
                .PDF_2_0)));
            PdfFileSpec fs = PdfEncryptedPayloadFileSpecFactory.Create(document, inPath, new PdfEncryptedPayload(cryptoFilter
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
        private void ExtractEncrypted(String encryptedName, String wrapperName, byte[] password) {
            String inPath = sourceFolder + "cmp_" + wrapperName;
            String cmpPath = sourceFolder + "cmp_" + encryptedName;
            String outPath = destinationFolder + encryptedName;
            String diff = "diff_" + encryptedName + "_";
            PdfDocument document = new PdfDocument(new PdfReader(inPath));
            PdfEncryptedPayloadDocument encryptedDocument = document.GetEncryptedPayloadDocument();
            byte[] encryptedDocumentBytes = encryptedDocument.GetDocumentBytes();
            FileStream fos = new FileStream(outPath, FileMode.Create);
            fos.Write(encryptedDocumentBytes);
            fos.Dispose();
            document.Close();
            PdfEncryptedPayload ep = encryptedDocument.GetEncryptedPayload();
            NUnit.Framework.Assert.AreEqual(PdfEncryptedPayloadFileSpecFactory.GenerateFileDisplay(ep), encryptedDocument
                .GetName());
            if (password != null) {
                NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPath, cmpPath, destinationFolder, diff
                    , password, password));
            }
            else {
                RandomAccessFileOrArray raf = new RandomAccessFileOrArray(new RandomAccessSourceFactory().CreateBestSource
                    (cmpPath));
                byte[] cmpBytes = new byte[(int)raf.Length()];
                raf.ReadFully(cmpBytes);
                raf.Close();
                NUnit.Framework.Assert.AreEqual(cmpBytes, encryptedDocumentBytes);
            }
        }
    }
}
