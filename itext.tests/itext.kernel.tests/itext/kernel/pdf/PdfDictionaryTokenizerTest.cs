/*
This file is part of the iText (R) project.
Copyright (c) 1998-2025 Apryse Group NV
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
using iText.Test;

namespace iText.Kernel.Pdf {
    [NUnit.Framework.Category("IntegrationTest")]
    public class PdfDictionaryTokenizerTest : ExtendedITextTest {
        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/kernel/pdf/PdfDictionaryTokenizerTest/";

        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/kernel/pdf/PdfDictionaryTokenizerTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateOrClearDestinationFolder(destinationFolder);
        }

        [NUnit.Framework.Test]
        public virtual void ReaderTurnsCorrectlyNotWellFormattedValueInDictionary_01() {
            String inPath = sourceFolder + "documentWithMalformedNumberOnDictionary.pdf";
            String EXPECTED = "-12.";
            /*
            The following is the content included in the pdf file
            
            <</Ascent 800
            /CapHeight 700
            /Descent -200
            /Flags 32
            /FontBBox[-631 -462 1632 1230]
            /FontFamily(FreeSans)
            /FontFile2 8 0 R
            /FontName/XVVAXW+FreeSans
            /FontWeight 400
            /ItalicAngle -12.-23
            /StemV 80
            /Type/FontDescriptor>>
            */
            // ItalicAngle -12.-23 turns into -12.
            String result = GetItalicAngleValue(inPath);
            NUnit.Framework.Assert.AreEqual(EXPECTED, result);
        }

        [NUnit.Framework.Test]
        public virtual void ReaderTurnsCorrectlyNotWellFormattedValueInDictionary_02() {
            String inPath = sourceFolder + "documentWithMalformedNumberOnDictionary2.pdf";
            String EXPECTED = "-12.";
            /*
            The following is the content included in the pdf file
            
            <</Ascent 800
            /CapHeight 700
            /Descent -200
            /Flags 32
            /FontBBox[-631 -462 1632 1230]
            /FontFamily(FreeSans)
            /FontFile2 8 0 R
            /FontName/XVVAXW+FreeSans
            /FontWeight 400
            /StemV 80
            /Type/FontDescriptor
            /ItalicAngle -12.-23>>
            */
            // ItalicAngle -12.-23 turns into -12.
            String result = GetItalicAngleValue(inPath);
            NUnit.Framework.Assert.AreEqual(EXPECTED, result);
        }

        private String GetItalicAngleValue(String inPath) {
            String result = "";
            PdfReader pdfR = new PdfReader(inPath);
            PdfDocument attachmentPDF = new PdfDocument(pdfR);
            int max = attachmentPDF.GetNumberOfPdfObjects();
            for (int i = 0; i < max; i++) {
                PdfObject obj = attachmentPDF.GetPdfObject(i);
                if (obj != null) {
                    PdfDictionary pdfDict = (PdfDictionary)obj;
                    PdfObject x = pdfDict.Get(PdfName.Type);
                    if (x != null && x.Equals(PdfName.FontDescriptor)) {
                        PdfObject italicAngle = pdfDict.Get(PdfName.ItalicAngle);
                        result = italicAngle.ToString();
                    }
                }
            }
            attachmentPDF.Close();
            return result;
        }
    }
}
