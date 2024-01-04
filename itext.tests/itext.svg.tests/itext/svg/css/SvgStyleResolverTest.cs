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
using System;
using System.Collections.Generic;
using iText.IO.Util;
using iText.StyledXmlParser.Css;
using iText.StyledXmlParser.Css.Resolve;
using iText.StyledXmlParser.Jsoup.Nodes;
using iText.StyledXmlParser.Node;
using iText.StyledXmlParser.Node.Impl.Jsoup.Node;
using iText.Svg;
using iText.Svg.Css.Impl;
using iText.Svg.Processors.Impl;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Svg.Css {
    [NUnit.Framework.Category("UnitTest")]
    public class SvgStyleResolverTest : ExtendedITextTest {
        private static readonly String baseUri = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/svg/css/SvgStyleResolver/";

        //Single element test
        //Inherits values from parent?
        //Calculates values from parent
        [NUnit.Framework.Test]
        public virtual void SvgCssResolverBasicAttributeTest() {
            iText.StyledXmlParser.Jsoup.Nodes.Element jsoupCircle = new iText.StyledXmlParser.Jsoup.Nodes.Element(iText.StyledXmlParser.Jsoup.Parser.Tag
                .ValueOf("circle"), "");
            Attributes circleAttributes = jsoupCircle.Attributes();
            circleAttributes.Put(new iText.StyledXmlParser.Jsoup.Nodes.Attribute("id", "circle1"));
            circleAttributes.Put(new iText.StyledXmlParser.Jsoup.Nodes.Attribute("cx", "95"));
            circleAttributes.Put(new iText.StyledXmlParser.Jsoup.Nodes.Attribute("cy", "95"));
            circleAttributes.Put(new iText.StyledXmlParser.Jsoup.Nodes.Attribute("rx", "53"));
            circleAttributes.Put(new iText.StyledXmlParser.Jsoup.Nodes.Attribute("ry", "53"));
            circleAttributes.Put(new iText.StyledXmlParser.Jsoup.Nodes.Attribute("style", "stroke-width:1.5;stroke:#da0000;"
                ));
            AbstractCssContext cssContext = new SvgCssContext();
            INode circle = new JsoupElementNode(jsoupCircle);
            SvgProcessorContext context = new SvgProcessorContext(new SvgConverterProperties());
            ICssResolver resolver = new SvgStyleResolver(circle, context);
            IDictionary<String, String> actual = resolver.ResolveStyles(circle, cssContext);
            IDictionary<String, String> expected = new Dictionary<String, String>();
            expected.Put("id", "circle1");
            expected.Put("cx", "95");
            expected.Put("cy", "95");
            expected.Put("rx", "53");
            expected.Put("ry", "53");
            expected.Put("stroke-width", "1.5");
            expected.Put("stroke", "#da0000");
            expected.Put("font-size", "12pt");
            NUnit.Framework.Assert.AreEqual(expected, actual);
        }

        [NUnit.Framework.Test]
        public virtual void SvgCssResolverStylesheetTest() {
            iText.StyledXmlParser.Jsoup.Nodes.Element jsoupLink = new iText.StyledXmlParser.Jsoup.Nodes.Element(iText.StyledXmlParser.Jsoup.Parser.Tag
                .ValueOf(SvgConstants.Tags.LINK), "");
            Attributes linkAttributes = jsoupLink.Attributes();
            linkAttributes.Put(new iText.StyledXmlParser.Jsoup.Nodes.Attribute(SvgConstants.Attributes.XMLNS, "http://www.w3.org/1999/xhtml"
                ));
            linkAttributes.Put(new iText.StyledXmlParser.Jsoup.Nodes.Attribute(SvgConstants.Attributes.REL, SvgConstants.Attributes
                .STYLESHEET));
            linkAttributes.Put(new iText.StyledXmlParser.Jsoup.Nodes.Attribute(SvgConstants.Attributes.HREF, "styleSheetWithLinkStyle.css"
                ));
            linkAttributes.Put(new iText.StyledXmlParser.Jsoup.Nodes.Attribute("type", "text/css"));
            JsoupElementNode node = new JsoupElementNode(jsoupLink);
            SvgConverterProperties scp = new SvgConverterProperties();
            scp.SetBaseUri(baseUri);
            SvgProcessorContext processorContext = new SvgProcessorContext(scp);
            SvgStyleResolver sr = new SvgStyleResolver(node, processorContext);
            IDictionary<String, String> attr = sr.ResolveStyles(node, new SvgCssContext());
            IDictionary<String, String> expectedAttr = new Dictionary<String, String>();
            expectedAttr.Put(SvgConstants.Attributes.XMLNS, "http://www.w3.org/1999/xhtml");
            expectedAttr.Put(SvgConstants.Attributes.REL, SvgConstants.Attributes.STYLESHEET);
            expectedAttr.Put(SvgConstants.Attributes.HREF, "styleSheetWithLinkStyle.css");
            expectedAttr.Put(SvgConstants.Attributes.FONT_SIZE, "12pt");
            expectedAttr.Put("type", "text/css");
            // Attribute from external stylesheet
            expectedAttr.Put(SvgConstants.Attributes.FILL, "black");
            NUnit.Framework.Assert.AreEqual(expectedAttr, attr);
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.StyledXmlParser.Logs.StyledXmlParserLogMessageConstant.UNABLE_TO_RETRIEVE_STREAM_WITH_GIVEN_BASE_URI
            , LogLevel = LogLevelConstants.ERROR)]
        public virtual void SvgCssResolverInvalidNameStylesheetTest() {
            iText.StyledXmlParser.Jsoup.Nodes.Element jsoupLink = new iText.StyledXmlParser.Jsoup.Nodes.Element(iText.StyledXmlParser.Jsoup.Parser.Tag
                .ValueOf(SvgConstants.Tags.LINK), "");
            iText.StyledXmlParser.Jsoup.Nodes.Attributes linkAttributes = jsoupLink.Attributes();
            linkAttributes.Put(new iText.StyledXmlParser.Jsoup.Nodes.Attribute(SvgConstants.Attributes.XMLNS, "http://www.w3.org/1999/xhtml"
                ));
            linkAttributes.Put(new iText.StyledXmlParser.Jsoup.Nodes.Attribute(SvgConstants.Attributes.REL, SvgConstants.Attributes
                .STYLESHEET));
            linkAttributes.Put(new iText.StyledXmlParser.Jsoup.Nodes.Attribute(SvgConstants.Attributes.HREF, "!invalid name!externalSheet.css"
                ));
            linkAttributes.Put(new iText.StyledXmlParser.Jsoup.Nodes.Attribute("type", "text/css"));
            JsoupElementNode node = new JsoupElementNode(jsoupLink);
            SvgConverterProperties scp = new SvgConverterProperties();
            scp.SetBaseUri(baseUri);
            SvgProcessorContext processorContext = new SvgProcessorContext(scp);
            SvgStyleResolver sr = new SvgStyleResolver(node, processorContext);
            IDictionary<String, String> attr = sr.ResolveStyles(node, new SvgCssContext());
            IDictionary<String, String> expectedAttr = new Dictionary<String, String>();
            expectedAttr.Put(SvgConstants.Attributes.XMLNS, "http://www.w3.org/1999/xhtml");
            expectedAttr.Put(SvgConstants.Attributes.REL, SvgConstants.Attributes.STYLESHEET);
            expectedAttr.Put(SvgConstants.Attributes.HREF, "!invalid name!externalSheet.css");
            expectedAttr.Put(SvgConstants.Attributes.FONT_SIZE, "12pt");
            expectedAttr.Put("type", "text/css");
            NUnit.Framework.Assert.AreEqual(expectedAttr, attr);
        }

        [NUnit.Framework.Test]
        public virtual void SvgCssResolverXlinkTest() {
            iText.StyledXmlParser.Jsoup.Nodes.Element jsoupImage = new iText.StyledXmlParser.Jsoup.Nodes.Element(iText.StyledXmlParser.Jsoup.Parser.Tag
                .ValueOf("image"), "");
            iText.StyledXmlParser.Jsoup.Nodes.Attributes imageAttributes = jsoupImage.Attributes();
            imageAttributes.Put(new iText.StyledXmlParser.Jsoup.Nodes.Attribute("xlink:href", "itis.jpg"));
            JsoupElementNode node = new JsoupElementNode(jsoupImage);
            SvgConverterProperties scp = new SvgConverterProperties();
            scp.SetBaseUri(baseUri);
            SvgProcessorContext processorContext = new SvgProcessorContext(scp);
            SvgStyleResolver sr = new SvgStyleResolver(node, processorContext);
            IDictionary<String, String> attr = sr.ResolveStyles(node, new SvgCssContext());
            String fileName = baseUri + "itis.jpg";
            String expectedUrl = UrlUtil.ToNormalizedURI(fileName).ToString();
            String expectedUrlAnotherValidVersion;
            if (expectedUrl.StartsWith("file:///")) {
                expectedUrlAnotherValidVersion = "file:/" + expectedUrl.Substring("file:///".Length);
            }
            else {
                if (expectedUrl.StartsWith("file:/")) {
                    expectedUrlAnotherValidVersion = "file:///" + expectedUrl.Substring("file:/".Length);
                }
                else {
                    expectedUrlAnotherValidVersion = expectedUrl;
                }
            }
            String url = attr.Get("xlink:href");
            // Both variants(namely with triple and single slashes) are valid.
            NUnit.Framework.Assert.IsTrue(expectedUrl.Equals(url) || expectedUrlAnotherValidVersion.Equals(url));
        }

        [NUnit.Framework.Test]
        public virtual void SvgCssResolveHashXlinkTest() {
            iText.StyledXmlParser.Jsoup.Nodes.Element jsoupImage = new iText.StyledXmlParser.Jsoup.Nodes.Element(iText.StyledXmlParser.Jsoup.Parser.Tag
                .ValueOf("image"), "");
            iText.StyledXmlParser.Jsoup.Nodes.Attributes imageAttributes = jsoupImage.Attributes();
            imageAttributes.Put(new iText.StyledXmlParser.Jsoup.Nodes.Attribute("xlink:href", "#testid"));
            JsoupElementNode node = new JsoupElementNode(jsoupImage);
            SvgConverterProperties scp = new SvgConverterProperties();
            scp.SetBaseUri(baseUri);
            SvgProcessorContext processorContext = new SvgProcessorContext(scp);
            SvgStyleResolver sr = new SvgStyleResolver(node, processorContext);
            IDictionary<String, String> attr = sr.ResolveStyles(node, new SvgCssContext());
            NUnit.Framework.Assert.AreEqual("#testid", attr.Get("xlink:href"));
        }

        [NUnit.Framework.Test]
        public virtual void OverrideDefaultStyleTest() {
            ICssResolver styleResolver = new SvgStyleResolver(new SvgProcessorContext(new SvgConverterProperties()));
            iText.StyledXmlParser.Jsoup.Nodes.Element svg = new iText.StyledXmlParser.Jsoup.Nodes.Element(iText.StyledXmlParser.Jsoup.Parser.Tag
                .ValueOf("svg"), "");
            svg.Attributes().Put(SvgConstants.Attributes.STROKE, "white");
            INode svgNode = new JsoupElementNode(svg);
            IDictionary<String, String> resolvedStyles = styleResolver.ResolveStyles(svgNode, new SvgCssContext());
            NUnit.Framework.Assert.AreEqual("white", resolvedStyles.Get(SvgConstants.Attributes.STROKE));
        }

        [NUnit.Framework.Test]
        public virtual void SvgCssResolverStyleTagTest() {
            iText.StyledXmlParser.Jsoup.Nodes.Element styleTag = new iText.StyledXmlParser.Jsoup.Nodes.Element(iText.StyledXmlParser.Jsoup.Parser.Tag
                .ValueOf("style"), "");
            TextNode styleContents = new TextNode("\n" + "\tellipse{\n" + "\t\tstroke-width:1.76388889;\n" + "\t\tstroke:#da0000;\n"
                 + "\t\tstroke-opacity:1;\n" + "\t}\n" + "  ");
            JsoupElementNode jSoupStyle = new JsoupElementNode(styleTag);
            jSoupStyle.AddChild(new JsoupTextNode(styleContents));
            iText.StyledXmlParser.Jsoup.Nodes.Element ellipse = new iText.StyledXmlParser.Jsoup.Nodes.Element(iText.StyledXmlParser.Jsoup.Parser.Tag
                .ValueOf("ellipse"), "");
            JsoupElementNode jSoupEllipse = new JsoupElementNode(ellipse);
            SvgProcessorContext context = new SvgProcessorContext(new SvgConverterProperties());
            SvgStyleResolver resolver = new SvgStyleResolver(jSoupStyle, context);
            AbstractCssContext svgContext = new SvgCssContext();
            IDictionary<String, String> actual = resolver.ResolveStyles(jSoupEllipse, svgContext);
            IDictionary<String, String> expected = new Dictionary<String, String>();
            expected.Put("stroke-width", "1.76388889");
            expected.Put("stroke", "#da0000");
            expected.Put("stroke-opacity", "1");
            expected.Put("font-size", "12pt");
            NUnit.Framework.Assert.AreEqual(expected, actual);
        }

        [NUnit.Framework.Test]
        public virtual void FontsResolverTagTest() {
            iText.StyledXmlParser.Jsoup.Nodes.Element styleTag = new iText.StyledXmlParser.Jsoup.Nodes.Element(iText.StyledXmlParser.Jsoup.Parser.Tag
                .ValueOf("style"), "");
            TextNode styleContents = new TextNode("\n" + "\t@font-face{\n" + "\t\tfont-family:Courier;\n" + "\t\tsrc:url(#Super Sans);\n"
                 + "\t}\n" + "  ");
            JsoupElementNode jSoupStyle = new JsoupElementNode(styleTag);
            jSoupStyle.AddChild(new JsoupTextNode(styleContents));
            SvgProcessorContext context = new SvgProcessorContext(new SvgConverterProperties());
            SvgStyleResolver resolver = new SvgStyleResolver(jSoupStyle, context);
            IList<CssFontFaceRule> fontFaceRuleList = resolver.GetFonts();
            NUnit.Framework.Assert.AreEqual(1, fontFaceRuleList.Count);
            NUnit.Framework.Assert.AreEqual(2, fontFaceRuleList[0].GetProperties().Count);
        }
    }
}
