/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 Apryse Group NV
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
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Colorspace;
using iText.Kernel.Pdf.Function;
using iText.Kernel.Pdf.Xobject;
using iText.Pdfa.Exceptions;
using iText.Test;

namespace iText.Pdfa.Checker {
    [NUnit.Framework.Category("UnitTest")]
    public class PdfA1ImplementationLimitsCheckerTest : ExtendedITextTest {
        private PdfA1Checker pdfA1Checker = new PdfA1Checker(PdfAConformanceLevel.PDF_A_1B);

        private const int MAX_ARRAY_CAPACITY = 8191;

        private const int MAX_DICTIONARY_CAPACITY = 4095;

        [NUnit.Framework.SetUp]
        public virtual void Before() {
            pdfA1Checker.SetFullCheckMode(true);
        }

        [NUnit.Framework.Test]
        public virtual void ValidObjectsTest() {
            int maxNameLength = pdfA1Checker.GetMaxNameLength();
            int maxStringLength = pdfA1Checker.GetMaxStringLength();
            int maxArrayCapacity = MAX_ARRAY_CAPACITY;
            int maxDictionaryCapacity = MAX_DICTIONARY_CAPACITY;
            long maxIntegerValue = pdfA1Checker.GetMaxIntegerValue();
            long minIntegerValue = pdfA1Checker.GetMinIntegerValue();
            double maxRealValue = pdfA1Checker.GetMaxRealValue();
            NUnit.Framework.Assert.AreEqual(65535, maxStringLength);
            NUnit.Framework.Assert.AreEqual(127, maxNameLength);
            PdfString longString = PdfACheckerTestUtils.GetLongString(maxStringLength);
            PdfName longName = PdfACheckerTestUtils.GetLongName(maxNameLength);
            PdfArray longArray = PdfACheckerTestUtils.GetLongArray(maxArrayCapacity);
            PdfDictionary longDictionary = PdfACheckerTestUtils.GetLongDictionary(maxDictionaryCapacity);
            NUnit.Framework.Assert.AreEqual(2147483647, maxIntegerValue);
            NUnit.Framework.Assert.AreEqual(-2147483648, minIntegerValue);
            NUnit.Framework.Assert.AreEqual(32767, maxRealValue, 0.001);
            PdfNumber largeInteger = new PdfNumber(maxIntegerValue);
            PdfNumber negativeInteger = new PdfNumber(minIntegerValue);
            PdfNumber largeReal = new PdfNumber(maxRealValue - 0.001);
            PdfObject[] largeObjects = new PdfObject[] { longName, longString, longArray, longDictionary, largeInteger
                , negativeInteger, largeReal };
            // No exceptions should not be thrown as all values match the
            // limitations provided in specification
            foreach (PdfObject largeObject in largeObjects) {
                pdfA1Checker.CheckPdfObject(largeObject);
                CheckInArray(largeObject);
                CheckInDictionary(largeObject);
                CheckInComplexStructure(largeObject);
                CheckInContentStream(largeObject);
                CheckInArrayInContentStream(largeObject);
                CheckInDictionaryInContentStream(largeObject);
                CheckInFormXObject(largeObject);
                CheckInTilingPattern(largeObject);
                CheckInType3Font(largeObject);
            }
        }

        [NUnit.Framework.Test]
        public virtual void ValidStreamTest() {
            PdfStream longStream = PdfACheckerTestUtils.GetStreamWithLongDictionary(MAX_DICTIONARY_CAPACITY);
            // No exceptions should not be thrown as the stream match the
            // limitations provided in specification
            pdfA1Checker.CheckPdfObject(longStream);
        }

        [NUnit.Framework.Test]
        public virtual void IndependentLongStringTest() {
            PdfString longString = BuildLongString();
            // An exception should be thrown as provided String is longer then
            // it is allowed per specification
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => pdfA1Checker.CheckPdfObject
                (longString));
            NUnit.Framework.Assert.AreEqual(PdfAConformanceException.PDF_STRING_IS_TOO_LONG, e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void IndependentLongNameTest() {
            PdfName longName = BuildLongName();
            // An exception should be thrown as provided name is longer then
            // it is allowed per specification
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => pdfA1Checker.CheckPdfObject
                (longName));
            NUnit.Framework.Assert.AreEqual(PdfAConformanceException.PDF_NAME_IS_TOO_LONG, e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void IndependentLargeIntegerTest() {
            PdfNumber largeNumber = new PdfNumber(pdfA1Checker.GetMaxIntegerValue() + 1L);
            // An exception should be thrown as provided integer is larger then
            // it is allowed per specification
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => pdfA1Checker.CheckPdfObject
                (largeNumber));
            NUnit.Framework.Assert.AreEqual(PdfAConformanceException.INTEGER_NUMBER_IS_OUT_OF_RANGE, e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void IndependentLargeNegativeIntegerTest() {
            PdfNumber largeNumber = new PdfNumber(pdfA1Checker.GetMinIntegerValue() - 1L);
            // An exception should be thrown as provided integer is smaller then
            // it is allowed per specification
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => pdfA1Checker.CheckPdfObject
                (largeNumber));
            NUnit.Framework.Assert.AreEqual(PdfAConformanceException.INTEGER_NUMBER_IS_OUT_OF_RANGE, e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void IndependentLargeRealTest() {
            PdfNumber largeNumber = new PdfNumber(pdfA1Checker.GetMaxRealValue() + 1.0);
            // TODO DEVSIX-4182
            // An exception is not thrown as any number greater then 32767 is considered as Integer
            pdfA1Checker.CheckPdfObject(largeNumber);
        }

        [NUnit.Framework.Test]
        public virtual void IndependentLongArrayTest() {
            PdfArray longArray = BuildLongArray();
            // An exception should be thrown as provided array has more elements then
            // it is allowed per specification
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => pdfA1Checker.CheckPdfObject
                (longArray));
            NUnit.Framework.Assert.AreEqual(PdfAConformanceException.MAXIMUM_ARRAY_CAPACITY_IS_EXCEEDED, e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void IndependentLongDictionaryTest() {
            PdfDictionary longDictionary = BuildLongDictionary();
            // An exception should be thrown as provided dictionary has more entries
            // then it is allowed per specification
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => pdfA1Checker.CheckPdfObject
                (longDictionary));
            NUnit.Framework.Assert.AreEqual(PdfAConformanceException.MAXIMUM_DICTIONARY_CAPACITY_IS_EXCEEDED, e.Message
                );
        }

        [NUnit.Framework.Test]
        public virtual void IndependentStreamWithLongDictionaryTest() {
            PdfStream longStream = BuildStreamWithLongDictionary();
            // An exception should be thrown as dictionary of the stream has more entries
            // then it is allowed per specification
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => pdfA1Checker.CheckPdfObject
                (longStream));
            NUnit.Framework.Assert.AreEqual(PdfAConformanceException.MAXIMUM_DICTIONARY_CAPACITY_IS_EXCEEDED, e.Message
                );
        }

        [NUnit.Framework.Test]
        public virtual void LongStringInDictionaryTest() {
            PdfString longString = BuildLongString();
            // An exception should be thrown as dictionary contains value which is longer then
            // it is allowed per specification
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => CheckInDictionary(longString
                ));
            NUnit.Framework.Assert.AreEqual(PdfAConformanceException.PDF_STRING_IS_TOO_LONG, e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void LongNameAsKeyInDictionaryTest() {
            PdfName longName = BuildLongName();
            PdfDictionary dict = new PdfDictionary();
            dict.Put(new PdfName("Key1"), new PdfString("value1"));
            dict.Put(new PdfName("Key2"), new PdfString("value2"));
            dict.Put(longName, new PdfString("value3"));
            // An exception should be thrown as dictionary contains key which is longer then
            // it is allowed per specification
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => pdfA1Checker.CheckPdfObject
                (dict));
            NUnit.Framework.Assert.AreEqual(PdfAConformanceException.PDF_NAME_IS_TOO_LONG, e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void LongStringInArrayTest() {
            PdfString longString = BuildLongString();
            // An exception should be thrown as one element is longer then
            // it is allowed per specification
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => CheckInArray(longString
                ));
            NUnit.Framework.Assert.AreEqual(PdfAConformanceException.PDF_STRING_IS_TOO_LONG, e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void LongStringInContentStreamTest() {
            PdfString longString = BuildLongString();
            // An exception should be thrown as content stream has a string which
            // is longer then it is allowed per specification
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => CheckInContentStream(longString
                ));
            NUnit.Framework.Assert.AreEqual(PdfAConformanceException.PDF_STRING_IS_TOO_LONG, e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void LongNameInContentStreamTest() {
            PdfName longName = BuildLongName();
            // An exception should be thrown as content stream has a name which
            // is longer then it is allowed per specification
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => CheckInContentStream(longName
                ));
            NUnit.Framework.Assert.AreEqual(PdfAConformanceException.PDF_NAME_IS_TOO_LONG, e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void LargeIntegerInContentStreamTest() {
            PdfNumber largeNumber = new PdfNumber(pdfA1Checker.GetMaxIntegerValue() + 1L);
            // An exception should be thrown as provided integer is larger then
            // it is allowed per specification
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => CheckInContentStream(largeNumber
                ));
            NUnit.Framework.Assert.AreEqual(PdfAConformanceException.INTEGER_NUMBER_IS_OUT_OF_RANGE, e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void LargeNegativeIntegerInContentStreamTest() {
            PdfNumber largeNumber = new PdfNumber(pdfA1Checker.GetMinIntegerValue() - 1L);
            // An exception should be thrown as provided integer is smaller then
            // it is allowed per specification
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => CheckInContentStream(largeNumber
                ));
            NUnit.Framework.Assert.AreEqual(PdfAConformanceException.INTEGER_NUMBER_IS_OUT_OF_RANGE, e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void LargeRealInContentStreamTest() {
            PdfNumber largeNumber = new PdfNumber(pdfA1Checker.GetMaxRealValue() + 1.0);
            // TODO DEVSIX-4182
            // An exception is not thrown as any number greater then 32767 is considered as Integer
            CheckInContentStream(largeNumber);
        }

        [NUnit.Framework.Test]
        public virtual void LongArrayInContentStreamTest() {
            PdfArray longArray = BuildLongArray();
            // An exception should be thrown as provided array has more elements then
            // it is allowed per specification
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => CheckInContentStream(longArray
                ));
            NUnit.Framework.Assert.AreEqual(PdfAConformanceException.MAXIMUM_ARRAY_CAPACITY_IS_EXCEEDED, e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void LongDictionaryInContentStream() {
            PdfDictionary longDictionary = BuildLongDictionary();
            // An exception should be thrown as provided dictionary has more entries
            // then it is allowed per specification
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => CheckInContentStream(longDictionary
                ));
            NUnit.Framework.Assert.AreEqual(PdfAConformanceException.MAXIMUM_DICTIONARY_CAPACITY_IS_EXCEEDED, e.Message
                );
        }

        [NUnit.Framework.Test]
        public virtual void ContentStreamIsNotCheckedForNotModifiedObjectTest() {
            pdfA1Checker.SetFullCheckMode(false);
            PdfString longString = BuildLongString();
            PdfArray longArray = BuildLongArray();
            PdfDictionary longDictionary = BuildLongDictionary();
            // An exception should not be thrown as content stream considered as not modified
            // and won't be tested
            CheckInContentStream(longString);
            CheckInContentStream(longArray);
            CheckInContentStream(longDictionary);
        }

        [NUnit.Framework.Test]
        public virtual void IndirectObjectIsNotCheckTest() {
            pdfA1Checker.SetFullCheckMode(false);
            PdfStream longStream = BuildStreamWithLongDictionary();
            // An exception should not be thrown as pdf stream is an indirect object
            // it is ignored during array / dictionary validation as it is expected
            // to be validated and flushed independently
            CheckInArray(longStream);
        }

        [NUnit.Framework.Test]
        public virtual void LongStringInArrayInContentStreamTest() {
            PdfString longString = BuildLongString();
            // An exception should be thrown as content stream has a string which
            // is longer then it is allowed per specification
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => CheckInArrayInContentStream
                (longString));
            NUnit.Framework.Assert.AreEqual(PdfAConformanceException.PDF_STRING_IS_TOO_LONG, e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void LongStringInDictionaryInContentStreamTest() {
            PdfString longString = BuildLongString();
            // An exception should be thrown as content stream has a string which
            // is longer then it is allowed per specification
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => CheckInDictionaryInContentStream
                (longString));
            NUnit.Framework.Assert.AreEqual(PdfAConformanceException.PDF_STRING_IS_TOO_LONG, e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void LongNameAsKeyInDictionaryInContentStreamTest() {
            PdfName longName = BuildLongName();
            PdfDictionary dict = new PdfDictionary();
            dict.Put(new PdfName("Key1"), new PdfString("value1"));
            dict.Put(new PdfName("Key2"), new PdfString("value2"));
            dict.Put(longName, new PdfString("value3"));
            // An exception should be thrown as content stream has a string which
            // is longer then it is allowed per specification
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => CheckInContentStream(dict
                ));
            NUnit.Framework.Assert.AreEqual(PdfAConformanceException.PDF_NAME_IS_TOO_LONG, e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void LongStringInComplexStructureTest() {
            PdfString longString = BuildLongString();
            // An exception should be thrown as there is a string element which
            // doesn't match the limitations provided in specification
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => CheckInComplexStructure
                (longString));
            NUnit.Framework.Assert.AreEqual(PdfAConformanceException.PDF_STRING_IS_TOO_LONG, e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void LongArrayInComplexStructureTest() {
            PdfArray longArray = BuildLongArray();
            // An exception should be thrown as provided array has more elements then
            // it is allowed per specification
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => CheckInComplexStructure
                (longArray));
            NUnit.Framework.Assert.AreEqual(PdfAConformanceException.MAXIMUM_ARRAY_CAPACITY_IS_EXCEEDED, e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void LongDictionaryInComplexStructureTest() {
            PdfDictionary longDictionary = BuildLongDictionary();
            // An exception should be thrown as provided dictionary has more entries
            // then it is allowed per specification
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => CheckInComplexStructure
                (longDictionary));
            NUnit.Framework.Assert.AreEqual(PdfAConformanceException.MAXIMUM_DICTIONARY_CAPACITY_IS_EXCEEDED, e.Message
                );
        }

        [NUnit.Framework.Test]
        public virtual void LongStringInPdfFormXObjectTest() {
            PdfString longString = BuildLongString();
            // An exception should be thrown as form xobject content stream has a string which
            // is longer then it is allowed per specification
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => CheckInFormXObject(longString
                ));
            NUnit.Framework.Assert.AreEqual(PdfAConformanceException.PDF_STRING_IS_TOO_LONG, e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void LongStringInTilingPatternTest() {
            PdfString longString = BuildLongString();
            // An exception should be thrown as tiling pattern's content stream has a string which
            // is longer then it is allowed per specification
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => CheckInTilingPattern(longString
                ));
            NUnit.Framework.Assert.AreEqual(PdfAConformanceException.PDF_STRING_IS_TOO_LONG, e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void LongStringInShadingPatternTest() {
            PdfString longString = BuildLongString();
            // An exception should not be thrown as shading pattern doesn't have
            // content stream to validate
            CheckInShadingPattern(longString);
        }

        [NUnit.Framework.Test]
        public virtual void LongStringInType3FontTest() {
            PdfString longString = BuildLongString();
            // An exception should be thrown as content stream of type3 font has a string which
            // is longer then it is allowed per specification
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => CheckInType3Font(longString
                ));
            NUnit.Framework.Assert.AreEqual(PdfAConformanceException.PDF_STRING_IS_TOO_LONG, e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void DeviceNColorspaceWithMoreThan8Components() {
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => CheckColorspace(BuildDeviceNColorspace
                (10)));
            NUnit.Framework.Assert.AreEqual(PdfAConformanceException.THE_NUMBER_OF_COLOR_COMPONENTS_IN_DEVICE_N_COLORSPACE_SHOULD_NOT_EXCEED
                , e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void DeviceNColorspaceWith8Components() {
            CheckColorspace(BuildDeviceNColorspace(8));
        }

        [NUnit.Framework.Test]
        public virtual void DeviceNColorspaceWithLessThan8Components() {
            CheckColorspace(BuildDeviceNColorspace(2));
        }

        private PdfString BuildLongString() {
            int maxAllowedLength = pdfA1Checker.GetMaxStringLength();
            int testLength = maxAllowedLength + 1;
            NUnit.Framework.Assert.AreEqual(65536, testLength);
            return PdfACheckerTestUtils.GetLongString(testLength);
        }

        private PdfName BuildLongName() {
            int maxAllowedLength = pdfA1Checker.GetMaxNameLength();
            int testLength = maxAllowedLength + 1;
            NUnit.Framework.Assert.AreEqual(128, testLength);
            return PdfACheckerTestUtils.GetLongName(testLength);
        }

        private PdfArray BuildLongArray() {
            int testLength = MAX_ARRAY_CAPACITY + 1;
            return PdfACheckerTestUtils.GetLongArray(testLength);
        }

        private PdfDictionary BuildLongDictionary() {
            int testLength = MAX_DICTIONARY_CAPACITY + 1;
            return PdfACheckerTestUtils.GetLongDictionary(testLength);
        }

        private PdfStream BuildStreamWithLongDictionary() {
            int testLength = MAX_DICTIONARY_CAPACITY + 1;
            return PdfACheckerTestUtils.GetStreamWithLongDictionary(testLength);
        }

        private void CheckInDictionary(PdfObject @object) {
            PdfDictionary dict = new PdfDictionary();
            dict.Put(new PdfName("Key1"), new PdfString("value1"));
            dict.Put(new PdfName("Key2"), new PdfString("value2"));
            dict.Put(new PdfName("Key3"), @object);
            pdfA1Checker.CheckPdfObject(dict);
        }

        private void CheckInArray(PdfObject @object) {
            PdfArray array = new PdfArray();
            array.Add(new PdfString("value1"));
            array.Add(new PdfString("value2"));
            array.Add(@object);
            pdfA1Checker.CheckPdfObject(array);
        }

        private void CheckInContentStream(PdfObject @object) {
            String byteContent = PdfACheckerTestUtils.GetStreamWithValue(@object);
            byte[] newContent = byteContent.GetBytes(System.Text.Encoding.UTF8);
            PdfStream stream = new PdfStream(newContent);
            pdfA1Checker.CheckContentStream(stream);
        }

        private void CheckInArrayInContentStream(PdfObject @object) {
            CheckInContentStream(new PdfArray(@object));
        }

        private void CheckInDictionaryInContentStream(PdfObject @object) {
            PdfDictionary dict = new PdfDictionary();
            dict.Put(new PdfName("value"), @object);
            CheckInContentStream(dict);
        }

        private void CheckInComplexStructure(PdfObject @object) {
            PdfDictionary dict1 = new PdfDictionary();
            dict1.Put(new PdfName("Key1"), new PdfString("value1"));
            dict1.Put(new PdfName("Key2"), new PdfString("value2"));
            dict1.Put(new PdfName("Key3"), @object);
            PdfArray array = new PdfArray();
            array.Add(new PdfString("value3"));
            array.Add(new PdfString("value4"));
            array.Add(dict1);
            PdfDictionary dict = new PdfDictionary();
            dict.Put(new PdfName("Key4"), new PdfString("value5"));
            dict.Put(new PdfName("Key5"), new PdfString("value6"));
            dict.Put(new PdfName("Key6"), array);
            pdfA1Checker.CheckPdfObject(array);
        }

        private void CheckInFormXObject(PdfObject @object) {
            String newContentString = PdfACheckerTestUtils.GetStreamWithValue(@object);
            byte[] newContent = newContentString.GetBytes(System.Text.Encoding.UTF8);
            PdfStream stream = new PdfStream(newContent);
            PdfXObject xobject = new PdfFormXObject(stream);
            pdfA1Checker.CheckFormXObject(xobject.GetPdfObject());
        }

        private void CheckInTilingPattern(PdfObject @object) {
            String newContentString = PdfACheckerTestUtils.GetStreamWithValue(@object);
            byte[] newContent = newContentString.GetBytes(System.Text.Encoding.UTF8);
            PdfPattern pattern = new PdfPattern.Tiling(200, 200);
            ((PdfStream)pattern.GetPdfObject()).SetData(newContent);
            Color color = new PatternColor(pattern);
            pdfA1Checker.CheckColor(color, new PdfDictionary(), true, null);
        }

        private void CheckInShadingPattern(PdfObject @object) {
            String newContentString = PdfACheckerTestUtils.GetStreamWithValue(@object);
            byte[] newContent = newContentString.GetBytes(System.Text.Encoding.UTF8);
            PdfStream stream = new PdfStream(newContent);
            PdfPattern pattern = new PdfPattern.Shading(stream);
            pdfA1Checker.CheckPdfObject(pattern.GetPdfObject());
        }

        private void CheckInType3Font(PdfObject @object) {
            String newContentString = PdfACheckerTestUtils.GetStreamWithValue(@object);
            byte[] newContent = newContentString.GetBytes(System.Text.Encoding.UTF8);
            PdfFont font = PdfFontFactory.CreateType3Font(null, true);
            PdfDictionary charProcs = new PdfDictionary();
            charProcs.Put(PdfName.A, new PdfStream(newContent));
            PdfDictionary dictionary = font.GetPdfObject();
            dictionary.Put(PdfName.Subtype, PdfName.Type3);
            dictionary.Put(PdfName.CharProcs, charProcs);
            pdfA1Checker.CheckFont(font);
        }

        private void CheckColorspace(PdfColorSpace colorSpace) {
            PdfDictionary currentColorSpaces = new PdfDictionary();
            pdfA1Checker.CheckColorSpace(colorSpace, currentColorSpaces, false, false);
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
            return new PdfSpecialCs.DeviceN(tmpArray, new PdfDeviceCs.Rgb(), function);
        }
    }
}
