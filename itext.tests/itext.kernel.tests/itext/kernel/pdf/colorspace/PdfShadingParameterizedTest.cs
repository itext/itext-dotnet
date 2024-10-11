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
using iText.Commons.Utils;
using iText.Kernel.Exceptions;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Colorspace.Shading;
using iText.Test;

namespace iText.Kernel.Pdf.Colorspace {
    [NUnit.Framework.Category("UnitTest")]
    public class PdfShadingParameterizedTest : ExtendedITextTest {
        public static IEnumerable<Object[]> Parameters() {
            return JavaUtil.ArraysAsList(new Object[][] { new Object[] { "FreeFormGouraudShadedTriangleMesh", 4 }, new 
                Object[] { "LatticeFormGouraudShadedTriangleMesh", 5 }, new Object[] { "CoonsPatchMesh", 6 }, new Object
                [] { "TensorProductPatchMesh", 7 } });
        }

        [NUnit.Framework.TestCaseSource("Parameters")]
        public virtual void AllAboveType3FromDictionaryShouldFailTest(String shadingName, int shadingType) {
            PdfDictionary dict = new PdfDictionary();
            dict.Put(PdfName.ShadingType, new PdfNumber(shadingType));
            dict.Put(PdfName.ColorSpace, PdfName.DeviceRGB);
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfException), () => AbstractPdfShading.MakeShading(dict
                ), "Creating " + shadingName + " should throw PdfException.");
            NUnit.Framework.Assert.AreEqual(KernelExceptionMessageConstant.UNEXPECTED_SHADING_TYPE, e.Message);
        }
    }
}
