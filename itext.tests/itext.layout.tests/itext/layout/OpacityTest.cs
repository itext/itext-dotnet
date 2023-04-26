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
using iText.IO.Image;
using iText.Kernel.Colors;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Utils;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Test;

namespace iText.Layout {
    [NUnit.Framework.Category("IntegrationTest")]
    public class OpacityTest : ExtendedITextTest {
        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/layout/OpacityTest/";

        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/layout/OpacityTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateOrClearDestinationFolder(destinationFolder);
        }

        [NUnit.Framework.Test]
        public virtual void BackgroundOpacityTest01() {
            String outFileName = destinationFolder + "backgroundOpacityTest01.pdf";
            String cmpFileName = sourceFolder + "cmp_backgroundOpacityTest01.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document document = new Document(pdfDocument);
            DeviceRgb darkBlue = new DeviceRgb(32, 80, 129);
            Div div = new Div().SetBackgroundColor(darkBlue).SetHeight(200);
            div.Add(new Paragraph("Simple text inside of the div with transparent (0.0) background.").SetBackgroundColor
                (ColorConstants.RED, 0f));
            div.Add(new Paragraph("Simple text inside of the div with transparent (0.3) background.").SetBackgroundColor
                (ColorConstants.RED, 0.3f));
            div.Add(new Paragraph("Simple text inside of the div with transparent (0.5) background.").SetBackgroundColor
                (ColorConstants.RED, 0.5f));
            div.Add(new Paragraph("Simple text inside of the div with transparent (0.7) background.").SetBackgroundColor
                (ColorConstants.RED, 0.7f));
            div.Add(new Paragraph("Simple text inside of the div with transparent (1.0) background.").SetBackgroundColor
                (ColorConstants.RED, 1f));
            div.Add(new Paragraph("Simple text inside of the div with background.").SetBackgroundColor(ColorConstants.
                RED));
            document.Add(div);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void BackgroundOpacityTest02() {
            String outFileName = destinationFolder + "backgroundOpacityTest02.pdf";
            String cmpFileName = sourceFolder + "cmp_backgroundOpacityTest02.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document document = new Document(pdfDocument);
            document.Add(new Paragraph("Paragraph with ").SetBackgroundColor(ColorConstants.RED).Add(new Text("text element with transparent (0.0) background"
                ).SetBackgroundColor(ColorConstants.WHITE, 0.0f)));
            document.Add(new Paragraph("Paragraph with ").SetBackgroundColor(ColorConstants.RED).Add(new Text("text element with transparent (0.3) background"
                ).SetBackgroundColor(ColorConstants.WHITE, 0.3f)));
            document.Add(new Paragraph("Paragraph with ").SetBackgroundColor(ColorConstants.RED).Add(new Text("text element with transparent (0.5) background"
                ).SetBackgroundColor(ColorConstants.WHITE, 0.5f)));
            document.Add(new Paragraph("Paragraph with ").SetBackgroundColor(ColorConstants.RED).Add(new Text("text element with transparent (0.7) background"
                ).SetBackgroundColor(ColorConstants.WHITE, 0.7f)));
            document.Add(new Paragraph("Paragraph with ").SetBackgroundColor(ColorConstants.RED).Add(new Text("text element with transparent (1.0) background"
                ).SetBackgroundColor(ColorConstants.WHITE, 1.0f)));
            document.Add(new Paragraph("Paragraph with ").SetBackgroundColor(ColorConstants.RED).Add(new Text("text element with transparent (-1.0) background"
                ).SetBackgroundColor(ColorConstants.WHITE, -1.0f)));
            document.Add(new Paragraph("Paragraph with ").SetBackgroundColor(ColorConstants.RED).Add(new Text("text element with transparent (100.0) background"
                ).SetBackgroundColor(ColorConstants.WHITE, 100.0f)));
            document.Add(new Paragraph("Paragraph with ").SetBackgroundColor(ColorConstants.RED).Add(new Text("text element with transparent (NaN) background"
                ).SetBackgroundColor(ColorConstants.WHITE, float.NaN)));
            document.Add(new Paragraph("Paragraph with ").SetBackgroundColor(ColorConstants.RED).Add(new Text("text element with background"
                ).SetBackgroundColor(ColorConstants.WHITE)));
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void BorderOpacityTest01() {
            String outFileName = destinationFolder + "borderOpacityTest01.pdf";
            String cmpFileName = sourceFolder + "cmp_borderOpacityTest01.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document document = new Document(pdfDocument);
            DeviceRgb darkBlue = new DeviceRgb(32, 80, 129);
            Div div = new Div().SetBackgroundColor(darkBlue).SetHeight(300);
            div.Add(new Paragraph("Simple text inside of the div with transparent (0.0) border.").SetBorder(new DoubleBorder
                (ColorConstants.RED, 7f, 0.0f)));
            div.Add(new Paragraph("Simple text inside of the div with transparent (0.3) border.").SetBorder(new DoubleBorder
                (ColorConstants.RED, 7f, 0.3f)));
            div.Add(new Paragraph("Simple text inside of the div with transparent (0.5) border.").SetBorder(new DoubleBorder
                (ColorConstants.RED, 7f, 0.5f)));
            div.Add(new Paragraph("Simple text inside of the div with transparent (0.7) border.").SetBorder(new DoubleBorder
                (ColorConstants.RED, 7f, 0.7f)));
            div.Add(new Paragraph("Simple text inside of the div with transparent (1.0) border.").SetBorder(new DoubleBorder
                (ColorConstants.RED, 7f, 1.0f)));
            div.Add(new Paragraph("Simple text inside of the div with border.").SetBorder(new DoubleBorder(ColorConstants
                .RED, 7f)));
            document.Add(div);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void TextOpacityTest01() {
            String outFileName = destinationFolder + "textOpacityTest01.pdf";
            String cmpFileName = sourceFolder + "cmp_textOpacityTest01.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document document = new Document(pdfDocument);
            DeviceRgb darkBlue = new DeviceRgb(32, 80, 129);
            Div div = new Div().SetBackgroundColor(darkBlue).SetHeight(300);
            div.Add(new Paragraph("Simple text inside of the div with transparent (0.0) text.").SetFontColor(ColorConstants
                .RED, 0f));
            div.Add(new Paragraph("Simple text inside of the div with transparent (0.3) text.").SetFontColor(ColorConstants
                .RED, 0.3f));
            div.Add(new Paragraph("Simple text inside of the div with transparent (0.5) text.").SetFontColor(ColorConstants
                .RED, 0.5f));
            div.Add(new Paragraph("Simple text inside of the div with transparent (0.7) text.").SetFontColor(ColorConstants
                .RED, 0.7f));
            div.Add(new Paragraph("Simple text inside of the div with transparent (1.0) text.").SetFontColor(ColorConstants
                .RED, 1f));
            div.Add(new Paragraph("Simple text inside of the div with text.").SetFontColor(ColorConstants.RED));
            document.Add(div);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void UnderlineOpacityTest01() {
            String outFileName = destinationFolder + "underlineOpacityTest01.pdf";
            String cmpFileName = sourceFolder + "cmp_underlineOpacityTest01.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document document = new Document(pdfDocument);
            DeviceRgb darkBlue = new DeviceRgb(32, 80, 129);
            Div div = new Div().SetBackgroundColor(darkBlue).SetHeight(300);
            div.Add(new Paragraph("Simple text inside of the div with transparent (0.0) underline.").SetUnderline(ColorConstants
                .RED, 0.0f, .75f, 0, 0, -1 / 8f, PdfCanvasConstants.LineCapStyle.BUTT));
            div.Add(new Paragraph("Simple text inside of the div with transparent (0.3) underline.").SetUnderline(ColorConstants
                .RED, 0.3f, .75f, 0, 0, -1 / 8f, PdfCanvasConstants.LineCapStyle.BUTT));
            div.Add(new Paragraph("Simple text inside of the div with transparent (0.5) underline.").SetUnderline(ColorConstants
                .RED, 0.5f, .75f, 0, 0, -1 / 8f, PdfCanvasConstants.LineCapStyle.BUTT));
            div.Add(new Paragraph("Simple text inside of the div with transparent (0.7) underline.").SetUnderline(ColorConstants
                .RED, 0.7f, .75f, 0, 0, -1 / 8f, PdfCanvasConstants.LineCapStyle.BUTT));
            div.Add(new Paragraph("Simple text inside of the div with transparent (1.0) underline.").SetUnderline(ColorConstants
                .RED, 1.0f, .75f, 0, 0, -1 / 8f, PdfCanvasConstants.LineCapStyle.BUTT));
            div.Add(new Paragraph("Simple text inside of the div with underline.").SetUnderline(ColorConstants.RED, .75f
                , 0, 0, -1 / 8f, PdfCanvasConstants.LineCapStyle.BUTT));
            document.Add(div);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void TextElementOpacity01() {
            ElementOpacityTest("text");
        }

        [NUnit.Framework.Test]
        public virtual void DivElementOpacity01() {
            ElementOpacityTest("div");
        }

        [NUnit.Framework.Test]
        public virtual void ParaElementOpacity01() {
            ElementOpacityTest("para");
        }

        [NUnit.Framework.Test]
        public virtual void ImageElementOpacity01() {
            ElementOpacityTest("image");
        }

        [NUnit.Framework.Test]
        public virtual void CellElementOpacity01() {
            ElementOpacityTest("cell");
        }

        [NUnit.Framework.Test]
        public virtual void TableElementOpacity01() {
            ElementOpacityTest("table");
        }

        [NUnit.Framework.Test]
        public virtual void ListElementOpacity01() {
            ElementOpacityTest("list");
        }

        [NUnit.Framework.Test]
        public virtual void ListItemElementOpacity01() {
            ElementOpacityTest("listItem");
        }

        private void ElementOpacityTest(String elem) {
            String outFileName = destinationFolder + elem + "ElementOpacity01.pdf";
            String cmpFileName = sourceFolder + "cmp_" + elem + "ElementOpacity01.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document document = new Document(pdfDocument);
            DeviceRgb divBackground = WebColors.GetRGBColor("#82abd6");
            DeviceRgb paraBackground = WebColors.GetRGBColor("#994ec7");
            DeviceRgb textBackground = WebColors.GetRGBColor("#009688");
            DeviceRgb tableBackground = WebColors.GetRGBColor("#ffc107");
            document.SetFontColor(ColorConstants.WHITE);
            Div div = new Div().SetBackgroundColor(divBackground);
            if ("div".Equals(elem)) {
                div.SetOpacity(0.3f);
            }
            div.Add(new Paragraph("direct div content"));
            Paragraph p = new Paragraph("direct paragraph content").SetBackgroundColor(paraBackground);
            if ("para".Equals(elem)) {
                p.SetOpacity(0.3f);
            }
            Text text = new Text("text content").SetBackgroundColor(textBackground);
            p.Add(text);
            if ("text".Equals(elem)) {
                text.SetOpacity(0.3f);
            }
            div.Add(p);
            iText.Layout.Element.Image image = new Image(ImageDataFactory.Create(sourceFolder + "itis.jpg"));
            div.Add(image);
            if ("image".Equals(elem)) {
                image.SetOpacity(0.3f);
            }
            Table table = new Table(UnitValue.CreatePercentArray(2)).UseAllAvailableWidth().SetBackgroundColor(tableBackground
                );
            table.AddCell("Cell00");
            table.AddCell("Cell01");
            Cell cell10 = new Cell().Add(new Paragraph("Cell10"));
            if ("cell".Equals(elem)) {
                cell10.SetOpacity(0.3f);
            }
            table.AddCell(cell10);
            table.AddCell(new Cell().Add(new Paragraph("Cell11")));
            if ("table".Equals(elem)) {
                table.SetOpacity(0.3f);
            }
            div.Add(table);
            List list = new List();
            if ("list".Equals(elem)) {
                list.SetOpacity(0.3f);
            }
            ListItem listItem = new ListItem("item 0");
            list.Add(listItem);
            if ("listItem".Equals(elem)) {
                listItem.SetOpacity(0.3f);
            }
            list.Add("item 1");
            div.Add(list);
            document.Add(div);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }
    }
}
