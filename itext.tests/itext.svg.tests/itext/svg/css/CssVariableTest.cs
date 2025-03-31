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
    public class CssVariableTest : SvgIntegrationTest {
        private static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/svg/css/CssVariableTest/";

        private static readonly String DESTINATION_FOLDER = TestUtil.GetOutputPath() + "/svg/css/CssVariableTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            ITextTest.CreateDestinationFolder(DESTINATION_FOLDER);
        }

        [NUnit.Framework.Test]
        public virtual void CircleWithVariablesTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "circleWithVariables");
        }

        [NUnit.Framework.Test]
        public virtual void CircleWithVariablesInDefsTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "circleWithVariablesInDefs");
        }

        [NUnit.Framework.Test]
        public virtual void CircleWithVariablesInDefsWithInnerSvgTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "circleWithVariablesInDefsWithInnerSvg");
        }

        [NUnit.Framework.Test]
        public virtual void SvgWithVariablesInShorthandTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "svgWithVariablesInShorthand");
        }

        [NUnit.Framework.Test]
        public virtual void SvgWithVariablesAsShorthandTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "svgWithVariablesAsShorthand");
        }

        [NUnit.Framework.Test]
        public virtual void RootSelectorVariablesTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "rootSelectorVariables");
        }

        [NUnit.Framework.Test]
        public virtual void VariablesInStyleAttributeTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "variablesInStyleAttribute");
        }

        [NUnit.Framework.Test]
        public virtual void SymbolInheritanceTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "symbolInheritance");
        }
    }
}
