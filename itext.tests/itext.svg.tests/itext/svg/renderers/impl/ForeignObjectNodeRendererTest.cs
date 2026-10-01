/*
This file is part of the iText (R) project.
Copyright (c) 1998-2026 Apryse Group NV
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
using System.IO;
using iText.Commons.Internal.Runtime;
using iText.Kernel.Colors;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Svg.Converter;
using iText.Svg.Processors;
using iText.Svg.Renderers;
using iText.Test;

namespace iText.Svg.Renderers.Impl {
    [NUnit.Framework.Category("UnitTest")]
    public class ForeignObjectNodeRendererTest : ExtendedITextTest {
        private const String SAMPLE = "<foreignObject xmlns='http://www.w3.org/2000/svg' width='230' height='35'" 
            + " style='font-size:14px;color:rgb(67,39,135);font-family:Arial;font-weight:700;" + "text-align:left;letter-spacing:0em;line-height:1.5' x='64' y='24'>\n"
             + "<div xmlns='http://www.w3.org/1999/xhtml'>#1 Repository Of The Day</div>\n</foreignObject>";

        [NUnit.Framework.Test]
        public virtual void SampleBoundsAndStylesTest() {
            ForeignObjectNodeRenderer renderer = ProcessForeignObject(SAMPLE);
            SvgDrawContext context = new SvgDrawContext(null, null);
            NUnit.Framework.Assert.IsTrue(new Rectangle(48, 18, 172.5f, 26.25f).EqualsWithEpsilon(renderer.GetObjectBoundingBox
                (context)));
            Paragraph paragraph = renderer.CreateParagraph(context);
            NUnit.Framework.Assert.AreEqual("#1 Repository Of The Day", ParagraphText(paragraph));
            NUnit.Framework.Assert.AreEqual(10.5f, ((UnitValue)paragraph.GetProperty<UnitValue>(Property.FONT_SIZE)).GetValue
                ());
            NUnit.Framework.Assert.AreEqual(new String[] { "arial" }, paragraph.GetProperty<String[]>(Property.FONT));
            NUnit.Framework.Assert.AreEqual("700", paragraph.GetProperty<String>(Property.FONT_WEIGHT));
            NUnit.Framework.Assert.AreEqual(new DeviceRgb(67, 39, 135), paragraph.GetProperty<TransparentColor>(Property
                .FONT_COLOR).GetColor());
            NUnit.Framework.Assert.AreEqual(15.75f, paragraph.GetProperty<Leading>(Property.LEADING).GetValue());
        }

        [NUnit.Framework.Test]
        public virtual void SamplePdfTest() {
            NUnit.Framework.Assert.AreEqual("#1 Repository Of The Day", ConvertAndExtract(SAMPLE));
        }

        [NUnit.Framework.Test]
        public virtual void WrappingTest() {
            String text = ConvertAndExtract("<foreignObject width='65' height='120' style='font-size:12px'>" + "<div xmlns='http://www.w3.org/1999/xhtml'>one two three four five six seven</div></foreignObject>"
                );
            NUnit.Framework.Assert.IsTrue(text.Contains("\n"));
            NUnit.Framework.Assert.AreEqual("one two three four five six seven", iText.Commons.Utils.StringUtil.ReplaceAll
                (text, "\\s+", " "));
        }

        [NUnit.Framework.Test]
        public virtual void InheritedXhtmlNamespaceAndWhitespaceTest() {
            ForeignObjectNodeRenderer renderer = ProcessForeignObject("<foreignObject width='200' height='50'>" + "<div xmlns='http://www.w3.org/1999/xhtml'>one <span>two</span> <b>three</b> &amp; four</div>"
                 + "</foreignObject>");
            NUnit.Framework.Assert.AreEqual("one two three & four", ParagraphText(renderer.CreateParagraph(new SvgDrawContext
                (null, null))));
        }

        [NUnit.Framework.Test]
        public virtual void PrefixedXhtmlNamespaceTest() {
            ForeignObjectNodeRenderer renderer = ProcessForeignObject("<foreignObject width='200' height='50'" + " xmlns:h='http://www.w3.org/1999/xhtml'>"
                 + "<h:div>prefixed <h:span>text</h:span><span>ignored</span></h:div></foreignObject>");
            NUnit.Framework.Assert.AreEqual("prefixed text", ParagraphText(renderer.CreateParagraph(new SvgDrawContext
                (null, null))));
        }

        [NUnit.Framework.Test]
        public virtual void NamespaceOverridesTest() {
            ForeignObjectNodeRenderer renderer = ProcessForeignObject("<foreignObject width='200' height='50'" + " xmlns:h='http://www.w3.org/1999/xhtml'>"
                 + "<div xmlns='http://www.w3.org/1999/xhtml'>before " + "<span xmlns=''>no namespace</span>" + "<text xmlns='http://www.w3.org/2000/svg'>SVG text</text>"
                 + "<h:span xmlns:h='urn:other'>other namespace</h:span>" + "<h:span xmlns:h=''>empty prefix binding</h:span>"
                 + "<h:span>after</h:span></div></foreignObject>");
            NUnit.Framework.Assert.AreEqual("before after", ParagraphText(renderer.CreateParagraph(new SvgDrawContext(
                null, null))));
        }

        [NUnit.Framework.Test]
        public virtual void XhtmlScriptAndStyleContentTest() {
            ForeignObjectNodeRenderer renderer = ProcessForeignObject("<foreignObject width='200' height='50'" + " xmlns:h='http://www.w3.org/1999/xhtml'>"
                 + "<div xmlns='http://www.w3.org/1999/xhtml'>visible" + "<script>hidden</script><style>/* hidden */</style>"
                 + "<h:script>hidden</h:script><h:style>hidden</h:style>" + "<h:SCRIPT><h:span>hidden</h:span></h:SCRIPT></div></foreignObject>"
                );
            NUnit.Framework.Assert.AreEqual("visible", ParagraphText(renderer.CreateParagraph(new SvgDrawContext(null, 
                null))));
        }

        [NUnit.Framework.Test]
        public virtual void EmptyAndNonPositiveBoundsTest() {
            NUnit.Framework.Assert.AreEqual("", ConvertAndExtract("<foreignObject width='100' height='50'/>"));
            foreach (String dimensions in new String[] { "", "width='100'", "height='50'", "width='0' height='50'", "width='100' height='0'"
                , "width='-1' height='50'", "width='100' height='-1'" }) {
                NUnit.Framework.Assert.AreEqual("", ConvertAndExtract("<foreignObject " + dimensions + "><div xmlns='http://www.w3.org/1999/xhtml'>Hidden</div></foreignObject>"
                    ));
            }
        }

        private static ForeignObjectNodeRenderer ProcessForeignObject(String content) {
            ISvgProcessorResult result = SvgConverter.Process(SvgConverter.Parse(Svg(content)), null);
            return (ForeignObjectNodeRenderer)((IBranchSvgNodeRenderer)result.GetRootRenderer()).GetChildren()[0];
        }

        private static String ParagraphText(Paragraph paragraph) {
            return ((Text)paragraph.GetChildren()[0]).GetText();
        }

        private static String Svg(String content) {
            return "<svg xmlns='http://www.w3.org/2000/svg' width='400' height='200'>" + content + "</svg>";
        }

        private static String ConvertAndExtract(String content) {
            MemoryStream output = new MemoryStream();
            SvgConverter.CreatePdf(new MemoryStream(Svg(content).GetBytes(System.Text.Encoding.UTF8)), output);
            using (PdfDocument document = new PdfDocument(new PdfReader(new MemoryStream(output.ToArray())))) {
                return PdfTextExtractor.GetTextFromPage(document.GetPage(1));
            }
        }
    }
}
