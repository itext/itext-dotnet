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
    public class VisibilityTest : SvgIntegrationTest {
        public static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/svg/css/VisibilityTest/";

        public static readonly String DESTINATION_FOLDER = TestUtil.GetOutputPath() + "/svg/css/VisibilityTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            ITextTest.CreateDestinationFolder(DESTINATION_FOLDER);
        }

        [NUnit.Framework.Test]
        public virtual void VisibilityDiffValuesTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "visibility");
        }

        [NUnit.Framework.Test]
        public virtual void VisibilityDiffValuesTest2() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "visibility2");
        }

        [NUnit.Framework.Test]
        public virtual void VisibilityHiddenInLinearGradientTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "visibilityHiddenInLinearGradient");
        }

        [NUnit.Framework.Test]
        public virtual void VisibilityHiddenInMarkerTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "visibilityHiddenInMarker");
        }

        [NUnit.Framework.Test]
        public virtual void VisibilityHiddenInGTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "visibilityHiddenInG");
        }

        [NUnit.Framework.Test]
        public virtual void VisibilityHiddenInUseTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "visibilityHiddenInUse");
        }

        [NUnit.Framework.Test]
        public virtual void VisibilityHiddenInDefsTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "visibilityHiddenInDefs");
        }

        [NUnit.Framework.Test]
        public virtual void VisibilityHiddenInPatternTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "visibilityHiddenInPattern");
        }

        [NUnit.Framework.Test]
        public virtual void VisibilityHiddenInClipPathTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "visibilityHiddenInClipPath");
        }

        [NUnit.Framework.Test]
        public virtual void VisibilityHiddenInClipPathWithUseTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "visibilityHiddenInClipPathWithUse");
        }

        [NUnit.Framework.Test]
        public virtual void VisibilityHiddenInSymbolTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "visibilityHiddenInSymbol");
        }

        [NUnit.Framework.Test]
        public virtual void DisplayNoneInLinearGradientTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "displayNoneInLinearGradient");
        }

        [NUnit.Framework.Test]
        public virtual void DisplayNoneInMarkerTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "displayNoneInMarker");
        }

        [NUnit.Framework.Test]
        public virtual void DisplayNoneInGTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "displayNoneInG");
        }

        [NUnit.Framework.Test]
        public virtual void DisplayNoneInG2Test() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "displayNoneInG2");
        }

        [NUnit.Framework.Test]
        public virtual void DisplayNoneInUseTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "displayNoneInUse");
        }

        [NUnit.Framework.Test]
        public virtual void DisplayNoneInDefsTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "displayNoneInDefs");
        }

        [NUnit.Framework.Test]
        public virtual void DisplayNoneInPatternTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "displayNoneInPattern");
        }

        [NUnit.Framework.Test]
        public virtual void DisplayNoneInSymbolTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "displayNoneInSymbol");
        }

        [NUnit.Framework.Test]
        public virtual void DisplayNoneInClipPathTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "displayNoneInClipPath");
        }

        [NUnit.Framework.Test]
        public virtual void DisplayNoneInClipPathWithUseTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "displayNoneInClipPathWithUse");
        }
    }
}
