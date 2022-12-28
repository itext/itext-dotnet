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
using iText.StyledXmlParser.Node;
using iText.StyledXmlParser.Node.Impl.Jsoup.Node;
using iText.Svg.Dummy.Factories;
using iText.Svg.Dummy.Renderers.Impl;
using iText.Svg.Logs;
using iText.Svg.Renderers.Factories;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Svg.Renderers {
    [NUnit.Framework.Category("UnitTest")]
    public class DefaultSvgNodeRendererFactoryTest : ExtendedITextTest {
        private readonly ISvgNodeRendererFactory fact = new DummySvgNodeFactory();

        [NUnit.Framework.Test]
        [LogMessage(SvgLogMessageConstant.UNMAPPED_TAG)]
        public virtual void NonExistingTagTest() {
            ISvgNodeRendererFactory factory = new DefaultSvgNodeRendererFactory();
            iText.StyledXmlParser.Jsoup.Nodes.Element nonExistingElement = new iText.StyledXmlParser.Jsoup.Nodes.Element
                (iText.StyledXmlParser.Jsoup.Parser.Tag.ValueOf("notAnExistingTag"), "");
            IElementNode tag = new JsoupElementNode(nonExistingElement);
            factory.CreateSvgNodeRendererForTag(tag, null);
        }

        [NUnit.Framework.Test]
        public virtual void ArgumentedConstructorTest() {
            iText.StyledXmlParser.Jsoup.Nodes.Element protectedElement = new iText.StyledXmlParser.Jsoup.Nodes.Element
                (iText.StyledXmlParser.Jsoup.Parser.Tag.ValueOf("argumented"), "");
            IElementNode tag = new JsoupElementNode(protectedElement);
            ISvgNodeRenderer renderer = fact.CreateSvgNodeRendererForTag(tag, null);
            NUnit.Framework.Assert.IsTrue(renderer is DummyArgumentedConstructorSvgNodeRenderer);
            NUnit.Framework.Assert.AreEqual(15, ((DummyArgumentedConstructorSvgNodeRenderer)renderer).number);
        }

        [NUnit.Framework.Test]
        public virtual void RootTagTest() {
            iText.StyledXmlParser.Jsoup.Nodes.Element element = new iText.StyledXmlParser.Jsoup.Nodes.Element(iText.StyledXmlParser.Jsoup.Parser.Tag
                .ValueOf("dummy"), "");
            IElementNode tag = new JsoupElementNode(element);
            ISvgNodeRenderer childRenderer = fact.CreateSvgNodeRendererForTag(tag, null);
            NUnit.Framework.Assert.IsTrue(childRenderer is DummySvgNodeRenderer);
        }

        private class LocalSvgNodeRendererFactory : DefaultSvgNodeRendererFactory {
            public override ISvgNodeRenderer CreateSvgNodeRendererForTag(IElementNode tag, ISvgNodeRenderer parent) {
                ISvgNodeRenderer result;
                if ("test".Equals(tag.Name())) {
                    result = new DummyProcessableSvgNodeRenderer();
                    result.SetParent(parent);
                    return result;
                }
                else {
                    return null;
                }
            }
        }

        [NUnit.Framework.Test]
        public virtual void CustomMapperTest() {
            ISvgNodeRendererFactory factory = new DefaultSvgNodeRendererFactoryTest.LocalSvgNodeRendererFactory();
            iText.StyledXmlParser.Jsoup.Nodes.Element element = new iText.StyledXmlParser.Jsoup.Nodes.Element(iText.StyledXmlParser.Jsoup.Parser.Tag
                .ValueOf("test"), "");
            IElementNode tag = new JsoupElementNode(element);
            ISvgNodeRenderer rend = factory.CreateSvgNodeRendererForTag(tag, null);
            NUnit.Framework.Assert.IsTrue(rend is DummyProcessableSvgNodeRenderer);
        }

        [NUnit.Framework.Test]
        public virtual void HierarchyTagTest() {
            iText.StyledXmlParser.Jsoup.Nodes.Element parentEl = new iText.StyledXmlParser.Jsoup.Nodes.Element(iText.StyledXmlParser.Jsoup.Parser.Tag
                .ValueOf("dummy"), "");
            IElementNode parentTag = new JsoupElementNode(parentEl);
            iText.StyledXmlParser.Jsoup.Nodes.Element childEl = new iText.StyledXmlParser.Jsoup.Nodes.Element(iText.StyledXmlParser.Jsoup.Parser.Tag
                .ValueOf("dummy"), "");
            IElementNode childTag = new JsoupElementNode(childEl);
            ISvgNodeRenderer parentRenderer = fact.CreateSvgNodeRendererForTag(parentTag, null);
            ISvgNodeRenderer childRenderer = fact.CreateSvgNodeRendererForTag(childTag, parentRenderer);
            NUnit.Framework.Assert.AreEqual(parentRenderer, childRenderer.GetParent());
        }
    }
}
