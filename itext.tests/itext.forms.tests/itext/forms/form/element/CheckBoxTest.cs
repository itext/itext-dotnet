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
using System.IO;
using iText.Forms;
using iText.Forms.Fields;
using iText.Forms.Fields.Properties;
using iText.Forms.Form.Renderer;
using iText.Forms.Form.Renderer.Checkboximpl;
using iText.Forms.Logs;
using iText.IO.Util;
using iText.Kernel.Colors;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using iText.Layout;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Layout.Renderer;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Forms.Form.Element {
    [NUnit.Framework.Category("IntegrationTest")]
    public class CheckBoxTest : ExtendedITextTest {
        public static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/forms/form/element/CheckBoxTest/";

        public static readonly String DESTINATION_FOLDER = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/forms/form/element/CheckBoxTest/";

        private int counter = 0;

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateOrClearDestinationFolder(DESTINATION_FOLDER);
        }

        [NUnit.Framework.SetUp]
        public virtual void Before() {
            counter = 0;
        }

        [NUnit.Framework.Test]
        public virtual void RenderingModeDefaultValueTest() {
            CheckBox checkBoxPdf = new CheckBox("test");
            CheckBoxRenderer rendererPdf = (CheckBoxRenderer)checkBoxPdf.GetRenderer();
            NUnit.Framework.Assert.AreEqual(RenderingMode.DEFAULT_LAYOUT_MODE, rendererPdf.GetRenderingMode());
        }

        [NUnit.Framework.Test]
        public virtual void SetRenderingModeReturnsToDefaultMode() {
            CheckBox checkBoxPdf = new CheckBox("test");
            checkBoxPdf.SetProperty(Property.RENDERING_MODE, null);
            CheckBoxRenderer rendererPdf = (CheckBoxRenderer)checkBoxPdf.GetRenderer();
            NUnit.Framework.Assert.AreEqual(RenderingMode.DEFAULT_LAYOUT_MODE, rendererPdf.GetRenderingMode());
            CheckBox checkBoxHtml = new CheckBox("test");
            checkBoxHtml.SetProperty(Property.RENDERING_MODE, RenderingMode.HTML_MODE);
            checkBoxHtml.SetProperty(Property.RENDERING_MODE, null);
            CheckBoxRenderer rendererHtml = (CheckBoxRenderer)checkBoxHtml.GetRenderer();
            NUnit.Framework.Assert.AreEqual(RenderingMode.DEFAULT_LAYOUT_MODE, rendererHtml.GetRenderingMode());
        }

        [NUnit.Framework.Test]
        public virtual void SetRenderingModeTest() {
            CheckBox checkBoxPdf = new CheckBox("test");
            checkBoxPdf.SetProperty(Property.RENDERING_MODE, RenderingMode.HTML_MODE);
            CheckBoxRenderer rendererPdf = (CheckBoxRenderer)checkBoxPdf.GetRenderer();
            NUnit.Framework.Assert.AreEqual(RenderingMode.HTML_MODE, rendererPdf.GetRenderingMode());
        }

        [NUnit.Framework.Test]
        public virtual void CheckBoxSetCheckedTest() {
            CheckBox checkBox = new CheckBox("test");
            CheckBoxRenderer renderer = (CheckBoxRenderer)checkBox.GetRenderer();
            NUnit.Framework.Assert.IsFalse(renderer.IsBoxChecked());
            checkBox.SetChecked(true);
            NUnit.Framework.Assert.IsTrue(renderer.IsBoxChecked());
            checkBox.SetChecked(false);
            NUnit.Framework.Assert.IsFalse(renderer.IsBoxChecked());
        }

        [NUnit.Framework.Test]
        public virtual void CreateCheckBoxFactoryDefaultPdfTest() {
            CheckBox checkBox = new CheckBox("test");
            CheckBoxRenderer renderer = (CheckBoxRenderer)checkBox.GetRenderer();
            ICheckBoxRenderingStrategy strategy = renderer.CreateCheckBoxRenderStrategy();
            NUnit.Framework.Assert.IsTrue(strategy is PdfCheckBoxRenderingStrategy);
        }

        [NUnit.Framework.Test]
        public virtual void CreateCheckBoxFactoryPdfATest() {
            CheckBox checkBox = new CheckBox("test");
            checkBox.SetPdfAConformanceLevel(PdfAConformanceLevel.PDF_A_1B);
            CheckBoxRenderer renderer = (CheckBoxRenderer)checkBox.GetRenderer();
            ICheckBoxRenderingStrategy strategy = renderer.CreateCheckBoxRenderStrategy();
            NUnit.Framework.Assert.IsTrue(strategy is PdfACheckBoxRenderingStrategy);
        }

        [NUnit.Framework.Test]
        public virtual void CreateCheckBoxFactoryHtmlTest() {
            CheckBox checkBox = new CheckBox("test");
            checkBox.SetProperty(Property.RENDERING_MODE, RenderingMode.HTML_MODE);
            CheckBoxRenderer renderer = (CheckBoxRenderer)checkBox.GetRenderer();
            ICheckBoxRenderingStrategy strategy = renderer.CreateCheckBoxRenderStrategy();
            NUnit.Framework.Assert.IsTrue(strategy is HtmlCheckBoxRenderingStrategy);
        }

        [NUnit.Framework.Test]
        public virtual void CreateCheckBoxFactoryHtmlWithPdfATest() {
            CheckBox checkBox = new CheckBox("test");
            checkBox.SetProperty(Property.RENDERING_MODE, RenderingMode.HTML_MODE);
            checkBox.SetPdfAConformanceLevel(PdfAConformanceLevel.PDF_A_1B);
            CheckBoxRenderer renderer = (CheckBoxRenderer)checkBox.GetRenderer();
            ICheckBoxRenderingStrategy strategy = renderer.CreateCheckBoxRenderStrategy();
            NUnit.Framework.Assert.IsTrue(strategy is HtmlCheckBoxRenderingStrategy);
        }

        [NUnit.Framework.Test]
        public virtual void IsPdfATest() {
            CheckBox checkBox = new CheckBox("test");
            CheckBoxRenderer rendererPdf2 = (CheckBoxRenderer)checkBox.GetRenderer();
            NUnit.Framework.Assert.IsFalse(rendererPdf2.IsPdfA());
            checkBox.SetPdfAConformanceLevel(PdfAConformanceLevel.PDF_A_1B);
            CheckBoxRenderer rendererPdf = (CheckBoxRenderer)checkBox.GetRenderer();
            NUnit.Framework.Assert.IsTrue(rendererPdf.IsPdfA());
            checkBox.SetPdfAConformanceLevel(null);
            CheckBoxRenderer rendererPdf1 = (CheckBoxRenderer)checkBox.GetRenderer();
            NUnit.Framework.Assert.IsFalse(rendererPdf1.IsPdfA());
        }

        [NUnit.Framework.Test]
        public virtual void CheckBoxInHtmlModeKeeps1on1RatioAndTakesMaxValue() {
            CheckBox checkBox = new CheckBox("test");
            checkBox.SetProperty(Property.RENDERING_MODE, RenderingMode.HTML_MODE);
            int height = 100;
            int width = 50;
            checkBox.SetProperty(Property.HEIGHT, UnitValue.CreatePointValue(height));
            checkBox.SetProperty(Property.WIDTH, UnitValue.CreatePointValue(width));
            ParagraphRenderer renderer = (ParagraphRenderer)((CheckBoxRenderer)checkBox.GetRenderer()).CreateFlatRenderer
                ();
            UnitValue heightUnitValue = renderer.GetPropertyAsUnitValue(Property.HEIGHT);
            UnitValue widthUnitValue = renderer.GetPropertyAsUnitValue(Property.WIDTH);
            NUnit.Framework.Assert.AreEqual(height, heightUnitValue.GetValue(), 0);
            NUnit.Framework.Assert.AreEqual(height, widthUnitValue.GetValue(), 0);
        }

        [NUnit.Framework.Test]
        public virtual void BasicCheckBoxDrawingTestHtmlMode() {
            String outPdf = DESTINATION_FOLDER + "basicCheckBoxHtml.pdf";
            String cmpPdf = SOURCE_FOLDER + "cmp_basicCheckBoxHtml.pdf";
            using (Document document = new Document(new PdfDocument(new PdfWriter(outPdf)))) {
                CheckBox checkBoxUnset = new CheckBox("test");
                checkBoxUnset.SetBorder(new SolidBorder(ColorConstants.RED, 1));
                checkBoxUnset.SetProperty(Property.RENDERING_MODE, RenderingMode.HTML_MODE);
                document.Add(checkBoxUnset);
                CheckBox checkBox = new CheckBox("test1");
                checkBox.SetInteractive(true);
                checkBox.SetBorder(new SolidBorder(ColorConstants.RED, 1));
                checkBox.SetProperty(Property.RENDERING_MODE, RenderingMode.HTML_MODE);
                document.Add(checkBox);
                CheckBox checkBoxset = new CheckBox("test2");
                checkBoxset.SetProperty(Property.RENDERING_MODE, RenderingMode.HTML_MODE);
                checkBoxset.SetBorder(new SolidBorder(ColorConstants.RED, 1));
                checkBoxset.SetChecked(true);
                document.Add(checkBoxset);
                CheckBox checkBoxsetAndInteractive = new CheckBox("test3");
                checkBoxsetAndInteractive.SetProperty(Property.RENDERING_MODE, RenderingMode.HTML_MODE);
                checkBoxsetAndInteractive.SetBorder(new SolidBorder(ColorConstants.RED, 1));
                checkBoxsetAndInteractive.SetChecked(true);
                checkBoxsetAndInteractive.SetInteractive(true);
                document.Add(checkBoxsetAndInteractive);
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, DESTINATION_FOLDER));
        }

        [NUnit.Framework.Test]
        public virtual void BasicCheckBoxDrawingTestPdfMode() {
            String outPdf = DESTINATION_FOLDER + "basicCheckBoxPdf.pdf";
            String cmpPdf = SOURCE_FOLDER + "cmp_basicCheckBoxPdf.pdf";
            using (Document document = new Document(new PdfDocument(new PdfWriter(outPdf)))) {
                //should be invisble because there is no default border
                CheckBox checkBoxUnset = new CheckBox("test");
                checkBoxUnset.SetBorder(new SolidBorder(ColorConstants.RED, 1));
                document.Add(checkBoxUnset);
                CheckBox checkBoxset = new CheckBox("test0");
                checkBoxset.SetChecked(true);
                checkBoxset.SetBorder(new SolidBorder(ColorConstants.RED, 1));
                document.Add(checkBoxset);
                CheckBox checkBoxUnsetInteractive = new CheckBox("test1");
                checkBoxUnsetInteractive.SetInteractive(true);
                checkBoxUnsetInteractive.SetBorder(new SolidBorder(ColorConstants.RED, 1));
                document.Add(checkBoxUnsetInteractive);
                CheckBox checkBoxsetInteractive = new CheckBox("test2");
                checkBoxsetInteractive.SetInteractive(true);
                checkBoxsetInteractive.SetChecked(true);
                checkBoxsetInteractive.SetBorder(new SolidBorder(ColorConstants.RED, 1));
                document.Add(checkBoxsetInteractive);
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, DESTINATION_FOLDER));
        }

        [NUnit.Framework.Test]
        public virtual void BasicCheckBoxDrawingTestPdfAMode() {
            String outPdf = DESTINATION_FOLDER + "basicCheckBoxPdfA.pdf";
            String cmpPdf = SOURCE_FOLDER + "cmp_basicCheckBoxPdfA.pdf";
            using (Document document = new Document(new PdfDocument(new PdfWriter(outPdf)))) {
                //should be invisble because there is no default border
                CheckBox checkBoxUnset = new CheckBox("test").SetPdfAConformanceLevel(PdfAConformanceLevel.PDF_A_1B);
                document.Add(checkBoxUnset);
                CheckBox checkBoxset = new CheckBox("test").SetPdfAConformanceLevel(PdfAConformanceLevel.PDF_A_1B);
                checkBoxset.SetChecked(true);
                document.Add(checkBoxset);
                CheckBox checkBoxUnsetInteractive = new CheckBox("test1").SetPdfAConformanceLevel(PdfAConformanceLevel.PDF_A_1B
                    );
                checkBoxUnsetInteractive.SetInteractive(true);
                document.Add(checkBoxUnsetInteractive);
                CheckBox checkBoxsetInteractive = new CheckBox("test2").SetPdfAConformanceLevel(PdfAConformanceLevel.PDF_A_1B
                    );
                checkBoxsetInteractive.SetInteractive(true);
                checkBoxsetInteractive.SetChecked(true);
                document.Add(checkBoxsetInteractive);
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, DESTINATION_FOLDER));
        }

        [NUnit.Framework.Test]
        public virtual void BasicCheckBoxSetSize() {
            String outPdf = DESTINATION_FOLDER + "checkBoxSetSize.pdf";
            String cmpPdf = SOURCE_FOLDER + "cmp_checkBoxSetSize.pdf";
            int scaleFactor = 5;
            using (Document document = new Document(new PdfDocument(new PdfWriter(outPdf)))) {
                for (int i = 1; i < 5; i++) {
                    int size = i * i * scaleFactor;
                    GenerateCheckBoxesForAllRenderingModes(document, (checkBox) => {
                        checkBox.SetSize(size);
                    }
                    );
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, DESTINATION_FOLDER));
        }

        [NUnit.Framework.Test]
        [LogMessage(FormsLogMessageConstants.INVALID_VALUE_FALLBACK_TO_DEFAULT, Count = 8)]
        public virtual void BasicCheckBoxSetSizeNegativeValueFallsBackToDefaultValue() {
            String outPdf = DESTINATION_FOLDER + "checkBoxSetSizeBadSize.pdf";
            String cmpPdf = SOURCE_FOLDER + "cmp_checkBoxSetSizeBadSize.pdf";
            using (Document document = new Document(new PdfDocument(new PdfWriter(outPdf)))) {
                GenerateCheckBoxes(document, (checkBox) => {
                    checkBox.SetSize(0);
                }
                );
                GenerateCheckBoxes(document, (checkBox) => {
                    checkBox.SetSize(-1);
                }
                );
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, DESTINATION_FOLDER));
        }

        [NUnit.Framework.Test]
        public virtual void BasicCheckBoxSetBorderTest() {
            String outPdf = DESTINATION_FOLDER + "checkBox_setBorder.pdf";
            String cmpPdf = SOURCE_FOLDER + "cmp_setBorder.pdf";
            using (Document document = new Document(new PdfDocument(new PdfWriter(outPdf)))) {
                GenerateCheckBoxesForAllRenderingModes(document, (checkBox) => {
                    checkBox.SetBorder(new SolidBorder(ColorConstants.GREEN, .5f));
                }
                );
                GenerateCheckBoxesForAllRenderingModes(document, (checkBox) => {
                    checkBox.SetBorder(new SolidBorder(ColorConstants.YELLOW, 1));
                }
                );
                GenerateCheckBoxesForAllRenderingModes(document, (checkBox) => {
                    checkBox.SetBorder(new SolidBorder(ColorConstants.BLUE, 3));
                }
                );
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, DESTINATION_FOLDER));
        }

        [NUnit.Framework.Test]
        public virtual void BasicCheckBoxSetBackgroundTest() {
            String outPdf = DESTINATION_FOLDER + "checkBox_setBackground.pdf";
            String cmpPdf = SOURCE_FOLDER + "cmp_checkBox_setBackground.pdf";
            using (Document document = new Document(new PdfDocument(new PdfWriter(outPdf)))) {
                GenerateCheckBoxesForAllRenderingModes(document, (checkBox) => {
                    checkBox.SetBackgroundColor(ColorConstants.MAGENTA);
                }
                );
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, DESTINATION_FOLDER));
        }

        [NUnit.Framework.Test]
        public virtual void SetBorderTestSmallDefaultsToMinValue() {
            String outPdf = DESTINATION_FOLDER + "checkBox_setSmallBorder.pdf";
            String cmpPdf = SOURCE_FOLDER + "cmp_checkBox_setSmallBorder.pdf";
            using (Document document = new Document(new PdfDocument(new PdfWriter(outPdf)))) {
                float?[] i = new float?[] { .1f };
                document.Add(new Paragraph("Non interactive"));
                for (int j = 0; j < 30; j++) {
                    i[0] += .05f;
                    CheckBox checkBox = new CheckBox("test" + j);
                    checkBox.SetBorder(new SolidBorder(ColorConstants.GREEN, (float)i[0]));
                    document.Add(checkBox);
                }
                float?[] k = new float?[] { .1f };
                document.Add(new Paragraph("Interactive"));
                for (int j = 0; j < 30; j++) {
                    k[0] += .05f;
                    CheckBox checkBox = new CheckBox("test" + j);
                    checkBox.SetInteractive(true);
                    checkBox.SetBorder(new SolidBorder(ColorConstants.RED, (float)k[0]));
                    document.Add(checkBox);
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, DESTINATION_FOLDER));
        }

        [NUnit.Framework.Test]
        public virtual void CheckBoxSetCheckTypes() {
            String outPdf = DESTINATION_FOLDER + "checkBox_setCheckType.pdf";
            String cmpPdf = SOURCE_FOLDER + "cmp_checkBox_setCheckType.pdf";
            using (Document document = new Document(new PdfDocument(new PdfWriter(outPdf)))) {
                foreach (CheckBoxType enumConstant in EnumUtil.GetAllValuesOfEnum<CheckBoxType>()) {
                    GenerateCheckBoxes(document, (checkBox) => {
                        checkBox.SetCheckBoxType(enumConstant);
                    }
                    );
                    GenerateCheckBoxes(document, (checkBox) => {
                        checkBox.SetPdfAConformanceLevel(PdfAConformanceLevel.PDF_A_1B);
                        checkBox.SetCheckBoxType(enumConstant);
                    }
                    );
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, DESTINATION_FOLDER));
        }

        [NUnit.Framework.Test]
        public virtual void SetPdfAConformanceLevel() {
            String outPdf = DESTINATION_FOLDER + "checkBox_setConformanceLevel.pdf";
            String cmpPdf = SOURCE_FOLDER + "cmp_checkBox_setConformanceLevel.pdf";
            using (Document document = new Document(new PdfDocument(new PdfWriter(outPdf)))) {
                foreach (CheckBoxType enumConstant in EnumUtil.GetAllValuesOfEnum<CheckBoxType>()) {
                    GenerateCheckBoxes(document, (checkBox) => {
                        checkBox.SetSize(20);
                        checkBox.SetPdfAConformanceLevel(PdfAConformanceLevel.PDF_A_3B);
                        checkBox.SetCheckBoxType(enumConstant);
                    }
                    );
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, DESTINATION_FOLDER));
        }

        [NUnit.Framework.Test]
        public virtual void RemovingFormFieldsLeavesNoVisualTrace() {
            String outPdf = DESTINATION_FOLDER + "checkBox_removeFormField.pdf";
            String cmpPdf = SOURCE_FOLDER + "cmp_checkBox_removeFormField.pdf";
            MemoryStream baos = new MemoryStream();
            using (Document document = new Document(new PdfDocument(new PdfWriter(baos)))) {
                GenerateCheckBoxesForAllRenderingModes(document, (checkBox) => {
                    checkBox.SetBorder(new SolidBorder(ColorConstants.GREEN, 1f));
                    checkBox.SetBackgroundColor(ColorConstants.CYAN);
                }
                );
            }
            MemoryStream bais = new MemoryStream(baos.ToArray());
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(bais), new PdfWriter(outPdf));
            PdfAcroForm acroForm = PdfFormCreator.GetAcroForm(pdfDocument, true);
            foreach (KeyValuePair<String, PdfFormField> entry in acroForm.GetAllFormFields()) {
                String key = entry.Key;
                acroForm.RemoveField(key);
            }
            pdfDocument.Close();
            // all non flattend acroform fields should be removed
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, DESTINATION_FOLDER));
        }

        [NUnit.Framework.Test]
        public virtual void CheckBoxWithMarginsTest() {
            String outPdf = DESTINATION_FOLDER + "checkBoxWithMargins.pdf";
            String cmpPdf = SOURCE_FOLDER + "cmp_checkBoxWithMargins.pdf";
            using (Document document = new Document(new PdfDocument(new PdfWriter(outPdf)))) {
                Div div = new Div().SetBackgroundColor(ColorConstants.PINK);
                CheckBox checkBox = new CheckBox("check");
                checkBox.SetInteractive(true);
                checkBox.SetProperty(Property.MARGIN_BOTTOM, UnitValue.CreatePointValue(20));
                checkBox.SetProperty(Property.MARGIN_TOP, UnitValue.CreatePointValue(20));
                checkBox.SetProperty(Property.MARGIN_LEFT, UnitValue.CreatePointValue(20));
                checkBox.SetProperty(Property.MARGIN_RIGHT, UnitValue.CreatePointValue(20));
                checkBox.SetBorder(new SolidBorder(ColorConstants.DARK_GRAY, 20)).SetBackgroundColor(ColorConstants.LIGHT_GRAY
                    ).SetSize(100).SetChecked(true);
                div.Add(checkBox);
                document.Add(div);
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, DESTINATION_FOLDER));
        }

        [NUnit.Framework.Test]
        public virtual void BorderBoxesTest() {
            String outPdf = DESTINATION_FOLDER + "borderBoxes.pdf";
            String cmpPdf = SOURCE_FOLDER + "cmp_borderBoxes.pdf";
            using (Document document = new Document(new PdfDocument(new PdfWriter(outPdf)))) {
                // BORDER_BOX
                CheckBox interactiveCheckBox1 = new CheckBox("checkBox1").SetBorder(new SolidBorder(ColorConstants.PINK, 10
                    )).SetSize(50).SetChecked(false);
                interactiveCheckBox1.SetInteractive(true);
                interactiveCheckBox1.SetProperty(Property.BOX_SIZING, BoxSizingPropertyValue.BORDER_BOX);
                document.Add(interactiveCheckBox1);
                // CONTENT_BOX
                CheckBox interactiveCheckBox2 = new CheckBox("checkBox2").SetBorder(new SolidBorder(ColorConstants.YELLOW, 
                    10)).SetSize(50).SetChecked(true);
                interactiveCheckBox2.SetInteractive(true);
                interactiveCheckBox2.SetProperty(Property.BOX_SIZING, BoxSizingPropertyValue.CONTENT_BOX);
                document.Add(interactiveCheckBox2);
                // BORDER_BOX
                CheckBox flattenCheckBox1 = new CheckBox("checkBox3").SetBorder(new SolidBorder(ColorConstants.PINK, 10)).
                    SetSize(50).SetChecked(true);
                flattenCheckBox1.SetInteractive(false);
                flattenCheckBox1.SetProperty(Property.BOX_SIZING, BoxSizingPropertyValue.BORDER_BOX);
                document.Add(flattenCheckBox1);
                // CONTENT_BOX
                CheckBox flattenCheckBox2 = new CheckBox("checkBox4").SetBorder(new SolidBorder(ColorConstants.YELLOW, 10)
                    ).SetSize(50).SetChecked(false);
                flattenCheckBox2.SetInteractive(false);
                flattenCheckBox2.SetProperty(Property.BOX_SIZING, BoxSizingPropertyValue.CONTENT_BOX);
                document.Add(flattenCheckBox2);
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, DESTINATION_FOLDER));
        }

        [NUnit.Framework.Test]
        public virtual void AddFieldWithTwoWidgetsTest() {
            String outPdf = DESTINATION_FOLDER + "fieldWithTwoWidgets.pdf";
            String cmpPdf = SOURCE_FOLDER + "cmp_fieldWithTwoWidgets.pdf";
            using (Document document = new Document(new PdfDocument(new PdfWriter(outPdf)))) {
                // Create checkboxes using html element.
                CheckBox checkBox1 = new CheckBox("checkbox");
                checkBox1.SetInteractive(true);
                checkBox1.SetSize(100);
                checkBox1.SetBackgroundColor(ColorConstants.YELLOW);
                checkBox1.SetBorder(new SolidBorder(ColorConstants.PINK, 5));
                checkBox1.SetChecked(true);
                document.Add(checkBox1);
                // Add break to the end of the page.
                document.Add(new AreaBreak());
                // Note that fields with the same fully qualified field name shall have the same
                // field type, value, and default value.
                CheckBox checkBox2 = new CheckBox("checkbox");
                checkBox2.SetInteractive(true);
                checkBox2.SetSize(200);
                checkBox2.SetBackgroundColor(ColorConstants.PINK);
                checkBox2.SetBorder(new SolidBorder(ColorConstants.YELLOW, 10));
                checkBox2.SetChecked(true);
                document.Add(checkBox2);
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, DESTINATION_FOLDER));
        }

        [NUnit.Framework.Test]
        public virtual void MultiPageCheckboxFieldTest() {
            String outPdf = DESTINATION_FOLDER + "multiPageCheckboxField.pdf";
            String cmpPdf = SOURCE_FOLDER + "cmp_multiPageCheckBoxField.pdf";
            using (PdfDocument document = new PdfDocument(new PdfWriter(outPdf))) {
                PdfAcroForm form = PdfAcroForm.GetAcroForm(document, true);
                for (int i = 0; i < 10; i++) {
                    document.AddNewPage();
                    Rectangle rect = new Rectangle(210, 490, 150, 22);
                    PdfFormField inputField = new CheckBoxFormFieldBuilder(document, "fing").SetWidgetRectangle(rect).CreateCheckBox
                        ();
                    inputField.SetValue("1");
                    PdfPage page = document.GetPage(i + 1);
                    form.AddField(inputField, page);
                    if (i > 2) {
                        page.Flush();
                    }
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, DESTINATION_FOLDER));
        }

        private void GenerateCheckBoxesForAllRenderingModes(Document document, Action<CheckBox> alterFunction) {
            document.Add(new Paragraph("Normal rendering mode"));
            GenerateCheckBoxes(document, (checkBox) => {
                checkBox.SetProperty(Property.RENDERING_MODE, RenderingMode.DEFAULT_LAYOUT_MODE);
                alterFunction(checkBox);
            }
            );
            document.Add(new Paragraph("Pdfa rendering mode"));
            GenerateCheckBoxes(document, (checkBox) => {
                checkBox.SetProperty(Property.RENDERING_MODE, RenderingMode.DEFAULT_LAYOUT_MODE);
                checkBox.SetPdfAConformanceLevel(PdfAConformanceLevel.PDF_A_1B);
                alterFunction(checkBox);
            }
            );
            document.Add(new Paragraph("Html rendering mode"));
            GenerateCheckBoxes(document, (checkBox) => {
                checkBox.SetProperty(Property.RENDERING_MODE, RenderingMode.HTML_MODE);
                alterFunction(checkBox);
            }
            );
        }

        private IList<CheckBox> GenerateCheckBoxes(Document document, Action<CheckBox> alterFunction) {
            IList<CheckBox> checkBoxList = new List<CheckBox>();
            CheckBox formCheckbox = new CheckBox("checkbox_interactive_off_" + this.counter);
            formCheckbox.SetInteractive(true);
            checkBoxList.Add(formCheckbox);
            this.counter++;
            CheckBox flattenCheckbox = new CheckBox("checkbox_flatten_off_" + this.counter);
            checkBoxList.Add(flattenCheckbox);
            this.counter++;
            CheckBox formCheckboxChecked = new CheckBox("checkbox_interactive_checked_" + this.counter);
            formCheckboxChecked.SetInteractive(true);
            formCheckboxChecked.SetChecked(true);
            checkBoxList.Add(formCheckboxChecked);
            this.counter++;
            CheckBox flattenCheckboxChecked = new CheckBox("checkbox_flatten_checked_" + this.counter);
            flattenCheckboxChecked.SetChecked(true);
            checkBoxList.Add(flattenCheckboxChecked);
            this.counter++;
            foreach (CheckBox checkBox in checkBoxList) {
                alterFunction(checkBox);
                document.Add(checkBox);
            }
            return checkBoxList;
        }
    }
}
