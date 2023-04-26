/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 Apryse Group NV
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
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "widthHeightXYInCmUnitDiffPatternUnits");
        }

        [NUnit.Framework.Test]
        public virtual void WidthHeightXYInInchUnitDiffPatternUnitsTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "widthHeightXYInInchUnitDiffPatternUnits");
        }

        [NUnit.Framework.Test]
        public virtual void WidthHeightXYInEmUnitDiffPatternUnitsTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "widthHeightXYInEmUnitDiffPatternUnits");
        }

        [NUnit.Framework.Test]
        public virtual void WidthHeightXYInExUnitDiffPatternUnitsTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "widthHeightXYInExUnitDiffPatternUnits");
        }

        [NUnit.Framework.Test]
        public virtual void WidthHeightXYInPercentsDiffPatternUnitsTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "widthHeightXYInPercentsDiffPatternUnits");
        }

        [NUnit.Framework.Test]
        public virtual void WidthHeightXYInPxUnitDiffPatternUnitsTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "widthHeightXYInPxUnitDiffPatternUnits");
        }

        [NUnit.Framework.Test]
        public virtual void WidthHeightXYInMmUnitDiffPatternUnitsTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "widthHeightXYInMmUnitDiffPatternUnits");
        }

        [NUnit.Framework.Test]
        public virtual void WidthHeightXYInPtUnitDiffPatternUnitsTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "widthHeightXYInPtUnitDiffPatternUnits");
        }

        [NUnit.Framework.Test]
        public virtual void WidthHeightXYNoMeasureUnitTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "widthHeightXYNoMeasureUnit");
        }

        [NUnit.Framework.Test]
        public virtual void HrefAttributeTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "hrefAttribute");
        }

        [NUnit.Framework.Test]
        public virtual void PatternUnitsObjectBoundingBoxTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "patternUnitsObjectBoundingBox");
        }

        [NUnit.Framework.Test]
        public virtual void PatternUnitsUserSpaceOnUseTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "patternUnitsUserSpaceOnUse");
        }

        [NUnit.Framework.Test]
        public virtual void PreserveAspectRatioObjBoundingBoxTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "preserveAspectRatioObjBoundingBox", PageSize.A8);
        }

        [NUnit.Framework.Test]
        public virtual void PreserveAspectRatioUserSpaceOnUseTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "preserveAspectRatioUserSpaceOnUse", PageSize.A8);
        }

        [NUnit.Framework.Test]
        public virtual void ObjectBoundingBoxXMinYMidMeetTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "objectBoundingBoxXMinYMidMeet", PageSize.A8);
        }

        [NUnit.Framework.Test]
        public virtual void ObjectBoundingBoxXMidYMidMeetTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "objectBoundingBoxXMidYMidMeet", PageSize.A10);
        }

        [NUnit.Framework.Test]
        public virtual void ObjectBoundingBoxXMaxYMidMeetTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "objectBoundingBoxXMaxYMidMeet", PageSize.A10);
        }

        [NUnit.Framework.Test]
        public virtual void ObjectBoundingBoxXMidYMinMeetTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "objectBoundingBoxXMidYMinMeet", PageSize.A10);
        }

        [NUnit.Framework.Test]
        public virtual void ObjectBoundingBoxXMidYMaxMeetTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "objectBoundingBoxXMidYMaxMeet", PageSize.A10);
        }

        [NUnit.Framework.Test]
        public virtual void ObjectBoundingBoxXMidYMidMeetVerticalTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "objectBoundingBoxXMidYMidMeetVertical", PageSize.A10
                );
        }

        [NUnit.Framework.Test]
        public virtual void ObjectBoundingBoxXMinYMinMeetTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "objectBoundingBoxXMinYMinMeet", PageSize.A10);
        }

        [NUnit.Framework.Test]
        public virtual void ObjectBoundingBoxXMinYMinMeetVerticalTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "objectBoundingBoxXMinYMinMeetVertical", PageSize.A10
                );
        }

        [NUnit.Framework.Test]
        public virtual void ObjectBoundingBoxXMinYMaxMeetVerticalTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "objectBoundingBoxXMinYMaxMeetVertical", PageSize.A10
                );
        }

        [NUnit.Framework.Test]
        public virtual void ObjectBoundingBoxXMinYMaxMeetTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "objectBoundingBoxXMinYMaxMeet", PageSize.A10);
        }

        [NUnit.Framework.Test]
        public virtual void ObjectBoundingBoxXMaxYMinMeetTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "objectBoundingBoxXMaxYMinMeet", PageSize.A10);
        }

        [NUnit.Framework.Test]
        public virtual void ObjectBoundingBoxXMaxYMinMeetVerticalTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "objectBoundingBoxXMaxYMinMeetVertical", PageSize.A10
                );
        }

        [NUnit.Framework.Test]
        public virtual void ObjectBoundingBoxXMaxYMaxMeetTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "objectBoundingBoxXMaxYMaxMeet", PageSize.A10);
        }

        [NUnit.Framework.Test]
        public virtual void ObjectBoundingBoxXMaxYMaxMeetVerticalTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "objectBoundingBoxXMaxYMaxMeetVertical", PageSize.A10
                );
        }

        [NUnit.Framework.Test]
        public virtual void ObjectBoundingBoxNoneTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "objectBoundingBoxNone", PageSize.A10);
        }

        [NUnit.Framework.Test]
        public virtual void PatternContentUnitsUserSpaceOnUseTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "patternContentUnitsUserSpaceOnUse");
        }

        [NUnit.Framework.Test]
        public virtual void PatternContentUnitsObjBoundBoxTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "patternContentUnitsObjBoundBox");
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.StyledXmlParser.Logs.StyledXmlParserLogMessageConstant.UNKNOWN_ABSOLUTE_METRIC_LENGTH_PARSED
            , Count = 8)]
        public virtual void PatternContentUnitsObjBoundBoxAbsoluteCoordTest() {
            // TODO DEVSIX-4834 support relative units in attributes of svg elements
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "patternContentUnitsObjBoundBoxAbsoluteCoord");
        }

        [NUnit.Framework.Test]
        //TODO DEVSIX-4834 support relative units in attributes of svg elements
        [LogMessage(iText.StyledXmlParser.Logs.StyledXmlParserLogMessageConstant.UNKNOWN_ABSOLUTE_METRIC_LENGTH_PARSED
            , Count = 8)]
        public virtual void ViewBoxAndAbsoluteCoordinatesTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "viewBoxAndAbsoluteCoordinates");
        }

        [NUnit.Framework.Test]
        public virtual void PatternTransformSimpleTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "patternTransformSimple");
        }

        [NUnit.Framework.Test]
        public virtual void PatternTransformUnitsObjectBoundingBoxTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "patternTransformUnitsObjectBoundingBox");
        }

        [NUnit.Framework.Test]
        public virtual void PatternTransformUnitsUserSpaceOnUseTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "patternTransformUnitsUserSpaceOnUse");
        }

        [NUnit.Framework.Test]
        public virtual void PatternTransformObjBoundingBoxTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "patternTransformObjBoundingBox");
        }

        [NUnit.Framework.Test]
        public virtual void PatternTransformUserSpaceOnUseTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "patternTransformUserSpaceOnUse");
        }

        [NUnit.Framework.Test]
        public virtual void PatternTransformMixed1Test() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "patternTransformMixed1");
        }

        [NUnit.Framework.Test]
        public virtual void PatternTransformMixed2Test() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "patternTransformMixed2");
        }

        [NUnit.Framework.Test]
        public virtual void PatternTransformViewBoxUsrSpaceOnUseTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "patternTransformViewBoxUsrSpaceOnUse");
        }

        [NUnit.Framework.Test]
        public virtual void PatternTransformViewBoxObjBoundBoxTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "patternTransformViewBoxObjBoundBox");
        }

        [NUnit.Framework.Test]
        public virtual void PatternTransformElementTransformTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "patternTransformElementTransform", PageSize.A8);
        }

        [NUnit.Framework.Test]
        public virtual void PatternTransformTranslateTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "patternTransformTranslate");
        }

        [NUnit.Framework.Test]
        public virtual void PreserveAspectRatioXMaxYMidMeetTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "preserveAspectRatioXMaxYMidMeet");
        }

        [NUnit.Framework.Test]
        public virtual void PreserveAspectRatioXMaxYMidSliceTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "preserveAspectRatioXMaxYMidSlice");
        }

        [NUnit.Framework.Test]
        public virtual void PreserveAspectRatioXMidYMaxMeetTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "preserveAspectRatioXMidYMaxMeet");
        }

        [NUnit.Framework.Test]
        public virtual void PreserveAspectRatioXMidYMaxSliceTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "preserveAspectRatioXMidYMaxSlice");
        }

        [NUnit.Framework.Test]
        public virtual void RelativeUnitsResolveFromDefsTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "relativeUnitsResolveFromDefs");
        }

        [NUnit.Framework.Test]
        public virtual void RelativeUnitsResolveFromPatternTest() {
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
        [LogMessage(iText.StyledXmlParser.Logs.StyledXmlParserLogMessageConstant.UNKNOWN_ABSOLUTE_METRIC_LENGTH_PARSED
            , Count = 2)]
        public virtual void SeveralComplexElementsInsidePatternTest() {
            // TODO DEVSIX-4834 support relative units in attributes of svg elements
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "severalComplexElementsInsidePattern");
        }

        [NUnit.Framework.Test]
        public virtual void NestedPatternsWithComplexElementsInsideTest() {
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

        [NUnit.Framework.Test]
        public virtual void ViewBoxPatternXYTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "viewBoxPatternXY");
        }

        [NUnit.Framework.Test]
        public virtual void ViewBoxClippedTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "viewBoxClipped");
        }

        [NUnit.Framework.Test]
        public virtual void CoordSystemTransformUserSpaceOnUseTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "coordSystemTransformUserSpaceOnUse");
        }

        [NUnit.Framework.Test]
        public virtual void CoordSystemTransformObjBoundingBoxTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "coordSystemTransformObjBoundingBox");
        }

        [NUnit.Framework.Test]
        public virtual void CoordSystemTransformMixed1Test() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "coordSystemTransformMixed1");
        }

        [NUnit.Framework.Test]
        public virtual void CoordSystemTransformMixed2Test() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "coordSystemTransformMixed2");
        }

        [NUnit.Framework.Test]
        public virtual void CoordSystemTransform() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "coordSystemTransform");
        }

        [NUnit.Framework.Test]
        [LogMessage(SvgLogMessageConstant.VIEWBOX_VALUE_MUST_BE_FOUR_NUMBERS, Count = 1)]
        public virtual void IncorrectViewBoxValuesNumberTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "incorrectViewBoxValuesNumber");
        }

        [NUnit.Framework.Test]
        [LogMessage(SvgLogMessageConstant.VIEWBOX_WIDTH_AND_HEIGHT_CANNOT_BE_NEGATIVE)]
        public virtual void IncorrectViewBoxNegativeWidthTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "incorrectViewBoxNegativeWidth");
        }

        [NUnit.Framework.Test]
        [LogMessage(SvgLogMessageConstant.VIEWBOX_WIDTH_AND_HEIGHT_CANNOT_BE_NEGATIVE)]
        public virtual void IncorrectViewBoxNegativeHeightTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "incorrectViewBoxNegativeHeight");
        }

        [NUnit.Framework.Test]
        public virtual void ViewBoxZeroWidthTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "viewBoxZeroWidth");
        }

        [NUnit.Framework.Test]
        public virtual void ViewBoxZeroHeightTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "viewBoxZeroHeight");
        }

        [NUnit.Framework.Test]
        [LogMessage(SvgLogMessageConstant.PATTERN_WIDTH_OR_HEIGHT_IS_NEGATIVE)]
        public virtual void PatternNegativeWidthTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "patternNegativeWidth");
        }
    }
}
