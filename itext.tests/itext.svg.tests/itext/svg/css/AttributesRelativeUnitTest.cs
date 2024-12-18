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
using iText.Svg.Processors.Impl;
using iText.Svg.Renderers;
using iText.Test;

namespace iText.Svg.Css {
    public class AttributesRelativeUnitTest : SvgIntegrationTest {
        public static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/svg/css/AttributesRelativeUnitTest/";

        public static readonly String DESTINATION_FOLDER = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/svg/css/AttributesRelativeUnitTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            ITextTest.CreateDestinationFolder(DESTINATION_FOLDER);
        }

        [NUnit.Framework.Test]
        public virtual void RectangleAttributesEmUnitsTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "rectangleAttributesEmUnits");
        }

        [NUnit.Framework.Test]
        public virtual void RectangleAttributesExUnitsTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "rectangleAttributesExUnits");
        }

        [NUnit.Framework.Test]
        public virtual void RectangleAttributesPercentUnitsTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "rectangleAttributesPercentUnits");
        }

        [NUnit.Framework.Test]
        public virtual void ImageAttributesEmUnitsTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "imageAttributesEmUnits");
        }

        [NUnit.Framework.Test]
        public virtual void ImageAttributesExUnitsTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "imageAttributesExUnits");
        }

        [NUnit.Framework.Test]
        public virtual void ImageAttributesPercentUnitsTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "imageAttributesPercentUnits");
        }

        //-------------- Nested svg
        [NUnit.Framework.Test]
        public virtual void NestedSvgWidthPercentTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "nestedSvgWidthPercentTest");
        }

        [NUnit.Framework.Test]
        public virtual void NestedSvgXPercentTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "nestedSvgXPercentTest");
        }

        [NUnit.Framework.Test]
        public virtual void NestedSvgHeightPercentTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "nestedSvgHeightPercentTest");
        }

        [NUnit.Framework.Test]
        public virtual void NestedSvgYPercentTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "nestedSvgYPercentTest");
        }

        [NUnit.Framework.Test]
        public virtual void NestedSvgWidthEmTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "nestedSvgWidthEmTest");
        }

        [NUnit.Framework.Test]
        public virtual void NestedSvgAllPercentTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "nestedSvgAllPercentTest");
        }

        //-------------- Top level svg
        [NUnit.Framework.Test]
        public virtual void SvgWidthPercentTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "svgWidthPercentTest");
        }

        [NUnit.Framework.Test]
        public virtual void SvgViewboxWidthPercentTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "svgViewboxWidthPercentTest");
        }

        [NUnit.Framework.Test]
        public virtual void SvgHeightPercentTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "svgHeightPercentTest");
        }

        [NUnit.Framework.Test]
        public virtual void SvgWidthAndHeightEmAndRemTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "svgWidthAndHeightEmAndRemTest");
        }

        //-------------- use
        [NUnit.Framework.Test]
        public virtual void UseXPercentTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "useXPercentTest");
        }

        [NUnit.Framework.Test]
        public virtual void UseYPercentTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "useYPercentTest");
        }

        [NUnit.Framework.Test]
        public virtual void UseXAndYEmAndRemTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "useXAndYEmAndRemTest");
        }

        [NUnit.Framework.Test]
        public virtual void UseWidthPercentTest() {
            // TODO DEVSIX-4566 Processing of width&height attributes in use tag are not currently supported
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "useWidthPercentTest");
        }

        //-------------- symbol
        [NUnit.Framework.Test]
        public virtual void SymbolXPercentTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "symbolXPercentTest");
        }

        [NUnit.Framework.Test]
        public virtual void SymbolYPercentTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "symbolYPercentTest");
        }

        [NUnit.Framework.Test]
        public virtual void SymbolXAndYEmAndRemTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "symbolXAndYEmAndRemTest");
        }

        [NUnit.Framework.Test]
        public virtual void SymbolWidthAndHeightPercentTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "symbolWidthAndHeightPercentTest");
        }

        [NUnit.Framework.Test]
        public virtual void SymbolWidthAndHeightEmAndRemTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "symbolWidthAndHeightEmAndRemTest");
        }

        //-------------- misc
        [NUnit.Framework.Test]
        public virtual void LinePercentTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "linePercentTest");
        }

        [NUnit.Framework.Test]
        public virtual void DiffViewBoxAndPortPercent1Test() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "diffViewBoxAndPortPercent1Test");
        }

        [NUnit.Framework.Test]
        public virtual void DiffViewBoxAndPortPercent2Test() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "diffViewBoxAndPortPercent2Test");
        }

        [NUnit.Framework.Test]
        public virtual void NoViewBoxAndViewPortPercentTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "noViewBoxAndViewPortPercentTest");
        }

        [NUnit.Framework.Test]
        public virtual void NoViewBoxPercentTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "noViewBoxPercentTest");
        }

        [NUnit.Framework.Test]
        public virtual void ViewportFromConverterPropertiesTest() {
            SvgConverterProperties properties = new SvgConverterProperties();
            properties.SetCustomViewport(new Rectangle(500, 500));
            // It is expected that the result is different with browser. In
            // browsers the result should be bigger but with the same proportions
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "viewportFromConverterPropertiesTest", properties);
        }
    }
}
