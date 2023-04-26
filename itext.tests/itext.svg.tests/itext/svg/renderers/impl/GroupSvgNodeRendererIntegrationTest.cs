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
using iText.Svg.Processors.Impl;
using iText.Svg.Renderers;
using iText.Test;

namespace iText.Svg.Renderers.Impl {
    [NUnit.Framework.Category("IntegrationTest")]
    public class GroupSvgNodeRendererIntegrationTest : SvgIntegrationTest {
        private static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/svg/renderers/impl/GroupRendererTest/";

        private static readonly String DESTINATION_FOLDER = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/svg/renderers/impl/GroupRendererTest/";

        private SvgConverterProperties properties;

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            ITextTest.CreateDestinationFolder(DESTINATION_FOLDER);
        }

        [NUnit.Framework.Test]
        public virtual void NestedGroupReuseTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "nestedGroupReuse");
        }

        [NUnit.Framework.Test]
        public virtual void NestedGroupWithoutReuseTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "nestedGroupWithoutReuse");
        }

        [NUnit.Framework.Test]
        public virtual void SimpleGroupReuseTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "simpleGroupReuse");
        }

        [NUnit.Framework.Test]
        public virtual void NestedGroupTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "nestedGroup");
        }

        [NUnit.Framework.Test]
        public virtual void OverlayingGroupsTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "overlayingGroups");
        }

        [NUnit.Framework.Test]
        public virtual void OverlappingBorderTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "overlappingBorder");
        }

        [NUnit.Framework.Test]
        public virtual void MoreOverlappingBorderTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "moreOverlappingBorder");
        }

        [NUnit.Framework.Test]
        public virtual void MoreOverlappingBorderWithCenterSquareTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "moreOverlappingBorderWithCenterSquare");
        }

        [NUnit.Framework.Test]
        public virtual void MoreOverlappingBorderWithTwoSideSquaresTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "moreOverlappingBorderWithTwoSideSquares");
        }

        [NUnit.Framework.Test]
        public virtual void CompleteOverlappingBorderTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "completeOverlappingBorder");
        }

        [NUnit.Framework.Test]
        public virtual void TranslatedGroupTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "translated");
        }

        [NUnit.Framework.Test]
        public virtual void MultipleTransformationsTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "multipleTransformations");
        }

        [NUnit.Framework.Test]
        public virtual void FillGradientTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "fillGradient");
        }
    }
}
