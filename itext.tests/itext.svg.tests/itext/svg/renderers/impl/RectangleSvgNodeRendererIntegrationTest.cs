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
using iText.Svg.Renderers;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Svg.Renderers.Impl {
    [NUnit.Framework.Category("IntegrationTest")]
    public class RectangleSvgNodeRendererIntegrationTest : SvgIntegrationTest {
        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/svg/renderers/impl/RectangleSvgNodeRendererTest/";

        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/svg/renderers/impl/RectangleSvgNodeRendererTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            ITextTest.CreateDestinationFolder(destinationFolder);
        }

        [NUnit.Framework.Test]
        public virtual void BasicRectangleTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "basicRectangle");
        }

        [NUnit.Framework.Test]
        public virtual void BasicRectangleRxRyZeroTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "basicRectangleRxRyZero");
        }

        [NUnit.Framework.Test]
        public virtual void BasicCircularRoundedRectangleRyZeroTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "basicCircularRoundedRectangleRyZero");
        }

        [NUnit.Framework.Test]
        public virtual void BasicCircularRoundedRectangleRxZeroTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "basicCircularRoundedRectangleRxZero");
        }

        [NUnit.Framework.Test]
        public virtual void BasicCircularRoundedRxRectangleTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "basicCircularRoundedRxRectangle");
        }

        [NUnit.Framework.Test]
        public virtual void BasicCircularRoundedRyRectangleTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "basicCircularRoundedRyRectangle");
        }

        [NUnit.Framework.Test]
        public virtual void BasicEllipticalRoundedRectangleXTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "basicEllipticalRoundedRectangleX");
        }

        [NUnit.Framework.Test]
        public virtual void BasicEllipticalRoundedRectangleYTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "basicEllipticalRoundedRectangleY");
        }

        [NUnit.Framework.Test]
        public virtual void BasicEllipticalWidthCappedRoundedRectangleTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "basicEllipticalWidthCappedRoundedRectangle");
        }

        [NUnit.Framework.Test]
        public virtual void BasicEllipticalHeightCappedRoundedRectangleTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "basicEllipticalHeightCappedRoundedRectangle");
        }

        [NUnit.Framework.Test]
        public virtual void BasicEllipticalNegativeWidthRoundedRectangleTest() {
            //TODO change cmp-file after DEVSIX-3121 fixed
            ConvertAndCompare(sourceFolder, destinationFolder, "basicEllipticalNegativeWidthRoundedRectangle");
        }

        [NUnit.Framework.Test]
        public virtual void BasicEllipticalNegativeHeightRoundedRectangleTest() {
            //TODO change cmp-file after DEVSIX-3121 fixed
            ConvertAndCompare(sourceFolder, destinationFolder, "basicEllipticalNegativeHeightRoundedRectangle");
        }

        [NUnit.Framework.Test]
        public virtual void ComplexRectangleTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "complexRectangle");
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.StyledXmlParser.Logs.StyledXmlParserLogMessageConstant.INVALID_CSS_PROPERTY_DECLARATION, 
            Count = 1)]
        public virtual void NoFillRectangleTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "noFillRectangle");
        }

        [NUnit.Framework.Test]
        public virtual void RectangleNoWidthNoHeightTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "rectangleNoWidthNoHeight");
        }
    }
}
