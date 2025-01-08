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
using iText.Svg.Processors;
using iText.Svg.Processors.Impl;
using iText.Svg.Renderers;
using iText.Test;

namespace iText.Svg.Renderers.Impl {
    [NUnit.Framework.Category("IntegrationTest")]
    public class SymbolTest : SvgIntegrationTest {
        public static readonly String DESTINATION_FOLDER = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/svg/renderers/impl/SymbolTest/";

        public static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/svg/renderers/impl/SymbolTest/";

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
        public virtual void SimpleSymbolTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "simpleSymbolTest");
        }

        [NUnit.Framework.Test]
        public virtual void UseTagFirstSymbolAfterTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "useTagFirstSymbolAfterTest");
        }

        [NUnit.Framework.Test]
        public virtual void HeightPxAttrTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "heightPxAttrTest");
        }

        [NUnit.Framework.Test]
        public virtual void HeightPercentsAttrTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "heightPercentsAttrTest");
        }

        [NUnit.Framework.Test]
        public virtual void HeightEmTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "heightEmTest");
        }

        [NUnit.Framework.Test]
        public virtual void WidthPxAttrTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "widthPxAttrTest");
        }

        [NUnit.Framework.Test]
        public virtual void WidthPercentsAttrTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "widthPercentsAttrTest");
        }

        [NUnit.Framework.Test]
        public virtual void WidthHeightAttrPxTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "widthHeightAttrPxTest");
        }

        [NUnit.Framework.Test]
        public virtual void WidthHeightAttrPercentsPxTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "widthHeightAttrPercentsPxTest");
        }

        [NUnit.Framework.Test]
        public virtual void PreserveAspectRatioViewBoxTest() {
            // TODO DEVSIX-3537 Processing of preserveAspectRatio attribute with offsets x and y is not currently supported
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "preserveAspectRatioViewBoxTest");
        }

        [NUnit.Framework.Test]
        public virtual void XYInUseWithDefsTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "xYInUseWithDefsTest");
        }

        [NUnit.Framework.Test]
        public virtual void ClassAttributeTestWithCssTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "classAttrTest");
        }

        [NUnit.Framework.Test]
        public virtual void StyleAttributeTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "styleAttrTest");
        }

        [NUnit.Framework.Test]
        public virtual void StyleAttrInUseTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "styleAttrInUseTest");
        }

        [NUnit.Framework.Test]
        public virtual void BothStyleAttributeTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "bothStyleAttrTest");
        }

        [NUnit.Framework.Test]
        public virtual void OpacityAttributeTest() {
            // TODO DEVSIX-2258 Processing of stroke attribute is not currently correct supported
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "opacityAttrTest");
        }

        [NUnit.Framework.Test]
        public virtual void VisibilityAttributeTest() {
            // TODO DEVSIX-2254 Processing of visibility attribute is not currently supported
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "visibilityAttrTest");
        }

        [NUnit.Framework.Test]
        public virtual void DisplayNoneAttributeTest() {
            // TODO DEVSIX-4564 Processing of display attribute is not currently supported
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "displayNoneAttrTest");
        }

        [NUnit.Framework.Test]
        public virtual void DisplayAttributeWithNoUseTagTest() {
            // TODO DEVSIX-4564 Processing of display attribute is not currently supported
            //Expects that nothing will be displayed on the page as it's done in Chrome browser
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "displayAttrWithNoUseTagTest");
        }

        [NUnit.Framework.Test]
        public virtual void SimpleImageTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "simpleImageTest", properties);
        }

        [NUnit.Framework.Test]
        public virtual void LinearGradientSymbolTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "linearGradientSymbolTest", properties);
        }

        [NUnit.Framework.Test]
        public virtual void UseHeightWidthAllUnitsTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "useHeightWidthAllUnitsTest", properties);
        }

        [NUnit.Framework.Test]
        public virtual void UseSymbolHeightWidthAllUnitsTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "useSymbolHeightWidthAllUnitsTest", properties
                );
        }

        [NUnit.Framework.Test]
        public virtual void UseSymbolXYContrudictionAllUnitsTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "useSymbolXYContrudictionAllUnitsTest", properties
                );
        }

        [NUnit.Framework.Test]
        public virtual void UseSymbolCoordinatesContrudictionTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "useSymbolCoordinatesContrudiction", properties
                );
        }

        [NUnit.Framework.Test]
        public virtual void WidthHeightAttrInteractionTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "widthHeightAttrInteraction", properties);
        }
    }
}
