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
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Annot;
using iText.Kernel.Utils;
using iText.Test;

namespace iText.Forms.Fields {
    [NUnit.Framework.Category("UnitTest")]
    public class TextFormFieldBuilderTest : ExtendedITextTest {
        private static readonly PdfDocument DUMMY_DOCUMENT = new PdfDocument(new PdfWriter(new MemoryStream()));

        private const String DUMMY_NAME = "dummy name";

        private static readonly Rectangle DUMMY_RECTANGLE = new Rectangle(7, 11, 13, 17);

        [NUnit.Framework.Test]
        public virtual void ConstructorTest() {
            TextFormFieldBuilder builder = new TextFormFieldBuilder(DUMMY_DOCUMENT, DUMMY_NAME);
            NUnit.Framework.Assert.AreSame(DUMMY_DOCUMENT, builder.GetDocument());
            NUnit.Framework.Assert.AreSame(DUMMY_NAME, builder.GetFormFieldName());
        }

        [NUnit.Framework.Test]
        public virtual void CreateTextWithWidgetTest() {
            PdfTextFormField textFormField = new TextFormFieldBuilder(DUMMY_DOCUMENT, DUMMY_NAME).SetWidgetRectangle(DUMMY_RECTANGLE
                ).CreateText();
            PdfDictionary expectedDictionary = new PdfDictionary();
            expectedDictionary.Put(PdfName.Ff, new PdfNumber(0));
            CompareTexts(expectedDictionary, textFormField, true);
        }

        [NUnit.Framework.Test]
        public virtual void CreateTextWithoutWidgetTest() {
            PdfTextFormField textFormField = new TextFormFieldBuilder(DUMMY_DOCUMENT, DUMMY_NAME).CreateText();
            PdfDictionary expectedDictionary = new PdfDictionary();
            expectedDictionary.Put(PdfName.Ff, new PdfNumber(0));
            CompareTexts(expectedDictionary, textFormField, false);
        }

        [NUnit.Framework.Test]
        public virtual void CreateTextWithConformanceLevelTest() {
            PdfTextFormField textFormField = new TextFormFieldBuilder(DUMMY_DOCUMENT, DUMMY_NAME).SetWidgetRectangle(DUMMY_RECTANGLE
                ).SetConformanceLevel(PdfAConformanceLevel.PDF_A_1A).CreateText();
            PdfDictionary expectedDictionary = new PdfDictionary();
            expectedDictionary.Put(PdfName.Ff, new PdfNumber(0));
            CompareTexts(expectedDictionary, textFormField, true);
        }

        [NUnit.Framework.Test]
        public virtual void CreateMultilineTextWithWidgetTest() {
            PdfTextFormField textFormField = new TextFormFieldBuilder(DUMMY_DOCUMENT, DUMMY_NAME).SetWidgetRectangle(DUMMY_RECTANGLE
                ).CreateMultilineText();
            PdfDictionary expectedDictionary = new PdfDictionary();
            expectedDictionary.Put(PdfName.Ff, new PdfNumber(PdfTextFormField.FF_MULTILINE));
            CompareTexts(expectedDictionary, textFormField, true);
        }

        [NUnit.Framework.Test]
        public virtual void CreateMultilineTextWithoutWidgetTest() {
            PdfTextFormField textFormField = new TextFormFieldBuilder(DUMMY_DOCUMENT, DUMMY_NAME).CreateMultilineText(
                );
            PdfDictionary expectedDictionary = new PdfDictionary();
            expectedDictionary.Put(PdfName.Ff, new PdfNumber(PdfTextFormField.FF_MULTILINE));
            CompareTexts(expectedDictionary, textFormField, false);
        }

        [NUnit.Framework.Test]
        public virtual void CreateMultilineTextWithConformanceLevelTest() {
            PdfTextFormField textFormField = new TextFormFieldBuilder(DUMMY_DOCUMENT, DUMMY_NAME).SetWidgetRectangle(DUMMY_RECTANGLE
                ).SetConformanceLevel(PdfAConformanceLevel.PDF_A_1A).CreateMultilineText();
            PdfDictionary expectedDictionary = new PdfDictionary();
            expectedDictionary.Put(PdfName.Ff, new PdfNumber(PdfTextFormField.FF_MULTILINE));
            CompareTexts(expectedDictionary, textFormField, true);
        }

        private static void CompareTexts(PdfDictionary expectedDictionary, PdfTextFormField textFormField, bool widgetExpected
            ) {
            IList<PdfWidgetAnnotation> widgets = textFormField.GetWidgets();
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
            PutIfAbsent(expectedDictionary, PdfName.FT, PdfName.Tx);
            PutIfAbsent(expectedDictionary, PdfName.T, new PdfString(DUMMY_NAME));
            PutIfAbsent(expectedDictionary, PdfName.V, new PdfString(""));
            PutIfAbsent(expectedDictionary, PdfName.DA, new PdfString("/F1 12 Tf"));
            expectedDictionary.MakeIndirect(DUMMY_DOCUMENT);
            textFormField.MakeIndirect(DUMMY_DOCUMENT);
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareDictionariesStructure(expectedDictionary, textFormField
                .GetPdfObject()));
        }

        private static void PutIfAbsent(PdfDictionary dictionary, PdfName name, PdfObject value) {
            if (!dictionary.ContainsKey(name)) {
                dictionary.Put(name, value);
            }
        }
    }
}
