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
using iText.Svg.Renderers.Factories;
using iText.Test;

namespace iText.Svg.Renderers {
    [NUnit.Framework.Category("UnitTest")]
    public class DefaultSvgNodeRendererFactoryDrawTest : ExtendedITextTest {
        private readonly ISvgNodeRendererFactory fact = new DummySvgNodeFactory();

        [NUnit.Framework.Test]
        public virtual void BasicProcessedRendererTest() {
            iText.StyledXmlParser.Jsoup.Nodes.Element element = new iText.StyledXmlParser.Jsoup.Nodes.Element(iText.StyledXmlParser.Jsoup.Parser.Tag
                .ValueOf("processable"), "");
            IElementNode tag = new JsoupElementNode(element);
            ISvgNodeRenderer renderer = fact.CreateSvgNodeRendererForTag(tag, null);
            NUnit.Framework.Assert.IsTrue(renderer is DummyProcessableSvgNodeRenderer);
            renderer.Draw(new SvgDrawContext(null, null));
            DummyProcessableSvgNodeRenderer processed = (DummyProcessableSvgNodeRenderer)renderer;
            NUnit.Framework.Assert.IsTrue(processed.IsProcessed());
        }

        [NUnit.Framework.Test]
        public virtual void NestedProcessedRendererTest() {
            iText.StyledXmlParser.Jsoup.Nodes.Element parentEl = new iText.StyledXmlParser.Jsoup.Nodes.Element(iText.StyledXmlParser.Jsoup.Parser.Tag
                .ValueOf("processable"), "");
            iText.StyledXmlParser.Jsoup.Nodes.Element childEl = new iText.StyledXmlParser.Jsoup.Nodes.Element(iText.StyledXmlParser.Jsoup.Parser.Tag
                .ValueOf("processable"), "");
            IElementNode parentTag = new JsoupElementNode(parentEl);
            IElementNode childTag = new JsoupElementNode(childEl);
            ISvgNodeRenderer parentRenderer = fact.CreateSvgNodeRendererForTag(parentTag, null);
            ISvgNodeRenderer childRenderer = fact.CreateSvgNodeRendererForTag(childTag, parentRenderer);
            parentRenderer.Draw(new SvgDrawContext(null, null));
            DummyProcessableSvgNodeRenderer parentProcessed = (DummyProcessableSvgNodeRenderer)parentRenderer;
            NUnit.Framework.Assert.IsTrue(parentProcessed.IsProcessed());
            DummyProcessableSvgNodeRenderer childProcessed = (DummyProcessableSvgNodeRenderer)childRenderer;
            // child is not processed unless instructed thus in its parent
            NUnit.Framework.Assert.IsFalse(childProcessed.IsProcessed());
        }
    }
}
