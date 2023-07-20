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
using System.Collections.Generic;
using iText.Commons.Utils;
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
                flattenComboBoxField.SetInteractive(false);
                flattenComboBoxField.SetBackgroundColor(ColorConstants.RED);
                document.Add(flattenComboBoxField);
                ComboBoxField comboBoxWithBorder = new ComboBoxField("with boderder");
                comboBoxWithBorder.SetBorder(new SolidBorder(ColorConstants.BLUE, 1));
                document.Add(comboBoxWithBorder);
                ComboBoxField comboBoxWithBackgroundColor = new ComboBoxField("with background color");
                comboBoxWithBackgroundColor.SetBackgroundColor(ColorConstants.GREEN);
                comboBoxWithBackgroundColor.SetInteractive(true);
                document.Add(comboBoxWithBackgroundColor);
                ComboBoxField comboBoxWithBorderAndBackgroundColor = new ComboBoxField("with border");
                comboBoxWithBorderAndBackgroundColor.SetBorder(new SolidBorder(ColorConstants.BLUE, 1));
                comboBoxWithBorderAndBackgroundColor.SetInteractive(true);
                document.Add(comboBoxWithBorderAndBackgroundColor);
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, DESTINATION_FOLDER));
        }

        [NUnit.Framework.Test]
        public virtual void BasicComboBoxFieldTest() {
            String outPdf = DESTINATION_FOLDER + "basicComboBoxField.pdf";
            String cmpPdf = SOURCE_FOLDER + "cmp_basicComboBoxField.pdf";
            using (Document document = new Document(new PdfDocument(new PdfWriter(outPdf)))) {
                ComboBoxField formComboBoxField = new ComboBoxField("form combo box field");
                formComboBoxField.SetInteractive(true);
                formComboBoxField.AddOption(new SelectFieldItem("option 1"));
                formComboBoxField.AddOption(new SelectFieldItem("option 2"));
                document.Add(formComboBoxField);
                ComboBoxField flattenComboBoxField = new ComboBoxField("flatten combo box field");
                flattenComboBoxField.SetInteractive(false);
                flattenComboBoxField.AddOption(new SelectFieldItem("option 1"));
                flattenComboBoxField.AddOption(new SelectFieldItem("option 2"));
                document.Add(flattenComboBoxField);
                ComboBoxField formComboBoxFieldSelected = new ComboBoxField("form combo box field selected");
                formComboBoxFieldSelected.SetInteractive(true);
                formComboBoxFieldSelected.AddOption(new SelectFieldItem("option 1"));
                formComboBoxFieldSelected.AddOption(new SelectFieldItem("option 2"));
                formComboBoxFieldSelected.SetSelected("option 1");
                document.Add(formComboBoxFieldSelected);
                ComboBoxField flattenComboBoxFieldSelected = new ComboBoxField("flatten combo box field selected");
                flattenComboBoxFieldSelected.SetInteractive(false);
                flattenComboBoxFieldSelected.AddOption(new SelectFieldItem("option 1"));
                flattenComboBoxFieldSelected.AddOption(new SelectFieldItem("option 2"));
                flattenComboBoxFieldSelected.SetSelected("option 1");
                document.Add(flattenComboBoxFieldSelected);
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, DESTINATION_FOLDER));
        }

        [NUnit.Framework.Test]
        public virtual void BasicComboBoxFieldWithBordersTest() {
            String outPdf = DESTINATION_FOLDER + "basicComboBoxBorderTest.pdf";
            String cmpPdf = SOURCE_FOLDER + "cmp_basicComboBoxBorderTest.pdf";
            IList<Border> borderList = new List<Border>();
            borderList.Add(new SolidBorder(ColorConstants.RED, .7f));
            borderList.Add(new SolidBorder(ColorConstants.GREEN, 1));
            borderList.Add(new SolidBorder(ColorConstants.BLUE, 2));
            using (Document document = new Document(new PdfDocument(new PdfWriter(outPdf)))) {
                for (int i = 0; i < borderList.Count; i++) {
                    ComboBoxField formComboBoxField = new ComboBoxField("form combo box field" + i);
                    formComboBoxField.SetInteractive(true);
                    SelectFieldItem option1 = new SelectFieldItem("option 1");
                    formComboBoxField.AddOption(option1);
                    SelectFieldItem option2 = new SelectFieldItem("option 2");
                    formComboBoxField.AddOption(option2);
                    formComboBoxField.SetSelected(option1);
                    formComboBoxField.SetBorder(borderList[i]);
                    document.Add(formComboBoxField);
                    ComboBoxField flattenComboBoxField = new ComboBoxField("flatten combo box field" + i);
                    flattenComboBoxField.SetInteractive(false);
                    SelectFieldItem option3 = new SelectFieldItem("option 1");
                    flattenComboBoxField.AddOption(option3);
                    flattenComboBoxField.SetSelected(option3);
                    flattenComboBoxField.SetBorder(borderList[i]);
                    SelectFieldItem option4 = new SelectFieldItem("option 2");
                    flattenComboBoxField.AddOption(option4);
                    document.Add(flattenComboBoxField);
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, DESTINATION_FOLDER));
        }

        [NUnit.Framework.Test]
        public virtual void BasicComboBoxFieldWithBackgroundTest() {
            String outPdf = DESTINATION_FOLDER + "basicComboBoxBackgroundTest.pdf";
            String cmpPdf = SOURCE_FOLDER + "cmp_basicComboBoxBackgroundTest.pdf";
            IList<Color> borderList = new List<Color>();
            borderList.Add(ColorConstants.RED);
            borderList.Add(ColorConstants.GREEN);
            borderList.Add(ColorConstants.BLUE);
            using (Document document = new Document(new PdfDocument(new PdfWriter(outPdf)))) {
                for (int i = 0; i < borderList.Count; i++) {
                    ComboBoxField formComboBoxField = new ComboBoxField("form combo box field" + i);
                    formComboBoxField.SetInteractive(true);
                    SelectFieldItem option1 = new SelectFieldItem("option 1");
                    formComboBoxField.AddOption(option1);
                    SelectFieldItem option2 = new SelectFieldItem("option 2");
                    formComboBoxField.AddOption(option2);
                    formComboBoxField.SetSelected(option1);
                    formComboBoxField.SetBackgroundColor(borderList[i]);
                    document.Add(formComboBoxField);
                    ComboBoxField flattenComboBoxField = new ComboBoxField("flatten combo box field" + i);
                    flattenComboBoxField.SetInteractive(false);
                    SelectFieldItem option3 = new SelectFieldItem("option 1");
                    flattenComboBoxField.AddOption(option3);
                    flattenComboBoxField.SetSelected(option3);
                    flattenComboBoxField.SetBackgroundColor(borderList[i]);
                    SelectFieldItem option4 = new SelectFieldItem("option 2");
                    flattenComboBoxField.AddOption(option4);
                    document.Add(flattenComboBoxField);
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, DESTINATION_FOLDER));
        }

        [NUnit.Framework.Test]
        public virtual void ComboBoxFieldWithoutSelectionTest() {
            String outPdf = DESTINATION_FOLDER + "comboBoxFieldWithoutSelection.pdf";
            String cmpPdf = SOURCE_FOLDER + "cmp_comboBoxFieldWithoutSelection.pdf";
            using (Document document = new Document(new PdfDocument(new PdfWriter(outPdf)))) {
                ComboBoxField flattenComboBoxFieldWithFont = new ComboBoxField("flatten combo box field with font");
                flattenComboBoxFieldWithFont.SetProperty(FormProperty.FORM_FIELD_FLATTEN, true);
                flattenComboBoxFieldWithFont.SetBackgroundColor(ColorConstants.RED);
                flattenComboBoxFieldWithFont.AddOption(new SelectFieldItem("option 1"));
                flattenComboBoxFieldWithFont.AddOption(new SelectFieldItem("option 2"));
                document.Add(flattenComboBoxFieldWithFont);
                ComboBoxField flattenComboBoxFieldWithPercentFont = new ComboBoxField("flatten combo box field with percent font"
                    );
                flattenComboBoxFieldWithPercentFont.SetProperty(FormProperty.FORM_FIELD_FLATTEN, true);
                flattenComboBoxFieldWithPercentFont.SetBackgroundColor(ColorConstants.RED);
                flattenComboBoxFieldWithPercentFont.AddOption(new SelectFieldItem("option 1"));
                flattenComboBoxFieldWithPercentFont.AddOption(new SelectFieldItem("option 2"));
                flattenComboBoxFieldWithPercentFont.SetProperty(Property.FONT_SIZE, UnitValue.CreatePercentValue(30));
                document.Add(flattenComboBoxFieldWithPercentFont);
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, DESTINATION_FOLDER));
        }

        [NUnit.Framework.Test]
        public virtual void ComboBoxFieldWithHeightTest() {
            String outPdf = DESTINATION_FOLDER + "comboBoxFieldWithHeight.pdf";
            String cmpPdf = SOURCE_FOLDER + "cmp_comboBoxFieldWithHeight.pdf";
            using (Document document = new Document(new PdfDocument(new PdfWriter(outPdf)))) {
                ComboBoxField flattenComboBoxField = new ComboBoxField("flatten combo box field with height");
                flattenComboBoxField.SetBackgroundColor(ColorConstants.RED);
                flattenComboBoxField.AddOption(new SelectFieldItem("option 1"));
                flattenComboBoxField.AddOption(new SelectFieldItem("option 2"));
                flattenComboBoxField.SetSelected("option 2");
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
                ComboBoxField flattenComboBoxField = new ComboBoxField("flatten combo box field with min height");
                flattenComboBoxField.SetBackgroundColor(ColorConstants.RED);
                flattenComboBoxField.AddOption(new SelectFieldItem("option 1"));
                flattenComboBoxField.AddOption(new SelectFieldItem("option 2"));
                flattenComboBoxField.SetSelected("option 2");
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
                ComboBoxField flattenComboBoxField = new ComboBoxField("flatten combo box field with max height");
                flattenComboBoxField.SetBackgroundColor(ColorConstants.RED);
                flattenComboBoxField.AddOption(new SelectFieldItem("option 1"));
                flattenComboBoxField.AddOption(new SelectFieldItem("option 2"));
                flattenComboBoxField.SetSelected("option 1");
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
                ComboBoxField flattenComboBoxField = new ComboBoxField("flatten combo box cannot fit");
                flattenComboBoxField.SetProperty(FormProperty.FORM_FIELD_FLATTEN, true);
                flattenComboBoxField.SetBackgroundColor(ColorConstants.RED);
                flattenComboBoxField.AddOption(new SelectFieldItem("option 1"));
                flattenComboBoxField.AddOption(new SelectFieldItem("option 2"));
                flattenComboBoxField.SetSelected("option 1");
                document.Add(flattenComboBoxField);
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, DESTINATION_FOLDER));
        }

        [NUnit.Framework.Test]
        public virtual void ComboBoxFieldWithLangTest() {
            String outPdf = DESTINATION_FOLDER + "comboBoxFieldWithLang.pdf";
            String cmpPdf = SOURCE_FOLDER + "cmp_comboBoxFieldWithLang.pdf";
            using (Document document = new Document(new PdfDocument(new PdfWriter(outPdf)))) {
                ComboBoxField flattenComboBoxField = new ComboBoxField("flatten combo box with lang");
                flattenComboBoxField.SetBackgroundColor(ColorConstants.RED);
                flattenComboBoxField.AddOption(new SelectFieldItem("option 1"));
                flattenComboBoxField.AddOption(new SelectFieldItem("option 2"));
                flattenComboBoxField.SetSelected("option 1");
                flattenComboBoxField.SetProperty(FormProperty.FORM_ACCESSIBILITY_LANGUAGE, "random_lang");
                document.Add(flattenComboBoxField);
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, DESTINATION_FOLDER));
        }

        [NUnit.Framework.Test]
        public virtual void SetFontSizeTest() {
            // test different font sizes
            String outPdf = DESTINATION_FOLDER + "comboBoxFontSizeTest.pdf";
            String cmpPdf = SOURCE_FOLDER + "cmp_comboBoxFontSizeTest.pdf";
            float?[] fontSizes = new float?[] { 4F, 8F, 12F, 16F, 20F, 24F };
            using (Document document = new Document(new PdfDocument(new PdfWriter(outPdf)))) {
                foreach (float? fontSize in fontSizes) {
                    ComboBoxField formComboBoxFieldSelected = new ComboBoxField("form combo box field selected" + MathematicUtil.Round
                        ((float)fontSize));
                    formComboBoxFieldSelected.SetInteractive(true);
                    formComboBoxFieldSelected.AddOption(new SelectFieldItem("option 1"));
                    formComboBoxFieldSelected.AddOption(new SelectFieldItem("option 2"));
                    formComboBoxFieldSelected.SetFontSize((float)fontSize);
                    formComboBoxFieldSelected.SetSelected("option 1");
                    document.Add(formComboBoxFieldSelected);
                    ComboBoxField flattenComboBoxFieldSelected = new ComboBoxField("flatten combo box field selected" + MathematicUtil.Round
                        ((float)fontSize));
                    flattenComboBoxFieldSelected.SetInteractive(false);
                    flattenComboBoxFieldSelected.AddOption(new SelectFieldItem("option 1"));
                    flattenComboBoxFieldSelected.AddOption(new SelectFieldItem("option 2"));
                    flattenComboBoxFieldSelected.SetFontSize((float)fontSize);
                    flattenComboBoxFieldSelected.SetSelected("option 1");
                    document.Add(flattenComboBoxFieldSelected);
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, DESTINATION_FOLDER));
        }

        [NUnit.Framework.Test]
        public virtual void NonSelectedInHtml2PdfSelectsFirstTest() {
            // test different font sizes
            String outPdf = DESTINATION_FOLDER + "nonSelectedInHtml2PdfSelectsFirst.pdf";
            String cmpPdf = SOURCE_FOLDER + "cmp_nonSelectedInHtml2PdfSelectsFirst.pdf";
            using (Document document = new Document(new PdfDocument(new PdfWriter(outPdf)))) {
                ComboBoxField formComboBoxFieldSelected = new ComboBoxField("form combo box field selected");
                formComboBoxFieldSelected.SetProperty(Property.RENDERING_MODE, RenderingMode.HTML_MODE);
                formComboBoxFieldSelected.SetInteractive(true);
                formComboBoxFieldSelected.SetWidth(150);
                formComboBoxFieldSelected.AddOption(new SelectFieldItem("option 1"));
                formComboBoxFieldSelected.AddOption(new SelectFieldItem("option 2"));
                document.Add(formComboBoxFieldSelected);
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, DESTINATION_FOLDER));
        }

        [NUnit.Framework.Test]
        public virtual void SetFontColorTest() {
            // test different font sizes
            String outPdf = DESTINATION_FOLDER + "comboBoxFontColorTest.pdf";
            String cmpPdf = SOURCE_FOLDER + "cmp_comboBoxFontColorTest.pdf";
            Color[] colors = new Color[] { ColorConstants.GREEN, ColorConstants.RED, ColorConstants.BLUE, ColorConstants
                .YELLOW, ColorConstants.ORANGE, ColorConstants.PINK };
            using (Document document = new Document(new PdfDocument(new PdfWriter(outPdf)))) {
                for (int i = 0; i < colors.Length; i++) {
                    Color color = colors[i];
                    ComboBoxField formComboBoxFieldSelected = new ComboBoxField("form combo box field selected" + i);
                    formComboBoxFieldSelected.SetInteractive(true);
                    formComboBoxFieldSelected.AddOption(new SelectFieldItem("option 1"));
                    formComboBoxFieldSelected.AddOption(new SelectFieldItem("option 2"));
                    formComboBoxFieldSelected.SetFontColor(color);
                    formComboBoxFieldSelected.SetSelected("option 1");
                    document.Add(formComboBoxFieldSelected);
                    ComboBoxField flattenComboBoxFieldSelected = new ComboBoxField("flatten combo box field selected" + i);
                    flattenComboBoxFieldSelected.SetInteractive(false);
                    flattenComboBoxFieldSelected.AddOption(new SelectFieldItem("option 1"));
                    flattenComboBoxFieldSelected.AddOption(new SelectFieldItem("option 2"));
                    flattenComboBoxFieldSelected.SetFontColor(color);
                    flattenComboBoxFieldSelected.SetSelected("option 1");
                    document.Add(flattenComboBoxFieldSelected);
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, DESTINATION_FOLDER));
        }

        [NUnit.Framework.Test]
        public virtual void NoneSelectedIsNullTest() {
            ComboBoxField comboBoxField = new ComboBoxField("test");
            comboBoxField.AddOption(new SelectFieldItem("option 1"));
            comboBoxField.AddOption(new SelectFieldItem("option 2"));
            NUnit.Framework.Assert.IsNull(comboBoxField.GetSelectedOption());
        }

        [NUnit.Framework.Test]
        public virtual void SetSelectedByExportValueTest() {
            ComboBoxField comboBoxField = new ComboBoxField("test");
            comboBoxField.AddOption(new SelectFieldItem("option 1"));
            comboBoxField.AddOption(new SelectFieldItem("option 2"));
            comboBoxField.AddOption(new SelectFieldItem("option 3"));
            comboBoxField.SetSelected("option 1");
            NUnit.Framework.Assert.AreEqual("option 1", comboBoxField.GetSelectedOption().GetDisplayValue());
            NUnit.Framework.Assert.AreEqual("option 1", comboBoxField.GetSelectedOption().GetExportValue());
        }

        [NUnit.Framework.Test]
        public virtual void SetSelectedByDisplayValueTest() {
            ComboBoxField comboBoxField = new ComboBoxField("test");
            comboBoxField.AddOption(new SelectFieldItem("option 1", "1"));
            comboBoxField.AddOption(new SelectFieldItem("option 2", "2"));
            comboBoxField.AddOption(new SelectFieldItem("option 3", "3"));
            comboBoxField.SetSelected("1");
            NUnit.Framework.Assert.IsNull(comboBoxField.GetSelectedOption());
        }

        [NUnit.Framework.Test]
        public virtual void SetSelectByDisplayValueTest() {
            ComboBoxField comboBoxField = new ComboBoxField("test");
            comboBoxField.AddOption(new SelectFieldItem("option 1", "1"));
            comboBoxField.AddOption(new SelectFieldItem("option 2", "2"));
            comboBoxField.AddOption(new SelectFieldItem("option 3", "3"));
            comboBoxField.SetSelected("option 1");
            NUnit.Framework.Assert.AreEqual("option 1", comboBoxField.GetSelectedOption().GetExportValue());
            NUnit.Framework.Assert.AreEqual("1", comboBoxField.GetSelectedOption().GetDisplayValue());
        }

        [NUnit.Framework.Test]
        public virtual void SetSelectedByIndexTest() {
            ComboBoxField comboBoxField = new ComboBoxField("test");
            comboBoxField.AddOption(new SelectFieldItem("option 1"));
            comboBoxField.AddOption(new SelectFieldItem("option 2"));
            comboBoxField.AddOption(new SelectFieldItem("option 3"));
            comboBoxField.SetSelected(1);
            NUnit.Framework.Assert.AreEqual("option 2", comboBoxField.GetSelectedOption().GetDisplayValue());
            NUnit.Framework.Assert.AreEqual("option 2", comboBoxField.GetSelectedOption().GetExportValue());
        }

        [NUnit.Framework.Test]
        public virtual void SetSelectedByIndexOutOfBoundsTest() {
            ComboBoxField comboBoxField = new ComboBoxField("test");
            comboBoxField.AddOption(new SelectFieldItem("option 1"));
            comboBoxField.AddOption(new SelectFieldItem("option 2"));
            comboBoxField.AddOption(new SelectFieldItem("option 3"));
            NUnit.Framework.Assert.Catch(typeof(IndexOutOfRangeException), () => comboBoxField.SetSelected(3));
        }

        [NUnit.Framework.Test]
        public virtual void SetSelectByIndexNegativeOutOfBoundsTest() {
            ComboBoxField comboBoxField = new ComboBoxField("test");
            comboBoxField.AddOption(new SelectFieldItem("option 1"));
            comboBoxField.AddOption(new SelectFieldItem("option 2"));
            comboBoxField.AddOption(new SelectFieldItem("option 3"));
            NUnit.Framework.Assert.Catch(typeof(IndexOutOfRangeException), () => comboBoxField.SetSelected(-1));
        }

        [NUnit.Framework.Test]
        public virtual void SetBySelectFieldItem() {
            ComboBoxField comboBoxField = new ComboBoxField("test");
            SelectFieldItem option1 = new SelectFieldItem("option 1", "1");
            comboBoxField.AddOption(option1);
            comboBoxField.AddOption(new SelectFieldItem("option 2", "2"));
            comboBoxField.AddOption(new SelectFieldItem("option 3", "3"));
            comboBoxField.SetSelected(option1);
            NUnit.Framework.Assert.AreEqual("option 1", comboBoxField.GetSelectedOption().GetExportValue());
            NUnit.Framework.Assert.AreEqual("1", comboBoxField.GetSelectedOption().GetDisplayValue());
        }

        [NUnit.Framework.Test]
        public virtual void SetBySelectFieldItemNullTest() {
            ComboBoxField comboBoxField = new ComboBoxField("test");
            comboBoxField.AddOption(new SelectFieldItem("option 1", "1"));
            comboBoxField.AddOption(new SelectFieldItem("option 2", "2"));
            comboBoxField.SetSelected((SelectFieldItem)null);
            NUnit.Framework.Assert.IsNull(comboBoxField.GetSelectedOption());
        }

        [NUnit.Framework.Test]
        public virtual void SetBySelectFieldItemNotInOptionsTest() {
            ComboBoxField comboBoxField = new ComboBoxField("test");
            comboBoxField.AddOption(new SelectFieldItem("option 1", "1"));
            comboBoxField.AddOption(new SelectFieldItem("option 2", "2"));
            comboBoxField.SetSelected(new SelectFieldItem("option 3", "3"));
            NUnit.Framework.Assert.IsNull(comboBoxField.GetSelectedOption());
        }

        [NUnit.Framework.Test]
        [LogMessage(FormsLogMessageConstants.DUPLICATE_EXPORT_VALUE, Count = 1)]
        public virtual void AddingOptionsWithSameExportValuesLogsWarningTest() {
            ComboBoxField comboBoxField = new ComboBoxField("test");
            comboBoxField.AddOption(new SelectFieldItem("option 1", "1"));
            comboBoxField.AddOption(new SelectFieldItem("option 1", "2"));
            NUnit.Framework.Assert.AreEqual(2, comboBoxField.GetItems().Count);
        }

        [NUnit.Framework.Test]
        public virtual void AddingWithDuplicateDisplayValueTest() {
            ComboBoxField comboBoxField = new ComboBoxField("test");
            comboBoxField.AddOption(new SelectFieldItem("option 1", "1"));
            comboBoxField.AddOption(new SelectFieldItem("option 2", "1"));
            NUnit.Framework.Assert.AreEqual(2, comboBoxField.GetItems().Count);
        }

        [NUnit.Framework.Test]
        public virtual void AddingOptionWithNullExportValueTest() {
            ComboBoxField comboBoxField = new ComboBoxField("test");
            NUnit.Framework.Assert.Catch(typeof(ArgumentException), () => comboBoxField.AddOption(new SelectFieldItem(
                "option 1", (String)null)));
        }
    }
}
