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
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Annot;
using iText.Kernel.Utils;
using iText.Test;

namespace iText.Forms.Fields {
    [NUnit.Framework.Category("UnitTest")]
    public class TextFormFieldBuilderTest : ExtendedITextTest {
        private const String DUMMY_NAME = "dummy name";

        private static readonly Rectangle DUMMY_RECTANGLE = new Rectangle(7, 11, 13, 17);

        [NUnit.Framework.Test]
        public virtual void ConstructorTest() {
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(new MemoryStream()));
            TextFormFieldBuilder builder = new TextFormFieldBuilder(pdfDoc, DUMMY_NAME);
            NUnit.Framework.Assert.AreSame(pdfDoc, builder.GetDocument());
            NUnit.Framework.Assert.AreSame(DUMMY_NAME, builder.GetFormFieldName());
        }

        [NUnit.Framework.Test]
        public virtual void CreateTextWithWidgetTest() {
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(new MemoryStream()));
            PdfTextFormField textFormField = new TextFormFieldBuilder(pdfDoc, DUMMY_NAME).SetWidgetRectangle(DUMMY_RECTANGLE
                ).CreateText();
            PdfDictionary expectedDictionary = new PdfDictionary();
            expectedDictionary.Put(PdfName.Ff, new PdfNumber(0));
            CompareTexts(expectedDictionary, textFormField, pdfDoc, true);
        }

        [NUnit.Framework.Test]
        public virtual void CreateTextWithoutWidgetTest() {
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(new MemoryStream()));
            PdfTextFormField textFormField = new TextFormFieldBuilder(pdfDoc, DUMMY_NAME).CreateText();
            PdfDictionary expectedDictionary = new PdfDictionary();
            expectedDictionary.Put(PdfName.Ff, new PdfNumber(0));
            CompareTexts(expectedDictionary, textFormField, pdfDoc, false);
        }

        [NUnit.Framework.Test]
        public virtual void CreateTextWithIncorrectNameTest() {
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(new MemoryStream()));
            NUnit.Framework.Assert.DoesNotThrow(() => new TextFormFieldBuilder(pdfDoc, "incorrect.name").SetWidgetRectangle
                (DUMMY_RECTANGLE).CreateText());
        }

        [NUnit.Framework.Test]
        public virtual void CreateTextWithConformanceLevelTest() {
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(new MemoryStream()));
            PdfTextFormField textFormField = new TextFormFieldBuilder(pdfDoc, DUMMY_NAME).SetWidgetRectangle(DUMMY_RECTANGLE
                ).SetConformance(PdfConformance.PDF_A_1A).CreateText();
            PdfDictionary expectedDictionary = new PdfDictionary();
            expectedDictionary.Put(PdfName.Ff, new PdfNumber(0));
            CompareTexts(expectedDictionary, textFormField, pdfDoc, true);
        }

        [NUnit.Framework.Test]
        public virtual void CreateMultilineTextWithWidgetTest() {
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(new MemoryStream()));
            PdfTextFormField textFormField = new TextFormFieldBuilder(pdfDoc, DUMMY_NAME).SetWidgetRectangle(DUMMY_RECTANGLE
                ).CreateMultilineText();
            PdfDictionary expectedDictionary = new PdfDictionary();
            expectedDictionary.Put(PdfName.Ff, new PdfNumber(PdfTextFormField.FF_MULTILINE));
            CompareTexts(expectedDictionary, textFormField, pdfDoc, true);
        }

        [NUnit.Framework.Test]
        public virtual void CreateMultilineTextWithoutWidgetTest() {
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(new MemoryStream()));
            PdfTextFormField textFormField = new TextFormFieldBuilder(pdfDoc, DUMMY_NAME).CreateMultilineText();
            PdfDictionary expectedDictionary = new PdfDictionary();
            expectedDictionary.Put(PdfName.Ff, new PdfNumber(PdfTextFormField.FF_MULTILINE));
            CompareTexts(expectedDictionary, textFormField, pdfDoc, false);
        }

        [NUnit.Framework.Test]
        public virtual void CreateMultilineTextWithIncorrectNameTest() {
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(new MemoryStream()));
            NUnit.Framework.Assert.DoesNotThrow(() => new TextFormFieldBuilder(pdfDoc, "incorrect.name").SetWidgetRectangle
                (DUMMY_RECTANGLE).CreateMultilineText());
        }

        [NUnit.Framework.Test]
        public virtual void CreateMultilineTextWithConformanceLevelTest() {
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(new MemoryStream()));
            PdfTextFormField textFormField = new TextFormFieldBuilder(pdfDoc, DUMMY_NAME).SetWidgetRectangle(DUMMY_RECTANGLE
                ).SetConformance(PdfConformance.PDF_A_1A).CreateMultilineText();
            PdfDictionary expectedDictionary = new PdfDictionary();
            expectedDictionary.Put(PdfName.Ff, new PdfNumber(PdfTextFormField.FF_MULTILINE));
            CompareTexts(expectedDictionary, textFormField, pdfDoc, true);
        }

        private static void CompareTexts(PdfDictionary expectedDictionary, PdfTextFormField textFormField, PdfDocument
             pdfDoc, bool widgetExpected) {
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
            expectedDictionary.MakeIndirect(pdfDoc);
            textFormField.MakeIndirect(pdfDoc);
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
