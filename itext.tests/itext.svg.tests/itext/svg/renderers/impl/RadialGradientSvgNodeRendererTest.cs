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
        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/svg/renderers/impl/RadialGradientSvgNodeRendererTest/";

        public static readonly String destinationFolder = TestUtil.GetOutputPath() + "/svg/renderers/impl/RadialGradientSvgNodeRendererTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            ITextTest.CreateOrClearDestinationFolder(destinationFolder);
        }

        [NUnit.Framework.Test]
        public virtual void RadialGradientBasicTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "radialGradientBasic");
        }

        [NUnit.Framework.Test]
        public virtual void CircleTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "circle");
        }

        [NUnit.Framework.Test]
        public virtual void EllipseTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "ellipse");
        }

        [NUnit.Framework.Test]
        public virtual void RectTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "rect");
        }

        [NUnit.Framework.Test]
        public virtual void PolygonTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "polygon");
        }

        [NUnit.Framework.Test]
        public virtual void PathTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "path");
        }

        [NUnit.Framework.Test]
        public virtual void TextTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "text");
        }

        [NUnit.Framework.Test]
        public virtual void SpreadMethodPadTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "spreadMethodPad");
        }

        [NUnit.Framework.Test]
        public virtual void SpreadMethodReflectTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "spreadMethodReflect");
        }

        [NUnit.Framework.Test]
        public virtual void SpreadMethodRepeatTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "spreadMethodRepeat");
        }

        [NUnit.Framework.Test]
        [LogMessage(SvgLogMessageConstant.GRADIENT_INVALID_SPREAD_METHOD_LOG, LogLevel = LogLevelConstants.WARN)]
        public virtual void InvalidSpreadMethodTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "invalidSpreadMethod");
        }

        // TODO: DEVSIX-4136 update cmp_ after fix
        //  (opacity is not implemented. No stops defines no color, i.e. transparent color or black with 100% opacity)
        [NUnit.Framework.Test]
        public virtual void NoStopsTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "noStops");
        }

        [NUnit.Framework.Test]
        public virtual void SingleStop0Test() {
            ConvertAndCompare(sourceFolder, destinationFolder, "singleStop0");
        }

        [NUnit.Framework.Test]
        public virtual void SingleStop1Test() {
            ConvertAndCompare(sourceFolder, destinationFolder, "singleStop1");
        }

        [NUnit.Framework.Test]
        public virtual void StopWithoutColorTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "stopWithoutColor");
        }

        [NUnit.Framework.Test]
        public virtual void TransformedTargetTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "transformedTarget");
        }

        [NUnit.Framework.Test]
        public virtual void RectWithGradientTransformTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "rectWithGradientTransform");
        }

        [NUnit.Framework.Test]
        public virtual void RectWithMultipleTransformsTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "rectWithMultipleTransforms");
        }

        [NUnit.Framework.Test]
        public virtual void UserSpaceOnUseAbsoluteCoordinatesTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "userSpaceOnUseAbsoluteCoordinates");
        }

        [NUnit.Framework.Test]
        public virtual void UserSpaceOnUseRelativeUnitsTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "userSpaceOnUseRelativeUnits");
        }

        [NUnit.Framework.Test]
        public virtual void ObjectBoundingBoxAbsoluteCoordinatesTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "objectBoundingBoxAbsoluteCoordinates");
        }

        [NUnit.Framework.Test]
        public virtual void ObjectBoundingBoxRelativeCoordinatesTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "objectBoundingBoxRelativeCoordinates");
        }

        [NUnit.Framework.Test]
        [LogMessage(SvgLogMessageConstant.GRADIENT_INVALID_GRADIENT_UNITS_LOG, LogLevel = LogLevelConstants.WARN)]
        public virtual void InvalidGradientUnitsTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "invalidGradientUnits");
        }

        [NUnit.Framework.Test]
        public virtual void LowerCaseGradientUnitsAttributeTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "lowerCaseGradientUnitsAttribute");
        }

        [NUnit.Framework.Test]
        public virtual void HrefBasicReferenceTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "hrefBasicReference");
        }

        [NUnit.Framework.Test]
        public virtual void TransitiveHrefReferenceTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "transitiveHrefReference");
        }

        [NUnit.Framework.Test]
        public virtual void XlinkHrefReferenceTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "xlinkHrefReference");
        }

        [NUnit.Framework.Test]
        public virtual void HrefOverrideSpreadMethodTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "hrefOverrideSpreadMethod");
        }

        [NUnit.Framework.Test]
        public virtual void HrefOverrideGeometryTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "hrefOverrideGeometry");
        }

        [NUnit.Framework.Test]
        public virtual void ExplicitFocalPointTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "explicitFocalPoint");
        }

        [NUnit.Framework.Test]
        public virtual void NegativeFrTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "negativeFr");
        }

        [NUnit.Framework.Test]
        public virtual void EndRadiusEqualZeroTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "endRadiusEqualZero");
        }

        [NUnit.Framework.Test]
        public virtual void StartRadiusEqualZeroTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "startRadiusEqualZero");
        }

        [NUnit.Framework.Test]
        public virtual void EndRadiusLessThanZeroTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "endRadiusLessThanZero");
        }

        [NUnit.Framework.Test]
        public virtual void StartRadiusLessThanZeroTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "startRadiusLessThanZero");
        }

        [NUnit.Framework.Test]
        public virtual void EqualRadiiTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "equalRadii");
        }

        [NUnit.Framework.Test]
        public virtual void SimilarSizeRadiiTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "similarSizeRadii");
        }

        [NUnit.Framework.Test]
        public virtual void SecondCircleMuchBiggerTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "secondCircleMuchBigger");
        }

        [NUnit.Framework.Test]
        public virtual void PlaneToRightManyCirclesTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "planeToRightManyCircles");
        }

        [NUnit.Framework.Test]
        public virtual void PlaneToRightLargeSecondCircleTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "planeToRightLargeSecondCircle");
        }

        [NUnit.Framework.Test]
        public virtual void PlaneToLeftManyCirclesTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "planeToLeftManyCircles");
        }

        [NUnit.Framework.Test]
        public virtual void PlaneToLeftLargeSecondCircleTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "planeToLeftLargeSecondCircle");
        }

        [NUnit.Framework.Test]
        public virtual void HalfPlaneToRightManyCirclesTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "halfPlaneToRightManyCircles");
        }

        [NUnit.Framework.Test]
        public virtual void HalfPlaneToRightLargeSecondCircleTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "halfPlaneToRightLargeSecondCircle");
        }

        [NUnit.Framework.Test]
        public virtual void HalfPlaneToLeftManyCirclesTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "halfPlaneToLeftManyCircles");
        }

        [NUnit.Framework.Test]
        public virtual void HalfPlaneToLeftLargeSecondCircleTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "halfPlaneToLeftLargeSecondCircle");
        }

        [NUnit.Framework.Test]
        public virtual void LowerCaseRadialGradientTagTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "lowerCaseRadialGradientTag");
        }

        [NUnit.Framework.Test]
        public virtual void MultipleGradientsAndTargetsTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "multipleGradientsAndTargets");
        }

        [NUnit.Framework.Test]
        public virtual void RectMultipleStopsWithSameOffsetPadTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "rectMultipleStopsWithSameOffsetPad");
        }

        [NUnit.Framework.Test]
        public virtual void RectMultipleStopsWithSameOffsetReflectTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "rectMultipleStopsWithSameOffsetReflect");
        }

        [NUnit.Framework.Test]
        public virtual void RectMultipleStopsWithSameOffsetRepeatTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "rectMultipleStopsWithSameOffsetRepeat");
        }

        [NUnit.Framework.Test]
        public virtual void RectInvalidStopsSequenceTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "rectInvalidStopsSequence");
        }

        [NUnit.Framework.Test]
        public virtual void RectInvalidStopsSequenceWithoutBoundingStopsTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "rectInvalidStopsSequenceWithoutBoundingStops");
        }

        [NUnit.Framework.Test]
        public virtual void UserSpaceOnUseDiffAbsoluteUnitsInGradientTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "userSpaceOnUseDiffAbsoluteUnitsInGradient");
        }

        [NUnit.Framework.Test]
        public virtual void UserSpaceOnUseWithUnitsRelativeToFontTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "userSpaceOnUseWithUnitsRelativeToFont");
        }

        [NUnit.Framework.Test]
        public virtual void UserSpaceOnUseWithUnitsRelativeToViewportTest() {
            // TODO: DEVSIX-3596 update cmp_ after fix ("vh" "vw" "vmin" "vmax" units are not implemented yet)
            ConvertAndCompare(sourceFolder, destinationFolder, "userSpaceOnUseWithUnitsRelativeToViewport");
        }

        [NUnit.Framework.Test]
        public virtual void UserSpaceOnUseDiffRelativeUnitsInGradientTest() {
            // TODO: DEVSIX-3596 update cmp_ after fix ("ch" "vmin"+"vmax"+"vw"+"vh" not implemented yet)
            ConvertAndCompare(sourceFolder, destinationFolder, "userSpaceOnUseDiffRelativeUnitsInGradient");
        }

        [NUnit.Framework.Test]
        public virtual void ObjectBoundingBoxDifferentAbsoluteUnitsInGradientTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "objectBoundingBoxDifferentAbsoluteUnitsInGradient");
        }

        [NUnit.Framework.Test]
        public virtual void ObjectBoundingBoxWithUnitsRelativeToFontTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "objectBoundingBoxWithUnitsRelativeToFont");
        }

        [NUnit.Framework.Test]
        public virtual void ObjectBoundingBoxWithUnitsRelativeToViewportTest() {
            // TODO: DEVSIX-3596 update cmp_ after fix ("vmin", "vmax", "vw", "vh" not implemented yet)
            //  actually the value type should not affect on the objectBoundingBox coordinate, but as
            //  we are not recognize these values as valid relative type,
            //  we get the the resulted coordinate uses defaults
            ConvertAndCompare(sourceFolder, destinationFolder, "objectBoundingBoxWithUnitsRelativeToViewport");
        }

        [NUnit.Framework.Test]
        public virtual void ObjectBoundingBoxDifferentRelativeUnitsInGradientTest() {
            // TODO: DEVSIX-3596 update cmp_ after fix ("ch" "vmin"+"vmax"+"vw"+"vh" not implemented yet)
            //  actually the value type should not affect on the objectBoundingBox coordinate, but as
            //  we are not recognize these values as valid relative type,
            //  we get the the resulted coordinate uses defaults
            ConvertAndCompare(sourceFolder, destinationFolder, "objectBoundingBoxDifferentRelativeUnitsInGradient");
        }

        [NUnit.Framework.Test]
        public virtual void TranslateTransformInGradientWithObjectBoundingBoxUnitsTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "translateTransformInGradientWithObjectBoundingBoxUnits"
                );
        }

        [NUnit.Framework.Test]
        public virtual void MatrixTransformInGradientWithObjectBoundingBoxUnitsTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "matrixTransformInGradientWithObjectBoundingBoxUnits");
        }

        [NUnit.Framework.Test]
        public virtual void SeveralTransformsInGradientWithObjectBoundingBoxUnitsTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "severalTransformsInGradientWithObjectBoundingBoxUnits"
                );
        }

        [NUnit.Framework.Test]
        public virtual void RadialGradXlink3StopsTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "radialGradXlink3Stops");
        }

        [NUnit.Framework.Test]
        public virtual void RadialGradXlinkGradientTransformTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "radialGradXlinkGradientTransform");
        }

        [NUnit.Framework.Test]
        public virtual void RadialGradHrefGradientTransformTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "radialGradHrefGradientTransform");
        }

        [NUnit.Framework.Test]
        public virtual void RadialGradXlinkNegativeOffsetTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "radialGradXlinkNegativeOffset");
        }

        // TODO: DEVSIX-4136 change cmp when gradient opacity is added
        [NUnit.Framework.Test]
        public virtual void RadialGradXlinkNegativeOpacityTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "radialGradXlinkNegativeOpacity");
        }

        // TODO: DEVSIX-4136 change cmp when gradient opacity is added
        [NUnit.Framework.Test]
        public virtual void RadialGradXlinkOpacityTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "radialGradXlinkOpacity");
        }

        // TODO: DEVSIX-4136 change cmp when gradient opacity is added
        [NUnit.Framework.Test]
        public virtual void RadialGradXlinkOpacity2Test() {
            ConvertAndCompare(sourceFolder, destinationFolder, "radialGradXlinkOpacity2");
        }

        [NUnit.Framework.Test]
        public virtual void RadialGradXlinkSpreadMethodTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "radialGradXlinkSpreadMethod");
        }

        [NUnit.Framework.Test]
        public virtual void RadialGradXlinkSpreadMethod2Test() {
            ConvertAndCompare(sourceFolder, destinationFolder, "radialGradXlinkSpreadMethod2");
        }

        [NUnit.Framework.Test]
        public virtual void RadialGradXlinkSpreadMethod3Test() {
            ConvertAndCompare(sourceFolder, destinationFolder, "radialGradXlinkSpreadMethod3");
        }

        [NUnit.Framework.Test]
        public virtual void RadialGradXlinkHreOffsetSwapTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "radialGradXlinkHreOffsetSwap");
        }

        // TODO: DEVSIX-4136 change cmp when gradient opacity is added
        [NUnit.Framework.Test]
        public virtual void RadialGradTransitiveHrefOpacityTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "radialGradTransitiveHrefOpacity");
        }

        // TODO: DEVSIX-4136 change cmp when gradient opacity is added
        [NUnit.Framework.Test]
        public virtual void RadialGradTransitiveHrefNegativeOpacityTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "radialGradTransitiveHrefNegativeOpacity");
        }

        [NUnit.Framework.Test]
        public virtual void RadialGradTransitiveHrefNegativeOffsetTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "radialGradTransitiveHrefNegativeOffset");
        }

        [NUnit.Framework.Test]
        public virtual void RadialGradTransitiveHref3stopsTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "radialGradTransitiveHref3stops");
        }

        [NUnit.Framework.Test]
        public virtual void RadialGradHrefTransitiveSpreadMethodTopLayerTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "radialGradHrefTransitiveSpreadMethodTopLayer");
        }

        [NUnit.Framework.Test]
        public virtual void RadialGradHrefTransitiveSpreadMethodBottomLayerTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "radialGradHrefTransitiveSpreadMethodBottomLayer");
        }

        [NUnit.Framework.Test]
        public virtual void RadialGradHrefTransitiveSpreadMethod3Test() {
            ConvertAndCompare(sourceFolder, destinationFolder, "radialGradHrefTransitiveSpreadMethod3");
        }

        [NUnit.Framework.Test]
        public virtual void RadialGradHrefTransitiveSpreadMethod2Test() {
            ConvertAndCompare(sourceFolder, destinationFolder, "radialGradHrefTransitiveSpreadMethod2");
        }

        [NUnit.Framework.Test]
        public virtual void TspanTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "tspan");
        }

        [NUnit.Framework.Test]
        public virtual void TextNestedTSpansTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "textNestedTSpansTest");
        }

        [NUnit.Framework.Test]
        public virtual void TextRotatedTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "textRotatedTest");
        }

        [NUnit.Framework.Test]
        public virtual void TextDxTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "textDxTest");
        }

        [NUnit.Framework.Test]
        public virtual void ChineseTextDxTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "chineseTextDxTest");
        }

        [NUnit.Framework.Test]
        public virtual void ChineseTextDxVerticalTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "chineseTextDxVerticalTest");
        }

        [NUnit.Framework.Test]
        public virtual void TextAnchorTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "textAnchorTest");
        }

        [NUnit.Framework.Test]
        public virtual void TextDyTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "textDyTest");
        }

        [NUnit.Framework.Test]
        public virtual void TextXYOffsetTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "textXYOffset");
        }

        [NUnit.Framework.Test]
        public virtual void TextXOffsetTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "textXOffset");
        }

        [NUnit.Framework.Test]
        public virtual void TextXYDxDyOffsetTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "textXYDxDyOffset");
        }

        [NUnit.Framework.Test]
        public virtual void TextGradientEmUnitsTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "textGradientEmUnits");
        }

        [NUnit.Framework.Test]
        public virtual void TextGradientEmUnitsRelatedTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "textGradientEmUnitsRelated");
        }

        [NUnit.Framework.Test]
        public virtual void TextGradientEmUnitsRelatedNotDefsTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "textGradientEmUnitsRelatedNotDefs");
        }

        [NUnit.Framework.Test]
        public virtual void TextGradientEmUnitsRelatedDefaultTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "textGradientEmUnitsRelatedDefault");
        }

        [NUnit.Framework.Test]
        public virtual void TextGradientExUnitsTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "textGradientExUnits");
        }

        [NUnit.Framework.Test]
        public virtual void TextGradientRemUnitsTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "textGradientRemUnits");
        }

        [NUnit.Framework.Test]
        public virtual void TextGradientRemUnitsNestedSvgTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "textGradientRemUnitsNestedSvg");
        }

        [NUnit.Framework.Test]
        public virtual void RectMultipleStopsWithSameOffsetAndZeroCoordLengthPadTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "rectMultipleStopsWithSameOffsetAndZeroCoordLengthPad");
        }

        [NUnit.Framework.Test]
        public virtual void RectMultipleStopsWithSameOffsetAndZeroCoordLengthReflectTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "rectMultipleStopsWithSameOffsetAndZeroCoordLengthReflect"
                );
        }

        [NUnit.Framework.Test]
        public virtual void RectMultipleStopsWithDiffOffsetAndZeroCoordLengthRepeatTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "rectMultipleStopsWithDiffOffsetAndZeroCoordLengthRepeat"
                );
        }

        [NUnit.Framework.Test]
        public virtual void RectMultipleStopsWithDiffOffsetAndZeroCoordLengthPadTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "rectMultipleStopsWithDiffOffsetAndZeroCoordLengthPad");
        }

        [NUnit.Framework.Test]
        public virtual void RectMultipleStopsWithDiffOffsetAndZeroCoordLengthReflectTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "rectMultipleStopsWithDiffOffsetAndZeroCoordLengthReflect"
                );
        }

        [NUnit.Framework.Test]
        public virtual void RectMultipleStopsWithSameOffsetAndZeroCoordLengthRepeatTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "rectMultipleStopsWithSameOffsetAndZeroCoordLengthRepeat"
                );
        }

        [NUnit.Framework.Test]
        public virtual void RectInvalidCoordinatesMetricsTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "rectInvalidCoordinatesMetrics");
        }

        // TODO: DEVSIX-3596 update cmp_ after fix ("ch" not implemented yet)
        //  actually the value type should not affect on the objectBoundingBox coordinate, but as
        //  we are not recognize these values as valid relative type,
        //  we get the the resulted coordinate uses defaults
        [NUnit.Framework.Test]
        public virtual void UserSpaceOnUseWithChUnitTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "userSpaceOnUseWithChUnit");
        }

        // TODO: DEVSIX-3596 update cmp_ after fix ("ch" not implemented yet)
        //  actually the value type should not affect on the objectBoundingBox coordinate, but as
        //  we are not recognize these values as valid relative type,
        //  we get the the resulted coordinate uses defaults
        [NUnit.Framework.Test]
        public virtual void ObjectBoundingBoxWithChUnitTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "objectBoundingBoxWithChUnit");
        }

        [NUnit.Framework.Test]
        public virtual void RadialGradHrefXYvals1Test() {
            ConvertAndCompare(sourceFolder, destinationFolder, "radialGradHrefXYvals1");
        }

        [NUnit.Framework.Test]
        public virtual void RadialGradHrefXYvals2Test() {
            ConvertAndCompare(sourceFolder, destinationFolder, "radialGradHrefXYvals2");
        }

        [NUnit.Framework.Test]
        public virtual void RadialGradHrefXYvals3Test() {
            ConvertAndCompare(sourceFolder, destinationFolder, "radialGradHrefXYvals3");
        }

        [NUnit.Framework.Test]
        public virtual void LowerCaseGradientUnitsTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "lowerCaseGradientUnits");
        }
    }
}
