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
using iText.Forms.Form;
using iText.Kernel.Colors;
using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using iText.Layout;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Forms.Form.Element {
    [NUnit.Framework.Category("IntegrationTest")]
    public class TextAreaTest : ExtendedITextTest {
        public static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/forms/form/element/TextAreaTest/";

        public static readonly String DESTINATION_FOLDER = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/forms/form/element/TextAreaTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateOrClearDestinationFolder(DESTINATION_FOLDER);
        }

        [NUnit.Framework.Test]
        public virtual void BasicTextAreaTest() {
            String outPdf = DESTINATION_FOLDER + "basicTextArea.pdf";
            String cmpPdf = SOURCE_FOLDER + "cmp_basicTextArea.pdf";
            using (Document document = new Document(new PdfDocument(new PdfWriter(outPdf)))) {
                TextArea formTextArea = new TextArea("form text area");
                formTextArea.SetProperty(FormProperty.FORM_FIELD_FLATTEN, false);
                formTextArea.SetProperty(FormProperty.FORM_FIELD_VALUE, "form\ntext\narea");
                document.Add(formTextArea);
                TextArea flattenTextArea = new TextArea("flatten text area");
                flattenTextArea.SetProperty(FormProperty.FORM_FIELD_FLATTEN, true);
                flattenTextArea.SetProperty(FormProperty.FORM_FIELD_VALUE, "flatten\ntext\narea");
                document.Add(flattenTextArea);
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, DESTINATION_FOLDER));
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.PROPERTY_IN_PERCENTS_NOT_SUPPORTED, Count = 16)]
        public virtual void PercentFontTextAreaTest() {
            String outPdf = DESTINATION_FOLDER + "percentFontTextArea.pdf";
            String cmpPdf = SOURCE_FOLDER + "cmp_percentFontTextArea.pdf";
            using (Document document = new Document(new PdfDocument(new PdfWriter(outPdf)))) {
                TextArea formTextArea = new TextArea("form text area");
                formTextArea.SetProperty(FormProperty.FORM_FIELD_FLATTEN, false);
                formTextArea.SetProperty(FormProperty.FORM_FIELD_VALUE, "form\ntext\narea");
                formTextArea.SetProperty(Property.FONT_SIZE, UnitValue.CreatePercentValue(10));
                document.Add(formTextArea);
                TextArea flattenTextArea = new TextArea("flatten text area");
                flattenTextArea.SetProperty(FormProperty.FORM_FIELD_FLATTEN, true);
                flattenTextArea.SetProperty(FormProperty.FORM_FIELD_VALUE, "flatten\ntext\narea");
                formTextArea.SetProperty(Property.FONT_SIZE, UnitValue.CreatePercentValue(10));
                document.Add(flattenTextArea);
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, DESTINATION_FOLDER));
        }

        [NUnit.Framework.Test]
        public virtual void HeightTextAreaTest() {
            String outPdf = DESTINATION_FOLDER + "heightTextArea.pdf";
            String cmpPdf = SOURCE_FOLDER + "cmp_heightTextArea.pdf";
            using (Document document = new Document(new PdfDocument(new PdfWriter(outPdf)))) {
                TextArea flattenTextArea = new TextArea("flatten text area with height");
                flattenTextArea.SetProperty(FormProperty.FORM_FIELD_FLATTEN, true);
                flattenTextArea.SetProperty(FormProperty.FORM_FIELD_VALUE, "flatten\ntext area\nwith height");
                flattenTextArea.SetProperty(Property.HEIGHT, new UnitValue(UnitValue.POINT, 100));
                flattenTextArea.SetProperty(Property.BORDER, new SolidBorder(2f));
                document.Add(flattenTextArea);
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, DESTINATION_FOLDER));
        }

        [NUnit.Framework.Test]
        public virtual void MinHeightTextAreaTest() {
            String outPdf = DESTINATION_FOLDER + "minHeightTextArea.pdf";
            String cmpPdf = SOURCE_FOLDER + "cmp_minHeightTextArea.pdf";
            using (Document document = new Document(new PdfDocument(new PdfWriter(outPdf)))) {
                TextArea flattenTextArea = new TextArea("flatten text area with height");
                flattenTextArea.SetProperty(FormProperty.FORM_FIELD_FLATTEN, true);
                flattenTextArea.SetProperty(FormProperty.FORM_FIELD_VALUE, "flatten\ntext area\nwith height");
                flattenTextArea.SetProperty(Property.MIN_HEIGHT, new UnitValue(UnitValue.POINT, 100));
                flattenTextArea.SetProperty(Property.BORDER, new SolidBorder(2f));
                document.Add(flattenTextArea);
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, DESTINATION_FOLDER));
        }

        [NUnit.Framework.Test]
        public virtual void HugeMarginPaddingBorderTest() {
            String outPdf = DESTINATION_FOLDER + "hugeMarginPaddingBorder.pdf";
            String cmpPdf = SOURCE_FOLDER + "cmp_hugeMarginPaddingBorder.pdf";
            using (Document document = new Document(new PdfDocument(new PdfWriter(outPdf)))) {
                TextArea formTextArea = new TextArea("interactive text area with paddings");
                formTextArea.SetInteractive(true);
                formTextArea.SetValue("interactive\ntext area\nwith paddings");
                formTextArea.SetBorder(new SolidBorder(20));
                formTextArea.SetProperty(Property.PADDING_BOTTOM, UnitValue.CreatePointValue(20));
                formTextArea.SetProperty(Property.PADDING_TOP, UnitValue.CreatePointValue(20));
                formTextArea.SetProperty(Property.PADDING_RIGHT, UnitValue.CreatePointValue(20));
                formTextArea.SetProperty(Property.PADDING_LEFT, UnitValue.CreatePointValue(20));
                formTextArea.SetProperty(Property.MARGIN_BOTTOM, UnitValue.CreatePointValue(20));
                formTextArea.SetProperty(Property.MARGIN_TOP, UnitValue.CreatePointValue(20));
                formTextArea.SetProperty(Property.MARGIN_RIGHT, UnitValue.CreatePointValue(20));
                formTextArea.SetProperty(Property.MARGIN_LEFT, UnitValue.CreatePointValue(20));
                document.Add(formTextArea);
                TextArea flattenTextArea = new TextArea("flatten text area with paddings");
                flattenTextArea.SetInteractive(false);
                flattenTextArea.SetValue("flatten\ntext area\nwith paddings");
                flattenTextArea.SetBorder(new SolidBorder(20));
                flattenTextArea.SetProperty(Property.PADDING_BOTTOM, UnitValue.CreatePointValue(20));
                flattenTextArea.SetProperty(Property.PADDING_TOP, UnitValue.CreatePointValue(20));
                flattenTextArea.SetProperty(Property.PADDING_RIGHT, UnitValue.CreatePointValue(20));
                flattenTextArea.SetProperty(Property.PADDING_LEFT, UnitValue.CreatePointValue(20));
                flattenTextArea.SetProperty(Property.MARGIN_BOTTOM, UnitValue.CreatePointValue(20));
                flattenTextArea.SetProperty(Property.MARGIN_TOP, UnitValue.CreatePointValue(20));
                flattenTextArea.SetProperty(Property.MARGIN_RIGHT, UnitValue.CreatePointValue(20));
                flattenTextArea.SetProperty(Property.MARGIN_LEFT, UnitValue.CreatePointValue(20));
                document.Add(flattenTextArea);
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, DESTINATION_FOLDER));
        }

        [NUnit.Framework.Test]
        public virtual void TextAreaDoesNotFitTest() {
            String outPdf = DESTINATION_FOLDER + "textAreaDoesNotFit.pdf";
            String cmpPdf = SOURCE_FOLDER + "cmp_textAreaDoesNotFit.pdf";
            using (Document document = new Document(new PdfDocument(new PdfWriter(outPdf)))) {
                Div div = new Div();
                div.SetWidth(UnitValue.CreatePointValue(400));
                div.SetHeight(UnitValue.CreatePointValue(730));
                div.SetBackgroundColor(ColorConstants.PINK);
                document.Add(div);
                TextArea textArea = new TextArea("text area");
                textArea.SetInteractive(true);
                textArea.SetProperty(FormProperty.FORM_FIELD_VALUE, "some text to not\nbe able to fit in on the page\nmore text just text\nreally big height"
                    );
                textArea.SetHeight(50);
                textArea.SetProperty(Property.BORDER, new SolidBorder(2f));
                document.Add(textArea);
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, DESTINATION_FOLDER));
        }

        [NUnit.Framework.Test]
        public virtual void TextAreaWith0FontSizeDoesNotFitTest() {
            String outPdf = DESTINATION_FOLDER + "textAreaWith0FontSizeDoesNotFit.pdf";
            String cmpPdf = SOURCE_FOLDER + "cmp_textAreaWith0FontSizeDoesNotFit.pdf";
            using (Document document = new Document(new PdfDocument(new PdfWriter(outPdf)))) {
                document.Add(new Div().SetBackgroundColor(ColorConstants.RED).SetHeight(695));
                TextArea textArea = new TextArea("text area");
                textArea.SetInteractive(true);
                textArea.SetProperty(FormProperty.FORM_FIELD_VALUE, "Font\n size \nof this\nText Area will \nbe approximated\nbased on the content"
                    );
                textArea.SetProperty(Property.BORDER, new SolidBorder(1f));
                textArea.SetFontSize(0);
                textArea.SetHeight(75);
                document.Add(textArea);
                document.Add(new Div().SetBackgroundColor(ColorConstants.RED).SetHeight(695));
                TextArea flattenTextArea = new TextArea("text area");
                flattenTextArea.SetInteractive(false);
                flattenTextArea.SetProperty(FormProperty.FORM_FIELD_VALUE, "Font\n size \nof this\nText Area will \nbe approximated\nbased on the content"
                    );
                flattenTextArea.SetProperty(Property.BORDER, new SolidBorder(1f));
                flattenTextArea.SetFontSize(0);
                flattenTextArea.SetHeight(75);
                document.Add(flattenTextArea);
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, DESTINATION_FOLDER));
        }

        [NUnit.Framework.Test]
        public virtual void TextAreaWith0FontSizeFitsTest() {
            String outPdf = DESTINATION_FOLDER + "textAreaWith0FontSizeFits.pdf";
            String cmpPdf = SOURCE_FOLDER + "cmp_textAreaWith0FontSizeFits.pdf";
            using (Document document = new Document(new PdfDocument(new PdfWriter(outPdf)))) {
                TextArea textArea = new TextArea("text area");
                textArea.SetInteractive(true);
                textArea.SetProperty(FormProperty.FORM_FIELD_VALUE, "Font\n size \nof this\nText Area will \nbe approximated\nbased on the content"
                    );
                textArea.SetProperty(Property.BORDER, new SolidBorder(1f));
                textArea.SetFontSize(0);
                textArea.SetHeight(75);
                document.Add(textArea);
                TextArea flattenTextArea = new TextArea("text area");
                flattenTextArea.SetInteractive(false);
                flattenTextArea.SetProperty(FormProperty.FORM_FIELD_VALUE, "Font\n size \nof this\nText Area will \nbe approximated\nbased on the content"
                    );
                flattenTextArea.SetProperty(Property.BORDER, new SolidBorder(1f));
                flattenTextArea.SetFontSize(0);
                flattenTextArea.SetHeight(75);
                document.Add(flattenTextArea);
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, DESTINATION_FOLDER));
        }

        [NUnit.Framework.Test]
        public virtual void TextAreaWith0FontSizeWithoutHeightTest() {
            String outPdf = DESTINATION_FOLDER + "textAreaWith0FontSizeWithoutHeight.pdf";
            String cmpPdf = SOURCE_FOLDER + "cmp_textAreaWith0FontSizeWithoutHeight.pdf";
            using (Document document = new Document(new PdfDocument(new PdfWriter(outPdf)))) {
                TextArea textArea = new TextArea("text area");
                textArea.SetInteractive(true);
                textArea.SetProperty(FormProperty.FORM_FIELD_VALUE, "Font\n size \nof this\nText Area will not " + "\nbe approximated\nbased on the content\nbecause height is not set"
                    );
                textArea.SetProperty(Property.BORDER, new SolidBorder(1f));
                textArea.SetFontSize(0);
                document.Add(textArea);
                TextArea flattenTextArea = new TextArea("text area");
                flattenTextArea.SetInteractive(false);
                flattenTextArea.SetProperty(FormProperty.FORM_FIELD_VALUE, "Font\n size \nof this\nText Area will not " + 
                    "\nbe approximated\nbased on the content\nbecause height is not set");
                flattenTextArea.SetProperty(Property.BORDER, new SolidBorder(1f));
                flattenTextArea.SetFontSize(0);
                document.Add(flattenTextArea);
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, DESTINATION_FOLDER));
        }

        [NUnit.Framework.Test]
        public virtual void TextAreaWithBorderLessThan1Test() {
            String outPdf = DESTINATION_FOLDER + "textAreaWithBorderLessThan1.pdf";
            String cmpPdf = SOURCE_FOLDER + "cmp_textAreaWithBorderLessThan1.pdf";
            using (Document document = new Document(new PdfDocument(new PdfWriter(outPdf)))) {
                TextArea textArea = new TextArea("text area");
                textArea.SetInteractive(true);
                textArea.SetProperty(FormProperty.FORM_FIELD_VALUE, "Is border visible?\nAnd after clicking on the field?\nIt should be by the way"
                    );
                textArea.SetProperty(Property.BORDER, new SolidBorder(0.5f));
                document.Add(textArea);
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, DESTINATION_FOLDER));
        }

        [NUnit.Framework.Test]
        public virtual void TextAreaWithJustificationTest() {
            String outPdf = DESTINATION_FOLDER + "textAreaWithJustification.pdf";
            String cmpPdf = SOURCE_FOLDER + "cmp_textAreaWithJustification.pdf";
            using (Document document = new Document(new PdfDocument(new PdfWriter(outPdf)))) {
                TextArea textArea = new TextArea("text area");
                textArea.SetValue("text area with justification\nWords shall be in the center\nAre they?");
                textArea.SetInteractive(true);
                textArea.SetTextAlignment(TextAlignment.CENTER);
                document.Add(textArea);
                TextArea flattenedTextArea = new TextArea("flattened text area");
                flattenedTextArea.SetValue("text area with justification\nWords shall be in the center\nAre they?");
                flattenedTextArea.SetInteractive(false);
                flattenedTextArea.SetTextAlignment(TextAlignment.CENTER);
                document.Add(flattenedTextArea);
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, DESTINATION_FOLDER));
        }

        [NUnit.Framework.Test]
        public virtual void TextAreaWithCustomBorderTest() {
            String outPdf = DESTINATION_FOLDER + "textAreaWithCustomBorder.pdf";
            String cmpPdf = SOURCE_FOLDER + "cmp_textAreaWithCustomBorder.pdf";
            using (Document document = new Document(new PdfDocument(new PdfWriter(outPdf)))) {
                TextArea textArea = new TextArea("text area");
                textArea.SetValue("text area with custom border\nBorder shall be orange, 10 points wide and dashed");
                textArea.SetInteractive(true);
                textArea.SetBorder(new DashedBorder(ColorConstants.ORANGE, 10));
                document.Add(textArea);
                TextArea flattenedTextArea = new TextArea("flattened text area");
                flattenedTextArea.SetValue("text area with custom border\nBorder shall be orange, 10 points wide and dashed"
                    );
                flattenedTextArea.SetInteractive(false);
                flattenedTextArea.SetBorder(new DashedBorder(ColorConstants.ORANGE, 10));
                document.Add(flattenedTextArea);
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, DESTINATION_FOLDER));
        }

        [NUnit.Framework.Test]
        public virtual void MaxHeightTextAreaTest() {
            String outPdf = DESTINATION_FOLDER + "maxHeightTextArea.pdf";
            String cmpPdf = SOURCE_FOLDER + "cmp_maxHeightTextArea.pdf";
            using (Document document = new Document(new PdfDocument(new PdfWriter(outPdf)))) {
                TextArea flattenTextArea = new TextArea("flatten text area with height");
                flattenTextArea.SetProperty(FormProperty.FORM_FIELD_FLATTEN, true);
                flattenTextArea.SetProperty(FormProperty.FORM_FIELD_VALUE, "flatten\ntext area\nwith height");
                flattenTextArea.SetProperty(Property.MAX_HEIGHT, new UnitValue(UnitValue.POINT, 28));
                flattenTextArea.SetProperty(Property.BORDER, new SolidBorder(2f));
                document.Add(flattenTextArea);
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, DESTINATION_FOLDER));
        }

        [NUnit.Framework.Test]
        public virtual void TextAreaWithCustomLeadingTest() {
            String outPdf = DESTINATION_FOLDER + "textAreaWithCustomLeading.pdf";
            String cmpPdf = SOURCE_FOLDER + "cmp_textAreaWithCustomLeading.pdf";
            using (Document document = new Document(new PdfDocument(new PdfWriter(outPdf)))) {
                TextArea textArea = new TextArea("text1").SetBorder(new SolidBorder(ColorConstants.PINK, 1));
                textArea.SetValue("text area with 1 used as the basis for the leading calculation");
                textArea.SetInteractive(true);
                textArea.SetProperty(Property.LEADING, new Leading(Leading.MULTIPLIED, 1));
                textArea.SetProperty(Property.MARGIN_BOTTOM, UnitValue.CreatePointValue(5));
                document.Add(textArea);
                TextArea textArea2 = new TextArea("text2").SetBorder(new SolidBorder(ColorConstants.YELLOW, 1));
                textArea2.SetValue("text area with 3 used as the basis for the leading calculation");
                textArea2.SetInteractive(true);
                textArea2.SetProperty(Property.LEADING, new Leading(Leading.MULTIPLIED, 3));
                textArea2.SetProperty(Property.MARGIN_BOTTOM, UnitValue.CreatePointValue(5));
                document.Add(textArea2);
                TextArea flattenedTextArea = new TextArea("text3").SetBorder(new SolidBorder(ColorConstants.PINK, 1));
                flattenedTextArea.SetValue("text area with 5 used as the basis for the leading calculation");
                flattenedTextArea.SetInteractive(false);
                flattenedTextArea.SetProperty(Property.LEADING, new Leading(Leading.MULTIPLIED, 5));
                flattenedTextArea.SetProperty(Property.MARGIN_BOTTOM, UnitValue.CreatePointValue(5));
                document.Add(flattenedTextArea);
                TextArea flattenedTextArea2 = new TextArea("text4").SetBorder(new SolidBorder(ColorConstants.YELLOW, 1));
                flattenedTextArea2.SetValue("text area with 0.5 used as the basis for the leading calculation");
                flattenedTextArea2.SetInteractive(false);
                flattenedTextArea2.SetProperty(Property.LEADING, new Leading(Leading.MULTIPLIED, 0.5f));
                document.Add(flattenedTextArea2);
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, DESTINATION_FOLDER));
        }
    }
}
