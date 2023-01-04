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
using iText.IO.Font;
using iText.IO.Font.Constants;
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
    }
}
