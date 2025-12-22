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
using System.IO;
using iText.Kernel.Exceptions;
using iText.Kernel.Pdf;
using iText.Test;

namespace iText.Kernel.Pdf.Filters {
    /// <summary>
    /// Unit tests for
    /// <see cref="BrotliFilter"/>.
    /// </summary>
    [NUnit.Framework.Category("IntegrationTest")]
    public class BrotliFilterIntegrationTest : ExtendedITextTest {
        public static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/kernel/pdf/filters" + "/BrotliFilterIntegrationTest/";

        public static readonly String DESTINATION_FOLDER = TestUtil.GetOutputPath() + "/kernel/filters/BrotliFilterIntegrationTest/";

        [NUnit.Framework.Test]
        public virtual void DecodeBrotliWithDecodeParmsTest() {
            String sourcePdf = SOURCE_FOLDER + "brotli_correct_decode.pdf";
            byte[] originalBytes = File.ReadAllBytes(System.IO.Path.Combine(SOURCE_FOLDER + "simple.bmp"));
            using (PdfDocument pdfDoc = new PdfDocument(new PdfReader(sourcePdf))) {
                PdfPage page = pdfDoc.GetFirstPage();
                PdfDictionary xObjects = page.GetResources().GetPdfObject().GetAsDictionary(PdfName.XObject);
                PdfStream imgStream = xObjects.GetAsStream(new PdfName("Im1"));
                NUnit.Framework.Assert.AreEqual(originalBytes, imgStream.GetBytes(), "Decoded image should match original"
                    );
            }
        }

        [NUnit.Framework.Test]
        [NUnit.Framework.Ignore("DEVSIX-9595: We cannot create an integrationtest yet for this.")]
        public virtual void DecodeBrotliContentStreamWithDDictionaryTest() {
            String src = SOURCE_FOLDER + "brotli_with_D_dictionary.pdf";
            byte[] expected = "The quick brown fox jumps over the lazy dog.".GetBytes(System.Text.Encoding.UTF8);
            using (PdfDocument pdfDoc = new PdfDocument(new PdfReader(src))) {
                PdfPage page = pdfDoc.GetFirstPage();
                PdfStream stream = page.GetContentStream(0);
                NUnit.Framework.Assert.AreEqual(expected, stream.GetBytes());
            }
        }

        [NUnit.Framework.Test]
        public virtual void DecodeBrotliContentStreamWithWrongDecodeParamsTest() {
            String src = SOURCE_FOLDER + "brotli_contentstream_wrong_DecodeParms.pdf";
            using (PdfDocument pdfDoc = new PdfDocument(new PdfReader(src))) {
                PdfPage page = pdfDoc.GetFirstPage();
                PdfStream stream = page.GetContentStream(0);
                Exception exception = NUnit.Framework.Assert.Catch(typeof(PdfException), () => stream.GetBytes());
                NUnit.Framework.Assert.AreEqual(KernelExceptionMessageConstant.PNG_FILTER_UNKNOWN, exception.Message);
            }
        }
    }
}
