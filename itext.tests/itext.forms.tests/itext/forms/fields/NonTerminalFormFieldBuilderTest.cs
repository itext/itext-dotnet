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
using System.Collections.Generic;
using System.IO;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Annot;
using iText.Kernel.Utils;
using iText.Test;

namespace iText.Forms.Fields {
    [NUnit.Framework.Category("UnitTest")]
    public class NonTerminalFormFieldBuilderTest : ExtendedITextTest {
        private static readonly PdfDocument DUMMY_DOCUMENT = new PdfDocument(new PdfWriter(new MemoryStream()));

        private const String DUMMY_NAME = "dummy name";

        [NUnit.Framework.Test]
        public virtual void ConstructorTest() {
            NonTerminalFormFieldBuilder builder = new NonTerminalFormFieldBuilder(DUMMY_DOCUMENT, DUMMY_NAME);
            NUnit.Framework.Assert.AreSame(DUMMY_DOCUMENT, builder.GetDocument());
            NUnit.Framework.Assert.AreSame(DUMMY_NAME, builder.GetFormFieldName());
        }

        [NUnit.Framework.Test]
        public virtual void CreateNonTerminalFormField() {
            PdfFormField nonTerminalFormField = new NonTerminalFormFieldBuilder(DUMMY_DOCUMENT, DUMMY_NAME).CreateNonTerminalFormField
                ();
            CompareNonTerminalFormFields(nonTerminalFormField);
        }

        private static void CompareNonTerminalFormFields(PdfFormField nonTerminalFormField) {
            PdfDictionary expectedDictionary = new PdfDictionary();
            IList<PdfWidgetAnnotation> widgets = nonTerminalFormField.GetWidgets();
            NUnit.Framework.Assert.AreEqual(0, widgets.Count);
            PutIfAbsent(expectedDictionary, PdfName.T, new PdfString(DUMMY_NAME));
            expectedDictionary.MakeIndirect(DUMMY_DOCUMENT);
            nonTerminalFormField.MakeIndirect(DUMMY_DOCUMENT);
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareDictionariesStructure(expectedDictionary, nonTerminalFormField
                .GetPdfObject()));
        }

        private static void PutIfAbsent(PdfDictionary dictionary, PdfName name, PdfObject value) {
            if (!dictionary.ContainsKey(name)) {
                dictionary.Put(name, value);
            }
        }
    }
}
