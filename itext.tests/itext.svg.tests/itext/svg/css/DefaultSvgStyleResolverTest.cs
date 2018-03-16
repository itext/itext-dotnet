using System;
using System.Collections.Generic;
using iText.StyledXmlParser.Css;
using iText.StyledXmlParser.Jsoup.Nodes;
using iText.StyledXmlParser.Node;
using iText.StyledXmlParser.Node.Impl.Jsoup.Node;
using iText.Svg.Css.Impl;

namespace iText.Svg.Css {
    public class DefaultSvgStyleResolverTest {
        //Single element test
        //Inherits values from parent?
        //Calculates values from parent
        [NUnit.Framework.Test]
        public virtual void DefaultSvgCssResolverBasicAttributeTest() {
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
            ICssContext cssContext = new SvgCssContext();
            INode circle = new JsoupElementNode(jsoupCircle);
            ICssResolver resolver = new DefaultSvgStyleResolver(circle);
            IDictionary<String, String> actual = resolver.ResolveStyles(circle, cssContext);
            IDictionary<String, String> expected = new Dictionary<String, String>();
            expected.Put("id", "circle1");
            expected.Put("cx", "95");
            expected.Put("cy", "95");
            expected.Put("rx", "53");
            expected.Put("ry", "53");
            expected.Put("stroke-width", "1.5");
            expected.Put("stroke", "#da0000");
            NUnit.Framework.Assert.AreEqual(expected, actual);
        }

        [NUnit.Framework.Test]
        public virtual void DefaultSvgCssResolverStyleTagTest() {
            iText.StyledXmlParser.Jsoup.Nodes.Element styleTag = new iText.StyledXmlParser.Jsoup.Nodes.Element(iText.StyledXmlParser.Jsoup.Parser.Tag
                .ValueOf("style"), "");
            TextNode styleContents = new TextNode("\n" + "\tellipse{\n" + "\t\tstroke-width:1.76388889;\n" + "\t\tstroke:#da0000;\n"
                 + "\t\tstroke-opacity:1;\n" + "\t}\n" + "  ", "");
            JsoupElementNode jSoupStyle = new JsoupElementNode(styleTag);
            jSoupStyle.AddChild(new JsoupTextNode(styleContents));
            iText.StyledXmlParser.Jsoup.Nodes.Element ellipse = new iText.StyledXmlParser.Jsoup.Nodes.Element(iText.StyledXmlParser.Jsoup.Parser.Tag
                .ValueOf("ellipse"), "");
            JsoupElementNode jSoupEllipse = new JsoupElementNode(ellipse);
            DefaultSvgStyleResolver resolver = new DefaultSvgStyleResolver(jSoupStyle);
            ICssContext svgContext = new SvgCssContext();
            IDictionary<String, String> actual = resolver.ResolveStyles(jSoupEllipse, svgContext);
            IDictionary<String, String> expected = new Dictionary<String, String>();
            expected.Put("stroke-width", "1.76388889");
            expected.Put("stroke", "#da0000");
            expected.Put("stroke-opacity", "1");
            NUnit.Framework.Assert.AreEqual(expected, actual);
        }

        [NUnit.Framework.Test]
        public virtual void DefaultSvgCssResolverInheritedTest() {
        }

        [NUnit.Framework.Test]
        public virtual void SvgCssResolverRelativeValueTest() {
        }

        [NUnit.Framework.Test]
        public virtual void DefaultSvgCssResolverDefaultsTest() {
        }
    }
}
