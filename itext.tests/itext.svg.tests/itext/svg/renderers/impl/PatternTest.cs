/*
This file is part of the iText (R) project.
Copyright (c) 1998-2020 iText Group NV
Authors: iText Software.

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
using iText.Svg.Renderers;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Svg.Renderers.Impl {
    public class PatternTest : SvgIntegrationTest {
        public static readonly String DESTINATION_FOLDER = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/svg/renderers/impl/PatternTest/";

        public static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/svg/renderers/impl/PatternTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            ITextTest.CreateDestinationFolder(DESTINATION_FOLDER);
        }

        [NUnit.Framework.Test]
        public virtual void WidthHeightXYInCmUnitDiffPatternUnitsTest() {
            //TODO: DEVSIX-4782 support 'viewbox'
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "widthHeightXYInCmUnitDiffPatternUnits");
        }

        [NUnit.Framework.Test]
        public virtual void WidthHeightXYInInchUnitDiffPatternUnitsTest() {
            //TODO: DEVSIX-4782 support 'viewbox'
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "widthHeightXYInInchUnitDiffPatternUnits");
        }

        [NUnit.Framework.Test]
        public virtual void WidthHeightXYInEmUnitDiffPatternUnitsTest() {
            //TODO: DEVSIX-4782 support 'viewbox'
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "widthHeightXYInEmUnitDiffPatternUnits");
        }

        [NUnit.Framework.Test]
        public virtual void WidthHeightXYInExUnitDiffPatternUnitsTest() {
            //TODO: DEVSIX-4782 support 'viewbox'
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "widthHeightXYInExUnitDiffPatternUnits");
        }

        [NUnit.Framework.Test]
        public virtual void WidthHeightXYInPercentsDiffPatternUnitsTest() {
            //TODO: DEVSIX-4782 support viewbox
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "widthHeightXYInPercentsDiffPatternUnits");
        }

        [NUnit.Framework.Test]
        public virtual void WidthHeightXYInPxUnitDiffPatternUnitsTest() {
            //TODO: DEVSIX-4782 support viewbox
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "widthHeightXYInPxUnitDiffPatternUnits");
        }

        [NUnit.Framework.Test]
        public virtual void WidthHeightXYInMmUnitDiffPatternUnitsTest() {
            //TODO: DEVSIX-4782 support 'viewbox'
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "widthHeightXYInMmUnitDiffPatternUnits");
        }

        [NUnit.Framework.Test]
        public virtual void WidthHeightXYInPtUnitDiffPatternUnitsTest() {
            //TODO: DEVSIX-4782 support viewbox
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "widthHeightXYInPtUnitDiffPatternUnits");
        }

        [NUnit.Framework.Test]
        public virtual void WidthHeightXYNoMeasureUnitTest() {
            //TODO: DEVSIX-4782 support viewbox
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "widthHeightXYNoMeasureUnit");
        }

        [NUnit.Framework.Test]
        public virtual void HrefAttributeTest() {
            //TODO DEVSIX-4782 support 'viewbox' and `preserveAspectRatio' attribute for SVG pattern element
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "hrefAttribute");
        }

        [NUnit.Framework.Test]
        public virtual void PatternUnitsObjectBoundingBoxTest() {
            //TODO DEVSIX-4782 support 'viewbox' and `preserveAspectRatio' attribute for SVG pattern element
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "patternUnitsObjectBoundingBox");
        }

        [NUnit.Framework.Test]
        public virtual void PatternUnitsUserSpaceOnUseTest() {
            //TODO: DEVSIX-4782 Support 'viewbox' and `preserveAspectRatio' attribute for SVG pattern element
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "patternUnitsUserSpaceOnUse");
        }

        [NUnit.Framework.Test]
        public virtual void PatternContentUnitsUserSpaceOnUseTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "patternContentUnitsUserSpaceOnUse");
        }

        [NUnit.Framework.Test]
        public virtual void PatternContentUnitsObjBoundBoxTest() {
            //TODO DEVSIX-4782 support 'viewbox' and `preserveAspectRatio' attribute for SVG pattern element
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "patternContentUnitsObjBoundBox");
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.StyledXmlParser.LogMessageConstant.UNKNOWN_ABSOLUTE_METRIC_LENGTH_PARSED, Count = 8)]
        public virtual void PatternContentUnitsObjBoundBoxAbsoluteCoordTest() {
            // TODO DEVSIX-4834 support relative units in attributes of svg elements
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "patternContentUnitsObjBoundBoxAbsoluteCoord");
        }

        [NUnit.Framework.Test]
        public virtual void ViewBoxAndAbsoluteCoordinatesTest() {
            //TODO DEVSIX-4782 support 'viewbox' and `preserveAspectRatio' attribute for SVG pattern element
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "viewBoxAndAbsoluteCoordinates");
        }

        [NUnit.Framework.Test]
        public virtual void PatternTransformSimpleTest() {
            //TODO DEVSIX-4782 support 'viewbox' and `preserveAspectRatio' attribute for SVG pattern element
            //TODO DEVSIX-4811 support 'patternTransform' attribute for SVG pattern element
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "patternTransformSimple");
        }

        [NUnit.Framework.Test]
        public virtual void PatternTransformUnitsObjectBoundingBoxTest() {
            //TODO DEVSIX-4782 support 'viewbox' and `preserveAspectRatio' attribute for SVG pattern element
            //TODO DEVSIX-4811 support 'patternTransform' attribute for SVG pattern element
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "patternTransformUnitsObjectBoundingBox");
        }

        [NUnit.Framework.Test]
        public virtual void PatternTransformUnitsUserSpaceOnUseTest() {
            //TODO DEVSIX-4782 support 'viewbox' and `preserveAspectRatio' attribute for SVG pattern element
            //TODO DEVSIX-4811 support 'patternTransform' attribute for SVG pattern element
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "patternTransformUnitsUserSpaceOnUse");
        }

        [NUnit.Framework.Test]
        public virtual void PreserveAspectRatioXMaxYMidMeetTest() {
            //TODO DEVSIX-4782 support 'viewbox' and `preserveAspectRatio' attribute for SVG pattern element
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "preserveAspectRatioXMaxYMidMeet");
        }

        [NUnit.Framework.Test]
        public virtual void PreserveAspectRatioXMaxYMidSliceTest() {
            //TODO DEVSIX-4782 support 'viewbox' and `preserveAspectRatio' attribute for SVG pattern element
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "preserveAspectRatioXMaxYMidSlice");
        }

        [NUnit.Framework.Test]
        public virtual void PreserveAspectRatioXMidYMaxMeetTest() {
            //TODO DEVSIX-4782 support 'viewbox' and `preserveAspectRatio' attribute for SVG pattern element
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "preserveAspectRatioXMidYMaxMeet");
        }

        [NUnit.Framework.Test]
        public virtual void PreserveAspectRatioXMidYMaxSliceTest() {
            //TODO DEVSIX-4782 support 'viewbox' and `preserveAspectRatio' attribute for SVG pattern element
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "preserveAspectRatioXMidYMaxSlice");
        }

        [NUnit.Framework.Test]
        public virtual void RelativeUnitsResolveFromDefsTest() {
            //TODO DEVSIX-4782 support 'viewbox' and `preserveAspectRatio' attribute for SVG pattern element
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "relativeUnitsResolveFromDefs");
        }

        [NUnit.Framework.Test]
        public virtual void RelativeUnitsResolveFromPatternTest() {
            //TODO DEVSIX-4782 support 'viewbox' and `preserveAspectRatio' attribute for SVG pattern element
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "relativeUnitsResolveFromPattern");
        }

        [NUnit.Framework.Test]
        public virtual void LinearGradientInsidePatternTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "linearGradientInsidePattern");
        }

        [NUnit.Framework.Test]
        public virtual void NestedPatternsTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "nestedPatterns");
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.StyledXmlParser.LogMessageConstant.UNKNOWN_ABSOLUTE_METRIC_LENGTH_PARSED, Count = 2)]
        public virtual void SeveralComplexElementsInsidePatternTest() {
            // TODO DEVSIX-4834 support relative units in attributes of svg elements
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "severalComplexElementsInsidePattern");
        }

        [NUnit.Framework.Test]
        public virtual void NestedPatternsWithComplexElementsInsideTest() {
            //TODO DEVSIX-4782 support 'viewbox' and `preserveAspectRatio' attribute for SVG pattern element
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "nestedPatternsWithComplexElementsInside");
        }

        [NUnit.Framework.Test]
        public virtual void PatternUseItselfTest() {
            // Behavior differs from browser. In our implementation we use default color for element with cycled pattern.
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "patternUseItself");
        }

        [NUnit.Framework.Test]
        public virtual void NestedPatternsLinkedToEachOtherTest() {
            // Behavior differs from browser. In our implementation we use default color for element with cycled pattern.
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "nestedPatternsLinkedToEachOther");
        }

        [NUnit.Framework.Test]
        public virtual void SimplePatternTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "simplePatternTest");
        }

        [NUnit.Framework.Test]
        public virtual void SimplePatternInheritStylesTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "simplePatternInheritStylesTest");
        }

        [NUnit.Framework.Test]
        public virtual void SimplePatternNestedTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "simplePatternNestedTest");
        }

        [NUnit.Framework.Test]
        public virtual void SimplePatternStrokeTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "simplePatternStrokeTest");
        }

        [NUnit.Framework.Test]
        public virtual void SimplePatternNestedFillInheritanceTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "simplePatternNestedFillInheritanceTest");
        }

        [NUnit.Framework.Test]
        public virtual void PatternContentUnitsObjectBoundingBoxTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "patternContentUnitsObjectBoundingBox");
        }

        [NUnit.Framework.Test]
        public virtual void CycledPatternsUserSpaceOnUseTest() {
            // Behavior differs from browser. We use default color instead cycled pattern.
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "cycledPatternsUserSpaceOnUse");
        }

        [NUnit.Framework.Test]
        public virtual void ObjBoundingBoxWithMarginsTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "objBoundingBoxWithMargins");
        }

        [NUnit.Framework.Test]
        public virtual void ObjBoundingBoxUserSpaceOnUseTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "objBoundingBoxUserSpaceOnUse");
        }

        [NUnit.Framework.Test]
        public virtual void UserSpaceOnUseObjBoundingBoxTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "userSpaceOnUseObjBoundingBox");
        }

        [NUnit.Framework.Test]
        public virtual void PatternDefaultWidthTest() {
            // we print the default color that is black
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "patternDefaultWidth");
        }

        [NUnit.Framework.Test]
        public virtual void PatternDefaultHeightTest() {
            // we print the default color that is black
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "patternDefaultHeight");
        }
    }
}
