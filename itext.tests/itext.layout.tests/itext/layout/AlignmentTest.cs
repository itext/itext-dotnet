/*
This file is part of the iText (R) project.
Copyright (c) 1998-2024 Apryse Group NV
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
using iText.IO.Font;
using iText.IO.Image;
using iText.IO.Util;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Xobject;
using iText.Kernel.Utils;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Layout.Renderer;
using iText.Test;

namespace iText.Layout {
    [NUnit.Framework.Category("IntegrationTest")]
    public class AlignmentTest : ExtendedITextTest {
        private static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/layout/AlignmentTest/";

        private static readonly String DESTINATION_FOLDER = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/layout/AlignmentTest/";

        private static readonly String FONTS_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/layout/fonts/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateOrClearDestinationFolder(DESTINATION_FOLDER);
        }

        [NUnit.Framework.Test]
        public virtual void JustifyAlignmentTest01() {
            String outFileName = DESTINATION_FOLDER + "justifyAlignmentTest01.pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_justifyAlignmentTest01.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document document = new Document(pdfDocument);
            Paragraph paragraph = new Paragraph().SetTextAlignment(TextAlignment.JUSTIFIED);
            for (int i = 0; i < 21; i++) {
                paragraph.Add(new Text("Hello World! Hello People! " + "Hello Sky! Hello Sun! Hello Moon! Hello Stars!").SetBackgroundColor
                    (ColorConstants.RED));
            }
            document.Add(paragraph);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void JustifyAlignmentTest02() {
            String outFileName = DESTINATION_FOLDER + "justifyAlignmentTest02.pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_justifyAlignmentTest02.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document document = new Document(pdfDocument);
            Paragraph paragraph = new Paragraph().SetTextAlignment(TextAlignment.JUSTIFIED);
            paragraph.Add(new Text("Hello World!")).Add(new Text(" ")).Add(new Text("Hello People! ")).Add("End");
            document.Add(paragraph);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void JustifyAlignmentTest03() {
            String outFileName = DESTINATION_FOLDER + "justifyAlignmentTest03.pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_justifyAlignmentTest03.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document document = new Document(pdfDocument);
            Paragraph paragraph = new Paragraph().SetTextAlignment(TextAlignment.JUSTIFIED);
            for (int i = 0; i < 21; i++) {
                paragraph.Add(new Text("Hello World! Hello People! " + "Hello Sky! Hello Sun! Hello Moon! Hello Stars!").SetBorder
                    (new SolidBorder(ColorConstants.GREEN, 0.1f))).SetMultipliedLeading(1);
            }
            document.Add(paragraph);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void JustifyAlignmentTest04() {
            String outFileName = DESTINATION_FOLDER + "justifyAlignmentTest04.pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_justifyAlignmentTest04.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document document = new Document(pdfDocument);
            Paragraph paragraph = new Paragraph().SetTextAlignment(TextAlignment.JUSTIFIED);
            for (int i = 0; i < 21; i++) {
                paragraph.Add(new Text("Hello World! Hello People! " + "Hello Sky! Hello Sun! Hello Moon! Hello Stars!")).
                    SetFixedLeading(24);
            }
            document.Add(paragraph);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void JustifyAlignmentForcedNewlinesTest01() {
            String outFileName = DESTINATION_FOLDER + "justifyAlignmentForcedNewlinesTest01.pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_justifyAlignmentForcedNewlinesTest01.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document document = new Document(pdfDocument);
            Paragraph paragraph = new Paragraph().SetTextAlignment(TextAlignment.JUSTIFIED);
            paragraph.Add("Video provides a powerful way to help you prove your point. When you click Online Video, you can paste in the embed code for the video you want to add. You can also type a keyword to search online for the video that best fits your document.\n"
                 + "To make your document look professionally produced, Word provides header, footer, cover page, and text box designs that complement each other. For example, you can add a matching cover page, header, and sidebar. Click Insert and then choose the elements you want from the different galleries.\n"
                 + "Themes and styles also help keep your document coordinated. When you click Design and choose a new Theme, the pictures, charts, and SmartArt graphics change to match your new theme. When you apply styles, your headings change to match the new theme.\n"
                 + "Save time in Word with new buttons that show up where you need them. To change the way a picture fits in your document, click it and a button for layout options appears next to it. When you work on a table, click where you want to add a row or a column, and then click the plus sign.\n"
                 + "Reading is easier, too, in the new Reading view. You can collapse parts of the document and focus on the text you want. If you need to stop reading before you reach the end, Word remembers where you left off - even on another device.\n"
                );
            document.Add(paragraph);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void JustifyAllTest01() {
            String outFileName = DESTINATION_FOLDER + "justifyAllTest01.pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_justifyAllTest01.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document document = new Document(pdfDocument);
            Paragraph paragraph = new Paragraph().SetTextAlignment(TextAlignment.JUSTIFIED_ALL);
            paragraph.Add("Video provides a powerful way to help you prove your point. When you click Online Video, you can paste in the embed code for the video you want to add. You can also type a keyword to search online for the video that best fits your document.\n"
                 + "To make your document look professionally produced, Word provides header, footer, cover page, and text box designs that complement each other. For example, you can add a matching cover page, header, and sidebar. Click Insert and then choose the elements you want from the different galleries.\n"
                 + "Themes and styles also help keep your document coordinated. When you click Design and choose a new Theme, the pictures, charts, and SmartArt graphics change to match your new theme. When you apply styles, your headings change to match the new theme.\n"
                 + "Save time in Word with new buttons that show up where you need them. To change the way a picture fits in your document, click it and a button for layout options appears next to it. When you work on a table, click where you want to add a row or a column, and then click the plus sign.\n"
                 + "Reading is easier, too, in the new Reading view. You can collapse parts of the document and focus on the text you want. If you need to stop reading before you reach the end, Word remembers where you left off - even on another device.\n"
                );
            document.Add(paragraph);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void JustifyAllTest02() {
            String outFileName = DESTINATION_FOLDER + "justifyAllTest02.pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_justifyAllTest02.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document document = new Document(pdfDocument);
            PdfFont type0 = PdfFontFactory.CreateFont(SOURCE_FOLDER + "/../fonts/NotoSans-Regular.ttf", PdfEncodings.IDENTITY_H
                );
            PdfFont simpleFont = PdfFontFactory.CreateFont(SOURCE_FOLDER + "/../fonts/NotoSans-Regular.ttf", PdfFontFactory.EmbeddingStrategy
                .PREFER_EMBEDDED);
            Paragraph paragraph = new Paragraph().SetSpacingRatio(1).SetTextAlignment(TextAlignment.JUSTIFIED_ALL);
            paragraph.Add("If you need to stop reading before you reach the end");
            document.Add(paragraph.SetFont(type0));
            paragraph.SetFont(simpleFont);
            document.Add(paragraph);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void BlockAlignmentTest01() {
            String outFileName = DESTINATION_FOLDER + "blockAlignmentTest01.pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_blockAlignmentTest01.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document document = new Document(pdfDocument);
            List list = new List(ListNumberingType.GREEK_LOWER);
            for (int i = 0; i < 10; i++) {
                list.Add("Item # " + (i + 1));
            }
            list.SetWidth(250);
            list.SetHorizontalAlignment(HorizontalAlignment.CENTER);
            list.SetBackgroundColor(ColorConstants.GREEN);
            document.Add(list);
            list.SetHorizontalAlignment(HorizontalAlignment.RIGHT).SetBackgroundColor(ColorConstants.RED);
            list.SetTextAlignment(TextAlignment.CENTER);
            document.Add(list);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void BlockAlignmentTest02() {
            String outFileName = DESTINATION_FOLDER + "blockAlignmentTest02.pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_blockAlignmentTest02.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document document = new Document(pdfDocument);
            Div div = new Div();
            PdfImageXObject xObject = new PdfImageXObject(ImageDataFactory.CreateJpeg(UrlUtil.ToURL(SOURCE_FOLDER + "Desert.jpg"
                )));
            iText.Layout.Element.Image image1 = new iText.Layout.Element.Image(xObject, 100).SetHorizontalAlignment(HorizontalAlignment
                .RIGHT);
            iText.Layout.Element.Image image2 = new iText.Layout.Element.Image(xObject, 100).SetHorizontalAlignment(HorizontalAlignment
                .CENTER);
            iText.Layout.Element.Image image3 = new iText.Layout.Element.Image(xObject, 100).SetHorizontalAlignment(HorizontalAlignment
                .LEFT);
            div.Add(image1).Add(image2).Add(image3);
            document.Add(div);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void ImageAlignmentTest01() {
            String outFileName = DESTINATION_FOLDER + "imageAlignmentTest01.pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_imageAlignmentTest01.pdf";
            PdfWriter writer = new PdfWriter(outFileName);
            PdfDocument pdfDoc = new PdfDocument(writer);
            Document doc = new Document(pdfDoc);
            PdfImageXObject xObject = new PdfImageXObject(ImageDataFactory.CreateJpeg(UrlUtil.ToURL(SOURCE_FOLDER + "Desert.jpg"
                )));
            iText.Layout.Element.Image image = new iText.Layout.Element.Image(xObject, 100).SetHorizontalAlignment(HorizontalAlignment
                .RIGHT);
            doc.Add(image);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void EmptyLineJustification01() {
            String outFileName = DESTINATION_FOLDER + "emptyLineJustification01.pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_emptyLineJustification01.pdf";
            PdfWriter writer = new PdfWriter(outFileName);
            PdfDocument pdfDoc = new PdfDocument(writer);
            Document doc = new Document(pdfDoc);
            doc.Add(new Paragraph().SetTextAlignment(TextAlignment.JUSTIFIED));
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void FloatAlignmentTest01() {
            String outFileName = DESTINATION_FOLDER + "floatAlignmentTest01.pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_floatAlignmentTest01.pdf";
            PdfWriter writer = new PdfWriter(outFileName);
            PdfDocument pdfDoc = new PdfDocument(writer);
            pdfDoc.SetDefaultPageSize(new PageSize(350, 450));
            Document doc = new Document(pdfDoc);
            AddFloatAndText(doc, FloatPropertyValue.RIGHT);
            AddFloatAndText(doc, FloatPropertyValue.LEFT);
            doc.Add(new AreaBreak());
            doc.Add(new Paragraph("All lines after this one have first line indent = 20. " + "Float left is correct, right is not."
                ));
            doc.SetProperty(Property.FIRST_LINE_INDENT, 20f);
            AddFloatAndText(doc, FloatPropertyValue.RIGHT);
            AddFloatAndText(doc, FloatPropertyValue.LEFT);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff"));
        }

        private static void AddFloatAndText(Document doc, FloatPropertyValue? floatPropertyValue) {
            Div div = new Div();
            div.SetWidth(150).SetHeight(120);
            div.SetProperty(Property.FLOAT, floatPropertyValue);
            div.SetBorder(new SolidBorder(1));
            doc.Add(div);
            doc.Add(new Paragraph("Left aligned.").SetTextAlignment(TextAlignment.LEFT));
            doc.Add(new Paragraph("Right aligned.").SetTextAlignment(TextAlignment.RIGHT));
            doc.Add(new Paragraph("Center aligned.").SetTextAlignment(TextAlignment.CENTER));
            doc.Add(new Paragraph("Justified. " + "The text is laid out using the correct width, but  the alignment value uses the full width."
                ).SetTextAlignment(TextAlignment.JUSTIFIED));
        }

        [NUnit.Framework.Test]
        public virtual void FloatAlignmentTest02() {
            String outFileName = DESTINATION_FOLDER + "floatAlignmentTest02.pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_floatAlignmentTest02.pdf";
            PdfWriter writer = new PdfWriter(outFileName);
            PdfDocument pdfDoc = new PdfDocument(writer);
            pdfDoc.SetDefaultPageSize(new PageSize(350, 450));
            Document doc = new Document(pdfDoc);
            doc.Add(new Paragraph("All lines after this one have first line indent = 20."));
            doc.SetProperty(Property.FIRST_LINE_INDENT, 20f);
            Div div = new Div().Add(new Paragraph("float"));
            div.SetBorder(new SolidBorder(1));
            div.SetProperty(Property.FLOAT, FloatPropertyValue.LEFT);
            AddInlineBlockFloatAndText(doc, div);
            doc.Add(new AreaBreak());
            div.SetProperty(Property.FLOAT, FloatPropertyValue.RIGHT);
            AddInlineBlockFloatAndText(doc, div);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void VerticalAlignmentMiddleTest() {
            //TODO DEVSIX-6490 Support verticalAlignment property in layout
            String outPdf = DESTINATION_FOLDER + "verticalAlignmentMiddle.pdf";
            String cmpPdf = SOURCE_FOLDER + "cmp_verticalAlignmentMiddle.pdf";
            CreateDocumentWithAlignment(outPdf, cmpPdf, VerticalAlignment.MIDDLE);
        }

        [NUnit.Framework.Test]
        public virtual void VerticalAlignmentBottomTest() {
            //TODO DEVSIX-6490 Support verticalAlignment property in layout
            String outPdf = DESTINATION_FOLDER + "verticalAlignmentBottom.pdf";
            String cmpPdf = SOURCE_FOLDER + "cmp_verticalAlignmentBottom.pdf";
            CreateDocumentWithAlignment(outPdf, cmpPdf, VerticalAlignment.BOTTOM);
        }

        [NUnit.Framework.Test]
        public virtual void VerticalAlignmentTopTest() {
            String outPdf = DESTINATION_FOLDER + "verticalAlignmentTop.pdf";
            String cmpPdf = SOURCE_FOLDER + "cmp_verticalAlignmentTop.pdf";
            CreateDocumentWithAlignment(outPdf, cmpPdf, VerticalAlignment.TOP);
        }

        [NUnit.Framework.Test]
        public virtual void InlineVerticalAlignmentTopTest() {
            String outPdf = DESTINATION_FOLDER + "inlineVerticalAlignmentTop.pdf";
            String cmpPdf = SOURCE_FOLDER + "cmp_inlineVerticalAlignmentTop.pdf";
            CreateDocumentWithInlineAlignment(outPdf, cmpPdf, InlineVerticalAlignmentType.TOP);
        }

        [NUnit.Framework.Test]
        public virtual void InlineVerticalAlignmentBottomTest() {
            String outPdf = DESTINATION_FOLDER + "inlineVerticalAlignmentBottom.pdf";
            String cmpPdf = SOURCE_FOLDER + "cmp_inlineVerticalAlignmentBottom.pdf";
            CreateDocumentWithInlineAlignment(outPdf, cmpPdf, InlineVerticalAlignmentType.BOTTOM);
        }

        [NUnit.Framework.Test]
        public virtual void InlineVerticalAlignmentMiddleTest() {
            String outPdf = DESTINATION_FOLDER + "inlineVerticalAlignmentMiddle.pdf";
            String cmpPdf = SOURCE_FOLDER + "cmp_inlineVerticalAlignmentMiddle.pdf";
            CreateDocumentWithInlineAlignment(outPdf, cmpPdf, InlineVerticalAlignmentType.MIDDLE);
        }

        [NUnit.Framework.Test]
        public virtual void InlineVerticalAlignmentBaseLineTest() {
            String outPdf = DESTINATION_FOLDER + "inlineVerticalAlignmentBaseLine.pdf";
            String cmpPdf = SOURCE_FOLDER + "cmp_inlineVerticalAlignmentBaseLine.pdf";
            CreateDocumentWithInlineAlignment(outPdf, cmpPdf, InlineVerticalAlignmentType.BASELINE);
        }

        [NUnit.Framework.Test]
        public virtual void InlineVerticalAlignmentTextTopTest() {
            String outPdf = DESTINATION_FOLDER + "inlineVerticalAlignmentTextTop.pdf";
            String cmpPdf = SOURCE_FOLDER + "cmp_inlineVerticalAlignmentTextTop.pdf";
            CreateDocumentWithInlineAlignment(outPdf, cmpPdf, InlineVerticalAlignmentType.TEXT_TOP);
        }

        [NUnit.Framework.Test]
        public virtual void InlineVerticalAlignmentTextBottomTest() {
            String outPdf = DESTINATION_FOLDER + "inlineVerticalAlignmentTextBottom.pdf";
            String cmpPdf = SOURCE_FOLDER + "cmp_inlineVerticalAlignmentTextBottom.pdf";
            CreateDocumentWithInlineAlignment(outPdf, cmpPdf, InlineVerticalAlignmentType.TEXT_BOTTOM);
        }

        [NUnit.Framework.Test]
        public virtual void InlineVerticalAlignmentFixedTest() {
            String outPdf = DESTINATION_FOLDER + "inlineVerticalAlignmentFixed.pdf";
            String cmpPdf = SOURCE_FOLDER + "cmp_inlineVerticalAlignmentFixed.pdf";
            CreateDocumentWithInlineAlignment(outPdf, cmpPdf, InlineVerticalAlignmentType.FIXED);
        }

        [NUnit.Framework.Test]
        public virtual void InlineVerticalAlignmentFractionTest() {
            String outPdf = DESTINATION_FOLDER + "inlineVerticalAlignmentFraction.pdf";
            String cmpPdf = SOURCE_FOLDER + "cmp_inlineVerticalAlignmentFraction.pdf";
            CreateDocumentWithInlineAlignment(outPdf, cmpPdf, InlineVerticalAlignmentType.FRACTION);
        }

        [NUnit.Framework.Test]
        public virtual void InlineVerticalAlignmentSubTest() {
            String outPdf = DESTINATION_FOLDER + "inlineVerticalAlignmentSub.pdf";
            String cmpPdf = SOURCE_FOLDER + "cmp_inlineVerticalAlignmentSub.pdf";
            CreateDocumentWithInlineAlignment(outPdf, cmpPdf, InlineVerticalAlignmentType.SUB, (d, p, t) => t.SetFontSize
                (20));
        }

        [NUnit.Framework.Test]
        public virtual void InlineVerticalAlignmentSuperTest() {
            String outPdf = DESTINATION_FOLDER + "inlineVerticalAlignmentSuper.pdf";
            String cmpPdf = SOURCE_FOLDER + "cmp_inlineVerticalAlignmentSuper.pdf";
            CreateDocumentWithInlineAlignment(outPdf, cmpPdf, InlineVerticalAlignmentType.SUPER, (d, p, t) => t.SetFontSize
                (20));
        }

        [NUnit.Framework.Test]
        public virtual void InlineVerticalAlignmentmixedTest() {
            // sub and super are resolved in html2Pdf to relative
            String outPdf = DESTINATION_FOLDER + "inlineVerticalAlignmentMixed.pdf";
            String cmpPdf = SOURCE_FOLDER + "cmp_inlineVerticalAlignmentMixed.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outPdf));
            Document doc = new Document(pdfDoc, PageSize.A4.Rotate());
            Paragraph p = new Paragraph();
            p.SetBackgroundColor(new DeviceRgb(189, 239, 73));
            p.SetProperty(Property.RENDERING_MODE, RenderingMode.HTML_MODE);
            p.SetFontSize(20);
            Text heightdefiner = new Text(" Xj ");
            p.Add(heightdefiner);
            AddAlignedElement(p, InlineVerticalAlignmentType.BASELINE);
            AddAlignedElement(p, InlineVerticalAlignmentType.TEXT_TOP);
            AddAlignedElement(p, InlineVerticalAlignmentType.TEXT_BOTTOM);
            AddAlignedElement(p, InlineVerticalAlignmentType.SUB);
            AddAlignedElement(p, InlineVerticalAlignmentType.SUPER);
            AddAlignedElement(p, InlineVerticalAlignmentType.FIXED);
            AddAlignedElement(p, InlineVerticalAlignmentType.FRACTION);
            AddAlignedElement(p, InlineVerticalAlignmentType.MIDDLE);
            AddAlignedElement(p, InlineVerticalAlignmentType.TOP);
            AddAlignedElement(p, InlineVerticalAlignmentType.BOTTOM);
            PdfImageXObject xObject = new PdfImageXObject(ImageDataFactory.CreateJpeg(UrlUtil.ToURL(SOURCE_FOLDER + "itis.jpg"
                )));
            iText.Layout.Element.Image image1 = new iText.Layout.Element.Image(xObject, 50);
            p.Add(image1);
            p.Add(heightdefiner);
            doc.Add(p);
            pdfDoc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, DESTINATION_FOLDER));
        }

        private void AddAlignedElement(Paragraph p, InlineVerticalAlignmentType? verticalAlignment) {
            Text text1 = new Text(" " + verticalAlignment + " ");
            text1.SetProperty(Property.RENDERING_MODE, RenderingMode.HTML_MODE);
            text1.SetFontSize(12);
            if (verticalAlignment == InlineVerticalAlignmentType.FIXED) {
                text1.SetProperty(Property.INLINE_VERTICAL_ALIGNMENT, new InlineVerticalAlignment(verticalAlignment, 20F));
            }
            else {
                if (verticalAlignment == InlineVerticalAlignmentType.FRACTION) {
                    text1.SetProperty(Property.INLINE_VERTICAL_ALIGNMENT, new InlineVerticalAlignment(verticalAlignment, 0.20F
                        ));
                }
                else {
                    text1.SetProperty(Property.INLINE_VERTICAL_ALIGNMENT, new InlineVerticalAlignment(verticalAlignment));
                }
            }
            p.Add(text1);
        }

        [NUnit.Framework.Test]
        public virtual void InlineVerticalAlignmentWithLineHeightSettingTest() {
            String outPdf = DESTINATION_FOLDER + "inlineVerticalAlignmentLineHeight.pdf";
            String cmpPdf = SOURCE_FOLDER + "cmp_inlineVerticalAlignmentLineHeight.pdf";
            CreateDocumentWithInlineAlignment(outPdf, cmpPdf, InlineVerticalAlignmentType.BOTTOM, (d, p, t) => p.SetProperty
                (Property.LINE_HEIGHT, LineHeight.CreateFixedValue(100)));
        }

        [NUnit.Framework.Test]
        public virtual void InlineVerticalAlignmentWithFloatsTest() {
            String outPdf = DESTINATION_FOLDER + "inlineVerticalAlignmentWithFloat.pdf";
            String cmpPdf = SOURCE_FOLDER + "cmp_inlineVerticalAlignmentWithFloat.pdf";
            CreateDocumentWithInlineAlignment(outPdf, cmpPdf, InlineVerticalAlignmentType.BOTTOM, (d, p, t) => {
                PdfImageXObject xObject = new PdfImageXObject(ImageDataFactory.CreateJpeg(UrlUtil.ToURL(SOURCE_FOLDER + "itis.jpg"
                    )));
                iText.Layout.Element.Image image1 = new iText.Layout.Element.Image(xObject, 200);
                image1.SetProperty(Property.FLOAT, FloatPropertyValue.LEFT);
                d.Add(image1);
            }
            );
        }

        [NUnit.Framework.Test]
        public virtual void FloatAlignmentTest03() {
            String outFileName = DESTINATION_FOLDER + "floatAlignmentTest03.pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_floatAlignmentTest03.pdf";
            PdfWriter writer = new PdfWriter(outFileName);
            PdfDocument pdfDoc = new PdfDocument(writer);
            pdfDoc.SetDefaultPageSize(new PageSize(350, 450));
            Document doc = new Document(pdfDoc);
            doc.Add(new Paragraph("All lines after this one have first line indent = 20."));
            doc.SetProperty(Property.FIRST_LINE_INDENT, 20f);
            Text text = new Text("float");
            text.SetBorder(new SolidBorder(1));
            text.SetProperty(Property.FLOAT, FloatPropertyValue.LEFT);
            AddInlineBlockFloatAndText(doc, text);
            doc.Add(new AreaBreak());
            text.SetProperty(Property.FLOAT, FloatPropertyValue.RIGHT);
            AddInlineBlockFloatAndText(doc, text);
            doc.Close();
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void FlexItemHorizontalAlignmentTest() {
            String outFileName = DESTINATION_FOLDER + "flexItemHorizontalAlignmentTest.pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_flexItemHorizontalAlignmentTest.pdf";
            using (PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName))) {
                Document doc = new Document(pdfDoc);
                DocumentRenderer documentRenderer = new DocumentRenderer(doc);
                Div div = new Div();
                FlexContainerRenderer flexContainerRenderer = new FlexContainerRenderer(div);
                flexContainerRenderer.SetParent(documentRenderer);
                div.SetNextRenderer(flexContainerRenderer);
                Div innerDiv = new Div();
                innerDiv.SetBorder(new SolidBorder(1));
                innerDiv.SetProperty(Property.WIDTH, UnitValue.CreatePointValue(50));
                innerDiv.SetProperty(Property.HEIGHT, UnitValue.CreatePointValue(100));
                innerDiv.SetProperty(Property.BACKGROUND, new Background(ColorConstants.GREEN));
                innerDiv.SetHorizontalAlignment(HorizontalAlignment.RIGHT);
                div.Add(innerDiv).Add(innerDiv);
                doc.Add(div);
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void JustifiedAlignmentWithZeroFreeSpaceTest() {
            String outFileName = DESTINATION_FOLDER + "justifiedAlignmentWithZeroFreeSpaceTest.pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_justifiedAlignmentWithZeroFreeSpaceTest.pdf";
            using (PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName))) {
                Document document = new Document(pdfDoc);
                PdfFont font = PdfFontFactory.CreateFont(FONTS_FOLDER + "NotoSansCJKjp-Regular.otf");
                Text t1 = new Text("期期期").SetFont(font);
                Text t2 = new Text("期期期").SetFont(font);
                Paragraph p = new Paragraph(t1).Add(t2).SetSpacingRatio(1).SetWidth(60).SetTextAlignment(TextAlignment.JUSTIFIED
                    );
                document.Add(p);
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff"));
        }

        private static void CreateDocumentWithInlineAlignment(String outPdf, String cmpPdf, InlineVerticalAlignmentType?
             verticalAlignment1) {
            CreateDocumentWithInlineAlignment(outPdf, cmpPdf, verticalAlignment1, null);
        }

        private static void CreateDocumentWithInlineAlignment(String outPdf, String cmpPdf, InlineVerticalAlignmentType?
             verticalAlignment1, AlignmentTest.IInlineTestObjectModifier adjustTestObjects) {
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outPdf));
            Document doc = new Document(pdfDoc);
            Paragraph p = new Paragraph();
            p.SetBackgroundColor(new DeviceRgb(189, 239, 73));
            p.SetProperty(Property.RENDERING_MODE, RenderingMode.HTML_MODE);
            p.SetFontSize(20);
            Text heightdefiner = new Text(" Xj ");
            p.Add(heightdefiner);
            Text text1 = new Text(" vAlign " + verticalAlignment1 + " ");
            if (verticalAlignment1 == InlineVerticalAlignmentType.FIXED) {
                text1.SetProperty(Property.INLINE_VERTICAL_ALIGNMENT, new InlineVerticalAlignment(verticalAlignment1, 20F)
                    );
            }
            else {
                if (verticalAlignment1 == InlineVerticalAlignmentType.FRACTION) {
                    text1.SetProperty(Property.INLINE_VERTICAL_ALIGNMENT, new InlineVerticalAlignment(verticalAlignment1, 0.20F
                        ));
                }
                else {
                    text1.SetProperty(Property.INLINE_VERTICAL_ALIGNMENT, new InlineVerticalAlignment(verticalAlignment1));
                }
            }
            text1.SetProperty(Property.RENDERING_MODE, RenderingMode.HTML_MODE);
            text1.SetFontSize(12);
            if (adjustTestObjects != null) {
                adjustTestObjects(doc, p, text1);
            }
            p.Add(text1);
            p.Add(heightdefiner);
            doc.Add(p);
            pdfDoc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, DESTINATION_FOLDER));
        }

        private static void CreateDocumentWithAlignment(String outPdf, String cmpPdf, VerticalAlignment? verticalAlignment
            ) {
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outPdf));
            Document doc = new Document(pdfDoc);
            Paragraph p = new Paragraph();
            p.SetBackgroundColor(new DeviceRgb(189, 239, 73));
            p.SetVerticalAlignment(verticalAlignment);
            p.Add("vertical Alignment " + verticalAlignment + " one");
            p.Add("\n vertical Alignment " + verticalAlignment + " two");
            p.Add("\n vertical Alignment " + verticalAlignment + " three");
            doc.Add(p);
            pdfDoc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, DESTINATION_FOLDER));
        }

        private void AddInlineBlockFloatAndText(Document doc, Div div) {
            doc.Add(new Paragraph("Left aligned.").SetMarginBottom(30).Add(div).SetTextAlignment(TextAlignment.LEFT));
            doc.Add(new Paragraph("Right aligned.").SetMarginBottom(30).Add(div).SetTextAlignment(TextAlignment.RIGHT)
                );
            doc.Add(new Paragraph("Center aligned.").SetMarginBottom(30).Add(div).SetTextAlignment(TextAlignment.CENTER
                ));
            doc.Add(new Paragraph().Add(div).Add("Justified. " + "The text is laid out using the correct width, but  the alignment value uses the full width."
                ).SetTextAlignment(TextAlignment.JUSTIFIED));
        }

        private void AddInlineBlockFloatAndText(Document doc, Text text) {
            doc.Add(new Paragraph("Left aligned.").SetMarginBottom(30).Add(text).SetTextAlignment(TextAlignment.LEFT));
            doc.Add(new Paragraph("Right aligned.").SetMarginBottom(30).Add(text).SetTextAlignment(TextAlignment.RIGHT
                ));
            doc.Add(new Paragraph("Center aligned.").SetMarginBottom(30).Add(text).SetTextAlignment(TextAlignment.CENTER
                ));
            doc.Add(new Paragraph().Add(text).Add("Justified. " + "The text is laid out using the correct width, but  the alignment value uses the full width."
                ).SetTextAlignment(TextAlignment.JUSTIFIED));
        }

        private delegate void IInlineTestObjectModifier(Document d, Paragraph p, Text t);
    }
}
