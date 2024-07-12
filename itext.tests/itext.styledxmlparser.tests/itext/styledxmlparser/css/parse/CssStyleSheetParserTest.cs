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
using iText.Commons.Utils;
using iText.IO.Util;
using iText.StyledXmlParser.Css;
using iText.Test;

namespace iText.StyledXmlParser.Css.Parse {
    [NUnit.Framework.Category("UnitTest")]
    public class CssStyleSheetParserTest : ExtendedITextTest {
        private static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/styledxmlparser/css/parse/CssStyleSheetParserTest/";

        [NUnit.Framework.Test]
        public virtual void Test01() {
            String cssFile = sourceFolder + "css01.css";
            CssStyleSheet styleSheet = CssStyleSheetParser.Parse(FileUtil.GetInputStreamForFile(cssFile));
            NUnit.Framework.Assert.AreEqual(GetCssFileContents(cssFile), styleSheet.ToString());
        }

        [NUnit.Framework.Test]
        public virtual void Test02() {
            String cssFile = sourceFolder + "css02.css";
            String cmpFile = sourceFolder + "cmp_css02.css";
            CssStyleSheet styleSheet = CssStyleSheetParser.Parse(FileUtil.GetInputStreamForFile(cssFile));
            NUnit.Framework.Assert.AreEqual(GetCssFileContents(cmpFile), styleSheet.ToString());
        }

        [NUnit.Framework.Test]
        public virtual void Test03() {
            String cssFile = sourceFolder + "css03.css";
            String cmpFile = sourceFolder + "cmp_css03.css";
            CssStyleSheet styleSheet = CssStyleSheetParser.Parse(FileUtil.GetInputStreamForFile(cssFile));
            NUnit.Framework.Assert.AreEqual(GetCssFileContents(cmpFile), styleSheet.ToString());
        }

        [NUnit.Framework.Test]
        public virtual void Test04() {
            String cssFile = sourceFolder + "css04.css";
            CssStyleSheet styleSheet = CssStyleSheetParser.Parse(FileUtil.GetInputStreamForFile(cssFile));
            NUnit.Framework.Assert.AreEqual("", styleSheet.ToString());
        }

        [NUnit.Framework.Test]
        public virtual void Test05() {
            String cssFile = sourceFolder + "css05.css";
            String cmpFile = sourceFolder + "cmp_css05.css";
            CssStyleSheet styleSheet = CssStyleSheetParser.Parse(FileUtil.GetInputStreamForFile(cssFile));
            NUnit.Framework.Assert.AreEqual(GetCssFileContents(cmpFile), styleSheet.ToString());
        }

        [NUnit.Framework.Test]
        public virtual void Test06() {
            String cssFile = sourceFolder + "css06.css";
            String cmpFile = sourceFolder + "cmp_css06.css";
            CssStyleSheet styleSheet = CssStyleSheetParser.Parse(FileUtil.GetInputStreamForFile(cssFile));
            NUnit.Framework.Assert.AreEqual(GetCssFileContents(cmpFile), styleSheet.ToString());
        }

        [NUnit.Framework.Test]
        public virtual void Test07() {
            String cssFile = sourceFolder + "css07.css";
            CssStyleSheet styleSheet = CssStyleSheetParser.Parse(FileUtil.GetInputStreamForFile(cssFile));
            NUnit.Framework.Assert.AreEqual(GetCssFileContents(cssFile), styleSheet.ToString());
        }

        [NUnit.Framework.Test]
        public virtual void Test08() {
            String cssFile = sourceFolder + "css08.css";
            String cmpFile = sourceFolder + "cmp_css08.css";
            CssStyleSheet styleSheet = CssStyleSheetParser.Parse(FileUtil.GetInputStreamForFile(cssFile));
            NUnit.Framework.Assert.AreEqual(GetCssFileContents(cmpFile), styleSheet.ToString());
        }

        [NUnit.Framework.Test]
        public virtual void Test09() {
            String cssFile = sourceFolder + "css09.css";
            CssStyleSheet styleSheet = CssStyleSheetParser.Parse(FileUtil.GetInputStreamForFile(cssFile));
            NUnit.Framework.Assert.AreEqual(GetCssFileContents(cssFile), styleSheet.ToString());
        }

        [NUnit.Framework.Test]
        public virtual void Test10() {
            String cssFile = sourceFolder + "css10.css";
            String cmpFile = sourceFolder + "cmp_css10.css";
            CssStyleSheet styleSheet = CssStyleSheetParser.Parse(FileUtil.GetInputStreamForFile(cssFile));
            NUnit.Framework.Assert.AreEqual(GetCssFileContents(cmpFile), styleSheet.ToString());
        }

        [NUnit.Framework.Test]
        public virtual void Test11() {
            // TODO DEVSIX-6364 Fix the body declarations duplication for each pageSelector part
            String cssFile = sourceFolder + "css11.css";
            String cmpFile = sourceFolder + "cmp_css11.css";
            CssStyleSheet styleSheet = CssStyleSheetParser.Parse(FileUtil.GetInputStreamForFile(cssFile));
            NUnit.Framework.Assert.AreEqual(GetCssFileContents(cmpFile), styleSheet.ToString());
        }

        [NUnit.Framework.Test]
        public virtual void Test12() {
            String cssFile = sourceFolder + "css12.css";
            String cmpFile = sourceFolder + "cmp_css12.css";
            CssStyleSheet styleSheet = CssStyleSheetParser.Parse(FileUtil.GetInputStreamForFile(cssFile));
            NUnit.Framework.Assert.AreEqual(GetCssFileContents(cmpFile), styleSheet.ToString());
        }

        private String GetCssFileContents(String filePath) {
            byte[] bytes = StreamUtil.InputStreamToArray(FileUtil.GetInputStreamForFile(filePath));
            String content = iText.Commons.Utils.JavaUtil.GetStringForBytes(bytes);
            content = content.Trim();
            content = content.Replace("\r\n", "\n");
            return content;
        }
    }
}
