/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
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
using iText.Test;
using iText.Test.Attributes;

namespace iText.Kernel.Pdf.Canvas.Parser {
    [NUnit.Framework.Category("IntegrationTest")]
    public class PdfTextExtractorTest : ExtendedITextTest {
        private static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/kernel/parser/PdfTextExtractorTest/";

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.PDF_REFERS_TO_NOT_EXISTING_PROPERTY_DICTIONARY)]
        public virtual void NoSpecifiedDictionaryInPropertiesTest() {
            String inFile = sourceFolder + "noSpecifiedDictionaryInProperties.pdf";
            using (PdfDocument pdfDocument = new PdfDocument(new PdfReader(inFile))) {
                String text = PdfTextExtractor.GetTextFromPage(pdfDocument.GetPage(1));
                // Here we check that no NPE wasn't thrown. There is no text on the page so the extracted string should be empty.
                NUnit.Framework.Assert.AreEqual("", text);
            }
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.PDF_REFERS_TO_NOT_EXISTING_PROPERTY_DICTIONARY)]
        public virtual void NoPropertiesInResourcesTest() {
            String inFile = sourceFolder + "noPropertiesInResources.pdf";
            using (PdfDocument pdfDocument = new PdfDocument(new PdfReader(inFile))) {
                String text = PdfTextExtractor.GetTextFromPage(pdfDocument.GetPage(1));
                // Here we check that no NPE wasn't thrown. There is no text on the page so the extracted string should be empty.
                NUnit.Framework.Assert.AreEqual("", text);
            }
        }

        [NUnit.Framework.Test]
        public virtual void Type3FontNoCMapTest() {
            String inFile = sourceFolder + "type3NoCMap.pdf";
            using (PdfDocument pdfDocument = new PdfDocument(new PdfReader(inFile))) {
                NUnit.Framework.Assert.AreEqual("*0*", PdfTextExtractor.GetTextFromPage(pdfDocument.GetPage(1)));
            }
        }

        [NUnit.Framework.Test]
        public virtual void NoBaseEncodingTest() {
            String inFile = sourceFolder + "noBaseEncoding.pdf";
            using (PdfDocument pdfDocument = new PdfDocument(new PdfReader(inFile))) {
                NUnit.Framework.Assert.AreEqual("HELLO WORLD", PdfTextExtractor.GetTextFromPage(pdfDocument.GetPage(1)));
            }
        }

        [NUnit.Framework.Test]
        public virtual void SimpleFontWithoutEncodingToUnicodeTest() {
            String inFile = sourceFolder + "simpleFontWithoutEncodingToUnicode.pdf";
            using (PdfDocument pdfDocument = new PdfDocument(new PdfReader(inFile))) {
                NUnit.Framework.Assert.AreEqual("MyriadPro-Bold font.", PdfTextExtractor.GetTextFromPage(pdfDocument.GetPage
                    (1)));
            }
        }

        [NUnit.Framework.Test]
        public virtual void Type0FontToUnicodeTest() {
            String inFile = sourceFolder + "type0FontToUnicode.pdf";
            using (PdfDocument pdfDocument = new PdfDocument(new PdfReader(inFile))) {
                NUnit.Framework.Assert.AreEqual("€ 390", PdfTextExtractor.GetTextFromPage(pdfDocument.GetPage(1)));
            }
        }

        [NUnit.Framework.Test]
        public virtual void ParseTextDiacriticShiftedLessThanTwo() {
            String inFile = sourceFolder + "diacriticShiftedLessThanTwo.pdf";
            // संस्कृत म्
            String expected = "\u0938\u0902\u0938\u094d\u0915\u0943\u0924 \u092e\u094d";
            using (PdfDocument pdfDocument = new PdfDocument(new PdfReader(inFile))) {
                NUnit.Framework.Assert.AreEqual(expected, PdfTextExtractor.GetTextFromPage(pdfDocument.GetPage(1)));
            }
        }

        [NUnit.Framework.Test]
        public virtual void ParseTextDiacriticShiftedMoreThanTwo() {
            String inFile = sourceFolder + "diacriticShiftedMoreThanTwo.pdf";
            // ृ
            //संस्कृत म्
            String expected = "\u0943\n\u0938\u0902\u0938\u094d\u0915\u0943\u0924 \u092e\u094d";
            using (PdfDocument pdfDocument = new PdfDocument(new PdfReader(inFile))) {
                NUnit.Framework.Assert.AreEqual(expected, PdfTextExtractor.GetTextFromPage(pdfDocument.GetPage(1)));
            }
        }
    }
}
