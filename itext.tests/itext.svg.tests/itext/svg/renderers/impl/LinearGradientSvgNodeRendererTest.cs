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
using iText.Kernel.Geom;
using iText.Svg.Logs;
using iText.Svg.Renderers;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Svg.Renderers.Impl {
    [NUnit.Framework.Category("IntegrationTest")]
    public class LinearGradientSvgNodeRendererTest : SvgIntegrationTest {
        private static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/svg/renderers/impl/LinearGradientSvgNodeRendererTest/";

        private static readonly String DESTINATION_FOLDER = TestUtil.GetOutputPath() + "/svg/renderers/impl/LinearGradientSvgNodeRendererTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            ITextTest.CreateOrClearDestinationFolder(DESTINATION_FOLDER);
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
        public virtual void LineTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "line");
        }

        [NUnit.Framework.Test]
        public virtual void PathLinesBasedTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "pathLinesBased");
        }

        [NUnit.Framework.Test]
        public virtual void PathLinesBasedTransformedTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "pathLinesBasedTransformed");
        }

        [NUnit.Framework.Test]
        public virtual void PathLinesBasedWithMoveTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "pathLinesBasedWithMove");
        }

        [NUnit.Framework.Test]
        public virtual void PathLinesBasedWithTwoFiguresTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "pathLinesBasedWithTwoFigures");
        }

        [NUnit.Framework.Test]
        public virtual void CubicBezierTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "cubicBezier");
        }

        [NUnit.Framework.Test]
        public virtual void CubicBezier2Test() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "cubicBezier2");
        }

        [NUnit.Framework.Test]
        public virtual void CubicBezier3Test() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "cubicBezier3");
        }

        [NUnit.Framework.Test]
        public virtual void CubicBezier4Test() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "cubicBezier4");
        }

        [NUnit.Framework.Test]
        public virtual void CubicBezierZeroDiscriminantTest() {
            // See CurveTo#calculateTValues to see which discriminant is mentioned.
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "cubicBezierZeroDiscriminant");
        }

        [NUnit.Framework.Test]
        public virtual void CubicBezierNegativeDiscriminantTest() {
            // See CurveTo#calculateTValues to see which discriminant is mentioned.
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "cubicBezierNegativeDiscriminant");
        }

        [NUnit.Framework.Test]
        public virtual void CubicBezierInsideOtherCubicBezierTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "cubicBezierInsideOtherCubicBezier");
        }

        [NUnit.Framework.Test]
        public virtual void SmoothCubicBezierWithAbsoluteCoordinatesTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "smoothCubicBezierWithAbsoluteCoordinates");
        }

        [NUnit.Framework.Test]
        public virtual void SmoothCubicBezierWithRelativeCoordinatesTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "smoothCubicBezierWithRelativeCoordinates");
        }

        [NUnit.Framework.Test]
        public virtual void SmoothCubicBezierRelativeAndAbsoluteCoordWithMoveTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "smoothCubicBezierRelativeAndAbsoluteCoordWithMove");
        }

        [NUnit.Framework.Test]
        public virtual void SmoothCubicBezierRelativeAndAbsoluteCoordNoZOperatorTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "smoothCubicBezierRelativeAndAbsoluteCoordNoZOperator"
                );
        }

        [NUnit.Framework.Test]
        public virtual void QuadraticBezierTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "quadraticBezier");
        }

        [NUnit.Framework.Test]
        public virtual void QuadraticBezier2Test() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "quadraticBezier2");
        }

        [NUnit.Framework.Test]
        public virtual void QuadraticBezier3Test() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "quadraticBezier3");
        }

        [NUnit.Framework.Test]
        public virtual void QuadraticBezierInsideOtherQuadraticBezierTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "quadraticBezierInsideOtherQuadraticBezier");
        }

        [NUnit.Framework.Test]
        public virtual void SmoothQuadraticBezierWithAbsoluteCoordinatesTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "smoothQuadraticBezierWithAbsoluteCoordinates");
        }

        [NUnit.Framework.Test]
        public virtual void SmoothQuadraticBezierWithRelativeCoordinatesTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "smoothQuadraticBezierWithRelativeCoordinates");
        }

        [NUnit.Framework.Test]
        public virtual void SmoothQuadraticBezierAbsoluteAndRelativeCoordWithMoveTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "smoothQuadraticBezierAbsoluteAndRelativeCoordWithMove"
                );
        }

        [NUnit.Framework.Test]
        public virtual void SmoothQuadraticBezierRelativeAndAbsoluteCoordNoZOperatorTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "smoothQuadraticBezierRelativeAndAbsoluteCoordNoZOperator"
                );
        }

        [NUnit.Framework.Test]
        public virtual void EllipticalArcsTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "ellipticalArcs");
        }

        [NUnit.Framework.Test]
        public virtual void EllipticalArcsNegativeRxRyTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "ellipticalArcsNegativeRxRy");
        }

        [NUnit.Framework.Test]
        public virtual void EllipticalArcZeroRxRyTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "ellipticalArcZeroRxRy");
        }

        [NUnit.Framework.Test]
        public virtual void EllipticalArcsWithPhiTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "ellipticalArcsWithPhi", PageSize.A3.Rotate());
        }

        [NUnit.Framework.Test]
        public virtual void EllipticalArcsWithPhi0Test() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "ellipticalArcsWithPhi0");
        }

        [NUnit.Framework.Test]
        public virtual void EllipticalArcsWithPhi90Test() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "ellipticalArcsWithPhi90");
        }

        [NUnit.Framework.Test]
        public virtual void EllipticalArcsWithPhi180Test() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "ellipticalArcsWithPhi180");
        }

        [NUnit.Framework.Test]
        public virtual void EllipticalArcsWithPhi270Test() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "ellipticalArcsWithPhi270");
        }

        [NUnit.Framework.Test]
        public virtual void EllipticalArcsWithPhiRelativeTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "ellipticalArcsWithPhiRelative");
        }

        [NUnit.Framework.Test]
        public virtual void EllipticalArcsWithPhiAbsoluteTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "ellipticalArcsWithPhiAbsolute");
        }

        [NUnit.Framework.Test]
        public virtual void EllipticalArcsRelativeCoordinatesTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "ellipticalArcsRelativeCoordinates");
        }

        [NUnit.Framework.Test]
        public virtual void ArcInsideOtherEllipticalArcTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "arcInsideOtherEllipticalArc");
        }

        [NUnit.Framework.Test]
        public virtual void PolygonTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "polygon");
        }

        [NUnit.Framework.Test]
        public virtual void PolylineTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "polyline");
        }

        [NUnit.Framework.Test]
        public virtual void RectTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "rect");
        }

        [NUnit.Framework.Test]
        [LogMessage(SvgLogMessageConstant.GRADIENT_INVALID_SPREAD_METHOD_LOG, LogLevel = LogLevelConstants.WARN)]
        public virtual void RectWithInvalidSpreadMethodValueTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "rectWithInvalidSpreadMethodValue");
        }

        [NUnit.Framework.Test]
        public virtual void RectsWithFallBackColorsTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "rectsWithFallBackColors");
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

        // TODO: DEVSIX-4136 update cmp_ after fix
        //  (opacity is not implemented. No stops defines no color, i.e. transparent color or black with 100% opacity)
        [NUnit.Framework.Test]
        public virtual void RectNoStopsTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "rectNoStops");
        }

        [NUnit.Framework.Test]
        public virtual void RectSingle0StopTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "rectSingle0Stop");
        }

        [NUnit.Framework.Test]
        public virtual void RectSingle1StopTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "rectSingle1Stop");
        }

        [NUnit.Framework.Test]
        public virtual void RectStopWithoutColorTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "rectStopWithoutColor");
        }

        [NUnit.Framework.Test]
        public virtual void RectTransformedTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "rectTransformed");
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
        public virtual void TextTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "text");
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
        public virtual void TextXYOffset() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "textXYOffset");
        }

        [NUnit.Framework.Test]
        public virtual void TextXOffset() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "textXOffset");
        }

        [NUnit.Framework.Test]
        public virtual void TextXYDxDyOffset() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "textXYDxDyOffset");
        }

        [NUnit.Framework.Test]
        public virtual void TextGradientEmUnits() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "textGradientEmUnits");
        }

        [NUnit.Framework.Test]
        public virtual void TextGradientEmUnitsRelated() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "textGradientEmUnitsRelated");
        }

        [NUnit.Framework.Test]
        public virtual void TextGradientEmUnitsRelatedNotDefs() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "textGradientEmUnitsRelatedNotDefs");
        }

        [NUnit.Framework.Test]
        public virtual void TextGradientEmUnitsRelatedDefault() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "textGradientEmUnitsRelatedDefault");
        }

        [NUnit.Framework.Test]
        public virtual void TextGradientExUnits() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "textGradientExUnits");
        }

        [NUnit.Framework.Test]
        public virtual void TextGradientRemUnits() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "textGradientRemUnits");
        }

        [NUnit.Framework.Test]
        public virtual void TextGradientRemUnitsNestedSvg() {
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
        public virtual void RectMultipleStopsWithSameOffsetAndZeroCoordLengthRepeatTest() {
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
        public virtual void RectMultipleStopsWithDiffOffsetAndZeroCoordLengthRepeatTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "rectMultipleStopsWithSameOffsetAndZeroCoordLengthRepeat"
                );
        }

        [NUnit.Framework.Test]
        public virtual void RectInvalidStopsSequenceTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "rectInvalidStopsSequence");
        }

        [NUnit.Framework.Test]
        public virtual void RectInvalidCoordinatesMetricsTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "rectInvalidCoordinatesMetrics");
        }

        [NUnit.Framework.Test]
        public virtual void RectInvalidStopsSequenceWithoutBoundingStopsTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "rectInvalidStopsSequenceWithoutBoundingStops");
        }

        [NUnit.Framework.Test]
        public virtual void UserSpaceOnUseWithAbsoluteCoordinatesTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "userSpaceOnUseWithAbsoluteCoordinates");
        }

        [NUnit.Framework.Test]
        public virtual void UserSpaceOnUseDiffAbsoluteUnitsInGradientTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "userSpaceOnUseDiffAbsoluteUnitsInGradient");
        }

        [NUnit.Framework.Test]
        public virtual void UserSpaceOnUseWithChUnitTest() {
            // TODO: DEVSIX-3596 update cmp_ after fix ("ch" unit is not implemented yet)
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "userSpaceOnUseWithChUnit");
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
        public virtual void ObjectBoundingBoxWithAbsoluteCoordinatesTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "objectBoundingBoxWithAbsoluteCoordinates");
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
        public virtual void ObjectBoundingBoxWithChUnitTest() {
            // TODO: DEVSIX-3596 update cmp_ after fix ("ch" not implemented yet)
            //  actually the value type should not affect on the objectBoundingBox coordinate, but as
            //  we are not recognize these values as valid relative type,
            //  we get the the resulted coordinate uses defaults
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "objectBoundingBoxWithChUnit");
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
        public virtual void HrefBasicReferenceTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "hrefBasicReference");
        }

        [NUnit.Framework.Test]
        public virtual void TransitiveHrefBasicReferenceTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "transitiveHrefBasicReference");
        }

        [NUnit.Framework.Test]
        public virtual void LinearGradXlinkTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "linearGradHref");
        }

        [NUnit.Framework.Test]
        public virtual void LinearGradXlink3StopsTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "linearGradHref3Stops");
        }

        [NUnit.Framework.Test]
        public virtual void LinearGradXlinkGradientTransformTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "linearGradHrefGradientTransform");
        }

        [NUnit.Framework.Test]
        public virtual void LinearGradXlinkNegativeOffsetTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "linearGradHrefNegativeOffset");
        }

        [NUnit.Framework.Test]
        public virtual void LinearGradXlinkNegativeOpacityTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "linearGradHrefNegativeOpacity");
        }

        [NUnit.Framework.Test]
        public virtual void LinearGradXlinkOpacityTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "linearGradHrefOpacity");
        }

        [NUnit.Framework.Test]
        public virtual void LinearGradXlinkOpacity2Test() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "linearGradHrefOpacity2");
        }

        [NUnit.Framework.Test]
        public virtual void LinearGradXlinkSpreadMethodTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "linearGradHrefSpreadMethod1");
        }

        [NUnit.Framework.Test]
        public virtual void LinearGradXlinkSpreadMethod2Test() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "linearGradHrefSpreadMethod2");
        }

        [NUnit.Framework.Test]
        public virtual void LinearGradXlinkSpreadMethod3Test() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "linearGradHrefSpreadMethod3");
        }

        [NUnit.Framework.Test]
        public virtual void LinearGradXlinkHrefXYvalsTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "linearGradHrefXYvals1");
        }

        [NUnit.Framework.Test]
        public virtual void LinearGradXlinkHrefXYvals2Test() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "linearGradHrefXYvals2");
        }

        [NUnit.Framework.Test]
        public virtual void LinearGradXlinkHrefXYvals3Test() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "linearGradHrefXYvals3");
        }

        [NUnit.Framework.Test]
        public virtual void LinearGradXlinkHreOffsetSwapTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "linearGradHreOffsetSwap");
        }

        [NUnit.Framework.Test]
        public virtual void LinearGradTransitiveHrefOpacityTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "linearGradTransitiveHrefOpacity");
        }

        [NUnit.Framework.Test]
        public virtual void LinearGradTransitiveHrefNegativeOpacityTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "linearGradTransitiveHrefNegativeOpacity");
        }

        [NUnit.Framework.Test]
        public virtual void LinearGradTransitiveHrefNegativeOffsetTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "linearGradTransitiveHrefNegativeOffset");
        }

        [NUnit.Framework.Test]
        public virtual void LinearGradTransitiveHref3stopsTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "linearGradTransitiveHref3stops");
        }

        [NUnit.Framework.Test]
        public virtual void LinearGradHrefTransitiveSpreadMethodTopLayerTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "linearGradHrefTransitiveSpreadMethodTopLayer");
        }

        [NUnit.Framework.Test]
        public virtual void LinearGradHrefTransitiveSpreadMethodBottomLayerTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "linearGradHrefTransitiveSpreadMethodBottomLayer");
        }

        [NUnit.Framework.Test]
        public virtual void LinearGradHrefTransitiveSpreadMethod3Test() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "linearGradHrefTransitiveSpreadMethod3");
        }

        [NUnit.Framework.Test]
        public virtual void LinearGradHrefTransitiveSpreadMethod2Test() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "linearGradHrefTransitiveSpreadMethod2");
        }

        [NUnit.Framework.Test]
        public virtual void LowerCaseGradientUnitsTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "lowerCaseGradientUnits");
        }
    }
}
