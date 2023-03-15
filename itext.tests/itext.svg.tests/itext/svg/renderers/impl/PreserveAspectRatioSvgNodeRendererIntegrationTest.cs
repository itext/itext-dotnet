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
using iText.Svg.Renderers;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Svg.Renderers.Impl {
    [NUnit.Framework.Category("IntegrationTest")]
    public class PreserveAspectRatioSvgNodeRendererIntegrationTest : SvgIntegrationTest {
        private static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/svg/renderers/impl/PreserveAspectRatioSvgNodeRendererIntegrationTest/";

        private static readonly String DESTINATION_FOLDER = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/svg/renderers/impl/PreserveAspectRatioSvgNodeRendererIntegrationTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            ITextTest.CreateDestinationFolder(DESTINATION_FOLDER);
        }

        [NUnit.Framework.Test]
        public virtual void AspectRatioPreservationMidXMidYMeetMinimalTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "MidXMidYMeetMinimalTest");
        }

        [NUnit.Framework.Test]
        public virtual void ViewBoxScalingTestPreserveAspectDefaultAll() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "PreserveAspectDefaultAll");
        }

        [NUnit.Framework.Test]
        public virtual void ViewBoxWithoutSetPreserveAspectRatioTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "PreserveAspectRatio");
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.StyledXmlParser.Logs.StyledXmlParserLogMessageConstant.INVALID_CSS_PROPERTY_DECLARATION, 
            Count = 19)]
        public virtual void DifferentAspectRatiosTest() {
            //TODO: update cmp_ when DEVSIX-2250 fixed
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "differentAspectRatios");
        }

        [NUnit.Framework.Test]
        public virtual void ViewBoxScalingTestPreserveAspectDefaultAllGroup() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "PreserveAspectDefaultAllGroup");
        }

        [NUnit.Framework.Test]
        public virtual void ViewBoxScalingTestDoNotPreserveAspectMin() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "DoNotPreserveAspectMin");
        }

        [NUnit.Framework.Test]
        public virtual void ViewBoxScalingTestDoNotPreserveAspectAll() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "DoNotPreserveAspectAll");
        }

        [NUnit.Framework.Test]
        public virtual void ViewBoxScalingTestDoNotPreserveAspectMetricDimensionsMin() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "DoNotPreserveAspectMetricDimensionsMin");
        }

        [NUnit.Framework.Test]
        public virtual void ViewBoxScalingTestDoNotPreserveAspectMetricDimensionsAll() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "DoNotPreserveAspectMetricDimensionsAll");
        }

        [NUnit.Framework.Test]
        public virtual void ViewBoxScalingTestPreserveAspectRatioXMinYMinMeetScaling() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "PreserveAspectRatioXMinYMinMeetScaling");
        }

        [NUnit.Framework.Test]
        public virtual void ViewBoxScalingTestPreserveAspectRatioXMinYMidMeetScaling() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "PreserveAspectRatioXMinYMidMeetScaling");
        }

        [NUnit.Framework.Test]
        public virtual void ViewBoxScalingTestPreserveAspectRatioXMinYMaxMeetScaling() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "PreserveAspectRatioXMinYMaxMeetScaling");
        }

        [NUnit.Framework.Test]
        public virtual void ViewBoxScalingTestPreserveAspectRatioXMidYMinMeetScaling() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "PreserveAspectRatioXMidYMinMeetScaling");
        }

        [NUnit.Framework.Test]
        public virtual void ViewBoxScalingTestPreserveAspectRatioXMidYMaxMeetScaling() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "PreserveAspectRatioXMidYMaxMeetScaling");
        }

        [NUnit.Framework.Test]
        public virtual void ViewBoxScalingTestPreserveAspectRatioXMaxYMinMeetScaling() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "PreserveAspectRatioXMaxYMinMeetScaling");
        }

        [NUnit.Framework.Test]
        public virtual void ViewBoxScalingTestPreserveAspectRatioXMaxYMidMeetScaling() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "PreserveAspectRatioXMaxYMidMeetScaling");
        }

        [NUnit.Framework.Test]
        public virtual void ViewBoxTranslationTestInnerZeroCoordinatesViewBox() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "InnerZeroCoordinatesViewBox");
        }

        [NUnit.Framework.Test]
        public virtual void ViewBoxTranslationTestOuterZeroCoordinatesViewBox() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "OuterZeroCoordinatesViewBox");
        }

        [NUnit.Framework.Test]
        public virtual void ViewBoxTranslationTestMultipleViewBoxes() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "MultipleViewBoxes");
        }

        [NUnit.Framework.Test]
        public virtual void SvgTranslationYMinMeetTest() {
            //TODO (DEVSIX-3537) change cmp files after the ticket will be fixed 
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "svgTranslationYMinMeet");
        }

        [NUnit.Framework.Test]
        public virtual void SvgTranslationYMidMeetTest() {
            //TODO (DEVSIX-3537) change cmp files after the ticket will be fixed
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "svgTranslationYMidMeet");
        }

        [NUnit.Framework.Test]
        public virtual void SvgTranslationYMaxMeetTest() {
            //TODO (DEVSIX-3537) change cmp files after the ticket will be fixed
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "svgTranslationYMaxMeet");
        }

        [NUnit.Framework.Test]
        public virtual void SvgTranslationXMinMeetTest() {
            //TODO (DEVSIX-3537) change cmp files after the ticket will be fixed
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "svgTranslationXMinMeet");
        }

        [NUnit.Framework.Test]
        public virtual void SvgTranslationXMidMeetTest() {
            //TODO (DEVSIX-3537) change cmp files after the ticket will be fixed
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "svgTranslationXMidMeet");
        }

        [NUnit.Framework.Test]
        public virtual void SvgTranslationXMaxMeetTest() {
            //TODO (DEVSIX-3537) change cmp files after the ticket will be fixed
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "svgTranslationXMaxMeet");
        }
    }
}
