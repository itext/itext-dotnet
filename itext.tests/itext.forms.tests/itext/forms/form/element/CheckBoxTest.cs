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
using System.Collections.Generic;
using iText.Commons.Utils;
using iText.Forms.Fields.Properties;
using iText.Forms.Form.Renderer;
using iText.Forms.Form.Renderer.Checkboximpl;
using iText.Forms.Logs;
using iText.IO.Util;
using iText.Kernel.Colors;
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
    public class CheckBoxTest : ExtendedITextTest {
        public static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/forms/form/element/CheckBoxTest/";

        public static readonly String DESTINATION_FOLDER = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/forms/form/element/CheckBoxTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            ExperimentalFeatures.ENABLE_EXPERIMENTAL_CHECKBOX_RENDERING = true;
            CreateOrClearDestinationFolder(DESTINATION_FOLDER);
        }

        [NUnit.Framework.OneTimeTearDown]
        public static void AfterClass() {
            ExperimentalFeatures.ENABLE_EXPERIMENTAL_CHECKBOX_RENDERING = false;
        }

        [NUnit.Framework.Test]
        public virtual void RenderingModeDefaultValueTest() {
            CheckBox checkBoxPdf = new CheckBox("test");
            CheckBoxRenderer rendererPdf = (CheckBoxRenderer)checkBoxPdf.GetRenderer();
            NUnit.Framework.Assert.AreEqual(RenderingMode.DEFAULT_LAYOUT_MODE, rendererPdf.GetRenderingMode());
        }

        [NUnit.Framework.Test]
        [LogMessage(FormsLogMessageConstants.INVALID_VALUE_FALLBACK_TO_DEFAULT, Count = 2)]
        public virtual void SetRenderingModeNullKeepsOriginalValue() {
            CheckBox checkBoxPdf = new CheckBox("test");
            checkBoxPdf.SetRenderingMode(null);
            CheckBoxRenderer rendererPdf = (CheckBoxRenderer)checkBoxPdf.GetRenderer();
            NUnit.Framework.Assert.AreEqual(RenderingMode.DEFAULT_LAYOUT_MODE, rendererPdf.GetRenderingMode());
            CheckBox checkBoxHtml = new CheckBox("test");
            checkBoxHtml.SetRenderingMode(null);
            CheckBoxRenderer rendererHtml = (CheckBoxRenderer)checkBoxHtml.GetRenderer();
            NUnit.Framework.Assert.AreEqual(RenderingMode.DEFAULT_LAYOUT_MODE, rendererHtml.GetRenderingMode());
        }

        [NUnit.Framework.Test]
        public virtual void SetRenderingModeTest() {
            CheckBox checkBoxPdf = new CheckBox("test");
            checkBoxPdf.SetRenderingMode(RenderingMode.HTML_MODE);
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
            AbstractCheckBoxRendererFactory factory = renderer.CreateCheckBoxRenderFactory();
            NUnit.Framework.Assert.IsTrue(factory is CheckBoxPdfRendererFactory);
        }

        [NUnit.Framework.Test]
        public virtual void CreateCheckBoxFactoryPdfATest() {
            CheckBox checkBox = new CheckBox("test");
            checkBox.SetPdfAConformanceLevel(PdfAConformanceLevel.PDF_A_1B);
            CheckBoxRenderer renderer = (CheckBoxRenderer)checkBox.GetRenderer();
            AbstractCheckBoxRendererFactory factory = renderer.CreateCheckBoxRenderFactory();
            NUnit.Framework.Assert.IsTrue(factory is CheckBoxPdfARendererFactory);
        }

        [NUnit.Framework.Test]
        public virtual void CreateCheckBoxFactoryHtmlTest() {
            CheckBox checkBox = new CheckBox("test");
            checkBox.SetRenderingMode(RenderingMode.HTML_MODE);
            CheckBoxRenderer renderer = (CheckBoxRenderer)checkBox.GetRenderer();
            AbstractCheckBoxRendererFactory factory = renderer.CreateCheckBoxRenderFactory();
            NUnit.Framework.Assert.IsTrue(factory is CheckBoxHtmlRendererFactory);
        }

        [NUnit.Framework.Test]
        public virtual void CreateCheckBoxFactoryHtmlWithPdfATest() {
            CheckBox checkBox = new CheckBox("test");
            checkBox.SetRenderingMode(RenderingMode.HTML_MODE);
            checkBox.SetPdfAConformanceLevel(PdfAConformanceLevel.PDF_A_1B);
            CheckBoxRenderer renderer = (CheckBoxRenderer)checkBox.GetRenderer();
            AbstractCheckBoxRendererFactory factory = renderer.CreateCheckBoxRenderFactory();
            NUnit.Framework.Assert.IsTrue(factory is CheckBoxHtmlRendererFactory);
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
        public virtual void BasicCheckBoxDrawingTestHtmlMode() {
            String outPdf = DESTINATION_FOLDER + "basicCheckBoxHtml.pdf";
            String cmpPdf = SOURCE_FOLDER + "cmp_basicCheckBoxHtml.pdf";
            using (Document document = new Document(new PdfDocument(new PdfWriter(outPdf)))) {
                CheckBox checkBoxUnset = new CheckBox("test");
                checkBoxUnset.SetRenderingMode(RenderingMode.HTML_MODE);
                document.Add(checkBoxUnset);
                CheckBox checkBoxset = new CheckBox("test");
                checkBoxset.SetRenderingMode(RenderingMode.HTML_MODE);
                checkBoxset.SetChecked(true);
                document.Add(checkBoxset);
            }
            //TODO DEVSIX-7426 add interactive mode this will cause flaky layout fix and then fix tests in html2pdf
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, DESTINATION_FOLDER));
        }

        [NUnit.Framework.Test]
        public virtual void BasicCheckBoxDrawingTestPdfMode() {
            String outPdf = DESTINATION_FOLDER + "basicCheckBoxPdf.pdf";
            String cmpPdf = SOURCE_FOLDER + "cmp_basicCheckBoxPdf.pdf";
            using (Document document = new Document(new PdfDocument(new PdfWriter(outPdf)))) {
                //should be invisble because there is no default border
                CheckBox checkBoxUnset = new CheckBox("test");
                document.Add(checkBoxUnset);
                CheckBox checkBoxset = new CheckBox("test");
                checkBoxset.SetChecked(true);
                document.Add(checkBoxset);
                CheckBox checkBoxUnsetInteractive = new CheckBox("test1");
                checkBoxUnsetInteractive.SetInteractive(true);
                document.Add(checkBoxUnsetInteractive);
                CheckBox checkBoxsetInteractive = new CheckBox("test2");
                checkBoxsetInteractive.SetInteractive(true);
                checkBoxsetInteractive.SetChecked(true);
                document.Add(checkBoxsetInteractive);
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, DESTINATION_FOLDER));
        }

        [NUnit.Framework.Test]
        [LogMessage(LayoutLogMessageConstant.ELEMENT_DOES_NOT_FIT_AREA, Count = 7)]
        [LogMessage(FormsLogMessageConstants.INPUT_FIELD_DOES_NOT_FIT, Count = 7)]
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
            //TODO DEVSIX-7426 pdfa toggeling shows normal cross while opening in acrobat
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, DESTINATION_FOLDER));
        }

        [NUnit.Framework.Test]
        [LogMessage(LayoutLogMessageConstant.ELEMENT_DOES_NOT_FIT_AREA, Count = 28)]
        [LogMessage(FormsLogMessageConstants.INPUT_FIELD_DOES_NOT_FIT, Count = 28)]
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
                GenerateCheckBoxes(document, (checkBox) => {
                    checkBox.SetBorder(new SolidBorder(ColorConstants.GREEN, .5f));
                }
                );
                GenerateCheckBoxes(document, (checkBox) => {
                    checkBox.SetBorder(new SolidBorder(ColorConstants.YELLOW, 1));
                }
                );
                GenerateCheckBoxes(document, (checkBox) => {
                    checkBox.SetBorder(new SolidBorder(ColorConstants.BLUE, 3));
                }
                );
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, DESTINATION_FOLDER));
        }

        [NUnit.Framework.Test]
        [LogMessage(LayoutLogMessageConstant.ELEMENT_DOES_NOT_FIT_AREA, Count = 7)]
        [LogMessage(FormsLogMessageConstants.INPUT_FIELD_DOES_NOT_FIT, Count = 7)]
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
        [LogMessage(LayoutLogMessageConstant.ELEMENT_DOES_NOT_FIT_AREA, Count = 42)]
        [LogMessage(FormsLogMessageConstants.INPUT_FIELD_DOES_NOT_FIT, Count = 42)]
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
        [LogMessage(LayoutLogMessageConstant.ELEMENT_DOES_NOT_FIT_AREA, Count = 42)]
        [LogMessage(FormsLogMessageConstants.INPUT_FIELD_DOES_NOT_FIT, Count = 42)]
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

        public static void GenerateCheckBoxesForAllRenderingModes(Document document, Action<CheckBox> alterFunction
            ) {
            document.Add(new Paragraph("Normal rendering mode"));
            GenerateCheckBoxes(document, (checkBox) => {
                checkBox.SetRenderingMode(RenderingMode.DEFAULT_LAYOUT_MODE);
                alterFunction(checkBox);
            }
            );
            document.Add(new Paragraph("Pdfa rendering mode"));
            GenerateCheckBoxes(document, (checkBox) => {
                checkBox.SetRenderingMode(RenderingMode.DEFAULT_LAYOUT_MODE);
                checkBox.SetPdfAConformanceLevel(PdfAConformanceLevel.PDF_A_1B);
                alterFunction(checkBox);
            }
            );
            document.Add(new Paragraph("Html rendering mode"));
            GenerateCheckBoxes(document, (checkBox) => {
                checkBox.SetRenderingMode(RenderingMode.HTML_MODE);
                alterFunction(checkBox);
            }
            );
        }

        public static IList<CheckBox> GenerateCheckBoxes(Document document, Action<CheckBox> alterFunction) {
            IList<CheckBox> checkBoxList = new List<CheckBox>();
            CheckBox formCheckbox = new CheckBox("checkbox_interactive_off");
            formCheckbox.SetInteractive(true);
            checkBoxList.Add(formCheckbox);
            CheckBox flattenCheckbox = new CheckBox("checkbox_flatten_off");
            checkBoxList.Add(flattenCheckbox);
            CheckBox formCheckboxChecked = new CheckBox("checkbox_interactive_checked");
            formCheckboxChecked.SetInteractive(true);
            formCheckboxChecked.SetChecked(true);
            checkBoxList.Add(formCheckboxChecked);
            CheckBox flattenCheckboxChecked = new CheckBox("checkbox_flatten_checked");
            flattenCheckboxChecked.SetChecked(true);
            checkBoxList.Add(flattenCheckboxChecked);
            foreach (CheckBox checkBox in checkBoxList) {
                alterFunction(checkBox);
                document.Add(checkBox);
            }
            return checkBoxList;
        }
    }
}
