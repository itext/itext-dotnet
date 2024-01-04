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
using System.Collections.Generic;
using System.IO;
using iText.StyledXmlParser;
using iText.StyledXmlParser.Css.Media;
using iText.StyledXmlParser.Css.Parse;
using iText.StyledXmlParser.Node;
using iText.StyledXmlParser.Node.Impl.Jsoup;
using iText.StyledXmlParser.Node.Impl.Jsoup.Node;
using iText.Test;

namespace iText.StyledXmlParser.Css {
    [NUnit.Framework.Category("UnitTest")]
    public class CssMatchingTest : ExtendedITextTest {
        private static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/styledxmlparser/css/CssMatchingTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
        }

        [NUnit.Framework.Test]
        public virtual void Test01() {
            String htmlFileName = sourceFolder + "html01.html";
            String cssFileName = sourceFolder + "css01.css";
            IXmlParser htmlParser = new JsoupHtmlParser();
            IDocumentNode document = htmlParser.Parse(new FileStream(htmlFileName, FileMode.Open, FileAccess.Read), "UTF-8"
                );
            CssStyleSheet css = CssStyleSheetParser.Parse(new FileStream(cssFileName, FileMode.Open, FileAccess.Read));
            MediaDeviceDescription deviceDescription = new MediaDeviceDescription("all");
            IElementNode element = new JsoupElementNode(((JsoupDocumentNode)document).GetDocument().GetElementsByTag("p"
                ).First());
            IList<CssDeclaration> declarations = css.GetCssDeclarations(element, deviceDescription);
            NUnit.Framework.Assert.AreEqual(1, declarations.Count);
            NUnit.Framework.Assert.AreEqual("font-weight: bold", declarations[0].ToString());
        }

        [NUnit.Framework.Test]
        public virtual void Test02() {
            String htmlFileName = sourceFolder + "html02.html";
            String cssFileName = sourceFolder + "css02.css";
            IXmlParser htmlParser = new JsoupHtmlParser();
            IDocumentNode document = htmlParser.Parse(new FileStream(htmlFileName, FileMode.Open, FileAccess.Read), "UTF-8"
                );
            CssStyleSheet css = CssStyleSheetParser.Parse(new FileStream(cssFileName, FileMode.Open, FileAccess.Read));
            MediaDeviceDescription deviceDescription = new MediaDeviceDescription("all");
            IElementNode element = new JsoupElementNode(((JsoupDocumentNode)document).GetDocument().GetElementsByTag("p"
                ).First());
            IList<CssDeclaration> declarations = css.GetCssDeclarations(element, deviceDescription);
            NUnit.Framework.Assert.AreEqual(2, declarations.Count);
            NUnit.Framework.Assert.AreEqual("font-weight: bold", declarations[1].ToString());
            NUnit.Framework.Assert.AreEqual("color: red", declarations[0].ToString());
        }

        [NUnit.Framework.Test]
        public virtual void Test03() {
            String htmlFileName = sourceFolder + "html03.html";
            String cssFileName = sourceFolder + "css03.css";
            IXmlParser htmlParser = new JsoupHtmlParser();
            IDocumentNode document = htmlParser.Parse(new FileStream(htmlFileName, FileMode.Open, FileAccess.Read), "UTF-8"
                );
            CssStyleSheet css = CssStyleSheetParser.Parse(new FileStream(cssFileName, FileMode.Open, FileAccess.Read));
            MediaDeviceDescription deviceDescription = new MediaDeviceDescription("all");
            IElementNode element = new JsoupElementNode(((JsoupDocumentNode)document).GetDocument().GetElementsByTag("p"
                ).First());
            IList<CssDeclaration> declarations = css.GetCssDeclarations(element, deviceDescription);
            NUnit.Framework.Assert.AreEqual(2, declarations.Count);
            NUnit.Framework.Assert.AreEqual("font-weight: bold", declarations[0].ToString());
            NUnit.Framework.Assert.AreEqual("color: black", declarations[1].ToString());
        }

        [NUnit.Framework.Test]
        public virtual void Test04() {
            String htmlFileName = sourceFolder + "html04.html";
            String cssFileName = sourceFolder + "css04.css";
            IXmlParser htmlParser = new JsoupHtmlParser();
            IDocumentNode document = htmlParser.Parse(new FileStream(htmlFileName, FileMode.Open, FileAccess.Read), "UTF-8"
                );
            CssStyleSheet css = CssStyleSheetParser.Parse(new FileStream(cssFileName, FileMode.Open, FileAccess.Read));
            MediaDeviceDescription deviceDescription = new MediaDeviceDescription("all");
            IElementNode element = new JsoupElementNode(((JsoupDocumentNode)document).GetDocument().GetElementsByTag("p"
                ).First());
            IList<CssDeclaration> declarations = css.GetCssDeclarations(element, deviceDescription);
            NUnit.Framework.Assert.AreEqual(1, declarations.Count);
            NUnit.Framework.Assert.AreEqual("font-size: 100px", declarations[0].ToString());
        }

        [NUnit.Framework.Test]
        public virtual void Test05() {
            String htmlFileName = sourceFolder + "html05.html";
            String cssFileName = sourceFolder + "css05.css";
            IXmlParser htmlParser = new JsoupHtmlParser();
            IDocumentNode document = htmlParser.Parse(new FileStream(htmlFileName, FileMode.Open, FileAccess.Read), "UTF-8"
                );
            CssStyleSheet css = CssStyleSheetParser.Parse(new FileStream(cssFileName, FileMode.Open, FileAccess.Read));
            MediaDeviceDescription deviceDescription = new MediaDeviceDescription("all");
            IElementNode element = new JsoupElementNode(((JsoupDocumentNode)document).GetDocument().GetElementsByTag("p"
                ).First());
            IList<CssDeclaration> declarations = css.GetCssDeclarations(element, deviceDescription);
            NUnit.Framework.Assert.AreEqual(1, declarations.Count);
            NUnit.Framework.Assert.AreEqual("color: red", declarations[0].ToString());
        }

        [NUnit.Framework.Test]
        public virtual void Test06() {
            String htmlFileName = sourceFolder + "html06.html";
            String cssFileName = sourceFolder + "css06.css";
            IXmlParser htmlParser = new JsoupHtmlParser();
            IDocumentNode document = htmlParser.Parse(new FileStream(htmlFileName, FileMode.Open, FileAccess.Read), "UTF-8"
                );
            CssStyleSheet css = CssStyleSheetParser.Parse(new FileStream(cssFileName, FileMode.Open, FileAccess.Read));
            MediaDeviceDescription deviceDescription = new MediaDeviceDescription("all");
            IElementNode element = new JsoupElementNode(((JsoupDocumentNode)document).GetDocument().GetElementsByTag("p"
                ).First());
            IList<CssDeclaration> declarations = css.GetCssDeclarations(element, deviceDescription);
            NUnit.Framework.Assert.AreEqual(1, declarations.Count);
            NUnit.Framework.Assert.AreEqual("color: blue", declarations[0].ToString());
        }
    }
}
