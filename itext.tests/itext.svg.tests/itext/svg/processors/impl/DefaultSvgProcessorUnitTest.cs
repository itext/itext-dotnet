/*
This file is part of the iText (R) project.
Copyright (c) 1998-2025 Apryse Group NV
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
using iText.IO.Util;
using iText.StyledXmlParser.Node;
using iText.StyledXmlParser.Node.Impl.Jsoup.Node;
using iText.Svg;
using iText.Svg.Dummy.Processors.Impl;
using iText.Svg.Dummy.Renderers.Impl;
using iText.Svg.Exceptions;
using iText.Svg.Logs;
using iText.Svg.Processors;
using iText.Svg.Renderers;
using iText.Svg.Renderers.Impl;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Svg.Processors.Impl {
    [NUnit.Framework.Category("UnitTest")]
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
        [LogMessage(iText.StyledXmlParser.Logs.StyledXmlParserLogMessageConstant.ERROR_ADDING_CHILD_NODE)]
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
            iText.StyledXmlParser.Jsoup.Nodes.Element jsoupSVGRoot = new iText.StyledXmlParser.Jsoup.Nodes.Element(iText.StyledXmlParser.Jsoup.Parser.Tag
                .ValueOf("polygon"), "");
            iText.StyledXmlParser.Jsoup.Nodes.Element jsoupSVGCircle = new iText.StyledXmlParser.Jsoup.Nodes.Element(iText.StyledXmlParser.Jsoup.Parser.Tag
                .ValueOf("circle"), "");
            INode root = new JsoupElementNode(jsoupSVGRoot);
            root.AddChild(new JsoupElementNode(jsoupSVGCircle));
            //Run
            DefaultSvgProcessor processor = new DefaultSvgProcessor();
            ISvgConverterProperties props = new DummySvgConverterProperties();
            Exception e = NUnit.Framework.Assert.Catch(typeof(SvgProcessingException), () => processor.Process(root, props
                ).GetRootRenderer());
            NUnit.Framework.Assert.AreEqual(SvgExceptionMessageConstant.NO_ROOT, e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void DummyProcessingTestNullInput() {
            DefaultSvgProcessor processor = new DefaultSvgProcessor();
            NUnit.Framework.Assert.Catch(typeof(SvgProcessingException), () => processor.Process(null, null));
        }

        [NUnit.Framework.Test]
        public virtual void ProcessWithNullPropertiesTest() {
            iText.StyledXmlParser.Jsoup.Nodes.Element jsoupSVGRoot = new iText.StyledXmlParser.Jsoup.Nodes.Element(iText.StyledXmlParser.Jsoup.Parser.Tag
                .ValueOf("svg"), "");
            INode root = new JsoupElementNode(jsoupSVGRoot);
            DefaultSvgProcessor processor = new DefaultSvgProcessor();
            SvgConverterProperties convProps = new SvgConverterProperties();
            convProps.SetRendererFactory(null);
            convProps.SetCharset(null);
            ISvgNodeRenderer rootRenderer = processor.Process(root, convProps).GetRootRenderer();
            NUnit.Framework.Assert.IsTrue(rootRenderer is SvgTagSvgNodeRenderer);
            NUnit.Framework.Assert.AreEqual(0, ((SvgTagSvgNodeRenderer)rootRenderer).GetChildren().Count);
        }

        [NUnit.Framework.Test]
        public virtual void DefaultProcessingCorrectlyNestedRenderersTest() {
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
            SvgConverterProperties convProps = new SvgConverterProperties();
            ISvgNodeRenderer rootRenderer = processor.Process(root, convProps).GetRootRenderer();
            NUnit.Framework.Assert.IsTrue(rootRenderer is SvgTagSvgNodeRenderer);
            IList<ISvgNodeRenderer> children = ((SvgTagSvgNodeRenderer)rootRenderer).GetChildren();
            NUnit.Framework.Assert.AreEqual(2, children.Count);
            NUnit.Framework.Assert.IsTrue(children[0] is CircleSvgNodeRenderer);
            NUnit.Framework.Assert.IsTrue(children[1] is PathSvgNodeRenderer);
        }

        [NUnit.Framework.Test]
        public virtual void FindFirstElementNullTest() {
            DefaultSvgProcessor processor = new DefaultSvgProcessor();
            IElementNode actual = processor.FindFirstElement(null, "name");
            NUnit.Framework.Assert.IsNull(actual);
        }

        [NUnit.Framework.Test]
        [LogMessage(SvgLogMessageConstant.UNMAPPED_TAG)]
        public virtual void DepthFirstNullRendererTest() {
            iText.StyledXmlParser.Jsoup.Nodes.Element jsoupNonExistingElement = new iText.StyledXmlParser.Jsoup.Nodes.Element
                (iText.StyledXmlParser.Jsoup.Parser.Tag.ValueOf("nonExisting"), "");
            INode root = new JsoupElementNode(jsoupNonExistingElement);
            DefaultSvgProcessor dsp = new DefaultSvgProcessor();
            ISvgConverterProperties scp = new SvgConverterProperties();
            dsp.PerformSetup(root, scp);
            // below method must not throw a NullPointerException
            NUnit.Framework.Assert.DoesNotThrow(() => dsp.ExecuteDepthFirstTraversal(root));
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
            String expectedURLAnotherValidVersion = CreateAnotherValidUrlVersion(expectedURL);
            ISvgNodeRenderer imageRendered = rootActual.GetChildren()[0];
            String url = imageRendered.GetAttribute(SvgConstants.Attributes.XLINK_HREF);
            // Both variants(namely with triple and single slashes) are valid.
            NUnit.Framework.Assert.IsTrue(expectedURL.Equals(url) || expectedURLAnotherValidVersion.Equals(url));
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
            String expectedURLAnotherValidVersion = CreateAnotherValidUrlVersion(expectedURL);
            ISvgNodeRenderer imageRendered = rootActual.GetChildren()[0];
            String url = imageRendered.GetAttribute(SvgConstants.Attributes.XLINK_HREF);
            // Both variants(namely with triple and single slashes) are valid.
            NUnit.Framework.Assert.IsTrue(expectedURL.Equals(url) || expectedURLAnotherValidVersion.Equals(url));
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

        private static String CreateAnotherValidUrlVersion(String url) {
            if (url.StartsWith("file:///")) {
                return "file:/" + url.Substring("file:///".Length);
            }
            else {
                if (url.StartsWith("file:/")) {
                    return "file:///" + url.Substring("file:/".Length);
                }
                else {
                    return url;
                }
            }
        }

        private static ISvgProcessor Processor() {
            return new DefaultSvgProcessor();
        }
    }
}
