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

namespace iText.Svg.Renderers.Impl {
    [NUnit.Framework.Category("IntegrationTest")]
    public class MarkerSvgNodeRendererIntegrationTest : SvgIntegrationTest {
        private static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/svg/renderers/impl/MarkerSvgNodeRendererIntegrationTest/";

        private static readonly String DESTINATION_FOLDER = TestUtil.GetOutputPath() + "/svg/renderers/impl/MarkerSvgNodeRendererIntegrationTest/";

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
        public virtual void MarkerPathAutoOrientTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "markerPathAutoOrient");
        }

        [NUnit.Framework.Test]
        public virtual void MarkerPathAngleOrientTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "markerPathAngleOrient");
        }

        [NUnit.Framework.Test]
        public virtual void MarkerPathRefXRefYNoAspectRatioPreservationTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "markerPathRefXRefYNoAspectRatioPreservation");
        }

        [NUnit.Framework.Test]
        public virtual void MarkerPathViewboxRightOrientTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "markerPathViewboxRightOrient");
        }

        [NUnit.Framework.Test]
        public virtual void MarkerPathViewboxRightOrientNoAspectRatioPreservationTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "markerPathViewboxRightOrientNoAspectRatioPreservation"
                );
        }

        [NUnit.Framework.Test]
        public virtual void MarkerPathViewboxLeftOrientTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "markerPathViewboxLeftOrient");
        }

        [NUnit.Framework.Test]
        public virtual void MarkerPathViewboxUpOrientTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "markerPathViewboxUpOrient");
        }

        [NUnit.Framework.Test]
        public virtual void MarkerPathViewboxDownOrientTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "markerPathViewboxDownOrient");
        }

        [NUnit.Framework.Test]
        public virtual void MarkerPathViewboxAngledOrientTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "markerPathViewboxAngledOrient");
        }

        [NUnit.Framework.Test]
        public virtual void MarkerPathPreserveAspectRatioTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "markerPathPreserveAspectRatio");
        }

        // Markers in different elements
        [NUnit.Framework.Test]
        public virtual void MarkerTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "marker");
        }

        [NUnit.Framework.Test]
        public virtual void MarkerInLineElementTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "markerInLineElement");
        }

        [NUnit.Framework.Test]
        public virtual void MarkerInPolylineElementTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "markerInPolylineElement");
        }

        [NUnit.Framework.Test]
        public virtual void MarkerInPolygonElementTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "markerInPolygonElement");
        }

        [NUnit.Framework.Test]
        public virtual void MarkerInPolygonElementWithComplexAngleTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "markerInPolygonElementWithComplexAngle");
        }

        [NUnit.Framework.Test]
        public virtual void MarkerShorthandWithFillAndStrokeTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "markerShorthandWithFillAndStroke");
        }

        [NUnit.Framework.Test]
        public virtual void MarkerInPathTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "markerInPath");
        }

        [NUnit.Framework.Test]
        public virtual void MarkerInPathWithAngledMarkerTest() {
            // TODO: update when DEVSIX-8749 will be closed
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "markerInPathWithAngledMarker");
        }

        [NUnit.Framework.Test]
        public virtual void MarkerShorthandInPolylineTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "markerShorthandInPolyline");
        }

        [NUnit.Framework.Test]
        public virtual void MarkerShorthandInheritanceTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "markerShorthandInheritance");
        }

        [NUnit.Framework.Test]
        public virtual void MarkerShorthandTagInheritanceTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "markerShorthandTagInheritance");
        }

        [NUnit.Framework.Test]
        public virtual void MarkerUnitsTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "markerUnits");
        }

        [NUnit.Framework.Test]
        public virtual void MarkerRefXYTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "markerRefXY");
        }

        // orient attribute tests
        [NUnit.Framework.Test]
        public virtual void MarkerOrientTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "markerOrient");
        }

        [NUnit.Framework.Test]
        public virtual void OrientAutoLineInDifferentPositionTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "orientAutoLineInDifferentPosition");
        }

        [NUnit.Framework.Test]
        public virtual void OrientAutoPolylineInDifferentPositionTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "orientAutoPolylineInDifferentPosition");
        }

        [NUnit.Framework.Test]
        public virtual void OrientAutoPolygonInDifferentPositionTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "orientAutoPolygonInDifferentPosition");
        }

        [NUnit.Framework.Test]
        public virtual void MarkerUnitsStrokeWidthWhenParentStrokeWidthIsFontRelativeValuesTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "parentStrokeWidthIsFontRelativeValues");
        }

        [NUnit.Framework.Test]
        public virtual void MarkerUnitsStrokeWidthWhenParentStrokeWidthIsMetricValuesTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "parentStrokeWidthIsMetricValues");
        }

        [NUnit.Framework.Test]
        public virtual void MarkerUnitsStrokeWidthWhenParentStrokeWidthIsPercentageValuesTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "parentStrokeWidthIsPercentageValues");
        }

        [NUnit.Framework.Test]
        public virtual void MarkerDefaultValuesTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "markerDefaultValues");
        }

        // Style inheritance
        [NUnit.Framework.Test]
        public virtual void MarkerInheritFillAttributeTest0() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "markerInheritFillAttribute0");
        }

        [NUnit.Framework.Test]
        public virtual void MarkerInheritFillAttributeTest1() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "markerInheritFillAttribute1");
        }

        [NUnit.Framework.Test]
        public virtual void MarkerInheritFillAttributeTest2() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "markerInheritFillAttribute2");
        }

        [NUnit.Framework.Test]
        public virtual void MarkerInheritFillAttributeNestedMarkerTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "markerInheritFillAttributeNestedMarker");
        }

        [NUnit.Framework.Test]
        public virtual void FontRelativeValueInRefXTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "fontRelativeValueInRefX");
        }

        [NUnit.Framework.Test]
        public virtual void FontRelativeValueInRefXDefaultTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "fontRelativeValueInRefXDefault");
        }

        [NUnit.Framework.Test]
        public virtual void MarkerAspectRatioTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "markerAspectRatio");
        }

        // Overflow attribute
        [NUnit.Framework.Test]
        public virtual void MarkerOverflowVisibleIncreaseViewBoxScaleRootElementTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "markerOverflowVisibleIncreaseViewBoxScaleRootElement"
                );
        }

        [NUnit.Framework.Test]
        public virtual void MarkerOverflowVisibleIncreaseViewBoxScaleSvgElementTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "markerOverflowVisibleIncreaseViewBoxScaleSvgElement"
                );
        }

        [NUnit.Framework.Test]
        public virtual void MarkerOverflowVisibleTransformScaleTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "markerOverflowVisibleTransformScale");
        }

        [NUnit.Framework.Test]
        public virtual void MarkerOverflowVisibleTransformTranslateTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "markerOverflowVisibleTransformTranslate");
        }

        [NUnit.Framework.Test]
        public virtual void MarkerOverflowVisibleNestedSvgViewBoxesTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "markerOverflowVisibleNestedSvgViewBoxes");
        }

        [NUnit.Framework.Test]
        public virtual void SquareInNotSquareViewBoxTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "squareInNotSquareViewBox");
        }

        [NUnit.Framework.Test]
        public virtual void MarkerOverflowVisibleNestedSvgViewBoxes2Test() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "markerOverflowVisibleNestedSvgViewBoxes2");
        }

        [NUnit.Framework.Test]
        public virtual void MarkerOverflowVisibleTransformRotateTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "markerOverflowVisibleTransformRotate");
        }

        [NUnit.Framework.Test]
        [LogMessage("markerWidth has zero value. Marker will not be rendered.")]
        [LogMessage("markerHeight has zero value. Marker will not be rendered.")]
        [LogMessage("markerWidth has negative value. Marker will not be rendered.")]
        [LogMessage("markerHeight has negative value. Marker will not be rendered.")]
        public virtual void MarkerEspecialMarkerWidthHeightValuesTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "markerEspecialMarkerWidthHeightValues");
        }

        [NUnit.Framework.Test]
        public virtual void DeformationWhenRotationAndPreserveAspectRationNoneTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "deformationWhenRotationAndPreserveAspectRationNone"
                );
        }

        [NUnit.Framework.Test]
        public virtual void MarkerParentElementTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "markerParentElement");
        }

        [NUnit.Framework.Test]
        public virtual void MarkerDefinedInStyleTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "markerDefinedInStyle");
        }

        [NUnit.Framework.Test]
        public virtual void MarkerOnGroupTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "markerOnGroup");
        }

        [NUnit.Framework.Test]
        public virtual void MarkerOnSvgTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "markerOnSvg");
        }

        [NUnit.Framework.Test]
        public virtual void MarkerOnSymbolTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "markerOnSymbol");
        }
    }
}
