/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 Apryse Group NV
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
using iText.Kernel.Colors;
using iText.Kernel.Events;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Layer;
using iText.Kernel.Utils;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Logs;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Layout {
    [NUnit.Framework.Category("IntegrationTest")]
    public class DefaultLayoutTest : ExtendedITextTest {
        public static float EPS = 0.001f;

        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/layout/DefaultLayoutTest/";

        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/layout/DefaultLayoutTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateDestinationFolder(destinationFolder);
        }

        [NUnit.Framework.Test]
        public virtual void MultipleAdditionsOfSameModelElementTest() {
            String outFileName = destinationFolder + "multipleAdditionsOfSameModelElementTest1.pdf";
            String cmpFileName = sourceFolder + "cmp_multipleAdditionsOfSameModelElementTest1.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            pdfDocument.SetTagged();
            Document document = new Document(pdfDocument);
            Paragraph p = new Paragraph("Hello. I am a paragraph. I want you to process me correctly");
            document.Add(p).Add(p).Add(new AreaBreak(PageSize.DEFAULT)).Add(p);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void RendererTest01() {
            String outFileName = destinationFolder + "rendererTest01.pdf";
            String cmpFileName = sourceFolder + "cmp_rendererTest01.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document document = new Document(pdfDocument);
            String str = "Hello. I am a fairly long paragraph. I really want you to process me correctly. You heard that? Correctly!!! Even if you will have to wrap me.";
            document.Add(new Paragraph(new Text(str).SetBackgroundColor(ColorConstants.RED)).SetBackgroundColor(ColorConstants
                .GREEN)).Add(new Paragraph(str)).Add(new AreaBreak(PageSize.DEFAULT)).Add(new Paragraph(str));
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void EmptyParagraphsTest01() {
            String outFileName = destinationFolder + "emptyParagraphsTest01.pdf";
            String cmpFileName = sourceFolder + "cmp_emptyParagraphsTest01.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document document = new Document(pdfDocument);
            document.Add(new Paragraph());
            // this line should not cause any effect
            document.Add(new Paragraph().SetBackgroundColor(ColorConstants.GREEN));
            document.Add(new Paragraph().SetBorder(new SolidBorder(ColorConstants.BLUE, 3)));
            document.Add(new Paragraph("Hello! I'm the first paragraph added to the document. Am i right?").SetBackgroundColor
                (ColorConstants.RED).SetBorder(new SolidBorder(1)));
            document.Add(new Paragraph().SetHeight(50));
            document.Add(new Paragraph("Hello! I'm the second paragraph added to the document. Am i right?"));
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void EmptyParagraphsTest02() {
            String outFileName = destinationFolder + "emptyParagraphsTest02.pdf";
            String cmpFileName = sourceFolder + "cmp_emptyParagraphsTest02.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document document = new Document(pdfDocument);
            document.Add(new Paragraph("Hello, i'm the text of the first paragraph on the first line. Let's break me and meet on the next line!\nSee? I'm on the second line. Now let's create some empty lines,\n for example one\n\nor two\n\n\nor three\n\n\n\nNow let's do something else"
                ));
            document.Add(new Paragraph("\n\n\nLook, i'm the the text of the second paragraph. But before me and the first one there are three empty lines!"
                ));
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void TextWithWhitespacesTest01() {
            String outFileName = destinationFolder + "textWithWhitespacesTest01.pdf";
            String cmpFileName = sourceFolder + "cmp_textWithWhitespacesTest01.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDoc);
            doc.Add(new Paragraph("Test non-breaking spaces"));
            doc.Add(new Paragraph("\u00a0\u00a0\u00a0\u00a0test test"));
            doc.Add(new Paragraph("test test\u00a0\u00a0\u00a0\u00a0test test"));
            doc.Add(new Paragraph("Test usual spaces"));
            doc.Add(new Paragraph("\u0020\u0020\u0020\u0020test test"));
            doc.Add(new Paragraph("test test\u0020\u0020\u0020\u0020test test"));
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        [NUnit.Framework.Test]
        [LogMessage(LayoutLogMessageConstant.ELEMENT_DOES_NOT_FIT_AREA, Count = 1)]
        public virtual void AddParagraphOnShortPage1() {
            String outFileName = destinationFolder + "addParagraphOnShortPage1.pdf";
            String cmpFileName = sourceFolder + "cmp_addParagraphOnShortPage1.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDoc, new PageSize(500, 70));
            Paragraph p = new Paragraph();
            p.Add("AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA");
            p.Add(new Text("BBB").SetFontSize(30));
            p.Add("CCC");
            p.Add("DDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDD");
            p.Add("EEE");
            doc.Add(p);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        [NUnit.Framework.Test]
        [LogMessage(LayoutLogMessageConstant.ELEMENT_DOES_NOT_FIT_AREA)]
        public virtual void AddParagraphOnShortPage2() {
            String outFileName = destinationFolder + "addParagraphOnShortPage2.pdf";
            String cmpFileName = sourceFolder + "cmp_addParagraphOnShortPage2.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDoc, new PageSize(300, 50));
            Paragraph p = new Paragraph();
            p.Add("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa"
                );
            doc.Add(p);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        [NUnit.Framework.Test]
        [LogMessage(LayoutLogMessageConstant.ELEMENT_DOES_NOT_FIT_AREA)]
        public virtual void AddWordOnShortPageTest01() {
            String outFileName = destinationFolder + "addWordOnShortPageTest01.pdf";
            String cmpFileName = sourceFolder + "cmp_addWordOnShortPageTest01.pdf";
            // Default font size
            float defaultFontSize = 12;
            // Use the default font to get the width which will be occupied by two letters
            float contentWidth = PdfFontFactory.CreateFont().GetWidth("he", defaultFontSize);
            // Not enough height to place letters without FORCED_PLACEMENT
            float shortHeight = 15;
            // The sum of either top and bottom page margins, or left and right page margins
            float margins = 36 + 36;
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDoc, new PageSize(margins + contentWidth + EPS, margins + shortHeight));
            Paragraph p = new Paragraph("hello");
            // The area's height is not enough to place the paragraph.
            // The area's width is enough to place 2 characters.
            doc.Add(p);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void CloseEmptyDocumentTest() {
            String outFileName = destinationFolder + "closeEmptyDocumentTest.pdf";
            String cmpFileName = sourceFolder + "cmp_closeEmptyDocumentTest.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document document = new Document(pdfDocument);
            NUnit.Framework.Assert.DoesNotThrow(() => document.Close());
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void CloseEmptyDocumentWithEventOnAddingPageTest() {
            String outFileName = destinationFolder + "closeEmptyDocumentWithEventTest.pdf";
            String cmpFileName = sourceFolder + "cmp_closeEmptyDocumentWithEventTest.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            new PdfLayer("Some layer", pdfDocument);
            DefaultLayoutTest.ParagraphAdderHandler handler = new DefaultLayoutTest.ParagraphAdderHandler();
            pdfDocument.AddEventHandler(PdfDocumentEvent.START_PAGE, handler);
            NUnit.Framework.Assert.DoesNotThrow(() => pdfDocument.Close());
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void CheckPageSizeOfClosedEmptyDocumentTest() {
            MemoryStream baos = new MemoryStream();
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(baos));
            NUnit.Framework.Assert.DoesNotThrow(() => pdfDocument.Close());
            byte[] bytes = baos.ToArray();
            baos.Dispose();
            PdfDocument newDoc = new PdfDocument(new PdfReader(new MemoryStream(bytes)));
            NUnit.Framework.Assert.IsTrue(PageSize.DEFAULT.EqualsWithEpsilon(newDoc.GetPage(1).GetPageSize()));
            newDoc.Close();
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.ATTEMPT_TO_GENERATE_PDF_PAGES_TREE_WITHOUT_ANY_PAGES, LogLevel
             = LogLevelConstants.INFO)]
        public virtual void CloseEmptyDocumentWithRemovingPageEventOnAddingPageTest() {
            String outFileName = destinationFolder + "closeEmptyDocumentWithRemovingEventTest.pdf";
            String cmpFileName = sourceFolder + "cmp_closeEmptyDocumentWithRemovingEventTest.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            DefaultLayoutTest.PageRemoverHandler handler = new DefaultLayoutTest.PageRemoverHandler();
            pdfDocument.AddEventHandler(PdfDocumentEvent.START_PAGE, handler);
            NUnit.Framework.Assert.DoesNotThrow(() => pdfDocument.Close());
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        private class ParagraphAdderHandler : iText.Kernel.Events.IEventHandler {
            public virtual void HandleEvent(Event @event) {
                PdfDocumentEvent docEvent = (PdfDocumentEvent)@event;
                PdfPage page = docEvent.GetPage();
                PdfDocument pdfDoc = ((PdfDocumentEvent)@event).GetDocument();
                IList<PdfLayer> group = new List<PdfLayer>();
                group.Add(new PdfLayer("Some second layer", pdfDoc));
                // If page will be added in PdfPagesTree#generateTree method, after flushing PdfOCProperties,
                // exception will be thrown, but page will be added before anu flushing, and there is no exception
                pdfDoc.GetCatalog().GetOCProperties(false).AddOCGRadioGroup(group);
                PdfCanvas canvas = new PdfCanvas(page.NewContentStreamBefore(), page.GetResources(), pdfDoc);
                new iText.Layout.Canvas(canvas, new Rectangle(0, 0, 600, 600)).Add(new Paragraph("Some text").SetFixedPosition
                    (100, 100, 100));
            }
        }

        private class PageRemoverHandler : iText.Kernel.Events.IEventHandler {
            public virtual void HandleEvent(Event @event) {
                PdfDocumentEvent docEvent = (PdfDocumentEvent)@event;
                PdfPage page = docEvent.GetPage();
                PdfDocument pdfDoc = ((PdfDocumentEvent)@event).GetDocument();
                pdfDoc.RemovePage(1);
            }
        }
    }
}
