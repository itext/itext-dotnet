using System;
using System.Collections.Generic;
using iText.StyledXmlParser.Node;
using iText.StyledXmlParser.Node.Impl.Jsoup.Node;
using iText.Svg.Dummy.Factories;
using iText.Svg.Dummy.Renderers.Impl;
using iText.Svg.Exceptions;
using iText.Svg.Renderers.Factories;

namespace iText.Svg.Renderers {
    public class DefaultSvgNodeRendererFactoryTest {
        private ISvgNodeRendererFactory fact;

        [NUnit.Framework.SetUp]
        public virtual void SetUp() {
            fact = new DefaultSvgNodeRendererFactory(new DummySvgNodeMapper());
        }

        [NUnit.Framework.Test]
        public virtual void NonExistingTagTest() {
            NUnit.Framework.Assert.That(() =>  {
                iText.StyledXmlParser.Jsoup.Nodes.Element nonExistingElement = new iText.StyledXmlParser.Jsoup.Nodes.Element
                    (iText.StyledXmlParser.Jsoup.Parser.Tag.ValueOf("notAnExistingTag"), "");
                IElementNode tag = new JsoupElementNode(nonExistingElement);
                fact.CreateSvgNodeRendererForTag(tag, null);
            }
            , NUnit.Framework.Throws.TypeOf<SvgProcessingException>());
;
        }

        [NUnit.Framework.Test]
        public virtual void ProtectedConstructorTest() {
            NUnit.Framework.Assert.That(() =>  {
                iText.StyledXmlParser.Jsoup.Nodes.Element protectedElement = new iText.StyledXmlParser.Jsoup.Nodes.Element
                    (iText.StyledXmlParser.Jsoup.Parser.Tag.ValueOf("protected"), "");
                IElementNode tag = new JsoupElementNode(protectedElement);
                fact.CreateSvgNodeRendererForTag(tag, null);
            }
            , NUnit.Framework.Throws.TypeOf<SvgProcessingException>());
;
        }

        /// <exception cref="System.MissingMethodException"/>
        [NUnit.Framework.Test]
        public virtual void ProtectedConstructorInnerTest() {
            NUnit.Framework.Assert.That(() =>  {
                iText.StyledXmlParser.Jsoup.Nodes.Element protectedElement = new iText.StyledXmlParser.Jsoup.Nodes.Element
                    (iText.StyledXmlParser.Jsoup.Parser.Tag.ValueOf("protected"), "");
                IElementNode tag = new JsoupElementNode(protectedElement);
                try {
                    fact.CreateSvgNodeRendererForTag(tag, null);
                }
                catch (SvgProcessingException spe) {
                    throw (MissingMethodException)spe.InnerException;
                }
            }
            , NUnit.Framework.Throws.TypeOf<MissingMethodException>());
;
        }

        [NUnit.Framework.Test]
        public virtual void ArgumentedConstructorTest() {
            NUnit.Framework.Assert.That(() =>  {
                iText.StyledXmlParser.Jsoup.Nodes.Element protectedElement = new iText.StyledXmlParser.Jsoup.Nodes.Element
                    (iText.StyledXmlParser.Jsoup.Parser.Tag.ValueOf("argumented"), "");
                IElementNode tag = new JsoupElementNode(protectedElement);
                fact.CreateSvgNodeRendererForTag(tag, null);
            }
            , NUnit.Framework.Throws.TypeOf<SvgProcessingException>());
;
        }

        /// <exception cref="System.MissingMethodException"/>
        [NUnit.Framework.Test]
        public virtual void ArgumentedConstructorInnerTest() {
            NUnit.Framework.Assert.That(() =>  {
                iText.StyledXmlParser.Jsoup.Nodes.Element protectedElement = new iText.StyledXmlParser.Jsoup.Nodes.Element
                    (iText.StyledXmlParser.Jsoup.Parser.Tag.ValueOf("argumented"), "");
                IElementNode tag = new JsoupElementNode(protectedElement);
                try {
                    fact.CreateSvgNodeRendererForTag(tag, null);
                }
                catch (SvgProcessingException spe) {
                    throw (MissingMethodException)spe.InnerException;
                }
            }
            , NUnit.Framework.Throws.TypeOf<MissingMethodException>());
;
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
            NUnit.Framework.Assert.AreEqual(1, parentRenderer.GetChildren().Count);
            NUnit.Framework.Assert.AreEqual(childRenderer, parentRenderer.GetChildren()[0]);
        }

        private class FaultyTestMapper : ISvgNodeRendererMapper {
            public virtual IDictionary<String, Type> GetMapping() {
                throw new Exception();
            }
        }

        /// <summary>Tests that exception is already thrown in constructor</summary>
        [NUnit.Framework.Test]
        public virtual void FaultyMapperTest() {
            NUnit.Framework.Assert.That(() =>  {
                fact = new DefaultSvgNodeRendererFactory(new DefaultSvgNodeRendererFactoryTest.FaultyTestMapper());
            }
            , NUnit.Framework.Throws.TypeOf<Exception>());
;
        }
    }
}
