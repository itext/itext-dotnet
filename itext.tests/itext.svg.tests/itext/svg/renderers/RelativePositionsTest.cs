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
    public class RelativePositionsTest : SvgIntegrationTest {
        private static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/svg/renderers/impl/RelativePositionsTest/";

        private static readonly String DESTINATION_FOLDER = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/svg/renderers/impl/RelativePositionsTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            ITextTest.CreateDestinationFolder(DESTINATION_FOLDER);
        }

        //TODO DEVSIX-5740: Update cmp file after supporting
        [NUnit.Framework.Test]
        public virtual void RelativePositionsBasic1Test() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "relativePositionsBasic");
        }

        //TODO DEVSIX-5740: Update cmp file after supporting
        [NUnit.Framework.Test]
        public virtual void RelativePositionsBasic2Test() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "relativePositionsBasic2");
        }

        //TODO DEVSIX-5740: Update cmp file after supporting
        [NUnit.Framework.Test]
        public virtual void RelativePositionsListValuesTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "relativePositionsListValues");
        }

        //TODO DEVSIX-5740: Update cmp file after supporting
        [NUnit.Framework.Test]
        public virtual void RelativePositionsListValuesNoXTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "relativePositionsListValuesNoX");
        }

        //TODO DEVSIX-5740: Update cmp file after supporting
        [NUnit.Framework.Test]
        public virtual void RelativePositionsListValuesNoYTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "relativePositionsListValuesNoY");
        }
    }
}
