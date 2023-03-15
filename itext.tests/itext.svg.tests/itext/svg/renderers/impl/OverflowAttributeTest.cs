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
using iText.Svg.Processors;
using iText.Svg.Processors.Impl;
using iText.Svg.Renderers;
using iText.Test;

namespace iText.Svg.Renderers.Impl {
    [NUnit.Framework.Category("IntegrationTest")]
    public class OverflowAttributeTest : SvgIntegrationTest {
        private static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/svg/renderers/impl/OverflowAttributeTest/";

        private static readonly String DESTINATION_FOLDER = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/svg/renderers/impl/OverflowAttributeTest/";

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
        public virtual void OverflowVisibleInMarkerElementTest() {
            //TODO: update when DEVSIX-3482 fixed
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "overflowVisibleInMarkerElement");
        }

        [NUnit.Framework.Test]
        public virtual void OverflowHiddenInMarkerElementTest() {
            //TODO: update when DEVSIX-3482 fixed
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "overflowHiddenInMarkerElement");
        }

        [NUnit.Framework.Test]
        public virtual void OverflowAutoInMarkerElementTest() {
            //TODO: update when DEVSIX-3482 fixed
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "overflowAutoInMarkerElement");
        }

        [NUnit.Framework.Test]
        public virtual void OverflowScrollInMarkerElementTest() {
            //TODO: update when DEVSIX-3482 fixed
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "overflowScrollInMarkerElement");
        }

        [NUnit.Framework.Test]
        public virtual void OverflowInitialInMarkerElementTest() {
            //TODO: update when DEVSIX-3482 fixed
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "overflowInitialInMarkerElement");
        }

        [NUnit.Framework.Test]
        public virtual void OverflowVisibleInSymbolElementTest() {
            //TODO: update when DEVSIX-3482 fixed
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "overflowVisibleInSymbol");
        }

        [NUnit.Framework.Test]
        public virtual void OverflowHiddenInSymbolElementTest() {
            //TODO: update when DEVSIX-3482 fixed
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "overflowHiddenInSymbol");
        }

        [NUnit.Framework.Test]
        public virtual void OverflowInitialInSymbolElementTest() {
            //TODO: update when DEVSIX-3482 fixed
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "overflowInitialInSymbol");
        }

        [NUnit.Framework.Test]
        public virtual void OverflowScrollInSymbolElementTest() {
            //TODO: update when DEVSIX-3482 fixed
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "overflowScrollInSymbol");
        }

        [NUnit.Framework.Test]
        public virtual void OverflowInSvgElementTest() {
            //TODO: update when DEVSIX-3482 fixed
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "overflowInSvgElement");
        }
    }
}
