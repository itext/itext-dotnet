using System;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using iText.Test;

namespace iText.Kernel.Pdf.Canvas.Parser {
    public class PdfContentExtractionTest : ExtendedITextTest {
        private static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/kernel/parser/PdfContentExtractionTest/";

        [NUnit.Framework.Test]
        public virtual void ContentExtractionInDocWithBigCoordinatesTest() {
            NUnit.Framework.Assert.That(() =>  {
                //TODO: remove the expected exception construct once the issue is fixed (DEVSIX-1279)
                String inputFileName = sourceFolder + "docWithBigCoordinates.pdf";
                //In this document the CTM shrinks coordinates and this coordinates are large numbers.
                // At the moment creation of this test clipper has a problem with handling large numbers
                // since internally it deals with integers and has to multuply large numbers even more
                // for internal purposes
                PdfDocument pdfDocument = new PdfDocument(new PdfReader(inputFileName));
                PdfDocumentContentParser contentParser = new PdfDocumentContentParser(pdfDocument);
                contentParser.ProcessContent(1, new LocationTextExtractionStrategy());
            }
            , NUnit.Framework.Throws.InstanceOf<InvalidOperationException>())
;
        }
    }
}
