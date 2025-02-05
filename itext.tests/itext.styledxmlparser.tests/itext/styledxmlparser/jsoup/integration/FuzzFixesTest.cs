/*
This file is part of the iText (R) project.
Copyright (c) 1998-2025 Apryse Group NV
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
using iText.StyledXmlParser.Jsoup.Nodes;
using iText.Test;

namespace iText.StyledXmlParser.Jsoup.Integration {
    /// <summary>Tests fixes for issues raised by the OSS Fuzz project @ https://oss-fuzz.com/testcases?project=jsoup
    ///     </summary>
    [NUnit.Framework.Category("IntegrationTest")]
    public class FuzzFixesTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void BlankAbsAttr() {
            // https://github.com/jhy/jsoup/issues/1541
            String html = "b<bodY abs: abs:abs: abs:abs:abs>";
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse(html);
            NUnit.Framework.Assert.IsNotNull(doc);
        }

        [NUnit.Framework.Test]
        public virtual void ResetInsertionMode() {
            // https://github.com/jhy/jsoup/issues/1538
            FileInfo @in = iText.StyledXmlParser.Jsoup.PortTestUtil.GetFile("/fuzztests/1538.html");
            // lots of escape chars etc.
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse(@in, "UTF-8");
            NUnit.Framework.Assert.IsNotNull(doc);
        }

        [NUnit.Framework.Test]
        public virtual void XmlDeclOverflow() {
            // https://github.com/jhy/jsoup/issues/1539
            FileInfo @in = iText.StyledXmlParser.Jsoup.PortTestUtil.GetFile("/fuzztests/1539.html");
            // lots of escape chars etc.
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse(@in, "UTF-8");
            NUnit.Framework.Assert.IsNotNull(doc);
            Document docXml = iText.StyledXmlParser.Jsoup.Jsoup.Parse(FileUtil.GetInputStreamForFile(@in), "UTF-8", "https://example.com"
                , iText.StyledXmlParser.Jsoup.Parser.Parser.XmlParser());
            NUnit.Framework.Assert.IsNotNull(docXml);
        }

        [NUnit.Framework.Test]
        public virtual void XmlDeclOverflowOOM() {
            // https://github.com/jhy/jsoup/issues/1569
            FileInfo @in = iText.StyledXmlParser.Jsoup.PortTestUtil.GetFile("/fuzztests/1569.html");
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse(@in, "UTF-8");
            NUnit.Framework.Assert.IsNotNull(doc);
            Document docXml = iText.StyledXmlParser.Jsoup.Jsoup.Parse(FileUtil.GetInputStreamForFile(@in), "UTF-8", "https://example.com"
                , iText.StyledXmlParser.Jsoup.Parser.Parser.XmlParser());
            NUnit.Framework.Assert.IsNotNull(docXml);
        }

        [NUnit.Framework.Test]
        public virtual void StackOverflowState14() {
            // https://github.com/jhy/jsoup/issues/1543
            FileInfo @in = iText.StyledXmlParser.Jsoup.PortTestUtil.GetFile("/fuzztests/1543.html");
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse(@in, "UTF-8");
            NUnit.Framework.Assert.IsNotNull(doc);
        }

        [NUnit.Framework.Test]
        public virtual void ParseTimeout() {
            // https://github.com/jhy/jsoup/issues/1544
            FileInfo @in = iText.StyledXmlParser.Jsoup.PortTestUtil.GetFile("/fuzztests/1544.html");
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse(@in, "UTF-8");
            NUnit.Framework.Assert.IsNotNull(doc);
        }
    }
}
