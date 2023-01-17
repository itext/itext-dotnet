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
using System.IO;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Annot;
using iText.Kernel.Utils;
using iText.Test;

namespace iText.Forms.Fields {
    [NUnit.Framework.Category("UnitTest")]
    public class RadioFormFieldBuilderTest : ExtendedITextTest {
        private static readonly PdfDocument DUMMY_DOCUMENT = new PdfDocument(new PdfWriter(new MemoryStream()));

        private const String DUMMY_NAME = "dummy name";

        private static readonly Rectangle DUMMY_RECTANGLE = new Rectangle(7, 11, 13, 17);

        private const String DUMMY_APPEARANCE_NAME = "dummy appearance name";

        [NUnit.Framework.Test]
        public virtual void OneParameterConstructorTest() {
            RadioFormFieldBuilder builder = new RadioFormFieldBuilder(DUMMY_DOCUMENT);
            NUnit.Framework.Assert.AreSame(DUMMY_DOCUMENT, builder.GetDocument());
            NUnit.Framework.Assert.IsNull(builder.GetFormFieldName());
        }

        [NUnit.Framework.Test]
        public virtual void TwoParametersConstructorTest() {
            RadioFormFieldBuilder builder = new RadioFormFieldBuilder(DUMMY_DOCUMENT, DUMMY_NAME);
            NUnit.Framework.Assert.AreSame(DUMMY_DOCUMENT, builder.GetDocument());
            NUnit.Framework.Assert.AreSame(DUMMY_NAME, builder.GetFormFieldName());
        }

        [NUnit.Framework.Test]
        public virtual void CreateRadioGroupTest() {
            PdfButtonFormField radioGroup = new RadioFormFieldBuilder(DUMMY_DOCUMENT, DUMMY_NAME).CreateRadioGroup();
            CompareRadioGroups(radioGroup);
        }

        [NUnit.Framework.Test]
        public virtual void CreateRadioButtonWithWidgetTest() {
            PdfButtonFormField radioGroup = new RadioFormFieldBuilder(DUMMY_DOCUMENT, DUMMY_NAME).CreateRadioGroup();
            PdfFormField radioFormField = new RadioFormFieldBuilder(DUMMY_DOCUMENT, DUMMY_NAME).SetWidgetRectangle(DUMMY_RECTANGLE
                ).CreateRadioButton(radioGroup, DUMMY_APPEARANCE_NAME);
            CompareRadioButtons(radioFormField, radioGroup, true);
        }

        [NUnit.Framework.Test]
        public virtual void CreateRadioButtonWithoutWidgetTest() {
            PdfButtonFormField radioGroup = new RadioFormFieldBuilder(DUMMY_DOCUMENT, DUMMY_NAME).CreateRadioGroup();
            PdfFormField radioFormField = new RadioFormFieldBuilder(DUMMY_DOCUMENT, DUMMY_NAME).CreateRadioButton(radioGroup
                , DUMMY_APPEARANCE_NAME);
            CompareRadioButtons(radioFormField, radioGroup, false);
        }

        [NUnit.Framework.Test]
        public virtual void CreateRadioButtonWithConformanceLevelTest() {
            PdfButtonFormField radioGroup = new RadioFormFieldBuilder(DUMMY_DOCUMENT, DUMMY_NAME).CreateRadioGroup();
            PdfFormField radioFormField = new RadioFormFieldBuilder(DUMMY_DOCUMENT, DUMMY_NAME).SetWidgetRectangle(DUMMY_RECTANGLE
                ).SetConformanceLevel(PdfAConformanceLevel.PDF_A_1A).CreateRadioButton(radioGroup, DUMMY_APPEARANCE_NAME
                );
            CompareRadioButtons(radioFormField, radioGroup, true);
        }

        private static void CompareRadioGroups(PdfButtonFormField radioGroupFormField) {
            PdfDictionary expectedDictionary = new PdfDictionary();
            PutIfAbsent(expectedDictionary, PdfName.FT, PdfName.Btn);
            PutIfAbsent(expectedDictionary, PdfName.Ff, new PdfNumber(PdfButtonFormField.FF_RADIO));
            PutIfAbsent(expectedDictionary, PdfName.T, new PdfString(DUMMY_NAME));
            expectedDictionary.MakeIndirect(DUMMY_DOCUMENT);
            radioGroupFormField.MakeIndirect(DUMMY_DOCUMENT);
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareDictionariesStructure(expectedDictionary, radioGroupFormField
                .GetPdfObject()));
        }

        private static void CompareRadioButtons(PdfFormField radioButtonFormField, PdfButtonFormField radioGroup, 
            bool widgetExpected) {
            PdfDictionary expectedDictionary = new PdfDictionary();
            IList<PdfWidgetAnnotation> widgets = radioButtonFormField.GetWidgets();
            if (widgetExpected) {
                NUnit.Framework.Assert.AreEqual(1, widgets.Count);
                PdfWidgetAnnotation annotation = widgets[0];
                NUnit.Framework.Assert.IsTrue(DUMMY_RECTANGLE.EqualsWithEpsilon(annotation.GetRectangle().ToRectangle()));
                PdfArray kids = new PdfArray();
                kids.Add(annotation.GetPdfObject());
                PutIfAbsent(expectedDictionary, PdfName.Kids, kids);
            }
            else {
                NUnit.Framework.Assert.AreEqual(0, widgets.Count);
            }
            PutIfAbsent(expectedDictionary, PdfName.Parent, radioGroup.GetPdfObject());
            PutIfAbsent(expectedDictionary, PdfName.FT, PdfName.Btn);
            expectedDictionary.MakeIndirect(DUMMY_DOCUMENT);
            radioButtonFormField.MakeIndirect(DUMMY_DOCUMENT);
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
