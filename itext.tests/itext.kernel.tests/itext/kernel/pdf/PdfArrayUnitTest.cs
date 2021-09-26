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
using iText.Kernel.Exceptions;
using iText.Test;

namespace iText.Kernel.Pdf {
    public class PdfArrayUnitTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void CannotConvertArrayOfPdfStringsToArrayOfBooleansTest() {
            NUnit.Framework.Assert.That(() =>  {
                PdfArray pdfArray = new PdfArray(new PdfString(""));
                pdfArray.ToBooleanArray();
            }
            , NUnit.Framework.Throws.InstanceOf<PdfException>().With.Message.EqualTo(KernelExceptionMessageConstant.CANNOT_CONVERT_PDF_ARRAY_TO_AN_ARRAY_OF_BOOLEANS))
;
        }

        [NUnit.Framework.Test]
        public virtual void CannotConvertArrayOfPdfStringsToDoubleArrayTest() {
            NUnit.Framework.Assert.That(() =>  {
                PdfArray pdfArray = new PdfArray(new PdfString(""));
                pdfArray.ToDoubleArray();
            }
            , NUnit.Framework.Throws.InstanceOf<PdfException>().With.Message.EqualTo(KernelExceptionMessageConstant.CANNOT_CONVERT_PDF_ARRAY_TO_DOUBLE_ARRAY))
;
        }

        [NUnit.Framework.Test]
        public virtual void CannotConvertArrayOfPdfStringsToIntArrayTest() {
            NUnit.Framework.Assert.That(() =>  {
                PdfArray pdfArray = new PdfArray(new PdfString(""));
                pdfArray.ToIntArray();
            }
            , NUnit.Framework.Throws.InstanceOf<PdfException>().With.Message.EqualTo(KernelExceptionMessageConstant.CANNOT_CONVERT_PDF_ARRAY_TO_INT_ARRAY))
;
        }

        [NUnit.Framework.Test]
        public virtual void CannotConvertArrayOfPdfStringsToFloatArrayTest() {
            NUnit.Framework.Assert.That(() =>  {
                PdfArray pdfArray = new PdfArray(new PdfString(""));
                pdfArray.ToFloatArray();
            }
            , NUnit.Framework.Throws.InstanceOf<PdfException>().With.Message.EqualTo(KernelExceptionMessageConstant.CANNOT_CONVERT_PDF_ARRAY_TO_FLOAT_ARRAY))
;
        }

        [NUnit.Framework.Test]
        public virtual void CannotConvertArrayOfPdfStringsToLongArrayTest() {
            NUnit.Framework.Assert.That(() =>  {
                PdfArray pdfArray = new PdfArray(new PdfString(""));
                pdfArray.ToLongArray();
            }
            , NUnit.Framework.Throws.InstanceOf<PdfException>().With.Message.EqualTo(KernelExceptionMessageConstant.CANNOT_CONVERT_PDF_ARRAY_TO_LONG_ARRAY))
;
        }

        [NUnit.Framework.Test]
        public virtual void CannotConvertArrayOfPdfStringsToRectangleTest() {
            NUnit.Framework.Assert.That(() =>  {
                PdfArray pdfArray = new PdfArray(new PdfString(""));
                pdfArray.ToRectangle();
            }
            , NUnit.Framework.Throws.InstanceOf<PdfException>().With.Message.EqualTo(KernelExceptionMessageConstant.CANNOT_CONVERT_PDF_ARRAY_TO_RECTANGLE))
;
        }
    }
}
