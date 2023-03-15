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
using iText.Forms.Logs;
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
    public class ListBoxFieldTest : ExtendedITextTest {
        public static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/forms/form/element/ListBoxFieldTest/";

        public static readonly String DESTINATION_FOLDER = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/forms/form/element/ListBoxFieldTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateOrClearDestinationFolder(DESTINATION_FOLDER);
        }

        [NUnit.Framework.Test]
        public virtual void EmptyListBoxFieldTest() {
            String outPdf = DESTINATION_FOLDER + "emptyListBoxField.pdf";
            String cmpPdf = SOURCE_FOLDER + "cmp_emptyListBoxField.pdf";
            using (Document document = new Document(new PdfDocument(new PdfWriter(outPdf)))) {
                ListBoxField flattenListBoxField = new ListBoxField("flatten empty list box field", 0, false);
                flattenListBoxField.SetProperty(FormProperty.FORM_FIELD_FLATTEN, true);
                flattenListBoxField.SetBackgroundColor(ColorConstants.RED);
                document.Add(flattenListBoxField);
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, DESTINATION_FOLDER));
        }

        [NUnit.Framework.Test]
        [LogMessage(FormsLogMessageConstants.ACROFORM_NOT_SUPPORTED_FOR_SELECT)]
        public virtual void BasicListBoxFieldTest() {
            String outPdf = DESTINATION_FOLDER + "basicListBoxField.pdf";
            String cmpPdf = SOURCE_FOLDER + "cmp_basicListBoxField.pdf";
            using (Document document = new Document(new PdfDocument(new PdfWriter(outPdf)))) {
                ListBoxField formListBoxField = new ListBoxField("form list box field", 2, false);
                formListBoxField.SetProperty(FormProperty.FORM_FIELD_FLATTEN, false);
                Paragraph option1 = new Paragraph("option 1");
                option1.SetProperty(FormProperty.FORM_FIELD_LABEL, "option 1");
                formListBoxField.AddOption(option1);
                Paragraph option2 = new Paragraph("option 2");
                option2.SetProperty(FormProperty.FORM_FIELD_SELECTED, true);
                option2.SetProperty(FormProperty.FORM_FIELD_LABEL, "option 2");
                formListBoxField.AddOption(option2);
                document.Add(formListBoxField);
                ListBoxField flattenListBoxField = new ListBoxField("flatten list box field", 2, false);
                flattenListBoxField.SetProperty(FormProperty.FORM_FIELD_FLATTEN, true);
                flattenListBoxField.AddOption(option1);
                flattenListBoxField.AddOption(option2);
                document.Add(flattenListBoxField);
                Paragraph option3 = new Paragraph("option 3");
                option3.SetProperty(FormProperty.FORM_FIELD_SELECTED, true);
                option3.SetProperty(FormProperty.FORM_FIELD_LABEL, "option 3");
                ListBoxField flattenListBoxFieldWithMultipleSelection = new ListBoxField("flatten list box field with multiple selection"
                    , 3, true);
                flattenListBoxFieldWithMultipleSelection.SetProperty(FormProperty.FORM_FIELD_FLATTEN, true);
                flattenListBoxFieldWithMultipleSelection.AddOption(option1);
                flattenListBoxFieldWithMultipleSelection.AddOption(option2);
                flattenListBoxFieldWithMultipleSelection.AddOption(option3);
                document.Add(flattenListBoxFieldWithMultipleSelection);
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, DESTINATION_FOLDER));
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.PROPERTY_IN_PERCENTS_NOT_SUPPORTED, Count = 22)]
        public virtual void ListBoxFieldWithoutSelectionTest() {
            String outPdf = DESTINATION_FOLDER + "listBoxFieldWithoutSelection.pdf";
            String cmpPdf = SOURCE_FOLDER + "cmp_listBoxFieldWithoutSelection.pdf";
            using (Document document = new Document(new PdfDocument(new PdfWriter(outPdf)))) {
                Paragraph option1 = new Paragraph("option 1");
                option1.SetProperty(FormProperty.FORM_FIELD_LABEL, "option 1");
                Paragraph option2 = new Paragraph("option 2");
                option2.SetProperty(FormProperty.FORM_FIELD_LABEL, "option 2");
                ListBoxField flattenListBoxFieldWithFont = new ListBoxField("flatten list box field with font", 0, false);
                flattenListBoxFieldWithFont.SetProperty(FormProperty.FORM_FIELD_FLATTEN, true);
                flattenListBoxFieldWithFont.SetBackgroundColor(ColorConstants.RED);
                flattenListBoxFieldWithFont.AddOption(option1);
                flattenListBoxFieldWithFont.AddOption(option2);
                document.Add(flattenListBoxFieldWithFont);
                ListBoxField flattenListBoxFieldWithPercentFont = new ListBoxField("flatten list box field with percent font"
                    , 0, false);
                flattenListBoxFieldWithPercentFont.SetProperty(FormProperty.FORM_FIELD_FLATTEN, true);
                flattenListBoxFieldWithPercentFont.SetBackgroundColor(ColorConstants.RED);
                flattenListBoxFieldWithPercentFont.AddOption(option1);
                flattenListBoxFieldWithPercentFont.AddOption(option2);
                flattenListBoxFieldWithPercentFont.SetProperty(Property.FONT_SIZE, UnitValue.CreatePercentValue(10));
                document.Add(flattenListBoxFieldWithPercentFont);
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, DESTINATION_FOLDER));
        }

        [NUnit.Framework.Test]
        public virtual void ListBoxFieldWithOverflowTest() {
            String outPdf = DESTINATION_FOLDER + "listBoxFieldWithOverflow.pdf";
            String cmpPdf = SOURCE_FOLDER + "cmp_listBoxFieldWithOverflow.pdf";
            using (Document document = new Document(new PdfDocument(new PdfWriter(outPdf)))) {
                Paragraph option1 = new Paragraph("option 1");
                option1.SetProperty(FormProperty.FORM_FIELD_LABEL, "option 1");
                Paragraph option2 = new Paragraph("option 2");
                option2.SetProperty(FormProperty.FORM_FIELD_SELECTED, true);
                option2.SetProperty(FormProperty.FORM_FIELD_LABEL, "option 2");
                ListBoxField flattenListBoxField = new ListBoxField("flatten list box field with overflow", 0, false);
                flattenListBoxField.SetProperty(FormProperty.FORM_FIELD_FLATTEN, true);
                flattenListBoxField.SetBackgroundColor(ColorConstants.RED);
                flattenListBoxField.AddOption(option1);
                flattenListBoxField.AddOption(option2);
                flattenListBoxField.SetProperty(Property.OVERFLOW_Y, OverflowPropertyValue.HIDDEN);
                document.Add(flattenListBoxField);
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, DESTINATION_FOLDER));
        }

        [NUnit.Framework.Test]
        public virtual void ListBoxFieldWithHeightTest() {
            String outPdf = DESTINATION_FOLDER + "listBoxFieldWithHeight.pdf";
            String cmpPdf = SOURCE_FOLDER + "cmp_listBoxFieldWithHeight.pdf";
            using (Document document = new Document(new PdfDocument(new PdfWriter(outPdf)))) {
                Paragraph option1 = new Paragraph("option 1");
                option1.SetProperty(FormProperty.FORM_FIELD_LABEL, "option 1");
                Paragraph option2 = new Paragraph("option 2");
                option2.SetProperty(FormProperty.FORM_FIELD_SELECTED, true);
                option2.SetProperty(FormProperty.FORM_FIELD_LABEL, "option 2");
                ListBoxField flattenListBoxField = new ListBoxField("flatten list box field with height", 0, false);
                flattenListBoxField.SetProperty(FormProperty.FORM_FIELD_FLATTEN, true);
                flattenListBoxField.SetBackgroundColor(ColorConstants.RED);
                flattenListBoxField.AddOption(option1);
                flattenListBoxField.AddOption(option2);
                flattenListBoxField.SetProperty(Property.HEIGHT, UnitValue.CreatePointValue(100));
                document.Add(flattenListBoxField);
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, DESTINATION_FOLDER));
        }

        [NUnit.Framework.Test]
        public virtual void ListBoxFieldWithMinHeightTest() {
            String outPdf = DESTINATION_FOLDER + "listBoxFieldWithMinHeight.pdf";
            String cmpPdf = SOURCE_FOLDER + "cmp_listBoxFieldWithMinHeight.pdf";
            using (Document document = new Document(new PdfDocument(new PdfWriter(outPdf)))) {
                Paragraph option1 = new Paragraph("option 1");
                option1.SetProperty(FormProperty.FORM_FIELD_LABEL, "option 1");
                Paragraph option2 = new Paragraph("option 2");
                option2.SetProperty(FormProperty.FORM_FIELD_SELECTED, true);
                option2.SetProperty(FormProperty.FORM_FIELD_LABEL, "option 2");
                ListBoxField flattenListBoxField = new ListBoxField("flatten list box field with min height", 0, false);
                flattenListBoxField.SetProperty(FormProperty.FORM_FIELD_FLATTEN, true);
                flattenListBoxField.SetBackgroundColor(ColorConstants.RED);
                flattenListBoxField.AddOption(option1);
                flattenListBoxField.AddOption(option2);
                flattenListBoxField.SetProperty(Property.MIN_HEIGHT, UnitValue.CreatePointValue(100));
                document.Add(flattenListBoxField);
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, DESTINATION_FOLDER));
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.CLIP_ELEMENT)]
        public virtual void ListBoxFieldWithMaxHeightTest() {
            String outPdf = DESTINATION_FOLDER + "listBoxFieldWithMaxHeight.pdf";
            String cmpPdf = SOURCE_FOLDER + "cmp_listBoxFieldWithMaxHeight.pdf";
            using (Document document = new Document(new PdfDocument(new PdfWriter(outPdf)))) {
                Paragraph option1 = new Paragraph("option 1");
                option1.SetProperty(FormProperty.FORM_FIELD_SELECTED, true);
                option1.SetProperty(FormProperty.FORM_FIELD_LABEL, "option 1");
                Paragraph option2 = new Paragraph("option 2");
                option2.SetProperty(FormProperty.FORM_FIELD_LABEL, "option 2");
                ListBoxField flattenListBoxField = new ListBoxField("flatten list box field with max height", 0, false);
                flattenListBoxField.SetProperty(FormProperty.FORM_FIELD_FLATTEN, true);
                flattenListBoxField.SetBackgroundColor(ColorConstants.RED);
                flattenListBoxField.AddOption(option1);
                flattenListBoxField.AddOption(option2);
                flattenListBoxField.SetProperty(Property.MAX_HEIGHT, UnitValue.CreatePointValue(40));
                document.Add(flattenListBoxField);
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, DESTINATION_FOLDER));
        }

        [NUnit.Framework.Test]
        public virtual void ListBoxFieldCannotFitTest() {
            String outPdf = DESTINATION_FOLDER + "listBoxFieldCannotFit.pdf";
            String cmpPdf = SOURCE_FOLDER + "cmp_listBoxFieldCannotFit.pdf";
            using (Document document = new Document(new PdfDocument(new PdfWriter(outPdf)))) {
                Div div = new Div();
                div.SetWidth(UnitValue.CreatePointValue(400));
                div.SetHeight(UnitValue.CreatePointValue(740));
                div.SetBackgroundColor(ColorConstants.PINK);
                document.Add(div);
                Paragraph option1 = new Paragraph("option 1");
                option1.SetProperty(FormProperty.FORM_FIELD_SELECTED, true);
                option1.SetProperty(FormProperty.FORM_FIELD_LABEL, "option 1");
                Paragraph option2 = new Paragraph("option 2");
                option2.SetProperty(FormProperty.FORM_FIELD_LABEL, "option 2");
                ListBoxField flattenListBoxField = new ListBoxField("flatten list box field cannot fit", 0, false);
                flattenListBoxField.SetProperty(FormProperty.FORM_FIELD_FLATTEN, true);
                flattenListBoxField.SetBackgroundColor(ColorConstants.RED);
                flattenListBoxField.AddOption(option1);
                flattenListBoxField.AddOption(option2);
                document.Add(flattenListBoxField);
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, DESTINATION_FOLDER));
        }

        [NUnit.Framework.Test]
        public virtual void ListBoxFieldCannotFitByWidthTest() {
            String outPdf = DESTINATION_FOLDER + "listBoxFieldCannotFitByWidth.pdf";
            String cmpPdf = SOURCE_FOLDER + "cmp_listBoxFieldCannotFitByWidth.pdf";
            using (Document document = new Document(new PdfDocument(new PdfWriter(outPdf)))) {
                Paragraph option1 = new Paragraph("option 1");
                option1.SetProperty(FormProperty.FORM_FIELD_SELECTED, true);
                option1.SetProperty(FormProperty.FORM_FIELD_LABEL, "option 1");
                Paragraph option2 = new Paragraph("option 2");
                option2.SetProperty(FormProperty.FORM_FIELD_LABEL, "option 2");
                ListBoxField flattenListBoxField = new ListBoxField("flatten list box field cannot fit by width", 0, false
                    );
                flattenListBoxField.SetProperty(FormProperty.FORM_FIELD_FLATTEN, true);
                flattenListBoxField.SetBackgroundColor(ColorConstants.RED);
                flattenListBoxField.SetProperty(Property.WIDTH, UnitValue.CreatePointValue(600));
                flattenListBoxField.SetBorder(new SolidBorder(20));
                flattenListBoxField.AddOption(option1);
                flattenListBoxField.AddOption(option2);
                document.Add(flattenListBoxField);
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, DESTINATION_FOLDER));
        }

        [NUnit.Framework.Test]
        public virtual void ListBoxFieldWithLangTest() {
            String outPdf = DESTINATION_FOLDER + "listBoxFieldWithLang.pdf";
            String cmpPdf = SOURCE_FOLDER + "cmp_listBoxFieldWithLang.pdf";
            using (Document document = new Document(new PdfDocument(new PdfWriter(outPdf)))) {
                Paragraph option1 = new Paragraph("option 1");
                option1.SetProperty(FormProperty.FORM_FIELD_SELECTED, true);
                option1.SetProperty(FormProperty.FORM_FIELD_LABEL, "option 1");
                Paragraph option2 = new Paragraph("option 2");
                option2.SetProperty(FormProperty.FORM_FIELD_LABEL, "option 2");
                ListBoxField flattenListBoxField = new ListBoxField("flatten list box field with lang", 0, false);
                flattenListBoxField.SetProperty(FormProperty.FORM_FIELD_FLATTEN, true);
                flattenListBoxField.SetBackgroundColor(ColorConstants.RED);
                flattenListBoxField.AddOption(option1);
                flattenListBoxField.AddOption(option2);
                flattenListBoxField.SetProperty(FormProperty.FORM_ACCESSIBILITY_LANGUAGE, "random_lang");
                document.Add(flattenListBoxField);
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, DESTINATION_FOLDER));
        }
    }
}
