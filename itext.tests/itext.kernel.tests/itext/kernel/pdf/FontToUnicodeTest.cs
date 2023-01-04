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
using System.Collections.Generic;
using iText.Commons.Utils;
using iText.IO.Font;
using iText.IO.Font.Otf;
using iText.Kernel.Font;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Test;

namespace iText.Kernel.Pdf {
    [NUnit.Framework.Category("IntegrationTest")]
    public class FontToUnicodeTest : ExtendedITextTest {
        public static readonly String fontsFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/kernel/pdf/fonts/";

        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/kernel/pdf/FontToUnicodeTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateDestinationFolder(destinationFolder);
        }

        [NUnit.Framework.Test]
        public virtual void SeveralUnicodesWithinOneGlyphTest() {
            // TODO DEVSIX-3634. In the output now we don't expect the \u2F46 unicode range.
            // TODO DEVSIX-3634. SUBSTITUTE "Assert.assertEquals("\u2F46"..." to "Assert.assertEquals("\u65E0"..." after the fix
            String outFileName = destinationFolder + "severalUnicodesWithinOneGlyphTest.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            PdfFont font = PdfFontFactory.CreateFont(fontsFolder + "NotoSansCJKjp-Bold.otf", PdfEncodings.IDENTITY_H);
            IList<Glyph> glyphs = JavaCollectionsUtil.SingletonList(font.GetGlyph((int)'\u65E0'));
            GlyphLine glyphLine = new GlyphLine(glyphs);
            PdfCanvas canvas2 = new PdfCanvas(pdfDocument.AddNewPage());
            canvas2.SaveState().BeginText().MoveText(36, 800).SetFontAndSize(font, 12).ShowText(glyphLine).EndText().RestoreState
                ();
            pdfDocument.Close();
            PdfDocument resultantPdfAsFile = new PdfDocument(new PdfReader(outFileName));
            String actualUnicode = PdfTextExtractor.GetTextFromPage(resultantPdfAsFile.GetFirstPage());
            NUnit.Framework.Assert.AreEqual("\u2F46", actualUnicode);
        }
    }
}
