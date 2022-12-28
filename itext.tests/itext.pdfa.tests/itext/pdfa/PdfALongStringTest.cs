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
using System.IO;
using System.Text;
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
            using (Stream icm = new FileStream(sourceFolder + "sRGB Color Space Profile.icm", FileMode.Open, FileAccess.Read
                )) {
                using (FileStream fos = new FileStream(filename, FileMode.Create)) {
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
                    NUnit.Framework.Assert.AreEqual(PdfAConformanceException.PDF_STRING_IS_TOO_LONG, e.Message);
                }
            }
        }
    }
}
