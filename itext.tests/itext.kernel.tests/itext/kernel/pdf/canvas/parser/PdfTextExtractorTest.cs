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
        public virtual void SimpleFontWithPartialToUnicodeTest() {
            String inFile = sourceFolder + "simpleFontWithPartialToUnicode.pdf";
            using (PdfDocument pdfDocument = new PdfDocument(new PdfReader(inFile))) {
                NUnit.Framework.Assert.AreEqual("Registered", PdfTextExtractor.GetTextFromPage(pdfDocument.GetPage(1)));
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

        [NUnit.Framework.Test]
        public virtual void ShortOctalDataAsTextTest() {
            String inFile = sourceFolder + "shortOctalDataAsText.pdf";
            using (PdfDocument pdfDocument = new PdfDocument(new PdfReader(inFile))) {
                NUnit.Framework.Assert.AreEqual("EC", PdfTextExtractor.GetTextFromPage(pdfDocument.GetPage(1)));
            }
        }
    }
}
