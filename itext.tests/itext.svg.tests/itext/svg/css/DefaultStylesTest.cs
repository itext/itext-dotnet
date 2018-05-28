using System;
using System.Collections.Generic;
using System.IO;
using iText.StyledXmlParser.Css;
using iText.StyledXmlParser.Node;
using iText.StyledXmlParser.Node.Impl.Jsoup.Node;
using iText.Svg;
using iText.Svg.Css.Impl;
using iText.Svg.Dummy.Sdk;
using iText.Svg.Exceptions;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Svg.Css {
    public class DefaultStylesTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void CheckDefaultStrokeValuesTest() {
            ICssResolver styleResolver = new DefaultSvgStyleResolver();
            iText.StyledXmlParser.Jsoup.Nodes.Element svg = new iText.StyledXmlParser.Jsoup.Nodes.Element(iText.StyledXmlParser.Jsoup.Parser.Tag
                .ValueOf("svg"), "");
            INode svgNode = new JsoupElementNode(svg);
            IDictionary<String, String> resolvedStyles = styleResolver.ResolveStyles(svgNode, null);
            NUnit.Framework.Assert.AreEqual("1", resolvedStyles.Get(SvgConstants.Attributes.STROKE_OPACITY));
            NUnit.Framework.Assert.AreEqual("1px", resolvedStyles.Get(SvgConstants.Attributes.STROKE_WIDTH));
            NUnit.Framework.Assert.AreEqual(SvgConstants.Values.NONE, resolvedStyles.Get(SvgConstants.Attributes.STROKE
                ));
            NUnit.Framework.Assert.AreEqual(SvgConstants.Values.BUTT, resolvedStyles.Get(SvgConstants.Attributes.STROKE_LINECAP
                ));
            NUnit.Framework.Assert.AreEqual("0", resolvedStyles.Get(SvgConstants.Attributes.STROKE_DASHOFFSET));
            NUnit.Framework.Assert.AreEqual(SvgConstants.Values.NONE, resolvedStyles.Get(SvgConstants.Attributes.STROKE_DASHARRAY
                ));
            NUnit.Framework.Assert.AreEqual("4", resolvedStyles.Get(SvgConstants.Attributes.STROKE_MITERLIMIT));
        }

        [NUnit.Framework.Test]
        public virtual void CheckDefaultFillValuesTest() {
            ICssResolver styleResolver = new DefaultSvgStyleResolver();
            iText.StyledXmlParser.Jsoup.Nodes.Element svg = new iText.StyledXmlParser.Jsoup.Nodes.Element(iText.StyledXmlParser.Jsoup.Parser.Tag
                .ValueOf("svg"), "");
            INode svgNode = new JsoupElementNode(svg);
            IDictionary<String, String> resolvedStyles = styleResolver.ResolveStyles(svgNode, null);
            NUnit.Framework.Assert.AreEqual("black", resolvedStyles.Get(SvgConstants.Attributes.FILL));
            NUnit.Framework.Assert.AreEqual(SvgConstants.Values.FILL_RULE_NONZERO, resolvedStyles.Get(SvgConstants.Attributes
                .FILL_RULE));
            NUnit.Framework.Assert.AreEqual("1", resolvedStyles.Get(SvgConstants.Attributes.FILL_OPACITY));
        }

        [NUnit.Framework.Test]
        public virtual void CheckDefaultFontValuesTest() {
            ICssResolver styleResolver = new DefaultSvgStyleResolver();
            iText.StyledXmlParser.Jsoup.Nodes.Element svg = new iText.StyledXmlParser.Jsoup.Nodes.Element(iText.StyledXmlParser.Jsoup.Parser.Tag
                .ValueOf("svg"), "");
            INode svgNode = new JsoupElementNode(svg);
            IDictionary<String, String> resolvedStyles = styleResolver.ResolveStyles(svgNode, null);
            NUnit.Framework.Assert.AreEqual("helvetica", resolvedStyles.Get(SvgConstants.Attributes.FONT_FAMILY));
            NUnit.Framework.Assert.AreEqual("12px", resolvedStyles.Get(SvgConstants.Attributes.FONT_SIZE));
        }

        [NUnit.Framework.Test]
        public virtual void EmptyStreamTest() {
            ICssResolver styleResolver = new DefaultSvgStyleResolver(new MemoryStream(new byte[] {  }));
            iText.StyledXmlParser.Jsoup.Nodes.Element svg = new iText.StyledXmlParser.Jsoup.Nodes.Element(iText.StyledXmlParser.Jsoup.Parser.Tag
                .ValueOf("svg"), "");
            INode svgNode = new JsoupElementNode(svg);
            IDictionary<String, String> resolvedStyles = styleResolver.ResolveStyles(svgNode, null);
            NUnit.Framework.Assert.AreEqual(0, resolvedStyles.Count);
        }

        [NUnit.Framework.Test]
        [LogMessage(SvgLogMessageConstant.ERROR_INITIALIZING_DEFAULT_CSS, Count = 1)]
        public virtual void EmptyStylesFallbackTest() {
            ICssResolver styleResolver = new DefaultSvgStyleResolver(new ExceptionInputStream());
            iText.StyledXmlParser.Jsoup.Nodes.Element svg = new iText.StyledXmlParser.Jsoup.Nodes.Element(iText.StyledXmlParser.Jsoup.Parser.Tag
                .ValueOf("svg"), "");
            INode svgNode = new JsoupElementNode(svg);
            IDictionary<String, String> resolvedStyles = styleResolver.ResolveStyles(svgNode, null);
            NUnit.Framework.Assert.AreEqual(0, resolvedStyles.Count);
        }

        [NUnit.Framework.Test]
        public virtual void OverrideDefaultStyleTest() {
            ICssResolver styleResolver = new DefaultSvgStyleResolver();
            iText.StyledXmlParser.Jsoup.Nodes.Element svg = new iText.StyledXmlParser.Jsoup.Nodes.Element(iText.StyledXmlParser.Jsoup.Parser.Tag
                .ValueOf("svg"), "");
            svg.Attributes().Put(SvgConstants.Attributes.STROKE, "white");
            INode svgNode = new JsoupElementNode(svg);
            IDictionary<String, String> resolvedStyles = styleResolver.ResolveStyles(svgNode, null);
            NUnit.Framework.Assert.AreEqual("white", resolvedStyles.Get(SvgConstants.Attributes.STROKE));
        }

        [NUnit.Framework.Test]
        [NUnit.Framework.Ignore("RND-880")]
        public virtual void InheritedDefaultStyleTest() {
            // TODO RND-880
            ICssResolver styleResolver = new DefaultSvgStyleResolver();
            iText.StyledXmlParser.Jsoup.Nodes.Element svg = new iText.StyledXmlParser.Jsoup.Nodes.Element(iText.StyledXmlParser.Jsoup.Parser.Tag
                .ValueOf("svg"), "");
            iText.StyledXmlParser.Jsoup.Nodes.Element circle = new iText.StyledXmlParser.Jsoup.Nodes.Element(iText.StyledXmlParser.Jsoup.Parser.Tag
                .ValueOf("circle"), "");
            INode svgNode = new JsoupElementNode(svg);
            svgNode.AddChild(new JsoupElementNode(circle));
            IDictionary<String, String> resolvedStyles = styleResolver.ResolveStyles(svgNode.ChildNodes()[0], null);
            NUnit.Framework.Assert.AreEqual("black", resolvedStyles.Get(SvgConstants.Attributes.STROKE));
        }
    }
}
