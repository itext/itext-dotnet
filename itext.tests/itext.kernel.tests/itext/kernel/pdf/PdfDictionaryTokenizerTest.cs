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
