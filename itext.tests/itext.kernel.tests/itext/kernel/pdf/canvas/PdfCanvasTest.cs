/*
This file is part of the iText (R) project.
Copyright (c) 1998-2025 Apryse Group NV
Authors: Apryse Software.

This program is offered under a commercial and under the AGPL license.
For commercial licensing, contact us at https://itextpdf.com/sales.  For AGPL licensing, see below.

AGPL licensing:
This program is free software: you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU Affero General Public License for more details.

You should have received a copy of the GNU Affero General Public License
along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/
using System;
using System.Collections.Generic;
using System.IO;
using iText.Commons.Utils;
using iText.IO.Font;
using iText.IO.Font.Constants;
using iText.IO.Font.Otf;
using iText.IO.Image;
using iText.IO.Source;
using iText.IO.Util;
using iText.Kernel.Colors;
using iText.Kernel.Exceptions;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Wmf;
using iText.Kernel.Pdf.Extgstate;
using iText.Kernel.Utils;
using iText.Test;

namespace iText.Kernel.Pdf.Canvas {
    [NUnit.Framework.Category("IntegrationTest")]
    public class PdfCanvasTest : ExtendedITextTest {
        /// <summary>Paths to images.</summary>
        private static readonly String[] RESOURCES = new String[] { "Desert.jpg", "bulb.gif", "0047478.jpg", "itext.png"
             };

        private static readonly String DESTINATION_FOLDER = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/kernel/pdf/canvas/PdfCanvasTest/";

        private static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/kernel/pdf/canvas/PdfCanvasTest/";

        private static readonly String FONTS_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/kernel/pdf/fonts/";

        private const String AUTHOR = "iText Software";

        private const String CREATOR = "iText";

        private const String TITLE = "Empty iText Document";

        private sealed class _ContentProvider_87 : PdfCanvasTest.ContentProvider {
            public _ContentProvider_87() {
            }

            public void DrawOnCanvas(PdfCanvas canvas, int pageNumber) {
                canvas.SaveState().BeginText().MoveText(36, 700).SetFontAndSize(PdfFontFactory.CreateFont(StandardFonts.HELVETICA
                    ), 72).ShowText(JavaUtil.IntegerToString(pageNumber + 1)).EndText().RestoreState();
                canvas.Rectangle(100, 500, 100, 100).Fill();
            }
        }

        private static readonly PdfCanvasTest.ContentProvider DEFAULT_CONTENT_PROVIDER = new _ContentProvider_87();

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateOrClearDestinationFolder(DESTINATION_FOLDER);
        }

        [NUnit.Framework.OneTimeTearDown]
        public static void AfterClass() {
            CompareTool.Cleanup(DESTINATION_FOLDER);
        }

        [NUnit.Framework.Test]
        public virtual void CreateSimpleCanvas() {
            String filename = DESTINATION_FOLDER + "simpleCanvas.pdf";
            PdfDocument pdfDoc = new PdfDocument(CompareTool.CreateTestPdfWriter(filename));
            pdfDoc.GetDocumentInfo().SetAuthor(AUTHOR).SetCreator(CREATOR).SetTitle(TITLE);
            PdfPage page1 = pdfDoc.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page1);
            canvas.Rectangle(100, 100, 100, 100).Fill();
            canvas.Release();
            pdfDoc.Close();
            AssertStandardDocument(filename, 1);
        }

        [NUnit.Framework.Test]
        public virtual void CanvasDrawArcsTest() {
            String fileName = "canvasDrawArcsTest.pdf";
            String output = DESTINATION_FOLDER + fileName;
            String cmp = SOURCE_FOLDER + "cmp_" + fileName;
            using (PdfDocument doc = new PdfDocument(CompareTool.CreateTestPdfWriter(output))) {
                PdfPage page = doc.AddNewPage();
                PdfCanvas canvas = new PdfCanvas(page);
                canvas.SetLineWidth(5);
                canvas.SetStrokeColor(ColorConstants.BLUE);
                canvas.MoveTo(10, 300);
                canvas.LineTo(50, 300);
                canvas.Arc(100, 550, 200, 600, 90, -135);
                canvas.ClosePath();
                canvas.Stroke();
                canvas.SetStrokeColor(ColorConstants.RED);
                canvas.MoveTo(210, 300);
                canvas.LineTo(250, 300);
                canvas.ArcContinuous(300, 550, 400, 600, 90, -135);
                canvas.ClosePath();
                canvas.Stroke();
                canvas.Release();
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(output, cmp, DESTINATION_FOLDER, "diff_")
                );
        }

        [NUnit.Framework.Test]
        public virtual void CreateSimpleCanvasWithDrawing() {
            String fileName = DESTINATION_FOLDER + "simpleCanvasWithDrawing.pdf";
            PdfDocument pdfDoc = new PdfDocument(CompareTool.CreateTestPdfWriter(fileName));
            pdfDoc.GetDocumentInfo().SetAuthor(AUTHOR).SetCreator(CREATOR).SetTitle(TITLE);
            PdfPage page1 = pdfDoc.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page1);
            canvas.SaveState().SetLineWidth(30).MoveTo(36, 700).LineTo(300, 300).Stroke().RestoreState();
            canvas.SaveState().Rectangle(250, 500, 100, 100).Fill().RestoreState();
            canvas.SaveState().Circle(100, 400, 25).Fill().RestoreState();
            canvas.SaveState().RoundRectangle(100, 650, 100, 100, 10).Fill().RestoreState();
            canvas.SaveState().SetLineWidth(10).RoundRectangle(250, 650, 100, 100, 10).Stroke().RestoreState();
            canvas.SaveState().SetLineWidth(5).Arc(400, 650, 550, 750, 0, 180).Stroke().RestoreState();
            canvas.SaveState().SetLineWidth(5).MoveTo(400, 550).CurveTo(500, 570, 450, 450, 550, 550).Stroke().RestoreState
                ();
            canvas.Release();
            pdfDoc.Close();
            AssertStandardDocument(fileName, 1);
        }

        [NUnit.Framework.Test]
        public virtual void CreateSimpleCanvasWithText() {
            String fileName = DESTINATION_FOLDER + "simpleCanvasWithText.pdf";
            PdfDocument pdfDoc = new PdfDocument(CompareTool.CreateTestPdfWriter(fileName));
            pdfDoc.GetDocumentInfo().SetAuthor(AUTHOR).SetCreator(CREATOR).SetTitle(TITLE);
            PdfPage page1 = pdfDoc.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page1);
            //Initialize canvas and write text to it
            canvas.SaveState().BeginText().MoveText(36, 750).SetFontAndSize(PdfFontFactory.CreateFont(StandardFonts.HELVETICA
                ), 16).ShowText("Hello Helvetica!").EndText().RestoreState();
            canvas.SaveState().BeginText().MoveText(36, 700).SetFontAndSize(PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLDOBLIQUE
                ), 16).ShowText("Hello Helvetica Bold Oblique!").EndText().RestoreState();
            canvas.SaveState().BeginText().MoveText(36, 650).SetFontAndSize(PdfFontFactory.CreateFont(StandardFonts.COURIER
                ), 16).ShowText("Hello Courier!").EndText().RestoreState();
            canvas.SaveState().BeginText().MoveText(36, 600).SetFontAndSize(PdfFontFactory.CreateFont(StandardFonts.TIMES_ITALIC
                ), 16).ShowText("Hello Times Italic!").EndText().RestoreState();
            canvas.SaveState().BeginText().MoveText(36, 550).SetFontAndSize(PdfFontFactory.CreateFont(StandardFonts.SYMBOL
                ), 16).ShowText("Hello Ellada!").EndText().RestoreState();
            canvas.SaveState().BeginText().MoveText(36, 500).SetFontAndSize(PdfFontFactory.CreateFont(StandardFonts.ZAPFDINGBATS
                ), 16).ShowText("Hello ZapfDingbats!").EndText().RestoreState();
            canvas.Release();
            pdfDoc.Close();
            AssertStandardDocument(fileName, 1);
        }

        [NUnit.Framework.Test]
        public virtual void CreateSimpleCanvasWithPageFlush() {
            String filename = DESTINATION_FOLDER + "simpleCanvasWithPageFlush.pdf";
            PdfDocument pdfDoc = new PdfDocument(CompareTool.CreateTestPdfWriter(filename));
            pdfDoc.GetDocumentInfo().SetAuthor(AUTHOR).SetCreator(CREATOR).SetTitle(TITLE);
            PdfPage page1 = pdfDoc.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page1);
            canvas.Rectangle(100, 100, 100, 100).Fill();
            canvas.Release();
            page1.Flush();
            pdfDoc.Close();
            AssertStandardDocument(filename, 1);
        }

        [NUnit.Framework.Test]
        public virtual void CreateSimpleCanvasWithFullCompression() {
            String filename = DESTINATION_FOLDER + "simpleCanvasWithFullCompression.pdf";
            PdfWriter writer = CompareTool.CreateTestPdfWriter(filename, new WriterProperties().SetFullCompressionMode
                (true));
            PdfDocument pdfDoc = new PdfDocument(writer);
            pdfDoc.GetDocumentInfo().SetAuthor(AUTHOR).SetCreator(CREATOR).SetTitle(TITLE);
            PdfPage page1 = pdfDoc.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page1);
            canvas.Rectangle(100, 100, 100, 100).Fill();
            canvas.Release();
            pdfDoc.Close();
            AssertStandardDocument(filename, 1);
        }

        [NUnit.Framework.Test]
        public virtual void CreateSimpleCanvasWithPageFlushAndFullCompression() {
            String filename = DESTINATION_FOLDER + "simpleCanvasWithPageFlushAndFullCompression.pdf";
            PdfWriter writer = CompareTool.CreateTestPdfWriter(filename, new WriterProperties().SetFullCompressionMode
                (true));
            PdfDocument pdfDoc = new PdfDocument(writer);
            pdfDoc.GetDocumentInfo().SetAuthor(AUTHOR).SetCreator(CREATOR).SetTitle(TITLE);
            PdfPage page1 = pdfDoc.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page1);
            canvas.Rectangle(100, 100, 100, 100).Fill();
            canvas.Release();
            page1.Flush();
            pdfDoc.Close();
            AssertStandardDocument(filename, 1);
        }

        [NUnit.Framework.Test]
        public virtual void Create1000PagesDocument() {
            int pageCount = 1000;
            String filename = DESTINATION_FOLDER + pageCount + "PagesDocument.pdf";
            PdfWriter writer = CompareTool.CreateTestPdfWriter(filename);
            CreateStandardDocument(writer, pageCount, DEFAULT_CONTENT_PROVIDER);
            AssertStandardDocument(filename, pageCount);
        }

        [NUnit.Framework.Test]
        public virtual void Create100PagesDocument() {
            int pageCount = 100;
            String filename = DESTINATION_FOLDER + pageCount + "PagesDocument.pdf";
            PdfWriter writer = CompareTool.CreateTestPdfWriter(filename);
            CreateStandardDocument(writer, pageCount, DEFAULT_CONTENT_PROVIDER);
            AssertStandardDocument(filename, pageCount);
        }

        [NUnit.Framework.Test]
        public virtual void Create10PagesDocument() {
            int pageCount = 10;
            String filename = DESTINATION_FOLDER + pageCount + "PagesDocument.pdf";
            PdfWriter writer = CompareTool.CreateTestPdfWriter(filename);
            CreateStandardDocument(writer, pageCount, DEFAULT_CONTENT_PROVIDER);
            AssertStandardDocument(filename, pageCount);
        }

        [NUnit.Framework.Test]
        public virtual void Create1000PagesDocumentWithText() {
            int pageCount = 1000;
            String filename = DESTINATION_FOLDER + "1000PagesDocumentWithText.pdf";
            PdfWriter writer = CompareTool.CreateTestPdfWriter(filename);
            CreateStandardDocument(writer, pageCount, new _ContentProvider_388());
            AssertStandardDocument(filename, pageCount);
        }

        private sealed class _ContentProvider_388 : PdfCanvasTest.ContentProvider {
            public _ContentProvider_388() {
            }

            public void DrawOnCanvas(PdfCanvas canvas, int pageNumber) {
                canvas.SaveState().BeginText().MoveText(36, 650).SetFontAndSize(PdfFontFactory.CreateFont(StandardFonts.COURIER
                    ), 16).ShowText("Page " + (pageNumber + 1)).EndText();
                canvas.Rectangle(100, 100, 100, 100).Fill();
            }
        }

        [NUnit.Framework.Test]
        public virtual void Create1000PagesDocumentWithFullCompression() {
            int pageCount = 1000;
            String filename = DESTINATION_FOLDER + "1000PagesDocumentWithFullCompression.pdf";
            PdfWriter writer = CompareTool.CreateTestPdfWriter(filename, new WriterProperties().SetFullCompressionMode
                (true));
            CreateStandardDocument(writer, pageCount, DEFAULT_CONTENT_PROVIDER);
            AssertStandardDocument(filename, pageCount);
        }

        [NUnit.Framework.Test]
        public virtual void SmallDocumentWithFullCompression() {
            String filename = DESTINATION_FOLDER + "smallDocumentWithFullCompression.pdf";
            PdfWriter writer = CompareTool.CreateTestPdfWriter(filename, new WriterProperties().SetFullCompressionMode
                (true));
            PdfDocument pdfDoc = new PdfDocument(writer);
            pdfDoc.GetDocumentInfo().SetAuthor(AUTHOR).SetCreator(CREATOR).SetTitle(TITLE);
            PdfPage page = pdfDoc.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page);
            canvas.SaveState().BeginText().MoveText(36, 700).SetFontAndSize(PdfFontFactory.CreateFont(StandardFonts.HELVETICA
                ), 72).ShowText("Hi!").EndText().RestoreState();
            page.Flush();
            pdfDoc.Close();
            AssertStandardDocument(filename, 1);
        }

        [NUnit.Framework.Test]
        public virtual void Create100PagesDocumentWithFullCompression() {
            int pageCount = 100;
            String filename = DESTINATION_FOLDER + pageCount + "PagesDocumentWithFullCompression.pdf";
            PdfWriter writer = CompareTool.CreateTestPdfWriter(filename, new WriterProperties().SetFullCompressionMode
                (true));
            CreateStandardDocument(writer, pageCount, DEFAULT_CONTENT_PROVIDER);
            AssertStandardDocument(filename, pageCount);
        }

        [NUnit.Framework.Test]
        public virtual void Create197PagesDocumentWithFullCompression() {
            int pageCount = 197;
            String filename = DESTINATION_FOLDER + pageCount + "PagesDocumentWithFullCompression.pdf";
            PdfWriter writer = CompareTool.CreateTestPdfWriter(filename, new WriterProperties().SetFullCompressionMode
                (true));
            CreateStandardDocument(writer, pageCount, DEFAULT_CONTENT_PROVIDER);
            AssertStandardDocument(filename, pageCount);
        }

        [NUnit.Framework.Test]
        public virtual void Create10PagesDocumentWithFullCompression() {
            int pageCount = 10;
            String filename = DESTINATION_FOLDER + pageCount + "PagesDocumentWithFullCompression.pdf";
            PdfWriter writer = CompareTool.CreateTestPdfWriter(filename, new WriterProperties().SetFullCompressionMode
                (true));
            CreateStandardDocument(writer, pageCount, DEFAULT_CONTENT_PROVIDER);
            AssertStandardDocument(filename, pageCount);
        }

        [NUnit.Framework.Test]
        public virtual void CopyPagesTest1() {
            String file1 = DESTINATION_FOLDER + "copyPages1_1.pdf";
            String file2 = DESTINATION_FOLDER + "copyPages1_2.pdf";
            PdfDocument pdfDoc1 = new PdfDocument(CompareTool.CreateTestPdfWriter(file1));
            PdfPage page1 = pdfDoc1.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page1);
            canvas.Rectangle(100, 600, 100, 100);
            canvas.Fill();
            canvas.BeginText();
            canvas.SetFontAndSize(PdfFontFactory.CreateFont(StandardFonts.COURIER), 12);
            canvas.SetTextMatrix(1, 0, 0, 1, 100, 500);
            canvas.ShowText("Hello World!");
            canvas.EndText();
            canvas.Release();
            page1.Flush();
            pdfDoc1.Close();
            pdfDoc1 = new PdfDocument(CompareTool.CreateOutputReader(file1));
            page1 = pdfDoc1.GetPage(1);
            PdfDocument pdfDoc2 = new PdfDocument(CompareTool.CreateTestPdfWriter(file2));
            PdfPage page2 = page1.CopyTo(pdfDoc2);
            pdfDoc2.AddPage(page2);
            page2.Flush();
            pdfDoc2.Close();
            PdfReader reader = CompareTool.CreateOutputReader(file2);
            PdfDocument pdfDocument = new PdfDocument(reader);
            for (int i = 1; i <= pdfDocument.GetNumberOfPages(); i++) {
                PdfDictionary page = pdfDocument.GetPage(i).GetPdfObject();
                NUnit.Framework.Assert.AreEqual(PdfName.Page, page.Get(PdfName.Type));
            }
            reader.Close();
            NUnit.Framework.Assert.AreEqual(false, reader.HasRebuiltXref(), "Rebuilt");
            PdfDictionary page_1 = pdfDocument.GetPage(1).GetPdfObject();
            NUnit.Framework.Assert.IsNotNull(page_1.Get(PdfName.Parent));
            pdfDocument.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(file1, file2, DESTINATION_FOLDER, "diff_"
                ));
        }

        [NUnit.Framework.Test]
        public virtual void CopyPagesTest2() {
            String file1 = DESTINATION_FOLDER + "copyPages2_1.pdf";
            String file2 = DESTINATION_FOLDER + "copyPages2_2.pdf";
            PdfWriter writer1 = CompareTool.CreateTestPdfWriter(file1);
            PdfDocument pdfDoc1 = new PdfDocument(writer1);
            for (int i = 0; i < 10; i++) {
                PdfPage page1 = pdfDoc1.AddNewPage();
                PdfCanvas canvas = new PdfCanvas(page1);
                canvas.Rectangle(100, 600, 100, 100);
                canvas.Fill();
                canvas.BeginText();
                canvas.SetFontAndSize(PdfFontFactory.CreateFont(StandardFonts.COURIER), 12);
                canvas.SetTextMatrix(1, 0, 0, 1, 100, 500);
                canvas.ShowText(MessageFormatUtil.Format("Page_{0}", i + 1));
                canvas.EndText();
                canvas.Release();
                page1.Flush();
            }
            pdfDoc1.Close();
            pdfDoc1 = new PdfDocument(CompareTool.CreateOutputReader(file1));
            PdfWriter writer2 = CompareTool.CreateTestPdfWriter(file2);
            PdfDocument pdfDoc2 = new PdfDocument(writer2);
            for (int i = 9; i >= 0; i--) {
                PdfPage page2 = pdfDoc1.GetPage(i + 1).CopyTo(pdfDoc2);
                pdfDoc2.AddPage(page2);
            }
            pdfDoc1.Close();
            pdfDoc2.Close();
            PdfReader reader = CompareTool.CreateOutputReader(file2);
            PdfDocument pdfDocument = new PdfDocument(reader);
            NUnit.Framework.Assert.AreEqual(false, reader.HasRebuiltXref(), "Rebuilt");
            PdfDictionary page = pdfDocument.GetPage(1).GetPdfObject();
            NUnit.Framework.Assert.IsNotNull(page.Get(PdfName.Parent));
            pdfDocument.Close();
            CompareTool cmpTool = new CompareTool();
            PdfDocument doc1 = new PdfDocument(CompareTool.CreateOutputReader(file1));
            PdfDocument doc2 = new PdfDocument(CompareTool.CreateOutputReader(file2));
            for (int i = 0; i < 10; i++) {
                PdfDictionary page1 = doc1.GetPage(i + 1).GetPdfObject();
                PdfDictionary page2 = doc2.GetPage(10 - i).GetPdfObject();
                NUnit.Framework.Assert.IsTrue(cmpTool.CompareDictionaries(page1, page2));
            }
            doc1.Close();
            doc2.Close();
        }

        [NUnit.Framework.Test]
        public virtual void CopyPagesTest3() {
            String file1 = DESTINATION_FOLDER + "copyPages3_1.pdf";
            String file2 = DESTINATION_FOLDER + "copyPages3_2.pdf";
            PdfDocument pdfDoc1 = new PdfDocument(CompareTool.CreateTestPdfWriter(file1));
            PdfPage page1 = pdfDoc1.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page1);
            canvas.Rectangle(100, 600, 100, 100);
            canvas.Fill();
            canvas.BeginText();
            canvas.SetFontAndSize(PdfFontFactory.CreateFont(StandardFonts.COURIER), 12);
            canvas.SetTextMatrix(1, 0, 0, 1, 100, 500);
            canvas.ShowText("Hello World!!!");
            canvas.EndText();
            canvas.Release();
            page1.Flush();
            pdfDoc1.Close();
            pdfDoc1 = new PdfDocument(CompareTool.CreateOutputReader(file1));
            page1 = pdfDoc1.GetPage(1);
            PdfDocument pdfDoc2 = new PdfDocument(CompareTool.CreateTestPdfWriter(file2));
            for (int i = 0; i < 10; i++) {
                PdfPage page2 = page1.CopyTo(pdfDoc2);
                pdfDoc2.AddPage(page2);
                if (i % 2 == 0) {
                    page2.Flush();
                }
            }
            pdfDoc1.Close();
            pdfDoc2.Close();
            CompareTool cmpTool = new CompareTool();
            PdfReader reader1 = CompareTool.CreateOutputReader(file1);
            PdfDocument doc1 = new PdfDocument(reader1);
            NUnit.Framework.Assert.AreEqual(false, reader1.HasRebuiltXref(), "Rebuilt");
            PdfDictionary p1 = doc1.GetPage(1).GetPdfObject();
            PdfReader reader2 = CompareTool.CreateOutputReader(file2);
            PdfDocument doc2 = new PdfDocument(reader2);
            NUnit.Framework.Assert.AreEqual(false, reader2.HasRebuiltXref(), "Rebuilt");
            for (int i = 0; i < 10; i++) {
                PdfDictionary p2 = doc2.GetPage(i + 1).GetPdfObject();
                NUnit.Framework.Assert.IsTrue(cmpTool.CompareDictionaries(p1, p2));
            }
            doc1.Close();
            doc2.Close();
        }

        [NUnit.Framework.Test]
        public virtual void CopyPagesTest4() {
            String file1 = DESTINATION_FOLDER + "copyPages4_1.pdf";
            PdfWriter writer = CompareTool.CreateTestPdfWriter(file1);
            PdfDocument pdfDoc1 = new PdfDocument(writer);
            for (int i = 0; i < 5; i++) {
                PdfPage page1 = pdfDoc1.AddNewPage();
                PdfCanvas canvas = new PdfCanvas(page1);
                canvas.Rectangle(100, 600, 100, 100);
                canvas.Fill();
                canvas.BeginText();
                canvas.SetFontAndSize(PdfFontFactory.CreateFont(StandardFonts.COURIER), 12);
                canvas.SetTextMatrix(1, 0, 0, 1, 100, 500);
                canvas.ShowText(MessageFormatUtil.Format("Page_{0}", i + 1));
                canvas.EndText();
                canvas.Release();
            }
            pdfDoc1.Close();
            pdfDoc1 = new PdfDocument(CompareTool.CreateOutputReader(file1));
            for (int i = 0; i < 5; i++) {
                PdfDocument pdfDoc2 = new PdfDocument(CompareTool.CreateTestPdfWriter(DESTINATION_FOLDER + MessageFormatUtil
                    .Format("copyPages4_{0}.pdf", i + 2)));
                PdfPage page2 = pdfDoc1.GetPage(i + 1).CopyTo(pdfDoc2);
                pdfDoc2.AddPage(page2);
                pdfDoc2.Close();
            }
            pdfDoc1.Close();
            CompareTool cmpTool = new CompareTool();
            PdfReader reader1 = CompareTool.CreateOutputReader(file1);
            PdfDocument doc1 = new PdfDocument(reader1);
            NUnit.Framework.Assert.AreEqual(false, reader1.HasRebuiltXref(), "Rebuilt");
            for (int i = 0; i < 5; i++) {
                PdfDictionary page1 = doc1.GetPage(i + 1).GetPdfObject();
                PdfDocument doc2 = new PdfDocument(CompareTool.CreateOutputReader(DESTINATION_FOLDER + MessageFormatUtil.Format
                    ("copyPages4_{0}.pdf", i + 2)));
                PdfDictionary page = doc2.GetPage(1).GetPdfObject();
                NUnit.Framework.Assert.IsTrue(cmpTool.CompareDictionaries(page1, page));
                doc2.Close();
            }
            doc1.Close();
        }

        [NUnit.Framework.Test]
        public virtual void CopyPagesTest5() {
            int documentCount = 3;
            for (int i = 0; i < documentCount; i++) {
                PdfDocument pdfDoc1 = new PdfDocument(CompareTool.CreateTestPdfWriter(DESTINATION_FOLDER + MessageFormatUtil
                    .Format("copyPages5_{0}.pdf", i + 1)));
                PdfPage page1 = pdfDoc1.AddNewPage();
                PdfCanvas canvas = new PdfCanvas(page1);
                canvas.Rectangle(100, 600, 100, 100);
                canvas.Fill();
                canvas.BeginText();
                canvas.SetFontAndSize(PdfFontFactory.CreateFont(StandardFonts.COURIER), 12);
                canvas.SetTextMatrix(1, 0, 0, 1, 100, 500);
                canvas.ShowText(MessageFormatUtil.Format("Page_{0}", i + 1));
                canvas.EndText();
                canvas.Release();
                pdfDoc1.Close();
            }
            IList<PdfDocument> docs = new List<PdfDocument>();
            for (int i = 0; i < documentCount; i++) {
                PdfDocument pdfDoc1 = new PdfDocument(CompareTool.CreateOutputReader(DESTINATION_FOLDER + MessageFormatUtil
                    .Format("copyPages5_{0}.pdf", i + 1)));
                docs.Add(pdfDoc1);
            }
            PdfDocument pdfDoc2 = new PdfDocument(CompareTool.CreateTestPdfWriter(DESTINATION_FOLDER + "copyPages5_4.pdf"
                ));
            for (int i = 0; i < 3; i++) {
                pdfDoc2.AddPage(docs[i].GetPage(1).CopyTo(pdfDoc2));
            }
            pdfDoc2.Close();
            foreach (PdfDocument doc in docs) {
                doc.Close();
            }
            CompareTool cmpTool = new CompareTool();
            for (int i = 0; i < 3; i++) {
                PdfReader reader1 = CompareTool.CreateOutputReader(DESTINATION_FOLDER + MessageFormatUtil.Format("copyPages5_{0}.pdf"
                    , i + 1));
                PdfDocument doc1 = new PdfDocument(reader1);
                NUnit.Framework.Assert.AreEqual(false, reader1.HasRebuiltXref(), "Rebuilt");
                PdfReader reader2 = CompareTool.CreateOutputReader(DESTINATION_FOLDER + "copyPages5_4.pdf");
                PdfDocument doc2 = new PdfDocument(reader2);
                NUnit.Framework.Assert.AreEqual(false, reader2.HasRebuiltXref(), "Rebuilt");
                PdfDictionary page1 = doc1.GetPage(1).GetPdfObject();
                PdfDictionary page2 = doc2.GetPage(i + 1).GetPdfObject();
                NUnit.Framework.Assert.IsTrue(cmpTool.CompareDictionaries(page1, page2));
                doc1.Close();
                doc2.Close();
            }
        }

        [NUnit.Framework.Test]
        public virtual void CopyPagesTest6() {
            String file1 = DESTINATION_FOLDER + "copyPages6_1.pdf";
            String file2 = DESTINATION_FOLDER + "copyPages6_2.pdf";
            String file3 = DESTINATION_FOLDER + "copyPages6_3.pdf";
            String file1_upd = DESTINATION_FOLDER + "copyPages6_1_upd.pdf";
            PdfWriter writer1 = CompareTool.CreateTestPdfWriter(file1);
            PdfDocument pdfDoc1 = new PdfDocument(writer1);
            PdfPage page1 = pdfDoc1.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page1);
            canvas.Rectangle(100, 600, 100, 100);
            canvas.Fill();
            canvas.BeginText();
            canvas.SetFontAndSize(PdfFontFactory.CreateFont(StandardFonts.COURIER), 12);
            canvas.SetTextMatrix(1, 0, 0, 1, 100, 500);
            canvas.ShowText("Hello World!");
            canvas.EndText();
            canvas.Release();
            pdfDoc1.Close();
            pdfDoc1 = new PdfDocument(CompareTool.CreateOutputReader(file1));
            PdfWriter writer2 = CompareTool.CreateTestPdfWriter(file2);
            PdfDocument pdfDoc2 = new PdfDocument(writer2);
            pdfDoc2.AddPage(pdfDoc1.GetPage(1).CopyTo(pdfDoc2));
            pdfDoc2.Close();
            pdfDoc2 = new PdfDocument(CompareTool.CreateOutputReader(file2));
            PdfWriter writer3 = CompareTool.CreateTestPdfWriter(file3);
            PdfDocument pdfDoc3 = new PdfDocument(writer3);
            pdfDoc3.AddPage(pdfDoc2.GetPage(1).CopyTo(pdfDoc3));
            pdfDoc3.Close();
            pdfDoc3 = new PdfDocument(CompareTool.CreateOutputReader(file3));
            pdfDoc1.Close();
            PdfWriter writer1_ipd = CompareTool.CreateTestPdfWriter(file1_upd);
            pdfDoc1 = new PdfDocument(CompareTool.CreateOutputReader(file1), writer1_ipd);
            pdfDoc1.AddPage(pdfDoc3.GetPage(1).CopyTo(pdfDoc1));
            pdfDoc1.Close();
            pdfDoc2.Close();
            pdfDoc3.Close();
            CompareTool cmpTool = new CompareTool();
            for (int i = 0; i < 3; i++) {
                PdfReader reader1 = CompareTool.CreateOutputReader(file1);
                PdfDocument doc1 = new PdfDocument(reader1);
                NUnit.Framework.Assert.AreEqual(false, reader1.HasRebuiltXref(), "Rebuilt");
                PdfReader reader2 = CompareTool.CreateOutputReader(file2);
                PdfDocument doc2 = new PdfDocument(reader2);
                NUnit.Framework.Assert.AreEqual(false, reader2.HasRebuiltXref(), "Rebuilt");
                PdfReader reader3 = CompareTool.CreateOutputReader(file3);
                PdfDocument doc3 = new PdfDocument(reader3);
                NUnit.Framework.Assert.AreEqual(false, reader3.HasRebuiltXref(), "Rebuilt");
                PdfReader reader4 = CompareTool.CreateOutputReader(file1_upd);
                PdfDocument doc4 = new PdfDocument(reader4);
                NUnit.Framework.Assert.AreEqual(false, reader4.HasRebuiltXref(), "Rebuilt");
                NUnit.Framework.Assert.IsTrue(cmpTool.CompareDictionaries(doc1.GetPage(1).GetPdfObject(), doc4.GetPage(2).
                    GetPdfObject()));
                NUnit.Framework.Assert.IsTrue(cmpTool.CompareDictionaries(doc4.GetPage(2).GetPdfObject(), doc2.GetPage(1).
                    GetPdfObject()));
                NUnit.Framework.Assert.IsTrue(cmpTool.CompareDictionaries(doc2.GetPage(1).GetPdfObject(), doc4.GetPage(1).
                    GetPdfObject()));
                doc1.Close();
                doc2.Close();
                doc3.Close();
                doc4.Close();
            }
        }

        [NUnit.Framework.Test]
        public virtual void MarkedContentTest1() {
            String message = "";
            PdfDocument document = new PdfDocument(new PdfWriter(new ByteArrayOutputStream()));
            PdfPage page = document.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page);
            canvas.BeginMarkedContent(new PdfName("Tag1"));
            canvas.EndMarkedContent();
            try {
                canvas.EndMarkedContent();
            }
            catch (PdfException e) {
                message = e.Message;
            }
            canvas.Release();
            document.Close();
            NUnit.Framework.Assert.AreEqual(KernelExceptionMessageConstant.UNBALANCED_BEGIN_END_MARKED_CONTENT_OPERATORS
                , message);
        }

        [NUnit.Framework.Test]
        public virtual void MarkedContentTest2() {
            PdfDocument document = new PdfDocument(CompareTool.CreateTestPdfWriter(DESTINATION_FOLDER + "markedContentTest2.pdf"
                ));
            PdfPage page = document.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page);
            Dictionary<PdfName, PdfObject> tmpMap = new Dictionary<PdfName, PdfObject>();
            tmpMap.Put(new PdfName("Tag"), new PdfNumber(2));
            PdfDictionary tag2 = new PdfDictionary(tmpMap);
            tmpMap = new Dictionary<PdfName, PdfObject>();
            tmpMap.Put(new PdfName("Tag"), new PdfNumber(3).MakeIndirect(document));
            PdfDictionary tag3 = new PdfDictionary(tmpMap);
            canvas.BeginMarkedContent(new PdfName("Tag1")).EndMarkedContent().BeginMarkedContent(new PdfName("Tag2"), 
                tag2).EndMarkedContent().BeginMarkedContent(new PdfName("Tag3"), (PdfDictionary)tag3.MakeIndirect(document
                )).EndMarkedContent();
            canvas.Release();
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(DESTINATION_FOLDER + "markedContentTest2.pdf"
                , SOURCE_FOLDER + "cmp_markedContentTest2.pdf", DESTINATION_FOLDER, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void GraphicsStateTest1() {
            PdfDocument document = new PdfDocument(new PdfWriter(new ByteArrayOutputStream()));
            PdfPage page = document.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page);
            canvas.SetLineWidth(3);
            canvas.SaveState();
            canvas.SetLineWidth(5);
            NUnit.Framework.Assert.AreEqual(5, canvas.GetGraphicsState().GetLineWidth(), 0);
            canvas.RestoreState();
            NUnit.Framework.Assert.AreEqual(3, canvas.GetGraphicsState().GetLineWidth(), 0);
            PdfExtGState egs = new PdfExtGState();
            egs.GetPdfObject().Put(PdfName.LW, new PdfNumber(2));
            canvas.SetExtGState(egs);
            NUnit.Framework.Assert.AreEqual(2, canvas.GetGraphicsState().GetLineWidth(), 0);
            canvas.Release();
            document.Close();
        }

        [NUnit.Framework.Test]
        public virtual void WmfImageTest01() {
            PdfDocument document = new PdfDocument(CompareTool.CreateTestPdfWriter(DESTINATION_FOLDER + "wmfImageTest01.pdf"
                ));
            PdfPage page = document.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page);
            ImageData img = new WmfImageData(SOURCE_FOLDER + "example.wmf");
            canvas.AddImageFittedIntoRectangle(img, new Rectangle(0, 0, 0.1f, 0.1f), false);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(DESTINATION_FOLDER + "wmfImageTest01.pdf"
                , SOURCE_FOLDER + "cmp_wmfImageTest01.pdf", DESTINATION_FOLDER, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void WmfImageTest02() {
            PdfDocument document = new PdfDocument(CompareTool.CreateTestPdfWriter(DESTINATION_FOLDER + "wmfImageTest02.pdf"
                ));
            PdfPage page = document.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page);
            ImageData img = new WmfImageData(SOURCE_FOLDER + "butterfly.wmf");
            canvas.AddImageFittedIntoRectangle(img, new Rectangle(0, 0, 1, 1), false);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(DESTINATION_FOLDER + "wmfImageTest02.pdf"
                , SOURCE_FOLDER + "cmp_wmfImageTest02.pdf", DESTINATION_FOLDER, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void WmfImageTest03() {
            PdfDocument document = new PdfDocument(CompareTool.CreateTestPdfWriter(DESTINATION_FOLDER + "wmfImageTest03.pdf"
                ));
            PdfPage page = document.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page);
            ImageData img = new WmfImageData(SOURCE_FOLDER + "type1.wmf");
            canvas.AddImageFittedIntoRectangle(img, new Rectangle(0, 0, 1, 1), false);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(DESTINATION_FOLDER + "wmfImageTest03.pdf"
                , SOURCE_FOLDER + "cmp_wmfImageTest03.pdf", DESTINATION_FOLDER, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void WmfImageTest04() {
            PdfDocument document = new PdfDocument(CompareTool.CreateTestPdfWriter(DESTINATION_FOLDER + "wmfImageTest04.pdf"
                ));
            PdfPage page = document.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page);
            ImageData img = new WmfImageData(SOURCE_FOLDER + "type0.wmf");
            canvas.AddImageFittedIntoRectangle(img, new Rectangle(0, 0, 1, 1), false);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(DESTINATION_FOLDER + "wmfImageTest04.pdf"
                , SOURCE_FOLDER + "cmp_wmfImageTest04.pdf", DESTINATION_FOLDER, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void WmfImageTest05() {
            PdfDocument document = new PdfDocument(CompareTool.CreateTestPdfWriter(DESTINATION_FOLDER + "wmfImageTest05.pdf"
                ));
            PdfPage page = document.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page);
            Stream stream = UrlUtil.OpenStream(UrlUtil.ToURL(SOURCE_FOLDER + "example2.wmf"));
            ByteArrayOutputStream baos = new ByteArrayOutputStream();
            StreamUtil.TransferBytes(stream, baos);
            ImageData img = new WmfImageData(baos.ToArray());
            canvas.AddImageFittedIntoRectangle(img, new Rectangle(0, 0, 1, 1), false);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(DESTINATION_FOLDER + "wmfImageTest05.pdf"
                , SOURCE_FOLDER + "cmp_wmfImageTest05.pdf", DESTINATION_FOLDER, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void GifImageTest01() {
            PdfDocument document = new PdfDocument(CompareTool.CreateTestPdfWriter(DESTINATION_FOLDER + "gifImageTest01.pdf"
                ));
            PdfPage page = document.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page);
            ImageData img = ImageDataFactory.Create(SOURCE_FOLDER + "2-frames.gif");
            canvas.AddImageFittedIntoRectangle(img, new Rectangle(100, 100, 200, 188.24f), false);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(DESTINATION_FOLDER + "gifImageTest01.pdf"
                , SOURCE_FOLDER + "cmp_gifImageTest01.pdf", DESTINATION_FOLDER, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void GifImageTest02() {
            PdfDocument document = new PdfDocument(CompareTool.CreateTestPdfWriter(DESTINATION_FOLDER + "gifImageTest02.pdf"
                ));
            PdfPage page = document.AddNewPage();
            Stream @is = FileUtil.GetInputStreamForFile(SOURCE_FOLDER + "2-frames.gif");
            ByteArrayOutputStream baos = new ByteArrayOutputStream();
            int reads = @is.Read();
            while (reads != -1) {
                baos.Write(reads);
                reads = @is.Read();
            }
            PdfCanvas canvas = new PdfCanvas(page);
            ImageData img = ImageDataFactory.CreateGifFrame(baos.ToArray(), 1);
            canvas.AddImageFittedIntoRectangle(img, new Rectangle(100, 100, 200, 188.24f), false);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(DESTINATION_FOLDER + "gifImageTest02.pdf"
                , SOURCE_FOLDER + "cmp_gifImageTest02.pdf", DESTINATION_FOLDER, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void GifImageTest03() {
            PdfDocument document = new PdfDocument(CompareTool.CreateTestPdfWriter(DESTINATION_FOLDER + "gifImageTest03.pdf"
                ));
            PdfPage page = document.AddNewPage();
            Stream @is = FileUtil.GetInputStreamForFile(SOURCE_FOLDER + "2-frames.gif");
            ByteArrayOutputStream baos = new ByteArrayOutputStream();
            int reads = @is.Read();
            while (reads != -1) {
                baos.Write(reads);
                reads = @is.Read();
            }
            PdfCanvas canvas = new PdfCanvas(page);
            ImageData img = ImageDataFactory.CreateGifFrame(baos.ToArray(), 2);
            canvas.AddImageFittedIntoRectangle(img, new Rectangle(100, 100, 200, 262.07f), false);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(DESTINATION_FOLDER + "gifImageTest03.pdf"
                , SOURCE_FOLDER + "cmp_gifImageTest03.pdf", DESTINATION_FOLDER, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void GifImageTest04() {
            PdfDocument document = new PdfDocument(CompareTool.CreateTestPdfWriter(DESTINATION_FOLDER + "gifImageTest04.pdf"
                ));
            PdfPage page = document.AddNewPage();
            Stream @is = FileUtil.GetInputStreamForFile(SOURCE_FOLDER + "2-frames.gif");
            ByteArrayOutputStream baos = new ByteArrayOutputStream();
            int reads = @is.Read();
            while (reads != -1) {
                baos.Write(reads);
                reads = @is.Read();
            }
            PdfCanvas canvas = new PdfCanvas(page);
            try {
                ImageDataFactory.CreateGifFrame(baos.ToArray(), 3);
                NUnit.Framework.Assert.Fail("IOException expected");
            }
            catch (iText.IO.Exceptions.IOException) {
            }
        }

        [NUnit.Framework.Test]
        public virtual void GifImageTest05() {
            PdfDocument document = new PdfDocument(CompareTool.CreateTestPdfWriter(DESTINATION_FOLDER + "gifImageTest05.pdf"
                ));
            PdfPage page = document.AddNewPage();
            Stream @is = FileUtil.GetInputStreamForFile(SOURCE_FOLDER + "animated_fox_dog.gif");
            ByteArrayOutputStream baos = new ByteArrayOutputStream();
            int reads = @is.Read();
            while (reads != -1) {
                baos.Write(reads);
                reads = @is.Read();
            }
            PdfCanvas canvas = new PdfCanvas(page);
            IList<ImageData> frames = ImageDataFactory.CreateGifFrames(baos.ToArray(), new int[] { 1, 2, 5 });
            float y = 600;
            foreach (ImageData img in frames) {
                canvas.AddImageFittedIntoRectangle(img, new Rectangle(100, y, 200, 159.72f), false);
                y -= 200;
            }
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(DESTINATION_FOLDER + "gifImageTest05.pdf"
                , SOURCE_FOLDER + "cmp_gifImageTest05.pdf", DESTINATION_FOLDER, "diff_"));
        }

        // Android-Conversion-Skip-Block-Start (java.awt library isn't available on Android)
        // Android-Conversion-Skip-Block-End
        [NUnit.Framework.Test]
        public virtual void CanvasInitializationPageNoContentsKey() {
            String srcFile = SOURCE_FOLDER + "pageNoContents.pdf";
            String cmpFile = SOURCE_FOLDER + "cmp_pageNoContentsStamp.pdf";
            String destFile = DESTINATION_FOLDER + "pageNoContentsStamp.pdf";
            PdfDocument document = new PdfDocument(new PdfReader(srcFile), CompareTool.CreateTestPdfWriter(destFile));
            PdfCanvas canvas = new PdfCanvas(document.GetPage(1));
            canvas.SetLineWidth(5).Rectangle(50, 680, 300, 50).Stroke();
            canvas.Release();
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destFile, cmpFile, DESTINATION_FOLDER, "diff_"
                ));
        }

        [NUnit.Framework.Test]
        public virtual void CanvasInitializationStampingExistingStream() {
            String srcFile = SOURCE_FOLDER + "pageWithContent.pdf";
            String cmpFile = SOURCE_FOLDER + "cmp_stampingExistingStream.pdf";
            String destFile = DESTINATION_FOLDER + "stampingExistingStream.pdf";
            PdfDocument document = new PdfDocument(new PdfReader(srcFile), CompareTool.CreateTestPdfWriter(destFile));
            PdfPage page = document.GetPage(1);
            PdfCanvas canvas = new PdfCanvas(page.GetLastContentStream(), page.GetResources(), page.GetDocument());
            canvas.SetLineWidth(5).Rectangle(50, 680, 300, 50).Stroke();
            canvas.Release();
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destFile, cmpFile, DESTINATION_FOLDER, "diff_"
                ));
        }

        [NUnit.Framework.Test]
        public virtual void CanvasStreamFlushedNoException() {
            PdfDocument doc = new PdfDocument(new PdfWriter(new ByteArrayOutputStream()));
            PdfStream stream = new _PdfStream_1123();
            stream.Put(PdfName.Filter, new PdfName("FlateDecode"));
            NUnit.Framework.Assert.DoesNotThrow(() => {
                new PdfCanvas(stream, new PdfResources(), doc);
            }
            );
        }

        private sealed class _PdfStream_1123 : PdfStream {
            public _PdfStream_1123() {
                this.isFlushed = false;
            }

            private bool isFlushed;

            public override bool IsFlushed() {
                System.Console.Out.WriteLine("isFlushed: " + this.isFlushed);
                if (this.isFlushed) {
                    return true;
                }
                this.isFlushed = true;
                return false;
            }
        }

        [NUnit.Framework.Test]
        public virtual void CanvasInitializationStampingExistingStreamMemoryLimitAware() {
            String srcFile = SOURCE_FOLDER + "pageWithContent.pdf";
            ReaderProperties properties = new ReaderProperties();
            MemoryLimitsAwareHandler handler = new _MemoryLimitsAwareHandler_1146();
            handler.SetMaxSizeOfSingleDecompressedPdfStream(1);
            properties.SetMemoryLimitsAwareHandler(handler);
            PdfDocument document = new PdfDocument(new PdfReader(srcFile, properties));
            PdfPage page = document.GetPage(1);
            NUnit.Framework.Assert.Catch(typeof(MemoryLimitsAwareException), () => {
                new PdfCanvas(page.GetLastContentStream(), page.GetResources(), page.GetDocument());
            }
            );
        }

        private sealed class _MemoryLimitsAwareHandler_1146 : MemoryLimitsAwareHandler {
            public _MemoryLimitsAwareHandler_1146() {
            }

            public override bool IsMemoryLimitsAwarenessRequiredOnDecompression(PdfArray filters) {
                return true;
            }
        }

        [NUnit.Framework.Test]
        public virtual void CanvasStampingJustCopiedStreamWithCompression() {
            String srcFile = SOURCE_FOLDER + "pageWithContent.pdf";
            String cmpFile = SOURCE_FOLDER + "cmp_stampingJustCopiedStreamWithCompression.pdf";
            String destFile = DESTINATION_FOLDER + "stampingJustCopiedStreamWithCompression.pdf";
            PdfDocument srcDocument = new PdfDocument(new PdfReader(srcFile));
            PdfDocument document = new PdfDocument(CompareTool.CreateTestPdfWriter(destFile));
            srcDocument.CopyPagesTo(1, 1, document);
            srcDocument.Close();
            PdfPage page = document.GetPage(1);
            PdfCanvas canvas = new PdfCanvas(page.GetLastContentStream(), page.GetResources(), page.GetDocument());
            canvas.SetLineWidth(5).Rectangle(50, 680, 300, 50).Stroke();
            canvas.Release();
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destFile, cmpFile, DESTINATION_FOLDER, "diff_"
                ));
        }

        [NUnit.Framework.Test]
        public virtual void CanvasSmallFontSize01() {
            String cmpFile = SOURCE_FOLDER + "cmp_canvasSmallFontSize01.pdf";
            String destFile = DESTINATION_FOLDER + "canvasSmallFontSize01.pdf";
            PdfDocument document = new PdfDocument(CompareTool.CreateTestPdfWriter(destFile));
            PdfPage page = document.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page);
            canvas.SaveState().BeginText().MoveText(50, 750).SetFontAndSize(PdfFontFactory.CreateFont(), 0).ShowText("simple text"
                ).EndText().RestoreState();
            canvas.SaveState().BeginText().MoveText(50, 700).SetFontAndSize(PdfFontFactory.CreateFont(), -0.00005f).ShowText
                ("simple text").EndText().RestoreState();
            canvas.SaveState().BeginText().MoveText(50, 650).SetFontAndSize(PdfFontFactory.CreateFont(), 0.00005f).ShowText
                ("simple text").EndText().RestoreState();
            canvas.SaveState().BeginText().MoveText(50, 600).SetFontAndSize(PdfFontFactory.CreateFont(), -12).ShowText
                ("simple text").EndText().RestoreState();
            canvas.SaveState().BeginText().MoveText(50, 550).SetFontAndSize(PdfFontFactory.CreateFont(), 12).ShowText(
                "simple text").EndText().RestoreState();
            canvas.Release();
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destFile, cmpFile, DESTINATION_FOLDER, "diff_"
                ));
        }

        [NUnit.Framework.Test]
        public virtual void AddWmfImageTest() {
            PdfDocument document = new PdfDocument(CompareTool.CreateTestPdfWriter(DESTINATION_FOLDER + "addWmfImage.pdf"
                ));
            PdfPage page = document.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page);
            ImageData img = new WmfImageData(SOURCE_FOLDER + "example2.wmf");
            canvas.AddImageAt(img, 0, 0, false);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(DESTINATION_FOLDER + "addWmfImage.pdf", SOURCE_FOLDER
                 + "cmp_addWmfImage.pdf", DESTINATION_FOLDER, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void SetLeadingPositiveTest() {
            String cmpPdf = SOURCE_FOLDER + "cmp_setLeadingPositive.pdf";
            String outPdf = DESTINATION_FOLDER + "setLeadingPositive.pdf";
            PdfDocument document = new PdfDocument(CompareTool.CreateTestPdfWriter(outPdf));
            PdfPage documentPage = document.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(documentPage);
            canvas.SaveState().BeginText().MoveText(50, 700).SetFontAndSize(PdfFontFactory.CreateFont(), 14).ShowText(
                "normal text one").NewlineShowText("normal text two").NewlineShowText("normal text three").EndText().RestoreState
                ();
            canvas.SaveState().BeginText().MoveText(50, 650).SetFontAndSize(PdfFontFactory.CreateFont(), 14).SetLeading
                (20.0f).ShowText("set leading text with positive value one").NewlineShowText("set leading text with positive value two"
                ).NewlineShowText("set leading text with positive value three").EndText().RestoreState();
            canvas.Release();
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, DESTINATION_FOLDER));
        }

        [NUnit.Framework.Test]
        public virtual void SetLeadingNegativeTest() {
            String cmpPdf = SOURCE_FOLDER + "cmp_setLeadingNegative.pdf";
            String outPdf = DESTINATION_FOLDER + "setLeadingNegative.pdf";
            PdfDocument document = new PdfDocument(CompareTool.CreateTestPdfWriter(outPdf));
            PdfPage documentPage = document.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(documentPage);
            canvas.SaveState().BeginText().MoveText(50, 700).SetFontAndSize(PdfFontFactory.CreateFont(), 14).ShowText(
                "normal text one").NewlineShowText("normal text two").NewlineShowText("normal text three").EndText().RestoreState
                ();
            canvas.SaveState().BeginText().MoveText(50, 650).SetFontAndSize(PdfFontFactory.CreateFont(), 14).SetLeading
                (-10.0f).ShowText("set leading text with negative value one").NewlineShowText("set leading text with negative value two"
                ).NewlineShowText("set leading text with negative value three").EndText().RestoreState();
            canvas.Release();
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, DESTINATION_FOLDER));
        }

        [NUnit.Framework.Test]
        public virtual void WrongLengthOfTransMatrixTest() {
            //TODO DEVSIX-6486 Transformation matrix of wrong length are processed without any warning
            String cmpPdf = SOURCE_FOLDER + "cmp_wrongLengthOfTransMatrix.pdf";
            String outPdf = DESTINATION_FOLDER + "wrongLengthOfTransMatrix.pdf";
            PdfDocument document = new PdfDocument(CompareTool.CreateTestPdfWriter(outPdf));
            PdfPage documentPage = document.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(documentPage);
            PdfArray wrongNumberOfTransMatrix = new PdfArray(new int[] { 1, 0, 0, 1, 100 });
            canvas.SaveState().BeginText().ConcatMatrix(wrongNumberOfTransMatrix).SetFontAndSize(PdfFontFactory.CreateFont
                (), 14).ShowText("Hello World").EndText().RestoreState();
            canvas.Release();
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, DESTINATION_FOLDER));
        }

        [NUnit.Framework.Test]
        public virtual void ConcatMatrixPdfArrayTest() {
            String cmpPdf = SOURCE_FOLDER + "cmp_concatMatrixPdfArray.pdf";
            String outPdf = DESTINATION_FOLDER + "concatMatrixPdfArray.pdf";
            PdfDocument document = new PdfDocument(CompareTool.CreateTestPdfWriter(outPdf));
            PdfPage documentPage = document.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(documentPage);
            PdfArray arrayTransformationMatrix = new PdfArray(new int[] { 3, 1, 1, 3, 50, 700 });
            canvas.SaveState().BeginText().ConcatMatrix(arrayTransformationMatrix).SetFontAndSize(PdfFontFactory.CreateFont
                (), 14).ShowText("hello world").EndText().RestoreState();
            canvas.Release();
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, DESTINATION_FOLDER));
        }

        [NUnit.Framework.Test]
        public virtual void SetMoveTextWithLeadingTest() {
            String cmpPdf = SOURCE_FOLDER + "cmp_setMoveTextWithLeading.pdf";
            String outPdf = DESTINATION_FOLDER + "setMoveTextWithLeading.pdf";
            PdfDocument document = new PdfDocument(CompareTool.CreateTestPdfWriter(outPdf));
            PdfPage documentPage = document.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(documentPage);
            canvas.SaveState().BeginText().MoveText(50, 700).SetFontAndSize(PdfFontFactory.CreateFont(), 14).ShowText(
                "normal text one").NewlineShowText("normal text two").NewlineShowText("normal text three").EndText().RestoreState
                ();
            canvas.SaveState().BeginText().MoveText(50, 700).SetFontAndSize(PdfFontFactory.CreateFont(), 14).MoveTextWithLeading
                (0, -200).ShowText("move text with leading one").NewlineShowText("move text with leading two").NewlineShowText
                ("move text with leading three").EndText().RestoreState();
            canvas.Release();
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, DESTINATION_FOLDER));
        }

        [NUnit.Framework.Test]
        public virtual void SetNewLineTextTest() {
            String cmpPdf = SOURCE_FOLDER + "cmp_setNewLineText.pdf";
            String outPdf = DESTINATION_FOLDER + "setNewLineText.pdf";
            PdfDocument document = new PdfDocument(CompareTool.CreateTestPdfWriter(outPdf));
            PdfPage documentPage = document.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(documentPage);
            canvas.SaveState().BeginText().MoveText(50, 700).SetFontAndSize(PdfFontFactory.CreateFont(), 14).ShowText(
                "text before").EndText().RestoreState();
            canvas.SaveState().BeginText().MoveText(50, 700).SetFontAndSize(PdfFontFactory.CreateFont(), 14).SetLeading
                (10f).NewlineText().ShowText("text after").EndText().RestoreState();
            canvas.Release();
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, DESTINATION_FOLDER));
        }

        [NUnit.Framework.Test]
        public virtual void SetPositiveTextRiseValueTest() {
            String cmpPdf = SOURCE_FOLDER + "cmp_setPositiveTextRiseValue.pdf";
            String outPdf = DESTINATION_FOLDER + "setPositiveTextRiseValue.pdf";
            PdfDocument document = new PdfDocument(CompareTool.CreateTestPdfWriter(outPdf));
            PdfPage documentPage = document.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(documentPage);
            canvas.SaveState().BeginText().MoveText(100, 700).SetFontAndSize(PdfFontFactory.CreateFont(), 14).ShowText
                ("normal text").EndText().RestoreState();
            canvas.SaveState().BeginText().MoveText(100, 700).SetFontAndSize(PdfFontFactory.CreateFont(), 14).SetTextRise
                (10f).ShowText("rise text positive value").EndText().RestoreState();
            canvas.Release();
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, DESTINATION_FOLDER));
        }

        [NUnit.Framework.Test]
        public virtual void SetNegativeTextRiseValueTest() {
            String cmpPdf = SOURCE_FOLDER + "cmp_setNegativeTextRiseValue.pdf";
            String outPdf = DESTINATION_FOLDER + "setNegativeTextRiseValue.pdf";
            PdfDocument document = new PdfDocument(CompareTool.CreateTestPdfWriter(outPdf));
            PdfPage documentPage = document.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(documentPage);
            canvas.SaveState().BeginText().MoveText(100, 700).SetFontAndSize(PdfFontFactory.CreateFont(), 14).ShowText
                ("normal text").EndText().RestoreState();
            canvas.SaveState().BeginText().MoveText(100, 700).SetFontAndSize(PdfFontFactory.CreateFont(), 14).SetTextRise
                (-10f).ShowText("rise text negative value").EndText().RestoreState();
            canvas.Release();
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, DESTINATION_FOLDER));
        }

        [NUnit.Framework.Test]
        public virtual void SetPositiveWordSpacingValueTest() {
            String cmpPdf = SOURCE_FOLDER + "cmp_setPositiveWordSpacingValue.pdf";
            String outPdf = DESTINATION_FOLDER + "setPositiveWordSpacingValue.pdf";
            PdfDocument document = new PdfDocument(CompareTool.CreateTestPdfWriter(outPdf));
            PdfPage documentPage = document.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(documentPage);
            canvas.SaveState().BeginText().MoveText(50, 700).SetFontAndSize(PdfFontFactory.CreateFont(), 14).ShowText(
                "normal text").EndText().RestoreState();
            canvas.SaveState().BeginText().SetFontAndSize(PdfFontFactory.CreateFont(), 14).MoveText(50, 650).SetWordSpacing
                (20f).ShowText("positive word spacing test").EndText().RestoreState();
            canvas.Release();
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, DESTINATION_FOLDER));
        }

        [NUnit.Framework.Test]
        public virtual void SetNegativeWordSpacingValueTest() {
            String cmpPdf = SOURCE_FOLDER + "cmp_setNegativeWordSpacingValue.pdf";
            String outPdf = DESTINATION_FOLDER + "setNegativeWordSpacingValue.pdf";
            PdfDocument document = new PdfDocument(CompareTool.CreateTestPdfWriter(outPdf));
            PdfPage documentPage = document.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(documentPage);
            canvas.SaveState().BeginText().MoveText(50, 700).SetFontAndSize(PdfFontFactory.CreateFont(), 14).ShowText(
                "normal text").EndText().RestoreState();
            canvas.SaveState().BeginText().SetFontAndSize(PdfFontFactory.CreateFont(), 14).MoveText(50, 650).SetWordSpacing
                (-5f).ShowText("negative word spacing test").EndText().RestoreState();
            canvas.Release();
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, DESTINATION_FOLDER));
        }

        [NUnit.Framework.Test]
        public virtual void SetPositiveCharSpacingValueTest() {
            String cmpPdf = SOURCE_FOLDER + "cmp_setPositiveCharSpacingValue.pdf";
            String outPdf = DESTINATION_FOLDER + "setPositiveCharSpacingValue.pdf";
            PdfDocument document = new PdfDocument(CompareTool.CreateTestPdfWriter(outPdf));
            PdfPage documentPage = document.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(documentPage);
            canvas.SaveState().BeginText().MoveText(50, 700).SetFontAndSize(PdfFontFactory.CreateFont(), 14).ShowText(
                "normal text").EndText().RestoreState();
            canvas.SaveState().BeginText().SetFontAndSize(PdfFontFactory.CreateFont(), 14).MoveText(50, 650).SetCharacterSpacing
                (5f).ShowText("positive char spacing test").EndText();
            canvas.Release();
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, DESTINATION_FOLDER));
        }

        [NUnit.Framework.Test]
        public virtual void SetNegativeCharSpacingValueTest() {
            String cmpPdf = SOURCE_FOLDER + "cmp_setNegativeCharSpacingValue.pdf";
            String outPdf = DESTINATION_FOLDER + "setNegativeCharSpacingValue.pdf";
            PdfDocument document = new PdfDocument(CompareTool.CreateTestPdfWriter(outPdf));
            PdfPage documentPage = document.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(documentPage);
            canvas.SaveState().BeginText().MoveText(50, 700).SetFontAndSize(PdfFontFactory.CreateFont(), 14).ShowText(
                "normal text").EndText().RestoreState();
            canvas.SaveState().BeginText().SetFontAndSize(PdfFontFactory.CreateFont(), 14).MoveText(50, 650).SetCharacterSpacing
                (-1f).ShowText("negative char spacing test").EndText();
            canvas.Release();
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, DESTINATION_FOLDER));
        }

        [NUnit.Framework.Test]
        public virtual void SetNegativeHorizontalScalingValueTest() {
            String cmpPdf = SOURCE_FOLDER + "cmp_setNegativeHorizontalScalingValue.pdf";
            String outPdf = DESTINATION_FOLDER + "setNegativeHorizontalScalingValue.pdf";
            PdfDocument document = new PdfDocument(CompareTool.CreateTestPdfWriter(outPdf));
            PdfPage documentPage = document.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(documentPage);
            canvas.SaveState().BeginText().MoveText(50, 700).SetFontAndSize(PdfFontFactory.CreateFont(), 14).ShowText(
                "normal text").EndText().RestoreState();
            canvas.SaveState().BeginText().SetFontAndSize(PdfFontFactory.CreateFont(), 14).MoveText(50, 650).SetHorizontalScaling
                (-10f).ShowText("negative horizontal scaling").EndText();
            canvas.Release();
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, DESTINATION_FOLDER));
        }

        [NUnit.Framework.Test]
        public virtual void SetPositiveHorizontalScalingValueTest() {
            String cmpPdf = SOURCE_FOLDER + "cmp_setPositiveHorizontalScalingValue.pdf";
            String outPdf = DESTINATION_FOLDER + "setPositiveHorizontalScalingValue.pdf";
            PdfDocument document = new PdfDocument(CompareTool.CreateTestPdfWriter(outPdf));
            PdfPage documentPage = document.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(documentPage);
            canvas.SaveState().BeginText().MoveText(50, 700).SetFontAndSize(PdfFontFactory.CreateFont(), 14).ShowText(
                "normal text").EndText().RestoreState();
            canvas.SaveState().BeginText().SetFontAndSize(PdfFontFactory.CreateFont(), 14).MoveText(50, 650).SetHorizontalScaling
                (10f).ShowText("positive horizontal scaling").EndText();
            canvas.Release();
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, DESTINATION_FOLDER));
        }

        [NUnit.Framework.Test]
        public virtual void CreateSimpleCanvasWithPdfArrayText() {
            String outPdf = DESTINATION_FOLDER + "createSimpleCanvasWithPdfArrayText.pdf";
            String cmpPdf = SOURCE_FOLDER + "cmp_createSimpleCanvasWithPdfArrayText.pdf";
            PdfDocument pdfDoc = new PdfDocument(CompareTool.CreateTestPdfWriter(outPdf));
            PdfPage page1 = pdfDoc.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page1);
            PdfArray pdfArray = new PdfArray();
            pdfArray.Add(new PdfString("ABC"));
            pdfArray.Add(new PdfNumber(-250));
            pdfArray.Add(new PdfString("DFG"));
            //Initialize canvas and write text to it
            canvas.SaveState().BeginText().MoveText(36, 750).SetFontAndSize(PdfFontFactory.CreateFont(StandardFonts.HELVETICA
                ), 16).ShowText(pdfArray).EndText().RestoreState();
            canvas.Release();
            pdfDoc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, DESTINATION_FOLDER, "diff_"
                ));
        }

        [NUnit.Framework.Test]
        public virtual void IgnorePageRotationForContentTest() {
            String outPdf = DESTINATION_FOLDER + "ignorePageRotationForContent.pdf";
            String cmpPdf = SOURCE_FOLDER + "cmp_ignorePageRotationForContent.pdf";
            using (PdfDocument pdfDoc = new PdfDocument(CompareTool.CreateTestPdfWriter(outPdf))) {
                pdfDoc.GetDocumentInfo().SetAuthor(AUTHOR).SetCreator(CREATOR).SetTitle(TITLE);
                PdfPage page = pdfDoc.AddNewPage().SetRotation(270);
                // When "true": in case the page has a rotation, then new content will be automatically rotated in the
                // opposite direction. On the rotated page this would look as if new content ignores page rotation.
                page.SetIgnorePageRotationForContent(true);
                PdfCanvas canvas = new PdfCanvas(page, false);
                canvas.SaveState().BeginText().MoveText(180, 350).SetFontAndSize(PdfFontFactory.CreateFont(StandardFonts.HELVETICA
                    ), 30).ShowText("Page rotation is set to 270 degrees,").EndText().RestoreState();
                PdfCanvas canvas2 = new PdfCanvas(page, false);
                canvas2.SaveState().BeginText().MoveText(180, 250).SetFontAndSize(PdfFontFactory.CreateFont(StandardFonts.
                    HELVETICA), 30).ShowText("but new content ignores page rotation").EndText().RestoreState();
                page.Flush();
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, DESTINATION_FOLDER, "diff_"
                ));
        }

        [NUnit.Framework.Test]
        public virtual void GetResourcesTest() {
            String outPdf = DESTINATION_FOLDER + "getResourcesDoc.pdf";
            PdfDocument pdfDoc = new PdfDocument(CompareTool.CreateTestPdfWriter(outPdf));
            PdfPage page1 = pdfDoc.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page1);
            canvas.SaveState().BeginText().MoveText(150, 400).SetFontAndSize(PdfFontFactory.CreateFont(), 8).ShowText(
                "test text").EndText().RestoreState();
            PdfResources resources = canvas.GetResources();
            pdfDoc.Close();
            NUnit.Framework.Assert.AreEqual(1, resources.GetResourceNames().Count);
        }

        [NUnit.Framework.Test]
        public virtual void AttachContentStreamTest() {
            String outPdf = DESTINATION_FOLDER + "attachContentStreamDoc.pdf";
            PdfDocument pdfDoc = new PdfDocument(CompareTool.CreateTestPdfWriter(outPdf));
            PdfPage page1 = pdfDoc.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page1);
            canvas.AttachContentStream(new PdfStream("test".GetBytes(System.Text.Encoding.UTF8)));
            String contentFromStream = iText.Commons.Utils.JavaUtil.GetStringForBytes(canvas.GetContentStream().GetBytes
                (), System.Text.Encoding.UTF8);
            pdfDoc.Close();
            NUnit.Framework.Assert.AreEqual("test", contentFromStream);
        }

        [NUnit.Framework.Test]
        public virtual void GraphicStateFontNullTest() {
            String outPdf = DESTINATION_FOLDER + "showTextDoc.pdf";
            using (PdfDocument pdfDoc = new PdfDocument(CompareTool.CreateTestPdfWriter(outPdf))) {
                PdfPage page1 = pdfDoc.AddNewPage();
                PdfCanvas canvas = new PdfCanvas(page1);
                GlyphLine glyphLine = new GlyphLine();
                canvas.GetGraphicsState().SetFont(null);
                ActualTextIterator actualTextIterator = new ActualTextIterator(glyphLine);
                NUnit.Framework.Assert.Catch(typeof(PdfException), () => canvas.ShowText(glyphLine, actualTextIterator));
            }
        }

        [NUnit.Framework.Test]
        public virtual void GlyphlineActualTextTest() {
            String outFileName = DESTINATION_FOLDER + "glyphlineActualText.pdf";
            using (PdfDocument pdfDocument = new PdfDocument(CompareTool.CreateTestPdfWriter(outFileName))) {
                PdfFont font = PdfFontFactory.CreateFont(FONTS_FOLDER + "NotoSansCJKjp-Bold.otf", PdfEncodings.IDENTITY_H);
                IList<Glyph> glyphs = JavaCollectionsUtil.SingletonList(font.GetGlyph((int)'\u65E0'));
                GlyphLine glyphLine = new GlyphLine(glyphs);
                glyphLine.SetActualText(0, 1, "TEST");
                PdfCanvas canvas = new PdfCanvas(pdfDocument.AddNewPage());
                canvas.SaveState().BeginText().SetFontAndSize(font, 7).ShowText(glyphLine).EndText().RestoreState();
                String contentstream = iText.Commons.Utils.JavaUtil.GetStringForBytes(canvas.GetContentStream().GetBytes()
                    , System.Text.Encoding.UTF8);
                canvas.Release();
                NUnit.Framework.Assert.IsTrue(contentstream.Contains("/ActualText"));
            }
        }

        private void CreateStandardDocument(PdfWriter writer, int pageCount, PdfCanvasTest.ContentProvider contentProvider
            ) {
            PdfDocument pdfDoc = new PdfDocument(writer);
            pdfDoc.GetDocumentInfo().SetAuthor(AUTHOR).SetCreator(CREATOR).SetTitle(TITLE);
            for (int i = 0; i < pageCount; i++) {
                PdfPage page = pdfDoc.AddNewPage();
                PdfCanvas canvas = new PdfCanvas(page);
                contentProvider.DrawOnCanvas(canvas, i);
                canvas.Release();
                page.Flush();
            }
            pdfDoc.Close();
        }

        private void AssertStandardDocument(String filename, int pageCount) {
            PdfReader reader = CompareTool.CreateOutputReader(filename);
            PdfDocument pdfDocument = new PdfDocument(reader);
            NUnit.Framework.Assert.IsFalse(reader.HasRebuiltXref(), "Rebuilt");
            PdfDictionary info = pdfDocument.GetTrailer().GetAsDictionary(PdfName.Info);
            NUnit.Framework.Assert.AreEqual(AUTHOR, info.Get(PdfName.Author).ToString(), "Author");
            NUnit.Framework.Assert.AreEqual(CREATOR, info.Get(PdfName.Creator).ToString(), "Creator");
            NUnit.Framework.Assert.AreEqual(TITLE, info.Get(PdfName.Title).ToString(), "Title");
            NUnit.Framework.Assert.AreEqual(pageCount, pdfDocument.GetNumberOfPages(), "Page count");
            for (int i = 1; i <= pageCount; i++) {
                PdfDictionary page = pdfDocument.GetPage(i).GetPdfObject();
                NUnit.Framework.Assert.AreEqual(PdfName.Page, page.Get(PdfName.Type));
            }
            pdfDocument.Close();
        }

        [FunctionalInterfaceAttribute]
        private interface ContentProvider {
            void DrawOnCanvas(PdfCanvas canvas, int pageNumber);
        }
    }
}
