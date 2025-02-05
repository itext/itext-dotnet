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
using iText.IO.Font.Constants;
using iText.IO.Image;
using iText.Kernel.Colors;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Xobject;
using iText.Kernel.Utils;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Font;
using iText.Layout.Properties;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Layout {
    [NUnit.Framework.Category("IntegrationTest")]
    public class BlockTest : ExtendedITextTest {
        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/layout/BlockTest/";

        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/layout/BlockTest/";

        private const String textByronNarrow = "When a man hath no freedom to fight for at home, " + "Let him combat for that of his neighbours; "
             + "Let him think of the glories of Greece and of Rome, " + "And get knocked on the head for his labours. "
             + "\n" + "To do good to Mankind is the chivalrous plan, " + "And is always as nobly requited; " + "Then battle for Freedom wherever you can, "
             + "And, if not shot or hanged, you'll get knighted.";

        private const String textByron = "When a man hath no freedom to fight for at home,\n" + "    Let him combat for that of his neighbours;\n"
             + "Let him think of the glories of Greece and of Rome,\n" + "    And get knocked on the head for his labours.\n"
             + "\n" + "To do good to Mankind is the chivalrous plan,\n" + "    And is always as nobly requited;\n"
             + "Then battle for Freedom wherever you can,\n" + "    And, if not shot or hanged, you'll get knighted.";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateOrClearDestinationFolder(destinationFolder);
        }

        [LogMessage(iText.IO.Logs.IoLogMessageConstant.CLIP_ELEMENT, Count = 2)]
        [NUnit.Framework.Test]
        public virtual void BlockWithSetHeightProperties01() {
            String outFileName = destinationFolder + "blockWithSetHeightProperties01.pdf";
            String cmpFileName = sourceFolder + "cmp_blockWithSetHeightProperties01.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDocument);
            Paragraph p = new Paragraph(textByron);
            for (int i = 0; i < 10; i++) {
                p.Add(textByron);
            }
            p.SetBorder(new SolidBorder(0.5f));
            doc.Add(new Paragraph("Default layout:"));
            doc.Add(p);
            doc.Add(new AreaBreak());
            doc.Add(new Paragraph("Paragraph's height is set shorter than needed:"));
            p.SetHeight(1300);
            doc.Add(p);
            doc.Add(new AreaBreak());
            doc.Add(new Paragraph("Paragraph's min height is set shorter than needed:"));
            p.DeleteOwnProperty(Property.HEIGHT);
            p.SetMinHeight(1300);
            doc.Add(p);
            doc.Add(new AreaBreak());
            doc.Add(new Paragraph("Paragraph's max height is set shorter than needed:"));
            p.DeleteOwnProperty(Property.MIN_HEIGHT);
            p.SetMaxHeight(1300);
            doc.Add(p);
            doc.Add(new AreaBreak());
            doc.Add(new Paragraph("Paragraph's height is set bigger than needed:"));
            p.DeleteOwnProperty(Property.MAX_HEIGHT);
            p.SetHeight(2500);
            doc.Add(p);
            doc.Add(new AreaBreak());
            doc.Add(new Paragraph("Paragraph's min height is set bigger than needed:"));
            p.DeleteOwnProperty(Property.HEIGHT);
            p.SetMinHeight(2500);
            doc.Add(p);
            doc.Add(new AreaBreak());
            doc.Add(new Paragraph("Paragraph's max height is set bigger than needed:"));
            p.DeleteOwnProperty(Property.MIN_HEIGHT);
            p.SetMaxHeight(2500);
            doc.Add(p);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        [LogMessage(iText.IO.Logs.IoLogMessageConstant.CLIP_ELEMENT, Count = 2)]
        [NUnit.Framework.Test]
        public virtual void BlockWithSetHeightProperties02() {
            String outFileName = destinationFolder + "blockWithSetHeightProperties02.pdf";
            String cmpFileName = sourceFolder + "cmp_blockWithSetHeightProperties02.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDocument);
            Paragraph p = new Paragraph(textByron);
            Div div = new Div();
            div.SetBorder(new SolidBorder(ColorConstants.RED, 2));
            for (int i = 0; i < 5; i++) {
                div.Add(p);
            }
            PdfImageXObject xObject = new PdfImageXObject(ImageDataFactory.Create(sourceFolder + "Desert.jpg"));
            iText.Layout.Element.Image image = new iText.Layout.Element.Image(xObject, 300);
            div.Add(image);
            doc.Add(new Paragraph("Default layout:"));
            doc.Add(div);
            doc.Add(new AreaBreak());
            doc.Add(new Paragraph("Div's height is set shorter than needed:"));
            div.SetHeight(1000);
            doc.Add(div);
            doc.Add(new AreaBreak());
            doc.Add(new Paragraph("Div's min height is set shorter than needed:"));
            div.DeleteOwnProperty(Property.HEIGHT);
            div.SetMinHeight(1000);
            doc.Add(div);
            doc.Add(new AreaBreak());
            doc.Add(new Paragraph("Div's max height is set shorter than needed:"));
            div.DeleteOwnProperty(Property.MIN_HEIGHT);
            div.SetMaxHeight(1000);
            doc.Add(div);
            doc.Add(new AreaBreak());
            doc.Add(new Paragraph("Div's height is set bigger than needed:"));
            div.DeleteOwnProperty(Property.MAX_HEIGHT);
            div.SetHeight(2500);
            doc.Add(div);
            doc.Add(new AreaBreak());
            doc.Add(new Paragraph("Div's min height is set bigger than needed:"));
            div.DeleteOwnProperty(Property.HEIGHT);
            div.SetMinHeight(2500);
            doc.Add(div);
            doc.Add(new AreaBreak());
            doc.Add(new Paragraph("Div's max height is set bigger than needed:"));
            div.DeleteOwnProperty(Property.MIN_HEIGHT);
            div.SetMaxHeight(2500);
            doc.Add(div);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        [LogMessage(iText.IO.Logs.IoLogMessageConstant.CLIP_ELEMENT, Count = 3)]
        [NUnit.Framework.Test]
        public virtual void BlockWithSetHeightProperties03() {
            //Relative height declaration tests
            String outFileName = destinationFolder + "blockWithSetHeightProperties03.pdf";
            String cmpFileName = sourceFolder + "cmp_blockWithSetHeightProperties03.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDocument);
            float parentHeight = 650;
            Div d = new Div();
            d.Add(new Paragraph(textByron));
            d.SetBorder(new SolidBorder(0.5f));
            doc.Add(new Paragraph("Default layout:"));
            Div parent = new Div();
            parent.SetHeight(parentHeight);
            parent.SetBorder(new SolidBorder(ColorConstants.BLUE, 1));
            parent.Add(d);
            doc.Add(parent);
            doc.Add(new AreaBreak());
            doc.Add(new Paragraph("Paragraph's height is set to 80% of the parent"));
            parent = new Div();
            parent.SetHeight(parentHeight);
            parent.SetBorder(new SolidBorder(ColorConstants.BLUE, 1));
            d.SetProperty(Property.HEIGHT, UnitValue.CreatePercentValue(80f));
            parent.Add(d);
            doc.Add(parent);
            doc.Add(new AreaBreak());
            doc.Add(new Paragraph("Paragraph's height is set to 150% of the parent"));
            parent = new Div();
            parent.SetHeight(parentHeight);
            parent.SetBorder(new SolidBorder(ColorConstants.BLUE, 1));
            d.SetProperty(Property.HEIGHT, UnitValue.CreatePercentValue(150f));
            parent.Add(d);
            doc.Add(parent);
            doc.Add(new AreaBreak());
            doc.Add(new Paragraph("Paragraph's height is set to 10% of the parent"));
            parent = new Div();
            parent.SetHeight(parentHeight);
            parent.SetBorder(new SolidBorder(ColorConstants.BLUE, 1));
            d.SetProperty(Property.HEIGHT, UnitValue.CreatePercentValue(10f));
            parent.Add(d);
            doc.Add(parent);
            doc.Add(new AreaBreak());
            doc.Add(new Paragraph("Paragraph's height is set to 40% of the parent and two paragraphs are added"));
            parent = new Div();
            parent.SetHeight(parentHeight);
            parent.SetBorder(new SolidBorder(ColorConstants.BLUE, 1));
            d.SetProperty(Property.HEIGHT, UnitValue.CreatePercentValue(40f));
            parent.Add(d);
            parent.Add(d);
            doc.Add(parent);
            doc.Add(new AreaBreak());
            doc.Add(new Paragraph("Paragraph's height is set to 50% of the parent and two paragraphs are added"));
            parent = new Div();
            parent.SetHeight(parentHeight);
            parent.SetBorder(new SolidBorder(ColorConstants.BLUE, 1));
            d.SetProperty(Property.HEIGHT, UnitValue.CreatePercentValue(50f));
            parent.Add(d);
            parent.Add(d);
            doc.Add(parent);
            doc.Add(new AreaBreak());
            doc.Add(new Paragraph("Paragraph's min height is set to 80% of the parent"));
            parent = new Div();
            parent.SetHeight(parentHeight);
            parent.SetBorder(new SolidBorder(ColorConstants.BLUE, 1));
            d.SetProperty(Property.MIN_HEIGHT, UnitValue.CreatePercentValue(80f));
            parent.Add(d);
            doc.Add(parent);
            doc.Add(new AreaBreak());
            doc.Add(new Paragraph("Paragraph's max height is set to 30% of the parent"));
            parent = new Div();
            parent.SetHeight(parentHeight);
            parent.SetBorder(new SolidBorder(ColorConstants.BLUE, 1));
            //Min-height trumps max-height, so we have to remove it when re-using the div
            d.DeleteOwnProperty(Property.MIN_HEIGHT);
            d.SetProperty(Property.MAX_HEIGHT, UnitValue.CreatePercentValue(30f));
            parent.Add(d);
            doc.Add(parent);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        [LogMessage(iText.IO.Logs.IoLogMessageConstant.CLIP_ELEMENT, Count = 3)]
        [NUnit.Framework.Test]
        public virtual void BlockWithSetHeightProperties04() {
            //Relative height declaration tests
            String outFileName = destinationFolder + "blockWithSetHeightProperties04.pdf";
            String cmpFileName = sourceFolder + "cmp_blockWithSetHeightProperties04.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDocument);
            float parentHeight = 650;
            Paragraph p = new Paragraph();
            p.Add(new Text(textByron));
            p.SetBorder(new SolidBorder(0.5f));
            doc.Add(new Paragraph("Default layout:"));
            Div parent = new Div();
            parent.SetHeight(parentHeight);
            parent.SetBorder(new SolidBorder(ColorConstants.BLUE, 1));
            parent.Add(p);
            doc.Add(parent);
            doc.Add(new AreaBreak());
            doc.Add(new Paragraph("Paragraph's height is set to 80% of the parent"));
            parent = new Div();
            parent.SetHeight(parentHeight);
            parent.SetBorder(new SolidBorder(ColorConstants.BLUE, 1));
            p.SetProperty(Property.HEIGHT, UnitValue.CreatePercentValue(80f));
            parent.Add(p);
            doc.Add(parent);
            doc.Add(new AreaBreak());
            doc.Add(new Paragraph("Paragraph's height is set to 150% of the parent"));
            parent = new Div();
            parent.SetHeight(parentHeight);
            parent.SetBorder(new SolidBorder(ColorConstants.BLUE, 1));
            p.SetProperty(Property.HEIGHT, UnitValue.CreatePercentValue(150f));
            parent.Add(p);
            doc.Add(parent);
            doc.Add(new AreaBreak());
            doc.Add(new Paragraph("Paragraph's height is set to 10% of the parent"));
            parent = new Div();
            parent.SetHeight(parentHeight);
            parent.SetBorder(new SolidBorder(ColorConstants.BLUE, 1));
            p.SetProperty(Property.HEIGHT, UnitValue.CreatePercentValue(10f));
            parent.Add(p);
            doc.Add(parent);
            doc.Add(new AreaBreak());
            doc.Add(new Paragraph("Paragraph's height is set to 40% of the parent and two paragraphs are added"));
            parent = new Div();
            parent.SetHeight(parentHeight);
            parent.SetBorder(new SolidBorder(ColorConstants.BLUE, 1));
            p.SetProperty(Property.HEIGHT, UnitValue.CreatePercentValue(40f));
            parent.Add(p);
            parent.Add(p);
            doc.Add(parent);
            doc.Add(new AreaBreak());
            doc.Add(new Paragraph("Paragraph's height is set to 50% of the parent and two paragraphs are added"));
            parent = new Div();
            parent.SetHeight(parentHeight);
            parent.SetBorder(new SolidBorder(ColorConstants.BLUE, 1));
            p.SetProperty(Property.HEIGHT, UnitValue.CreatePercentValue(50f));
            parent.Add(p);
            parent.Add(p);
            doc.Add(parent);
            doc.Add(new AreaBreak());
            doc.Add(new Paragraph("Paragraph's min height is set to 80% of the parent"));
            parent = new Div();
            parent.SetHeight(parentHeight);
            parent.SetBorder(new SolidBorder(ColorConstants.BLUE, 1));
            p.SetProperty(Property.MIN_HEIGHT, UnitValue.CreatePercentValue(80f));
            parent.Add(p);
            doc.Add(parent);
            doc.Add(new AreaBreak());
            doc.Add(new Paragraph("Paragraph's max height is set to 30% of the parent"));
            parent = new Div();
            parent.SetHeight(parentHeight);
            parent.SetBorder(new SolidBorder(ColorConstants.BLUE, 1));
            //Min-height trumps max, so we have to remove it when re-using the paragraph
            p.DeleteOwnProperty(Property.MIN_HEIGHT);
            p.SetProperty(Property.MAX_HEIGHT, UnitValue.CreatePercentValue(30f));
            parent.Add(p);
            doc.Add(parent);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void OverflowTest01() {
            // TODO DEVSIX-1373
            String outFileName = destinationFolder + "overflowTest01.pdf";
            String cmpFileName = sourceFolder + "cmp_overflowTest01.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDocument);
            Paragraph explanation = new Paragraph("In this sample iText will not try to fit text in container's width, because overflow property is set. However no text is hidden."
                );
            doc.Add(explanation);
            Paragraph p = new Paragraph(textByronNarrow);
            p.SetWidth(200);
            p.SetBorder(new SolidBorder(ColorConstants.BLUE, 1));
            p.SetProperty(Property.OVERFLOW_X, OverflowPropertyValue.HIDDEN);
            Div div = new Div();
            div.SetWidth(100);
            div.SetBorder(new SolidBorder(ColorConstants.BLACK, 1));
            div.SetProperty(Property.OVERFLOW_X, OverflowPropertyValue.VISIBLE);
            div.Add(p);
            doc.Add(div);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void OverflowTest02() {
            String outFileName = destinationFolder + "overflowTest02.pdf";
            String cmpFileName = sourceFolder + "cmp_overflowTest02.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDocument);
            Paragraph p = new Paragraph();
            p.SetWidth(200);
            p.SetHeight(100);
            p.SetBorder(new SolidBorder(ColorConstants.BLUE, 1));
            p.SetBackgroundColor(ColorConstants.YELLOW);
            for (int i = 0; i < 10; i++) {
                p.Add(textByronNarrow);
            }
            p.SetProperty(Property.OVERFLOW_Y, OverflowPropertyValue.VISIBLE);
            doc.Add(p);
            doc.Add(new Paragraph("Hello!!!").SetBackgroundColor(ColorConstants.RED));
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void OverflowTest03() {
            String outFileName = destinationFolder + "overflowTest03.pdf";
            String cmpFileName = sourceFolder + "cmp_overflowTest03.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDocument);
            Paragraph p = new Paragraph();
            p.SetWidth(1400);
            p.SetHeight(1400);
            p.SetBorder(new SolidBorder(ColorConstants.BLUE, 1));
            p.SetBackgroundColor(ColorConstants.YELLOW);
            for (int i = 0; i < 100; i++) {
                p.Add(textByronNarrow);
            }
            p.SetProperty(Property.OVERFLOW_Y, OverflowPropertyValue.VISIBLE);
            p.SetProperty(Property.OVERFLOW_X, OverflowPropertyValue.VISIBLE);
            doc.Add(p);
            doc.Add(new Paragraph("Hello!!!").SetBackgroundColor(ColorConstants.RED));
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        [NUnit.Framework.Ignore("DEVSIX-1375")]
        [NUnit.Framework.Test]
        public virtual void OverflowTest04() {
            String outFileName = destinationFolder + "overflowTest04.pdf";
            String cmpFileName = sourceFolder + "cmp_overflowTest04.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            iText.Layout.Element.Image image = new iText.Layout.Element.Image(ImageDataFactory.Create(sourceFolder + "Desert.jpg"
                ));
            image.SetWidth(200);
            Document doc = new Document(pdfDocument);
            Paragraph p = new Paragraph();
            p.SetRotationAngle(Math.PI / 2);
            p.SetWidth(100);
            p.SetHeight(100);
            p.SetBorder(new SolidBorder(ColorConstants.BLUE, 1));
            p.SetProperty(Property.OVERFLOW_Y, OverflowPropertyValue.VISIBLE);
            p.SetProperty(Property.OVERFLOW_X, OverflowPropertyValue.VISIBLE);
            p.Add(image);
            doc.Add(p);
            doc.Add(new Paragraph("Hello!!!").SetBackgroundColor(ColorConstants.RED));
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        [NUnit.Framework.Ignore("DEVSIX-1373")]
        [NUnit.Framework.Test]
        public virtual void OverflowTest05() {
            String outFileName = destinationFolder + "overflowTest05.pdf";
            String cmpFileName = sourceFolder + "cmp_overflowTest05.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDocument);
            Div div = new Div();
            div.SetWidth(100);
            div.SetHeight(150);
            div.SetBackgroundColor(ColorConstants.GREEN);
            div.SetProperty(Property.OVERFLOW_X, OverflowPropertyValue.VISIBLE);
            div.SetProperty(Property.OVERFLOW_Y, OverflowPropertyValue.VISIBLE);
            List list = new List();
            list.Add("Make Greeeeeeeeeetzky Great Again");
            list.Add("Greeeeeeeeeetzky Great Again Make");
            list.Add("Great Again Make Greeeeeeeeeetzky");
            list.Add("Again Make Greeeeeeeeeetzky Great");
            div.Add(list);
            doc.Add(div);
            doc.Add(new Paragraph("Hello!!!").SetBackgroundColor(ColorConstants.RED));
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        [NUnit.Framework.Ignore("DEVSIX-1373")]
        [NUnit.Framework.Test]
        public virtual void OverflowTest06() {
            String outFileName = destinationFolder + "overflowTest06.pdf";
            String cmpFileName = sourceFolder + "cmp_overflowTest06.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDocument);
            Div div = new Div();
            div.SetWidth(100);
            div.SetHeight(100);
            div.SetBackgroundColor(ColorConstants.GREEN);
            div.SetProperty(Property.OVERFLOW_Y, OverflowPropertyValue.VISIBLE);
            div.Add(new Paragraph(textByron));
            doc.Add(div);
            doc.Add(new Paragraph("Hello!!!").SetBackgroundColor(ColorConstants.RED));
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void BlockFillAvailableArea01() {
            String outFileName = destinationFolder + "blockFillAvailableArea01.pdf";
            String cmpFileName = sourceFolder + "cmp_blockFillAvailableArea01.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            String textByron = "When a man hath no freedom to fight for at home,\n" + "    Let him combat for that of his neighbours;\n"
                 + "Let him think of the glories of Greece and of Rome,\n" + "    And get knocked on the head for his labours.\n"
                 + "\n" + "To do good to Mankind is the chivalrous plan,\n" + "    And is always as nobly requited;\n"
                 + "Then battle for Freedom wherever you can,\n" + "    And, if not shot or hanged, you'll get knighted."
                 + "To do good to Mankind is the chivalrous plan,\n" + "    And is always as nobly requited;\n" + "Then battle for Freedom wherever you can,\n"
                 + "    And, if not shot or hanged, you'll get knighted." + "To do good to Mankind is the chivalrous plan,\n"
                 + "    And is always as nobly requited;\n" + "Then battle for Freedom wherever you can,\n" + "    And, if not shot or hanged, you'll get knighted.";
            textByron = textByron + textByron;
            Document doc = new Document(pdfDocument);
            DeviceRgb blue = new DeviceRgb(80, 114, 153);
            Div text = new Div().Add(new Paragraph(textByron));
            Div image = new Div().Add(new iText.Layout.Element.Image(ImageDataFactory.Create(sourceFolder + "Desert.jpg"
                )).SetHeight(500).SetAutoScaleWidth(true));
            doc.Add(new Div().Add(new Paragraph("Fill on split").SetFontSize(30).SetFontColor(blue).SetTextAlignment(TextAlignment
                .CENTER)).SetVerticalAlignment(VerticalAlignment.MIDDLE).SetFillAvailableArea(true)).Add(new AreaBreak
                ());
            doc.Add(new Paragraph("text").SetFontSize(18).SetFontColor(blue));
            Div div = CreateDiv(text, textByron, blue, true, false, true);
            doc.Add(div);
            doc.Add(new AreaBreak());
            doc.Add(new Paragraph("image").SetFontSize(18).SetFontColor(blue));
            div = CreateDiv(image, textByron, blue, false, false, true);
            doc.Add(div);
            doc.Add(new AreaBreak());
            doc.Add(new Div().Add(new Paragraph("Fill always").SetFontSize(30).SetFontColor(blue).SetTextAlignment(TextAlignment
                .CENTER)).SetVerticalAlignment(VerticalAlignment.MIDDLE).SetFillAvailableArea(true)).Add(new AreaBreak
                ());
            doc.Add(new Paragraph("text").SetFontSize(18).SetFontColor(blue));
            div = CreateDiv(text, textByron, blue, true, true, false);
            doc.Add(div);
            doc.Add(new Paragraph("image").SetFontSize(18).SetFontColor(blue));
            div = CreateDiv(image, textByron, blue, false, true, false);
            doc.Add(div);
            doc.Add(new Div().Add(new Paragraph("No fill").SetFontSize(30).SetFontColor(blue).SetTextAlignment(TextAlignment
                .CENTER)).SetVerticalAlignment(VerticalAlignment.MIDDLE).SetFillAvailableArea(true)).Add(new AreaBreak
                ());
            doc.Add(new Paragraph("text").SetFontSize(18).SetFontColor(blue));
            div = CreateDiv(text, textByron, blue, true, false, false);
            doc.Add(div);
            doc.Add(new AreaBreak());
            doc.Add(new Paragraph("image").SetFontSize(18).SetFontColor(blue));
            div = CreateDiv(image, textByron, blue, false, false, false);
            doc.Add(div);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void MarginsBordersPaddingOverflow01() {
            String outFileName = destinationFolder + "marginsBordersPaddingOverflow01.pdf";
            String cmpFileName = sourceFolder + "cmp_marginsBordersPaddingOverflow01.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDocument);
            Div div = new Div();
            div.SetHeight(760).SetBackgroundColor(ColorConstants.DARK_GRAY);
            doc.Add(div);
            Div div1 = new Div().SetMarginTop(42).SetMarginBottom(42).SetBackgroundColor(ColorConstants.BLUE).SetHeight
                (1);
            doc.Add(div1);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void MarginsBordersPaddingOverflow02() {
            String outFileName = destinationFolder + "marginsBordersPaddingOverflow02.pdf";
            String cmpFileName = sourceFolder + "cmp_marginsBordersPaddingOverflow02.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDocument);
            // TODO DEVSIX-1092 div with fixed height is bigger than 60pt
            Div div = new Div();
            div.SetHeight(60).SetBackgroundColor(ColorConstants.DARK_GRAY);
            Div div1 = new Div().SetMarginTop(200).SetMarginBottom(200).SetBorder(new SolidBorder(6));
            div.Add(div1);
            doc.Add(div);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void MarginsBordersPaddingOverflow03() {
            String outFileName = destinationFolder + "marginsBordersPaddingOverflow03.pdf";
            String cmpFileName = sourceFolder + "cmp_marginsBordersPaddingOverflow03.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDocument);
            Div div = new Div();
            div.SetHeight(710).SetBackgroundColor(ColorConstants.DARK_GRAY);
            doc.Add(div);
            // TODO DEVSIX-1092 this element is below first page visible area
            Div div1 = new Div().SetMarginTop(200).SetMarginBottom(200).SetBorder(new SolidBorder(6));
            doc.Add(div1);
            doc.Add(new AreaBreak());
            // TODO DEVSIX-1092 same with this one the second page
            SolidBorder border = new SolidBorder(400);
            Div div2 = new Div().SetBorderTop(border).SetBorderBottom(border);
            doc.Add(div);
            doc.Add(div2);
            doc.Add(new AreaBreak());
            // TODO DEVSIX-1092 same with this one the third page
            Div div3 = new Div().SetBorder(new SolidBorder(6)).SetPaddingTop(400).SetPaddingBottom(400);
            doc.Add(div);
            doc.Add(div3);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void BorderRadiusTest01() {
            String outFileName = destinationFolder + "borderRadiusTest01.pdf";
            String cmpFileName = sourceFolder + "cmp_borderRadiusTest01.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDocument);
            Div div = new Div();
            Style divStyle = new Style().SetHeight(500).SetWidth(500).SetBackgroundColor(ColorConstants.BLUE);
            divStyle.SetBorderRadius(new BorderRadius(50));
            // solid
            div.AddStyle(divStyle);
            div.SetBorderTop(new SolidBorder(ColorConstants.RED, 20)).SetBorderRight(new SolidBorder(ColorConstants.YELLOW
                , 20));
            doc.Add(div);
            doc.Add(new AreaBreak());
            // dashed
            div = new Div();
            div.AddStyle(divStyle);
            div.SetBorderTop(new DashedBorder(ColorConstants.RED, 20)).SetBorderRight(new DashedBorder(ColorConstants.
                YELLOW, 20));
            doc.Add(div);
            doc.Add(new AreaBreak());
            // dotted
            div = new Div();
            div.AddStyle(divStyle);
            div.SetBorderTop(new DottedBorder(ColorConstants.RED, 20)).SetBorderRight(new DottedBorder(ColorConstants.
                YELLOW, 20));
            doc.Add(div);
            doc.Add(new AreaBreak());
            // round dotted
            div = new Div();
            div.AddStyle(divStyle);
            div.SetBorderTop(new RoundDotsBorder(ColorConstants.RED, 20)).SetBorderRight(new RoundDotsBorder(ColorConstants
                .YELLOW, 20));
            doc.Add(div);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void BorderRadiusTest02() {
            String outFileName = destinationFolder + "borderRadiusTest02.pdf";
            String cmpFileName = sourceFolder + "cmp_borderRadiusTest02.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDocument);
            // width and height > 2 * radius
            Div div = new Div();
            div.SetHeight(500).SetWidth(500).SetBackgroundColor(ColorConstants.GREEN).SetBorderRadius(new BorderRadius
                (100));
            doc.Add(div);
            doc.Add(new AreaBreak());
            // 2 * radius > width and height > radius
            div = new Div();
            div.SetHeight(150).SetWidth(150).SetBackgroundColor(ColorConstants.GREEN).SetBorderRadius(new BorderRadius
                (100));
            doc.Add(div);
            doc.Add(new AreaBreak());
            // radius > width and height
            div = new Div();
            div.SetHeight(50).SetWidth(50).SetBackgroundColor(ColorConstants.GREEN).SetBorderRadius(new BorderRadius(100
                ));
            doc.Add(div);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void BorderRadiusTest03() {
            String outFileName = destinationFolder + "borderRadiusTest03.pdf";
            String cmpFileName = sourceFolder + "cmp_borderRadiusTest03.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDocument);
            Div div = new Div();
            Style divStyle = new Style().SetHeight(500).SetWidth(500).SetBackgroundColor(ColorConstants.GREEN);
            divStyle.SetBorderRadius(new BorderRadius(200));
            // solid
            div.AddStyle(divStyle);
            div.SetBorderLeft(new SolidBorder(ColorConstants.MAGENTA, 100)).SetBorderBottom(new SolidBorder(ColorConstants
                .BLACK, 100)).SetBorderTop(new SolidBorder(ColorConstants.RED, 100)).SetBorderRight(new SolidBorder(ColorConstants
                .BLUE, 100));
            doc.Add(div);
            doc.Add(new AreaBreak());
            // dashed
            div = new Div();
            div.AddStyle(divStyle);
            div.SetBorderLeft(new DashedBorder(ColorConstants.MAGENTA, 100)).SetBorderBottom(new DashedBorder(ColorConstants
                .BLACK, 100)).SetBorderTop(new DashedBorder(ColorConstants.RED, 100)).SetBorderRight(new DashedBorder(
                ColorConstants.BLUE, 100));
            doc.Add(div);
            doc.Add(new AreaBreak());
            // dotted
            div = new Div();
            div.AddStyle(divStyle);
            div.SetBorderLeft(new DottedBorder(ColorConstants.MAGENTA, 100)).SetBorderBottom(new DottedBorder(ColorConstants
                .BLACK, 100)).SetBorderTop(new DottedBorder(ColorConstants.RED, 100)).SetBorderRight(new DottedBorder(
                ColorConstants.BLUE, 100));
            doc.Add(div);
            doc.Add(new AreaBreak());
            // round dotted
            div = new Div();
            div.AddStyle(divStyle);
            div.SetBorderLeft(new RoundDotsBorder(ColorConstants.MAGENTA, 100)).SetBorderBottom(new RoundDotsBorder(ColorConstants
                .BLACK, 100)).SetBorderTop(new RoundDotsBorder(ColorConstants.RED, 100)).SetBorderRight(new RoundDotsBorder
                (ColorConstants.BLUE, 100));
            doc.Add(div);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void BorderRadiusTest04() {
            String outFileName = destinationFolder + "borderRadiusTest04.pdf";
            String cmpFileName = sourceFolder + "cmp_borderRadiusTest04.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDocument);
            Div div = new Div();
            Style divStyle = new Style().SetHeight(120).SetWidth(120).SetBackgroundColor(ColorConstants.MAGENTA);
            divStyle.SetBorderRadius(new BorderRadius(90));
            // solid
            div.AddStyle(divStyle);
            div.SetBorderBottom(new SolidBorder(ColorConstants.RED, 30)).SetBorderLeft(new SolidBorder(ColorConstants.
                GREEN, 15)).SetBorderTop(new SolidBorder(ColorConstants.BLACK, 60)).SetBorderRight(new SolidBorder(ColorConstants
                .BLUE, 150));
            doc.Add(div);
            doc.Add(new AreaBreak());
            // dashed
            div = new Div();
            div.AddStyle(divStyle);
            div.SetBorderBottom(new DashedBorder(ColorConstants.RED, 30)).SetBorderLeft(new DashedBorder(ColorConstants
                .GREEN, 15)).SetBorderTop(new DashedBorder(ColorConstants.BLACK, 60)).SetBorderRight(new DashedBorder(
                ColorConstants.BLUE, 150));
            doc.Add(div);
            doc.Add(new AreaBreak());
            // dotted
            div = new Div();
            div.AddStyle(divStyle);
            div.SetBorderBottom(new DottedBorder(ColorConstants.RED, 30)).SetBorderLeft(new DottedBorder(ColorConstants
                .GREEN, 15)).SetBorderTop(new DottedBorder(ColorConstants.BLACK, 60)).SetBorderRight(new DottedBorder(
                ColorConstants.BLUE, 150));
            doc.Add(div);
            doc.Add(new AreaBreak());
            // round dotted
            div = new Div();
            div.AddStyle(divStyle);
            div.SetBorderBottom(new RoundDotsBorder(ColorConstants.RED, 30)).SetBorderLeft(new RoundDotsBorder(ColorConstants
                .GREEN, 15)).SetBorderTop(new RoundDotsBorder(ColorConstants.BLACK, 60)).SetBorderRight(new RoundDotsBorder
                (ColorConstants.BLUE, 150));
            doc.Add(div);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void BorderRadiusTest05() {
            String outFileName = destinationFolder + "borderRadiusTest05.pdf";
            String cmpFileName = sourceFolder + "cmp_borderRadiusTest05.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDocument);
            Div div = new Div();
            Style divStyle = new Style().SetHeight(460).SetWidth(360).SetBackgroundColor(ColorConstants.MAGENTA);
            divStyle.SetBorderRadius(new BorderRadius(100));
            // solid
            div.AddStyle(divStyle);
            div.SetBorderBottom(new SolidBorder(ColorConstants.RED, 30)).SetBorderLeft(new SolidBorder(ColorConstants.
                BLUE, 15)).SetBorderTop(new SolidBorder(ColorConstants.GREEN, 60)).SetBorderRight(new SolidBorder(ColorConstants
                .YELLOW, 150));
            doc.Add(div);
            doc.Add(new AreaBreak());
            // dashed
            div = new Div();
            div.AddStyle(divStyle);
            div.SetBorderBottom(new DashedBorder(ColorConstants.RED, 30)).SetBorderLeft(new DashedBorder(ColorConstants
                .BLUE, 15)).SetBorderTop(new DashedBorder(ColorConstants.GREEN, 60)).SetBorderRight(new DashedBorder(ColorConstants
                .YELLOW, 150));
            doc.Add(div);
            doc.Add(new AreaBreak());
            // dotted
            div = new Div();
            div.AddStyle(divStyle);
            div.SetBorderBottom(new DottedBorder(ColorConstants.RED, 30)).SetBorderLeft(new DottedBorder(ColorConstants
                .BLUE, 15)).SetBorderTop(new DottedBorder(ColorConstants.GREEN, 60)).SetBorderRight(new DottedBorder(ColorConstants
                .YELLOW, 150));
            doc.Add(div);
            doc.Add(new AreaBreak());
            // round dotted
            div = new Div();
            div.AddStyle(divStyle);
            div.SetBorderBottom(new RoundDotsBorder(ColorConstants.RED, 30)).SetBorderLeft(new RoundDotsBorder(ColorConstants
                .BLUE, 15)).SetBorderTop(new RoundDotsBorder(ColorConstants.GREEN, 60)).SetBorderRight(new RoundDotsBorder
                (ColorConstants.YELLOW, 150));
            doc.Add(div);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void BorderRadiusTest06() {
            String outFileName = destinationFolder + "borderRadiusTest06.pdf";
            String cmpFileName = sourceFolder + "cmp_borderRadiusTest06.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDocument);
            Div div = new Div();
            Style divStyle = new Style().SetHeight(460).SetWidth(360).SetBackgroundColor(ColorConstants.MAGENTA);
            divStyle.SetBorderRadius(new BorderRadius(40, 120));
            // solid
            div.AddStyle(divStyle);
            div.SetBorderBottom(new SolidBorder(ColorConstants.RED, 30)).SetBorderLeft(new SolidBorder(ColorConstants.
                BLUE, 15)).SetBorderTop(new SolidBorder(ColorConstants.GREEN, 60)).SetBorderRight(new SolidBorder(ColorConstants
                .YELLOW, 150));
            doc.Add(div);
            doc.Add(new AreaBreak());
            // dashed
            div = new Div();
            div.AddStyle(divStyle);
            div.SetBorderBottom(new DashedBorder(ColorConstants.RED, 30)).SetBorderLeft(new DashedBorder(ColorConstants
                .BLUE, 15)).SetBorderTop(new DashedBorder(ColorConstants.GREEN, 60)).SetBorderRight(new DashedBorder(ColorConstants
                .YELLOW, 150));
            doc.Add(div);
            doc.Add(new AreaBreak());
            // dotted
            div = new Div();
            div.AddStyle(divStyle);
            div.SetBorderBottom(new DottedBorder(ColorConstants.RED, 30)).SetBorderLeft(new DottedBorder(ColorConstants
                .BLUE, 15)).SetBorderTop(new DottedBorder(ColorConstants.GREEN, 60)).SetBorderRight(new DottedBorder(ColorConstants
                .YELLOW, 150));
            doc.Add(div);
            doc.Add(new AreaBreak());
            // round dotted
            div = new Div();
            div.AddStyle(divStyle);
            div.SetBorderBottom(new RoundDotsBorder(ColorConstants.RED, 30)).SetBorderLeft(new RoundDotsBorder(ColorConstants
                .BLUE, 15)).SetBorderTop(new RoundDotsBorder(ColorConstants.GREEN, 60)).SetBorderRight(new RoundDotsBorder
                (ColorConstants.YELLOW, 150));
            doc.Add(div);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void HeightShouldBeIncreasedUpToSetHeightTest01() {
            // TODO DEVSIX-1895 if height bigger than min-height is set,
            // then the element's height should be increased up to height
            String outFileName = destinationFolder + "heightShouldBeIncreasedUpToSetHeightTest01.pdf";
            String cmpFileName = sourceFolder + "cmp_heightShouldBeIncreasedUpToSetHeightTest01.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDocument);
            Div div = new Div().SetWidth(100).SetMinHeight(100).SetHeight(200).SetBackgroundColor(ColorConstants.BLUE);
            doc.Add(div);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        [NUnit.Framework.Test]
        [NUnit.Framework.Ignore("DEVSIX-1897")]
        public virtual void ParagraphVerticalAlignmentTest01() {
            String outFileName = destinationFolder + "paragraphVerticalAlignmentTest01.pdf";
            String cmpFileName = sourceFolder + "paragraphVerticalAlignmentTest01.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDocument);
            FontProvider fontProvider = new FontProvider();
            fontProvider.AddStandardPdfFonts();
            doc.SetFontProvider(fontProvider);
            String loremIpsum = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.";
            doc.Add(new Paragraph(loremIpsum).SetHeight(100).SetVerticalAlignment(VerticalAlignment.MIDDLE).SetBorder(
                new SolidBorder(3)).SetFontFamily(StandardFonts.TIMES_ROMAN));
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        private Div CreateDiv(Div innerOverflowDiv, String text, DeviceRgb backgroundColor, bool keepTogether, bool
             fillAlways, bool fillOnSplit) {
            Div div = new Div().SetBorder(new DoubleBorder(10)).SetBackgroundColor(new DeviceRgb(216, 243, 255)).SetFillAvailableAreaOnSplit
                (fillOnSplit).SetFillAvailableArea(fillAlways);
            div.Add(new Paragraph(text));
            div.Add(innerOverflowDiv.SetKeepTogether(keepTogether));
            if (backgroundColor != null) {
                innerOverflowDiv.SetBackgroundColor(backgroundColor);
            }
            return div;
        }
    }
}
