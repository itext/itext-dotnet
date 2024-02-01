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
using System.IO;
using iText.Forms;
using iText.Forms.Exceptions;
using iText.Forms.Fields;
using iText.Forms.Form;
using iText.IO.Font.Constants;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Geom;
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
        public virtual void BasicListBoxFieldTest() {
            String outPdf = DESTINATION_FOLDER + "basicListBoxField.pdf";
            String cmpPdf = SOURCE_FOLDER + "cmp_basicListBoxField.pdf";
            using (Document document = new Document(new PdfDocument(new PdfWriter(outPdf)))) {
                ListBoxField formListBoxField = new ListBoxField("form list box field", 2, false);
                formListBoxField.SetInteractive(true);
                formListBoxField.AddOption("option 1", false);
                formListBoxField.AddOption("option 2", true);
                document.Add(formListBoxField);
                ListBoxField flattenListBoxField = new ListBoxField("flatten list box field", 2, false);
                flattenListBoxField.SetProperty(FormProperty.FORM_FIELD_FLATTEN, true);
                flattenListBoxField.AddOption("option 1", false);
                flattenListBoxField.AddOption("option 2", true);
                document.Add(flattenListBoxField);
                Paragraph option3 = new Paragraph("option 3");
                option3.SetProperty(FormProperty.FORM_FIELD_SELECTED, true);
                option3.SetMargin(0);
                option3.SetMultipliedLeading(2);
                ListBoxField flattenListBoxFieldWithMultipleSelection = new ListBoxField("flatten list box field with multiple selection"
                    , 3, true);
                flattenListBoxFieldWithMultipleSelection.SetInteractive(false);
                flattenListBoxFieldWithMultipleSelection.AddOption("option 1", false);
                flattenListBoxFieldWithMultipleSelection.AddOption("option 2", true);
                flattenListBoxFieldWithMultipleSelection.AddOption(option3);
                document.Add(flattenListBoxFieldWithMultipleSelection);
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, DESTINATION_FOLDER));
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.PROPERTY_IN_PERCENTS_NOT_SUPPORTED, Count = 22)]
        public virtual void ListBoxFieldWithFontSizeTest() {
            String outPdf = DESTINATION_FOLDER + "listBoxFieldWithFontSize.pdf";
            String cmpPdf = SOURCE_FOLDER + "cmp_listBoxFieldWithFontSize.pdf";
            using (Document document = new Document(new PdfDocument(new PdfWriter(outPdf)))) {
                ListBoxField formListBoxFieldWithFont = new ListBoxField("flatten list box field with font", 0, false);
                formListBoxFieldWithFont.SetInteractive(true);
                formListBoxFieldWithFont.SetBackgroundColor(ColorConstants.RED);
                formListBoxFieldWithFont.AddOption("option 1");
                formListBoxFieldWithFont.AddOption("option 2");
                formListBoxFieldWithFont.SetFont(PdfFontFactory.CreateFont(StandardFonts.COURIER));
                formListBoxFieldWithFont.SetFontSize(6);
                document.Add(formListBoxFieldWithFont);
                document.Add(new Paragraph("line break"));
                ListBoxField flattenListBoxFieldWithFont = new ListBoxField("flatten list box field with font", 0, false);
                flattenListBoxFieldWithFont.SetInteractive(false);
                flattenListBoxFieldWithFont.SetBackgroundColor(ColorConstants.RED);
                flattenListBoxFieldWithFont.AddOption("option 1");
                flattenListBoxFieldWithFont.AddOption("option 2");
                flattenListBoxFieldWithFont.SetFont(PdfFontFactory.CreateFont(StandardFonts.COURIER));
                flattenListBoxFieldWithFont.SetFontSize(6);
                document.Add(flattenListBoxFieldWithFont);
                document.Add(new Paragraph("line break"));
                ListBoxField flattenListBoxFieldWithPercentFont = new ListBoxField("flatten list box field with percent font"
                    , 0, false);
                flattenListBoxFieldWithFont.SetInteractive(false);
                flattenListBoxFieldWithPercentFont.SetBackgroundColor(ColorConstants.RED);
                flattenListBoxFieldWithPercentFont.AddOption("option 1");
                flattenListBoxFieldWithPercentFont.AddOption("option 2");
                flattenListBoxFieldWithPercentFont.SetFont(PdfFontFactory.CreateFont(StandardFonts.COURIER));
                flattenListBoxFieldWithPercentFont.SetProperty(Property.FONT_SIZE, UnitValue.CreatePercentValue(6));
                document.Add(flattenListBoxFieldWithPercentFont);
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, DESTINATION_FOLDER));
        }

        [NUnit.Framework.Test]
        public virtual void ListBoxFieldWithMarginsTest() {
            String outPdf = DESTINATION_FOLDER + "listBoxFieldWithMargins.pdf";
            String cmpPdf = SOURCE_FOLDER + "cmp_listBoxFieldWithMargins.pdf";
            using (Document document = new Document(new PdfDocument(new PdfWriter(outPdf)))) {
                Paragraph option1 = new Paragraph("option 1");
                option1.SetProperty(FormProperty.FORM_FIELD_LABEL, "option 1");
                option1.SetMargin(4);
                Paragraph option2 = new Paragraph("option 2");
                option2.SetProperty(FormProperty.FORM_FIELD_SELECTED, true);
                option2.SetProperty(FormProperty.FORM_FIELD_LABEL, "option 2");
                option2.SetMargin(4);
                ListBoxField listBoxField = new ListBoxField("list box field with margins", 1, false);
                listBoxField.SetInteractive(false);
                listBoxField.SetBackgroundColor(ColorConstants.RED);
                listBoxField.AddOption(option1);
                listBoxField.AddOption(option2);
                document.Add(listBoxField);
                document.Add(new Paragraph("line break"));
                document.Add(listBoxField);
                document.Add(new Paragraph("line break"));
                document.Add(listBoxField.SetInteractive(true));
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, DESTINATION_FOLDER));
        }

        [NUnit.Framework.Test]
        public virtual void ListBoxFieldWithHeightTest() {
            String outPdf = DESTINATION_FOLDER + "listBoxFieldWithHeight.pdf";
            String cmpPdf = SOURCE_FOLDER + "cmp_listBoxFieldWithHeight.pdf";
            using (Document document = new Document(new PdfDocument(new PdfWriter(outPdf)))) {
                ListBoxField listBoxField = new ListBoxField("list box field with height", 0, false);
                listBoxField.SetInteractive(false);
                listBoxField.SetBackgroundColor(ColorConstants.RED);
                listBoxField.AddOption("option 1");
                listBoxField.AddOption("option 2", true);
                listBoxField.SetHeight(100);
                document.Add(listBoxField);
                document.Add(new Paragraph("line break"));
                document.Add(listBoxField.SetInteractive(true));
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, DESTINATION_FOLDER));
        }

        [NUnit.Framework.Test]
        public virtual void ListBoxFieldWithMinHeightTest() {
            String outPdf = DESTINATION_FOLDER + "listBoxFieldWithMinHeight.pdf";
            String cmpPdf = SOURCE_FOLDER + "cmp_listBoxFieldWithMinHeight.pdf";
            using (Document document = new Document(new PdfDocument(new PdfWriter(outPdf)))) {
                ListBoxField listBoxField = new ListBoxField("list box field with height", 0, false);
                listBoxField.SetInteractive(false);
                listBoxField.SetBackgroundColor(ColorConstants.RED);
                listBoxField.AddOption("option 1");
                listBoxField.AddOption("option 2", true);
                listBoxField.SetProperty(Property.MIN_HEIGHT, UnitValue.CreatePointValue(100));
                document.Add(listBoxField);
                document.Add(new Paragraph("line break"));
                document.Add(listBoxField.SetInteractive(true));
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, DESTINATION_FOLDER));
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.CLIP_ELEMENT, Count = 2)]
        public virtual void ListBoxFieldWithMaxHeightTest() {
            String outPdf = DESTINATION_FOLDER + "listBoxFieldWithMaxHeight.pdf";
            String cmpPdf = SOURCE_FOLDER + "cmp_listBoxFieldWithMaxHeight.pdf";
            using (Document document = new Document(new PdfDocument(new PdfWriter(outPdf)))) {
                ListBoxField listBoxField = new ListBoxField("list box field with height", 0, false);
                listBoxField.SetInteractive(false);
                listBoxField.SetBackgroundColor(ColorConstants.RED);
                listBoxField.AddOption("option 1", false);
                listBoxField.AddOption("option 2", true);
                listBoxField.SetProperty(Property.MAX_HEIGHT, UnitValue.CreatePointValue(25));
                document.Add(listBoxField);
                document.Add(new Paragraph("line break"));
                document.Add(listBoxField.SetInteractive(true));
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
                ListBoxField listBoxField = new ListBoxField("list box field cannot fit", 0, false);
                listBoxField.SetInteractive(true);
                listBoxField.SetBackgroundColor(ColorConstants.RED);
                listBoxField.AddOption("option 1", true);
                listBoxField.AddOption("option 2");
                document.Add(listBoxField);
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
                ListBoxField listBoxField = new ListBoxField("list box field cannot fit by width", 0, false);
                listBoxField.SetInteractive(false);
                listBoxField.SetBackgroundColor(ColorConstants.RED);
                listBoxField.SetProperty(Property.WIDTH, UnitValue.CreatePointValue(600));
                listBoxField.SetBorder(new SolidBorder(20));
                listBoxField.AddOption(option1);
                listBoxField.AddOption(option2);
                document.Add(listBoxField);
                document.Add(new Paragraph("Line break"));
                document.Add(listBoxField.SetInteractive(true));
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, DESTINATION_FOLDER));
        }

        [NUnit.Framework.Test]
        public virtual void ListBoxFieldWithLangTest() {
            String outPdf = DESTINATION_FOLDER + "listBoxFieldWithLang.pdf";
            String cmpPdf = SOURCE_FOLDER + "cmp_listBoxFieldWithLang.pdf";
            using (Document document = new Document(new PdfDocument(new PdfWriter(outPdf)))) {
                document.GetPdfDocument().SetTagged();
                ListBoxField listBoxField = new ListBoxField("list box field with lang", 0, false);
                listBoxField.SetInteractive(false);
                listBoxField.SetBackgroundColor(ColorConstants.RED);
                listBoxField.AddOption("option 1");
                listBoxField.AddOption("option 2");
                listBoxField.SetProperty(FormProperty.FORM_ACCESSIBILITY_LANGUAGE, "random_lang");
                document.Add(listBoxField);
                document.Add(new Paragraph("Line break"));
                document.Add(listBoxField.SetInteractive(true));
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, DESTINATION_FOLDER));
        }

        [NUnit.Framework.Test]
        public virtual void ColorsBordersTest() {
            String outPdf = DESTINATION_FOLDER + "colorsBorders.pdf";
            String cmpPdf = SOURCE_FOLDER + "cmp_colorsBorders.pdf";
            using (Document document = new Document(new PdfDocument(new PdfWriter(outPdf)))) {
                ListBoxField listBoxField = new ListBoxField("coloured list box field with borders", 0, false);
                listBoxField.SetInteractive(false);
                listBoxField.SetBackgroundColor(ColorConstants.RED);
                listBoxField.AddOption("option 1");
                listBoxField.AddOption("option 2", true);
                listBoxField.SetBorder(new DashedBorder(ColorConstants.BLUE, 3));
                listBoxField.SetFontColor(ColorConstants.GREEN);
                document.Add(listBoxField);
                document.Add(new Paragraph("Line break"));
                document.Add(listBoxField.SetInteractive(true));
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, DESTINATION_FOLDER));
        }

        [NUnit.Framework.Test]
        public virtual void LongListTest() {
            String outPdf = DESTINATION_FOLDER + "longList.pdf";
            String cmpPdf = SOURCE_FOLDER + "cmp_longList.pdf";
            using (Document document = new Document(new PdfDocument(new PdfWriter(outPdf)))) {
                ListBoxField listBoxField = new ListBoxField("long list box field", 4, true);
                listBoxField.SetInteractive(false);
                listBoxField.AddOption("option 1");
                listBoxField.AddOption("option 2");
                listBoxField.AddOption("option 3");
                listBoxField.AddOption("option 4");
                listBoxField.AddOption("option 5");
                listBoxField.AddOption("option 6", true);
                listBoxField.AddOption("option 7");
                listBoxField.AddOption("option 8");
                listBoxField.AddOption("option 9");
                listBoxField.AddOption("very very very long long long option 10", true);
                listBoxField.AddOption("option 11");
                document.Add(listBoxField);
                document.Add(new Paragraph("Line break"));
                document.Add(listBoxField.SetInteractive(true));
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, DESTINATION_FOLDER));
        }

        [NUnit.Framework.Test]
        public virtual void JustificationTest() {
            String outPdf = DESTINATION_FOLDER + "justification.pdf";
            String cmpPdf = SOURCE_FOLDER + "cmp_justification.pdf";
            using (Document document = new Document(new PdfDocument(new PdfWriter(outPdf)))) {
                ListBoxField listBoxField = new ListBoxField("left box field", 0, false);
                listBoxField.SetInteractive(false);
                listBoxField.SetWidth(200);
                listBoxField.SetTextAlignment(TextAlignment.LEFT);
                listBoxField.AddOption("option 1");
                listBoxField.AddOption("option 2", true);
                document.Add(listBoxField);
                document.Add(new Paragraph("Line break"));
                document.Add(listBoxField.SetInteractive(true));
                ListBoxField centerListBoxField = new ListBoxField("center box field", 0, false);
                centerListBoxField.SetInteractive(false);
                centerListBoxField.SetWidth(200);
                centerListBoxField.SetTextAlignment(TextAlignment.CENTER);
                centerListBoxField.AddOption("option 1");
                centerListBoxField.AddOption("option 2", true);
                document.Add(centerListBoxField);
                document.Add(new Paragraph("Line break"));
                document.Add(centerListBoxField.SetInteractive(true));
                ListBoxField rightListBoxField = new ListBoxField("right box field", 0, false);
                rightListBoxField.SetInteractive(false);
                rightListBoxField.SetWidth(200);
                rightListBoxField.SetTextAlignment(TextAlignment.RIGHT);
                rightListBoxField.AddOption("option 1");
                rightListBoxField.AddOption("option 2", true);
                document.Add(rightListBoxField);
                document.Add(new Paragraph("Line break"));
                document.Add(rightListBoxField.SetInteractive(true));
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, DESTINATION_FOLDER));
        }

        [NUnit.Framework.Test]
        public virtual void ExportValueTest() {
            String outPdf = DESTINATION_FOLDER + "exportValue.pdf";
            String cmpPdf = SOURCE_FOLDER + "cmp_exportValue.pdf";
            using (Document document = new Document(new PdfDocument(new PdfWriter(outPdf)))) {
                ListBoxField listBoxField = new ListBoxField("export value field", 0, true);
                listBoxField.SetInteractive(false);
                listBoxField.SetWidth(200);
                listBoxField.AddOption(new SelectFieldItem("English"));
                listBoxField.AddOption(new SelectFieldItem("German", "Deutch"), true);
                listBoxField.AddOption(new SelectFieldItem("Italian", "Italiano"), true);
                document.Add(listBoxField);
                document.Add(new Paragraph("Line break"));
                document.Add(listBoxField.SetInteractive(true));
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, DESTINATION_FOLDER));
        }

        [NUnit.Framework.Test]
        public virtual void InvalidOptionsTest() {
            String outPdf = DESTINATION_FOLDER + "invalidOptions.pdf";
            String cmpPdf = SOURCE_FOLDER + "cmp_invalidOptions.pdf";
            using (PdfDocument doc = new PdfDocument(new PdfWriter(outPdf))) {
                ListBoxField listBoxField = new ListBoxField("invalid", 0, true);
                listBoxField.SetInteractive(true);
                listBoxField.SetWidth(200);
                // Invalid options array here
                PdfArray option1 = new PdfArray();
                option1.Add(new PdfString("English"));
                option1.Add(new PdfString("English"));
                option1.Add(new PdfString("English3"));
                PdfArray option2 = new PdfArray();
                option2.Add(new PdfString("German"));
                option2.Add(new PdfString("Deutch"));
                PdfArray option3 = new PdfArray();
                option3.Add(new PdfString("Italian"));
                PdfArray options = new PdfArray();
                options.Add(option1);
                options.Add(option2);
                options.Add(option3);
                options.Add(new PdfArray());
                PdfChoiceFormField field = new ChoiceFormFieldBuilder(doc, "invalid").SetWidgetRectangle(new Rectangle(100
                    , 500, 100, 100)).CreateList();
                field.SetOptions(options);
                field.GetFirstFormAnnotation().SetFormFieldElement(listBoxField);
                PdfAcroForm.GetAcroForm(doc, true).AddField(field);
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, DESTINATION_FOLDER));
        }

        [NUnit.Framework.Test]
        public virtual void InvalidOptionsExceptionTest() {
            using (PdfDocument doc = new PdfDocument(new PdfWriter(new MemoryStream()))) {
                ChoiceFormFieldBuilder builder = new ChoiceFormFieldBuilder(doc, "invalid").SetWidgetRectangle(new Rectangle
                    (100, 500, 100, 100));
                PdfArray option1 = new PdfArray();
                option1.Add(new PdfString("English"));
                option1.Add(new PdfString("English"));
                option1.Add(new PdfString("English3"));
                PdfArray options = new PdfArray();
                options.Add(option1);
                Exception e = NUnit.Framework.Assert.Catch(typeof(ArgumentException), () => builder.SetOptions(options));
                NUnit.Framework.Assert.AreEqual(FormsExceptionMessageConstant.INNER_ARRAY_SHALL_HAVE_TWO_ELEMENTS, e.Message
                    );
                options.Clear();
                option1 = new PdfArray();
                option1.Add(new PdfString("English"));
                option1.Add(new PdfNumber(1));
                options.Add(option1);
                e = NUnit.Framework.Assert.Catch(typeof(ArgumentException), () => builder.SetOptions(options));
                NUnit.Framework.Assert.AreEqual(FormsExceptionMessageConstant.OPTION_ELEMENT_MUST_BE_STRING_OR_ARRAY, e.Message
                    );
                PdfArray options2 = new PdfArray();
                options2.Add(new PdfNumber(1));
                e = NUnit.Framework.Assert.Catch(typeof(ArgumentException), () => builder.SetOptions(options2));
                NUnit.Framework.Assert.AreEqual(FormsExceptionMessageConstant.OPTION_ELEMENT_MUST_BE_STRING_OR_ARRAY, e.Message
                    );
            }
        }
    }
}
