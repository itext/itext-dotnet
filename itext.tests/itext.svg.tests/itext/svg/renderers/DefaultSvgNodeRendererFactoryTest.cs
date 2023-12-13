/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 Apryse Group NV
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
using iText.StyledXmlParser.Node;
using iText.StyledXmlParser.Node.Impl.Jsoup.Node;
using iText.Svg.Dummy.Factories;
using iText.Svg.Dummy.Renderers.Impl;
using iText.Svg.Exceptions;
using iText.Svg.Renderers.Factories;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Svg.Renderers {
    public class DefaultSvgNodeRendererFactoryTest : ExtendedITextTest {
        private ISvgNodeRendererFactory fact;

        [NUnit.Framework.SetUp]
        public virtual void SetUp() {
            fact = new DefaultSvgNodeRendererFactory(new DummySvgNodeMapper());
        }

        [NUnit.Framework.Test]
        [LogMessage(SvgLogMessageConstant.UNMAPPEDTAG)]
        public virtual void NonExistingTagTest() {
            iText.StyledXmlParser.Jsoup.Nodes.Element nonExistingElement = new iText.StyledXmlParser.Jsoup.Nodes.Element
                (iText.StyledXmlParser.Jsoup.Parser.Tag.ValueOf("notAnExistingTag"), "");
            IElementNode tag = new JsoupElementNode(nonExistingElement);
            fact.CreateSvgNodeRendererForTag(tag, null);
        }

        [NUnit.Framework.Test]
        public virtual void ProtectedConstructorTest() {
            iText.StyledXmlParser.Jsoup.Nodes.Element protectedElement = new iText.StyledXmlParser.Jsoup.Nodes.Element
                (iText.StyledXmlParser.Jsoup.Parser.Tag.ValueOf("protected"), "");
            IElementNode tag = new JsoupElementNode(protectedElement);
            NUnit.Framework.Assert.Catch(typeof(SvgProcessingException), () => fact.CreateSvgNodeRendererForTag(tag, null
                ));
        }

        [NUnit.Framework.Test]
        public virtual void ProtectedConstructorInnerTest() {
            iText.StyledXmlParser.Jsoup.Nodes.Element protectedElement = new iText.StyledXmlParser.Jsoup.Nodes.Element
                (iText.StyledXmlParser.Jsoup.Parser.Tag.ValueOf("protected"), "");
            IElementNode tag = new JsoupElementNode(protectedElement);
            Exception e = NUnit.Framework.Assert.Catch(typeof(SvgProcessingException), () => fact.CreateSvgNodeRendererForTag
                (tag, null));
            NUnit.Framework.Assert.IsTrue(e.InnerException is MissingMethodException);
        }

        [NUnit.Framework.Test]
        public virtual void ArgumentedConstructorTest() {
            iText.StyledXmlParser.Jsoup.Nodes.Element protectedElement = new iText.StyledXmlParser.Jsoup.Nodes.Element
                (iText.StyledXmlParser.Jsoup.Parser.Tag.ValueOf("argumented"), "");
            IElementNode tag = new JsoupElementNode(protectedElement);
            NUnit.Framework.Assert.Catch(typeof(SvgProcessingException), () => NUnit.Framework.Assert.IsNull(fact.CreateSvgNodeRendererForTag
                (tag, null)));
        }

        [NUnit.Framework.Test]
        public virtual void ArgumentedConstructorInnerTest() {
            iText.StyledXmlParser.Jsoup.Nodes.Element protectedElement = new iText.StyledXmlParser.Jsoup.Nodes.Element
                (iText.StyledXmlParser.Jsoup.Parser.Tag.ValueOf("argumented"), "");
            IElementNode tag = new JsoupElementNode(protectedElement);
            Exception e = NUnit.Framework.Assert.Catch(typeof(SvgProcessingException), () => fact.CreateSvgNodeRendererForTag
                (tag, null));
            NUnit.Framework.Assert.IsTrue(e.InnerException is MissingMethodException);
        }

        [NUnit.Framework.Test]
        public virtual void RootTagTest() {
            iText.StyledXmlParser.Jsoup.Nodes.Element element = new iText.StyledXmlParser.Jsoup.Nodes.Element(iText.StyledXmlParser.Jsoup.Parser.Tag
                .ValueOf("dummy"), "");
            IElementNode tag = new JsoupElementNode(element);
            ISvgNodeRenderer childRenderer = fact.CreateSvgNodeRendererForTag(tag, null);
            NUnit.Framework.Assert.IsTrue(childRenderer is DummySvgNodeRenderer);
        }

        private class LocalTestMapper : ISvgNodeRendererMapper {
            public virtual IDictionary<String, Type> GetMapping() {
                IDictionary<String, Type> result = new Dictionary<String, Type>();
                result.Put("test", typeof(DummyProcessableSvgNodeRenderer));
                return result;
            }

            public virtual ICollection<String> GetIgnoredTags() {
                return new List<String>();
            }
        }

        [NUnit.Framework.Test]
        public virtual void CustomMapperTest() {
            fact = new DefaultSvgNodeRendererFactory(new DefaultSvgNodeRendererFactoryTest.LocalTestMapper());
            iText.StyledXmlParser.Jsoup.Nodes.Element element = new iText.StyledXmlParser.Jsoup.Nodes.Element(iText.StyledXmlParser.Jsoup.Parser.Tag
                .ValueOf("test"), "");
            IElementNode tag = new JsoupElementNode(element);
            ISvgNodeRenderer rend = fact.CreateSvgNodeRendererForTag(tag, null);
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

        private class FaultyTestMapper : ISvgNodeRendererMapper {
            public virtual IDictionary<String, Type> GetMapping() {
                throw new Exception();
            }

            public virtual ICollection<String> GetIgnoredTags() {
                return null;
            }
        }

        /// <summary>Tests that exception is already thrown in constructor</summary>
        [NUnit.Framework.Test]
        public virtual void FaultyMapperTest() {
            NUnit.Framework.Assert.Catch(typeof(Exception), () => new DefaultSvgNodeRendererFactory(new DefaultSvgNodeRendererFactoryTest.FaultyTestMapper
                ()));
        }
    }
}
