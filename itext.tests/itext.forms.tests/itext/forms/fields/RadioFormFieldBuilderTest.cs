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
using System.Collections.Generic;
using System.IO;
using iText.Forms;
using iText.Kernel.Exceptions;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Annot;
using iText.Kernel.Utils;
using iText.Test;

namespace iText.Forms.Fields {
    [NUnit.Framework.Category("UnitTest")]
    public class RadioFormFieldBuilderTest : ExtendedITextTest {
        private const String DUMMY_NAME = "dummy name";

        private static readonly Rectangle DUMMY_RECTANGLE = new Rectangle(7, 11, 13, 17);

        private const String DUMMY_APPEARANCE_NAME = "dummy appearance name";

        private const String DUMMY_APPEARANCE_NAME2 = "dummy appearance name 2";

        [NUnit.Framework.Test]
        public virtual void TwoParametersConstructorTest() {
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(new MemoryStream()));
            RadioFormFieldBuilder builder = new RadioFormFieldBuilder(pdfDoc, DUMMY_NAME);
            NUnit.Framework.Assert.AreSame(pdfDoc, builder.GetDocument());
            NUnit.Framework.Assert.AreSame(DUMMY_NAME, builder.GetFormFieldName());
        }

        [NUnit.Framework.Test]
        public virtual void CreateRadioGroupTest() {
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(new MemoryStream()));
            RadioFormFieldBuilder builder = new RadioFormFieldBuilder(pdfDoc, DUMMY_NAME);
            PdfButtonFormField radioGroup = builder.CreateRadioGroup();
            CompareRadioGroups(radioGroup, pdfDoc);
        }

        [NUnit.Framework.Test]
        public virtual void CreateRadioButtonWithWidgetTest() {
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(new MemoryStream()));
            RadioFormFieldBuilder builder = new RadioFormFieldBuilder(pdfDoc, DUMMY_NAME);
            PdfButtonFormField radioGroup = builder.CreateRadioGroup();
            PdfFormAnnotation radioAnnotation = builder.CreateRadioButton(DUMMY_APPEARANCE_NAME, DUMMY_RECTANGLE);
            CompareRadioButtons(radioAnnotation, radioGroup, pdfDoc, false);
        }

        [NUnit.Framework.Test]
        public virtual void CreateRadioButtonWithIncorrectNameTest() {
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(new MemoryStream()));
            NUnit.Framework.Assert.DoesNotThrow(() => new RadioFormFieldBuilder(pdfDoc, "incorrect.name").SetWidgetRectangle
                (DUMMY_RECTANGLE).CreateRadioGroup());
        }

        [NUnit.Framework.Test]
        public virtual void CreateRadioButtonWithWidgetUseSetWidgetRectangleTest() {
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(new MemoryStream()));
            RadioFormFieldBuilder builder = new RadioFormFieldBuilder(pdfDoc, DUMMY_NAME);
            PdfButtonFormField radioGroup = builder.CreateRadioGroup();
            PdfFormAnnotation radioAnnotation = builder.SetWidgetRectangle(DUMMY_RECTANGLE).CreateRadioButton(DUMMY_APPEARANCE_NAME
                , null);
            CompareRadioButtons(radioAnnotation, radioGroup, pdfDoc, false);
        }

        [NUnit.Framework.Test]
        public virtual void CreateRadioButtonWithEmptyAppearanceNameThrowsExceptionTest() {
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(new MemoryStream()));
            RadioFormFieldBuilder builder = new RadioFormFieldBuilder(pdfDoc, DUMMY_NAME);
            NUnit.Framework.Assert.Catch(typeof(PdfException), () => {
                builder.CreateRadioButton(null, DUMMY_RECTANGLE);
            }
            );
            NUnit.Framework.Assert.Catch(typeof(PdfException), () => {
                builder.CreateRadioButton("", DUMMY_RECTANGLE);
            }
            );
        }

        [NUnit.Framework.Test]
        public virtual void CreateRadioButtonWithWidgetAddedToRadioGroupTest() {
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(new MemoryStream()));
            RadioFormFieldBuilder builder = new RadioFormFieldBuilder(pdfDoc, DUMMY_NAME);
            PdfButtonFormField radioGroup = builder.CreateRadioGroup();
            PdfFormAnnotation radioAnnotation = builder.CreateRadioButton(DUMMY_APPEARANCE_NAME, DUMMY_RECTANGLE);
            radioGroup.AddKid(radioAnnotation);
            CompareRadioButtons(radioAnnotation, radioGroup, pdfDoc, true);
        }

        [NUnit.Framework.Test]
        public virtual void Create2RadioButtonWithWidgetAddedToRadioGroupTest() {
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(new MemoryStream()));
            RadioFormFieldBuilder builder = new RadioFormFieldBuilder(pdfDoc, DUMMY_NAME);
            PdfButtonFormField radioGroup = builder.CreateRadioGroup();
            PdfFormAnnotation radioAnnotation = builder.CreateRadioButton(DUMMY_APPEARANCE_NAME, DUMMY_RECTANGLE);
            PdfFormAnnotation radioAnnotation2 = builder.CreateRadioButton(DUMMY_APPEARANCE_NAME, DUMMY_RECTANGLE);
            radioGroup.AddKid(radioAnnotation);
            radioGroup.AddKid(radioAnnotation2);
            CompareRadioButtons(radioAnnotation, radioGroup, pdfDoc, true);
            CompareRadioButtons(radioAnnotation2, radioGroup, pdfDoc, true);
            NUnit.Framework.Assert.AreEqual(2, radioGroup.GetWidgets().Count);
        }

        [NUnit.Framework.Test]
        public virtual void Create2RadioButtonWithWidgetAddedToRadioGroupOneSelectedTest() {
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(new MemoryStream()));
            RadioFormFieldBuilder builder = new RadioFormFieldBuilder(pdfDoc, DUMMY_NAME);
            PdfButtonFormField radioGroup = builder.CreateRadioGroup();
            PdfFormAnnotation radioAnnotation = builder.CreateRadioButton(DUMMY_APPEARANCE_NAME, DUMMY_RECTANGLE);
            PdfFormAnnotation radioAnnotation2 = builder.CreateRadioButton(DUMMY_APPEARANCE_NAME2, DUMMY_RECTANGLE);
            radioGroup.SetValue(DUMMY_APPEARANCE_NAME);
            radioGroup.AddKid(radioAnnotation);
            radioGroup.AddKid(radioAnnotation2);
            NUnit.Framework.Assert.AreEqual(PdfFormAnnotation.OFF_STATE_VALUE, radioAnnotation2.GetAppearanceStates()[
                0]);
            NUnit.Framework.Assert.AreEqual(DUMMY_APPEARANCE_NAME, radioAnnotation.GetPdfObject().GetAsName(PdfName.AS
                ).GetValue());
            CompareRadioButtons(radioAnnotation, radioGroup, pdfDoc, true);
            CompareRadioButtons(radioAnnotation2, radioGroup, pdfDoc, true);
            NUnit.Framework.Assert.AreEqual(2, radioGroup.GetWidgets().Count);
        }

        [NUnit.Framework.Test]
        public virtual void CreateRadioButtonWithoutWidgetTest() {
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(new MemoryStream()));
            RadioFormFieldBuilder builder = new RadioFormFieldBuilder(pdfDoc, DUMMY_NAME);
            PdfButtonFormField radioGroup = builder.CreateRadioGroup();
            PdfFormAnnotation radioAnnotation = builder.CreateRadioButton(DUMMY_APPEARANCE_NAME, DUMMY_RECTANGLE);
            CompareRadioButtons(radioAnnotation, radioGroup, pdfDoc, false);
        }

        [NUnit.Framework.Test]
        public virtual void CreateRadioButtonWithoutWidgetThrowsExceptionTest() {
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(new MemoryStream()));
            RadioFormFieldBuilder builder = new RadioFormFieldBuilder(pdfDoc, DUMMY_NAME);
            NUnit.Framework.Assert.Catch(typeof(PdfException), () => {
                PdfFormAnnotation radioAnnotation = builder.CreateRadioButton(DUMMY_APPEARANCE_NAME, null);
            }
            );
        }

        [NUnit.Framework.Test]
        public virtual void CreateRadioButtonWithConformanceLevelTest() {
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(new MemoryStream()));
            RadioFormFieldBuilder builder = new RadioFormFieldBuilder(pdfDoc, DUMMY_NAME);
            PdfButtonFormField radioGroup = builder.CreateRadioGroup();
            PdfFormAnnotation radioAnnotation = builder.SetConformance(PdfConformance.PDF_A_1A).CreateRadioButton(DUMMY_APPEARANCE_NAME
                , DUMMY_RECTANGLE);
            CompareRadioButtons(radioAnnotation, radioGroup, pdfDoc, false);
        }

        [NUnit.Framework.Test]
        public virtual void CreateRadioButtonWithConformanceLevelAddedToGroupTest() {
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(new MemoryStream()));
            RadioFormFieldBuilder builder = new RadioFormFieldBuilder(pdfDoc, DUMMY_NAME);
            PdfButtonFormField radioGroup = builder.CreateRadioGroup();
            PdfFormAnnotation radioAnnotation = builder.SetConformance(PdfConformance.PDF_A_1A).CreateRadioButton(DUMMY_APPEARANCE_NAME
                , DUMMY_RECTANGLE);
            radioGroup.AddKid(radioAnnotation);
            CompareRadioButtons(radioAnnotation, radioGroup, pdfDoc, true);
        }

        private static void CompareRadioGroups(PdfButtonFormField radioGroupFormField, PdfDocument pdfDoc) {
            PdfDictionary expectedDictionary = new PdfDictionary();
            PutIfAbsent(expectedDictionary, PdfName.FT, PdfName.Btn);
            PutIfAbsent(expectedDictionary, PdfName.Ff, new PdfNumber(PdfButtonFormField.FF_RADIO));
            PutIfAbsent(expectedDictionary, PdfName.T, new PdfString(DUMMY_NAME));
            expectedDictionary.MakeIndirect(pdfDoc);
            radioGroupFormField.MakeIndirect(pdfDoc);
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareDictionariesStructure(expectedDictionary, radioGroupFormField
                .GetPdfObject()));
        }

        [NUnit.Framework.Test]
        public virtual void CreateRadioButtonShouldNotContainTerminalFieldKeys() {
            using (PdfDocument document = new PdfDocument(new PdfWriter(new MemoryStream()))) {
                PdfAcroForm form = PdfFormCreator.GetAcroForm(document, true);
                PdfButtonFormField radioGroup = new RadioFormFieldBuilder(document, DUMMY_NAME).CreateRadioGroup();
                PdfFormAnnotation radioAnnotation = new RadioFormFieldBuilder(document, DUMMY_NAME).CreateRadioButton(DUMMY_APPEARANCE_NAME
                    , DUMMY_RECTANGLE);
                form.AddField(radioGroup);
                NUnit.Framework.Assert.IsTrue(PdfFormAnnotationUtil.IsPureWidget(radioAnnotation.GetPdfObject()));
            }
        }

        [NUnit.Framework.Test]
        public virtual void CreateRadioButtonButDontAddToGroupGroupContainsNoRadioButton() {
            using (PdfDocument document = new PdfDocument(new PdfWriter(new MemoryStream()))) {
                PdfAcroForm form = PdfFormCreator.GetAcroForm(document, true);
                PdfButtonFormField radioGroup = new RadioFormFieldBuilder(document, DUMMY_NAME).CreateRadioGroup();
                new RadioFormFieldBuilder(document, DUMMY_NAME).CreateRadioButton(DUMMY_APPEARANCE_NAME, DUMMY_RECTANGLE);
                form.AddField(radioGroup);
                NUnit.Framework.Assert.IsNull(radioGroup.GetPdfObject().Get(PdfName.Kids));
                NUnit.Framework.Assert.AreEqual(0, radioGroup.GetWidgets().Count);
            }
        }

        [NUnit.Framework.Test]
        public virtual void CreateRadioButtonAddToGroupGroupContainsOneRadioButton() {
            using (PdfDocument document = new PdfDocument(new PdfWriter(new MemoryStream()))) {
                PdfAcroForm form = PdfFormCreator.GetAcroForm(document, true);
                PdfButtonFormField radioGroup = new RadioFormFieldBuilder(document, DUMMY_NAME).CreateRadioGroup();
                PdfFormAnnotation radioAnnotation = new RadioFormFieldBuilder(document, DUMMY_NAME).CreateRadioButton(DUMMY_APPEARANCE_NAME
                    , DUMMY_RECTANGLE);
                radioGroup.AddKid(radioAnnotation);
                form.AddField(radioGroup);
                //In the previous implementation the radio buttons got added as kids, we want to avoid this
                NUnit.Framework.Assert.IsNull(radioGroup.GetKids());
                // It should now contain one single radio button
                NUnit.Framework.Assert.AreEqual(1, radioGroup.GetWidgets().Count);
            }
        }

        private static void CompareRadioButtons(PdfFormAnnotation radioButtonFormField, PdfButtonFormField radioGroup
            , PdfDocument pdfDoc, bool isAddedToRadioGroup) {
            PdfDictionary expectedDictionary = new PdfDictionary();
            IList<PdfWidgetAnnotation> widgets = new List<PdfWidgetAnnotation>();
            PdfWidgetAnnotation buttonWidget = radioButtonFormField.GetWidget();
            if (buttonWidget != null) {
                widgets.Add(buttonWidget);
                // if a rectangle is assigned in the builder than we should check it
                PdfArray buttonWidgetRectangle = buttonWidget.GetRectangle();
                if (buttonWidgetRectangle != null && buttonWidgetRectangle.ToRectangle() != null) {
                    NUnit.Framework.Assert.AreEqual(1, widgets.Count);
                    PdfWidgetAnnotation annotation = widgets[0];
                    NUnit.Framework.Assert.IsTrue(DUMMY_RECTANGLE.EqualsWithEpsilon(annotation.GetRectangle().ToRectangle()));
                    PutIfAbsent(expectedDictionary, PdfName.Rect, new PdfArray(DUMMY_RECTANGLE));
                    // if the radiobutton has been added to the radiogroup we expect the AP to be generated
                    if (isAddedToRadioGroup) {
                        PutIfAbsent(expectedDictionary, PdfName.AP, radioButtonFormField.GetPdfObject().GetAsDictionary(PdfName.AP
                            ));
                    }
                }
            }
            if (radioButtonFormField.pdfConformance != null && radioButtonFormField.pdfConformance.IsPdfAOrUa()) {
                PutIfAbsent(expectedDictionary, PdfName.F, new PdfNumber(PdfAnnotation.PRINT));
            }
            // for the AS key if it's added to the group we expect it to be off or the value if the radiogroup was selected
            // if its was not added we expect it to be the value
            if (isAddedToRadioGroup) {
                PdfName expectedAS = new PdfName(PdfFormAnnotation.OFF_STATE_VALUE);
                PdfName radioGroupValue = radioGroup.GetPdfObject().GetAsName(PdfName.V);
                if (radioGroupValue != null && radioGroupValue.Equals(radioButtonFormField.GetPdfObject().Get(PdfName.AS))
                    ) {
                    expectedAS = new PdfName(DUMMY_APPEARANCE_NAME);
                }
                PutIfAbsent(expectedDictionary, PdfName.AS, expectedAS);
                PutIfAbsent(expectedDictionary, PdfName.Parent, radioGroup.GetPdfObject());
            }
            else {
                PutIfAbsent(expectedDictionary, PdfName.AS, new PdfName(DUMMY_APPEARANCE_NAME));
            }
            PutIfAbsent(expectedDictionary, PdfName.Subtype, PdfName.Widget);
            expectedDictionary.MakeIndirect(pdfDoc);
            radioButtonFormField.MakeIndirect(pdfDoc);
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareDictionariesStructure(expectedDictionary, radioButtonFormField
                .GetPdfObject()));
        }

        private static void PutIfAbsent(PdfDictionary dictionary, PdfName name, PdfObject value) {
            if (!dictionary.ContainsKey(name)) {
                dictionary.Put(name, value);
            }
        }
    }
}
