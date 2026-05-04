/*
This file is part of the iText (R) project.
Copyright (c) 1998-2026 Apryse Group NV
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
using iText.IO.Image;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Data;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using iText.Kernel.Utils;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Logs;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Layout.Renderer {
    [NUnit.Framework.Category("IntegrationTest")]
    public class TableMultiPageRenderingTest : ExtendedITextTest {
        private static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/layout/TableMultiPageRenderingTest/";

        private static readonly String DESTINATION_FOLDER = TestUtil.GetOutputPath() + "/layout/TableMultiPageRenderingTest/";

        private const float MARGIN = 36f;

        private static readonly PageSize SHORT_PAGE = new PageSize(PageSize.A4.GetWidth(), 180);

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateDestinationFolder(DESTINATION_FOLDER);
        }

        [NUnit.Framework.Test]
        public virtual void TableStartsNearBottomTest() {
            String fileName = "tableBottomPage.pdf";
            String destPath = DESTINATION_FOLDER + fileName;
            Document document = CreateA4Document(destPath);
            AddFiller(document, 88);
            Table table = new Table(new float[] { 1, 3 });
            table.SetWidth(PageSize.A4.GetWidth() - 2 * MARGIN);
            table.AddHeaderCell(new Cell().Add(new Paragraph("ID")));
            table.AddHeaderCell(new Cell().Add(new Paragraph("Description")));
            for (int i = 1; i <= 20; i++) {
                table.AddCell(new Cell().Add(new Paragraph(i.ToString())));
                table.AddCell(new Cell().Add(new Paragraph("Row " + i + " - some longer content to make the table taller."
                    )));
            }
            document.Add(table);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destPath, SOURCE_FOLDER + "cmp_" + fileName
                , DESTINATION_FOLDER));
        }

        [NUnit.Framework.Test]
        public virtual void TableWithIncompleteRowStartsNearBottomTest() {
            String fileName = "tableLong2ndCellBottomPage.pdf";
            String destPath = DESTINATION_FOLDER + fileName;
            Document document = CreateA4Document(destPath);
            AddFiller(document, 88);
            Table table = new Table(new float[] { 1, 3 });
            table.SetWidth(PageSize.A4.GetWidth() - 2 * MARGIN);
            table.AddHeaderCell(new Cell().Add(new Paragraph("ID")));
            table.AddHeaderCell(new Cell().Add(new Paragraph("Description")));
            table.AddCell(new Cell().Add(new Paragraph("Lorem ipsum dolor sit amet, consectetur adipiscing elit. Nam tincidunt urna "
                 + "vel massa iaculis, ultrices posuere ex iaculis. Ut dignissim imperdiet " + "libero sit amet eleifend. Nulla congue porta mi, et cursus lorem iaculis "
                 + "eget. Vestibulum ante ipsum primis in faucibus orci luctus et ultrices " + "posuere cubilia curae; Fusce commodo elementum massa eu euismod."
                )));
            table.AddCell(new Cell().Add(new Paragraph("0")));
            for (int i = 1; i <= 20; i++) {
                table.AddCell(new Cell().Add(new Paragraph(i.ToString())));
                table.AddCell(new Cell().Add(new Paragraph("Row " + i + " - some longer content to make the table taller."
                    )));
            }
            document.Add(table);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destPath, SOURCE_FOLDER + "cmp_" + fileName
                , DESTINATION_FOLDER));
        }

        [NUnit.Framework.Test]
        public virtual void ImageRowFlowsToSecondPageTest() {
            String fileName = "tableWithImageStartsBottomPage.pdf";
            String destPath = DESTINATION_FOLDER + fileName;
            Document document = CreateA4Document(destPath);
            AddFiller(document, 85);
            document.Add(CreateBaseImageTable(false));
            document.Close();
            NUnit.Framework.Assert.IsTrue(PageContainsImage(destPath, 2));
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destPath, SOURCE_FOLDER + "cmp_" + fileName
                , DESTINATION_FOLDER));
        }

        //TODO DEVSIX-7410: Fix test after fix
        [NUnit.Framework.Test]
        [LogMessage(LayoutLogMessageConstant.ELEMENT_DOES_NOT_FIT_AREA)]
        public virtual void ImageDoesNotFitOnShortFirstPageTest() {
            String fileName = "tableImageShortPage.pdf";
            String destPath = DESTINATION_FOLDER + fileName;
            Document document = CreateShortFirstPageDocument(destPath);
            document.Add(CreateBaseImageTable(false));
            document.GetPdfDocument().SetDefaultPageSize(PageSize.A4);
            document.Close();
            NUnit.Framework.Assert.IsTrue(PageContainsImage(destPath, 1));
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destPath, SOURCE_FOLDER + "cmp_" + fileName
                , DESTINATION_FOLDER));
        }

        //TODO DEVSIX-7410: Fix cmp after fix
        [NUnit.Framework.Test]
        [LogMessage(LayoutLogMessageConstant.ELEMENT_DOES_NOT_FIT_AREA)]
        public virtual void ImageDoesNotFitOnShortFirstPageTest2() {
            String fileName = "tableImageShortPage2.pdf";
            String destPath = DESTINATION_FOLDER + fileName;
            Document document = CreateShortFirstPageDocument(destPath, true);
            Table table = CreateBaseImageTable(false);
            table.AddCell(new Cell().Add(new Paragraph("Testing.")));
            document.Add(table);
            document.Close();
            NUnit.Framework.Assert.IsTrue(PageContainsImage(destPath, 1));
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destPath, SOURCE_FOLDER + "cmp_" + fileName
                , DESTINATION_FOLDER));
        }

        //TODO DEVSIX-7410: Fix cmp after fix
        [NUnit.Framework.Test]
        [LogMessage(LayoutLogMessageConstant.ELEMENT_DOES_NOT_FIT_AREA)]
        public virtual void ThirdRowImageFlowsToSecondPageTest() {
            String fileName = "extraRowImageToSecondPageTest.pdf";
            String destPath = DESTINATION_FOLDER + fileName;
            Document document = CreateShortFirstPageDocument(destPath);
            Table table = new Table(1);
            table.SetWidth(PageSize.A4.GetWidth() - 2 * MARGIN);
            table.AddHeaderCell(CreateHeaderCell());
            table.AddCell(new Cell().Add(new Paragraph("Extra cell first.")));
            table.AddCell(new Cell().Add(new Image(ImageDataFactory.Create(SOURCE_FOLDER + "itis.jpg"))));
            document.Add(table);
            document.Close();
            NUnit.Framework.Assert.IsTrue(PageContainsImage(destPath, 2));
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destPath, SOURCE_FOLDER + "cmp_" + fileName
                , DESTINATION_FOLDER));
        }

        //TODO DEVSIX-7410: Fix cmp after rix
        [NUnit.Framework.Test]
        public virtual void ThirdRowImageFlowsToSecondPageA4Test() {
            String fileName = "extraRowImageToSecondA4PageTest.pdf";
            String destPath = DESTINATION_FOLDER + fileName;
            Document document = CreateShortFirstPageDocument(destPath, true);
            Table table = new Table(1);
            table.SetWidth(PageSize.A4.GetWidth() - 2 * MARGIN);
            table.AddHeaderCell(CreateHeaderCell());
            table.AddCell(new Cell().Add(new Paragraph("Extra cell first.")));
            table.AddCell(new Cell().Add(new iText.Layout.Element.Image(ImageDataFactory.Create(SOURCE_FOLDER + "itis.jpg"
                ))));
            document.Add(table);
            document.Close();
            NUnit.Framework.Assert.IsTrue(PageContainsImage(destPath, 2));
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destPath, SOURCE_FOLDER + "cmp_" + fileName
                , DESTINATION_FOLDER));
        }

        //TODO DEVSIX-7410: Fix test after fix
        [NUnit.Framework.Test]
        [LogMessage(LayoutLogMessageConstant.ELEMENT_DOES_NOT_FIT_AREA, Count = 6)]
        public virtual void MultipleImageRowsStartOnShortFirstPageTest() {
            String fileName = "tableMultiImagesShortPage.pdf";
            String destPath = DESTINATION_FOLDER + fileName;
            Document document = CreateShortFirstPageDocument(destPath);
            Table table = CreateBaseImageTable(false);
            for (int i = 0; i < 5; i++) {
                table.AddCell(new Cell().Add(new iText.Layout.Element.Image(ImageDataFactory.Create(SOURCE_FOLDER + "itis.jpg"
                    ))));
            }
            document.Add(table);
            document.GetPdfDocument().SetDefaultPageSize(PageSize.A4);
            document.Close();
            NUnit.Framework.Assert.IsTrue(PageContainsImage(destPath, 1));
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destPath, SOURCE_FOLDER + "cmp_" + fileName
                , DESTINATION_FOLDER));
        }

        [NUnit.Framework.Test]
        public virtual void AutoScaledImageFitsOnShortFirstPageTest() {
            String fileName = "tableImageShortPageScaled.pdf";
            String destPath = DESTINATION_FOLDER + fileName;
            Document document = CreateShortFirstPageDocument(destPath);
            document.Add(CreateBaseImageTable(true));
            document.Close();
            NUnit.Framework.Assert.IsTrue(PageContainsImage(destPath, 1));
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destPath, SOURCE_FOLDER + "cmp_" + fileName
                , DESTINATION_FOLDER));
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.TABLE_WIDTH_IS_MORE_THAN_EXPECTED_DUE_TO_MIN_WIDTH)]
        public virtual void ImageOnVeryThinPageTest() {
            String fileName = "tableImageThinPage.pdf";
            String destPath = DESTINATION_FOLDER + fileName;
            PdfWriter writer = new PdfWriter(destPath);
            PdfDocument pdf = new PdfDocument(writer);
            PageSize thinPage = new PageSize(180, PageSize.A4.GetHeight());
            pdf.SetDefaultPageSize(thinPage);
            Document document = new Document(pdf);
            document.SetMargins(MARGIN, MARGIN, MARGIN, MARGIN);
            Table table = new Table(1);
            table.SetWidth(thinPage.GetWidth() - 2 * MARGIN);
            table.AddHeaderCell(CreateHeaderCell());
            iText.Layout.Element.Image image = new iText.Layout.Element.Image(ImageDataFactory.Create(SOURCE_FOLDER + 
                "itis.jpg"));
            table.AddCell(new Cell().Add(image));
            document.Add(table);
            document.Close();
            NUnit.Framework.Assert.IsTrue(PageContainsImage(destPath, 1));
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destPath, SOURCE_FOLDER + "cmp_" + fileName
                , DESTINATION_FOLDER));
        }

        private Document CreateA4Document(String fileName) {
            PdfWriter writer = new PdfWriter(fileName);
            PdfDocument pdf = new PdfDocument(writer);
            Document document = new Document(pdf, PageSize.A4);
            document.SetMargins(MARGIN, MARGIN, MARGIN, MARGIN);
            return document;
        }

        private Document CreateShortFirstPageDocument(String fileName) {
            return CreateShortFirstPageDocument(fileName, false);
        }

        private Document CreateShortFirstPageDocument(String fileName, bool addSecondA4Page) {
            PdfWriter writer = new PdfWriter(fileName);
            PdfDocument pdf = new PdfDocument(writer);
            pdf.SetDefaultPageSize(SHORT_PAGE);
            if (addSecondA4Page) {
                pdf.AddNewPage();
                pdf.SetDefaultPageSize(PageSize.A4);
            }
            Document document = new Document(pdf);
            document.SetMargins(MARGIN, MARGIN, MARGIN, MARGIN);
            return document;
        }

        private Table CreateBaseImageTable(bool autoScale) {
            Table table = new Table(1);
            table.SetWidth(PageSize.A4.GetWidth() - 2 * MARGIN);
            table.AddHeaderCell(CreateHeaderCell());
            iText.Layout.Element.Image image = new iText.Layout.Element.Image(ImageDataFactory.Create(SOURCE_FOLDER + 
                "itis.jpg"));
            image.SetAutoScale(autoScale);
            table.AddCell(new Cell().Add(image));
            return table;
        }

        private Cell CreateHeaderCell() {
            return new Cell().Add(new Paragraph("Header"));
        }

        private void AddFiller(Document document, int lines) {
            for (int i = 0; i < lines; i++) {
                document.Add(new Paragraph(" "));
            }
        }

        private static bool PageContainsImage(String pdfPath, int pageNumber) {
            bool[] found = new bool[] { false };
            using (PdfDocument pdfDocument = new PdfDocument(new PdfReader(pdfPath))) {
                PdfPage page = pdfDocument.GetPage(pageNumber);
                IEventListener listener = new _IEventListener_401(found);
                PdfCanvasProcessor processor = new PdfCanvasProcessor(listener);
                processor.ProcessPageContent(page);
            }
            return found[0];
        }

        private sealed class _IEventListener_401 : IEventListener {
            public _IEventListener_401(bool[] found) {
                this.found = found;
            }

            public void EventOccurred(IEventData data, EventType type) {
                if (type == EventType.RENDER_IMAGE) {
                    found[0] = true;
                }
            }

            public ICollection<EventType> GetSupportedEvents() {
                ICollection<EventType> types = new HashSet<EventType>();
                types.Add(EventType.RENDER_IMAGE);
                return types;
            }

            private readonly bool[] found;
        }
    }
}
