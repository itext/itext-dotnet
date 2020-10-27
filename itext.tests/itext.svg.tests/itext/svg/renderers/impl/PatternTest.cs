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
using iText.Svg;
using iText.Test;

namespace iText.Svg.Renderers.Impl {
    public class PatternTest : ExtendedITextTest {
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
            //TODO: DEVSIX-3347 pattern element isn't supported
            SvgNodeRendererIntegrationTestUtil.ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "widthHeightXYInCmUnitDiffPatternUnits"
                );
        }

        [NUnit.Framework.Test]
        public virtual void WidthHeightXYInInchUnitDiffPatternUnitsTest() {
            //TODO: DEVSIX-3347 pattern element isn't supported
            SvgNodeRendererIntegrationTestUtil.ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "widthHeightXYInInchUnitDiffPatternUnits"
                );
        }

        [NUnit.Framework.Test]
        public virtual void WidthHeightXYInEmUnitDiffPatternUnitsTest() {
            //TODO: DEVSIX-3347 pattern element isn't supported
            SvgNodeRendererIntegrationTestUtil.ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "widthHeightXYInEmUnitDiffPatternUnits"
                );
        }

        [NUnit.Framework.Test]
        public virtual void WidthHeightXYInExUnitDiffPatternUnitsTest() {
            //TODO: DEVSIX-3347 pattern element isn't supported
            SvgNodeRendererIntegrationTestUtil.ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "widthHeightXYInExUnitDiffPatternUnits"
                );
        }

        [NUnit.Framework.Test]
        public virtual void WidthHeightXYInPercentsDiffPatternUnitsTest() {
            //TODO: DEVSIX-3347 pattern element isn't supported
            SvgNodeRendererIntegrationTestUtil.ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "widthHeightXYInPercentsDiffPatternUnits"
                );
        }

        [NUnit.Framework.Test]
        public virtual void WidthHeightXYInPxUnitDiffPatternUnitsTest() {
            //TODO: DEVSIX-3347 pattern element isn't supported
            SvgNodeRendererIntegrationTestUtil.ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "widthHeightXYInPxUnitDiffPatternUnits"
                );
        }

        [NUnit.Framework.Test]
        public virtual void WidthHeightXYInMmUnitDiffPatternUnitsTest() {
            //TODO: DEVSIX-3347 pattern element isn't supported
            SvgNodeRendererIntegrationTestUtil.ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "widthHeightXYInMmUnitDiffPatternUnits"
                );
        }

        [NUnit.Framework.Test]
        public virtual void WidthHeightXYInPtUnitDiffPatternUnitsTest() {
            //TODO: DEVSIX-3347 pattern element isn't supported
            SvgNodeRendererIntegrationTestUtil.ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "widthHeightXYInPtUnitDiffPatternUnits"
                );
        }

        [NUnit.Framework.Test]
        public virtual void WidthHeightXYNoMeasureUnitTest() {
            //TODO: DEVSIX-3347 pattern element isn't supported
            SvgNodeRendererIntegrationTestUtil.ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "widthHeightXYNoMeasureUnit"
                );
        }

        [NUnit.Framework.Test]
        public virtual void HrefAttributeTest() {
            //TODO: DEVSIX-3347 pattern element isn't supported
            SvgNodeRendererIntegrationTestUtil.ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "hrefAttribute");
        }

        [NUnit.Framework.Test]
        public virtual void PatternUnitsObjectBoundingBoxTest() {
            //TODO: DEVSIX-3347 pattern element isn't supported
            SvgNodeRendererIntegrationTestUtil.ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "patternUnitsObjectBoundingBox"
                );
        }

        [NUnit.Framework.Test]
        public virtual void PatternUnitsUserSpaceOnUseTest() {
            //TODO: DEVSIX-3347 pattern element isn't supported
            SvgNodeRendererIntegrationTestUtil.ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "patternUnitsUserSpaceOnUse"
                );
        }

        [NUnit.Framework.Test]
        public virtual void PatternContentUnitsUserSpaceOnUseTest() {
            //TODO: DEVSIX-3347 pattern element isn't supported
            SvgNodeRendererIntegrationTestUtil.ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "patternContentUnitsUserSpaceOnUse"
                );
        }

        [NUnit.Framework.Test]
        public virtual void PatternContentUnitsObjBoundBoxTest() {
            //TODO: DEVSIX-3347 pattern element isn't supported
            SvgNodeRendererIntegrationTestUtil.ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "patternContentUnitsObjBoundBox"
                );
        }

        [NUnit.Framework.Test]
        public virtual void PatternContentUnitsObjBoundBoxAbsoluteCoordTest() {
            //TODO: DEVSIX-3347 pattern element isn't supported
            SvgNodeRendererIntegrationTestUtil.ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "patternContentUnitsObjBoundBoxAbsoluteCoord"
                );
        }

        [NUnit.Framework.Test]
        public virtual void ViewBoxAndAbsoluteCoordinatesTest() {
            //TODO: DEVSIX-3347 pattern element isn't supported
            SvgNodeRendererIntegrationTestUtil.ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "viewBoxAndAbsoluteCoordinates"
                );
        }

        [NUnit.Framework.Test]
        public virtual void PatternTransformSimpleTest() {
            //TODO: DEVSIX-3347 pattern element isn't supported
            SvgNodeRendererIntegrationTestUtil.ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "patternTransformSimple"
                );
        }

        [NUnit.Framework.Test]
        public virtual void PatternTransformUnitsObjectBoundingBoxTest() {
            //TODO: DEVSIX-3347 pattern element isn't supported
            SvgNodeRendererIntegrationTestUtil.ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "patternTransformUnitsObjectBoundingBox"
                );
        }

        [NUnit.Framework.Test]
        public virtual void PatternTransformUnitsUserSpaceOnUseTest() {
            //TODO: DEVSIX-3347 pattern element isn't supported
            SvgNodeRendererIntegrationTestUtil.ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "patternTransformUnitsUserSpaceOnUse"
                );
        }

        [NUnit.Framework.Test]
        public virtual void PreserveAspectRatioXMaxYMidMeetTest() {
            //TODO: DEVSIX-3347 pattern element isn't supported
            SvgNodeRendererIntegrationTestUtil.ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "preserveAspectRatioXMaxYMidMeet"
                );
        }

        [NUnit.Framework.Test]
        public virtual void PreserveAspectRatioXMaxYMidSliceTest() {
            //TODO: DEVSIX-3347 pattern element isn't supported
            SvgNodeRendererIntegrationTestUtil.ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "preserveAspectRatioXMaxYMidSlice"
                );
        }

        [NUnit.Framework.Test]
        public virtual void PreserveAspectRatioXMidYMaxMeetTest() {
            //TODO: DEVSIX-3347 pattern element isn't supported
            SvgNodeRendererIntegrationTestUtil.ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "preserveAspectRatioXMidYMaxMeet"
                );
        }

        [NUnit.Framework.Test]
        public virtual void PreserveAspectRatioXMidYMaxSliceTest() {
            //TODO: DEVSIX-3347 pattern element isn't supported
            SvgNodeRendererIntegrationTestUtil.ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "preserveAspectRatioXMidYMaxSlice"
                );
        }

        [NUnit.Framework.Test]
        public virtual void RelativeUnitsResolveFromDefsTest() {
            //TODO: DEVSIX-3347 pattern element isn't supported
            SvgNodeRendererIntegrationTestUtil.ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "relativeUnitsResolveFromDefs"
                );
        }

        [NUnit.Framework.Test]
        public virtual void RelativeUnitsResolveFromPatternTest() {
            //TODO: DEVSIX-3347 pattern element isn't supported
            SvgNodeRendererIntegrationTestUtil.ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "relativeUnitsResolveFromPattern"
                );
        }

        [NUnit.Framework.Test]
        public virtual void LinearGradientInsidePatternTest() {
            //TODO: DEVSIX-3347 pattern element isn't supported
            SvgNodeRendererIntegrationTestUtil.ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "linearGradientInsidePattern"
                );
        }

        [NUnit.Framework.Test]
        public virtual void NestedPatternsTest() {
            //TODO: DEVSIX-3347 pattern element isn't supported
            SvgNodeRendererIntegrationTestUtil.ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "nestedPatterns");
        }

        [NUnit.Framework.Test]
        public virtual void SeveralComplexElementsInsidePatternTest() {
            //TODO: DEVSIX-3347 pattern element isn't supported
            SvgNodeRendererIntegrationTestUtil.ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "severalComplexElementsInsidePattern"
                );
        }

        [NUnit.Framework.Test]
        public virtual void NestedPatternsWithComplexElementsInsideTest() {
            //TODO: DEVSIX-3347 pattern element isn't supported
            SvgNodeRendererIntegrationTestUtil.ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "nestedPatternsWithComplexElementsInside"
                );
        }

        [NUnit.Framework.Test]
        public virtual void PatternUseItselfTest() {
            //TODO: DEVSIX-3347 pattern element isn't supported
            SvgNodeRendererIntegrationTestUtil.ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "patternUseItself"
                );
        }

        [NUnit.Framework.Test]
        public virtual void NestedPatternsLinkedToEachOtherTest() {
            //TODO: DEVSIX-3347 pattern element isn't supported
            SvgNodeRendererIntegrationTestUtil.ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "nestedPatternsLinkedToEachOther"
                );
        }
    }
}
