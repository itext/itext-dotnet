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
using iText.Pdfa.Exceptions;
using iText.Test;

namespace iText.Pdfa.Checker {
    [NUnit.Framework.Category("UnitTest")]
    public class PdfA2ImplementationLimitsCheckerTest : ExtendedITextTest {
        private PdfA2Checker pdfA2Checker;

        [NUnit.Framework.SetUp]
        public virtual void Before() {
            pdfA2Checker = new PdfA2Checker(PdfAConformanceLevel.PDF_A_2B);
            pdfA2Checker.SetFullCheckMode(true);
        }

        [NUnit.Framework.Test]
        public virtual void IndependentLongStringTest() {
            int maxAllowedLength = pdfA2Checker.GetMaxStringLength();
            int testLength = maxAllowedLength + 1;
            NUnit.Framework.Assert.AreEqual(testLength, 32768);
            PdfString longString = PdfACheckerTestUtils.GetLongString(testLength);
            // An exception should be thrown as provided String is longer then
            // it is allowed per specification
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => pdfA2Checker.CheckPdfObject
                (longString));
            NUnit.Framework.Assert.AreEqual(PdfaExceptionMessageConstant.PDF_STRING_IS_TOO_LONG, e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void LongStringInContentStreamTest() {
            pdfA2Checker.SetFullCheckMode(true);
            int maxAllowedLength = pdfA2Checker.GetMaxStringLength();
            int testLength = maxAllowedLength + 1;
            NUnit.Framework.Assert.AreEqual(testLength, 32768);
            PdfString longString = PdfACheckerTestUtils.GetLongString(testLength);
            String newContentString = PdfACheckerTestUtils.GetStreamWithValue(longString);
            byte[] newContent = newContentString.GetBytes(System.Text.Encoding.UTF8);
            PdfStream stream = new PdfStream(newContent);
            // An exception should be thrown as content stream has a string which
            // is longer then it is allowed per specification
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => pdfA2Checker.CheckContentStream
                (stream));
            NUnit.Framework.Assert.AreEqual(PdfaExceptionMessageConstant.PDF_STRING_IS_TOO_LONG, e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void ArrayCapacityHasNoLimitsTest() {
            PdfArray longArray = PdfACheckerTestUtils.GetLongArray(999999);
            // An exception should not be thrown as there is no limits for capacity of an array
            // in PDFA 2
            pdfA2Checker.CheckPdfObject(longArray);
        }

        [NUnit.Framework.Test]
        public virtual void DictionaryCapacityHasNoLimitsTest() {
            // Using 9999 dictionary pairs which is more than pdfA1 4095 limit (see PDF/A 4.3.2 Limits)
            PdfDictionary longDictionary = PdfACheckerTestUtils.GetLongDictionary(9999);
            // An exception should not be thrown as there is no limits for capacity of a dictionary
            // in PDFA 2
            pdfA2Checker.CheckPdfObject(longDictionary);
            // Using 9999 dictionary pairs which is more than pdfA1 4095 limit (see PDF/A 4.3.2 Limits)
            PdfStream longStream = PdfACheckerTestUtils.GetStreamWithLongDictionary(9999);
            // An exception should not be thrown as there is no limits for capacity of a dictionary
            // and stream in PDFA 2
            pdfA2Checker.CheckPdfObject(longStream);
        }

        [NUnit.Framework.Test]
        public virtual void IndependentLargeRealTest() {
            PdfNumber largeNumber = new PdfNumber(pdfA2Checker.GetMaxRealValue());
            // TODO DEVSIX-4182
            // An exception is thrown as any number greater then 32767 is considered as Integer
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => pdfA2Checker.CheckPdfObject
                (largeNumber));
            NUnit.Framework.Assert.AreEqual(PdfaExceptionMessageConstant.INTEGER_NUMBER_IS_OUT_OF_RANGE, e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void DeviceNColorspaceWithMoreThan32Components() {
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => CheckColorspace(BuildDeviceNColorspace
                (34)));
            NUnit.Framework.Assert.AreEqual(PdfaExceptionMessageConstant.THE_NUMBER_OF_COLOR_COMPONENTS_IN_DEVICE_N_COLORSPACE_SHOULD_NOT_EXCEED
                , e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void DeviceNColorspaceWithLessThan32Components() {
            CheckColorspace(BuildDeviceNColorspace(16));
        }

        [NUnit.Framework.Test]
        public virtual void DeviceNColorspaceWith32Components() {
            CheckColorspace(BuildDeviceNColorspace(32));
        }

        private void CheckColorspace(PdfColorSpace colorSpace) {
            PdfDictionary currentColorSpaces = new PdfDictionary();
            pdfA2Checker.CheckColorSpace(colorSpace, null, currentColorSpaces, true, false);
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
            //TODO DEVSIX-4205 Replace with a constructor with 4 parameters or use a setter for attributes dictionary
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
