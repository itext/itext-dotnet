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
using iText.Kernel.Pdf;
using iText.Pdfa;
using iText.Test;

namespace iText.Pdfa.Checker {
    public class PdfA2ImplementationLimitsCheckerTest : ExtendedITextTest {
        private PdfA2Checker pdfA2Checker = new PdfA2Checker(PdfAConformanceLevel.PDF_A_2B);

        [NUnit.Framework.SetUp]
        public virtual void Before() {
            pdfA2Checker.SetFullCheckMode(true);
        }

        [NUnit.Framework.Test]
        public virtual void IndependentLongStringTest() {
            NUnit.Framework.Assert.That(() =>  {
                int maxAllowedLength = pdfA2Checker.GetMaxStringLength();
                int testLength = maxAllowedLength + 1;
                NUnit.Framework.Assert.AreEqual(testLength, 32768);
                PdfString longString = PdfACheckerTestUtils.GetLongString(testLength);
                // An exception should be thrown as provided String is longer then
                // it is allowed per specification
                pdfA2Checker.CheckPdfObject(longString);
            }
            , NUnit.Framework.Throws.InstanceOf<PdfAConformanceException>().With.Message.EqualTo(PdfAConformanceException.PDF_STRING_IS_TOO_LONG))
;
        }

        [NUnit.Framework.Test]
        public virtual void LongStringInContentStreamTest() {
            NUnit.Framework.Assert.That(() =>  {
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
                pdfA2Checker.CheckContentStream(stream);
            }
            , NUnit.Framework.Throws.InstanceOf<PdfAConformanceException>().With.Message.EqualTo(PdfAConformanceException.PDF_STRING_IS_TOO_LONG))
;
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
            PdfDictionary longDictionary = PdfACheckerTestUtils.GetLongDictionary(999999);
            // An exception should not be thrown as there is no limits for capacity of a dictionary
            // in PDFA 2
            pdfA2Checker.CheckPdfObject(longDictionary);
            PdfStream longStream = PdfACheckerTestUtils.GetStreamWithLongDictionary(999999);
            // An exception should not be thrown as there is no limits for capacity of a dictionary
            // and stream in PDFA 2
            pdfA2Checker.CheckPdfObject(longStream);
        }
    }
}
