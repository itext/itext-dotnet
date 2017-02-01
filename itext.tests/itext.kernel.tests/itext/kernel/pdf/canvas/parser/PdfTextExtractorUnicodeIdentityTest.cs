using System;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser.Filter;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using iText.Test;

namespace iText.Kernel.Pdf.Canvas.Parser {
    public class PdfTextExtractorUnicodeIdentityTest : ExtendedITextTest {
        private static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/kernel/parser/PdfTextExtractorUnicodeIdentityTest/";

        /// <exception cref="System.IO.IOException"/>
        [NUnit.Framework.Test]
        public virtual void Test() {
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(sourceFolder + "user10.pdf"));
            Rectangle rectangle = new Rectangle(71, 708, 154, 9);
            IEventFilter filter = new TextRegionEventFilter(rectangle);
            String txt = PdfTextExtractor.GetTextFromPage(pdfDocument.GetPage(1), new FilteredTextEventListener(new LocationTextExtractionStrategy
                (), filter));
            NUnit.Framework.Assert.AreEqual("Pname Dname Email Address", txt);
        }
    }
}
