using System;
using System.Collections.Generic;
using iText.IO.Source;
using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using iText.Test;

namespace iText.Kernel.Pdf.Canvas.Parser.Util {
    public class PdfCanvasParserTest : ExtendedITextTest {
        private static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/kernel/parser/PdfCanvasParserTest/";

        [NUnit.Framework.Test]
        public virtual void InnerArraysInContentStreamTest() {
            String inputFileName = sourceFolder + "innerArraysInContentStream.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(inputFileName));
            byte[] docInBytes = pdfDocument.GetFirstPage().GetContentBytes();
            RandomAccessSourceFactory factory = new RandomAccessSourceFactory();
            PdfTokenizer tokeniser = new PdfTokenizer(new RandomAccessFileOrArray(factory.CreateSource(docInBytes)));
            PdfResources resources = pdfDocument.GetPage(1).GetResources();
            PdfCanvasParser ps = new PdfCanvasParser(tokeniser, resources);
            IList<PdfObject> actual = ps.Parse(null);
            IList<PdfObject> expected = new List<PdfObject>();
            expected.Add(new PdfString("Cyan"));
            expected.Add(new PdfArray(new int[] { 1, 0, 0, 0 }));
            expected.Add(new PdfString("Magenta"));
            expected.Add(new PdfArray(new int[] { 0, 1, 0, 0 }));
            expected.Add(new PdfString("Yellow"));
            expected.Add(new PdfArray(new int[] { 0, 0, 1, 0 }));
            PdfArray cmpArray = new PdfArray(expected);
            NUnit.Framework.Assert.IsTrue(new CompareTool().CompareArrays(cmpArray, (((PdfDictionary)actual[1]).GetAsArray
                (new PdfName("ColorantsDef")))));
        }
    }
}
