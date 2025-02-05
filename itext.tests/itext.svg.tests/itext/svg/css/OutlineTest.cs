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

namespace iText.Svg.Css {
    [NUnit.Framework.Category("IntegrationTest")]
    public class OutlineTest : SvgIntegrationTest {
        //TODO DEVSIX-4044: Update cmp files
        private static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/svg/css/OutlineTest/";

        private static readonly String DESTINATION_FOLDER = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/svg/css/OutlineTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            ITextTest.CreateDestinationFolder(DESTINATION_FOLDER);
        }

        [NUnit.Framework.Test]
        public virtual void OutlineShortHandTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "outlineShortHand");
        }

        [NUnit.Framework.Test]
        public virtual void OutlineColorTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "outlineColor");
        }

        [NUnit.Framework.Test]
        public virtual void OutlineWidthGlobalTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "outlineWidthGlobal");
        }

        [NUnit.Framework.Test]
        public virtual void OutlineOffsetTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "outlineOffset");
        }

        [NUnit.Framework.Test]
        public virtual void OutlineStylesTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "outlineStyles");
        }

        [NUnit.Framework.Test]
        public virtual void OutlineWidthTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "outlineWidth");
        }

        [NUnit.Framework.Test]
        public virtual void OutlineSimpleTopLevelTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "outlineSimpleTopLevel");
        }

        [NUnit.Framework.Test]
        public virtual void OutlineWidthSeparateTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "outlineWidthSeparate");
        }

        [NUnit.Framework.Test]
        public virtual void OutlineStylesSeparateTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "outlineStylesSeparate");
        }

        [NUnit.Framework.Test]
        public virtual void OutlineOffsetSeparateTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "outlineOffsetSeparate");
        }

        [NUnit.Framework.Test]
        public virtual void OutlineWidthGlobalSeparateTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "outlineWidthGlobalSeparate");
        }

        [NUnit.Framework.Test]
        public virtual void OutlineShortHandSeparateTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "outlineShortHandSeparate");
        }
    }
}
