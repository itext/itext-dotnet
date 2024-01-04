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
