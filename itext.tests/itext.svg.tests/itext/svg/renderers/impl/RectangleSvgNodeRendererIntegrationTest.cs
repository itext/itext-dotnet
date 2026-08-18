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
using iText.Svg.Renderers;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Svg.Renderers.Impl {
    [NUnit.Framework.Category("IntegrationTest")]
    public class RectangleSvgNodeRendererIntegrationTest : SvgIntegrationTest {
        private static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/svg/renderers/impl/RectangleSvgNodeRendererTest/";

        private static readonly String DESTINATION_FOLDER = TestUtil.GetOutputPath() + "/svg/renderers/impl/RectangleSvgNodeRendererTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            ITextTest.CreateDestinationFolder(DESTINATION_FOLDER);
        }

        [NUnit.Framework.Test]
        public virtual void BasicRectangleTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "basicRectangle");
        }

        [NUnit.Framework.Test]
        public virtual void BasicRectangleRxRyZeroTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "basicRectangleRxRyZero");
        }

        [NUnit.Framework.Test]
        public virtual void BasicCircularRoundedRectangleRyZeroTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "basicCircularRoundedRectangleRyZero");
        }

        [NUnit.Framework.Test]
        public virtual void BasicCircularRoundedRectangleRxZeroTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "basicCircularRoundedRectangleRxZero");
        }

        [NUnit.Framework.Test]
        public virtual void BasicCircularRoundedRxRectangleTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "basicCircularRoundedRxRectangle");
        }

        [NUnit.Framework.Test]
        public virtual void BasicCircularRoundedRyRectangleTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "basicCircularRoundedRyRectangle");
        }

        [NUnit.Framework.Test]
        public virtual void BasicEllipticalRoundedRectangleXTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "basicEllipticalRoundedRectangleX");
        }

        [NUnit.Framework.Test]
        public virtual void BasicEllipticalRoundedRectangleYTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "basicEllipticalRoundedRectangleY");
        }

        [NUnit.Framework.Test]
        public virtual void BasicEllipticalWidthCappedRoundedRectangleTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "basicEllipticalWidthCappedRoundedRectangle");
        }

        [NUnit.Framework.Test]
        public virtual void BasicEllipticalHeightCappedRoundedRectangleTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "basicEllipticalHeightCappedRoundedRectangle");
        }

        [NUnit.Framework.Test]
        public virtual void BasicEllipticalNegativeWidthRoundedRectangleTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "basicEllipticalNegativeWidthRoundedRectangle");
        }

        [NUnit.Framework.Test]
        public virtual void BasicEllipticalNegativeHeightRoundedRectangleTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "basicEllipticalNegativeHeightRoundedRectangle");
        }

        [NUnit.Framework.Test]
        public virtual void ComplexRectangleTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "complexRectangle");
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.StyledXmlParser.Logs.StyledXmlParserLogMessageConstant.INVALID_CSS_PROPERTY_DECLARATION, 
            Count = 1)]
        public virtual void NoFillRectangleTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "noFillRectangle");
        }

        [NUnit.Framework.Test]
        public virtual void RectangleNoWidthNoHeightTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "rectangleNoWidthNoHeight");
        }
    }
}
