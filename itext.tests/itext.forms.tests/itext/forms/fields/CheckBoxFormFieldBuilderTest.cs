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
using iText.Forms.Fields.Properties;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Annot;
using iText.Kernel.Utils;
using iText.Test;

namespace iText.Forms.Fields {
    [NUnit.Framework.Category("UnitTest")]
    public class CheckBoxFormFieldBuilderTest : ExtendedITextTest {
        private static readonly PdfDocument DUMMY_DOCUMENT = new PdfDocument(new PdfWriter(new MemoryStream()));

        private const String DUMMY_NAME = "dummy name";

        private static readonly Rectangle DUMMY_RECTANGLE = new Rectangle(7, 11, 13, 17);

        [NUnit.Framework.Test]
        public virtual void ConstructorTest() {
            CheckBoxFormFieldBuilder builder = new CheckBoxFormFieldBuilder(DUMMY_DOCUMENT, DUMMY_NAME);
            NUnit.Framework.Assert.AreSame(DUMMY_DOCUMENT, builder.GetDocument());
            NUnit.Framework.Assert.AreSame(DUMMY_NAME, builder.GetFormFieldName());
        }

        [NUnit.Framework.Test]
        public virtual void SetGetCheckType() {
            CheckBoxFormFieldBuilder builder = new CheckBoxFormFieldBuilder(DUMMY_DOCUMENT, DUMMY_NAME);
            builder.SetCheckType(CheckBoxType.DIAMOND);
            NUnit.Framework.Assert.AreEqual(CheckBoxType.DIAMOND, builder.GetCheckType());
        }

        [NUnit.Framework.Test]
        public virtual void CreateCheckBoxWithWidgetTest() {
            PdfButtonFormField checkBoxFormField = new CheckBoxFormFieldBuilder(DUMMY_DOCUMENT, DUMMY_NAME).SetWidgetRectangle
                (DUMMY_RECTANGLE).CreateCheckBox();
            CompareCheckBoxes(checkBoxFormField, true);
        }

        [NUnit.Framework.Test]
        public virtual void CreateCheckBoxWithoutWidgetTest() {
            PdfButtonFormField checkBoxFormField = new CheckBoxFormFieldBuilder(DUMMY_DOCUMENT, DUMMY_NAME).CreateCheckBox
                ();
            CompareCheckBoxes(checkBoxFormField, false);
        }

        [NUnit.Framework.Test]
        public virtual void CreateCheckBoxWithConformanceLevelTest() {
            PdfButtonFormField checkBoxFormField = new CheckBoxFormFieldBuilder(DUMMY_DOCUMENT, DUMMY_NAME).SetWidgetRectangle
                (DUMMY_RECTANGLE).SetConformance(PdfConformance.PDF_A_1A).CreateCheckBox();
            CompareCheckBoxes(checkBoxFormField, true);
        }

        private static void CompareCheckBoxes(PdfButtonFormField checkBoxFormField, bool widgetExpected) {
            PdfDictionary expectedDictionary = new PdfDictionary();
            IList<PdfWidgetAnnotation> widgets = checkBoxFormField.GetWidgets();
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
            PutIfAbsent(expectedDictionary, PdfName.FT, PdfName.Btn);
            PutIfAbsent(expectedDictionary, PdfName.T, new PdfString(DUMMY_NAME));
            PutIfAbsent(expectedDictionary, PdfName.V, new PdfName(PdfFormAnnotation.OFF_STATE_VALUE));
            expectedDictionary.MakeIndirect(DUMMY_DOCUMENT);
            checkBoxFormField.MakeIndirect(DUMMY_DOCUMENT);
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareDictionariesStructure(expectedDictionary, checkBoxFormField
                .GetPdfObject()));
        }

        private static void PutIfAbsent(PdfDictionary dictionary, PdfName name, PdfObject value) {
            if (!dictionary.ContainsKey(name)) {
                dictionary.Put(name, value);
            }
        }
    }
}
