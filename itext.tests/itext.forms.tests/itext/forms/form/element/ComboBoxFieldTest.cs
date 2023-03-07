/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
Authors: iText Software.

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
using iText.Forms.Logs;
using iText.Kernel.Colors;
using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Forms.Form.Element {
    [NUnit.Framework.Category("IntegrationTest")]
    public class ComboBoxFieldTest : ExtendedITextTest {
        public static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/forms/form/element/ComboBoxFieldTest/";

        public static readonly String DESTINATION_FOLDER = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/forms/form/element/ComboBoxFieldTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateOrClearDestinationFolder(DESTINATION_FOLDER);
        }

        [NUnit.Framework.Test]
        public virtual void EmptyComboBoxFieldTest() {
            String outPdf = DESTINATION_FOLDER + "emptyComboBoxField.pdf";
            String cmpPdf = SOURCE_FOLDER + "cmp_emptyComboBoxField.pdf";
            using (Document document = new Document(new PdfDocument(new PdfWriter(outPdf)))) {
                ComboBoxField flattenComboBoxField = new ComboBoxField("flatten empty combo box field");
                flattenComboBoxField.SetProperty(FormProperty.FORM_FIELD_FLATTEN, true);
                flattenComboBoxField.SetBackgroundColor(ColorConstants.RED);
                document.Add(flattenComboBoxField);
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, DESTINATION_FOLDER));
        }

        [NUnit.Framework.Test]
        [LogMessage(FormsLogMessageConstants.ACROFORM_NOT_SUPPORTED_FOR_SELECT)]
        public virtual void BasicComboBoxFieldTest() {
            String outPdf = DESTINATION_FOLDER + "basicComboBoxField.pdf";
            String cmpPdf = SOURCE_FOLDER + "cmp_basicComboBoxField.pdf";
            using (Document document = new Document(new PdfDocument(new PdfWriter(outPdf)))) {
                ComboBoxField formComboBoxField = new ComboBoxField("form combo box field");
                formComboBoxField.SetProperty(FormProperty.FORM_FIELD_FLATTEN, false);
                Paragraph option1 = new Paragraph("option 1");
                option1.SetProperty(FormProperty.FORM_FIELD_SELECTED, true);
                option1.SetProperty(FormProperty.FORM_FIELD_LABEL, "option 1");
                formComboBoxField.AddOption(option1);
                Paragraph option2 = new Paragraph("option 2");
                option2.SetProperty(FormProperty.FORM_FIELD_LABEL, "option 2");
                formComboBoxField.AddOption(option2);
                document.Add(formComboBoxField);
                ComboBoxField flattenComboBoxField = new ComboBoxField("flatten combo box field");
                flattenComboBoxField.SetProperty(FormProperty.FORM_FIELD_FLATTEN, true);
                flattenComboBoxField.AddOption(option1);
                flattenComboBoxField.AddOption(option2);
                document.Add(flattenComboBoxField);
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, DESTINATION_FOLDER));
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.PROPERTY_IN_PERCENTS_NOT_SUPPORTED, Count = 11)]
        public virtual void ComboBoxFieldWithoutSelectionTest() {
            String outPdf = DESTINATION_FOLDER + "comboBoxFieldWithoutSelection.pdf";
            String cmpPdf = SOURCE_FOLDER + "cmp_comboBoxFieldWithoutSelection.pdf";
            using (Document document = new Document(new PdfDocument(new PdfWriter(outPdf)))) {
                Paragraph option1 = new Paragraph("option 1");
                option1.SetProperty(FormProperty.FORM_FIELD_LABEL, "option 1");
                Paragraph option2 = new Paragraph("option 2");
                option2.SetProperty(FormProperty.FORM_FIELD_LABEL, "option 2");
                ComboBoxField flattenComboBoxFieldWithFont = new ComboBoxField("flatten combo box field with font");
                flattenComboBoxFieldWithFont.SetProperty(FormProperty.FORM_FIELD_FLATTEN, true);
                flattenComboBoxFieldWithFont.SetBackgroundColor(ColorConstants.RED);
                flattenComboBoxFieldWithFont.AddOption(option1);
                flattenComboBoxFieldWithFont.AddOption(option2);
                document.Add(flattenComboBoxFieldWithFont);
                ComboBoxField flattenComboBoxFieldWithPercentFont = new ComboBoxField("flatten combo box field with percent font"
                    );
                flattenComboBoxFieldWithPercentFont.SetProperty(FormProperty.FORM_FIELD_FLATTEN, true);
                flattenComboBoxFieldWithPercentFont.SetBackgroundColor(ColorConstants.RED);
                flattenComboBoxFieldWithPercentFont.AddOption(option1);
                flattenComboBoxFieldWithPercentFont.AddOption(option2);
                flattenComboBoxFieldWithPercentFont.SetProperty(Property.FONT_SIZE, UnitValue.CreatePercentValue(10));
                document.Add(flattenComboBoxFieldWithPercentFont);
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, DESTINATION_FOLDER));
        }

        [NUnit.Framework.Test]
        public virtual void ComboBoxFieldWithHeightTest() {
            String outPdf = DESTINATION_FOLDER + "comboBoxFieldWithHeight.pdf";
            String cmpPdf = SOURCE_FOLDER + "cmp_comboBoxFieldWithHeight.pdf";
            using (Document document = new Document(new PdfDocument(new PdfWriter(outPdf)))) {
                Paragraph option1 = new Paragraph("option 1");
                option1.SetProperty(FormProperty.FORM_FIELD_LABEL, "option 1");
                Paragraph option2 = new Paragraph("option 2");
                option2.SetProperty(FormProperty.FORM_FIELD_SELECTED, true);
                option2.SetProperty(FormProperty.FORM_FIELD_LABEL, "option 2");
                ComboBoxField flattenComboBoxField = new ComboBoxField("flatten combo box field with height");
                flattenComboBoxField.SetProperty(FormProperty.FORM_FIELD_FLATTEN, true);
                flattenComboBoxField.SetBackgroundColor(ColorConstants.RED);
                flattenComboBoxField.AddOption(option1);
                flattenComboBoxField.AddOption(option2);
                flattenComboBoxField.SetProperty(Property.HEIGHT, UnitValue.CreatePointValue(100));
                document.Add(flattenComboBoxField);
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, DESTINATION_FOLDER));
        }

        [NUnit.Framework.Test]
        public virtual void ComboBoxFieldWithMinHeightTest() {
            String outPdf = DESTINATION_FOLDER + "comboBoxFieldWithMinHeight.pdf";
            String cmpPdf = SOURCE_FOLDER + "cmp_comboBoxFieldWithMinHeight.pdf";
            using (Document document = new Document(new PdfDocument(new PdfWriter(outPdf)))) {
                Paragraph option1 = new Paragraph("option 1");
                option1.SetProperty(FormProperty.FORM_FIELD_LABEL, "option 1");
                Paragraph option2 = new Paragraph("option 2");
                option2.SetProperty(FormProperty.FORM_FIELD_SELECTED, true);
                option2.SetProperty(FormProperty.FORM_FIELD_LABEL, "option 2");
                ComboBoxField flattenComboBoxField = new ComboBoxField("flatten combo box field with min height");
                flattenComboBoxField.SetProperty(FormProperty.FORM_FIELD_FLATTEN, true);
                flattenComboBoxField.SetBackgroundColor(ColorConstants.RED);
                flattenComboBoxField.AddOption(option1);
                flattenComboBoxField.AddOption(option2);
                flattenComboBoxField.SetProperty(Property.MIN_HEIGHT, UnitValue.CreatePointValue(100));
                document.Add(flattenComboBoxField);
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, DESTINATION_FOLDER));
        }

        [NUnit.Framework.Test]
        public virtual void ComboBoxFieldWithMaxHeightTest() {
            String outPdf = DESTINATION_FOLDER + "comboBoxFieldWithMaxHeight.pdf";
            String cmpPdf = SOURCE_FOLDER + "cmp_comboBoxFieldWithMaxHeight.pdf";
            using (Document document = new Document(new PdfDocument(new PdfWriter(outPdf)))) {
                Paragraph option1 = new Paragraph("option 1");
                option1.SetProperty(FormProperty.FORM_FIELD_SELECTED, true);
                option1.SetProperty(FormProperty.FORM_FIELD_LABEL, "option 1");
                Paragraph option2 = new Paragraph("option 2");
                option2.SetProperty(FormProperty.FORM_FIELD_LABEL, "option 2");
                ComboBoxField flattenComboBoxField = new ComboBoxField("flatten combo box field with max height");
                flattenComboBoxField.SetProperty(FormProperty.FORM_FIELD_FLATTEN, true);
                flattenComboBoxField.SetBackgroundColor(ColorConstants.RED);
                flattenComboBoxField.AddOption(option1);
                flattenComboBoxField.AddOption(option2);
                flattenComboBoxField.SetProperty(Property.MAX_HEIGHT, UnitValue.CreatePointValue(10));
                document.Add(flattenComboBoxField);
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, DESTINATION_FOLDER));
        }

        [NUnit.Framework.Test]
        public virtual void ComboBoxFieldCannotFitTest() {
            String outPdf = DESTINATION_FOLDER + "comboBoxFieldCannotFit.pdf";
            String cmpPdf = SOURCE_FOLDER + "cmp_comboBoxFieldCannotFit.pdf";
            using (Document document = new Document(new PdfDocument(new PdfWriter(outPdf)))) {
                Div div = new Div();
                div.SetWidth(UnitValue.CreatePointValue(400));
                div.SetHeight(UnitValue.CreatePointValue(755));
                div.SetBackgroundColor(ColorConstants.PINK);
                document.Add(div);
                Paragraph option1 = new Paragraph("option 1");
                option1.SetProperty(FormProperty.FORM_FIELD_SELECTED, true);
                option1.SetProperty(FormProperty.FORM_FIELD_LABEL, "option 1");
                Paragraph option2 = new Paragraph("option 2");
                option2.SetProperty(FormProperty.FORM_FIELD_LABEL, "option 2");
                ComboBoxField flattenComboBoxField = new ComboBoxField("flatten combo box cannot fit");
                flattenComboBoxField.SetProperty(FormProperty.FORM_FIELD_FLATTEN, true);
                flattenComboBoxField.SetBackgroundColor(ColorConstants.RED);
                flattenComboBoxField.AddOption(option1);
                flattenComboBoxField.AddOption(option2);
                document.Add(flattenComboBoxField);
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, DESTINATION_FOLDER));
        }

        [NUnit.Framework.Test]
        public virtual void ComboBoxFieldWithLangTest() {
            String outPdf = DESTINATION_FOLDER + "comboBoxFieldWithLang.pdf";
            String cmpPdf = SOURCE_FOLDER + "cmp_comboBoxFieldWithLang.pdf";
            using (Document document = new Document(new PdfDocument(new PdfWriter(outPdf)))) {
                Paragraph option1 = new Paragraph("option 1");
                option1.SetProperty(FormProperty.FORM_FIELD_SELECTED, true);
                option1.SetProperty(FormProperty.FORM_FIELD_LABEL, "option 1");
                Paragraph option2 = new Paragraph("option 2");
                option2.SetProperty(FormProperty.FORM_FIELD_LABEL, "option 2");
                ComboBoxField flattenComboBoxField = new ComboBoxField("flatten combo box with lang");
                flattenComboBoxField.SetProperty(FormProperty.FORM_FIELD_FLATTEN, true);
                flattenComboBoxField.SetBackgroundColor(ColorConstants.RED);
                flattenComboBoxField.AddOption(option1);
                flattenComboBoxField.AddOption(option2);
                flattenComboBoxField.SetProperty(FormProperty.FORM_ACCESSIBILITY_LANGUAGE, "random_lang");
                document.Add(flattenComboBoxField);
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, DESTINATION_FOLDER));
        }
    }
}
