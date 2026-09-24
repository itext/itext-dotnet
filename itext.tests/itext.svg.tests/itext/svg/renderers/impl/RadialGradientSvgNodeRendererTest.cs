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
using iText.Svg.Logs;
using iText.Svg.Renderers;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Svg.Renderers.Impl {
    [NUnit.Framework.Category("IntegrationTest")]
    public class RadialGradientSvgNodeRendererTest : SvgIntegrationTest {
        private static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/svg/renderers/impl/RadialGradientSvgNodeRendererTest/";

        private static readonly String DESTINATION_FOLDER = TestUtil.GetOutputPath() + "/svg/renderers/impl/RadialGradientSvgNodeRendererTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            ITextTest.CreateOrClearDestinationFolder(DESTINATION_FOLDER);
        }

        [NUnit.Framework.Test]
        public virtual void RadialGradientBasicTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "radialGradientBasic");
        }

        [NUnit.Framework.Test]
        public virtual void CircleTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "circle");
        }

        [NUnit.Framework.Test]
        public virtual void EllipseTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "ellipse");
        }

        [NUnit.Framework.Test]
        public virtual void RectTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "rect");
        }

        [NUnit.Framework.Test]
        public virtual void PolygonTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "polygon");
        }

        [NUnit.Framework.Test]
        public virtual void PathTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "path");
        }

        [NUnit.Framework.Test]
        public virtual void TextTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "text");
        }

        [NUnit.Framework.Test]
        public virtual void SpreadMethodPadTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "spreadMethodPad");
        }

        [NUnit.Framework.Test]
        public virtual void SpreadMethodReflectTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "spreadMethodReflect");
        }

        [NUnit.Framework.Test]
        public virtual void SpreadMethodRepeatTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "spreadMethodRepeat");
        }

        [NUnit.Framework.Test]
        [LogMessage(SvgLogMessageConstant.GRADIENT_INVALID_SPREAD_METHOD_LOG, LogLevel = LogLevelConstants.WARN)]
        public virtual void InvalidSpreadMethodTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "invalidSpreadMethod");
        }

        // TODO: DEVSIX-4136 update cmp_ after fix
        //  (opacity is not implemented. No stops defines no color, i.e. transparent color or black with 100% opacity)
        [NUnit.Framework.Test]
        public virtual void NoStopsTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "noStops");
        }

        [NUnit.Framework.Test]
        public virtual void SingleStop0Test() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "singleStop0");
        }

        [NUnit.Framework.Test]
        public virtual void SingleStop1Test() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "singleStop1");
        }

        [NUnit.Framework.Test]
        public virtual void StopWithoutColorTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "stopWithoutColor");
        }

        [NUnit.Framework.Test]
        public virtual void TransformedTargetTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "transformedTarget");
        }

        [NUnit.Framework.Test]
        public virtual void RectWithGradientTransformTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "rectWithGradientTransform");
        }

        [NUnit.Framework.Test]
        public virtual void RectWithMultipleTransformsTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "rectWithMultipleTransforms");
        }

        [NUnit.Framework.Test]
        public virtual void UserSpaceOnUseAbsoluteCoordinatesTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "userSpaceOnUseAbsoluteCoordinates");
        }

        [NUnit.Framework.Test]
        public virtual void UserSpaceOnUseRelativeUnitsTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "userSpaceOnUseRelativeUnits");
        }

        [NUnit.Framework.Test]
        public virtual void ObjectBoundingBoxAbsoluteCoordinatesTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "objectBoundingBoxAbsoluteCoordinates");
        }

        [NUnit.Framework.Test]
        public virtual void ObjectBoundingBoxRelativeCoordinatesTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "objectBoundingBoxRelativeCoordinates");
        }

        [NUnit.Framework.Test]
        [LogMessage(SvgLogMessageConstant.GRADIENT_INVALID_GRADIENT_UNITS_LOG, LogLevel = LogLevelConstants.WARN)]
        public virtual void InvalidGradientUnitsTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "invalidGradientUnits");
        }

        [NUnit.Framework.Test]
        public virtual void LowerCaseGradientUnitsAttributeTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "lowerCaseGradientUnitsAttribute");
        }

        [NUnit.Framework.Test]
        public virtual void HrefBasicReferenceTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "hrefBasicReference");
        }

        [NUnit.Framework.Test]
        public virtual void TransitiveHrefReferenceTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "transitiveHrefReference");
        }

        [NUnit.Framework.Test]
        public virtual void XlinkHrefReferenceTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "xlinkHrefReference");
        }

        [NUnit.Framework.Test]
        public virtual void HrefOverrideSpreadMethodTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "hrefOverrideSpreadMethod");
        }

        [NUnit.Framework.Test]
        public virtual void HrefOverrideGeometryTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "hrefOverrideGeometry");
        }

        [NUnit.Framework.Test]
        public virtual void ExplicitFocalPointTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "explicitFocalPoint");
        }

        [NUnit.Framework.Test]
        public virtual void NegativeFrTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "negativeFr");
        }

        [NUnit.Framework.Test]
        public virtual void EndRadiusEqualZeroTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "endRadiusEqualZero");
        }

        [NUnit.Framework.Test]
        public virtual void StartRadiusEqualZeroTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "startRadiusEqualZero");
        }

        [NUnit.Framework.Test]
        public virtual void EndRadiusLessThanZeroTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "endRadiusLessThanZero");
        }

        [NUnit.Framework.Test]
        public virtual void StartRadiusLessThanZeroTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "startRadiusLessThanZero");
        }

        [NUnit.Framework.Test]
        public virtual void EqualRadiiTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "equalRadii");
        }

        [NUnit.Framework.Test]
        public virtual void SimilarSizeRadiiTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "similarSizeRadii");
        }

        [NUnit.Framework.Test]
        public virtual void SecondCircleMuchBiggerTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "secondCircleMuchBigger");
        }

        [NUnit.Framework.Test]
        public virtual void PlaneToRightManyCirclesTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "planeToRightManyCircles");
        }

        [NUnit.Framework.Test]
        public virtual void PlaneToRightLargeSecondCircleTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "planeToRightLargeSecondCircle");
        }

        [NUnit.Framework.Test]
        public virtual void PlaneToLeftManyCirclesTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "planeToLeftManyCircles");
        }

        [NUnit.Framework.Test]
        public virtual void PlaneToLeftLargeSecondCircleTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "planeToLeftLargeSecondCircle");
        }

        [NUnit.Framework.Test]
        public virtual void HalfPlaneToRightManyCirclesTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "halfPlaneToRightManyCircles");
        }

        [NUnit.Framework.Test]
        public virtual void HalfPlaneToRightLargeSecondCircleTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "halfPlaneToRightLargeSecondCircle");
        }

        [NUnit.Framework.Test]
        public virtual void HalfPlaneToLeftManyCirclesTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "halfPlaneToLeftManyCircles");
        }

        [NUnit.Framework.Test]
        public virtual void HalfPlaneToLeftLargeSecondCircleTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "halfPlaneToLeftLargeSecondCircle");
        }

        [NUnit.Framework.Test]
        public virtual void LowerCaseRadialGradientTagTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "lowerCaseRadialGradientTag");
        }

        [NUnit.Framework.Test]
        public virtual void MultipleGradientsAndTargetsTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "multipleGradientsAndTargets");
        }

        [NUnit.Framework.Test]
        public virtual void RectMultipleStopsWithSameOffsetPadTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "rectMultipleStopsWithSameOffsetPad");
        }

        [NUnit.Framework.Test]
        public virtual void RectMultipleStopsWithSameOffsetReflectTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "rectMultipleStopsWithSameOffsetReflect");
        }

        [NUnit.Framework.Test]
        public virtual void RectMultipleStopsWithSameOffsetRepeatTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "rectMultipleStopsWithSameOffsetRepeat");
        }

        [NUnit.Framework.Test]
        public virtual void RectInvalidStopsSequenceTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "rectInvalidStopsSequence");
        }

        [NUnit.Framework.Test]
        public virtual void RectInvalidStopsSequenceWithoutBoundingStopsTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "rectInvalidStopsSequenceWithoutBoundingStops");
        }

        [NUnit.Framework.Test]
        public virtual void UserSpaceOnUseDiffAbsoluteUnitsInGradientTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "userSpaceOnUseDiffAbsoluteUnitsInGradient");
        }

        [NUnit.Framework.Test]
        public virtual void UserSpaceOnUseWithUnitsRelativeToFontTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "userSpaceOnUseWithUnitsRelativeToFont");
        }

        [NUnit.Framework.Test]
        public virtual void UserSpaceOnUseWithUnitsRelativeToViewportTest() {
            // TODO: DEVSIX-3596 update cmp_ after fix ("vh" "vw" "vmin" "vmax" units are not implemented yet)
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "userSpaceOnUseWithUnitsRelativeToViewport");
        }

        [NUnit.Framework.Test]
        public virtual void UserSpaceOnUseDiffRelativeUnitsInGradientTest() {
            // TODO: DEVSIX-3596 update cmp_ after fix ("ch" "vmin"+"vmax"+"vw"+"vh" not implemented yet)
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "userSpaceOnUseDiffRelativeUnitsInGradient");
        }

        [NUnit.Framework.Test]
        public virtual void ObjectBoundingBoxDifferentAbsoluteUnitsInGradientTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "objectBoundingBoxDifferentAbsoluteUnitsInGradient");
        }

        [NUnit.Framework.Test]
        public virtual void ObjectBoundingBoxWithUnitsRelativeToFontTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "objectBoundingBoxWithUnitsRelativeToFont");
        }

        [NUnit.Framework.Test]
        public virtual void ObjectBoundingBoxWithUnitsRelativeToViewportTest() {
            // TODO: DEVSIX-3596 update cmp_ after fix ("vmin", "vmax", "vw", "vh" not implemented yet)
            //  actually the value type should not affect on the objectBoundingBox coordinate, but as
            //  we are not recognize these values as valid relative type,
            //  we get the the resulted coordinate uses defaults
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "objectBoundingBoxWithUnitsRelativeToViewport");
        }

        [NUnit.Framework.Test]
        public virtual void ObjectBoundingBoxDifferentRelativeUnitsInGradientTest() {
            // TODO: DEVSIX-3596 update cmp_ after fix ("ch" "vmin"+"vmax"+"vw"+"vh" not implemented yet)
            //  actually the value type should not affect on the objectBoundingBox coordinate, but as
            //  we are not recognize these values as valid relative type,
            //  we get the the resulted coordinate uses defaults
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "objectBoundingBoxDifferentRelativeUnitsInGradient");
        }

        [NUnit.Framework.Test]
        public virtual void TranslateTransformInGradientWithObjectBoundingBoxUnitsTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "translateTransformInGradientWithObjectBoundingBoxUnits"
                );
        }

        [NUnit.Framework.Test]
        public virtual void MatrixTransformInGradientWithObjectBoundingBoxUnitsTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "matrixTransformInGradientWithObjectBoundingBoxUnits"
                );
        }

        [NUnit.Framework.Test]
        public virtual void SeveralTransformsInGradientWithObjectBoundingBoxUnitsTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "severalTransformsInGradientWithObjectBoundingBoxUnits"
                );
        }

        [NUnit.Framework.Test]
        public virtual void RadialGradXlink3StopsTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "radialGradXlink3Stops");
        }

        [NUnit.Framework.Test]
        public virtual void RadialGradXlinkGradientTransformTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "radialGradXlinkGradientTransform");
        }

        [NUnit.Framework.Test]
        public virtual void RadialGradHrefGradientTransformTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "radialGradHrefGradientTransform");
        }

        [NUnit.Framework.Test]
        public virtual void RadialGradXlinkNegativeOffsetTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "radialGradXlinkNegativeOffset");
        }

        // TODO: DEVSIX-4136 change cmp when gradient opacity is added
        [NUnit.Framework.Test]
        public virtual void RadialGradXlinkNegativeOpacityTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "radialGradXlinkNegativeOpacity");
        }

        // TODO: DEVSIX-4136 change cmp when gradient opacity is added
        [NUnit.Framework.Test]
        public virtual void RadialGradXlinkOpacityTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "radialGradXlinkOpacity");
        }

        // TODO: DEVSIX-4136 change cmp when gradient opacity is added
        [NUnit.Framework.Test]
        public virtual void RadialGradXlinkOpacity2Test() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "radialGradXlinkOpacity2");
        }

        [NUnit.Framework.Test]
        public virtual void RadialGradXlinkSpreadMethodTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "radialGradXlinkSpreadMethod");
        }

        [NUnit.Framework.Test]
        public virtual void RadialGradXlinkSpreadMethod2Test() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "radialGradXlinkSpreadMethod2");
        }

        [NUnit.Framework.Test]
        public virtual void RadialGradXlinkSpreadMethod3Test() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "radialGradXlinkSpreadMethod3");
        }

        [NUnit.Framework.Test]
        public virtual void RadialGradXlinkHreOffsetSwapTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "radialGradXlinkHreOffsetSwap");
        }

        // TODO: DEVSIX-4136 change cmp when gradient opacity is added
        [NUnit.Framework.Test]
        public virtual void RadialGradTransitiveHrefOpacityTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "radialGradTransitiveHrefOpacity");
        }

        // TODO: DEVSIX-4136 change cmp when gradient opacity is added
        [NUnit.Framework.Test]
        public virtual void RadialGradTransitiveHrefNegativeOpacityTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "radialGradTransitiveHrefNegativeOpacity");
        }

        [NUnit.Framework.Test]
        public virtual void RadialGradTransitiveHrefNegativeOffsetTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "radialGradTransitiveHrefNegativeOffset");
        }

        [NUnit.Framework.Test]
        public virtual void RadialGradTransitiveHref3stopsTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "radialGradTransitiveHref3stops");
        }

        [NUnit.Framework.Test]
        public virtual void RadialGradHrefTransitiveSpreadMethodTopLayerTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "radialGradHrefTransitiveSpreadMethodTopLayer");
        }

        [NUnit.Framework.Test]
        public virtual void RadialGradHrefTransitiveSpreadMethodBottomLayerTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "radialGradHrefTransitiveSpreadMethodBottomLayer");
        }

        [NUnit.Framework.Test]
        public virtual void RadialGradHrefTransitiveSpreadMethod3Test() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "radialGradHrefTransitiveSpreadMethod3");
        }

        [NUnit.Framework.Test]
        public virtual void RadialGradHrefTransitiveSpreadMethod2Test() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "radialGradHrefTransitiveSpreadMethod2");
        }

        [NUnit.Framework.Test]
        public virtual void TspanTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "tspan");
        }

        [NUnit.Framework.Test]
        public virtual void TextNestedTSpansTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "textNestedTSpansTest");
        }

        [NUnit.Framework.Test]
        public virtual void TextRotatedTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "textRotatedTest");
        }

        [NUnit.Framework.Test]
        public virtual void TextDxTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "textDxTest");
        }

        [NUnit.Framework.Test]
        public virtual void ChineseTextDxTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "chineseTextDxTest");
        }

        [NUnit.Framework.Test]
        public virtual void ChineseTextDxVerticalTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "chineseTextDxVerticalTest");
        }

        [NUnit.Framework.Test]
        public virtual void TextAnchorTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "textAnchorTest");
        }

        [NUnit.Framework.Test]
        public virtual void TextDyTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "textDyTest");
        }

        [NUnit.Framework.Test]
        public virtual void TextXYOffsetTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "textXYOffset");
        }

        [NUnit.Framework.Test]
        public virtual void TextXOffsetTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "textXOffset");
        }

        [NUnit.Framework.Test]
        public virtual void TextXYDxDyOffsetTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "textXYDxDyOffset");
        }

        [NUnit.Framework.Test]
        public virtual void TextGradientEmUnitsTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "textGradientEmUnits");
        }

        [NUnit.Framework.Test]
        public virtual void TextGradientEmUnitsRelatedTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "textGradientEmUnitsRelated");
        }

        [NUnit.Framework.Test]
        public virtual void TextGradientEmUnitsRelatedNotDefsTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "textGradientEmUnitsRelatedNotDefs");
        }

        [NUnit.Framework.Test]
        public virtual void TextGradientEmUnitsRelatedDefaultTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "textGradientEmUnitsRelatedDefault");
        }

        [NUnit.Framework.Test]
        public virtual void TextGradientExUnitsTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "textGradientExUnits");
        }

        [NUnit.Framework.Test]
        public virtual void TextGradientRemUnitsTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "textGradientRemUnits");
        }

        [NUnit.Framework.Test]
        public virtual void TextGradientRemUnitsNestedSvgTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "textGradientRemUnitsNestedSvg");
        }

        [NUnit.Framework.Test]
        public virtual void RectMultipleStopsWithSameOffsetAndZeroCoordLengthPadTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "rectMultipleStopsWithSameOffsetAndZeroCoordLengthPad"
                );
        }

        [NUnit.Framework.Test]
        public virtual void RectMultipleStopsWithSameOffsetAndZeroCoordLengthReflectTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "rectMultipleStopsWithSameOffsetAndZeroCoordLengthReflect"
                );
        }

        [NUnit.Framework.Test]
        public virtual void RectMultipleStopsWithDiffOffsetAndZeroCoordLengthRepeatTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "rectMultipleStopsWithDiffOffsetAndZeroCoordLengthRepeat"
                );
        }

        [NUnit.Framework.Test]
        public virtual void RectMultipleStopsWithDiffOffsetAndZeroCoordLengthPadTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "rectMultipleStopsWithDiffOffsetAndZeroCoordLengthPad"
                );
        }

        [NUnit.Framework.Test]
        public virtual void RectMultipleStopsWithDiffOffsetAndZeroCoordLengthReflectTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "rectMultipleStopsWithDiffOffsetAndZeroCoordLengthReflect"
                );
        }

        [NUnit.Framework.Test]
        public virtual void RectMultipleStopsWithSameOffsetAndZeroCoordLengthRepeatTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "rectMultipleStopsWithSameOffsetAndZeroCoordLengthRepeat"
                );
        }

        [NUnit.Framework.Test]
        public virtual void RectInvalidCoordinatesMetricsTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "rectInvalidCoordinatesMetrics");
        }

        // TODO: DEVSIX-3596 update cmp_ after fix ("ch" not implemented yet)
        //  actually the value type should not affect on the objectBoundingBox coordinate, but as
        //  we are not recognize these values as valid relative type,
        //  we get the the resulted coordinate uses defaults
        [NUnit.Framework.Test]
        public virtual void UserSpaceOnUseWithChUnitTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "userSpaceOnUseWithChUnit");
        }

        // TODO: DEVSIX-3596 update cmp_ after fix ("ch" not implemented yet)
        //  actually the value type should not affect on the objectBoundingBox coordinate, but as
        //  we are not recognize these values as valid relative type,
        //  we get the the resulted coordinate uses defaults
        [NUnit.Framework.Test]
        public virtual void ObjectBoundingBoxWithChUnitTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "objectBoundingBoxWithChUnit");
        }

        [NUnit.Framework.Test]
        public virtual void RadialGradHrefXYvals1Test() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "radialGradHrefXYvals1");
        }

        [NUnit.Framework.Test]
        public virtual void RadialGradHrefXYvals2Test() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "radialGradHrefXYvals2");
        }

        [NUnit.Framework.Test]
        public virtual void RadialGradHrefXYvals3Test() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "radialGradHrefXYvals3");
        }

        [NUnit.Framework.Test]
        public virtual void LowerCaseGradientUnitsTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "lowerCaseGradientUnits");
        }
    }
}
