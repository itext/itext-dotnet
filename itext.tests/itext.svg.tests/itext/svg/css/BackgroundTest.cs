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
using iText.Svg.Processors;
using iText.Svg.Processors.Impl;
using iText.Svg.Renderers;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Svg.Css {
    [NUnit.Framework.Category("IntegrationTest")]
    public class BackgroundTest : SvgIntegrationTest {
        //TODO DEVSIX-8832: Update cmp files
        private static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/svg/css/BackgroundTest/";

        private static readonly String DESTINATION_FOLDER = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/svg/css/BackgroundTest/";

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
        public virtual void TopSVGBackGroundColorNameTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "topSVGBackGroundColorName", properties);
        }

        [NUnit.Framework.Test]
        public virtual void TopSVGBackGroundBorderBoxAndColorNameTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "topSVGBackGroundBorderBoxAndColorName", properties
                );
        }

        [NUnit.Framework.Test]
        public virtual void BackgroundTopLevelLinGradTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "backgroundTopLevelLinGrad", properties);
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.StyledXmlParser.Logs.StyledXmlParserLogMessageConstant.WAS_NOT_ABLE_TO_DEFINE_BACKGROUND_CSS_SHORTHAND_PROPERTIES
            )]
        public virtual void BackgroundTopLevelRadGradTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "backgroundTopLevelRadGrad", properties);
        }

        [NUnit.Framework.Test]
        public virtual void TopSVGBackGroundImageTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "topSVGBackGroundImage", properties);
        }

        [NUnit.Framework.Test]
        public virtual void TopSVGImageSHNoRepeatCenterRelSizeTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "topSVGImageSHNoRepeatCenterRelSize", properties
                );
        }

        [NUnit.Framework.Test]
        public virtual void TopSVGImageSHNoRepeatRightRelSizeTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "topSVGImageSHNoRepeatRightRelSize", properties
                );
        }

        [NUnit.Framework.Test]
        public virtual void TopSVGRepeatYTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "topSVGRepeatY", properties);
        }

        [NUnit.Framework.Test]
        public virtual void TopSVGRepeatYShortHandTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "topSVGRepeatYShortHand", properties);
        }

        [NUnit.Framework.Test]
        public virtual void TopSVGRepeatXTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "topSVGRepeatX", properties);
        }

        [NUnit.Framework.Test]
        public virtual void TopSVGRepeatXShortHandTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "topSVGRepeatXShortHand", properties);
        }

        [NUnit.Framework.Test]
        public virtual void TopSVGRepeatTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "topSVGRepeat", properties);
        }

        [NUnit.Framework.Test]
        public virtual void TopSVGRepeatShortHandTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "topSVGRepeatShortHand", properties);
        }

        [NUnit.Framework.Test]
        public virtual void TopSVGRepeatSpaceTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "topSVGRepeatSpace", properties);
        }

        [NUnit.Framework.Test]
        public virtual void TopSVGRepeatSpaceShortHandTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "topSVGRepeatSpaceShortHand", properties);
        }

        [NUnit.Framework.Test]
        public virtual void TopSVGRepeatRoundTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "topSVGRepeatRound", properties);
        }

        [NUnit.Framework.Test]
        public virtual void TopSVGRepeatRoundShortHandTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "topSVGRepeatRoundShortHand", properties);
        }

        [NUnit.Framework.Test]
        public virtual void TopSVGNoRepeatTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "topSVGNoRepeat", properties);
        }

        [NUnit.Framework.Test]
        public virtual void TopSVGNoRepeatShortHandTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "topSVGNoRepeatShortHand", properties);
        }

        [NUnit.Framework.Test]
        public virtual void TopSVGSpaceRepeatTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "topSVGSpaceRepeat", properties);
        }

        [NUnit.Framework.Test]
        public virtual void TopSVGSpaceRepeatShortHandTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "topSVGSpaceRepeatShortHand", properties);
        }

        [NUnit.Framework.Test]
        public virtual void TopSvgRepeatSizePixelsTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "topSvgRepeatSizePixels", properties);
        }

        [NUnit.Framework.Test]
        public virtual void TopSvgRepeatRoundSizePixelsTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "topSvgRepeatRoundSizePixels", properties);
        }

        [NUnit.Framework.Test]
        public virtual void TopSvgRepeatYSizeRelativePercentageTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "topSvgRepeatYSizeRelativePercentage", properties
                );
        }

        [NUnit.Framework.Test]
        public virtual void TopSvgRepeatXSizeRelativePercentageTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "topSvgRepeatXSizeRelativePercentage", properties
                );
        }

        [NUnit.Framework.Test]
        public virtual void TopSvgRepeatSizeRelativeDiffPercentageTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "topSvgRepeatSizeRelativeDiffPercentage", properties
                );
        }

        [NUnit.Framework.Test]
        public virtual void TopSvgRepeatSizeRelativePercentageTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "topSvgRepeatSizeRelativePercentage", properties
                );
        }

        [NUnit.Framework.Test]
        public virtual void TopSvgRepeatAndSVGRelSizeTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "topSvgRepeatAndSVGRelSize", properties);
        }

        [NUnit.Framework.Test]
        public virtual void TopSvgRelSizeRepeatTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "topSvgRelSizeRepeat", properties);
        }

        [NUnit.Framework.Test]
        public virtual void TopSVGImageBlendingMultiplyTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "topSVGImageBlendingMultiply", properties);
        }

        [NUnit.Framework.Test]
        public virtual void TopSVGImageBlendingScreenTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "topSVGImageBlendingScreen", properties);
        }

        [NUnit.Framework.Test]
        public virtual void TopSVGImageBlendingHardLightTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "topSVGImageBlendingHardLight", properties);
        }

        [NUnit.Framework.Test]
        public virtual void TopSVGImageBlendingDifferenceTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "topSVGImageBlendingDifference", properties
                );
        }

        [NUnit.Framework.Test]
        public virtual void TopSVGImageBlendingDarkenTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "topSVGImageBlendingDarken", properties);
        }

        [NUnit.Framework.Test]
        public virtual void TopSVGImageBlendingLuminosityTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "topSVGImageBlendingLuminosity", properties
                );
        }

        [NUnit.Framework.Test]
        public virtual void TopSVGImageBlendingDarkenLuminosityTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "topSVGImageBlendingDarkenLuminosity", properties
                );
        }

        [NUnit.Framework.Test]
        public virtual void TopSVGBackGroundColorHexTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "topSVGBackGroundColorHex", properties);
        }

        [NUnit.Framework.Test]
        public virtual void TopSVGBackGroundColorByNameTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "topSVGBackGroundColorByName", properties);
        }

        [NUnit.Framework.Test]
        public virtual void TopSVGBackGroundColorRGBTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "topSVGBackGroundColorRGB", properties);
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.StyledXmlParser.Logs.StyledXmlParserLogMessageConstant.INVALID_CSS_PROPERTY_DECLARATION)]
        public virtual void TopSVGBackGroundColorHSLTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "topSVGBackGroundColorHSL", properties);
        }

        [NUnit.Framework.Test]
        public virtual void TopSVGBackGroundColorTransparentTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "topSVGBackGroundColorTransparent", properties
                );
        }

        [NUnit.Framework.Test]
        public virtual void TopSVGBackGroundColorCurrentColorTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "topSVGBackGroundColorCurrentColor", properties
                );
        }

        [NUnit.Framework.Test]
        public virtual void TopSVGPositionRightTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "topSVGPositionRight", properties);
        }

        [NUnit.Framework.Test]
        public virtual void TopSVGPositionLeftTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "topSVGPositionLeft", properties);
        }

        [NUnit.Framework.Test]
        public virtual void TopSVGPositionTopTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "topSVGPositionTop", properties);
        }

        [NUnit.Framework.Test]
        public virtual void TopSVGPositionBottomTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "topSVGPositionBottom", properties);
        }

        [NUnit.Framework.Test]
        public virtual void TopSVGPositionCenterTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "topSVGPositionCenter", properties);
        }

        [NUnit.Framework.Test]
        public virtual void TopSVGPositionPercentageTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "topSVGPositionPercentage", properties);
        }

        [NUnit.Framework.Test]
        public virtual void TopSVGPositionMixedPixelsTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "topSVGPositionMixedPixels", properties);
        }

        [NUnit.Framework.Test]
        public virtual void TopSVGPositionMixedPercentageTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "topSVGPositionMixedPercentage", properties
                );
        }

        [NUnit.Framework.Test]
        public virtual void TopSVGPositionXRightTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "topSVGPositionXRight", properties);
        }

        [NUnit.Framework.Test]
        public virtual void TopSVGPositionXPercentageTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "topSVGPositionXPercentage", properties);
        }

        [NUnit.Framework.Test]
        public virtual void TopSVGPositionXRemTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "topSVGPositionXRem", properties);
        }

        [NUnit.Framework.Test]
        public virtual void TopSVGPositionXRightPixelsTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "topSVGPositionXRightPixels", properties);
        }

        [NUnit.Framework.Test]
        public virtual void TopSVGPositionYCenterTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "topSVGPositionYCenter", properties);
        }

        [NUnit.Framework.Test]
        public virtual void TopSVGPositionYPercentageTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "topSVGPositionYPercentage", properties);
        }

        [NUnit.Framework.Test]
        public virtual void TopSVGPositionYRemTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "topSVGPositionYRem", properties);
        }

        [NUnit.Framework.Test]
        public virtual void TopSVGPositionYBottomPixelsTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "topSVGPositionYBottomPixels", properties);
        }

        [NUnit.Framework.Test]
        public virtual void TopSVGSizeCoverTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "topSVGSizeCover", properties);
        }

        [NUnit.Framework.Test]
        public virtual void TopSVGSizeContainTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "topSVGSizeContain", properties);
        }

        [NUnit.Framework.Test]
        public virtual void TopSVGSizePercentageTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "topSVGSizePercentage", properties);
        }

        [NUnit.Framework.Test]
        public virtual void TopSVGSizePixelsTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "topSVGSizePixels", properties);
        }

        [NUnit.Framework.Test]
        public virtual void TopSVGViewBoxTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "topSVGViewBox", properties);
        }

        [NUnit.Framework.Test]
        public virtual void TopSVGViewBoxWithRelBGSizeTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "topSVGViewBoxWithRelBGSize", properties);
        }

        [NUnit.Framework.Test]
        public virtual void TopSVGViewBoxWithRelSVGSizeTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "topSVGViewBoxWithRelSVGSize", properties);
        }

        [NUnit.Framework.Test]
        public virtual void TopSVGAndInnerSVGViewBoxTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "topSVGAndInnerSVGViewBox", properties);
        }

        [NUnit.Framework.Test]
        public virtual void TopSVGViewBoxARNoneTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "topSVGViewBoxARNone", properties);
        }

        [NUnit.Framework.Test]
        public virtual void TopSVGViewBoxARNoneRelSizeSVGTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "topSVGViewBoxARNoneRelSizeSVG", properties
                );
        }

        [NUnit.Framework.Test]
        public virtual void TopSVGViewBoxARNoneRelSizeSVGAndBGTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "topSVGViewBoxARNoneRelSizeSVGAndBG", properties
                );
        }

        [NUnit.Framework.Test]
        public virtual void TopSVGViewBoxARNoneRelSizeBGTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "topSVGViewBoxARNoneRelSizeBG", properties);
        }

        [NUnit.Framework.Test]
        public virtual void TopSVGViewBoxARNoneBGSizeTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "topSVGViewBoxARNoneBGSize", properties);
        }

        [NUnit.Framework.Test]
        public virtual void TopSVGAspectRatioNoViewBoxTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "topSVGAspectRatioNoViewBox", properties);
        }

        [NUnit.Framework.Test]
        public virtual void TopSVGViewBoxNestedElementARTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "topSVGViewBoxNestedElementAR", properties);
        }

        //Expected to not show any background on inner elements.
        [NUnit.Framework.Test]
        public virtual void NestedElementsBGTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "nestedElementsBG", properties);
        }
    }
}
