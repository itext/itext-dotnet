/*
This file is part of the iText (R) project.
Copyright (c) 1998-2020 iText Group NV
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
            NUnit.Framework.Assert.That(() =>  {
                iText.StyledXmlParser.Jsoup.Nodes.Element protectedElement = new iText.StyledXmlParser.Jsoup.Nodes.Element
                    (iText.StyledXmlParser.Jsoup.Parser.Tag.ValueOf("protected"), "");
                IElementNode tag = new JsoupElementNode(protectedElement);
                fact.CreateSvgNodeRendererForTag(tag, null);
            }
            , NUnit.Framework.Throws.InstanceOf<SvgProcessingException>())
;
        }

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
            , NUnit.Framework.Throws.InstanceOf<MissingMethodException>())
;
        }

        [NUnit.Framework.Test]
        public virtual void ArgumentedConstructorTest() {
            NUnit.Framework.Assert.That(() =>  {
                iText.StyledXmlParser.Jsoup.Nodes.Element protectedElement = new iText.StyledXmlParser.Jsoup.Nodes.Element
                    (iText.StyledXmlParser.Jsoup.Parser.Tag.ValueOf("argumented"), "");
                IElementNode tag = new JsoupElementNode(protectedElement);
                NUnit.Framework.Assert.IsNull(fact.CreateSvgNodeRendererForTag(tag, null));
            }
            , NUnit.Framework.Throws.InstanceOf<SvgProcessingException>())
;
        }

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
            , NUnit.Framework.Throws.InstanceOf<MissingMethodException>())
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
            NUnit.Framework.Assert.That(() =>  {
                fact = new DefaultSvgNodeRendererFactory(new DefaultSvgNodeRendererFactoryTest.FaultyTestMapper());
            }
            , NUnit.Framework.Throws.InstanceOf<Exception>())
;
        }
    }
}
