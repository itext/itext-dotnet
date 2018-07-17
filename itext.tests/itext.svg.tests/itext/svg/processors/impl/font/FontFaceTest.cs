/*
This file is part of the iText (R) project.
Copyright (c) 1998-2017 iText Group NV
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
using iText.Kernel.Utils;
using iText.Svg.Exceptions;
using iText.Svg.Renderers;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Svg.Processors.Impl.Font {
    public class FontFaceTest : SvgIntegrationTest {
        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/svg/processors/impl/font/FontFaceTest/";

        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/svg/processors/impl/font/FontFaceTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            ITextTest.CreateDestinationFolder(destinationFolder);
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void DroidSerifWebFontTest() {
            ConvertAndCompareVisually(sourceFolder, destinationFolder, "droidSerifWebFontTest");
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void DroidSerifLocalFontTest() {
            ConvertAndCompareVisually(sourceFolder, destinationFolder, "droidSerifLocalFontTest");
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void DroidSerifLocalLocalFontTest() {
            ConvertAndCompareVisually(sourceFolder, destinationFolder, "droidSerifLocalLocalFontTest");
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void DroidSerifLocalWithMediaFontTest() {
            ConvertAndCompareVisually(sourceFolder, destinationFolder, "droidSerifLocalWithMediaFontTest");
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void DroidSerifLocalWithMediaRuleFontTest() {
            ConvertAndCompareVisually(sourceFolder, destinationFolder, "droidSerifLocalWithMediaRuleFontTest");
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void FontSelectorTest01() {
            ConvertAndCompareVisually(sourceFolder, destinationFolder, "fontSelectorTest01");
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        [LogMessage(SvgLogMessageConstant.UNABLE_TO_RETRIEVE_STREAM_WITH_GIVEN_BASE_URI)]
        public virtual void FontFaceGrammarTest() {
            ConvertAndCompareVisually(sourceFolder, destinationFolder, "fontFaceGrammarTest");
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void FontFaceWoffTest01() {
            RunTest("fontFaceWoffTest01");
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void FontFaceWoffTest02() {
            RunTest("fontFaceWoffTest02");
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        [LogMessage(SvgLogMessageConstant.UNABLE_TO_RETRIEVE_FONT)]
        public virtual void FontFaceTtcTest() {
            //TODO (DEVSIX-2064) Cannot retrieve NotoSansCJK-Regular
            RunTest("fontFaceTtcTest");
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void FontFaceWoff2SimpleTest() {
            RunTest("fontFaceWoff2SimpleTest");
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        [LogMessage(SvgLogMessageConstant.UNABLE_TO_RETRIEVE_FONT)]
        public virtual void FontFaceWoff2TtcTest() {
            //TODO (DEVSIX-2064) Cannot retrieve NotoSansCJK-Regular
            RunTest("fontFaceWoff2TtcTest");
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void W3cProblemTest01() {
            //TODO: In w3c test suite this font is labeled as invalid though it correctly parsers both in browser and iText
            //See BlocksMetadataPadding001Test in io for decompression details
            RunTest("w3cProblemTest01");
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        [NUnit.Framework.Ignore("DEVSIX-1612")]
        public virtual void W3cProblemTest02() {
            //TODO: In w3c test suite this font is labeled as invalid though and its loading failed in browser, though iText parses its as correct one and LOADS!
            //See DirectoryTableOrder002Test in io for decompression details
            RunTest("w3cProblemTest02");
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void W3cProblemTest03() {
            //TODO: silently omitted, decompression should fail.
            //See HeaderFlavor001Test in io for decompression details
            RunTest("w3cProblemTest03");
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        [LogMessage(iText.IO.LogMessageConstant.FONT_SUBSET_ISSUE)]
        public virtual void W3cProblemTest04() {
            //TODO: silently omitted, decompression should fail. Browser loads font but don't draw glyph.
            //See HeaderFlavor002Test in io for decompression details
            //NOTE, iText fails on subsetting as expected.
            RunTest("w3cProblemTest04");
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void W3cProblemTest05() {
            //TODO: In w3c test suite this font is labeled as invalid though it correctly parsers both in browser and iText
            //See HeaderReserved001Test in io for decompression details
            RunTest("w3cProblemTest05");
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void W3cProblemTest06() {
            //TODO: In w3c test suite this font is labeled as invalid though it correctly parsers both in browser and iText
            //See TabledataHmtxTransform003Test in io for decompression details
            RunTest("w3cProblemTest06");
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        [NUnit.Framework.Ignore("DEVSIX-1612")]
        public virtual void W3cProblemTest07() {
            //TODO: In w3c test suite this font is labeled as invalid though and its loading failed in browser, though iText parses its as correct one and LOADS!
            //See ValidationOff012Test in io for decompression details
            RunTest("w3cProblemTest07");
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void IncorrectFontNameTest01() {
            RunTest("incorrectFontNameTest01");
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void IncorrectFontNameTest02() {
            // The result of te test is FAIL. However we consider it to be correct.
            // Although the font-family specified by the paragraph's class doesn't match the one of fontface,
            // font's full name contains specified font-family and iText takes it into account.
            RunTest("incorrectFontNameTest02");
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void IncorrectFontNameTest03() {
            //Checks that font used in previous two files is correct
            RunTest("incorrectFontNameTest03");
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void IncorrectFontNameTest04() {
            RunTest("incorrectFontNameTest04");
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        [NUnit.Framework.Ignore("DEVSIX-1759 - unicode in font family and different result in dotnet")]
        public virtual void FontFamilyTest01() {
            RunTest("fontFamilyTest01");
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        private void RunTest(String fileName) {
            Convert(sourceFolder + fileName + ".svg", destinationFolder + fileName + ".pdf");
            CompareTool compareTool = new CompareTool();
            String compareResult = compareTool.CompareByContent(destinationFolder + fileName + ".pdf", sourceFolder + 
                "cmp_" + fileName + ".pdf", destinationFolder, "diff_");
            NUnit.Framework.Assert.IsNull(compareResult);
        }
    }
}
