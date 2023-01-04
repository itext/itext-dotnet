/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
Authors: iText Software.

This program is free software; you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License version 3
as published by the Free Software Foundation with the addition of the
following permission added to Section 15 as permitted in Section 7(a):
FOR ANY PART OF THE COVERED WORK IN WHICH THE COPYRIGHT IS OWNED BY
ITEXT GROUP. ITEXT GROUP DISCLAIMS THE WARRANTY OF NON INFRINGEMENT
OF THIRD PARTY RIGHTS

This program is distributed in the hope that it will be useful, but
WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
or FITNESS FOR A PARTICULAR PURPOSE.
See the GNU Affero General Public License for more details.
You should have received a copy of the GNU Affero General Public License
along with this program; if not, see http://www.gnu.org/licenses or write to
the Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor,
Boston, MA, 02110-1301 USA, or download the license from the following URL:
http://itextpdf.com/terms-of-use/

The interactive user interfaces in modified source and object code versions
of this program must display Appropriate Legal Notices, as required under
Section 5 of the GNU Affero General Public License.

In accordance with Section 7(b) of the GNU Affero General Public License,
a covered work must retain the producer line in every PDF that is created
or manipulated using iText.

You can be released from the requirements of the license by purchasing
a commercial license. Buying such a license is mandatory as soon as you
develop commercial activities involving the iText software without
disclosing the source code of your own applications.
These activities include: offering paid services to customers as an ASP,
serving PDFs on the fly in a web application, shipping iText with a closed
source product.

For more information, please contact iText Software Corp. at this
address: sales@itextpdf.com
*/
using System;
using System.IO;
using iText.IO.Font.Constants;
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
    [NUnit.Framework.Category("IntegrationTest")]
    public class LocationTextExtractionStrategyTest : SimpleTextExtractionStrategyTest {
        private static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/layout/LocationTextExtractionStrategyTest/";

        public override ITextExtractionStrategy CreateRenderListenerForTest() {
            return new LocationTextExtractionStrategy();
        }

        [NUnit.Framework.Test]
        public virtual void TestYPosition() {
            PdfDocument doc = CreatePdfWithOverlappingTextVertical(new String[] { "A", "B", "C", "D" }, new String[] { 
                "AA", "BB", "CC", "DD" });
            String text = PdfTextExtractor.GetTextFromPage(doc.GetPage(1), CreateRenderListenerForTest());
            NUnit.Framework.Assert.AreEqual("A\nAA\nB\nBB\nC\nCC\nD\nDD", text);
        }

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
        [NUnit.Framework.Test]
        public virtual void TestRotatedPage() {
            byte[] bytes = CreateSimplePdf(new Rectangle(792, 612), "A\nB\nC\nD");
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(new MemoryStream(bytes)));
            String text = PdfTextExtractor.GetTextFromPage(pdfDocument.GetPage(1), CreateRenderListenerForTest());
            NUnit.Framework.Assert.AreEqual("A \nB \nC \nD", text);
        }

        [NUnit.Framework.Test]
        public virtual void TestRotatedPage2() {
            byte[] bytes = CreateSimplePdf(new Rectangle(792, 612), "A\nB\nC\nD");
            //TestResourceUtils.saveBytesToFile(bytes, new File("C:/temp/out.pdf"));
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(new MemoryStream(bytes)));
            String text = PdfTextExtractor.GetTextFromPage(pdfDocument.GetPage(1), CreateRenderListenerForTest());
            NUnit.Framework.Assert.AreEqual("A \nB \nC \nD", text);
        }

        [NUnit.Framework.Test]
        public virtual void TestRotatedPage3() {
            byte[] bytes = CreateSimplePdf(new Rectangle(792, 612), "A\nB\nC\nD");
            //TestResourceUtils.saveBytesToFile(bytes, new File("C:/temp/out.pdf"));
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(new MemoryStream(bytes)));
            String text = PdfTextExtractor.GetTextFromPage(pdfDocument.GetPage(1), CreateRenderListenerForTest());
            NUnit.Framework.Assert.AreEqual("A \nB \nC \nD", text);
        }

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

        [NUnit.Framework.Test]
        public virtual void TestSuperscript() {
            byte[] content = CreatePdfWithSupescript("Hel", "lo");
            //TestResourceUtils.openBytesAsPdf(content);
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(new MemoryStream(content)));
            String text = PdfTextExtractor.GetTextFromPage(pdfDocument.GetPage(1), CreateRenderListenerForTest());
            NUnit.Framework.Assert.AreEqual("Hello", text);
        }

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

        [NUnit.Framework.Test]
        public virtual void TestFontSpacingEqualsCharSpacing() {
            byte[] content = CreatePdfWithFontSpacingEqualsCharSpacing();
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(new MemoryStream(content)));
            String text = PdfTextExtractor.GetTextFromPage(pdfDocument.GetPage(1), CreateRenderListenerForTest());
            NUnit.Framework.Assert.AreEqual("Preface", text);
        }

        [NUnit.Framework.Test]
        public virtual void TestLittleFontSize() {
            byte[] content = CreatePdfWithLittleFontSize();
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(new MemoryStream(content)));
            String text = PdfTextExtractor.GetTextFromPage(pdfDocument.GetPage(1), CreateRenderListenerForTest());
            NUnit.Framework.Assert.AreEqual("Preface", text);
        }

        [NUnit.Framework.Test]
        public virtual void TestType3FontWithDifferences() {
            String sourcePdf = sourceFolder + "DocumentWithType3FontWithDifferences.pdf";
            String comparedTextFile = sourceFolder + "textFromDocWithType3FontWithDifferences.txt";
            using (PdfDocument pdf = new PdfDocument(new PdfReader(sourcePdf))) {
                LocationTextExtractionStrategy strategy = new LocationTextExtractionStrategy();
                String result = PdfTextExtractor.GetTextFromPage(pdf.GetPage(1), strategy);
                PdfDictionary pdfType3FontDict = (PdfDictionary)pdf.GetPdfObject(292);
                PdfType3Font pdfType3Font = (PdfType3Font)PdfFontFactory.CreateFont(pdfType3FontDict);
                byte[] bytes = File.ReadAllBytes(System.IO.Path.Combine(comparedTextFile));
                NUnit.Framework.Assert.AreEqual(iText.Commons.Utils.JavaUtil.GetStringForBytes(bytes, System.Text.Encoding
                    .UTF8), result);
                NUnit.Framework.Assert.AreEqual(177, pdfType3Font.GetNumberOfGlyphs());
                NUnit.Framework.Assert.AreEqual("gA", pdfType3Font.GetFontEncoding().GetDifference(10));
                NUnit.Framework.Assert.AreEqual(41, pdfType3Font.GetFontProgram().GetGlyphByCode(10).GetUnicode());
                NUnit.Framework.Assert.AreEqual(".notdef", pdfType3Font.GetFontEncoding().GetDifference(210));
                NUnit.Framework.Assert.AreEqual(928, pdfType3Font.GetFontProgram().GetGlyphByCode(210).GetUnicode());
            }
        }

        private byte[] CreatePdfWithNegativeCharSpacing(String str1, float charSpacing, String str2) {
            MemoryStream baos = new MemoryStream();
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(baos).SetCompressionLevel(0));
            PdfCanvas canvas = new PdfCanvas(pdfDocument.AddNewPage());
            canvas.BeginText();
            canvas.SetFontAndSize(PdfFontFactory.CreateFont(StandardFonts.HELVETICA), 12);
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

        private byte[] CreatePdfWithRotatedXObject(String xobjectText) {
            MemoryStream baos = new MemoryStream();
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(baos).SetCompressionLevel(0));
            Document document = new Document(pdfDocument);
            document.Add(new Paragraph("A"));
            document.Add(new Paragraph("B"));
            PdfFormXObject template = new PdfFormXObject(new Rectangle(20, 100));
            PdfCanvas canvas = new PdfCanvas(template, pdfDocument);
            canvas.SetStrokeColor(ColorConstants.GREEN).Rectangle(0, 0, template.GetWidth(), template.GetHeight()).Stroke
                ();
            AffineTransform tx = new AffineTransform();
            tx.Translate(0, template.GetHeight());
            tx.Rotate((float)(-90 / 180f * Math.PI));
            canvas.ConcatMatrix(tx).BeginText().SetFontAndSize(PdfFontFactory.CreateFont(StandardFonts.HELVETICA), 12)
                .MoveText(0, template.GetWidth() - 12).ShowText(xobjectText).EndText();
            document.Add(new Image(template).SetRotationAngle(Math.PI / 2)).Add(new Paragraph("C"));
            document.Close();
            return baos.ToArray();
        }

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

        private byte[] CreatePdfWithSupescript(String regularText, String superscriptText) {
            MemoryStream byteStream = new MemoryStream();
            Document document = new Document(new PdfDocument(new PdfWriter(byteStream)));
            document.Add(new Paragraph(regularText).Add(new iText.Layout.Element.Text(superscriptText).SetTextRise(7))
                );
            document.Close();
            return byteStream.ToArray();
        }

        private byte[] CreatePdfWithFontSpacingEqualsCharSpacing() {
            MemoryStream byteStream = new MemoryStream();
            PdfDocument document = new PdfDocument(new PdfWriter(byteStream));
            PdfPage page1 = document.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page1);
            PdfFont font = PdfFontFactory.CreateFont();
            PdfTextArray textArray = new PdfTextArray();
            textArray.Add(font.ConvertToBytes("P"));
            textArray.Add(-226.2f);
            textArray.Add(font.ConvertToBytes("r"));
            textArray.Add(-231.8f);
            textArray.Add(font.ConvertToBytes("e"));
            textArray.Add(-230.8f);
            textArray.Add(font.ConvertToBytes("f"));
            textArray.Add(-238);
            textArray.Add(font.ConvertToBytes("a"));
            textArray.Add(-238.9f);
            textArray.Add(font.ConvertToBytes("c"));
            textArray.Add(-228.9f);
            textArray.Add(font.ConvertToBytes("e"));
            float charSpace = font.GetWidth(' ', 12);
            canvas.SaveState().BeginText().SetFontAndSize(font, 12).SetCharacterSpacing(-charSpace).ShowText(textArray
                ).EndText().RestoreState();
            canvas.Release();
            document.Close();
            return byteStream.ToArray();
        }

        private byte[] CreatePdfWithLittleFontSize() {
            MemoryStream byteStream = new MemoryStream();
            PdfDocument document = new PdfDocument(new PdfWriter(byteStream));
            PdfPage page1 = document.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page1);
            PdfTextArray textArray = new PdfTextArray();
            PdfFont font = PdfFontFactory.CreateFont();
            textArray.Add(font.ConvertToBytes("P"));
            textArray.Add(1);
            textArray.Add(font.ConvertToBytes("r"));
            textArray.Add(1);
            textArray.Add(font.ConvertToBytes("e"));
            textArray.Add(1);
            textArray.Add(font.ConvertToBytes("f"));
            textArray.Add(1);
            textArray.Add(font.ConvertToBytes("a"));
            textArray.Add(1);
            textArray.Add(font.ConvertToBytes("c"));
            textArray.Add(1);
            textArray.Add(font.ConvertToBytes("e"));
            canvas.SaveState().BeginText().SetFontAndSize(font, 0.2f).ShowText(textArray).EndText().RestoreState();
            canvas.Release();
            document.Close();
            return byteStream.ToArray();
        }
    }
}
