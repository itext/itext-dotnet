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

namespace iText.Kernel.Pdf.Colorspace {
    [NUnit.Framework.Category("UnitTest")]
    public class PdfCieBasedCsUnitTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void WhitePointOfCalGrayIsIncorrectEmptyTest() {
            Exception exception = NUnit.Framework.Assert.Catch(typeof(PdfException), () => new PdfCieBasedCs.CalGray(new 
                float[] {  }));
            NUnit.Framework.Assert.AreEqual(KernelExceptionMessageConstant.WHITE_POINT_IS_INCORRECTLY_SPECIFIED, exception
                .Message);
        }

        [NUnit.Framework.Test]
        public virtual void WhitePointOfCalRgbIsIncorrectEmptyTest() {
            Exception exception = NUnit.Framework.Assert.Catch(typeof(PdfException), () => new PdfCieBasedCs.CalRgb(new 
                float[] {  }));
            NUnit.Framework.Assert.AreEqual(KernelExceptionMessageConstant.WHITE_POINT_IS_INCORRECTLY_SPECIFIED, exception
                .Message);
        }

        [NUnit.Framework.Test]
        public virtual void WhitePointOfLabIsIncorrectEmptyTest() {
            Exception exception = NUnit.Framework.Assert.Catch(typeof(PdfException), () => new PdfCieBasedCs.Lab(new float
                [] {  }));
            NUnit.Framework.Assert.AreEqual(KernelExceptionMessageConstant.WHITE_POINT_IS_INCORRECTLY_SPECIFIED, exception
                .Message);
        }

        [NUnit.Framework.Test]
        public virtual void WhitePointOfCalGrayIsIncorrectTooLittlePointsTest() {
            Exception exception = NUnit.Framework.Assert.Catch(typeof(PdfException), () => new PdfCieBasedCs.CalGray(new 
                float[] { 1, 2 }));
            NUnit.Framework.Assert.AreEqual(KernelExceptionMessageConstant.WHITE_POINT_IS_INCORRECTLY_SPECIFIED, exception
                .Message);
        }

        [NUnit.Framework.Test]
        public virtual void WhitePointOfCalRgbIsIncorrectTooLittlePointsTest() {
            Exception exception = NUnit.Framework.Assert.Catch(typeof(PdfException), () => new PdfCieBasedCs.CalRgb(new 
                float[] { 1, 2 }));
            NUnit.Framework.Assert.AreEqual(KernelExceptionMessageConstant.WHITE_POINT_IS_INCORRECTLY_SPECIFIED, exception
                .Message);
        }

        [NUnit.Framework.Test]
        public virtual void WhitePointOfLabIsIncorrectTooLittlePointsTest() {
            Exception exception = NUnit.Framework.Assert.Catch(typeof(PdfException), () => new PdfCieBasedCs.Lab(new float
                [] { 1, 2 }));
            NUnit.Framework.Assert.AreEqual(KernelExceptionMessageConstant.WHITE_POINT_IS_INCORRECTLY_SPECIFIED, exception
                .Message);
        }

        [NUnit.Framework.Test]
        public virtual void WhitePointOfCalGrayIsIncorrectTooMuchPointsTest() {
            Exception exception = NUnit.Framework.Assert.Catch(typeof(PdfException), () => new PdfCieBasedCs.CalGray(new 
                float[] { 1, 2, 3, 4 }));
            NUnit.Framework.Assert.AreEqual(KernelExceptionMessageConstant.WHITE_POINT_IS_INCORRECTLY_SPECIFIED, exception
                .Message);
        }

        [NUnit.Framework.Test]
        public virtual void WhitePointOfCalRgbIsIncorrectTooMuchPointsTest() {
            Exception exception = NUnit.Framework.Assert.Catch(typeof(PdfException), () => new PdfCieBasedCs.CalRgb(new 
                float[] { 1, 2, 3, 4 }));
            NUnit.Framework.Assert.AreEqual(KernelExceptionMessageConstant.WHITE_POINT_IS_INCORRECTLY_SPECIFIED, exception
                .Message);
        }

        [NUnit.Framework.Test]
        public virtual void WhitePointOfLabIsIncorrectTooMuchPointsTest() {
            Exception exception = NUnit.Framework.Assert.Catch(typeof(PdfException), () => new PdfCieBasedCs.Lab(new float
                [] { 1, 2, 3, 4 }));
            NUnit.Framework.Assert.AreEqual(KernelExceptionMessageConstant.WHITE_POINT_IS_INCORRECTLY_SPECIFIED, exception
                .Message);
        }
    }
}
