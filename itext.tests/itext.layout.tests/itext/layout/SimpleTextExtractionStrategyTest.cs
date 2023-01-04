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
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using iText.Kernel.Pdf.Xobject;
using iText.Layout.Element;
using iText.Test;

namespace iText.Layout {
    [NUnit.Framework.Category("IntegrationTest")]
    public class SimpleTextExtractionStrategyTest : ExtendedITextTest {
        private static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/layout/SimpleTextExtractionStrategyTest/";

        internal String TEXT1 = "TEXT1 TEXT1";

        internal String TEXT2 = "TEXT2 TEXT2";

        public virtual ITextExtractionStrategy CreateRenderListenerForTest() {
            return new SimpleTextExtractionStrategy();
        }

        [NUnit.Framework.Test]
        public virtual void TestCoLinnearText() {
            byte[] bytes = CreatePdfWithRotatedText(TEXT1, TEXT2, 0, false, 0);
            NUnit.Framework.Assert.AreEqual(TEXT1 + TEXT2, PdfTextExtractor.GetTextFromPage(new PdfDocument(new PdfReader
                (new MemoryStream(bytes))).GetPage(1), CreateRenderListenerForTest()));
        }

        [NUnit.Framework.Test]
        public virtual void TestCoLinnearTextWithSpace() {
            byte[] bytes = CreatePdfWithRotatedText(TEXT1, TEXT2, 0, false, 2);
            //saveBytesToFile(bytes, new File("c:/temp/test.pdf"));
            NUnit.Framework.Assert.AreEqual(TEXT1 + " " + TEXT2, PdfTextExtractor.GetTextFromPage(new PdfDocument(new 
                PdfReader(new MemoryStream(bytes))).GetPage(1), CreateRenderListenerForTest()));
        }

        [NUnit.Framework.Test]
        public virtual void TestCoLinnearTextEndingWithSpaceCharacter() {
            // in this case, we shouldn't be inserting an extra space
            byte[] bytes = CreatePdfWithRotatedText(TEXT1 + " ", TEXT2, 0, false, 2);
            //TestResourceUtils.openBytesAsPdf(bytes);
            NUnit.Framework.Assert.AreEqual(TEXT1 + " " + TEXT2, PdfTextExtractor.GetTextFromPage(new PdfDocument(new 
                PdfReader(new MemoryStream(bytes))).GetPage(1), CreateRenderListenerForTest()));
        }

        [NUnit.Framework.Test]
        public virtual void TestUnRotatedText() {
            byte[] bytes = CreatePdfWithRotatedText(TEXT1, TEXT2, 0, true, -20);
            NUnit.Framework.Assert.AreEqual(TEXT1 + "\n" + TEXT2, PdfTextExtractor.GetTextFromPage(new PdfDocument(new 
                PdfReader(new MemoryStream(bytes))).GetPage(1), CreateRenderListenerForTest()));
        }

        [NUnit.Framework.Test]
        public virtual void TestRotatedText() {
            byte[] bytes = CreatePdfWithRotatedText(TEXT1, TEXT2, -90, true, -20);
            NUnit.Framework.Assert.AreEqual(TEXT1 + "\n" + TEXT2, PdfTextExtractor.GetTextFromPage(new PdfDocument(new 
                PdfReader(new MemoryStream(bytes))).GetPage(1), CreateRenderListenerForTest()));
        }

        [NUnit.Framework.Test]
        public virtual void TestRotatedText2() {
            byte[] bytes = CreatePdfWithRotatedText(TEXT1, TEXT2, 90, true, -20);
            //TestResourceUtils.saveBytesToFile(bytes, new File("C:/temp/out.pdf"));
            NUnit.Framework.Assert.AreEqual(TEXT1 + "\n" + TEXT2, PdfTextExtractor.GetTextFromPage(new PdfDocument(new 
                PdfReader(new MemoryStream(bytes))).GetPage(1), CreateRenderListenerForTest()));
        }

        [NUnit.Framework.Test]
        public virtual void TestPartiallyRotatedText() {
            byte[] bytes = CreatePdfWithRotatedText(TEXT1, TEXT2, 33, true, -20);
            NUnit.Framework.Assert.AreEqual(TEXT1 + "\n" + TEXT2, PdfTextExtractor.GetTextFromPage(new PdfDocument(new 
                PdfReader(new MemoryStream(bytes))).GetPage(1), CreateRenderListenerForTest()));
        }

        [NUnit.Framework.Test]
        public virtual void TestWordSpacingCausedByExplicitGlyphPositioning() {
            byte[] bytes = CreatePdfWithArrayText(TEXT1, TEXT2, 250);
            NUnit.Framework.Assert.AreEqual(TEXT1 + " " + TEXT2, PdfTextExtractor.GetTextFromPage(new PdfDocument(new 
                PdfReader(new MemoryStream(bytes))).GetPage(1), CreateRenderListenerForTest()));
        }

        [NUnit.Framework.Test]
        public virtual void TestWordSpacingCausedByExplicitGlyphPositioning2() {
            PdfTextArray textArray = new PdfTextArray();
            textArray.Add(new PdfString("S"));
            textArray.Add(3.2f);
            textArray.Add(new PdfString("an"));
            textArray.Add(-255);
            textArray.Add(new PdfString("D"));
            textArray.Add(13);
            textArray.Add(new PdfString("i"));
            textArray.Add(8.3f);
            textArray.Add(new PdfString("e"));
            textArray.Add(-10.1f);
            textArray.Add(new PdfString("g"));
            textArray.Add(1.6f);
            textArray.Add(new PdfString("o"));
            textArray.Add(-247.5f);
            textArray.Add(new PdfString("C"));
            textArray.Add(2.4f);
            textArray.Add(new PdfString("h"));
            textArray.Add(5.8f);
            textArray.Add(new PdfString("ap"));
            textArray.Add(3);
            textArray.Add(new PdfString("t"));
            textArray.Add(10.7f);
            textArray.Add(new PdfString("er"));
            byte[] bytes = CreatePdfWithArrayText(textArray);
            NUnit.Framework.Assert.AreEqual("San Diego Chapter", PdfTextExtractor.GetTextFromPage(new PdfDocument(new 
                PdfReader(new MemoryStream(bytes))).GetPage(1), CreateRenderListenerForTest()));
        }

        [NUnit.Framework.Test]
        public virtual void TestTrailingSpace() {
            byte[] bytes = CreatePdfWithRotatedText(TEXT1 + " ", TEXT2, 0, false, 6);
            NUnit.Framework.Assert.AreEqual(TEXT1 + " " + TEXT2, PdfTextExtractor.GetTextFromPage(new PdfDocument(new 
                PdfReader(new MemoryStream(bytes))).GetPage(1), CreateRenderListenerForTest()));
        }

        [NUnit.Framework.Test]
        public virtual void TestLeadingSpace() {
            byte[] bytes = CreatePdfWithRotatedText(TEXT1, " " + TEXT2, 0, false, 6);
            NUnit.Framework.Assert.AreEqual(TEXT1 + " " + TEXT2, PdfTextExtractor.GetTextFromPage(new PdfDocument(new 
                PdfReader(new MemoryStream(bytes))).GetPage(1), CreateRenderListenerForTest()));
        }

        [NUnit.Framework.Test]
        public virtual void TestExtractXObjectText() {
            String text1 = "X";
            byte[] bytes = CreatePdfWithXObject(text1);
            String text = PdfTextExtractor.GetTextFromPage(new PdfDocument(new PdfReader(new MemoryStream(bytes))).GetPage
                (1), CreateRenderListenerForTest());
            NUnit.Framework.Assert.IsTrue(text.Contains(text1), "extracted text (" + text + ") must contain '" + text1
                 + "'");
        }

        [NUnit.Framework.Test]
        public virtual void ExtractFromPage229() {
            if (this.GetType() != typeof(SimpleTextExtractionStrategyTest)) {
                return;
            }
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(sourceFolder + "page229.pdf"));
            String text1 = PdfTextExtractor.GetTextFromPage(pdfDocument.GetPage(1), new SimpleTextExtractionStrategy()
                );
            String text2 = PdfTextExtractor.GetTextFromPage(pdfDocument.GetPage(1), new GlyphTextEventListener(new SimpleTextExtractionStrategy
                ()));
            pdfDocument.Close();
            NUnit.Framework.Assert.AreEqual(text1, text2);
        }

        [NUnit.Framework.Test]
        public virtual void ExtractFromIsoTc171() {
            if (this.GetType() != typeof(SimpleTextExtractionStrategyTest)) {
                return;
            }
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(sourceFolder + "ISO-TC171-SC2_N0896_SC2WG5_Edinburgh_Agenda.pdf"
                ));
            String text1 = PdfTextExtractor.GetTextFromPage(pdfDocument.GetPage(1), new SimpleTextExtractionStrategy()
                ) + "\n" + PdfTextExtractor.GetTextFromPage(pdfDocument.GetPage(2), new SimpleTextExtractionStrategy()
                );
            String text2 = PdfTextExtractor.GetTextFromPage(pdfDocument.GetPage(1), new GlyphTextEventListener(new SimpleTextExtractionStrategy
                ())) + "\n" + PdfTextExtractor.GetTextFromPage(pdfDocument.GetPage(2), new GlyphTextEventListener(new 
                SimpleTextExtractionStrategy()));
            pdfDocument.Close();
            NUnit.Framework.Assert.AreEqual(text1, text2);
        }

        internal virtual byte[] CreatePdfWithXObject(String xobjectText) {
            MemoryStream byteStream = new MemoryStream();
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(byteStream).SetCompressionLevel(0));
            Document document = new Document(pdfDocument);
            document.Add(new Paragraph("A"));
            document.Add(new Paragraph("B"));
            PdfFormXObject template = new PdfFormXObject(new Rectangle(100, 100));
            new PdfCanvas(template, pdfDocument).BeginText().SetFontAndSize(PdfFontFactory.CreateFont(StandardFonts.HELVETICA
                ), 12).MoveText(5, template.GetHeight() - 5).ShowText(xobjectText).EndText();
            document.Add(new Image(template));
            document.Add(new Paragraph("C"));
            document.Close();
            return byteStream.ToArray();
        }

        private static byte[] CreatePdfWithArrayText(PdfTextArray textArray) {
            MemoryStream byteStream = new MemoryStream();
            PdfDocument document = new PdfDocument(new PdfWriter(byteStream));
            document.SetDefaultPageSize(new PageSize(612, 792));
            PdfCanvas canvas = new PdfCanvas(document.AddNewPage());
            PdfFont font = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);
            canvas.BeginText().SetFontAndSize(font, 12);
            canvas.ShowText(textArray);
            canvas.EndText();
            document.Close();
            return byteStream.ToArray();
        }

        private static byte[] CreatePdfWithArrayText(String text1, String text2, int spaceInPoints) {
            PdfTextArray textArray = new PdfTextArray();
            textArray.Add(new PdfString(text1));
            textArray.Add(-spaceInPoints);
            textArray.Add(new PdfString(text2));
            return CreatePdfWithArrayText(textArray);
        }

        private static byte[] CreatePdfWithRotatedText(String text1, String text2, float rotation, bool moveTextToNextLine
            , float moveTextDelta) {
            MemoryStream byteStream = new MemoryStream();
            PdfDocument document = new PdfDocument(new PdfWriter(byteStream));
            document.SetDefaultPageSize(new PageSize(612, 792));
            PdfCanvas canvas = new PdfCanvas(document.AddNewPage());
            PdfFont font = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);
            float x = document.GetDefaultPageSize().GetWidth() / 2;
            float y = document.GetDefaultPageSize().GetHeight() / 2;
            canvas.ConcatMatrix(AffineTransform.GetTranslateInstance(x, y));
            canvas.MoveTo(-10, 0).LineTo(10, 0).MoveTo(0, -10).LineTo(0, 10).Stroke();
            canvas.BeginText().SetFontAndSize(font, 12).ConcatMatrix(AffineTransform.GetRotateInstance((float)(rotation
                 / 180f * Math.PI))).ShowText(text1);
            if (moveTextToNextLine) {
                canvas.MoveText(0, moveTextDelta);
            }
            else {
                canvas.ConcatMatrix(AffineTransform.GetTranslateInstance(moveTextDelta, 0));
            }
            canvas.ShowText(text2);
            canvas.EndText();
            document.Close();
            return byteStream.ToArray();
        }
    }
}
