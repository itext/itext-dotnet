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
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Colorspace;
using iText.Kernel.Pdf.Function;
using iText.Test;

namespace iText.Pdfa.Checker {
    [NUnit.Framework.Category("UnitTest")]
    public class PdfA4ImplementationLimitsTest : ExtendedITextTest {
        private PdfA4Checker pdfA4Checker = new PdfA4Checker(PdfAConformanceLevel.PDF_A_4);

        [NUnit.Framework.SetUp]
        public virtual void Before() {
            pdfA4Checker.SetFullCheckMode(true);
        }

        [NUnit.Framework.Test]
        public virtual void IndependentLongStringTest() {
            int maxAllowedLength = new PdfA2Checker(PdfAConformanceLevel.PDF_A_2B).GetMaxStringLength();
            int testLength = maxAllowedLength + 1;
            PdfString longString = PdfACheckerTestUtils.GetLongString(testLength);
            //An exception should not be thrown because pdf/a-4 spec allows any length strings
            pdfA4Checker.CheckPdfObject(longString);
            NUnit.Framework.Assert.AreEqual(testLength, longString.ToString().Length);
        }

        [NUnit.Framework.Test]
        public virtual void LongStringInContentStreamTest() {
            int maxAllowedLength = new PdfA2Checker(PdfAConformanceLevel.PDF_A_2B).GetMaxStringLength();
            int testLength = maxAllowedLength + 1;
            PdfString longString = PdfACheckerTestUtils.GetLongString(testLength);
            String newContentString = PdfACheckerTestUtils.GetStreamWithValue(longString);
            byte[] newContent = newContentString.GetBytes(System.Text.Encoding.UTF8);
            PdfStream stream = new PdfStream(newContent);
            //An exception should not be thrown because pdf/a-4 spec allows any length strings
            pdfA4Checker.CheckContentStream(stream);
            NUnit.Framework.Assert.AreEqual(testLength, longString.ToString().Length);
        }

        [NUnit.Framework.Test]
        public virtual void IndependentLargeRealTest() {
            PdfNumber largeNumber = new PdfNumber(new PdfA2Checker(PdfAConformanceLevel.PDF_A_2B).GetMaxRealValue());
            // An exception shall not be thrown pdf/a-4 has no number limits
            pdfA4Checker.CheckPdfObject(largeNumber);
            NUnit.Framework.Assert.AreEqual(float.MaxValue, largeNumber.FloatValue(), 0.001f);
        }

        [NUnit.Framework.Test]
        public virtual void DeviceNColorspaceWithMoreThan32Components() {
            //exception shall not be thrown as pdf/a-4 supports any number of deviceN components
            PdfDictionary currentColorSpaces = new PdfDictionary();
            pdfA4Checker.CheckColorSpace(BuildDeviceNColorspace(40), null, currentColorSpaces, true, false);
        }

        [NUnit.Framework.Test]
        public virtual void LongPdfNameTest() {
            //exception shall not be thrown as pdf/a-4 supports greater than 127 characters pdf names
            pdfA4Checker.CheckPdfObject(PdfACheckerTestUtils.GetLongName(200));
        }

        private PdfColorSpace BuildDeviceNColorspace(int numberOfComponents) {
            IList<String> tmpArray = new List<String>(numberOfComponents);
            float[] transformArray = new float[numberOfComponents * 2];
            for (int i = 0; i < numberOfComponents; i++) {
                tmpArray.Add("MyColor" + i + 1);
                transformArray[i * 2] = 0;
                transformArray[i * 2 + 1] = 1;
            }
            PdfType4Function function = new PdfType4Function(transformArray, new float[] { 0, 1, 0, 1, 0, 1 }, "{0}".GetBytes
                (iText.Commons.Utils.EncodingUtil.ISO_8859_1));
            PdfArray deviceNAsArray = ((PdfArray)(new PdfSpecialCs.DeviceN(tmpArray, new PdfDeviceCs.Rgb(), function))
                .GetPdfObject());
            PdfDictionary attributes = new PdfDictionary();
            PdfDictionary colourants = new PdfDictionary();
            String colourantName = "colourantTest";
            colourants.Put(new PdfName(colourantName), new PdfSpecialCs.DeviceN(((PdfArray)(new PdfSpecialCs.DeviceN(tmpArray
                , new PdfDeviceCs.Rgb(), function)).GetPdfObject())).GetPdfObject());
            attributes.Put(PdfName.Colorants, colourants);
            deviceNAsArray.Add(attributes);
            return new PdfSpecialCs.DeviceN(deviceNAsArray);
        }
    }
}
