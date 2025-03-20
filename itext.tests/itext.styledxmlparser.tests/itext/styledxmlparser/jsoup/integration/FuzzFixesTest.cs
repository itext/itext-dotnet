/*
This file is part of jsoup, see NOTICE.txt in the root of the repository.
It may contain modifications beyond the original version.
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
