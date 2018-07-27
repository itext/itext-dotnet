using System;
using System.Collections.Generic;
using System.IO;
using iText.StyledXmlParser.Css;
using iText.StyledXmlParser.Node;
using iText.StyledXmlParser.Node.Impl.Jsoup.Node;
using iText.Svg;
using iText.Svg.Css.Impl;
using iText.Svg.Dummy.Sdk;
using iText.Test;

namespace iText.Svg.Css {
    public class DefaultStylesTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void CheckDefaultStrokeValuesTest() {
            ICssResolver styleResolver = new SvgStyleResolver();
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
            ICssResolver styleResolver = new SvgStyleResolver();
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
            ICssResolver styleResolver = new SvgStyleResolver();
            iText.StyledXmlParser.Jsoup.Nodes.Element svg = new iText.StyledXmlParser.Jsoup.Nodes.Element(iText.StyledXmlParser.Jsoup.Parser.Tag
                .ValueOf("svg"), "");
            INode svgNode = new JsoupElementNode(svg);
            IDictionary<String, String> resolvedStyles = styleResolver.ResolveStyles(svgNode, null);
            NUnit.Framework.Assert.AreEqual("helvetica", resolvedStyles.Get(SvgConstants.Attributes.FONT_FAMILY));
            NUnit.Framework.Assert.AreEqual("12px", resolvedStyles.Get(SvgConstants.Attributes.FONT_SIZE));
        }

        /// <exception cref="System.IO.IOException"/>
        [NUnit.Framework.Test]
        public virtual void EmptyStreamTest() {
            ICssResolver styleResolver = new SvgStyleResolver(new MemoryStream(new byte[] {  }));
            iText.StyledXmlParser.Jsoup.Nodes.Element svg = new iText.StyledXmlParser.Jsoup.Nodes.Element(iText.StyledXmlParser.Jsoup.Parser.Tag
                .ValueOf("svg"), "");
            INode svgNode = new JsoupElementNode(svg);
            IDictionary<String, String> resolvedStyles = styleResolver.ResolveStyles(svgNode, null);
            NUnit.Framework.Assert.AreEqual(0, resolvedStyles.Count);
        }

        /// <exception cref="System.IO.IOException"/>
        [NUnit.Framework.Test]
        public virtual void EmptyStylesFallbackTest() {
            NUnit.Framework.Assert.That(() =>  {
                new SvgStyleResolver(new ExceptionInputStream());
            }
            , NUnit.Framework.Throws.TypeOf<System.IO.IOException>());
;
        }

        [NUnit.Framework.Test]
        public virtual void OverrideDefaultStyleTest() {
            ICssResolver styleResolver = new SvgStyleResolver();
            iText.StyledXmlParser.Jsoup.Nodes.Element svg = new iText.StyledXmlParser.Jsoup.Nodes.Element(iText.StyledXmlParser.Jsoup.Parser.Tag
                .ValueOf("svg"), "");
            svg.Attributes().Put(SvgConstants.Attributes.STROKE, "white");
            INode svgNode = new JsoupElementNode(svg);
            IDictionary<String, String> resolvedStyles = styleResolver.ResolveStyles(svgNode, null);
            NUnit.Framework.Assert.AreEqual("white", resolvedStyles.Get(SvgConstants.Attributes.STROKE));
        }

        [NUnit.Framework.Test]
        [NUnit.Framework.Ignore("RND-1089")]
        public virtual void InheritedDefaultStyleTest() {
            ICssResolver styleResolver = new SvgStyleResolver();
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
