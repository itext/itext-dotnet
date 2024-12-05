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
using iText.Commons.Utils;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using iText.Layout;
using iText.StyledXmlParser.Node;
using iText.StyledXmlParser.Resolver.Resource;
using iText.Svg.Converter;
using iText.Svg.Element;
using iText.Svg.Processors;
using iText.Svg.Processors.Impl;
using iText.Svg.Utils;
using iText.Svg.Xobject;

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
                Rectangle wh = SvgCssUtils.ExtractWidthAndHeight(topSvgRenderer, 0.0F, 0.0F);
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
                Rectangle wh = SvgCssUtils.ExtractWidthAndHeight(topSvgRenderer, 0.0F, 0.0F);
                SvgImageXObject svgImageXObject = new SvgImageXObject(wh, result, new ResourceResolver(SOURCE_FOLDER));
                SvgImage svgImage = new SvgImage(svgImageXObject);
                document.Add(svgImage);
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
                Rectangle wh = SvgCssUtils.ExtractWidthAndHeight(topSvgRenderer, 0.0F, 0.0F);
                document.Add(new SvgImage(new SvgImageXObject(wh, result, new ResourceResolver(SOURCE_FOLDER))));
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff"));
        }
    }
}
