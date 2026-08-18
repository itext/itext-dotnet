/*
This file is part of the iText (R) project.
Copyright (c) 1998-2026 Apryse Group NV
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
    public class DefaultStyleInheritanceIntegrationTest : SvgIntegrationTest {
        private static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/svg/css/DefaultInheritance/";

        private static readonly String DESTINATION_FOLDER = TestUtil.GetOutputPath() + "/svg/css/DefaultInheritance/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            ITextTest.CreateDestinationFolder(DESTINATION_FOLDER);
        }

        //Css inheritance
        [NUnit.Framework.Test]
        public virtual void SimpleGroupInheritanceTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "simpleGroupInheritance");
        }

        //Inheritance in use tags
        [NUnit.Framework.Test]
        public virtual void UseFillInheritanceTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "useFillInheritance");
        }

        //Inheritance and g-tags
        [NUnit.Framework.Test]
        public virtual void GroupInheritanceTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "groupInheritance");
        }

        [NUnit.Framework.Test]
        public virtual void UseInheritanceNotOverridingTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "useInheritanceNotOverriding");
        }

        [NUnit.Framework.Test]
        public virtual void UsePropertiesInheritanceTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "usePropertiesInheritance");
        }

        [NUnit.Framework.Test]
        public virtual void FillOpacityInheritanceTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "fillOpacityInheritance");
        }

        [NUnit.Framework.Test]
        public virtual void FillRuleInheritanceTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "fillRuleInheritance");
        }

        [NUnit.Framework.Test]
        public virtual void StrokeInheritanceTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "strokeInheritance");
        }

        [NUnit.Framework.Test]
        public virtual void StrokeWidthInheritanceTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "strokeWidthInheritance");
        }

        [NUnit.Framework.Test]
        public virtual void StrokeOpacityInheritanceTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "strokeOpacityInheritance");
        }

        [NUnit.Framework.Test]
        public virtual void StrokeLinecapInheritanceTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "strokeLinecapInheritance");
        }

        [NUnit.Framework.Test]
        public virtual void StrokeLinejoinInheritanceTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "strokeLinejoinInheritance");
        }

        [NUnit.Framework.Test]
        public virtual void StrokeDasharrayInheritanceTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "strokeDasharrayInheritance");
        }

        [NUnit.Framework.Test]
        public virtual void StrokeDashoffsetInheritanceTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "strokeDashoffsetInheritance");
        }

        [NUnit.Framework.Test]
        public virtual void OpacityInheritanceTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "opacityInheritance");
        }

        [NUnit.Framework.Test]
        public virtual void TextPropertiesInheritanceTest() {
            //TODO DEVSIX-4114 support vertical text attribute
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "textPropertiesInheritance");
        }

        [NUnit.Framework.Test]
        public virtual void NestedInheritanceTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "nestedInheritance");
        }
    }
}
