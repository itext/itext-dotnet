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
using iText.IO.Util;
using iText.StyledXmlParser.Node;
using iText.StyledXmlParser.Node.Impl.Jsoup.Node;
using iText.Svg;
using iText.Svg.Dummy.Processors.Impl;
using iText.Svg.Dummy.Renderers.Impl;
using iText.Svg.Exceptions;
using iText.Svg.Processors;
using iText.Svg.Renderers;
using iText.Svg.Renderers.Factories;
using iText.Svg.Renderers.Impl;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Svg.Processors.Impl {
    public class DefaultSvgProcessorUnitTest : ExtendedITextTest {
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
            INode root = new JsoupElementNode(jsoupSVGRoot);
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
            INode root = new JsoupElementNode(jsoupSVGRoot);
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
        /*
        Invalid input: null
        */
        [LogMessage(iText.StyledXmlParser.LogMessageConstant.ERROR_ADDING_CHILD_NODE)]
        public virtual void DummyProcessingTestNodeHasNullChild() {
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
            NUnit.Framework.Assert.AreEqual(rootExpected, rootActual);
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.StyledXmlParser.LogMessageConstant.ERROR_RESOLVING_PARENT_STYLES)]
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
                processor.Process(root, props).GetRootRenderer();
            }
            , NUnit.Framework.Throws.InstanceOf<SvgProcessingException>().With.Message.EqualTo(SvgLogMessageConstant.NOROOT))
;
        }

        [NUnit.Framework.Test]
        public virtual void DummyProcessingTestNullInput() {
            NUnit.Framework.Assert.That(() =>  {
                DefaultSvgProcessor processor = new DefaultSvgProcessor();
                processor.Process(null);
            }
            , NUnit.Framework.Throws.InstanceOf<SvgProcessingException>())
;
        }

        [NUnit.Framework.Ignore("TODO: Implement Tree comparison. Blocked by DEVSIX-2253\n")]
        [NUnit.Framework.Test]
        public virtual void DefaultProcessingTestNoPassedProperties() {
            //Setup nodes
            iText.StyledXmlParser.Jsoup.Nodes.Element jsoupSVGRoot = new iText.StyledXmlParser.Jsoup.Nodes.Element(iText.StyledXmlParser.Jsoup.Parser.Tag
                .ValueOf("svg"), "");
            iText.StyledXmlParser.Jsoup.Nodes.Element jsoupSVGCircle = new iText.StyledXmlParser.Jsoup.Nodes.Element(iText.StyledXmlParser.Jsoup.Parser.Tag
                .ValueOf("circle"), "");
            iText.StyledXmlParser.Jsoup.Nodes.Element jsoupSVGPath = new iText.StyledXmlParser.Jsoup.Nodes.Element(iText.StyledXmlParser.Jsoup.Parser.Tag
                .ValueOf("path"), "");
            INode root = new JsoupElementNode(jsoupSVGRoot);
            root.AddChild(new JsoupElementNode(jsoupSVGCircle));
            root.AddChild(new JsoupElementNode(jsoupSVGPath));
            //Run
            DefaultSvgProcessor processor = new DefaultSvgProcessor();
            ISvgNodeRenderer rootActual = processor.Process(root).GetRootRenderer();
            //Compare
            NUnit.Framework.Assert.IsNull(rootActual);
        }

        [NUnit.Framework.Ignore("TODO: Implement Tree comparison. Blocked by DEVSIX-2253\n")]
        [NUnit.Framework.Test]
        public virtual void DefaultProcessingTestPassedPropertiesNull() {
            //Setup nodes
            iText.StyledXmlParser.Jsoup.Nodes.Element jsoupSVGRoot = new iText.StyledXmlParser.Jsoup.Nodes.Element(iText.StyledXmlParser.Jsoup.Parser.Tag
                .ValueOf("svg"), "");
            iText.StyledXmlParser.Jsoup.Nodes.Element jsoupSVGCircle = new iText.StyledXmlParser.Jsoup.Nodes.Element(iText.StyledXmlParser.Jsoup.Parser.Tag
                .ValueOf("circle"), "");
            iText.StyledXmlParser.Jsoup.Nodes.Element jsoupSVGPath = new iText.StyledXmlParser.Jsoup.Nodes.Element(iText.StyledXmlParser.Jsoup.Parser.Tag
                .ValueOf("path"), "");
            INode root = new JsoupElementNode(jsoupSVGRoot);
            root.AddChild(new JsoupElementNode(jsoupSVGCircle));
            root.AddChild(new JsoupElementNode(jsoupSVGPath));
            //Run
            DefaultSvgProcessor processor = new DefaultSvgProcessor();
            ISvgNodeRenderer rootActual = processor.Process(root, null).GetRootRenderer();
            //Compare
            NUnit.Framework.Assert.IsNull(rootActual);
        }

        [NUnit.Framework.Ignore("TODO: Implement Tree comparison. Blocked by DEVSIX-2253\n")]
        [NUnit.Framework.Test]
        public virtual void DefaultProcessingTestPassedPropertiesReturnNullValues() {
            //Setup nodes
            iText.StyledXmlParser.Jsoup.Nodes.Element jsoupSVGRoot = new iText.StyledXmlParser.Jsoup.Nodes.Element(iText.StyledXmlParser.Jsoup.Parser.Tag
                .ValueOf("svg"), "");
            iText.StyledXmlParser.Jsoup.Nodes.Element jsoupSVGCircle = new iText.StyledXmlParser.Jsoup.Nodes.Element(iText.StyledXmlParser.Jsoup.Parser.Tag
                .ValueOf("circle"), "");
            iText.StyledXmlParser.Jsoup.Nodes.Element jsoupSVGPath = new iText.StyledXmlParser.Jsoup.Nodes.Element(iText.StyledXmlParser.Jsoup.Parser.Tag
                .ValueOf("path"), "");
            INode root = new JsoupElementNode(jsoupSVGRoot);
            root.AddChild(new JsoupElementNode(jsoupSVGCircle));
            root.AddChild(new JsoupElementNode(jsoupSVGPath));
            //Run
            DefaultSvgProcessor processor = new DefaultSvgProcessor();
            ISvgConverterProperties convProps = new DefaultSvgProcessorUnitTest.EmptySvgConverterProperties();
            ISvgNodeRenderer rootActual = processor.Process(root, convProps).GetRootRenderer();
            //Compare
            NUnit.Framework.Assert.IsNull(rootActual);
        }

        [NUnit.Framework.Test]
        public virtual void FindFirstElementNullTest() {
            DefaultSvgProcessor processor = new DefaultSvgProcessor();
            IElementNode actual = processor.FindFirstElement(null, "name");
            NUnit.Framework.Assert.IsNull(actual);
        }

        [NUnit.Framework.Test]
        [NUnit.Framework.Ignore("DEVSIX-2253")]
        public virtual void ProcessWithNullPropertiesTest() {
            DefaultSvgProcessor processor = new DefaultSvgProcessor();
            iText.StyledXmlParser.Jsoup.Nodes.Element jsoupSVGRoot = new iText.StyledXmlParser.Jsoup.Nodes.Element(iText.StyledXmlParser.Jsoup.Parser.Tag
                .ValueOf("svg"), "");
            INode root = new JsoupElementNode(jsoupSVGRoot);
            ISvgProcessorResult actual = processor.Process(root, null);
            ISvgProcessorResult expected = processor.Process(root);
            NUnit.Framework.Assert.AreEqual(expected.GetRootRenderer(), actual.GetRootRenderer());
        }

        [NUnit.Framework.Test]
        [LogMessage(SvgLogMessageConstant.UNMAPPEDTAG)]
        public virtual void DepthFirstNullRendererTest() {
            iText.StyledXmlParser.Jsoup.Nodes.Element jsoupNonExistingElement = new iText.StyledXmlParser.Jsoup.Nodes.Element
                (iText.StyledXmlParser.Jsoup.Parser.Tag.ValueOf("nonExisting"), "");
            INode root = new JsoupElementNode(jsoupNonExistingElement);
            DefaultSvgProcessor dsp = new DefaultSvgProcessor();
            ISvgConverterProperties scp = new SvgConverterProperties();
            dsp.PerformSetup(root, scp);
            // below method must not throw a NullPointerException
            dsp.ExecuteDepthFirstTraversal(root);
        }

        [NUnit.Framework.Test]
        public virtual void XLinkAttributeBaseDirDoesNotExistTest() {
            INode root = CreateSvgContainingImage();
            String resolvedBaseUrl = "/i7j/itextcore";
            String baseUrl = resolvedBaseUrl + "/wrongDirName";
            ISvgConverterProperties props = new SvgConverterProperties().SetBaseUri(baseUrl);
            SvgTagSvgNodeRenderer rootActual = (SvgTagSvgNodeRenderer)Processor().Process(root, props).GetRootRenderer
                ();
            String fileName = resolvedBaseUrl + "/img.png";
            String expectedURL = UrlUtil.ToNormalizedURI(fileName).ToString();
            ISvgNodeRenderer imageRendered = rootActual.GetChildren()[0];
            String url = imageRendered.GetAttribute(SvgConstants.Attributes.XLINK_HREF);
            NUnit.Framework.Assert.AreEqual(expectedURL, url);
        }

        [NUnit.Framework.Test]
        public virtual void XLinkAttributeResolveNonEmptyBaseUrlTest() {
            INode root = CreateSvgContainingImage();
            String baseUrl = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext.CurrentContext.
                TestDirectory) + "/resources/itext/svg/processors/impl/DefaultSvgProcessorIntegrationTest";
            ISvgConverterProperties props = new SvgConverterProperties().SetBaseUri(baseUrl);
            SvgTagSvgNodeRenderer rootActual = (SvgTagSvgNodeRenderer)Processor().Process(root, props).GetRootRenderer
                ();
            String fileName = baseUrl + "/img.png";
            String expectedURL = UrlUtil.ToNormalizedURI(fileName).ToString();
            ISvgNodeRenderer imageRendered = rootActual.GetChildren()[0];
            String url = imageRendered.GetAttribute(SvgConstants.Attributes.XLINK_HREF);
            NUnit.Framework.Assert.AreEqual(expectedURL, url);
        }

        private INode CreateSvgContainingImage() {
            iText.StyledXmlParser.Jsoup.Nodes.Element jsoupSVGRoot = new iText.StyledXmlParser.Jsoup.Nodes.Element(iText.StyledXmlParser.Jsoup.Parser.Tag
                .ValueOf("svg"), "");
            iText.StyledXmlParser.Jsoup.Nodes.Attributes attr = new iText.StyledXmlParser.Jsoup.Nodes.Attributes();
            attr.Put(SvgConstants.Attributes.XLINK_HREF, "img.png");
            iText.StyledXmlParser.Jsoup.Nodes.Element jsoupSVGImage = new iText.StyledXmlParser.Jsoup.Nodes.Element(iText.StyledXmlParser.Jsoup.Parser.Tag
                .ValueOf("image"), "", attr);
            INode root = new JsoupElementNode(jsoupSVGRoot);
            root.AddChild(new JsoupElementNode(jsoupSVGImage));
            return root;
        }

        private static ISvgProcessor Processor() {
            return new DefaultSvgProcessor();
        }

        private class EmptySvgConverterProperties : SvgConverterProperties {
            public override ISvgNodeRendererFactory GetRendererFactory() {
                return null;
            }

            public override String GetCharset() {
                return null;
            }
        }
    }
}
