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
