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
using iText.Forms;
using iText.Forms.Fields;
using iText.Forms.Form;
using iText.Forms.Logs;
using iText.Kernel.Colors;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using iText.Layout;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Logs;
using iText.Layout.Properties;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Forms.Form.Element {
    [NUnit.Framework.Category("IntegrationTest")]
    public class InputButtonTest : ExtendedITextTest {
        public static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/forms/form/element/InputButtonTest/";

        public static readonly String DESTINATION_FOLDER = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/forms/form/element/InputButtonTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateOrClearDestinationFolder(DESTINATION_FOLDER);
        }

        [NUnit.Framework.Test]
        public virtual void BasicInputButtonTest() {
            String outPdf = DESTINATION_FOLDER + "basicInputButton.pdf";
            String cmpPdf = SOURCE_FOLDER + "cmp_basicInputButton.pdf";
            using (Document document = new Document(new PdfDocument(new PdfWriter(outPdf)))) {
                Button formInputButton = new Button("form input button");
                formInputButton.SetProperty(FormProperty.FORM_FIELD_FLATTEN, false);
                formInputButton.SetSingleLineValue("form input button");
                document.Add(formInputButton);
                document.Add(new Paragraph(""));
                Button flattenInputButton = new Button("flatten input button");
                flattenInputButton.SetProperty(FormProperty.FORM_FIELD_FLATTEN, true);
                flattenInputButton.SetSingleLineValue("flatten input button");
                document.Add(flattenInputButton);
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, DESTINATION_FOLDER));
        }

        [NUnit.Framework.Test]
        public virtual void CustomizedInputButtonTest() {
            String outPdf = DESTINATION_FOLDER + "customizedInputButton.pdf";
            String cmpPdf = SOURCE_FOLDER + "cmp_customizedInputButton.pdf";
            using (Document document = new Document(new PdfDocument(new PdfWriter(outPdf)))) {
                Button formInputButton = new Button("form input button");
                formInputButton.SetProperty(FormProperty.FORM_FIELD_FLATTEN, false);
                formInputButton.SetSingleLineValue("form input button");
                formInputButton.SetFontColor(ColorConstants.BLUE);
                formInputButton.SetBackgroundColor(ColorConstants.YELLOW);
                formInputButton.SetBorder(new SolidBorder(ColorConstants.GREEN, 2));
                document.Add(formInputButton);
                document.Add(new Paragraph(""));
                Button flattenInputButton = new Button("flatten input button");
                flattenInputButton.SetProperty(FormProperty.FORM_FIELD_FLATTEN, true);
                flattenInputButton.SetSingleLineValue("flatten input button");
                flattenInputButton.SetFontColor(ColorConstants.BLUE);
                flattenInputButton.SetBackgroundColor(ColorConstants.YELLOW);
                flattenInputButton.SetBorder(new SolidBorder(ColorConstants.GREEN, 2));
                document.Add(flattenInputButton);
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, DESTINATION_FOLDER));
        }

        [NUnit.Framework.Test]
        public virtual void AddInputButtonInTwoWaysTest() {
            String outPdf = DESTINATION_FOLDER + "addInputButtonInTwoWays.pdf";
            String cmpPdf = SOURCE_FOLDER + "cmp_addInputButtonInTwoWays.pdf";
            using (Document document = new Document(new PdfDocument(new PdfWriter(outPdf)))) {
                // Create push button using html element
                Button formInputButton = new Button("button");
                formInputButton.SetProperty(FormProperty.FORM_FIELD_FLATTEN, false);
                formInputButton.SetSingleLineValue("html input button");
                formInputButton.SetFontColor(ColorConstants.BLUE);
                formInputButton.SetBackgroundColor(ColorConstants.YELLOW);
                formInputButton.SetBorder(new SolidBorder(ColorConstants.GREEN, 2));
                document.Add(formInputButton);
                // Create push button using form field
                PdfAcroForm form = PdfAcroForm.GetAcroForm(document.GetPdfDocument(), true);
                PdfButtonFormField button = new PushButtonFormFieldBuilder(document.GetPdfDocument(), "push").SetWidgetRectangle
                    (new Rectangle(36, 700, 94, 40)).SetCaption("form input button").CreatePushButton();
                button.SetFontSizeAutoScale().SetColor(ColorConstants.RED);
                button.GetFirstFormAnnotation().SetBorderWidth(5).SetBorderColor(ColorConstants.MAGENTA).SetBackgroundColor
                    (ColorConstants.PINK).SetVisibility(PdfFormAnnotation.VISIBLE);
                form.AddField(button);
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, DESTINATION_FOLDER));
        }

        [NUnit.Framework.Test]
        public virtual void InputButtonIsSplitTest() {
            String outPdf = DESTINATION_FOLDER + "inputButtonIsSplit.pdf";
            String cmpPdf = SOURCE_FOLDER + "cmp_inputButtonIsSplit.pdf";
            using (Document document = new Document(new PdfDocument(new PdfWriter(outPdf)))) {
                Button formInputButton = new Button("button");
                formInputButton.SetProperty(FormProperty.FORM_FIELD_FLATTEN, false);
                formInputButton.SetProperty(Property.WIDTH, UnitValue.CreatePointValue(280));
                formInputButton.SetProperty(Property.HEIGHT, UnitValue.CreatePointValue(30));
                formInputButton.SetSingleLineValue("text with default font size longer than button width won't be split");
                document.Add(formInputButton);
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, DESTINATION_FOLDER));
        }

        [NUnit.Framework.Test]
        [LogMessage(LayoutLogMessageConstant.ELEMENT_DOES_NOT_FIT_AREA, Count = 2)]
        [LogMessage(FormsLogMessageConstants.INPUT_FIELD_DOES_NOT_FIT, Count = 2)]
        public virtual void InputButtonIsForcedSplitTest() {
            String outPdf = DESTINATION_FOLDER + "inputButtonIsForcedSplit.pdf";
            String cmpPdf = SOURCE_FOLDER + "cmp_inputButtonIsForcedSplit.pdf";
            using (Document document = new Document(new PdfDocument(new PdfWriter(outPdf)))) {
                Button formInputButton = new Button("button");
                formInputButton.SetProperty(FormProperty.FORM_FIELD_FLATTEN, false);
                formInputButton.SetProperty(Property.WIDTH, UnitValue.CreatePointValue(280));
                formInputButton.SetProperty(Property.HEIGHT, UnitValue.CreatePointValue(30));
                formInputButton.SetSingleLineValue("text with line break\n which will be split");
                document.Add(formInputButton);
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, DESTINATION_FOLDER));
        }

        [NUnit.Framework.Test]
        public virtual void InputButtonWithPaddingsTest() {
            String outPdf = DESTINATION_FOLDER + "inputButtonWithPaddings.pdf";
            String cmpPdf = SOURCE_FOLDER + "cmp_inputButtonWithPaddings.pdf";
            using (Document document = new Document(new PdfDocument(new PdfWriter(outPdf)))) {
                Button formInputButton = new Button("button");
                formInputButton.SetProperty(FormProperty.FORM_FIELD_FLATTEN, false);
                formInputButton.SetProperty(Property.PADDING_BOTTOM, UnitValue.CreatePointValue(15));
                formInputButton.SetProperty(Property.PADDING_TOP, UnitValue.CreatePointValue(15));
                formInputButton.SetFontSize(50);
                formInputButton.SetSingleLineValue("Caption");
                document.Add(formInputButton);
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, DESTINATION_FOLDER));
        }

        [NUnit.Framework.Test]
        public virtual void InputButtonWithMarginsPaddingsTest() {
            String outPdf = DESTINATION_FOLDER + "inputButtonWithMarginsPaddings.pdf";
            String cmpPdf = SOURCE_FOLDER + "cmp_inputButtonWithMarginsPaddings.pdf";
            using (Document document = new Document(new PdfDocument(new PdfWriter(outPdf)))) {
                Div div = new Div().SetBackgroundColor(ColorConstants.PINK);
                Button formInputButton = new Button("button");
                formInputButton.SetProperty(FormProperty.FORM_FIELD_FLATTEN, false);
                formInputButton.SetProperty(Property.PADDING_BOTTOM, UnitValue.CreatePointValue(20));
                formInputButton.SetProperty(Property.PADDING_TOP, UnitValue.CreatePointValue(20));
                formInputButton.SetProperty(Property.PADDING_LEFT, UnitValue.CreatePointValue(20));
                formInputButton.SetProperty(Property.PADDING_RIGHT, UnitValue.CreatePointValue(20));
                formInputButton.SetProperty(Property.MARGIN_BOTTOM, UnitValue.CreatePointValue(20));
                formInputButton.SetProperty(Property.MARGIN_TOP, UnitValue.CreatePointValue(20));
                formInputButton.SetProperty(Property.MARGIN_LEFT, UnitValue.CreatePointValue(20));
                formInputButton.SetProperty(Property.MARGIN_RIGHT, UnitValue.CreatePointValue(20));
                formInputButton.SetBorder(new SolidBorder(ColorConstants.DARK_GRAY, 20));
                formInputButton.SetFontSize(20);
                formInputButton.SetSingleLineValue("Caption");
                div.Add(formInputButton);
                document.Add(div);
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, DESTINATION_FOLDER));
        }
    }
}
