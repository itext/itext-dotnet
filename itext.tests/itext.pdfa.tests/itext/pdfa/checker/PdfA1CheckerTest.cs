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
    public class PdfA1CheckerTest : ExtendedITextTest {
        private PdfA1Checker pdfA1Checker = new PdfA1Checker(PdfAConformanceLevel.PDF_A_1B);

        [NUnit.Framework.SetUp]
        public virtual void Before() {
            pdfA1Checker.SetFullCheckMode(true);
        }

        [NUnit.Framework.Test]
        public virtual void CheckCatalogDictionaryWithoutAAEntry() {
            NUnit.Framework.Assert.That(() =>  {
                PdfDictionary catalog = new PdfDictionary();
                catalog.Put(PdfName.AA, new PdfDictionary());
                pdfA1Checker.CheckCatalogValidEntries(catalog);
            }
            , NUnit.Framework.Throws.InstanceOf<PdfAConformanceException>().With.Message.EqualTo(PdfAConformanceException.A_CATALOG_DICTIONARY_SHALL_NOT_CONTAIN_AA_ENTRY))
;
        }

        [NUnit.Framework.Test]
        public virtual void CheckCatalogDictionaryWithoutOCPropertiesEntry() {
            NUnit.Framework.Assert.That(() =>  {
                PdfDictionary catalog = new PdfDictionary();
                catalog.Put(PdfName.OCProperties, new PdfDictionary());
                pdfA1Checker.CheckCatalogValidEntries(catalog);
            }
            , NUnit.Framework.Throws.InstanceOf<PdfAConformanceException>().With.Message.EqualTo(PdfAConformanceException.A_CATALOG_DICTIONARY_SHALL_NOT_CONTAIN_OCPROPERTIES_KEY))
;
        }

        [NUnit.Framework.Test]
        public virtual void CheckCatalogDictionaryWithoutEmbeddedFiles() {
            NUnit.Framework.Assert.That(() =>  {
                PdfDictionary names = new PdfDictionary();
                names.Put(PdfName.EmbeddedFiles, new PdfDictionary());
                PdfDictionary catalog = new PdfDictionary();
                catalog.Put(PdfName.Names, names);
                pdfA1Checker.CheckCatalogValidEntries(catalog);
            }
            , NUnit.Framework.Throws.InstanceOf<PdfAConformanceException>().With.Message.EqualTo(PdfAConformanceException.A_NAME_DICTIONARY_SHALL_NOT_CONTAIN_THE_EMBEDDED_FILES_KEY))
;
        }

        [NUnit.Framework.Test]
        public virtual void CheckValidCatalog() {
            pdfA1Checker.CheckCatalogValidEntries(new PdfDictionary());
        }

        // checkCatalogValidEntries doesn't change the state of any object
        // and doesn't return any value. The only result is exception which
        // was or wasn't thrown. Successful scenario is tested here therefore
        // no assertion is provided
        [NUnit.Framework.Test]
        public virtual void IndependentLongStringTest() {
            NUnit.Framework.Assert.That(() =>  {
                int maxAllowedLength = pdfA1Checker.GetMaxStringLength();
                int testLength = maxAllowedLength + 1;
                NUnit.Framework.Assert.AreEqual(testLength, 65536);
                PdfString longString = new PdfString(PdfACheckerTestUtils.GetLongString(testLength));
                // An exception should be thrown as provided String is longer then
                // it is allowed per specification
                pdfA1Checker.CheckPdfObject(longString);
            }
            , NUnit.Framework.Throws.InstanceOf<PdfAConformanceException>().With.Message.EqualTo(PdfAConformanceException.PDF_STRING_IS_TOO_LONG))
;
        }

        [NUnit.Framework.Test]
        public virtual void IndependentNormalStringTest() {
            int testLength = pdfA1Checker.GetMaxStringLength();
            NUnit.Framework.Assert.AreEqual(testLength, 65535);
            PdfString longString = new PdfString(PdfACheckerTestUtils.GetLongString(testLength));
            // An exception should not be thrown as provided String matches
            // the limitations provided in specification
            pdfA1Checker.CheckPdfObject(longString);
        }

        [NUnit.Framework.Test]
        public virtual void LongStringInDictionaryTest() {
            NUnit.Framework.Assert.That(() =>  {
                int maxAllowedLength = pdfA1Checker.GetMaxStringLength();
                int testLength = maxAllowedLength + 1;
                NUnit.Framework.Assert.AreEqual(testLength, 65536);
                PdfDictionary dict = new PdfDictionary();
                dict.Put(new PdfName("Key1"), new PdfString("value1"));
                dict.Put(new PdfName("Key2"), new PdfString("value2"));
                dict.Put(new PdfName("Key3"), new PdfString(PdfACheckerTestUtils.GetLongString(testLength)));
                // An exception should be thrown as value for 'key3' is longer then
                // it is allowed per specification
                pdfA1Checker.CheckPdfObject(dict);
            }
            , NUnit.Framework.Throws.InstanceOf<PdfAConformanceException>().With.Message.EqualTo(PdfAConformanceException.PDF_STRING_IS_TOO_LONG))
;
        }

        [NUnit.Framework.Test]
        public virtual void NormalStringInDictionaryTest() {
            int testLength = pdfA1Checker.GetMaxStringLength();
            PdfDictionary dict = new PdfDictionary();
            dict.Put(new PdfName("Key1"), new PdfString("value1"));
            dict.Put(new PdfName("Key2"), new PdfString("value2"));
            dict.Put(new PdfName("Key3"), new PdfString(PdfACheckerTestUtils.GetLongString(testLength)));
            // An exception should not be thrown as all values match the
            // limitations provided in specification
            pdfA1Checker.CheckPdfObject(dict);
        }

        [NUnit.Framework.Test]
        public virtual void LongStringInArrayTest() {
            NUnit.Framework.Assert.That(() =>  {
                int maxAllowedLength = pdfA1Checker.GetMaxStringLength();
                int testLength = maxAllowedLength + 1;
                NUnit.Framework.Assert.AreEqual(testLength, 65536);
                PdfArray array = new PdfArray();
                array.Add(new PdfString("value1"));
                array.Add(new PdfString("value2"));
                array.Add(new PdfString(PdfACheckerTestUtils.GetLongString(testLength)));
                // An exception should be thrown as 3rd element is longer then
                // it is allowed per specification
                pdfA1Checker.CheckPdfObject(array);
            }
            , NUnit.Framework.Throws.InstanceOf<PdfAConformanceException>().With.Message.EqualTo(PdfAConformanceException.PDF_STRING_IS_TOO_LONG))
;
        }

        [NUnit.Framework.Test]
        public virtual void NormalStringInArrayTest() {
            int testLength = pdfA1Checker.GetMaxStringLength();
            NUnit.Framework.Assert.AreEqual(testLength, 65535);
            PdfArray array = new PdfArray();
            array.Add(new PdfString("value1"));
            array.Add(new PdfString("value2"));
            array.Add(new PdfString(PdfACheckerTestUtils.GetLongString(testLength)));
            // An exception should not be thrown as all elements match the
            // limitations provided in specification
            pdfA1Checker.CheckPdfObject(array);
        }

        [NUnit.Framework.Test]
        public virtual void LongStringInContentStreamTest() {
            NUnit.Framework.Assert.That(() =>  {
                int maxAllowedLength = pdfA1Checker.GetMaxStringLength();
                int testLength = maxAllowedLength + 1;
                NUnit.Framework.Assert.AreEqual(testLength, 65536);
                String newContentString = PdfACheckerTestUtils.GetStreamWithLongString(testLength);
                byte[] newContent = newContentString.GetBytes(System.Text.Encoding.UTF8);
                PdfStream stream = new PdfStream(newContent);
                // An exception should be thrown as content stream has a string which
                // is longer then it is allowed per specification
                pdfA1Checker.CheckContentStream(stream);
            }
            , NUnit.Framework.Throws.InstanceOf<PdfAConformanceException>().With.Message.EqualTo(PdfAConformanceException.PDF_STRING_IS_TOO_LONG))
;
        }

        [NUnit.Framework.Test]
        public virtual void ContentStreamIsNotCheckedForNotModifiedObjectTest() {
            pdfA1Checker.SetFullCheckMode(false);
            int maxAllowedLength = pdfA1Checker.GetMaxStringLength();
            int testLength = maxAllowedLength + 1;
            NUnit.Framework.Assert.AreEqual(testLength, 65536);
            String newContentString = PdfACheckerTestUtils.GetStreamWithLongString(testLength);
            byte[] newContent = newContentString.GetBytes(System.Text.Encoding.UTF8);
            PdfStream stream = new PdfStream(newContent);
            // An exception should not be thrown as content stream considered as not modified
            // and won't be tested
            pdfA1Checker.CheckContentStream(stream);
        }

        [NUnit.Framework.Test]
        public virtual void NormalStringInContentStreamTest() {
            int testLength = pdfA1Checker.GetMaxStringLength();
            NUnit.Framework.Assert.AreEqual(testLength, 65535);
            String newContentString = PdfACheckerTestUtils.GetStreamWithLongString(testLength);
            byte[] newContent = newContentString.GetBytes(System.Text.Encoding.UTF8);
            PdfStream stream = new PdfStream(newContent);
            // An exception should be thrown as  all strings inside content stream
            // are not longer then it is allowed per specification
            pdfA1Checker.CheckContentStream(stream);
        }

        [NUnit.Framework.Test]
        public virtual void LongStringInArrayInContentStreamTest() {
            NUnit.Framework.Assert.That(() =>  {
                int maxAllowedLength = pdfA1Checker.GetMaxStringLength();
                int testLength = maxAllowedLength + 1;
                NUnit.Framework.Assert.AreEqual(testLength, 65536);
                String newContentString = PdfACheckerTestUtils.GetStreamWithLongStringInArray(testLength);
                byte[] newContent = newContentString.GetBytes(System.Text.Encoding.UTF8);
                PdfStream stream = new PdfStream(newContent);
                // An exception should be thrown as content stream has a string which
                // is longer then it is allowed per specification
                pdfA1Checker.CheckContentStream(stream);
            }
            , NUnit.Framework.Throws.InstanceOf<PdfAConformanceException>().With.Message.EqualTo(PdfAConformanceException.PDF_STRING_IS_TOO_LONG))
;
        }

        [NUnit.Framework.Test]
        public virtual void LongStringInDictionaryInContentStreamTest() {
            NUnit.Framework.Assert.That(() =>  {
                int maxAllowedLength = pdfA1Checker.GetMaxStringLength();
                int testLength = maxAllowedLength + 1;
                NUnit.Framework.Assert.AreEqual(testLength, 65536);
                String newContentString = PdfACheckerTestUtils.GetStreamWithLongStringInDictionary(testLength);
                byte[] newContent = newContentString.GetBytes(System.Text.Encoding.UTF8);
                PdfStream stream = new PdfStream(newContent);
                // An exception should be thrown as content stream has a string which
                // is longer then it is allowed per specification
                pdfA1Checker.CheckContentStream(stream);
            }
            , NUnit.Framework.Throws.InstanceOf<PdfAConformanceException>().With.Message.EqualTo(PdfAConformanceException.PDF_STRING_IS_TOO_LONG))
;
        }

        [NUnit.Framework.Test]
        public virtual void LongStringInComplexStructureTest() {
            NUnit.Framework.Assert.That(() =>  {
                int maxAllowedLength = pdfA1Checker.GetMaxStringLength();
                int testLength = maxAllowedLength + 1;
                NUnit.Framework.Assert.AreEqual(testLength, 65536);
                PdfDictionary dict1 = new PdfDictionary();
                dict1.Put(new PdfName("Key1"), new PdfString("value1"));
                dict1.Put(new PdfName("Key2"), new PdfString("value2"));
                dict1.Put(new PdfName("Key3"), new PdfString(PdfACheckerTestUtils.GetLongString(testLength)));
                PdfArray array = new PdfArray();
                array.Add(new PdfString("value3"));
                array.Add(new PdfString("value4"));
                array.Add(dict1);
                PdfDictionary dict = new PdfDictionary();
                dict.Put(new PdfName("Key4"), new PdfString("value5"));
                dict.Put(new PdfName("Key5"), new PdfString("value6"));
                dict.Put(new PdfName("Key6"), array);
                // An exception should be thrown as there is a string element which
                // doesn't match the limitations provided in specification
                pdfA1Checker.CheckPdfObject(array);
            }
            , NUnit.Framework.Throws.InstanceOf<PdfAConformanceException>().With.Message.EqualTo(PdfAConformanceException.PDF_STRING_IS_TOO_LONG))
;
        }

        [NUnit.Framework.Test]
        public virtual void LongStringInPdfFormXObjectTest() {
            NUnit.Framework.Assert.That(() =>  {
                int maxAllowedLength = pdfA1Checker.GetMaxStringLength();
                int testLength = maxAllowedLength + 1;
                NUnit.Framework.Assert.AreEqual(testLength, 65536);
                String newContentString = PdfACheckerTestUtils.GetStreamWithLongString(testLength);
                byte[] newContent = newContentString.GetBytes(System.Text.Encoding.UTF8);
                PdfStream stream = new PdfStream(newContent);
                PdfXObject xobject = new PdfFormXObject(stream);
                // An exception should be thrown as form xobject content stream has a string which
                // is longer then it is allowed per specification
                pdfA1Checker.CheckFormXObject(xobject.GetPdfObject());
            }
            , NUnit.Framework.Throws.InstanceOf<PdfAConformanceException>().With.Message.EqualTo(PdfAConformanceException.PDF_STRING_IS_TOO_LONG))
;
        }

        [NUnit.Framework.Test]
        public virtual void LongStringInTilingPatternTest() {
            NUnit.Framework.Assert.That(() =>  {
                int maxAllowedLength = pdfA1Checker.GetMaxStringLength();
                int testLength = maxAllowedLength + 1;
                NUnit.Framework.Assert.AreEqual(testLength, 65536);
                String newContentString = PdfACheckerTestUtils.GetStreamWithLongString(testLength);
                byte[] newContent = newContentString.GetBytes(System.Text.Encoding.UTF8);
                PdfPattern pattern = new PdfPattern.Tiling(200, 200);
                ((PdfStream)pattern.GetPdfObject()).SetData(newContent);
                Color color = new PatternColor(pattern);
                // An exception should be thrown as tiling pattern's content stream has a string which
                // is longer then it is allowed per specification
                pdfA1Checker.CheckColor(color, new PdfDictionary(), true, null);
            }
            , NUnit.Framework.Throws.InstanceOf<PdfAConformanceException>().With.Message.EqualTo(PdfAConformanceException.PDF_STRING_IS_TOO_LONG))
;
        }

        [NUnit.Framework.Test]
        public virtual void LongStringInShadingPatternTest() {
            int maxAllowedLength = pdfA1Checker.GetMaxStringLength();
            int testLength = maxAllowedLength + 1;
            NUnit.Framework.Assert.AreEqual(testLength, 65536);
            String newContentString = PdfACheckerTestUtils.GetStreamWithLongString(testLength);
            byte[] newContent = newContentString.GetBytes(System.Text.Encoding.UTF8);
            PdfStream stream = new PdfStream(newContent);
            PdfPattern pattern = new PdfPattern.Shading(stream);
            // An exception should not be thrown as shading pattern doesn't have
            // content stream to validate
            pdfA1Checker.CheckPdfObject(pattern.GetPdfObject());
        }

        [NUnit.Framework.Test]
        public virtual void LongStringInType3FontTest() {
            NUnit.Framework.Assert.That(() =>  {
                int maxAllowedLength = pdfA1Checker.GetMaxStringLength();
                int testLength = maxAllowedLength + 1;
                NUnit.Framework.Assert.AreEqual(testLength, 65536);
                String newContentString = PdfACheckerTestUtils.GetStreamWithLongString(testLength);
                byte[] newContent = newContentString.GetBytes(System.Text.Encoding.UTF8);
                PdfFont font = PdfFontFactory.CreateType3Font(null, true);
                PdfDictionary charProcs = new PdfDictionary();
                charProcs.Put(PdfName.A, new PdfStream(newContent));
                PdfDictionary dictionary = font.GetPdfObject();
                dictionary.Put(PdfName.Subtype, PdfName.Type3);
                dictionary.Put(PdfName.CharProcs, charProcs);
                // An exception should be thrown as content stream of type3 font has a string which
                // is longer then it is allowed per specification
                pdfA1Checker.CheckFont(font);
            }
            , NUnit.Framework.Throws.InstanceOf<PdfAConformanceException>().With.Message.EqualTo(PdfAConformanceException.PDF_STRING_IS_TOO_LONG))
;
        }
    }
}
