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
