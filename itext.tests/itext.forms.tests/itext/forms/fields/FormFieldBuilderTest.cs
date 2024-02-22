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
using System.IO;
using iText.Kernel.Pdf;
using iText.Test;

namespace iText.Forms.Fields {
    [NUnit.Framework.Category("UnitTest")]
    public class FormFieldBuilderTest : ExtendedITextTest {
        private static readonly PdfDocument DUMMY_DOCUMENT = new PdfDocument(new PdfWriter(new MemoryStream()));

        private const String DUMMY_NAME = "dummy name";

        [NUnit.Framework.Test]
        public virtual void ConstructorTest() {
            FormFieldBuilderTest.TestBuilder builder = new FormFieldBuilderTest.TestBuilder(DUMMY_DOCUMENT, DUMMY_NAME
                );
            NUnit.Framework.Assert.AreSame(DUMMY_DOCUMENT, builder.GetDocument());
            NUnit.Framework.Assert.AreSame(DUMMY_NAME, builder.GetFormFieldName());
        }

        [NUnit.Framework.Test]
        public virtual void GetSetConformanceLevelTest() {
            FormFieldBuilderTest.TestBuilder builder = new FormFieldBuilderTest.TestBuilder(DUMMY_DOCUMENT, DUMMY_NAME
                );
            builder.SetGenericConformanceLevel(PdfAConformanceLevel.PDF_A_1A);
            NUnit.Framework.Assert.AreSame(PdfAConformanceLevel.PDF_A_1A, builder.GetGenericConformanceLevel());
        }

        [NUnit.Framework.Test]
        public virtual void GetSetConformanceLevelDepreceatedTest() {
            FormFieldBuilderTest.TestBuilder builder = new FormFieldBuilderTest.TestBuilder(DUMMY_DOCUMENT, DUMMY_NAME
                );
            builder.SetConformanceLevel(PdfAConformanceLevel.PDF_A_1A);
            NUnit.Framework.Assert.AreSame(PdfAConformanceLevel.PDF_A_1A, builder.GetConformanceLevel());
        }

        [NUnit.Framework.Test]
        public virtual void GetSetConformanceLevelDifferentTest() {
            FormFieldBuilderTest.TestBuilder builder = new FormFieldBuilderTest.TestBuilder(DUMMY_DOCUMENT, DUMMY_NAME
                );
            builder.SetGenericConformanceLevel(PdfUAConformanceLevel.PDFUA_1);
            NUnit.Framework.Assert.IsNull(builder.GetConformanceLevel());
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
