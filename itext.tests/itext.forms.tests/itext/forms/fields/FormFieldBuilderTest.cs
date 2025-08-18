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
using System.IO;
using iText.Kernel.Pdf;
using iText.Test;

namespace iText.Forms.Fields {
    [NUnit.Framework.Category("UnitTest")]
    public class FormFieldBuilderTest : ExtendedITextTest {
        private const String DUMMY_NAME = "dummy name";

        [NUnit.Framework.Test]
        public virtual void ConstructorTest() {
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(new MemoryStream()));
            FormFieldBuilderTest.TestBuilder builder = new FormFieldBuilderTest.TestBuilder(pdfDoc, DUMMY_NAME);
            NUnit.Framework.Assert.AreSame(pdfDoc, builder.GetDocument());
            NUnit.Framework.Assert.AreSame(DUMMY_NAME, builder.GetFormFieldName());
        }

        [NUnit.Framework.Test]
        public virtual void GetSetConformanceLevelTest() {
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(new MemoryStream()));
            FormFieldBuilderTest.TestBuilder builder = new FormFieldBuilderTest.TestBuilder(pdfDoc, DUMMY_NAME);
            builder.SetConformance(PdfConformance.PDF_A_1A);
            NUnit.Framework.Assert.AreSame(PdfAConformance.PDF_A_1A, builder.GetConformance().GetAConformance());
        }

        [NUnit.Framework.Test]
        public virtual void GetSetConformanceLevelPdfUATest() {
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(new MemoryStream()));
            FormFieldBuilderTest.TestBuilder builder = new FormFieldBuilderTest.TestBuilder(pdfDoc, DUMMY_NAME);
            builder.SetConformance(PdfConformance.PDF_UA_1);
            NUnit.Framework.Assert.AreSame(PdfUAConformance.PDF_UA_1, builder.GetConformance().GetUAConformance());
        }

        private class TestBuilder : FormFieldBuilder<FormFieldBuilderTest.TestBuilder> {
            protected internal TestBuilder(PdfDocument document, String formFieldName)
                : base(document, formFieldName) {
            }

            protected internal override FormFieldBuilderTest.TestBuilder GetThis() {
                return this;
            }
        }
    }
}
