/*
This file is part of the iText (R) project.
Copyright (c) 1998-2021 iText Group NV
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
using iText.Forms.Exceptions;
using iText.IO.Source;
using iText.Kernel.Exceptions;
using iText.Kernel.Pdf;
using iText.Test;

namespace iText.Forms.Fields {
    public class PdfFormFieldUnitTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void CannotGetRectangleIfKidsIsNullTest() {
            NUnit.Framework.Assert.That(() =>  {
                PdfDocument pdfDocument = new PdfDocument(new PdfWriter(new ByteArrayOutputStream()));
                PdfDictionary pdfDictionary = new PdfDictionary();
                PdfFormField pdfFormField = new PdfFormField(pdfDocument);
                pdfFormField.GetRect(pdfDictionary);
            }
            , NUnit.Framework.Throws.InstanceOf<PdfException>().With.Message.EqualTo(FormsExceptionMessageConstant.WRONG_FORM_FIELD_ADD_ANNOTATION_TO_THE_FIELD))
;
        }
    }
}
