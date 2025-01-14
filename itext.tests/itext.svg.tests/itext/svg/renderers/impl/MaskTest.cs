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
using iText.Svg.Processors;
using iText.Svg.Processors.Impl;
using iText.Svg.Renderers;
using iText.Test;

namespace iText.Svg.Renderers.Impl {
    [NUnit.Framework.Category("IntegrationTest")]
    public class MaskTest : SvgIntegrationTest {
        private static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/svg/renderers/impl/MaskTest/";

        private static readonly String DESTINATION_FOLDER = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/svg/renderers/impl/MaskTest/";

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
        public virtual void MaskBasic() {
            //TODO: update after DEVSIX-2378 implementation
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "maskBasic", properties);
        }

        [NUnit.Framework.Test]
        public virtual void MaskWithGradient() {
            //TODO: update after DEVSIX-2378 implementation
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "maskWithGradient", properties);
        }

        [NUnit.Framework.Test]
        public virtual void MaskContentUnitsTest() {
            //TODO: update after DEVSIX-2378 implementation
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "mask-content-units", properties);
        }

        [NUnit.Framework.Test]
        public virtual void MaskPatternCombiTest() {
            //TODO: update after DEVSIX-2378 implementation
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "maskPatternCombi", properties);
        }

        [NUnit.Framework.Test]
        public virtual void MaskPatternAppliedTest() {
            //TODO: update after DEVSIX-2378 implementation
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "maskPatternApplied", properties);
        }

        [NUnit.Framework.Test]
        public virtual void MaskMultiShapesTest() {
            //TODO: update after DEVSIX-2378 implementation
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "maskMultiShapes", properties);
        }

        [NUnit.Framework.Test]
        public virtual void MaskPatternGradientAppliedTest() {
            //TODO: update after DEVSIX-2378 implementation
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "maskPatternGradientApplied", properties);
        }

        [NUnit.Framework.Test]
        public virtual void MaskGradientAppliedMaskContentUnitsTest() {
            //TODO: update after DEVSIX-2378 implementation
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "maskGradientAppliedMaskContentUnits", properties
                );
        }

        [NUnit.Framework.Test]
        public virtual void MaskPatternMaskContentUnitsUserSpaceOnUseTest() {
            //TODO: update after DEVSIX-2378 implementation
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "maskPatternMaskContentUnitsUserSpaceOnUse"
                , properties);
        }

        [NUnit.Framework.Test]
        public virtual void MaskUnitsObjectBoundingBoxTest() {
            //TODO: update after DEVSIX-2378 implementation
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "maskUnitsObjectBoundingBox", properties);
        }

        [NUnit.Framework.Test]
        public virtual void MaskUnitsUserSpaceOnUseTest() {
            //TODO: update after DEVSIX-2378 implementation
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "maskUnitsUserSpaceOnUse", properties);
        }

        [NUnit.Framework.Test]
        public virtual void MaskTransformTest() {
            //TODO: update after DEVSIX-2378 implementation
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "maskTransform", properties);
        }

        [NUnit.Framework.Test]
        public virtual void MaskTransform2Test() {
            //TODO: update after DEVSIX-2378 implementation
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "maskTransform2", properties);
        }

        [NUnit.Framework.Test]
        public virtual void MaskTransform3Test() {
            //TODO: update after DEVSIX-2378 implementation
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "maskTransform3", properties);
        }

        [NUnit.Framework.Test]
        public virtual void MaskInheritedBasicTest() {
            //TODO: update after DEVSIX-2378 implementation
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "maskInheritedBasic", properties);
        }

        [NUnit.Framework.Test]
        public virtual void MaskInherited3LevelTest() {
            //TODO: update after DEVSIX-2378 implementation
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "maskInherited3Level", properties);
        }

        [NUnit.Framework.Test]
        public virtual void MaskPatternAppliedInheritedTest() {
            //TODO: update after DEVSIX-2378 implementation
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "maskPatternAppliedInherited", properties);
        }

        [NUnit.Framework.Test]
        public virtual void MaskUnitsObjectBoundingBoxInheritedTest() {
            //TODO: update after DEVSIX-2378 implementation
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "maskUnitsObjectBoundingBoxInherited", properties
                );
        }

        [NUnit.Framework.Test]
        public virtual void MaskUnitsUserSpaceOnUseInherited2Test() {
            //TODO: update after DEVSIX-2378 implementation
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "maskUnitsUserSpaceOnUseInherited2", properties
                );
        }

        [NUnit.Framework.Test]
        public virtual void MaskUnitsUserSpaceOnUseInheritedTest() {
            //TODO: update after DEVSIX-2378 implementation
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "maskUnitsUserSpaceOnUseInherited", properties
                );
        }

        [NUnit.Framework.Test]
        public virtual void MaskUnitsObjectBoundingBoxInherited2Test() {
            //TODO: update after DEVSIX-2378 implementation
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "maskUnitsObjectBoundingBoxInherited2", properties
                );
        }

        [NUnit.Framework.Test]
        public virtual void MaskTransformInheritedTest() {
            //TODO: update after DEVSIX-2378 implementation
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "maskTransformInherited", properties);
        }
    }
}
