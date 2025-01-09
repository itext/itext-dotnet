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
using System.Collections.Generic;
using System.Linq;
using iText.Commons.Utils;
using iText.StyledXmlParser;
using iText.StyledXmlParser.Css;
using iText.StyledXmlParser.Css.Parse;
using iText.StyledXmlParser.Node;
using iText.StyledXmlParser.Node.Impl.Jsoup;
using iText.StyledXmlParser.Node.Impl.Jsoup.Node;
using iText.Test;

namespace iText.StyledXmlParser.Css.Media {
    [NUnit.Framework.Category("UnitTest")]
    public class CssMediaRuleTest : ExtendedITextTest {
        private static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/styledxmlparser/css/media/MediaRuleTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
        }

        [NUnit.Framework.Test]
        public virtual void Test01() {
            String htmlFileName = sourceFolder + "html01.html";
            String cssFileName = sourceFolder + "css01.css";
            IXmlParser htmlParser = new JsoupHtmlParser();
            IDocumentNode document = htmlParser.Parse(FileUtil.GetInputStreamForFile(htmlFileName), "UTF-8");
            CssStyleSheet css = CssStyleSheetParser.Parse(FileUtil.GetInputStreamForFile(cssFileName));
            MediaDeviceDescription deviceDescription = new MediaDeviceDescription(MediaType.PRINT);
            IElementNode element = new JsoupElementNode(((JsoupDocumentNode)document).GetDocument().GetElementsByTag("p"
                ).First());
            IList<CssDeclaration> declarations = css.GetCssDeclarations(element, deviceDescription);
            NUnit.Framework.Assert.AreEqual(3, declarations.Count);
            NUnit.Framework.Assert.AreEqual("font-weight: bold", declarations[0].ToString());
            NUnit.Framework.Assert.AreEqual("color: red", declarations[1].ToString());
            NUnit.Framework.Assert.AreEqual("font-size: 20pt", declarations[2].ToString());
        }

        [NUnit.Framework.Test]
        public virtual void Test02() {
            String htmlFileName = sourceFolder + "html02.html";
            String cssFileName = sourceFolder + "css02.css";
            IXmlParser htmlParser = new JsoupHtmlParser();
            IDocumentNode document = htmlParser.Parse(FileUtil.GetInputStreamForFile(htmlFileName), "UTF-8");
            CssStyleSheet css = CssStyleSheetParser.Parse(FileUtil.GetInputStreamForFile(cssFileName));
            IElementNode element = new JsoupElementNode(((JsoupDocumentNode)document).GetDocument().GetElementsByTag("p"
                ).First());
            MediaDeviceDescription deviceDescription1 = new MediaDeviceDescription(MediaType.PRINT);
            deviceDescription1.SetWidth(525);
            MediaDeviceDescription deviceDescription2 = new MediaDeviceDescription(MediaType.HANDHELD);
            deviceDescription2.SetOrientation("landscape");
            IList<CssDeclaration> declarations1 = css.GetCssDeclarations(element, deviceDescription1);
            IList<CssDeclaration> declarations2 = css.GetCssDeclarations(element, deviceDescription2);
            NUnit.Framework.Assert.IsTrue(Enumerable.SequenceEqual(declarations1, declarations2));
            NUnit.Framework.Assert.AreEqual(1, declarations1.Count);
            NUnit.Framework.Assert.AreEqual("font-weight: bold", declarations1[0].ToString());
        }

        [NUnit.Framework.Test]
        public virtual void Test03() {
            String htmlFileName = sourceFolder + "html03.html";
            String cssFileName = sourceFolder + "css03.css";
            IXmlParser htmlParser = new JsoupHtmlParser();
            IDocumentNode document = htmlParser.Parse(FileUtil.GetInputStreamForFile(htmlFileName), "UTF-8");
            CssStyleSheet css = CssStyleSheetParser.Parse(FileUtil.GetInputStreamForFile(cssFileName));
            MediaDeviceDescription deviceDescription = new MediaDeviceDescription(MediaType.PRINT);
            deviceDescription.SetResolution(300);
            IElementNode element = new JsoupElementNode(((JsoupDocumentNode)document).GetDocument().GetElementsByTag("p"
                ).First());
            IList<CssDeclaration> declarations = css.GetCssDeclarations(element, deviceDescription);
            NUnit.Framework.Assert.AreEqual(1, declarations.Count);
            NUnit.Framework.Assert.AreEqual("color: black", declarations[0].ToString());
        }

        [NUnit.Framework.Test]
        public virtual void Test04() {
            String htmlFileName = sourceFolder + "html04.html";
            String cssFileName = sourceFolder + "css04.css";
            IXmlParser htmlParser = new JsoupHtmlParser();
            IDocumentNode document = htmlParser.Parse(FileUtil.GetInputStreamForFile(htmlFileName), "UTF-8");
            CssStyleSheet css = CssStyleSheetParser.Parse(FileUtil.GetInputStreamForFile(cssFileName));
            MediaDeviceDescription deviceDescription = new MediaDeviceDescription(MediaType.PRINT).SetColorIndex(256);
            IElementNode element = new JsoupElementNode(((JsoupDocumentNode)document).GetDocument().GetElementsByTag("p"
                ).First());
            IList<CssDeclaration> declarations = css.GetCssDeclarations(element, deviceDescription);
            NUnit.Framework.Assert.AreEqual(2, declarations.Count);
            NUnit.Framework.Assert.AreEqual("color: red", declarations[0].ToString());
            NUnit.Framework.Assert.AreEqual("font-size: 20em", declarations[1].ToString());
        }

        [NUnit.Framework.Test]
        public virtual void Test05() {
            String htmlFileName = sourceFolder + "html05.html";
            String cssFileName = sourceFolder + "css05.css";
            IXmlParser htmlParser = new JsoupHtmlParser();
            IDocumentNode document = htmlParser.Parse(FileUtil.GetInputStreamForFile(htmlFileName), "UTF-8");
            CssStyleSheet css = CssStyleSheetParser.Parse(FileUtil.GetInputStreamForFile(cssFileName));
            IElementNode element = new JsoupElementNode(((JsoupDocumentNode)document).GetDocument().GetElementsByTag("p"
                ).First());
            MediaDeviceDescription deviceDescription1 = new MediaDeviceDescription(MediaType.PRINT).SetWidth(300).SetHeight
                (301);
            MediaDeviceDescription deviceDescription2 = new MediaDeviceDescription(MediaType.SCREEN).SetWidth(400).SetHeight
                (400);
            IList<CssDeclaration> declarations1 = css.GetCssDeclarations(element, deviceDescription1);
            IList<CssDeclaration> declarations2 = css.GetCssDeclarations(element, deviceDescription2);
            NUnit.Framework.Assert.AreEqual(0, declarations1.Count);
            NUnit.Framework.Assert.AreEqual(1, declarations2.Count);
            NUnit.Framework.Assert.AreEqual("color: red", declarations2[0].ToString());
        }

        [NUnit.Framework.Test]
        public virtual void Test06() {
            String htmlFileName = sourceFolder + "html06.html";
            String cssFileName = sourceFolder + "css06.css";
            IXmlParser htmlParser = new JsoupHtmlParser();
            IDocumentNode document = htmlParser.Parse(FileUtil.GetInputStreamForFile(htmlFileName), "UTF-8");
            CssStyleSheet css = CssStyleSheetParser.Parse(FileUtil.GetInputStreamForFile(cssFileName));
            IElementNode element = new JsoupElementNode(((JsoupDocumentNode)document).GetDocument().GetElementsByTag("p"
                ).First());
            MediaDeviceDescription deviceDescription1 = new MediaDeviceDescription(MediaType.PRINT).SetBitsPerComponent
                (2);
            MediaDeviceDescription deviceDescription2 = new MediaDeviceDescription(MediaType.HANDHELD).SetBitsPerComponent
                (2);
            MediaDeviceDescription deviceDescription3 = new MediaDeviceDescription(MediaType.SCREEN).SetBitsPerComponent
                (1);
            IList<CssDeclaration> declarations1 = css.GetCssDeclarations(element, deviceDescription1);
            IList<CssDeclaration> declarations2 = css.GetCssDeclarations(element, deviceDescription2);
            IList<CssDeclaration> declarations3 = css.GetCssDeclarations(element, deviceDescription3);
            NUnit.Framework.Assert.IsTrue(Enumerable.SequenceEqual(declarations1, declarations2));
            NUnit.Framework.Assert.AreEqual(0, declarations3.Count);
            NUnit.Framework.Assert.AreEqual(1, declarations1.Count);
            NUnit.Framework.Assert.AreEqual("color: red", declarations1[0].ToString());
        }

        [NUnit.Framework.Test]
        public virtual void MatchMediaDeviceTest() {
            MediaDeviceDescription deviceDescription = new MediaDeviceDescription("all");
            deviceDescription.SetHeight(450);
            deviceDescription.SetWidth(600);
            CssMediaRule rule = new CssMediaRule("@media all and (min-width: 600px) and (min-height: 600px)");
            NUnit.Framework.Assert.IsTrue(rule.MatchMediaDevice(deviceDescription));
        }

        [NUnit.Framework.Test]
        public virtual void GetCssRuleSetsTest() {
            MediaDeviceDescription deviceDescription = new MediaDeviceDescription("all");
            String html = "<a id=\"123\" class=\"baz = 'bar'\" style = media= all and (min-width: 600px) />";
            IDocumentNode node = new JsoupHtmlParser().Parse(html);
            IList<CssRuleSet> ruleSets = new CssMediaRule("only all and (min-width: 600px) and (min-height: 600px)").GetCssRuleSets
                (node, deviceDescription);
            NUnit.Framework.Assert.IsNotNull(ruleSets);
        }
    }
}
