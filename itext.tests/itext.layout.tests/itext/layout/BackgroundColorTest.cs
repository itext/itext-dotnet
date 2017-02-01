using System;
using System.IO;
using iText.Kernel.Colors;
using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using iText.Layout.Element;
using iText.Test;

namespace iText.Layout {
    public class BackgroundColorTest : ExtendedITextTest {
        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/layout/BackgroundColorTest/";

        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/layout/BackgroundColorTest/";

        public const String cmpPrefix = "cmp_";

        internal String fileName;

        internal String outFileName;

        internal String cmpFileName;

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateDestinationFolder(destinationFolder);
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="iText.Kernel.XMP.XMPException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void ShouldAddBackgroundColorAttributeToAccessiblityWhenBackgroundColorIsSet() {
            fileName = "simpleBackgroundColorTest.pdf";
            outFileName = destinationFolder + fileName;
            cmpFileName = sourceFolder + cmpPrefix + fileName;
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(new FileStream(outFileName, FileMode.Create)));
            pdfDocument.SetTagged();
            Document doc = new Document(pdfDocument);
            Text foo = new Text("foo");
            foo.SetBackgroundColor(Color.BLUE);
            doc.Add(new Paragraph(foo));
            CloseDocumentAndCompareOutputs(doc);
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        private void CloseDocumentAndCompareOutputs(Document document) {
            document.Close();
            String compareResult = new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder, "diff"
                );
            if (compareResult != null) {
                NUnit.Framework.Assert.Fail(compareResult);
            }
        }
    }
}
