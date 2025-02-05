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
using iText.Svg.Logs;
using iText.Svg.Renderers;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Svg.Renderers.Impl {
    [NUnit.Framework.Category("IntegrationTest")]
    public class XLinkDataTest : SvgIntegrationTest {
        private static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/svg/renderers/impl/XLinkDataTest/";

        private static readonly String DESTINATION_FOLDER = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/svg/renderers/impl/XLinkDataTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            ITextTest.CreateDestinationFolder(DESTINATION_FOLDER);
        }

        [NUnit.Framework.Test]
        public virtual void CorrectImageWithDataTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "correctImageWithData");
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.StyledXmlParser.Logs.StyledXmlParserLogMessageConstant.UNABLE_TO_RETRIEVE_IMAGE_WITH_GIVEN_DATA_URI
            )]
        public virtual void IncorrectImageWithDataTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "incorrectImageWithData");
        }

        [NUnit.Framework.Test]
        public virtual void LinearGradXlinkTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "linearGradXlinkHref");
        }

        [NUnit.Framework.Test]
        public virtual void LinearGradXlink3StopsTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "linearGradXlinkHref3Stops");
        }

        [NUnit.Framework.Test]
        public virtual void LinearGradXlinkGradientTransformTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "linearGradXlinkHrefGradientTransform");
        }

        [NUnit.Framework.Test]
        public virtual void LinearGradXlinkNegativeOffsetTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "linearGradXlinkHrefNegativeOffset");
        }

        [NUnit.Framework.Test]
        public virtual void LinearGradXlinkNegativeOpacityTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "linearGradXlinkHrefNegativeOpacity");
        }

        [NUnit.Framework.Test]
        public virtual void LinearGradXlinkOpacityTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "linearGradXlinkHrefOpacity");
        }

        [NUnit.Framework.Test]
        public virtual void LinearGradXlinkOpacity2Test() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "linearGradXlinkHrefOpacity2");
        }

        [NUnit.Framework.Test]
        public virtual void LinearGradXlinkSpreadMethodTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "linearGradXlinkHrefSpreadMethod1");
        }

        [NUnit.Framework.Test]
        public virtual void LinearGradXlinkSpreadMethod2Test() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "linearGradXlinkHrefSpreadMethod2");
        }

        [NUnit.Framework.Test]
        public virtual void LinearGradXlinkSpreadMethod3Test() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "linearGradXlinkHrefSpreadMethod3");
        }

        [NUnit.Framework.Test]
        public virtual void LinearGradXlinkHrefXYvalsTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "linearGradXlinkHrefXYvals1");
        }

        [NUnit.Framework.Test]
        public virtual void LinearGradXlinkHrefXYvals2Test() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "linearGradXlinkHrefXYvals2");
        }

        [NUnit.Framework.Test]
        public virtual void LinearGradXlinkHrefXYvals3Test() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "linearGradXlinkHrefXYvals3");
        }

        [NUnit.Framework.Test]
        public virtual void LinearGradXlinkHreOffsetSwapTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "linearGradXlinkHreOffsetSwap");
        }

        [NUnit.Framework.Test]
        public virtual void PatternXlinkTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "patternXlinkHref");
        }

        [NUnit.Framework.Test]
        public virtual void PatternXlinkHrefPatternContentUnits1Test() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "patternXlinkHrefPatternContentUnits1");
        }

        [NUnit.Framework.Test]
        public virtual void PatternXlinkHrefPatternContentUnits2Test() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "patternXlinkHrefPatternContentUnits2");
        }

        [NUnit.Framework.Test]
        public virtual void PatternXlinkHrefPatternUnitsTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "patternXlinkHrefPatternUnits");
        }

        [NUnit.Framework.Test]
        public virtual void PatternXlinkHrefPreserveAR1Test() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "patternXlinkHrefPreserveAR1");
        }

        [NUnit.Framework.Test]
        public virtual void PatternXlinkHrefPreserveAR2Test() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "patternXlinkHrefPreserveAR2");
        }

        //TODO DEVSIX-2255: Update cmp file after supporting
        [NUnit.Framework.Test]
        [LogMessage(SvgLogMessageConstant.UNMAPPED_TAG)]
        public virtual void TextPathXlinkTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "textPathXrefHref");
        }
    }
}
