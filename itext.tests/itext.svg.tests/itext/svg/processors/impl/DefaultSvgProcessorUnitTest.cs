/*
This file is part of the iText (R) project.
Copyright (c) 1998-2018 iText Group NV
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
using iText.StyledXmlParser.Css;
using iText.StyledXmlParser.Node;
using iText.StyledXmlParser.Node.Impl.Jsoup.Node;
using iText.Svg.Dummy.Processors.Impl;
using iText.Svg.Dummy.Renderers.Impl;
using iText.Svg.Exceptions;
using iText.Svg.Processors;
using iText.Svg.Renderers;
using iText.Svg.Renderers.Factories;

namespace iText.Svg.Processors.Impl {
    public class DefaultSvgProcessorUnitTest {
        //Main success scenario
        /// <summary>Simple correct example</summary>
        [NUnit.Framework.Test]
        public virtual void DummyProcessingTestCorrectSimple() {
            //Setup nodes
            iText.StyledXmlParser.Jsoup.Nodes.Element jsoupSVGRoot = new iText.StyledXmlParser.Jsoup.Nodes.Element(iText.StyledXmlParser.Jsoup.Parser.Tag
                .ValueOf("svg"), "");
            iText.StyledXmlParser.Jsoup.Nodes.Element jsoupSVGCircle = new iText.StyledXmlParser.Jsoup.Nodes.Element(iText.StyledXmlParser.Jsoup.Parser.Tag
                .ValueOf("circle"), "");
            iText.StyledXmlParser.Jsoup.Nodes.Element jsoupSVGPath = new iText.StyledXmlParser.Jsoup.Nodes.Element(iText.StyledXmlParser.Jsoup.Parser.Tag
                .ValueOf("path"), "");
            INode root = null;
            root = new JsoupElementNode(jsoupSVGRoot);
            root.AddChild(new JsoupElementNode(jsoupSVGCircle));
            root.AddChild(new JsoupElementNode(jsoupSVGPath));
            //Run
            DefaultSvgProcessor processor = new DefaultSvgProcessor();
            ISvgConverterProperties props = new DummySvgConverterProperties();
            ISvgNodeRenderer rootActual = processor.Process(root, props).GetRootRenderer();
            //setup expected
            IBranchSvgNodeRenderer rootExpected = new DummyBranchSvgNodeRenderer("svg");
            rootExpected.AddChild(new DummySvgNodeRenderer("circle"));
            rootExpected.AddChild(new DummySvgNodeRenderer("path"));
            //Compare
            NUnit.Framework.Assert.AreEqual(rootActual, rootExpected);
        }

        [NUnit.Framework.Test]
        public virtual void DummyProcessingTestCorrectNested() {
            //Setup nodes
            iText.StyledXmlParser.Jsoup.Nodes.Element jsoupSVGRoot = new iText.StyledXmlParser.Jsoup.Nodes.Element(iText.StyledXmlParser.Jsoup.Parser.Tag
                .ValueOf("svg"), "");
            iText.StyledXmlParser.Jsoup.Nodes.Element jsoupSVGCircle = new iText.StyledXmlParser.Jsoup.Nodes.Element(iText.StyledXmlParser.Jsoup.Parser.Tag
                .ValueOf("circle"), "");
            iText.StyledXmlParser.Jsoup.Nodes.Element jsoupSVGPath = new iText.StyledXmlParser.Jsoup.Nodes.Element(iText.StyledXmlParser.Jsoup.Parser.Tag
                .ValueOf("path"), "");
            INode root = null;
            root = new JsoupElementNode(jsoupSVGRoot);
            root.AddChild(new JsoupElementNode(jsoupSVGCircle));
            root.AddChild(new JsoupElementNode(jsoupSVGPath));
            INode nestedSvg = new JsoupElementNode(jsoupSVGRoot);
            nestedSvg.AddChild(new JsoupElementNode(jsoupSVGCircle));
            nestedSvg.AddChild(new JsoupElementNode(jsoupSVGCircle));
            root.AddChild(nestedSvg);
            //Run
            DefaultSvgProcessor processor = new DefaultSvgProcessor();
            ISvgConverterProperties props = new DummySvgConverterProperties();
            ISvgNodeRenderer rootActual = processor.Process(root, props).GetRootRenderer();
            //setup expected
            IBranchSvgNodeRenderer rootExpected = new DummyBranchSvgNodeRenderer("svg");
            rootExpected.AddChild(new DummySvgNodeRenderer("circle"));
            rootExpected.AddChild(new DummySvgNodeRenderer("path"));
            IBranchSvgNodeRenderer nestedSvgRend = new DummyBranchSvgNodeRenderer("svg");
            nestedSvgRend.AddChild(new DummySvgNodeRenderer("circle"));
            nestedSvgRend.AddChild(new DummySvgNodeRenderer("circle"));
            rootExpected.AddChild(nestedSvgRend);
            //Compare
            NUnit.Framework.Assert.AreEqual(rootActual, rootExpected);
        }

        //Edge cases
        [NUnit.Framework.Test]
        public virtual void DummyProcessingTestNodeHasNullChild() {
            /*
            Invalid input: null
            */
            iText.StyledXmlParser.Jsoup.Nodes.Element jsoupSVGRoot = new iText.StyledXmlParser.Jsoup.Nodes.Element(iText.StyledXmlParser.Jsoup.Parser.Tag
                .ValueOf("svg"), "");
            iText.StyledXmlParser.Jsoup.Nodes.Element jsoupSVGCircle = new iText.StyledXmlParser.Jsoup.Nodes.Element(iText.StyledXmlParser.Jsoup.Parser.Tag
                .ValueOf("circle"), "");
            INode root = new JsoupElementNode(jsoupSVGRoot);
            root.AddChild(new JsoupElementNode(jsoupSVGCircle));
            root.AddChild(null);
            root.AddChild(new JsoupElementNode(jsoupSVGCircle));
            //Run
            DefaultSvgProcessor processor = new DefaultSvgProcessor();
            ISvgConverterProperties props = new DummySvgConverterProperties();
            ISvgNodeRenderer rootActual = processor.Process(root, props).GetRootRenderer();
            //setup expected
            ISvgNodeRenderer rootExpected = new DummySvgNodeRenderer("svg");
        }

        [NUnit.Framework.Test]
        public virtual void DummyProcessingSvgTagIsNotRootOfInput() {
            iText.StyledXmlParser.Jsoup.Nodes.Element jsoupRandomElement = new iText.StyledXmlParser.Jsoup.Nodes.Element
                (iText.StyledXmlParser.Jsoup.Parser.Tag.ValueOf("body"), "");
            iText.StyledXmlParser.Jsoup.Nodes.Element jsoupSVGRoot = new iText.StyledXmlParser.Jsoup.Nodes.Element(iText.StyledXmlParser.Jsoup.Parser.Tag
                .ValueOf("svg"), "");
            iText.StyledXmlParser.Jsoup.Nodes.Element jsoupSVGCircle = new iText.StyledXmlParser.Jsoup.Nodes.Element(iText.StyledXmlParser.Jsoup.Parser.Tag
                .ValueOf("circle"), "");
            INode root = new JsoupElementNode(jsoupRandomElement);
            INode svg = new JsoupElementNode(jsoupSVGRoot);
            svg.AddChild(new JsoupElementNode(jsoupSVGCircle));
            root.AddChild(svg);
            //Run
            DefaultSvgProcessor processor = new DefaultSvgProcessor();
            ISvgConverterProperties props = new DummySvgConverterProperties();
            ISvgNodeRenderer rootActual = processor.Process(root, props).GetRootRenderer();
            //setup expected
            IBranchSvgNodeRenderer rootExpected = new DummyBranchSvgNodeRenderer("svg");
            rootExpected.AddChild(new DummySvgNodeRenderer("circle"));
            NUnit.Framework.Assert.AreEqual(rootActual, rootExpected);
        }

        [NUnit.Framework.Test]
        public virtual void DummyProcessingNoSvgTagInInput() {
            NUnit.Framework.Assert.That(() =>  {
                iText.StyledXmlParser.Jsoup.Nodes.Element jsoupSVGRoot = new iText.StyledXmlParser.Jsoup.Nodes.Element(iText.StyledXmlParser.Jsoup.Parser.Tag
                    .ValueOf("polygon"), "");
                iText.StyledXmlParser.Jsoup.Nodes.Element jsoupSVGCircle = new iText.StyledXmlParser.Jsoup.Nodes.Element(iText.StyledXmlParser.Jsoup.Parser.Tag
                    .ValueOf("circle"), "");
                INode root = new JsoupElementNode(jsoupSVGRoot);
                root.AddChild(new JsoupElementNode(jsoupSVGCircle));
                //Run
                DefaultSvgProcessor processor = new DefaultSvgProcessor();
                ISvgConverterProperties props = new DummySvgConverterProperties();
                ISvgNodeRenderer rootActual = processor.Process(root, props).GetRootRenderer();
            }
            , NUnit.Framework.Throws.TypeOf<SvgProcessingException>().With.Message.EqualTo(SvgLogMessageConstant.NOROOT));
;
        }

        [NUnit.Framework.Test]
        public virtual void DummyProcessingTestNullInput() {
            NUnit.Framework.Assert.That(() =>  {
                DefaultSvgProcessor processor = new DefaultSvgProcessor();
                processor.Process(null);
            }
            , NUnit.Framework.Throws.TypeOf<SvgProcessingException>());
;
        }

        [NUnit.Framework.Ignore("TODO: Implement Tree comparison. Blocked by RND-868\n")]
        [NUnit.Framework.Test]
        public virtual void DefaultProcessingTestNoPassedProperties() {
            //Setup nodes
            iText.StyledXmlParser.Jsoup.Nodes.Element jsoupSVGRoot = new iText.StyledXmlParser.Jsoup.Nodes.Element(iText.StyledXmlParser.Jsoup.Parser.Tag
                .ValueOf("svg"), "");
            iText.StyledXmlParser.Jsoup.Nodes.Element jsoupSVGCircle = new iText.StyledXmlParser.Jsoup.Nodes.Element(iText.StyledXmlParser.Jsoup.Parser.Tag
                .ValueOf("circle"), "");
            iText.StyledXmlParser.Jsoup.Nodes.Element jsoupSVGPath = new iText.StyledXmlParser.Jsoup.Nodes.Element(iText.StyledXmlParser.Jsoup.Parser.Tag
                .ValueOf("path"), "");
            INode root = null;
            root = new JsoupElementNode(jsoupSVGRoot);
            root.AddChild(new JsoupElementNode(jsoupSVGCircle));
            root.AddChild(new JsoupElementNode(jsoupSVGPath));
            //Run
            DefaultSvgProcessor processor = new DefaultSvgProcessor();
            ISvgNodeRenderer rootActual = processor.Process(root).GetRootRenderer();
            //setup expected
            ISvgNodeRenderer rootExpected = null;
            //Compare
            NUnit.Framework.Assert.AreEqual(rootActual, rootExpected);
        }

        [NUnit.Framework.Ignore("TODO: Implement Tree comparison. Blocked by RND-868\n")]
        [NUnit.Framework.Test]
        public virtual void DefaultProcessingTestPassedPropertiesNull() {
            //Setup nodes
            iText.StyledXmlParser.Jsoup.Nodes.Element jsoupSVGRoot = new iText.StyledXmlParser.Jsoup.Nodes.Element(iText.StyledXmlParser.Jsoup.Parser.Tag
                .ValueOf("svg"), "");
            iText.StyledXmlParser.Jsoup.Nodes.Element jsoupSVGCircle = new iText.StyledXmlParser.Jsoup.Nodes.Element(iText.StyledXmlParser.Jsoup.Parser.Tag
                .ValueOf("circle"), "");
            iText.StyledXmlParser.Jsoup.Nodes.Element jsoupSVGPath = new iText.StyledXmlParser.Jsoup.Nodes.Element(iText.StyledXmlParser.Jsoup.Parser.Tag
                .ValueOf("path"), "");
            INode root = null;
            root = new JsoupElementNode(jsoupSVGRoot);
            root.AddChild(new JsoupElementNode(jsoupSVGCircle));
            root.AddChild(new JsoupElementNode(jsoupSVGPath));
            //Run
            DefaultSvgProcessor processor = new DefaultSvgProcessor();
            ISvgNodeRenderer rootActual = processor.Process(root, null).GetRootRenderer();
            //setup expected
            ISvgNodeRenderer rootExpected = null;
            //Compare
            NUnit.Framework.Assert.AreEqual(rootActual, rootExpected);
        }

        [NUnit.Framework.Ignore("TODO: Implement Tree comparison. Blocked by RND-868\n")]
        [NUnit.Framework.Test]
        public virtual void DefaultProcessingTestPassedPropertiesReturnNullValues() {
            //Setup nodes
            iText.StyledXmlParser.Jsoup.Nodes.Element jsoupSVGRoot = new iText.StyledXmlParser.Jsoup.Nodes.Element(iText.StyledXmlParser.Jsoup.Parser.Tag
                .ValueOf("svg"), "");
            iText.StyledXmlParser.Jsoup.Nodes.Element jsoupSVGCircle = new iText.StyledXmlParser.Jsoup.Nodes.Element(iText.StyledXmlParser.Jsoup.Parser.Tag
                .ValueOf("circle"), "");
            iText.StyledXmlParser.Jsoup.Nodes.Element jsoupSVGPath = new iText.StyledXmlParser.Jsoup.Nodes.Element(iText.StyledXmlParser.Jsoup.Parser.Tag
                .ValueOf("path"), "");
            INode root = null;
            root = new JsoupElementNode(jsoupSVGRoot);
            root.AddChild(new JsoupElementNode(jsoupSVGCircle));
            root.AddChild(new JsoupElementNode(jsoupSVGPath));
            //Run
            DefaultSvgProcessor processor = new DefaultSvgProcessor();
            ISvgConverterProperties convProps = new _ISvgConverterProperties_251();
            ISvgNodeRenderer rootActual = processor.Process(root, convProps).GetRootRenderer();
            //setup expected
            ISvgNodeRenderer rootExpected = null;
            //Compare
            NUnit.Framework.Assert.AreEqual(rootActual, rootExpected);
        }

        private sealed class _ISvgConverterProperties_251 : ISvgConverterProperties {
            public _ISvgConverterProperties_251() {
            }

            public ICssResolver GetCssResolver() {
                return null;
            }

            public ISvgNodeRendererFactory GetRendererFactory() {
                return null;
            }

            public String GetCharset() {
                return null;
            }
        }

        [NUnit.Framework.Test]
        public virtual void FindFirstElementNullTest() {
            DefaultSvgProcessor processor = new DefaultSvgProcessor();
            IElementNode actual = processor.FindFirstElement(null, "name");
            NUnit.Framework.Assert.IsNull(actual);
        }
    }
}
