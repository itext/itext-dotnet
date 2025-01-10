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
using iText.Commons.Utils;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Logs;
using iText.Layout.Properties;
using iText.StyledXmlParser.Node;
using iText.StyledXmlParser.Resolver.Resource;
using iText.Svg.Converter;
using iText.Svg.Element;
using iText.Svg.Processors;
using iText.Svg.Processors.Impl;
using iText.Svg.Utils;
using iText.Svg.Xobject;
using iText.Test.Attributes;

namespace iText.Svg.Renderers {
    [NUnit.Framework.Category("IntegrationTest")]
    public class SvgImageRendererTest : SvgIntegrationTest {
        public static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/svg/renderers/SvgImageRendererTest/";

        public static readonly String DESTINATION_FOLDER = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/svg/SvgImageRendererTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateDestinationFolder(DESTINATION_FOLDER);
        }

        [NUnit.Framework.Test]
        public virtual void SvgWithSvgTest() {
            String svgFileName = SOURCE_FOLDER + "svgWithSvg.svg";
            String cmpFileName = SOURCE_FOLDER + "cmp_svgWithSvg.pdf";
            String outFileName = DESTINATION_FOLDER + "svgWithSvg.pdf";
            using (Document document = new Document(new PdfDocument(new PdfWriter(outFileName, new WriterProperties().
                SetCompressionLevel(0))))) {
                INode parsedSvg = SvgConverter.Parse(FileUtil.GetInputStreamForFile(svgFileName));
                ISvgProcessorResult result = new DefaultSvgProcessor().Process(parsedSvg, null);
                ISvgNodeRenderer topSvgRenderer = result.GetRootRenderer();
                Rectangle wh = SvgCssUtils.ExtractWidthAndHeight(topSvgRenderer, 0.0F, new SvgDrawContext(null, null));
                SvgImageXObject svgImageXObject = new SvgImageXObject(wh, result, new ResourceResolver(SOURCE_FOLDER));
                SvgImage svgImage = new SvgImage(svgImageXObject);
                document.Add(svgImage);
                document.Add(svgImage);
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void CustomSvgImageTest() {
            String svgFileName = SOURCE_FOLDER + "svgImage.svg";
            String cmpFileName = SOURCE_FOLDER + "cmp_svgImage.pdf";
            String outFileName = DESTINATION_FOLDER + "svgImage.pdf";
            using (Document document = new Document(new PdfDocument(new PdfWriter(outFileName, new WriterProperties().
                SetCompressionLevel(0))))) {
                INode parsedSvg = SvgConverter.Parse(FileUtil.GetInputStreamForFile(svgFileName));
                ISvgProcessorResult result = new DefaultSvgProcessor().Process(parsedSvg, new SvgConverterProperties().SetBaseUri
                    (svgFileName));
                ISvgNodeRenderer topSvgRenderer = result.GetRootRenderer();
                Rectangle wh = SvgCssUtils.ExtractWidthAndHeight(topSvgRenderer, 0.0F, new SvgDrawContext(null, null));
                SvgImageXObject svgImageXObject = new SvgImageXObject(wh, result, new ResourceResolver(SOURCE_FOLDER));
                SvgImage svgImage = new SvgImage(svgImageXObject);
                document.Add(svgImage);
                document.Add(svgImage);
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void SvgImageWithBackgroundTest() {
            String svgFileName = SOURCE_FOLDER + "svgImageWithBackground.svg";
            String cmpFileName = SOURCE_FOLDER + "cmp_svgImageWithBackground.pdf";
            String outFileName = DESTINATION_FOLDER + "svgImageWithBackground.pdf";
            using (Document document = new Document(new PdfDocument(new PdfWriter(outFileName, new WriterProperties().
                SetCompressionLevel(0))))) {
                INode parsedSvg = SvgConverter.Parse(FileUtil.GetInputStreamForFile(svgFileName));
                ISvgProcessorResult result = new DefaultSvgProcessor().Process(parsedSvg, new SvgConverterProperties().SetBaseUri
                    (svgFileName));
                ISvgNodeRenderer topSvgRenderer = result.GetRootRenderer();
                Rectangle wh = SvgCssUtils.ExtractWidthAndHeight(topSvgRenderer, 0.0F, new SvgDrawContext(null, null));
                SvgImageXObject svgImageXObject = new SvgImageXObject(wh, result, new ResourceResolver(SOURCE_FOLDER));
                SvgImage svgImage = new SvgImage(svgImageXObject);
                document.Add(svgImage);
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void NoSpecifiedWidthHeightImageTest() {
            String svgFileName = SOURCE_FOLDER + "noWidthHeightSvgImage.svg";
            String cmpFileName = SOURCE_FOLDER + "cmp_noWidthHeightSvg.pdf";
            String outFileName = DESTINATION_FOLDER + "noWidthHeightSvg.pdf";
            using (Document document = new Document(new PdfDocument(new PdfWriter(outFileName, new WriterProperties().
                SetCompressionLevel(0))))) {
                INode parsedSvg = SvgConverter.Parse(FileUtil.GetInputStreamForFile(svgFileName));
                ISvgProcessorResult result = new DefaultSvgProcessor().Process(parsedSvg, new SvgConverterProperties().SetBaseUri
                    (svgFileName));
                ISvgNodeRenderer topSvgRenderer = result.GetRootRenderer();
                Rectangle wh = SvgCssUtils.ExtractWidthAndHeight(topSvgRenderer, 0.0F, new SvgDrawContext(null, null));
                document.Add(new SvgImage(new SvgImageXObject(wh, result, new ResourceResolver(SOURCE_FOLDER))));
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void RelativeSizedSvg1Test() {
            String svgName = "fixed_height_percent_width";
            String svgFileName = SOURCE_FOLDER + svgName + ".svg";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + svgName + ".pdf";
            String outFileName = DESTINATION_FOLDER + svgName + ".pdf";
            using (Document document = new Document(new PdfDocument(new PdfWriter(outFileName, new WriterProperties().
                SetCompressionLevel(0))))) {
                INode parsedSvg = SvgConverter.Parse(FileUtil.GetInputStreamForFile(svgFileName));
                ISvgProcessorResult result = new DefaultSvgProcessor().Process(parsedSvg, new SvgConverterProperties().SetBaseUri
                    (svgFileName));
                SvgDrawContext svgDrawContext = new SvgDrawContext(new ResourceResolver(SOURCE_FOLDER), null);
                SvgImageXObject svgImageXObject = new SvgImageXObject(result, svgDrawContext, 12, document.GetPdfDocument(
                    ));
                SvgImage svgImage = new SvgImage(svgImageXObject);
                svgImage.SetWidth(100);
                svgImage.SetHeight(300);
                document.Add(svgImage);
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void RelativeSizedSvg3Test() {
            String svgName = "viewbox_fixed_height_percent_width";
            String svgFileName = SOURCE_FOLDER + svgName + ".svg";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + svgName + ".pdf";
            String outFileName = DESTINATION_FOLDER + svgName + ".pdf";
            using (Document document = new Document(new PdfDocument(new PdfWriter(outFileName, new WriterProperties().
                SetCompressionLevel(0))))) {
                INode parsedSvg = SvgConverter.Parse(FileUtil.GetInputStreamForFile(svgFileName));
                ISvgProcessorResult result = new DefaultSvgProcessor().Process(parsedSvg, new SvgConverterProperties().SetBaseUri
                    (svgFileName));
                SvgDrawContext svgDrawContext = new SvgDrawContext(new ResourceResolver(SOURCE_FOLDER), null);
                SvgImageXObject svgImageXObject = new SvgImageXObject(result, svgDrawContext, 12, document.GetPdfDocument(
                    ));
                SvgImage svgImage = new SvgImage(svgImageXObject);
                svgImage.SetWidth(100);
                svgImage.SetHeight(300);
                document.Add(svgImage);
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff"));
        }

        [NUnit.Framework.Test]
        [LogMessage(LayoutLogMessageConstant.ELEMENT_DOES_NOT_FIT_AREA)]
        public virtual void RelativeSizedSvg4Test() {
            String svgName = "viewbox_percent_height_percent_width";
            String svgFileName = SOURCE_FOLDER + svgName + ".svg";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + svgName + ".pdf";
            String outFileName = DESTINATION_FOLDER + svgName + ".pdf";
            using (Document document = new Document(new PdfDocument(new PdfWriter(outFileName, new WriterProperties().
                SetCompressionLevel(0))))) {
                INode parsedSvg = SvgConverter.Parse(FileUtil.GetInputStreamForFile(svgFileName));
                ISvgProcessorResult result = new DefaultSvgProcessor().Process(parsedSvg, new SvgConverterProperties().SetBaseUri
                    (svgFileName));
                SvgDrawContext svgDrawContext = new SvgDrawContext(new ResourceResolver(SOURCE_FOLDER), null);
                SvgImageXObject svgImageXObject = new SvgImageXObject(result, svgDrawContext, 12, document.GetPdfDocument(
                    ));
                SvgImage svgImage = new SvgImage(svgImageXObject);
                document.Add(svgImage);
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void RelativeSizedSvg5Test() {
            String svgName = "viewbox_percent_height_percent_width_prRatio_none";
            String svgFileName = SOURCE_FOLDER + svgName + ".svg";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + svgName + ".pdf";
            String outFileName = DESTINATION_FOLDER + svgName + ".pdf";
            using (Document document = new Document(new PdfDocument(new PdfWriter(outFileName, new WriterProperties().
                SetCompressionLevel(0))))) {
                INode parsedSvg = SvgConverter.Parse(FileUtil.GetInputStreamForFile(svgFileName));
                ISvgProcessorResult result = new DefaultSvgProcessor().Process(parsedSvg, new SvgConverterProperties().SetBaseUri
                    (svgFileName));
                SvgDrawContext svgDrawContext = new SvgDrawContext(new ResourceResolver(SOURCE_FOLDER), null);
                SvgImageXObject svgImageXObject = new SvgImageXObject(result, svgDrawContext, 12, document.GetPdfDocument(
                    ));
                Div div = new Div();
                div.SetWidth(100);
                div.SetHeight(300);
                SvgImage svgImage = new SvgImage(svgImageXObject);
                svgImage.SetWidth(UnitValue.CreatePercentValue(100));
                svgImage.SetHeight(UnitValue.CreatePercentValue(100));
                div.Add(svgImage);
                document.Add(div);
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff"));
        }
    }
}
