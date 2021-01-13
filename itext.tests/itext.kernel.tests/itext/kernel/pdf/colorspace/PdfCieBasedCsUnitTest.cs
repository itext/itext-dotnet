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
using iText.Kernel;
using iText.Kernel.Exceptions;
using iText.Test;

namespace iText.Kernel.Pdf.Colorspace {
    public class PdfCieBasedCsUnitTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void WhitePointOfCalGrayIsIncorrectEmptyTest() {
            NUnit.Framework.Assert.That(() =>  {
                PdfCieBasedCs basedCs = new PdfCieBasedCs.CalGray(new float[] {  });
            }
            , NUnit.Framework.Throws.InstanceOf<PdfException>().With.Message.EqualTo(KernelExceptionMessageConstant.WHITE_POINT_IS_INCORRECTLY_SPECIFIED))
;
        }

        [NUnit.Framework.Test]
        public virtual void WhitePointOfCalRgbIsIncorrectEmptyTest() {
            NUnit.Framework.Assert.That(() =>  {
                PdfCieBasedCs basedCs = new PdfCieBasedCs.CalRgb(new float[] {  });
            }
            , NUnit.Framework.Throws.InstanceOf<PdfException>().With.Message.EqualTo(KernelExceptionMessageConstant.WHITE_POINT_IS_INCORRECTLY_SPECIFIED))
;
        }

        [NUnit.Framework.Test]
        public virtual void WhitePointOfLabIsIncorrectEmptyTest() {
            NUnit.Framework.Assert.That(() =>  {
                PdfCieBasedCs basedCs = new PdfCieBasedCs.Lab(new float[] {  });
            }
            , NUnit.Framework.Throws.InstanceOf<PdfException>().With.Message.EqualTo(KernelExceptionMessageConstant.WHITE_POINT_IS_INCORRECTLY_SPECIFIED))
;
        }

        [NUnit.Framework.Test]
        public virtual void WhitePointOfCalGrayIsIncorrectTooLittlePointsTest() {
            NUnit.Framework.Assert.That(() =>  {
                PdfCieBasedCs basedCs = new PdfCieBasedCs.CalGray(new float[] { 1, 2 });
            }
            , NUnit.Framework.Throws.InstanceOf<PdfException>().With.Message.EqualTo(KernelExceptionMessageConstant.WHITE_POINT_IS_INCORRECTLY_SPECIFIED))
;
        }

        [NUnit.Framework.Test]
        public virtual void WhitePointOfCalRgbIsIncorrectTooLittlePointsTest() {
            NUnit.Framework.Assert.That(() =>  {
                PdfCieBasedCs basedCs = new PdfCieBasedCs.CalRgb(new float[] { 1, 2 });
            }
            , NUnit.Framework.Throws.InstanceOf<PdfException>().With.Message.EqualTo(KernelExceptionMessageConstant.WHITE_POINT_IS_INCORRECTLY_SPECIFIED))
;
        }

        [NUnit.Framework.Test]
        public virtual void WhitePointOfLabIsIncorrectTooLittlePointsTest() {
            NUnit.Framework.Assert.That(() =>  {
                PdfCieBasedCs basedCs = new PdfCieBasedCs.Lab(new float[] { 1, 2 });
            }
            , NUnit.Framework.Throws.InstanceOf<PdfException>().With.Message.EqualTo(KernelExceptionMessageConstant.WHITE_POINT_IS_INCORRECTLY_SPECIFIED))
;
        }

        [NUnit.Framework.Test]
        public virtual void WhitePointOfCalGrayIsIncorrectTooMuchPointsTest() {
            NUnit.Framework.Assert.That(() =>  {
                PdfCieBasedCs basedCs = new PdfCieBasedCs.CalGray(new float[] { 1, 2, 3, 4 });
            }
            , NUnit.Framework.Throws.InstanceOf<PdfException>().With.Message.EqualTo(KernelExceptionMessageConstant.WHITE_POINT_IS_INCORRECTLY_SPECIFIED))
;
        }

        [NUnit.Framework.Test]
        public virtual void WhitePointOfCalRgbIsIncorrectTooMuchPointsTest() {
            NUnit.Framework.Assert.That(() =>  {
                PdfCieBasedCs basedCs = new PdfCieBasedCs.CalRgb(new float[] { 1, 2, 3, 4 });
            }
            , NUnit.Framework.Throws.InstanceOf<PdfException>().With.Message.EqualTo(KernelExceptionMessageConstant.WHITE_POINT_IS_INCORRECTLY_SPECIFIED))
;
        }

        [NUnit.Framework.Test]
        public virtual void WhitePointOfLabIsIncorrectTooMuchPointsTest() {
            NUnit.Framework.Assert.That(() =>  {
                PdfCieBasedCs basedCs = new PdfCieBasedCs.Lab(new float[] { 1, 2, 3, 4 });
            }
            , NUnit.Framework.Throws.InstanceOf<PdfException>().With.Message.EqualTo(KernelExceptionMessageConstant.WHITE_POINT_IS_INCORRECTLY_SPECIFIED))
;
        }
    }
}
