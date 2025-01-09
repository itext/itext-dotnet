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
    public class BaselineTest : SvgIntegrationTest {
        public static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/svg/css/BaselineTest/";

        public static readonly String DESTINATION_FOLDER = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/svg/css/BaselineTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            ITextTest.CreateDestinationFolder(DESTINATION_FOLDER);
        }

        [NUnit.Framework.Test]
        public virtual void AlignmentBaselineTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "alignment-base");
        }

        [NUnit.Framework.Test]
        public virtual void BaselineShiftTest() {
            //TODO DEVSIX-2507: change cmp file after supporting
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "baseline-shift");
        }

        [NUnit.Framework.Test]
        public virtual void DominantBaselineTest() {
            //TODO DEVSIX-5890: update cmp file after supporting
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "dominant-baseline");
        }

        //TODO DEVSIX-2507: Update cmp file after supporting
        [NUnit.Framework.Test]
        public virtual void DominantBaselineTspanTest() {
            //TODO DEVSIX-5890: update cmp file after supporting
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "dominant-baseline-tspan");
        }
    }
}
