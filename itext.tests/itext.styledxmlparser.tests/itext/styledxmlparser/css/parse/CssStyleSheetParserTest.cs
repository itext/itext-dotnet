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
using iText.Commons.Utils;
using iText.IO.Util;
using iText.StyledXmlParser.Css;
using iText.Test;
using iText.Test.Attributes;

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
        [LogMessage(iText.StyledXmlParser.Logs.StyledXmlParserLogMessageConstant.UNABLE_TO_RETRIEVE_STREAM_WITH_GIVEN_BASE_URI
            , Count = 6, LogLevel = LogLevelConstants.ERROR)]
        public virtual void Test07() {
            String cssFile = sourceFolder + "css07.css";
            CssStyleSheet styleSheet = CssStyleSheetParser.Parse(FileUtil.GetInputStreamForFile(cssFile), sourceFolder
                );
            NUnit.Framework.Assert.AreEqual(".myclass {\n    font-size: 10pt\n}", styleSheet.ToString());
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

        [NUnit.Framework.Test]
        [LogMessage(iText.StyledXmlParser.Logs.StyledXmlParserLogMessageConstant.RULE_IS_NOT_SUPPORTED, LogLevel = 
            LogLevelConstants.ERROR)]
        public virtual void LayerUsingTest1() {
            String cssString = "@layer utilities {\n" + "           .padding-sm {\n" + "             padding: 0.5rem;\n"
                 + "           }\n" + "         }";
            CssStyleSheet styleSheet = CssStyleSheetParser.Parse(new MemoryStream(cssString.GetBytes()));
            NUnit.Framework.Assert.IsTrue(styleSheet.GetStatements().IsEmpty());
        }

        [NUnit.Framework.Test]
        public virtual void LayerUsingTest2() {
            String cssString = "@layer utilities;";
            CssStyleSheet styleSheet = CssStyleSheetParser.Parse(new MemoryStream(cssString.GetBytes()));
            NUnit.Framework.Assert.AreEqual(1, styleSheet.GetStatements().Count);
            NUnit.Framework.Assert.IsTrue(styleSheet.GetStatements()[0] is CssSemicolonAtRule);
        }

        [NUnit.Framework.Test]
        public virtual void CharsetBeforeImportTest() {
            String cssString = "@charset \"UTF-8\";\n" + "         @import url(\"css01.css\");";
            CssStyleSheet styleSheet = CssStyleSheetParser.Parse(new MemoryStream(cssString.GetBytes()), sourceFolder);
            NUnit.Framework.Assert.AreEqual(2, styleSheet.GetStatements().Count);
            NUnit.Framework.Assert.IsTrue(styleSheet.GetStatements()[0] is CssSemicolonAtRule);
            NUnit.Framework.Assert.IsTrue("charset".Equals(((CssSemicolonAtRule)styleSheet.GetStatements()[0]).GetRuleName
                ()));
            NUnit.Framework.Assert.IsTrue(styleSheet.GetStatements()[1] is CssRuleSet);
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.StyledXmlParser.Logs.StyledXmlParserLogMessageConstant.IMPORT_MUST_COME_BEFORE, LogLevel
             = LogLevelConstants.WARN)]
        public virtual void StyleBeforeImportTest() {
            String cssString = "div {background-color: red;}\n" + "         @import url(\"test.css\");";
            CssStyleSheet styleSheet = CssStyleSheetParser.Parse(new MemoryStream(cssString.GetBytes()));
            NUnit.Framework.Assert.AreEqual(1, styleSheet.GetStatements().Count);
            NUnit.Framework.Assert.IsFalse(styleSheet.GetStatements()[0] is CssImportAtRule);
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.StyledXmlParser.Logs.StyledXmlParserLogMessageConstant.RULE_IS_NOT_SUPPORTED, LogLevel = 
            LogLevelConstants.ERROR)]
        public virtual void LayerBeforeImportTest1() {
            String cssString = "@layer utilities {\n" + "           .padding-sm {\n" + "             padding: 0.5rem;\n"
                 + "           }\n" + "         }\n" + "         @import url(\"css01.css\");";
            CssStyleSheet styleSheet = CssStyleSheetParser.Parse(new MemoryStream(cssString.GetBytes()), sourceFolder);
            NUnit.Framework.Assert.AreEqual(1, styleSheet.GetStatements().Count);
            NUnit.Framework.Assert.IsTrue(styleSheet.GetStatements()[0] is CssRuleSet);
        }

        [NUnit.Framework.Test]
        public virtual void LayerBeforeImportTest2() {
            String cssString = "@layer utilities;" + "         @import url(\"css01.css\");";
            CssStyleSheet styleSheet = CssStyleSheetParser.Parse(new MemoryStream(cssString.GetBytes()), sourceFolder);
            NUnit.Framework.Assert.AreEqual(2, styleSheet.GetStatements().Count);
            NUnit.Framework.Assert.IsTrue(styleSheet.GetStatements()[0] is CssSemicolonAtRule);
            NUnit.Framework.Assert.IsTrue("layer".Equals(((CssSemicolonAtRule)styleSheet.GetStatements()[0]).GetRuleName
                ()));
            NUnit.Framework.Assert.IsTrue(styleSheet.GetStatements()[1] is CssRuleSet);
        }

        [NUnit.Framework.Test]
        public virtual void ImportBeforeImportTest() {
            String cssString = "@import url(\"css01.css\");\n" + "         @import url(\"css09.css\");";
            CssStyleSheet styleSheet = CssStyleSheetParser.Parse(new MemoryStream(cssString.GetBytes()), sourceFolder);
            NUnit.Framework.Assert.AreEqual(2, styleSheet.GetStatements().Count);
            NUnit.Framework.Assert.IsTrue(styleSheet.GetStatements()[0] is CssRuleSet);
            NUnit.Framework.Assert.IsTrue(styleSheet.GetStatements()[1] is CssRuleSet);
        }

        [NUnit.Framework.Test]
        public virtual void ImportWithoutSemicolonTest() {
            String cssString = "@import url(\"css01.css\")";
            CssStyleSheet styleSheet = CssStyleSheetParser.Parse(new MemoryStream(cssString.GetBytes()), sourceFolder);
            NUnit.Framework.Assert.IsTrue(styleSheet.GetStatements().IsEmpty());
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.StyledXmlParser.Logs.StyledXmlParserLogMessageConstant.IMPORT_RULE_URL_CAN_NOT_BE_RESOLVED
            , LogLevel = LogLevelConstants.ERROR)]
        public virtual void ImportWithoutBaseUrlTest() {
            String cssString = "@import url(\"css01.css\");";
            CssStyleSheet styleSheet = CssStyleSheetParser.Parse(new MemoryStream(cssString.GetBytes()));
            NUnit.Framework.Assert.IsTrue(styleSheet.GetStatements().IsEmpty());
        }

        [NUnit.Framework.Test]
        public virtual void BackgroundWithSvgDataTest() {
            String declaration = "background: url('data:image/svg+xml;utf8," + "<svg xmlns=\"http://www.w3.org/2000/svg\" xmlns:xlink=\"http://www.w3.org/1999/xlink\" width=\"300\" height=\"200\"> "
                 + "<polygon points=\"100,10 40,198 190,78 10,78 160,198\" style=\"fill:lime;stroke:purple;stroke-width:5;fill-rule:evenodd;\" />"
                 + "</svg>');";
            String cssString = "dif {" + declaration + "}";
            CssStyleSheet styleSheet = CssStyleSheetParser.Parse(new MemoryStream(cssString.GetBytes()), sourceFolder);
            NUnit.Framework.Assert.AreEqual(1, styleSheet.GetStatements().Count);
            // When we parse URL using CssDeclarationValueTokenizer, we lost `;` at the end of declaration
            NUnit.Framework.Assert.IsTrue(styleSheet.GetStatements()[0].ToString().Contains(declaration.JSubstring(0, 
                declaration.Length - 1)));
        }

        [NUnit.Framework.Test]
        public virtual void Base64Test() {
            String cssString = "data:image/jpeg;base64,/9j/aGVsbG8gd29ybGQ=";
            CssStyleSheet styleSheet = CssStyleSheetParser.Parse(new MemoryStream(cssString.GetBytes()), sourceFolder);
            NUnit.Framework.Assert.IsTrue(styleSheet.GetStatements().IsEmpty());
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
