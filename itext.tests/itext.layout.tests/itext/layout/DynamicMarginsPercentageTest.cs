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
using iText.Commons.Utils;
using iText.IO.Image;
using iText.Kernel.Colors;
using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using iText.Layout.Element;
using iText.Layout.Logs;
using iText.Layout.Properties;
using iText.Layout.Testutil;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Layout {
    [NUnit.Framework.Category("IntegrationTest")]
    public class DynamicMarginsPercentageTest : ExtendedITextTest {
        private static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/layout/DynamicMarginsPercentageTest/";

        private static readonly String DESTINATION_FOLDER = TestUtil.GetOutputPath() + "/layout/DynamicMarginsPercentageTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateOrClearDestinationFolder(DESTINATION_FOLDER);
        }

        public static IEnumerable<Object[]> DataSource() {
            return JavaUtil.ArraysAsList(new Object[][] { new Object[] { 20 }, new Object[] { 50 }, new Object[] { 100
                 }, new Object[] { 150 } });
        }

        // The percentage height value is ignored since the parent is a DocumentRenderer with null height.
        [NUnit.Framework.TestCaseSource("DataSource")]
        public virtual void DivPercentHeightTopMarginRenderTest(int percent) {
            String fileName = "divPercentHeightTopMargin" + percent;
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName))) {
                using (Document document = new Document(pdfDoc)) {
                    Div marginContent = new Div().SetHeight(UnitValue.CreatePercentValue(percent)).SetBackgroundColor(new DeviceRgb
                        (255, 200, 200));
                    marginContent.Add(new Paragraph(percent + "% height div in top margin"));
                    document.SetPageMargins((pageNum) => true, PageMarginsTestUtil.GetMarginBoxesWithContent(marginContent, null
                        , null, null));
                    document.Add(TestResourceUtil.GetTallDiv(2));
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff_" + fileName));
        }

        [NUnit.Framework.TestCaseSource("DataSource")]
        public virtual void DivPercentWidthTopMarginRenderTest(int percent) {
            String fileName = "divPercentWidthTopMargin" + percent;
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName))) {
                using (Document document = new Document(pdfDoc)) {
                    Div marginContent = new Div().SetWidth(UnitValue.CreatePercentValue(percent)).SetBackgroundColor(new DeviceRgb
                        (200, 255, 200));
                    marginContent.Add(new Paragraph(percent + "% width div in top margin"));
                    document.SetPageMargins((pageNum) => true, PageMarginsTestUtil.GetMarginBoxesWithContent(marginContent, null
                        , null, null));
                    document.Add(TestResourceUtil.GetTallDiv(2));
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff_" + fileName));
        }

        [NUnit.Framework.Test]
        public virtual void DivPercentHeightBottomMargin50RenderTest() {
            String fileName = "divPercentHeightBottomMargin50";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName))) {
                using (Document document = new Document(pdfDoc)) {
                    Div marginContent = new Div().SetHeight(UnitValue.CreatePercentValue(50)).SetBackgroundColor(new DeviceRgb
                        (200, 200, 255));
                    marginContent.Add(new Paragraph("50% height div in bottom margin"));
                    document.SetPageMargins((pageNum) => true, PageMarginsTestUtil.GetMarginBoxesWithContent(null, marginContent
                        , null, null));
                    document.Add(TestResourceUtil.GetTallDiv(2));
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff_" + fileName));
        }

        [NUnit.Framework.Test]
        public virtual void DivPercentWidthBottomMargin50RenderTest() {
            String fileName = "divPercentWidthBottomMargin50";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName))) {
                using (Document document = new Document(pdfDoc)) {
                    Div marginContent = new Div().SetWidth(UnitValue.CreatePercentValue(50)).SetBackgroundColor(new DeviceRgb(
                        200, 200, 255));
                    marginContent.Add(new Paragraph("50% width div in bottom margin"));
                    document.SetPageMargins((pageNum) => true, PageMarginsTestUtil.GetMarginBoxesWithContent(null, marginContent
                        , null, null));
                    document.Add(TestResourceUtil.GetTallDiv(2));
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff_" + fileName));
        }

        [NUnit.Framework.Test]
        public virtual void DivPercentWidthLeftMargin50RenderTest() {
            String fileName = "divPercentWidthLeftMargin50";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName))) {
                using (Document document = new Document(pdfDoc)) {
                    Div marginContent = new Div().SetWidth(UnitValue.CreatePercentValue(50)).SetBackgroundColor(new DeviceRgb(
                        255, 255, 200));
                    marginContent.Add(new Paragraph("50% width div in left margin"));
                    document.SetPageMargins((pageNum) => true, PageMarginsTestUtil.GetMarginBoxesWithContent(null, null, marginContent
                        , null));
                    document.Add(TestResourceUtil.GetTallDiv(2));
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff_" + fileName));
        }

        [NUnit.Framework.Test]
        public virtual void DivPercentHeightLeftMargin50RenderTest() {
            String fileName = "divPercentHeightLeftMargin50";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName))) {
                using (Document document = new Document(pdfDoc)) {
                    Div marginContent = new Div().SetHeight(UnitValue.CreatePercentValue(50)).SetBackgroundColor(new DeviceRgb
                        (255, 255, 200));
                    marginContent.Add(new Paragraph("50% height div in left margin"));
                    document.SetPageMargins((pageNum) => true, PageMarginsTestUtil.GetMarginBoxesWithContent(null, null, marginContent
                        , null));
                    document.Add(TestResourceUtil.GetTallDiv(2));
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff_" + fileName));
        }

        [NUnit.Framework.Test]
        public virtual void DivPercentWidthRightMargin50RenderTest() {
            String fileName = "divPercentWidthRightMargin50";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName))) {
                using (Document document = new Document(pdfDoc)) {
                    Div marginContent = new Div().SetWidth(UnitValue.CreatePercentValue(50)).SetBackgroundColor(new DeviceRgb(
                        255, 220, 180));
                    marginContent.Add(new Paragraph("50% width div in right margin"));
                    document.SetPageMargins((pageNum) => true, PageMarginsTestUtil.GetMarginBoxesWithContent(null, null, null, 
                        marginContent));
                    document.Add(TestResourceUtil.GetTallDiv(2));
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff_" + fileName));
        }

        [NUnit.Framework.Test]
        public virtual void DivPercentHeightAndWidthTopMarginRenderTest() {
            String fileName = "divPercentHeightAndWidthTopMargin";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName))) {
                using (Document document = new Document(pdfDoc)) {
                    Div marginContent = new Div().SetHeight(UnitValue.CreatePercentValue(50)).SetWidth(UnitValue.CreatePercentValue
                        (50)).SetBackgroundColor(new DeviceRgb(255, 180, 255));
                    marginContent.Add(new Paragraph("50% height + 50% width div in top margin"));
                    document.SetPageMargins((pageNum) => true, PageMarginsTestUtil.GetMarginBoxesWithContent(marginContent, null
                        , null, null));
                    document.Add(TestResourceUtil.GetTallDiv(2));
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff_" + fileName));
        }

        [NUnit.Framework.Test]
        public virtual void NestedDivPercentHeightTopMarginRenderTest() {
            String fileName = "nestedDivPercentHeightTopMargin";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName))) {
                using (Document document = new Document(pdfDoc)) {
                    Div inner = new Div().SetHeight(UnitValue.CreatePercentValue(50)).SetBackgroundColor(new DeviceRgb(100, 200
                        , 255));
                    inner.Add(new Paragraph("50% height inner div"));
                    Div outer = new Div().SetHeight(120).SetBackgroundColor(new DeviceRgb(200, 240, 255));
                    outer.Add(inner);
                    document.SetPageMargins((pageNum) => true, PageMarginsTestUtil.GetMarginBoxesWithContent(outer, null, null
                        , null));
                    document.Add(TestResourceUtil.GetTallDiv(2));
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff_" + fileName));
        }

        [NUnit.Framework.Test]
        public virtual void NestedDivPercentWidthTopMarginRenderTest() {
            String fileName = "nestedDivPercentWidthTopMargin";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName))) {
                using (Document document = new Document(pdfDoc)) {
                    Div inner = new Div().SetWidth(UnitValue.CreatePercentValue(50)).SetBackgroundColor(new DeviceRgb(100, 200
                        , 255));
                    inner.Add(new Paragraph("50% width inner div"));
                    Div outer = new Div().SetWidth(300).SetBackgroundColor(new DeviceRgb(200, 240, 255));
                    outer.Add(inner);
                    document.SetPageMargins((pageNum) => true, PageMarginsTestUtil.GetMarginBoxesWithContent(outer, null, null
                        , null));
                    document.Add(TestResourceUtil.GetTallDiv(2));
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff_" + fileName));
        }

        [NUnit.Framework.Test]
        [LogMessage(LayoutLogMessageConstant.PAGE_CONTENT_CANNOT_BE_DRAWN)]
        public virtual void ImagePercentHeightTopMargin50Test() {
            String fileName = "imgPercentHeightTopMargin50";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName))) {
                using (Document document = new Document(pdfDoc)) {
                    iText.Layout.Element.Image image = new Image(ImageDataFactory.Create(SOURCE_FOLDER + "bee.png")).SetHeight
                        (UnitValue.CreatePercentValue(50));
                    Div marginContent = new Div().SetBackgroundColor(new DeviceRgb(255, 230, 200));
                    marginContent.Add(image);
                    SectionBreak sectionBreak = new SectionBreak(PageMarginsTestUtil.GetMarginBoxesWithContent(marginContent, 
                        null, null, null));
                    document.Add(sectionBreak);
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff_" + fileName));
        }

        [NUnit.Framework.Test]
        public virtual void ImagePercentWidthTopMargin50RenderTest() {
            // The image renders incorrectly because PageMarginBoxes#layout does not take left and right margins
            // into account when laying out top and bottom margin boxes, resulting in a wrong calculated height.
            String fileName = "imagePercentWidthTopMargin50";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName))) {
                using (Document document = new Document(pdfDoc)) {
                    iText.Layout.Element.Image image = new iText.Layout.Element.Image(ImageDataFactory.Create(SOURCE_FOLDER + 
                        "bee.png")).SetWidth(UnitValue.CreatePercentValue(50));
                    Div marginContent = new Div().SetBackgroundColor(new DeviceRgb(255, 230, 200));
                    marginContent.Add(image);
                    document.SetPageMargins((pageNum) => true, PageMarginsTestUtil.GetMarginBoxesWithContent(marginContent, null
                        , null, null));
                    document.Add(TestResourceUtil.GetTallDiv(2));
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff_" + fileName));
        }

        [NUnit.Framework.Test]
        public virtual void ImagePercentHeightAndWidthTopMarginRenderTest() {
            // The image renders incorrectly because PageMarginBoxes#layout does not take left and right margins
            // into account when laying out top and bottom margin boxes, resulting in a wrong calculated height.
            String fileName = "imagePercentHeightAndWidthTopMargin";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName))) {
                using (Document document = new Document(pdfDoc)) {
                    iText.Layout.Element.Image image = new iText.Layout.Element.Image(ImageDataFactory.Create(SOURCE_FOLDER + 
                        "bee.png")).SetHeight(UnitValue.CreatePercentValue(50)).SetWidth(UnitValue.CreatePercentValue(50));
                    Div marginContent = new Div().SetBackgroundColor(new DeviceRgb(255, 230, 200));
                    marginContent.Add(image);
                    document.SetPageMargins((pageNum) => true, PageMarginsTestUtil.GetMarginBoxesWithContent(marginContent, null
                        , null, null));
                    document.Add(TestResourceUtil.GetTallDiv(2));
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff_" + fileName));
        }

        [NUnit.Framework.Test]
        [LogMessage(LayoutLogMessageConstant.PAGE_CONTENT_CANNOT_BE_DRAWN)]
        public virtual void ImagePercentHeightBottomMargin50Test() {
            String fileName = "imgPercentHeightBottomMargin50";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName))) {
                using (Document document = new Document(pdfDoc)) {
                    iText.Layout.Element.Image image = new iText.Layout.Element.Image(ImageDataFactory.Create(SOURCE_FOLDER + 
                        "bee.png")).SetHeight(UnitValue.CreatePercentValue(50));
                    Div marginContent = new Div().SetBackgroundColor(new DeviceRgb(200, 230, 255));
                    marginContent.Add(image);
                    SectionBreak sectionBreak = new SectionBreak(PageMarginsTestUtil.GetMarginBoxesWithContent(null, marginContent
                        , null, null));
                    document.Add(sectionBreak);
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff_" + fileName));
        }

        [NUnit.Framework.Test]
        public virtual void ImagePercentWidthBottomMargin50RenderTest() {
            // The image renders incorrectly because PageMarginBoxes#layout does not take left and right margins
            // into account when laying out top and bottom margin boxes, resulting in a wrong calculated height.
            String fileName = "imagePercentWidthBottomMargin50";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName))) {
                using (Document document = new Document(pdfDoc)) {
                    iText.Layout.Element.Image image = new iText.Layout.Element.Image(ImageDataFactory.Create(SOURCE_FOLDER + 
                        "bee.png")).SetWidth(UnitValue.CreatePercentValue(50));
                    Div marginContent = new Div().SetBackgroundColor(new DeviceRgb(200, 230, 255));
                    marginContent.Add(image);
                    document.SetPageMargins((pageNum) => true, PageMarginsTestUtil.GetMarginBoxesWithContent(null, marginContent
                        , null, null));
                    document.Add(TestResourceUtil.GetTallDiv(2));
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff_" + fileName));
        }

        [NUnit.Framework.Test]
        public virtual void ParagraphPercentHeightTopMargin50RenderTest() {
            String fileName = "paragraphPercentHeightTopMargin50";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName))) {
                using (Document document = new Document(pdfDoc)) {
                    Paragraph p = new Paragraph("50% height paragraph in top margin").SetHeight(UnitValue.CreatePercentValue(50
                        )).SetBackgroundColor(new DeviceRgb(220, 255, 220));
                    Div marginContent = new Div().Add(p);
                    document.SetPageMargins((pageNum) => true, PageMarginsTestUtil.GetMarginBoxesWithContent(marginContent, null
                        , null, null));
                    document.Add(TestResourceUtil.GetTallDiv(2));
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff_" + fileName));
        }

        [NUnit.Framework.Test]
        public virtual void ParagraphPercentWidthTopMargin50RenderTest() {
            String fileName = "paragraphPercentWidthTopMargin50";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName))) {
                using (Document document = new Document(pdfDoc)) {
                    Paragraph p = new Paragraph("50% width paragraph in top margin").SetWidth(UnitValue.CreatePercentValue(50)
                        ).SetBackgroundColor(new DeviceRgb(220, 255, 220));
                    Div marginContent = new Div().Add(p);
                    document.SetPageMargins((pageNum) => true, PageMarginsTestUtil.GetMarginBoxesWithContent(marginContent, null
                        , null, null));
                    document.Add(TestResourceUtil.GetTallDiv(2));
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff_" + fileName));
        }

        [NUnit.Framework.Test]
        public virtual void ParagraphPercentHeightBottomMargin50RenderTest() {
            String fileName = "paragraphPercentHeightBottomMargin50";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName))) {
                using (Document document = new Document(pdfDoc)) {
                    Paragraph p = new Paragraph("50% height paragraph in bottom margin").SetHeight(UnitValue.CreatePercentValue
                        (50)).SetBackgroundColor(new DeviceRgb(255, 220, 255));
                    Div marginContent = new Div().Add(p);
                    document.SetPageMargins((pageNum) => true, PageMarginsTestUtil.GetMarginBoxesWithContent(null, marginContent
                        , null, null));
                    document.Add(TestResourceUtil.GetTallDiv(2));
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff_" + fileName));
        }

        [NUnit.Framework.Test]
        public virtual void ParagraphPercentWidthBottomMargin50RenderTest() {
            String fileName = "paragraphPercentWidthBottomMargin50";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName))) {
                using (Document document = new Document(pdfDoc)) {
                    Paragraph p = new Paragraph("50% width paragraph in bottom margin").SetWidth(UnitValue.CreatePercentValue(
                        50)).SetBackgroundColor(new DeviceRgb(255, 220, 255));
                    Div marginContent = new Div().Add(p);
                    document.SetPageMargins((pageNum) => true, PageMarginsTestUtil.GetMarginBoxesWithContent(null, marginContent
                        , null, null));
                    document.Add(TestResourceUtil.GetTallDiv(2));
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff_" + fileName));
        }

        [NUnit.Framework.Test]
        public virtual void MixedPercentTopAndBottomMarginsRenderTest() {
            String fileName = "mixedPercentageTopAndBottomMargins";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName))) {
                using (Document document = new Document(pdfDoc)) {
                    Div topContent = new Div().SetHeight(UnitValue.CreatePercentValue(20)).SetWidth(UnitValue.CreatePercentValue
                        (100)).SetBackgroundColor(new DeviceRgb(255, 200, 200));
                    topContent.Add(new Paragraph("20% height + 100% width in top margin"));
                    Div bottomContent = new Div().SetHeight(UnitValue.CreatePercentValue(15)).SetWidth(UnitValue.CreatePercentValue
                        (50)).SetBackgroundColor(new DeviceRgb(200, 200, 255));
                    bottomContent.Add(new Paragraph("15% height + 50% width in bottom margin"));
                    document.SetPageMargins((pageNum) => true, PageMarginsTestUtil.GetMarginBoxesWithContent(topContent, bottomContent
                        , null, null));
                    document.Add(TestResourceUtil.GetTallDiv(2));
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff_" + fileName));
        }

        [NUnit.Framework.Test]
        public virtual void MixedPercentAllFourMarginsRenderTest() {
            String fileName = "mixedPercentageAllFourMargins";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName))) {
                using (Document document = new Document(pdfDoc)) {
                    Div topContent = new Div().SetHeight(UnitValue.CreatePercentValue(20)).SetBackgroundColor(new DeviceRgb(255
                        , 200, 200));
                    topContent.Add(new Paragraph("TOP 20%h"));
                    Div bottomContent = new Div().SetHeight(UnitValue.CreatePercentValue(15)).SetBackgroundColor(new DeviceRgb
                        (200, 200, 255));
                    bottomContent.Add(new Paragraph("BTM 15%h"));
                    Div leftContent = new Div().SetWidth(UnitValue.CreatePercentValue(50)).SetBackgroundColor(new DeviceRgb(200
                        , 255, 200));
                    leftContent.Add(new Paragraph("L 50%w"));
                    Div rightContent = new Div().SetWidth(UnitValue.CreatePercentValue(50)).SetBackgroundColor(new DeviceRgb(255
                        , 255, 200));
                    rightContent.Add(new Paragraph("R 50%w"));
                    document.SetPageMargins((pageNum) => true, PageMarginsTestUtil.GetMarginBoxesWithContent(topContent, bottomContent
                        , leftContent, rightContent));
                    document.Add(TestResourceUtil.GetTallDiv(2));
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff_" + fileName));
        }
    }
}
