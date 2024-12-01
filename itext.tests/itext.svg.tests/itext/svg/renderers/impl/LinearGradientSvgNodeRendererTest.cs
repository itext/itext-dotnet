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
using iText.Kernel.Geom;
using iText.Svg.Logs;
using iText.Svg.Renderers;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Svg.Renderers.Impl {
    [NUnit.Framework.Category("IntegrationTest")]
    public class LinearGradientSvgNodeRendererTest : SvgIntegrationTest {
        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/svg/renderers/impl/LinearGradientSvgNodeRendererTest/";

        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/svg/renderers/impl/LinearGradientSvgNodeRendererTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            ITextTest.CreateOrClearDestinationFolder(destinationFolder);
        }

        // TODO: DEVSIX-3932 update cmp_ after fix
        [NUnit.Framework.Test]
        public virtual void CircleTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "circle");
        }

        // TODO: DEVSIX-3932 update cmp_ after fix
        [NUnit.Framework.Test]
        public virtual void EllipseTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "ellipse");
        }

        [NUnit.Framework.Test]
        public virtual void LineTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "line");
        }

        [NUnit.Framework.Test]
        public virtual void PathLinesBasedTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "pathLinesBased");
        }

        [NUnit.Framework.Test]
        public virtual void PathLinesBasedTransformedTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "pathLinesBasedTransformed");
        }

        [NUnit.Framework.Test]
        public virtual void PathLinesBasedWithMoveTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "pathLinesBasedWithMove");
        }

        [NUnit.Framework.Test]
        public virtual void PathLinesBasedWithTwoFiguresTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "pathLinesBasedWithTwoFigures");
        }

        [NUnit.Framework.Test]
        public virtual void CubicBezierTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "cubicBezier");
        }

        [NUnit.Framework.Test]
        public virtual void CubicBezier2Test() {
            ConvertAndCompare(sourceFolder, destinationFolder, "cubicBezier2");
        }

        [NUnit.Framework.Test]
        public virtual void CubicBezier3Test() {
            ConvertAndCompare(sourceFolder, destinationFolder, "cubicBezier3");
        }

        [NUnit.Framework.Test]
        public virtual void CubicBezier4Test() {
            ConvertAndCompare(sourceFolder, destinationFolder, "cubicBezier4");
        }

        [NUnit.Framework.Test]
        public virtual void CubicBezierZeroDiscriminantTest() {
            // See CurveTo#calculateTValues to see which discriminant is mentioned.
            ConvertAndCompare(sourceFolder, destinationFolder, "cubicBezierZeroDiscriminant");
        }

        [NUnit.Framework.Test]
        public virtual void CubicBezierNegativeDiscriminantTest() {
            // See CurveTo#calculateTValues to see which discriminant is mentioned.
            ConvertAndCompare(sourceFolder, destinationFolder, "cubicBezierNegativeDiscriminant");
        }

        [NUnit.Framework.Test]
        public virtual void CubicBezierInsideOtherCubicBezierTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "cubicBezierInsideOtherCubicBezier");
        }

        [NUnit.Framework.Test]
        public virtual void SmoothCubicBezierWithAbsoluteCoordinatesTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "smoothCubicBezierWithAbsoluteCoordinates");
        }

        [NUnit.Framework.Test]
        public virtual void SmoothCubicBezierWithRelativeCoordinatesTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "smoothCubicBezierWithRelativeCoordinates");
        }

        [NUnit.Framework.Test]
        public virtual void SmoothCubicBezierRelativeAndAbsoluteCoordWithMoveTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "smoothCubicBezierRelativeAndAbsoluteCoordWithMove");
        }

        [NUnit.Framework.Test]
        public virtual void SmoothCubicBezierRelativeAndAbsoluteCoordNoZOperatorTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "smoothCubicBezierRelativeAndAbsoluteCoordNoZOperator");
        }

        [NUnit.Framework.Test]
        public virtual void QuadraticBezierTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "quadraticBezier");
        }

        [NUnit.Framework.Test]
        public virtual void QuadraticBezier2Test() {
            ConvertAndCompare(sourceFolder, destinationFolder, "quadraticBezier2");
        }

        [NUnit.Framework.Test]
        public virtual void QuadraticBezier3Test() {
            ConvertAndCompare(sourceFolder, destinationFolder, "quadraticBezier3");
        }

        [NUnit.Framework.Test]
        public virtual void QuadraticBezierInsideOtherQuadraticBezierTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "quadraticBezierInsideOtherQuadraticBezier");
        }

        [NUnit.Framework.Test]
        public virtual void SmoothQuadraticBezierWithAbsoluteCoordinatesTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "smoothQuadraticBezierWithAbsoluteCoordinates");
        }

        [NUnit.Framework.Test]
        public virtual void SmoothQuadraticBezierWithRelativeCoordinatesTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "smoothQuadraticBezierWithRelativeCoordinates");
        }

        [NUnit.Framework.Test]
        public virtual void SmoothQuadraticBezierAbsoluteAndRelativeCoordWithMoveTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "smoothQuadraticBezierAbsoluteAndRelativeCoordWithMove"
                );
        }

        [NUnit.Framework.Test]
        public virtual void SmoothQuadraticBezierRelativeAndAbsoluteCoordNoZOperatorTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "smoothQuadraticBezierRelativeAndAbsoluteCoordNoZOperator"
                );
        }

        [NUnit.Framework.Test]
        public virtual void EllipticalArcsTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "ellipticalArcs");
        }

        [NUnit.Framework.Test]
        public virtual void EllipticalArcsNegativeRxRyTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "ellipticalArcsNegativeRxRy");
        }

        [NUnit.Framework.Test]
        public virtual void EllipticalArcZeroRxRyTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "ellipticalArcZeroRxRy");
        }

        [NUnit.Framework.Test]
        public virtual void EllipticalArcsWithPhiTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "ellipticalArcsWithPhi", PageSize.A3.Rotate());
        }

        [NUnit.Framework.Test]
        public virtual void EllipticalArcsWithPhi0Test() {
            ConvertAndCompare(sourceFolder, destinationFolder, "ellipticalArcsWithPhi0");
        }

        [NUnit.Framework.Test]
        public virtual void EllipticalArcsWithPhi90Test() {
            ConvertAndCompare(sourceFolder, destinationFolder, "ellipticalArcsWithPhi90");
        }

        [NUnit.Framework.Test]
        public virtual void EllipticalArcsWithPhi180Test() {
            ConvertAndCompare(sourceFolder, destinationFolder, "ellipticalArcsWithPhi180");
        }

        [NUnit.Framework.Test]
        public virtual void EllipticalArcsWithPhi270Test() {
            ConvertAndCompare(sourceFolder, destinationFolder, "ellipticalArcsWithPhi270");
        }

        [NUnit.Framework.Test]
        public virtual void EllipticalArcsWithPhiRelativeTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "ellipticalArcsWithPhiRelative");
        }

        [NUnit.Framework.Test]
        public virtual void EllipticalArcsWithPhiAbsoluteTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "ellipticalArcsWithPhiAbsolute");
        }

        [NUnit.Framework.Test]
        public virtual void EllipticalArcsRelativeCoordinatesTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "ellipticalArcsRelativeCoordinates");
        }

        [NUnit.Framework.Test]
        public virtual void ArcInsideOtherEllipticalArcTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "arcInsideOtherEllipticalArc");
        }

        // TODO: DEVSIX-3932 update cmp_ after fix
        [NUnit.Framework.Test]
        public virtual void PolygonTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "polygon");
        }

        [NUnit.Framework.Test]
        public virtual void PolylineTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "polyline");
        }

        [NUnit.Framework.Test]
        public virtual void RectTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "rect");
        }

        [NUnit.Framework.Test]
        [LogMessage(SvgLogMessageConstant.GRADIENT_INVALID_SPREAD_METHOD_LOG, LogLevel = LogLevelConstants.WARN)]
        public virtual void RectWithInvalidSpreadMethodValueTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "rectWithInvalidSpreadMethodValue");
        }

        [NUnit.Framework.Test]
        public virtual void RectsWithFallBackColorsTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "rectsWithFallBackColors");
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

        // TODO: DEVSIX-4136 update cmp_ after fix
        //  (opacity is not implemented. No stops defines no color, i.e. transparent color or black with 100% opacity)
        [NUnit.Framework.Test]
        public virtual void RectNoStopsTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "rectNoStops");
        }

        [NUnit.Framework.Test]
        public virtual void RectSingle0StopTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "rectSingle0Stop");
        }

        [NUnit.Framework.Test]
        public virtual void RectSingle1StopTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "rectSingle1Stop");
        }

        [NUnit.Framework.Test]
        public virtual void RectStopWithoutColorTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "rectStopWithoutColor");
        }

        [NUnit.Framework.Test]
        public virtual void RectTransformedTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "rectTransformed");
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
        public virtual void TextTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "text");
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
        public virtual void TextDyTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "textDyTest");
        }

        [NUnit.Framework.Test]
        public virtual void TextXYOffset() {
            ConvertAndCompare(sourceFolder, destinationFolder, "textXYOffset");
        }

        [NUnit.Framework.Test]
        public virtual void TextXOffset() {
            ConvertAndCompare(sourceFolder, destinationFolder, "textXOffset");
        }

        [NUnit.Framework.Test]
        public virtual void TextXYDxDyOffset() {
            ConvertAndCompare(sourceFolder, destinationFolder, "textXYDxDyOffset");
        }

        [NUnit.Framework.Test]
        public virtual void TextGradientEmUnits() {
            ConvertAndCompare(sourceFolder, destinationFolder, "textGradientEmUnits");
        }

        [NUnit.Framework.Test]
        public virtual void TextGradientEmUnitsRelated() {
            ConvertAndCompare(sourceFolder, destinationFolder, "textGradientEmUnitsRelated");
        }

        [NUnit.Framework.Test]
        public virtual void TextGradientEmUnitsRelatedNotDefs() {
            ConvertAndCompare(sourceFolder, destinationFolder, "textGradientEmUnitsRelatedNotDefs");
        }

        [NUnit.Framework.Test]
        public virtual void TextGradientEmUnitsRelatedDefault() {
            ConvertAndCompare(sourceFolder, destinationFolder, "textGradientEmUnitsRelatedDefault");
        }

        [NUnit.Framework.Test]
        public virtual void TextGradientExUnits() {
            ConvertAndCompare(sourceFolder, destinationFolder, "textGradientExUnits");
        }

        [NUnit.Framework.Test]
        public virtual void TextGradientRemUnits() {
            ConvertAndCompare(sourceFolder, destinationFolder, "textGradientRemUnits");
        }

        [NUnit.Framework.Test]
        public virtual void TextGradientRemUnitsNestedSvg() {
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
        public virtual void RectMultipleStopsWithSameOffsetAndZeroCoordLengthRepeatTest() {
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
        public virtual void RectMultipleStopsWithDiffOffsetAndZeroCoordLengthRepeatTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "rectMultipleStopsWithSameOffsetAndZeroCoordLengthRepeat"
                );
        }

        [NUnit.Framework.Test]
        public virtual void RectInvalidStopsSequenceTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "rectInvalidStopsSequence");
        }

        [NUnit.Framework.Test]
        public virtual void RectInvalidCoordinatesMetricsTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "rectInvalidCoordinatesMetrics");
        }

        [NUnit.Framework.Test]
        public virtual void RectInvalidStopsSequenceWithoutBoundingStopsTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "rectInvalidStopsSequenceWithoutBoundingStops");
        }

        [NUnit.Framework.Test]
        public virtual void UserSpaceOnUseWithAbsoluteCoordinatesTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "userSpaceOnUseWithAbsoluteCoordinates");
        }

        [NUnit.Framework.Test]
        public virtual void UserSpaceOnUseDiffAbsoluteUnitsInGradientTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "userSpaceOnUseDiffAbsoluteUnitsInGradient");
        }

        [NUnit.Framework.Test]
        public virtual void UserSpaceOnUseWithChUnitTest() {
            // TODO: DEVSIX-3596 update cmp_ after fix ("ch" unit is not implemented yet)
            ConvertAndCompare(sourceFolder, destinationFolder, "userSpaceOnUseWithChUnit");
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
        public virtual void ObjectBoundingBoxWithAbsoluteCoordinatesTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "objectBoundingBoxWithAbsoluteCoordinates");
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
        public virtual void ObjectBoundingBoxWithChUnitTest() {
            // TODO: DEVSIX-3596 update cmp_ after fix ("ch" not implemented yet)
            //  actually the value type should not affect on the objectBoundingBox coordinate, but as
            //  we are not recognize these values as valid relative type,
            //  we get the the resulted coordinate uses defaults
            ConvertAndCompare(sourceFolder, destinationFolder, "objectBoundingBoxWithChUnit");
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
    }
}
