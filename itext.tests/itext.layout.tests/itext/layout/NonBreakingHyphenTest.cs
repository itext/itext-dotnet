/*
This file is part of the iText (R) project.
Copyright (c) 1998-2019 iText Group NV
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
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using iText.Layout.Element;
using iText.Layout.Font;
using iText.Layout.Properties;
using iText.Test;

namespace iText.Layout {
    public class NonBreakingHyphenTest : ExtendedITextTest {
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

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void NonBreakingHyphenDifferentFonts() {
            //TODO: update after fix of DEVSIX-2034
            String outFileName = destinationFolder + "nonBreakingHyphenDifferentFonts.pdf";
            String cmpFileName = sourceFolder + "cmp_nonBreakingHyphenDifferentFonts.pdf";
            String diffPrefix = "diff01_";
            Document document = new Document(new PdfDocument(new PdfWriter(outFileName)));
            FontProvider sel = new FontProvider();
            sel.GetFontSet().AddFont(StandardFonts.TIMES_ROMAN);
            sel.GetFontSet().AddFont(StandardFonts.COURIER);
            sel.GetFontSet().AddFont(fontsFolder + "Puritan2.otf", PdfEncodings.IDENTITY_H, "Puritan2");
            sel.GetFontSet().AddFont(fontsFolder + "NotoSans-Regular.ttf", PdfEncodings.IDENTITY_H, "NotoSans");
            document.SetFontProvider(sel);
            document.Add(new Paragraph("StandardFonts - non-breaking hyphen \\u2011").SetUnderline().SetTextAlignment(
                TextAlignment.CENTER));
            document.Add(new Paragraph("for Standard font TIMES_ROMAN: <&#8209;> non-breaking hyphen <\u2011> 2 hyphens<\u2011\u2011>here "
                ).SetFontFamily(StandardFonts.TIMES_ROMAN));
            document.Add(new Paragraph("for Standard font COURIER: <&#8209;> non-breaking hyphen<\u2011> 2hyphens <\u2011\u2011>here "
                ).SetFontFamily(StandardFonts.COURIER));
            document.Add(new Paragraph("for Standard font HELVETICA_BOLD: <&#8209;> non-breaking hyphen<\u2011> 2hyphens <\u2011\u2011>here "
                ).SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD)));
            document.Add(new Paragraph("for Standard font SYMBOL: <&#8209;> non-breaking hyphen<\u2011> 2hyphens <\u2011\u2011>here "
                ).SetFont(PdfFontFactory.CreateFont(StandardFonts.SYMBOL)));
            document.Add(new Paragraph("Non-Standard fonts - non-breaking hyphen \\u2011").SetUnderline().SetTextAlignment
                (TextAlignment.CENTER));
            document.Add(new Paragraph("for NotoSans: <&#8209;> hyphen<\u2011> 2hyphens <\u2011\u2011>here").SetFontFamily
                ("NotoSans"));
            document.Add(new Paragraph("for Puritan2: <&#8209;> hyphen<\u2011> 2hyphens <\u2011\u2011>here").SetFontFamily
                ("Puritan2"));
            sel.GetFontSet().AddFont(fontsFolder + "FreeSans.ttf", PdfEncodings.IDENTITY_H, "FreeSans");
            document.Add(new Paragraph("AFTER adding of FreeSans font with non-breaking hyphen \\u2011 support").SetUnderline
                ().SetTextAlignment(TextAlignment.CENTER));
            document.Add(new Paragraph("for Standard font TIMES_ROMAN: <&#8209;> non-breaking hyphen <\u2011> 2 hyphens<\u2011\u2011>here "
                ).SetFontFamily(StandardFonts.TIMES_ROMAN));
            document.Add(new Paragraph("for Standard font COURIER: <&#8209;> non-breaking hyphen<\u2011> 2hyphens <\u2011\u2011>here "
                ).SetFontFamily(StandardFonts.COURIER));
            document.Add(new Paragraph("for Standard font HELVETICA_BOLD: <&#8209;> non-breaking hyphen<\u2011> 2hyphens <\u2011\u2011>here "
                ).SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD)));
            document.Add(new Paragraph("for Standard font SYMBOL: <&#8209;> non-breaking hyphen<\u2011> 2hyphens <\u2011\u2011>here "
                ).SetFont(PdfFontFactory.CreateFont(StandardFonts.SYMBOL)));
            document.Add(new Paragraph("for FreeSans: <&#8209;> hyphen<\u2011> 2hyphens <\u2011\u2011>here").SetFontFamily
                ("FreeSans"));
            document.Add(new Paragraph("for NotoSans: <&#8209;> hyphen<\u2011> 2hyphens <\u2011\u2011>here").SetFontFamily
                ("NotoSans"));
            document.Add(new Paragraph("for Puritan2: <&#8209;> hyphen<\u2011> 2hyphens <\u2011\u2011>here").SetFontFamily
                ("Puritan2"));
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , diffPrefix));
        }
    }
}
