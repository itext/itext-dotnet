/*
This file is part of the iText (R) project.
Copyright (c) 1998-2020 iText Group NV
Authors: iText Software.

This program is free software; you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License version 3
as published by the Free Software Foundation with the addition of the
following permission added to Section 15 as permitted in Section 7(a):
FOR ANY PART OF THE COVERED WORK IN WHICH THE COPYRIGHT IS OWNED BY
ITEXT GROUP. ITEXT GROUP DISCLAIMS THE WARRANTY OF NON INFRINGEMENT
OF THIRD PARTY RIGHTS

This program is distributed in the hope that it will be useful, but
WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
or FITNESS FOR A PARTICULAR PURPOSE.
See the GNU Affero General Public License for more details.
You should have received a copy of the GNU Affero General Public License
along with this program; if not, see http://www.gnu.org/licenses or write to
the Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor,
Boston, MA, 02110-1301 USA, or download the license from the following URL:
http://itextpdf.com/terms-of-use/

The interactive user interfaces in modified source and object code versions
of this program must display Appropriate Legal Notices, as required under
Section 5 of the GNU Affero General Public License.

In accordance with Section 7(b) of the GNU Affero General Public License,
a covered work must retain the producer line in every PDF that is created
or manipulated using iText.

You can be released from the requirements of the license by purchasing
a commercial license. Buying such a license is mandatory as soon as you
develop commercial activities involving the iText software without
disclosing the source code of your own applications.
These activities include: offering paid services to customers as an ASP,
serving PDFs on the fly in a web application, shipping iText with a closed
source product.

For more information, please contact iText Software Corp. at this
address: sales@itextpdf.com
*/
using System;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Colorspace;
using iText.Kernel.Pdf.Xobject;
using iText.Pdfa;
using iText.Test;

namespace iText.Pdfa.Checker {
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
            int maxStringLength = pdfA1Checker.GetMaxStringLength();
            int maxArrayCapacity = MAX_ARRAY_CAPACITY;
            int maxDictionaryCapacity = MAX_DICTIONARY_CAPACITY;
            NUnit.Framework.Assert.AreEqual(maxStringLength, 65535);
            PdfString longString = PdfACheckerTestUtils.GetLongString(maxStringLength);
            PdfArray longArray = PdfACheckerTestUtils.GetLongArray(maxArrayCapacity);
            PdfDictionary longDictionary = PdfACheckerTestUtils.GetLongDictionary(maxDictionaryCapacity);
            PdfObject[] longObjects = new PdfObject[] { longString, longArray, longDictionary };
            // No exceptions should not be thrown as all values match the
            // limitations provided in specification
            foreach (PdfObject longObject in longObjects) {
                pdfA1Checker.CheckPdfObject(longObject);
                CheckInArray(longObject);
                CheckInDictionary(longObject);
                CheckInComplexStructure(longObject);
                CheckInContentStream(longObject);
                CheckInArrayInContentStream(longObject);
                CheckInDictionaryInContentStream(longObject);
                CheckInFormXObject(longObject);
                CheckInTilingPattern(longObject);
                CheckInType3Font(longObject);
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
            NUnit.Framework.Assert.That(() =>  {
                PdfString longString = BuildLongString();
                // An exception should be thrown as provided String is longer then
                // it is allowed per specification
                pdfA1Checker.CheckPdfObject(longString);
            }
            , NUnit.Framework.Throws.InstanceOf<PdfAConformanceException>().With.Message.EqualTo(PdfAConformanceException.PDF_STRING_IS_TOO_LONG))
;
        }

        [NUnit.Framework.Test]
        public virtual void IndependentLongArrayTest() {
            NUnit.Framework.Assert.That(() =>  {
                PdfArray longArray = BuildLongArray();
                // An exception should be thrown as provided array has more elements then
                // it is allowed per specification
                pdfA1Checker.CheckPdfObject(longArray);
            }
            , NUnit.Framework.Throws.InstanceOf<PdfAConformanceException>().With.Message.EqualTo(PdfAConformanceException.MAXIMUM_ARRAY_CAPACITY_IS_EXCEEDED))
;
        }

        [NUnit.Framework.Test]
        public virtual void IndependentLongDictionaryTest() {
            NUnit.Framework.Assert.That(() =>  {
                PdfDictionary longDictionary = BuildLongDictionary();
                // An exception should be thrown as provided dictionary has more entries
                // then it is allowed per specification
                pdfA1Checker.CheckPdfObject(longDictionary);
            }
            , NUnit.Framework.Throws.InstanceOf<PdfAConformanceException>().With.Message.EqualTo(PdfAConformanceException.MAXIMUM_DICTIONARY_CAPACITY_IS_EXCEEDED))
;
        }

        [NUnit.Framework.Test]
        public virtual void IndependentStreamWithLongDictionaryTest() {
            NUnit.Framework.Assert.That(() =>  {
                PdfStream longStream = BuildStreamWithLongDictionary();
                // An exception should be thrown as dictionary of the stream has more entries
                // then it is allowed per specification
                pdfA1Checker.CheckPdfObject(longStream);
            }
            , NUnit.Framework.Throws.InstanceOf<PdfAConformanceException>().With.Message.EqualTo(PdfAConformanceException.MAXIMUM_DICTIONARY_CAPACITY_IS_EXCEEDED))
;
        }

        [NUnit.Framework.Test]
        public virtual void LongStringInDictionaryTest() {
            NUnit.Framework.Assert.That(() =>  {
                PdfString longString = BuildLongString();
                // An exception should be thrown as dictionary contains value which is longer then
                // it is allowed per specification
                CheckInDictionary(longString);
            }
            , NUnit.Framework.Throws.InstanceOf<PdfAConformanceException>().With.Message.EqualTo(PdfAConformanceException.PDF_STRING_IS_TOO_LONG))
;
        }

        [NUnit.Framework.Test]
        public virtual void LongStringInArrayTest() {
            NUnit.Framework.Assert.That(() =>  {
                PdfString longString = BuildLongString();
                // An exception should be thrown as one element is longer then
                // it is allowed per specification
                CheckInArray(longString);
            }
            , NUnit.Framework.Throws.InstanceOf<PdfAConformanceException>().With.Message.EqualTo(PdfAConformanceException.PDF_STRING_IS_TOO_LONG))
;
        }

        [NUnit.Framework.Test]
        public virtual void LongStringInContentStreamTest() {
            NUnit.Framework.Assert.That(() =>  {
                PdfString longString = BuildLongString();
                // An exception should be thrown as content stream has a string which
                // is longer then it is allowed per specification
                CheckInContentStream(longString);
            }
            , NUnit.Framework.Throws.InstanceOf<PdfAConformanceException>().With.Message.EqualTo(PdfAConformanceException.PDF_STRING_IS_TOO_LONG))
;
        }

        [NUnit.Framework.Test]
        public virtual void LongArrayInContentStreamTest() {
            NUnit.Framework.Assert.That(() =>  {
                PdfArray longArray = BuildLongArray();
                // An exception should be thrown as provided array has more elements then
                // it is allowed per specification
                CheckInContentStream(longArray);
            }
            , NUnit.Framework.Throws.InstanceOf<PdfAConformanceException>().With.Message.EqualTo(PdfAConformanceException.MAXIMUM_ARRAY_CAPACITY_IS_EXCEEDED))
;
        }

        [NUnit.Framework.Test]
        public virtual void LongDictionaryInContentStream() {
            NUnit.Framework.Assert.That(() =>  {
                PdfDictionary longDictionary = BuildLongDictionary();
                // An exception should be thrown as provided dictionary has more entries
                // then it is allowed per specification
                CheckInContentStream(longDictionary);
            }
            , NUnit.Framework.Throws.InstanceOf<PdfAConformanceException>().With.Message.EqualTo(PdfAConformanceException.MAXIMUM_DICTIONARY_CAPACITY_IS_EXCEEDED))
;
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
            NUnit.Framework.Assert.That(() =>  {
                PdfString longString = BuildLongString();
                // An exception should be thrown as content stream has a string which
                // is longer then it is allowed per specification
                CheckInArrayInContentStream(longString);
            }
            , NUnit.Framework.Throws.InstanceOf<PdfAConformanceException>().With.Message.EqualTo(PdfAConformanceException.PDF_STRING_IS_TOO_LONG))
;
        }

        [NUnit.Framework.Test]
        public virtual void LongStringInDictionaryInContentStreamTest() {
            NUnit.Framework.Assert.That(() =>  {
                PdfString longString = BuildLongString();
                // An exception should be thrown as content stream has a string which
                // is longer then it is allowed per specification
                CheckInDictionaryInContentStream(longString);
            }
            , NUnit.Framework.Throws.InstanceOf<PdfAConformanceException>().With.Message.EqualTo(PdfAConformanceException.PDF_STRING_IS_TOO_LONG))
;
        }

        [NUnit.Framework.Test]
        public virtual void LongStringInComplexStructureTest() {
            NUnit.Framework.Assert.That(() =>  {
                PdfString longString = BuildLongString();
                // An exception should be thrown as there is a string element which
                // doesn't match the limitations provided in specification
                CheckInComplexStructure(longString);
            }
            , NUnit.Framework.Throws.InstanceOf<PdfAConformanceException>().With.Message.EqualTo(PdfAConformanceException.PDF_STRING_IS_TOO_LONG))
;
        }

        [NUnit.Framework.Test]
        public virtual void LongArrayInComplexStructureTest() {
            NUnit.Framework.Assert.That(() =>  {
                PdfArray longArray = BuildLongArray();
                // An exception should be thrown as provided array has more elements then
                // it is allowed per specification
                CheckInComplexStructure(longArray);
            }
            , NUnit.Framework.Throws.InstanceOf<PdfAConformanceException>().With.Message.EqualTo(PdfAConformanceException.MAXIMUM_ARRAY_CAPACITY_IS_EXCEEDED))
;
        }

        [NUnit.Framework.Test]
        public virtual void LongDictionaryInComplexStructureTest() {
            NUnit.Framework.Assert.That(() =>  {
                PdfDictionary longDictionary = BuildLongDictionary();
                // An exception should be thrown as provided dictionary has more entries
                // then it is allowed per specification
                CheckInComplexStructure(longDictionary);
            }
            , NUnit.Framework.Throws.InstanceOf<PdfAConformanceException>().With.Message.EqualTo(PdfAConformanceException.MAXIMUM_DICTIONARY_CAPACITY_IS_EXCEEDED))
;
        }

        [NUnit.Framework.Test]
        public virtual void LongStringInPdfFormXObjectTest() {
            NUnit.Framework.Assert.That(() =>  {
                PdfString longString = BuildLongString();
                // An exception should be thrown as form xobject content stream has a string which
                // is longer then it is allowed per specification
                CheckInFormXObject(longString);
            }
            , NUnit.Framework.Throws.InstanceOf<PdfAConformanceException>().With.Message.EqualTo(PdfAConformanceException.PDF_STRING_IS_TOO_LONG))
;
        }

        [NUnit.Framework.Test]
        public virtual void LongStringInTilingPatternTest() {
            NUnit.Framework.Assert.That(() =>  {
                PdfString longString = BuildLongString();
                // An exception should be thrown as tiling pattern's content stream has a string which
                // is longer then it is allowed per specification
                CheckInTilingPattern(longString);
            }
            , NUnit.Framework.Throws.InstanceOf<PdfAConformanceException>().With.Message.EqualTo(PdfAConformanceException.PDF_STRING_IS_TOO_LONG))
;
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
            NUnit.Framework.Assert.That(() =>  {
                PdfString longString = BuildLongString();
                // An exception should be thrown as content stream of type3 font has a string which
                // is longer then it is allowed per specification
                CheckInType3Font(longString);
            }
            , NUnit.Framework.Throws.InstanceOf<PdfAConformanceException>().With.Message.EqualTo(PdfAConformanceException.PDF_STRING_IS_TOO_LONG))
;
        }

        private PdfString BuildLongString() {
            int maxAllowedLength = pdfA1Checker.GetMaxStringLength();
            int testLength = maxAllowedLength + 1;
            NUnit.Framework.Assert.AreEqual(testLength, 65536);
            return PdfACheckerTestUtils.GetLongString(testLength);
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
    }
}
