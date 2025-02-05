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

namespace iText.Svg.Renderers.Impl {
    [NUnit.Framework.Category("IntegrationTest")]
    public class ViewBoxSvgTagSvgNodeRendererIntegrationTest : SvgIntegrationTest {
        private static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/svg/renderers/impl/RootSvgNodeRendererTest/viewbox/";

        private static readonly String DESTINATION_FOLDER = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/svg/renderers/impl/RootSvgNodeRendererTest/viewbox/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            ITextTest.CreateDestinationFolder(DESTINATION_FOLDER);
        }

        //Uniform viewboxes
        [NUnit.Framework.Test]
        public virtual void ViewBox100x100() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "viewbox_100x100");
        }

        [NUnit.Framework.Test]
        public virtual void ViewBox200x200() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "viewbox_200x200");
        }

        [NUnit.Framework.Test]
        public virtual void ViewBox400x400() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "viewbox_400x400");
        }

        //Non-uniform viewboxes
        [NUnit.Framework.Test]
        public virtual void ViewBox100x200() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "viewbox_100x200");
        }

        [NUnit.Framework.Test]
        public virtual void ViewBox100x400() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "viewbox_100x400");
        }

        [NUnit.Framework.Test]
        public virtual void ViewBox200x100() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "viewbox_200x100");
        }

        [NUnit.Framework.Test]
        public virtual void ViewBox200x400() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "viewbox_200x400");
        }

        [NUnit.Framework.Test]
        public virtual void ViewBox400x100() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "viewbox_400x100");
        }

        [NUnit.Framework.Test]
        public virtual void ViewBox400x200() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "viewbox_400x200");
        }

        [NUnit.Framework.Test]
        public virtual void ViewBoxXYValuesPreserveAspectRatioNoneValues() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "viewBoxXYValuesPreserveAspectRatioNoneValues"
                );
        }

        [NUnit.Framework.Test]
        public virtual void ViewBoxXYValuesPreserveAspectRatioXMaxYMaxMeetValues() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "viewBoxXYValuesPreserveAspectRatioXMaxYMaxMeetValues"
                );
        }

        [NUnit.Framework.Test]
        public virtual void ViewBoxXYValuesPreserveAspectRatioXMaxYMaxSliceValues() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "viewBoxXYValuesPreserveAspectRatioXMaxYMaxSliceValues"
                );
        }

        [NUnit.Framework.Test]
        public virtual void PreserveAspectRationAllOptionsTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "preserveAspectRationAllOptions");
        }
    }
}
