/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
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
