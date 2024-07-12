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
using System.IO;
using System.Text;
using iText.Commons.Utils;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Pdfa.Exceptions;
using iText.Test;

namespace iText.Pdfa {
    [NUnit.Framework.Category("IntegrationTest")]
    public class PdfALongStringTest : ExtendedITextTest {
        private static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/pdfa/";

        private static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/pdfa/PdfALongStringTest/";

        private const String LOREM_IPSUM = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Duis condimentum, tortor sit amet fermentum pharetra, sem felis finibus enim, vel consectetur nunc justo at nisi. In hac habitasse platea dictumst. Donec quis suscipit eros. Nam urna purus, scelerisque in placerat in, convallis vel sapien. Suspendisse sed lacus sit amet orci ornare vulputate. In hac habitasse platea dictumst. Ut eu aliquet felis, at consectetur neque.";

        private const int STRING_LENGTH_LIMIT = 32767;

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateDestinationFolder(destinationFolder);
        }

        [NUnit.Framework.Test]
        public virtual void RunTest() {
            String file = "pdfALongString.pdf";
            String filename = destinationFolder + file;
            using (Stream icm = FileUtil.GetInputStreamForFile(sourceFolder + "sRGB Color Space Profile.icm")) {
                using (Stream fos = FileUtil.GetFileOutputStream(filename)) {
                    Document document = new Document(new PdfADocument(new PdfWriter(fos), PdfAConformanceLevel.PDF_A_3U, new PdfOutputIntent
                        ("Custom", "", "http://www.color.org", "sRGB ICC preference", icm)));
                    StringBuilder stringBuilder = new StringBuilder(LOREM_IPSUM);
                    while (stringBuilder.Length < STRING_LENGTH_LIMIT) {
                        stringBuilder.Append(stringBuilder.ToString());
                    }
                    PdfFontFactory.Register(sourceFolder + "FreeSans.ttf", sourceFolder + "FreeSans.ttf");
                    PdfFont font = PdfFontFactory.CreateFont(sourceFolder + "FreeSans.ttf", PdfFontFactory.EmbeddingStrategy.PREFER_EMBEDDED
                        );
                    Paragraph p = new Paragraph(stringBuilder.ToString());
                    p.SetMinWidth(1e6f);
                    p.SetFont(font);
                    document.Add(p);
                    // when document is closing, ISO conformance check is performed
                    // this document contain a string which is longer than it is allowed
                    // per specification. That is why conformance exception should be thrown
                    Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => document.Close());
                    NUnit.Framework.Assert.AreEqual(PdfaExceptionMessageConstant.PDF_STRING_IS_TOO_LONG, e.Message);
                }
            }
        }
    }
}
