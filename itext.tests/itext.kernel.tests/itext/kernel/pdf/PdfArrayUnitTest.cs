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
using iText.Kernel.Exceptions;
using iText.Test;

namespace iText.Kernel.Pdf {
    [NUnit.Framework.Category("UnitTest")]
    public class PdfArrayUnitTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void CannotConvertArrayOfPdfStringsToArrayOfBooleansTest() {
            PdfArray pdfArray = new PdfArray(new PdfString(""));
            Exception exception = NUnit.Framework.Assert.Catch(typeof(PdfException), () => pdfArray.ToBooleanArray());
            NUnit.Framework.Assert.AreEqual(KernelExceptionMessageConstant.CANNOT_CONVERT_PDF_ARRAY_TO_AN_ARRAY_OF_BOOLEANS
                , exception.Message);
        }

        [NUnit.Framework.Test]
        public virtual void CannotConvertArrayOfPdfStringsToDoubleArrayTest() {
            PdfArray pdfArray = new PdfArray(new PdfString(""));
            Exception exception = NUnit.Framework.Assert.Catch(typeof(PdfException), () => pdfArray.ToDoubleArray());
            NUnit.Framework.Assert.AreEqual(KernelExceptionMessageConstant.CANNOT_CONVERT_PDF_ARRAY_TO_DOUBLE_ARRAY, exception
                .Message);
        }

        [NUnit.Framework.Test]
        public virtual void CannotConvertArrayOfPdfStringsToIntArrayTest() {
            PdfArray pdfArray = new PdfArray(new PdfString(""));
            Exception exception = NUnit.Framework.Assert.Catch(typeof(PdfException), () => pdfArray.ToIntArray());
            NUnit.Framework.Assert.AreEqual(KernelExceptionMessageConstant.CANNOT_CONVERT_PDF_ARRAY_TO_INT_ARRAY, exception
                .Message);
        }

        [NUnit.Framework.Test]
        public virtual void CannotConvertArrayOfPdfStringsToFloatArrayTest() {
            PdfArray pdfArray = new PdfArray(new PdfString(""));
            Exception exception = NUnit.Framework.Assert.Catch(typeof(PdfException), () => pdfArray.ToFloatArray());
            NUnit.Framework.Assert.AreEqual(KernelExceptionMessageConstant.CANNOT_CONVERT_PDF_ARRAY_TO_FLOAT_ARRAY, exception
                .Message);
        }

        [NUnit.Framework.Test]
        public virtual void CannotConvertArrayOfPdfStringsToLongArrayTest() {
            PdfArray pdfArray = new PdfArray(new PdfString(""));
            Exception exception = NUnit.Framework.Assert.Catch(typeof(PdfException), () => pdfArray.ToLongArray());
            NUnit.Framework.Assert.AreEqual(KernelExceptionMessageConstant.CANNOT_CONVERT_PDF_ARRAY_TO_LONG_ARRAY, exception
                .Message);
        }

        [NUnit.Framework.Test]
        public virtual void CannotConvertArrayOfPdfStringsToRectangleTest() {
            PdfArray pdfArray = new PdfArray(new PdfString(""));
            Exception exception = NUnit.Framework.Assert.Catch(typeof(PdfException), () => pdfArray.ToRectangle());
            NUnit.Framework.Assert.AreEqual(KernelExceptionMessageConstant.CANNOT_CONVERT_PDF_ARRAY_TO_RECTANGLE, exception
                .Message);
        }
    }
}
