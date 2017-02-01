using System;
using System.IO;
using iText.IO.Font;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using iText.Kernel.Pdf.Xobject;
using iText.Layout.Element;
using iText.Layout.Properties;

namespace iText.Layout {
    public class LocationTextExtractionStrategyTest : SimpleTextExtractionStrategyTest {
        private static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/layout/LocationTextExtractionStrategyTest/";

        public override ITextExtractionStrategy CreateRenderListenerForTest() {
            return new LocationTextExtractionStrategy();
        }

        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void TestYPosition() {
            PdfDocument doc = CreatePdfWithOverlappingTextVertical(new String[] { "A", "B", "C", "D" }, new String[] { 
                "AA", "BB", "CC", "DD" });
            String text = PdfTextExtractor.GetTextFromPage(doc.GetPage(1), CreateRenderListenerForTest());
            NUnit.Framework.Assert.AreEqual("A\nAA\nB\nBB\nC\nCC\nD\nDD", text);
        }

        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void TestXPosition() {
            byte[] content = CreatePdfWithOverlappingTextHorizontal(new String[] { "A", "B", "C", "D" }, new String[] 
                { "AA", "BB", "CC", "DD" });
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(new MemoryStream(content)));
            //TestResourceUtils.openBytesAsPdf(content);
            String text = PdfTextExtractor.GetTextFromPage(pdfDocument.GetPage(1), CreateRenderListenerForTest());
            NUnit.Framework.Assert.AreEqual("A AA B BB C CC D DD", text);
        }

        //        Assert.assertEquals("A\tAA\tB\tBB\tC\tCC\tD\tDD", text);
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void TestRotatedPage() {
            byte[] bytes = CreateSimplePdf(new Rectangle(792, 612), "A\nB\nC\nD");
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(new MemoryStream(bytes)));
            String text = PdfTextExtractor.GetTextFromPage(pdfDocument.GetPage(1), CreateRenderListenerForTest());
            NUnit.Framework.Assert.AreEqual("A\nB\nC\nD", text);
        }

        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void TestRotatedPage2() {
            byte[] bytes = CreateSimplePdf(new Rectangle(792, 612), "A\nB\nC\nD");
            //TestResourceUtils.saveBytesToFile(bytes, new File("C:/temp/out.pdf"));
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(new MemoryStream(bytes)));
            String text = PdfTextExtractor.GetTextFromPage(pdfDocument.GetPage(1), CreateRenderListenerForTest());
            NUnit.Framework.Assert.AreEqual("A\nB\nC\nD", text);
        }

        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void TestRotatedPage3() {
            byte[] bytes = CreateSimplePdf(new Rectangle(792, 612), "A\nB\nC\nD");
            //TestResourceUtils.saveBytesToFile(bytes, new File("C:/temp/out.pdf"));
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(new MemoryStream(bytes)));
            String text = PdfTextExtractor.GetTextFromPage(pdfDocument.GetPage(1), CreateRenderListenerForTest());
            NUnit.Framework.Assert.AreEqual("A\nB\nC\nD", text);
        }

        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void TestExtractXObjectTextWithRotation() {
            //LocationAwareTextExtractingPdfContentRenderListener.DUMP_STATE = true;
            String text1 = "X";
            byte[] content = CreatePdfWithRotatedXObject(text1);
            //TestResourceUtils.saveBytesToFile(content, new File("C:/temp/out.pdf"));
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(new MemoryStream(content)));
            String text = PdfTextExtractor.GetTextFromPage(pdfDocument.GetPage(1), CreateRenderListenerForTest());
            NUnit.Framework.Assert.AreEqual("A\nB\nX\nC", text);
        }

        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void TestNegativeCharacterSpacing() {
            byte[] content = CreatePdfWithNegativeCharSpacing("W", 200, "A");
            //TestResourceUtils.openBytesAsPdf(content);
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(new MemoryStream(content)));
            String text = PdfTextExtractor.GetTextFromPage(pdfDocument.GetPage(1), CreateRenderListenerForTest());
            NUnit.Framework.Assert.AreEqual("WA", text);
        }

        [NUnit.Framework.Test]
        public virtual void TestSanityCheckOnVectorMath() {
            Vector start = new Vector(0, 0, 1);
            Vector end = new Vector(1, 0, 1);
            Vector antiparallelStart = new Vector(0.9f, 0, 1);
            Vector parallelStart = new Vector(1.1f, 0, 1);
            float rsltAntiParallel = antiparallelStart.Subtract(end).Dot(end.Subtract(start).Normalize());
            NUnit.Framework.Assert.AreEqual(-0.1f, rsltAntiParallel, 0.0001);
            float rsltParallel = parallelStart.Subtract(end).Dot(end.Subtract(start).Normalize());
            NUnit.Framework.Assert.AreEqual(0.1f, rsltParallel, 0.0001);
        }

        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void TestSuperscript() {
            byte[] content = CreatePdfWithSupescript("Hel", "lo");
            //TestResourceUtils.openBytesAsPdf(content);
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(new MemoryStream(content)));
            String text = PdfTextExtractor.GetTextFromPage(pdfDocument.GetPage(1), CreateRenderListenerForTest());
            NUnit.Framework.Assert.AreEqual("Hello", text);
        }

        /// <exception cref="System.IO.IOException"/>
        [NUnit.Framework.Test]
        public virtual void Test01() {
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(sourceFolder + "test01.pdf"));
            String text = PdfTextExtractor.GetTextFromPage(pdfDocument.GetPage(1), new LocationTextExtractionStrategy(
                ));
            pdfDocument.Close();
            String expectedText = "        We asked each candidate company to distribute to 225 \n" + "randomly selected employees the Great Place to Work \n"
                 + "Trust Index. This employee survey was designed by the \n" + "Great Place to Work Institute of San Francisco to evaluate \n"
                 + "trust in management, pride in work/company, and \n" + "camaraderie. Responses were returned directly to us. ";
            NUnit.Framework.Assert.AreEqual(expectedText, text);
        }

        /// <exception cref="System.Exception"/>
        private byte[] CreatePdfWithNegativeCharSpacing(String str1, float charSpacing, String str2) {
            MemoryStream baos = new MemoryStream();
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(baos).SetCompressionLevel(0));
            PdfCanvas canvas = new PdfCanvas(pdfDocument.AddNewPage());
            canvas.BeginText();
            canvas.SetFontAndSize(PdfFontFactory.CreateFont(FontConstants.HELVETICA), 12);
            canvas.MoveText(45, pdfDocument.GetDefaultPageSize().GetHeight() - 45);
            PdfTextArray ta = new PdfTextArray();
            ta.Add(new PdfString(str1));
            ta.Add(charSpacing);
            ta.Add(new PdfString(str2));
            canvas.ShowText(ta);
            canvas.EndText();
            pdfDocument.Close();
            return baos.ToArray();
        }

        /// <exception cref="System.Exception"/>
        private byte[] CreatePdfWithRotatedXObject(String xobjectText) {
            MemoryStream baos = new MemoryStream();
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(baos).SetCompressionLevel(0));
            Document document = new Document(pdfDocument);
            document.Add(new Paragraph("A"));
            document.Add(new Paragraph("B"));
            PdfFormXObject template = new PdfFormXObject(new Rectangle(20, 100));
            PdfCanvas canvas = new PdfCanvas(template, pdfDocument);
            canvas.SetStrokeColor(Color.GREEN).Rectangle(0, 0, template.GetWidth(), template.GetHeight()).Stroke();
            AffineTransform tx = new AffineTransform();
            tx.Translate(0, template.GetHeight());
            tx.Rotate((float)(-90 / 180f * Math.PI));
            canvas.ConcatMatrix(tx).BeginText().SetFontAndSize(PdfFontFactory.CreateFont(FontConstants.HELVETICA), 12)
                .MoveText(0, template.GetWidth() - 12).ShowText(xobjectText).EndText();
            document.Add(new Image(template).SetRotationAngle(Math.PI / 2)).Add(new Paragraph("C"));
            document.Close();
            return baos.ToArray();
        }

        /// <exception cref="System.Exception"/>
        private byte[] CreateSimplePdf(Rectangle pageSize, params String[] text) {
            MemoryStream byteStream = new MemoryStream();
            Document document = new Document(new PdfDocument(new PdfWriter(byteStream)), new PageSize(pageSize));
            foreach (String @string in text) {
                document.Add(new Paragraph(@string));
                document.Add(new AreaBreak());
            }
            document.Close();
            return byteStream.ToArray();
        }

        /// <exception cref="System.Exception"/>
        protected internal virtual byte[] CreatePdfWithOverlappingTextHorizontal(String[] text1, String[] text2) {
            MemoryStream baos = new MemoryStream();
            Document doc = new Document(new PdfDocument(new PdfWriter(baos).SetCompressionLevel(0)));
            float ystart = 500;
            float xstart = 50;
            float x = xstart;
            float y = ystart;
            foreach (String text in text1) {
                doc.ShowTextAligned(text, x, y, TextAlignment.LEFT);
                x += 70.0f;
            }
            x = xstart + 12;
            y = ystart;
            foreach (String text in text2) {
                doc.ShowTextAligned(text, x, y, TextAlignment.LEFT);
                x += 70.0f;
            }
            doc.Close();
            return baos.ToArray();
        }

        /// <exception cref="System.Exception"/>
        private PdfDocument CreatePdfWithOverlappingTextVertical(String[] text1, String[] text2) {
            MemoryStream baos = new MemoryStream();
            Document doc = new Document(new PdfDocument(new PdfWriter(baos).SetCompressionLevel(0)));
            float ystart = 500;
            float x = 50;
            float y = ystart;
            foreach (String text in text1) {
                doc.ShowTextAligned(text, x, y, TextAlignment.LEFT);
                y -= 25.0f;
            }
            y = ystart - 13;
            foreach (String text in text2) {
                doc.ShowTextAligned(text, x, y, TextAlignment.LEFT);
                y -= 25.0f;
            }
            doc.Close();
            return new PdfDocument(new PdfReader(new MemoryStream(baos.ToArray())));
        }

        /// <exception cref="System.Exception"/>
        private byte[] CreatePdfWithSupescript(String regularText, String superscriptText) {
            MemoryStream byteStream = new MemoryStream();
            Document document = new Document(new PdfDocument(new PdfWriter(byteStream)));
            document.Add(new Paragraph(regularText).Add(new Text(superscriptText).SetTextRise(7)));
            document.Close();
            return byteStream.ToArray();
        }
    }
}
