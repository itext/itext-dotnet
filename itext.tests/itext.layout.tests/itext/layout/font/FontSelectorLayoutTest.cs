/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 Apryse Group NV
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
using iText.IO.Font;
using iText.IO.Font.Constants;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using iText.Layout;
using iText.Layout.Element;
using iText.Test;

namespace iText.Layout.Font {
    [NUnit.Framework.Category("IntegrationTest")]
    public class FontSelectorLayoutTest : ExtendedITextTest {
        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/layout/NonBreakingHyphenTest/";

        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/layout/NonBreakingHyphenTest/";

        public static readonly String fontsFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/layout/fonts/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateOrClearDestinationFolder(destinationFolder);
        }

        [NUnit.Framework.Test]
        public virtual void NonBreakingHyphenDifferentFonts() {
            //TODO: update after fix of DEVSIX-2052
            String outFileName = destinationFolder + "nonBreakingHyphenDifferentFonts.pdf";
            String cmpFileName = sourceFolder + "cmp_nonBreakingHyphenDifferentFonts.pdf";
            Document document = new Document(new PdfDocument(new PdfWriter(outFileName)));
            FontProvider sel = new FontProvider();
            sel.GetFontSet().AddFont(StandardFonts.TIMES_ROMAN);
            sel.GetFontSet().AddFont(StandardFonts.COURIER);
            sel.GetFontSet().AddFont(fontsFolder + "Puritan2.otf", PdfEncodings.IDENTITY_H, "Puritan2");
            sel.GetFontSet().AddFont(fontsFolder + "NotoSans-Regular.ttf", PdfEncodings.IDENTITY_H, "NotoSans");
            sel.GetFontSet().AddFont(fontsFolder + "FreeSans.ttf", PdfEncodings.IDENTITY_H, "FreeSans");
            document.SetFontProvider(sel);
            document.Add(CreateParagraph("For Standard font TIMES_ROMAN: ", StandardFonts.TIMES_ROMAN));
            document.Add(CreateParagraph("For Standard font COURIER: ", StandardFonts.COURIER));
            document.Add(CreateParagraph("For FreeSans: ", ("FreeSans")));
            document.Add(CreateParagraph("For NotoSans: ", ("NotoSans")));
            document.Add(CreateParagraph("For Puritan2: ", ("Puritan2")));
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diffPrefix"));
        }

        private static Paragraph CreateParagraph(String textParagraph, String font) {
            String text = "here is non-breaking hyphen: <\u2011> text after non-breaking hyphen.";
            Paragraph p = new Paragraph(textParagraph + text).SetFontFamily(font);
            return p;
        }

        [NUnit.Framework.Test]
        public virtual void UtfToGlyphToUtfRountripTest() {
            // this should not throw a null pointer exception
            using (PdfDocument pdfDoc = new PdfDocument(new PdfWriter(new MemoryStream()))) {
                using (Document doc = new Document(pdfDoc)) {
                    doc.SetFont(PdfFontFactory.CreateFont("HeiseiMin-W3", "UniJIS-UCS2-H"));
                    NUnit.Framework.Assert.DoesNotThrow(() => doc.Add(new Paragraph("\u9F9C")));
                }
            }
        }
    }
}
