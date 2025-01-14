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
using iText.Kernel.Geom;
using iText.StyledXmlParser.Node;
using iText.StyledXmlParser.Node.Impl.Jsoup;
using iText.Svg.Logs;
using iText.Svg.Processors.Impl;
using iText.Svg.Renderers;
using iText.Svg.Renderers.Impl;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Svg.Css {
    [NUnit.Framework.Category("IntegrationTest")]
    public class SvgStyleResolverIntegrationTest : SvgIntegrationTest {
        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/svg/css/SvgStyleResolver/";

        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/svg/css/SvgStyleResolver/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            ITextTest.CreateDestinationFolder(destinationFolder);
        }

        [NUnit.Framework.Test]
        public virtual void RedCirleTest() {
            String svg = "<svg\n" + "   width=\"210mm\"\n" + "   height=\"297mm\"\n" + "   viewBox=\"0 0 210 297\"\n" 
                + "   version=\"1.1\"\n" + "  <title id=\"title4508\">Red Circle</title>\n" + "    <ellipse\n" + "       id=\"path3699\"\n"
                 + "       cx=\"96.005951\"\n" + "       cy=\"110.65774\"\n" + "       rx=\"53.672619\"\n" + "       ry=\"53.294643\"\n"
                 + "       style=\"stroke-width:1.76388889;stroke:#da0000;stroke-opacity:1;fill:none;stroke-miterlimit:4;stroke-dasharray:none\" />\n"
                 + "</svg>\n";
            JsoupXmlParser xmlParser = new JsoupXmlParser();
            IDocumentNode root = xmlParser.Parse(svg);
            IBranchSvgNodeRenderer nodeRenderer = (IBranchSvgNodeRenderer)new DefaultSvgProcessor().Process(root, null
                ).GetRootRenderer();
            IDictionary<String, String> actual = new Dictionary<String, String>();
            //Traverse to ellipse
            ISvgNodeRenderer ellipse = nodeRenderer.GetChildren()[0];
            actual.Put("stroke", ellipse.GetAttribute("stroke"));
            actual.Put("stroke-width", ellipse.GetAttribute("stroke-width"));
            actual.Put("stroke-opacity", ellipse.GetAttribute("stroke-opacity"));
            IDictionary<String, String> expected = new Dictionary<String, String>();
            expected.Put("stroke-width", "1.76388889");
            expected.Put("stroke", "#da0000");
            expected.Put("stroke-opacity", "1");
            NUnit.Framework.Assert.AreEqual(expected, actual);
        }

        [NUnit.Framework.Test]
        public virtual void StyleTagProcessingTest() {
            String svg = "<svg\n" + "   width=\"210mm\"\n" + "   height=\"297mm\"\n" + "   viewBox=\"0 0 210 297\"\n" 
                + "   version=\"1.1\"\n" + "   id=\"svg8\"\n" + "   >\n" + "  <style>\n" + "\tellipse{\n" + "\t\tstroke-width:1.76388889;\n"
                 + "\t\tstroke:#da0000;\n" + "\t\tstroke-opacity:1;\n" + "\t}\n" + "  </style>\n" + "    <ellipse\n" +
                 "       id=\"path3699\"\n" + "       cx=\"96.005951\"\n" + "       cy=\"110.65774\"\n" + "       rx=\"53.672619\"\n"
                 + "       ry=\"53.294643\"\n" + "       style=\"fill:none;stroke-miterlimit:4;stroke-dasharray:none\" />\n"
                 + "</svg>\n";
            JsoupXmlParser xmlParser = new JsoupXmlParser();
            IDocumentNode root = xmlParser.Parse(svg);
            IBranchSvgNodeRenderer nodeRenderer = (IBranchSvgNodeRenderer)new DefaultSvgProcessor().Process(root, null
                ).GetRootRenderer();
            IDictionary<String, String> actual = new Dictionary<String, String>();
            //Traverse to ellipse
            ISvgNodeRenderer ellipse = nodeRenderer.GetChildren()[0];
            actual.Put("stroke", ellipse.GetAttribute("stroke"));
            actual.Put("stroke-width", ellipse.GetAttribute("stroke-width"));
            actual.Put("stroke-opacity", ellipse.GetAttribute("stroke-opacity"));
            IDictionary<String, String> expected = new Dictionary<String, String>();
            expected.Put("stroke-width", "1.76388889");
            expected.Put("stroke", "#da0000");
            expected.Put("stroke-opacity", "1");
            NUnit.Framework.Assert.AreEqual(expected, actual);
        }

        [NUnit.Framework.Test]
        public virtual void StylesOfSvgTagProcessingTest() {
            String svg = "<?xml version=\"1.0\" standalone=\"no\"?>\n" + "<!DOCTYPE svg PUBLIC \"-//W3C//DTD SVG 1.1//EN\"\n"
                 + "        \"http://www.w3.org/Graphics/SVG/1.1/DTD/svg11.dtd\">\n" + "<svg width=\"400\" height=\"200\"\n"
                 + "     viewBox=\"0 0 400 200\" version=\"1.1\"\n" + "     xmlns=\"http://www.w3.org/2000/svg\"\n" + 
                "     xmlns:xlink=\"http://www.w3.org/1999/xlink\"\n" + "     xmlns:v=\"http://schemas.microsoft.com/visio/2003/SVGExtensions/\"\n"
                 + "     class=\"st11\">\n" + "    <style type=\"text/css\">\n" + "        .st11 {fill:none;stroke:black;stroke-width:6}\n"
                 + "    </style>\n" + "    <g >\n" + "        <path d=\"M0 100 L0 50 L70 50\"/>\n" + "    </g>\n" + "</svg>";
            JsoupXmlParser xmlParser = new JsoupXmlParser();
            IDocumentNode root = xmlParser.Parse(svg);
            IBranchSvgNodeRenderer nodeRenderer = (IBranchSvgNodeRenderer)new DefaultSvgProcessor().Process(root, null
                ).GetRootRenderer();
            PathSvgNodeRenderer pathSvgNodeRenderer = (PathSvgNodeRenderer)((IBranchSvgNodeRenderer)nodeRenderer.GetChildren
                ()[0]).GetChildren()[0];
            IDictionary<String, String> actual = new Dictionary<String, String>();
            actual.Put("stroke", pathSvgNodeRenderer.GetAttribute("stroke"));
            actual.Put("fill", pathSvgNodeRenderer.GetAttribute("fill"));
            actual.Put("d", pathSvgNodeRenderer.GetAttribute("d"));
            IDictionary<String, String> expected = new Dictionary<String, String>();
            expected.Put("stroke", "black");
            expected.Put("fill", "none");
            expected.Put("d", "M0 100 L0 50 L70 50");
            NUnit.Framework.Assert.AreEqual(expected, actual);
        }

        [NUnit.Framework.Test]
        public virtual void FontResolverIntegrationTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "fontssvg");
        }

        [NUnit.Framework.Test]
        public virtual void ValidLocalFontTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "validLocalFontTest");
        }

        [NUnit.Framework.Test]
        public virtual void FontWeightTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "fontWeightTest");
        }

        [NUnit.Framework.Test]
        [LogMessage(SvgLogMessageConstant.UNMAPPED_TAG, LogLevel = LogLevelConstants.WARN)]
        public virtual void ExternalStyleSheetWithFillStyleTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "externalStyleSheetWithFillStyleTest");
        }

        [NUnit.Framework.Test]
        [LogMessage(SvgLogMessageConstant.UNMAPPED_TAG, LogLevel = LogLevelConstants.WARN)]
        public virtual void ExternalStyleSheetWithStrokeStyleTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "externalStyleSheetWithStrokeStyleTest");
        }

        [NUnit.Framework.Test]
        public virtual void GoogleFontsTest() {
            // Android-Conversion-Ignore-Test (TODO DEVSIX-6459 Android: fix the SecurityException(Permission denied) from UrlUtil method)
            ConvertAndCompare(sourceFolder, destinationFolder, "googleFontsTest");
        }

        [NUnit.Framework.Test]
        public virtual void SvgWithExternalCSStoSingleDefaultPage() {
            // TODO: update cmp files when DEVSIX-4396 resolved
            // TODO: update cmp files when DEVSIX-6405 resolved
            // TODO: update cmp files when DEVSIX-8821 resolved
            // TODO: update cmp files when DEVSIX-8822 resolved
            // TODO: update cmp files when DEVSIX-8832 resolved
            ConvertAndCompare(sourceFolder, destinationFolder, "externalCss");
        }

        [NUnit.Framework.Test]
        public virtual void SvgWithInternalCSStoSingleDefaultPage() {
            // TODO: update cmp files when DEVSIX-4396 resolved
            // TODO: update cmp files when DEVSIX-6405 resolved
            // TODO: update cmp files when DEVSIX-8821 resolved
            // TODO: update cmp files when DEVSIX-8822 resolved
            // TODO: update cmp files when DEVSIX-8832 resolved
            ConvertAndCompare(sourceFolder, destinationFolder, "internalCss");
        }

        [NUnit.Framework.Test]
        public virtual void SvgWithInternalCSSWithoutOverlapTest() {
            // TODO: update cmp files when DEVSIX-8822 resolved
            // TODO: update cmp files when DEVSIX-8832 resolved
            ConvertAndCompare(sourceFolder, destinationFolder, "internalCssNoOverlap");
        }

        [NUnit.Framework.Test]
        public virtual void SvgWithExternalCSStoCustomPage() {
            // TODO: update cmp files when DEVSIX-4396 resolved
            // TODO: update cmp files when DEVSIX-6405 resolved
            // TODO: update cmp files when DEVSIX-8821 resolved
            // TODO: update cmp files when DEVSIX-8822 resolved
            // TODO: update cmp files when DEVSIX-8832 resolved
            // Take a note this method differs from the one used in Default Page test
            ConvertAndCompare(sourceFolder, destinationFolder, "externalCss_custom", PageSize.A3.Rotate());
        }

        [NUnit.Framework.Test]
        public virtual void SvgWithInternalCSStoCustomPage() {
            // TODO: update cmp files when DEVSIX-4396 resolved
            // TODO: update cmp files when DEVSIX-6405 resolved
            // TODO: update cmp files when DEVSIX-8821 resolved
            // TODO: update cmp files when DEVSIX-8822 resolved
            // TODO: update cmp files when DEVSIX-8832 resolved
            ConvertAndCompare(sourceFolder, destinationFolder, "internalCss_custom", PageSize.A3.Rotate());
        }

        [NUnit.Framework.Test]
        public virtual void MultipleSVGtagsWithDiffStylesFromExternalCSS() {
            // TODO: update cmp files when DEVSIX-8823 resolved
            // TODO: update cmp files when DEVSIX-8832 resolved
            ConvertAndCompare(sourceFolder, destinationFolder, "externalCss_palette", PageSize.A3.Rotate());
        }

        [NUnit.Framework.Test]
        public virtual void RelativeStyleInheritanceTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "relativeStyleInheritanceTest");
        }

        [NUnit.Framework.Test]
        public virtual void TextTagNoFontSizeTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "textTagNoFontSize");
        }

        [NUnit.Framework.Test]
        public virtual void ChartWithText1Test() {
            ConvertAndCompare(sourceFolder, destinationFolder, "chartWithText1");
        }

        [NUnit.Framework.Test]
        public virtual void ChartWithText2Test() {
            ConvertAndCompare(sourceFolder, destinationFolder, "chartWithText2");
        }

        [NUnit.Framework.Test]
        public virtual void ImportStyleSheetWithStrokeStyleTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "importStyleSheetWithStrokeStyleTest");
        }

        [NUnit.Framework.Test]
        public virtual void StyleInCdataTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "styleInCDATA");
        }

        [NUnit.Framework.Test]
        public virtual void StyleInCdataWithNewLineBeforeTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "styleInCdataWithNewLineBefore");
        }

        [NUnit.Framework.Test]
        public virtual void CssStylesResolverOrder1Test() {
            ConvertAndCompare(sourceFolder, destinationFolder, "cssStylesResolverOrder1");
        }

        [NUnit.Framework.Test]
        public virtual void CssStylesResolverOrder2Test() {
            ConvertAndCompare(sourceFolder, destinationFolder, "cssStylesResolverOrder2");
        }

        [NUnit.Framework.Test]
        public virtual void CssStylesResolverOrder3Test() {
            ConvertAndCompare(sourceFolder, destinationFolder, "cssStylesResolverOrder3");
        }

        [NUnit.Framework.Test]
        public virtual void CssStylesResolverOrder4Test() {
            ConvertAndCompare(sourceFolder, destinationFolder, "cssStylesResolverOrder4");
        }

        [NUnit.Framework.Test]
        public virtual void CssStylesResolverOrder5Test() {
            ConvertAndCompare(sourceFolder, destinationFolder, "cssStylesResolverOrder5");
        }

        [NUnit.Framework.Test]
        public virtual void CssStylesResolverOrder6Test() {
            ConvertAndCompare(sourceFolder, destinationFolder, "cssStylesResolverOrder6");
        }

        [NUnit.Framework.Test]
        public virtual void CssStylesResolverOrder7Test() {
            ConvertAndCompare(sourceFolder, destinationFolder, "cssStylesResolverOrder7");
        }

        [NUnit.Framework.Test]
        public virtual void CssStylesResolverOrder8Test() {
            ConvertAndCompare(sourceFolder, destinationFolder, "cssStylesResolverOrder8");
        }

        [NUnit.Framework.Test]
        [LogMessage(SvgLogMessageConstant.UNMAPPED_TAG, LogLevel = LogLevelConstants.WARN)]
        public virtual void CssStylesResolverOrder9Test() {
            ConvertAndCompare(sourceFolder, destinationFolder, "cssStylesResolverOrder9");
        }

        [NUnit.Framework.Test]
        [LogMessage(SvgLogMessageConstant.UNMAPPED_TAG, LogLevel = LogLevelConstants.WARN)]
        public virtual void CssStylesResolverOrder10Test() {
            ConvertAndCompare(sourceFolder, destinationFolder, "cssStylesResolverOrder10");
        }

        [NUnit.Framework.Test]
        [LogMessage(SvgLogMessageConstant.UNMAPPED_TAG, LogLevel = LogLevelConstants.WARN)]
        public virtual void CssStylesResolverOrder11Test() {
            ConvertAndCompare(sourceFolder, destinationFolder, "cssStylesResolverOrder11");
        }

        [NUnit.Framework.Test]
        [LogMessage(SvgLogMessageConstant.UNMAPPED_TAG, LogLevel = LogLevelConstants.WARN)]
        public virtual void CssStylesResolverOrder12Test() {
            ConvertAndCompare(sourceFolder, destinationFolder, "cssStylesResolverOrder12");
        }

        [NUnit.Framework.Test]
        public virtual void CssStylesResolverOrder13Test() {
            ConvertAndCompare(sourceFolder, destinationFolder, "cssStylesResolverOrder13");
        }

        [NUnit.Framework.Test]
        public virtual void CssStylesResolverOrder14Test() {
            ConvertAndCompare(sourceFolder, destinationFolder, "cssStylesResolverOrder14");
        }

        [NUnit.Framework.Test]
        public virtual void CssStylesResolverOrder15Test() {
            ConvertAndCompare(sourceFolder, destinationFolder, "cssStylesResolverOrder15");
        }

        [NUnit.Framework.Test]
        public virtual void CssStylesResolverOrder16Test() {
            ConvertAndCompare(sourceFolder, destinationFolder, "cssStylesResolverOrder16");
        }

        [NUnit.Framework.Test]
        public virtual void CssStylesResolverOrder17Test() {
            ConvertAndCompare(sourceFolder, destinationFolder, "cssStylesResolverOrder17");
        }

        [NUnit.Framework.Test]
        public virtual void HeightWidthSvgStyleTest() {
            //TODO DEVSIX-8823: update after issue is fixed
            ConvertAndCompare(sourceFolder, destinationFolder, "height-width-style");
        }
    }
}
