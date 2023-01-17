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
            CssStyleSheet styleSheet = CssStyleSheetParser.Parse(new FileStream(cssFile, FileMode.Open, FileAccess.Read
                ));
            NUnit.Framework.Assert.AreEqual(GetCssFileContents(cssFile), styleSheet.ToString());
        }

        [NUnit.Framework.Test]
        public virtual void Test02() {
            String cssFile = sourceFolder + "css02.css";
            String cmpFile = sourceFolder + "cmp_css02.css";
            CssStyleSheet styleSheet = CssStyleSheetParser.Parse(new FileStream(cssFile, FileMode.Open, FileAccess.Read
                ));
            NUnit.Framework.Assert.AreEqual(GetCssFileContents(cmpFile), styleSheet.ToString());
        }

        [NUnit.Framework.Test]
        public virtual void Test03() {
            String cssFile = sourceFolder + "css03.css";
            String cmpFile = sourceFolder + "cmp_css03.css";
            CssStyleSheet styleSheet = CssStyleSheetParser.Parse(new FileStream(cssFile, FileMode.Open, FileAccess.Read
                ));
            NUnit.Framework.Assert.AreEqual(GetCssFileContents(cmpFile), styleSheet.ToString());
        }

        [NUnit.Framework.Test]
        public virtual void Test04() {
            String cssFile = sourceFolder + "css04.css";
            CssStyleSheet styleSheet = CssStyleSheetParser.Parse(new FileStream(cssFile, FileMode.Open, FileAccess.Read
                ));
            NUnit.Framework.Assert.AreEqual("", styleSheet.ToString());
        }

        [NUnit.Framework.Test]
        public virtual void Test05() {
            String cssFile = sourceFolder + "css05.css";
            String cmpFile = sourceFolder + "cmp_css05.css";
            CssStyleSheet styleSheet = CssStyleSheetParser.Parse(new FileStream(cssFile, FileMode.Open, FileAccess.Read
                ));
            NUnit.Framework.Assert.AreEqual(GetCssFileContents(cmpFile), styleSheet.ToString());
        }

        [NUnit.Framework.Test]
        public virtual void Test06() {
            String cssFile = sourceFolder + "css06.css";
            String cmpFile = sourceFolder + "cmp_css06.css";
            CssStyleSheet styleSheet = CssStyleSheetParser.Parse(new FileStream(cssFile, FileMode.Open, FileAccess.Read
                ));
            NUnit.Framework.Assert.AreEqual(GetCssFileContents(cmpFile), styleSheet.ToString());
        }

        [NUnit.Framework.Test]
        public virtual void Test07() {
            String cssFile = sourceFolder + "css07.css";
            CssStyleSheet styleSheet = CssStyleSheetParser.Parse(new FileStream(cssFile, FileMode.Open, FileAccess.Read
                ));
            NUnit.Framework.Assert.AreEqual(GetCssFileContents(cssFile), styleSheet.ToString());
        }

        [NUnit.Framework.Test]
        public virtual void Test08() {
            String cssFile = sourceFolder + "css08.css";
            String cmpFile = sourceFolder + "cmp_css08.css";
            CssStyleSheet styleSheet = CssStyleSheetParser.Parse(new FileStream(cssFile, FileMode.Open, FileAccess.Read
                ));
            NUnit.Framework.Assert.AreEqual(GetCssFileContents(cmpFile), styleSheet.ToString());
        }

        [NUnit.Framework.Test]
        public virtual void Test09() {
            String cssFile = sourceFolder + "css09.css";
            CssStyleSheet styleSheet = CssStyleSheetParser.Parse(new FileStream(cssFile, FileMode.Open, FileAccess.Read
                ));
            NUnit.Framework.Assert.AreEqual(GetCssFileContents(cssFile), styleSheet.ToString());
        }

        [NUnit.Framework.Test]
        public virtual void Test10() {
            String cssFile = sourceFolder + "css10.css";
            String cmpFile = sourceFolder + "cmp_css10.css";
            CssStyleSheet styleSheet = CssStyleSheetParser.Parse(new FileStream(cssFile, FileMode.Open, FileAccess.Read
                ));
            NUnit.Framework.Assert.AreEqual(GetCssFileContents(cmpFile), styleSheet.ToString());
        }

        [NUnit.Framework.Test]
        public virtual void Test11() {
            // TODO DEVSIX-6364 Fix the body declarations duplication for each pageSelector part
            String cssFile = sourceFolder + "css11.css";
            String cmpFile = sourceFolder + "cmp_css11.css";
            CssStyleSheet styleSheet = CssStyleSheetParser.Parse(new FileStream(cssFile, FileMode.Open, FileAccess.Read
                ));
            NUnit.Framework.Assert.AreEqual(GetCssFileContents(cmpFile), styleSheet.ToString());
        }

        [NUnit.Framework.Test]
        public virtual void Test12() {
            String cssFile = sourceFolder + "css12.css";
            String cmpFile = sourceFolder + "cmp_css12.css";
            CssStyleSheet styleSheet = CssStyleSheetParser.Parse(new FileStream(cssFile, FileMode.Open, FileAccess.Read
                ));
            NUnit.Framework.Assert.AreEqual(GetCssFileContents(cmpFile), styleSheet.ToString());
        }

        private String GetCssFileContents(String filePath) {
            byte[] bytes = StreamUtil.InputStreamToArray(new FileStream(filePath, FileMode.Open, FileAccess.Read));
            String content = iText.Commons.Utils.JavaUtil.GetStringForBytes(bytes);
            content = content.Trim();
            content = content.Replace("\r\n", "\n");
            return content;
        }
    }
}
