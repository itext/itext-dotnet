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
using System.IO;
using iText.StyledXmlParser.Css;
using iText.StyledXmlParser.Node;
using iText.StyledXmlParser.Node.Impl.Jsoup.Node;
using iText.Svg;
using iText.Svg.Css.Impl;
using iText.Svg.Dummy.Sdk;
using iText.Svg.Processors.Impl;
using iText.Test;

namespace iText.Svg.Css {
    [NUnit.Framework.Category("UnitTest")]
    public class DefaultStylesTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void CheckDefaultStrokeValuesTest() {
            ICssResolver styleResolver = new SvgStyleResolver(new SvgProcessorContext(new SvgConverterProperties()));
            iText.StyledXmlParser.Jsoup.Nodes.Element svg = new iText.StyledXmlParser.Jsoup.Nodes.Element(iText.StyledXmlParser.Jsoup.Parser.Tag
                .ValueOf("svg"), "");
            INode svgNode = new JsoupElementNode(svg);
            IDictionary<String, String> resolvedStyles = styleResolver.ResolveStyles(svgNode, new SvgCssContext());
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
            ICssResolver styleResolver = new SvgStyleResolver(new SvgProcessorContext(new SvgConverterProperties()));
            iText.StyledXmlParser.Jsoup.Nodes.Element svg = new iText.StyledXmlParser.Jsoup.Nodes.Element(iText.StyledXmlParser.Jsoup.Parser.Tag
                .ValueOf("svg"), "");
            INode svgNode = new JsoupElementNode(svg);
            IDictionary<String, String> resolvedStyles = styleResolver.ResolveStyles(svgNode, new SvgCssContext());
            NUnit.Framework.Assert.AreEqual("black", resolvedStyles.Get(SvgConstants.Attributes.FILL));
            NUnit.Framework.Assert.AreEqual(SvgConstants.Values.FILL_RULE_NONZERO, resolvedStyles.Get(SvgConstants.Attributes
                .FILL_RULE));
            NUnit.Framework.Assert.AreEqual("1", resolvedStyles.Get(SvgConstants.Attributes.FILL_OPACITY));
        }

        [NUnit.Framework.Test]
        public virtual void CheckDefaultFontValuesTest() {
            ICssResolver styleResolver = new SvgStyleResolver(new SvgProcessorContext(new SvgConverterProperties()));
            iText.StyledXmlParser.Jsoup.Nodes.Element svg = new iText.StyledXmlParser.Jsoup.Nodes.Element(iText.StyledXmlParser.Jsoup.Parser.Tag
                .ValueOf("svg"), "");
            INode svgNode = new JsoupElementNode(svg);
            IDictionary<String, String> resolvedStyles = styleResolver.ResolveStyles(svgNode, new SvgCssContext());
            NUnit.Framework.Assert.AreEqual("helvetica", resolvedStyles.Get(SvgConstants.Attributes.FONT_FAMILY));
            NUnit.Framework.Assert.AreEqual("9pt", resolvedStyles.Get(SvgConstants.Attributes.FONT_SIZE));
        }

        [NUnit.Framework.Test]
        public virtual void EmptyStreamTest() {
            ICssResolver styleResolver = new SvgStyleResolver(new MemoryStream(new byte[] {  }), new SvgProcessorContext
                (new SvgConverterProperties()));
            iText.StyledXmlParser.Jsoup.Nodes.Element svg = new iText.StyledXmlParser.Jsoup.Nodes.Element(iText.StyledXmlParser.Jsoup.Parser.Tag
                .ValueOf("svg"), "");
            INode svgNode = new JsoupElementNode(svg);
            IDictionary<String, String> resolvedStyles = styleResolver.ResolveStyles(svgNode, new SvgCssContext());
            NUnit.Framework.Assert.AreEqual(1, resolvedStyles.Count);
            NUnit.Framework.Assert.AreEqual("12pt", resolvedStyles.Get(SvgConstants.Attributes.FONT_SIZE));
        }

        [NUnit.Framework.Test]
        public virtual void EmptyStylesFallbackTest() {
            NUnit.Framework.Assert.Catch(typeof(System.IO.IOException), () => new SvgStyleResolver(new ExceptionInputStream
                (), new SvgProcessorContext(new SvgConverterProperties())));
        }
    }
}
