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
using iText.Test;

namespace iText.Svg.Renderers {
    [NUnit.Framework.Category("IntegrationTest")]
    public class WhiteSpaceTest : SvgIntegrationTest {
        //TODO DEVSIX-2284: Update cmp file after supporting
        private static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/svg/renderers/impl/WhiteSpaceTest/";

        private static readonly String DESTINATION_FOLDER = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/svg/renderers/impl/WhiteSpaceTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            ITextTest.CreateDestinationFolder(DESTINATION_FOLDER);
        }

        [NUnit.Framework.Test]
        public virtual void WhiteSpacexLinkBasicTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "whiteSpace");
        }

        [NUnit.Framework.Test]
        public virtual void WhiteSpaceBasicTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "white-space-basic");
        }

        [NUnit.Framework.Test]
        public virtual void WhiteSpaceBasicTspanTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "white-space-basic-tspan");
        }

        [NUnit.Framework.Test]
        public virtual void WhiteSpaceNestedTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "white-space-nested");
        }

        [NUnit.Framework.Test]
        public virtual void WhiteSpaceRelativePositionsTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "white-space-basic-relative-positions");
        }
    }
}
