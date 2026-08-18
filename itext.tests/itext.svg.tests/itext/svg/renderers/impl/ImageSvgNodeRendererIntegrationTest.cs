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
using iText.IO.Image;
using iText.Svg.Logs;
using iText.Svg.Processors;
using iText.Svg.Processors.Impl;
using iText.Svg.Renderers;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Svg.Renderers.Impl {
    [NUnit.Framework.Category("IntegrationTest")]
    public class ImageSvgNodeRendererIntegrationTest : SvgIntegrationTest {
        private static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/svg/renderers/impl/ImageSvgNodeRendererTest/";

        private static readonly String DESTINATION_FOLDER = TestUtil.GetOutputPath() + "/svg/renderers/impl/ImageSvgNodeRendererTest/";

        private ISvgConverterProperties properties;

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            ITextTest.CreateDestinationFolder(DESTINATION_FOLDER);
        }

        [NUnit.Framework.SetUp]
        public virtual void Before() {
            properties = new SvgConverterProperties().SetBaseUri(SOURCE_FOLDER);
        }

        [NUnit.Framework.Test]
        public virtual void SingleImageTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "singleImage", properties);
        }

        [NUnit.Framework.Test]
        public virtual void SingleImageHrefTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "singleImageHref", properties);
        }

        [NUnit.Framework.Test]
        public virtual void ImageWithRectangleTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "imageWithRectangle", properties);
        }

        [NUnit.Framework.Test]
        public virtual void ImageWithMultipleShapesTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "imageWithMultipleShapes", properties);
        }

        [NUnit.Framework.Test]
        public virtual void ImageXYTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "imageXY", properties);
        }

        [NUnit.Framework.Test]
        public virtual void MultipleImagesTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "multipleImages", properties);
        }

        [NUnit.Framework.Test]
        public virtual void NonSquareImageTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "nonSquareImage", properties);
        }

        [NUnit.Framework.Test]
        public virtual void SingleImageTranslateTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "singleImageTranslate", properties);
        }

        [NUnit.Framework.Test]
        public virtual void SingleImageRotateTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "singleImageRotate", properties);
        }

        [NUnit.Framework.Test]
        public virtual void SingleImageScaleUpTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "singleImageScaleUp", properties);
        }

        [NUnit.Framework.Test]
        public virtual void SingleImageScaleDownTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "singleImageScaleDown", properties);
        }

        [NUnit.Framework.Test]
        public virtual void SingleImageMultipleTransformationsTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "singleImageMultipleTransformations", properties
                );
        }

        [NUnit.Framework.Test]
        public virtual void TwoImagesWithTransformationsTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "twoImagesWithTransformations", properties);
        }

        [NUnit.Framework.Test]
        public virtual void DifferentDimensionsTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "differentDimensions", properties);
        }

        [NUnit.Framework.Test]
        public virtual void ImageWithTransparencyTest() {
            //TODO: update cmp_ when DEVSIX-2258 fixed
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "imageWithTransparency", properties);
        }

        [NUnit.Framework.Test]
        public virtual void ImageWithPreserveAspectRatioNoneTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "imageWithPreserveAspectRatioNone", properties
                );
        }

        [NUnit.Framework.Test]
        public virtual void ImageWithPreserveAspectRatioInvalidValueTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "imageWithPreserveAspectRatioInvalidValue", 
                properties);
        }

        [NUnit.Framework.Test]
        public virtual void ImageWithPreserveAspectRatioXMinYMinTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "imageWithPreserveAspectRatioXMinYMin", properties
                );
        }

        [NUnit.Framework.Test]
        public virtual void ImageWithPreserveAspectRatioXMinYMidTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "imageWithPreserveAspectRatioXMinYMid", properties
                );
        }

        [NUnit.Framework.Test]
        public virtual void ImageWithPreserveAspectRatioXMinYMaxTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "imageWithPreserveAspectRatioXMinYMax", properties
                );
        }

        [NUnit.Framework.Test]
        public virtual void ImageWithPreserveAspectRatioXMidYMinTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "imageWithPreserveAspectRatioXMidYMin", properties
                );
        }

        [NUnit.Framework.Test]
        public virtual void ImageWithPreserveAspectRatioXMidYMidTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "imageWithPreserveAspectRatioXMidYMid", properties
                );
        }

        [NUnit.Framework.Test]
        public virtual void ImageWithPreserveAspectRatioXMidYMaxTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "imageWithPreserveAspectRatioXMidYMax", properties
                );
        }

        [NUnit.Framework.Test]
        public virtual void ImageWithPreserveAspectRatioXMaxYMinTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "imageWithPreserveAspectRatioXMaxYMin", properties
                );
        }

        [NUnit.Framework.Test]
        public virtual void ImageWithPreserveAspectRatioXMaxYMidTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "imageWithPreserveAspectRatioXMaxYMid", properties
                );
        }

        [NUnit.Framework.Test]
        public virtual void ImageWithPreserveAspectRatioXMaxYMaxTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "imageWithPreserveAspectRatioXMaxYMax", properties
                );
        }

        [NUnit.Framework.Test]
        public virtual void ImageRenderingTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "image-rendering", properties);
        }

        [NUnit.Framework.Test]
        [LogMessage(SvgLogMessageConstant.UNMAPPED_TAG)]
        public virtual void ImageWithDescriptionsTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "image-descriptions", properties);
        }

        //TODO DEVSIX-4589: update after supporting
        //TODO DEVSIX-4901: update after supporting
        [NUnit.Framework.Test]
        public virtual void ImageBase64WithUrlTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "base64Image", properties);
        }

        [NUnit.Framework.Test]
        [LogMessage(WebPLogMessageConstant.WEBP_NOT_FOUND)]
        [LogMessage(iText.StyledXmlParser.Logs.StyledXmlParserLogMessageConstant.UNABLE_TO_PROCESS_IMAGE_WITH_GIVEN_BASE_URI
            )]
        public virtual void WebPImageWithoutWebPModuleTest() {
            ConvertToSinglePage(new FileInfo(SOURCE_FOLDER + "webPImageWithoutWebPModule.svg"), new FileInfo(DESTINATION_FOLDER
                 + "webPImageWithoutWebPModule.pdf"), properties);
        }
    }
}
