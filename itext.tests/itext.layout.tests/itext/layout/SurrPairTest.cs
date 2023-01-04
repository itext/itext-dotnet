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
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using iText.Layout.Element;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Layout {
    [NUnit.Framework.Category("IntegrationTest")]
    public class SurrPairTest : ExtendedITextTest {
        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/layout/SurrPairTest/";

        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/layout/SurrPairTest/";

        public static readonly String fontsFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/layout/fonts/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateDestinationFolder(destinationFolder);
        }

        [NUnit.Framework.Test]
        public virtual void SurrogatePairFrom2Chars() {
            String outFileName = destinationFolder + "surrogatePairFrom2Chars.pdf";
            String cmpFileName = sourceFolder + "cmp_" + "surrogatePairFrom2Chars.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document document = new Document(pdfDocument);
            PdfFont font = PdfFontFactory.CreateFont(fontsFolder + "NotoEmoji-Regular.ttf", PdfEncodings.IDENTITY_H);
            //😉
            String winkinkSmile = "\uD83D\uDE09";
            Paragraph paragraph = new Paragraph(winkinkSmile);
            document.SetFont(font);
            document.Add(paragraph);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                ));
        }

        [NUnit.Framework.Test]
        public virtual void SurrogatePair2Pairs() {
            String outFileName = destinationFolder + "surrogatePair2Pairs.pdf";
            String cmpFileName = sourceFolder + "cmp_" + "surrogatePair2Pairs.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document document = new Document(pdfDocument);
            PdfFont font = PdfFontFactory.CreateFont(fontsFolder + "NotoEmoji-Regular.ttf", PdfEncodings.IDENTITY_H);
            //🇧🇾
            String belarusAbbr = "\uD83C\uDDE7\uD83C\uDDFE";
            Paragraph paragraph = new Paragraph(belarusAbbr);
            document.SetFont(font);
            document.Add(paragraph);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                ));
        }

        [NUnit.Framework.Test]
        public virtual void SurrogatePairFullCharacter() {
            String outFileName = destinationFolder + "surrogatePairFullCharacter.pdf";
            String cmpFileName = sourceFolder + "cmp_" + "surrogatePairFullCharacter.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document document = new Document(pdfDocument);
            PdfFont font = PdfFontFactory.CreateFont(fontsFolder + "NotoEmoji-Regular.ttf", PdfEncodings.IDENTITY_H);
            //🛀
            String em = new String(iText.IO.Util.TextUtil.ToChars(0x0001F6C0));
            Paragraph paragraph = new Paragraph(em);
            document.SetFont(font);
            document.Add(paragraph);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                ));
        }

        [NUnit.Framework.Test]
        //TODO DEVSIX-3307
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.FONT_SUBSET_ISSUE)]
        public virtual void SurrogatePairCombingFullSurrs() {
            String outFileName = destinationFolder + "surrogatePairCombingFullSurrs.pdf";
            String cmpFileName = sourceFolder + "cmp_" + "surrogatePairCombingFullSurrs.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document document = new Document(pdfDocument);
            PdfFont font = PdfFontFactory.CreateFont(fontsFolder + "NotoColorEmoji.ttf", PdfEncodings.IDENTITY_H);
            //🏴󠁧󠁢󠁥󠁮󠁧󠁿
            String firstPair = new String(iText.IO.Util.TextUtil.ToChars(0x0001F3F4));
            String secondPair = new String(iText.IO.Util.TextUtil.ToChars(0x000E0067));
            String thirdPair = new String(iText.IO.Util.TextUtil.ToChars(0x000E0062));
            String forthPair = new String(iText.IO.Util.TextUtil.ToChars(0x000E0065));
            String fifthPair = new String(iText.IO.Util.TextUtil.ToChars(0x000E006E));
            String sixthPair = new String(iText.IO.Util.TextUtil.ToChars(0x000E0067));
            String seventhPair = new String(iText.IO.Util.TextUtil.ToChars(0x000E007F));
            String blackFlag = firstPair + secondPair + thirdPair + forthPair + fifthPair + sixthPair + seventhPair;
            Paragraph paragraph = new Paragraph(blackFlag);
            document.SetFont(font);
            document.Add(paragraph);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                ));
        }

        [NUnit.Framework.Test]
        //TODO DEVSIX-3307
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.FONT_SUBSET_ISSUE)]
        public virtual void SurrogatePairCombingFullSurrsWithNoSurrs() {
            String outFileName = destinationFolder + "surrogatePairCombingFullSurrsWithNoSurrs.pdf";
            String cmpFileName = sourceFolder + "cmp_" + "surrogatePairCombingFullSurrsWithNoSurrs.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document document = new Document(pdfDocument);
            PdfFont font = PdfFontFactory.CreateFont(fontsFolder + "NotoColorEmoji.ttf", PdfEncodings.IDENTITY_H);
            //World Map
            String firstPair = new String(iText.IO.Util.TextUtil.ToChars(0x0001F5FA));
            String space = "\u0020";
            //🗽
            String secondPair = new String(iText.IO.Util.TextUtil.ToChars(0x0001F5FD));
            //Satellite
            String thirdPair = new String(iText.IO.Util.TextUtil.ToChars(0x0001F6F0));
            String allPairs = firstPair + space + secondPair + space + thirdPair;
            Paragraph paragraph = new Paragraph(allPairs);
            document.SetFont(font);
            document.Add(paragraph);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                ));
        }
    }
}
