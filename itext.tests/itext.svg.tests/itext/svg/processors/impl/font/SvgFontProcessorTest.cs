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
using iText.Layout.Font;
using iText.StyledXmlParser.Css;
using iText.StyledXmlParser.Jsoup.Nodes;
using iText.StyledXmlParser.Node.Impl.Jsoup.Node;
using iText.Svg.Css.Impl;
using iText.Svg.Processors.Impl;
using iText.Test;

namespace iText.Svg.Processors.Impl.Font {
    [NUnit.Framework.Category("UnitTest")]
    public class SvgFontProcessorTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void AddFontFaceFontsTest() {
            iText.StyledXmlParser.Jsoup.Nodes.Element styleTag = new iText.StyledXmlParser.Jsoup.Nodes.Element(iText.StyledXmlParser.Jsoup.Parser.Tag
                .ValueOf("style"), "");
            TextNode styleContents = new TextNode("\n" + "\t@font-face{\n" + "\t\tfont-family:Courier;\n" + "\t\tsrc:local(Courier);\n"
                 + "\t}\n" + "  ");
            JsoupElementNode jSoupStyle = new JsoupElementNode(styleTag);
            jSoupStyle.AddChild(new JsoupTextNode(styleContents));
            SvgProcessorContext context = new SvgProcessorContext(new SvgConverterProperties());
            ICssResolver cssResolver = new SvgStyleResolver(jSoupStyle, context);
            SvgFontProcessor svgFontProcessor = new SvgFontProcessor(context);
            svgFontProcessor.AddFontFaceFonts(cssResolver);
            FontInfo info = (FontInfo)context.GetTempFonts().GetFonts().ToArray()[0];
            NUnit.Framework.Assert.AreEqual("Courier", info.GetFontName());
        }
    }
}
