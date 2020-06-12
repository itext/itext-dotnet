using System;
using iText.Svg.Exceptions;
using iText.Svg.Renderers;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Svg.Renderers.Impl {
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

        // TODO: DEVSIX-4018 add tests for all other types of path components
        // TODO: DEVSIX-4018 update cmp_ after fix (box for path is not implemented)
        [NUnit.Framework.Test]
        public virtual void PathLinesBasedTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "pathLinesBased");
        }

        // TODO: DEVSIX-4018 update cmp_ after fix (box for path is not implemented)
        [NUnit.Framework.Test]
        public virtual void PathLinesBasedWithMoveTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "pathLinesBasedWithMove");
        }

        // TODO: DEVSIX-4018 update cmp_ after fix (box for path is not implemented)
        [NUnit.Framework.Test]
        public virtual void PathLinesBasedWithTwoFiguresTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "pathLinesBasedWithTwoFigures");
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

        // TODO: DEVSIX-4018 update cmp_ after fix (box for text is not implemented)
        [NUnit.Framework.Test]
        [LogMessage(iText.StyledXmlParser.LogMessageConstant.UNKNOWN_ABSOLUTE_METRIC_LENGTH_PARSED)]
        public virtual void TextTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "text");
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
