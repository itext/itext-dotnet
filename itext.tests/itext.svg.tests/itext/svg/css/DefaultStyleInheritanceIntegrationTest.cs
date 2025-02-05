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
    public class DefaultStyleInheritanceIntegrationTest : SvgIntegrationTest {
        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/svg/css/DefaultInheritance/";

        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/svg/css/DefaultInheritance/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            ITextTest.CreateDestinationFolder(destinationFolder);
        }

        //Css inheritance
        [NUnit.Framework.Test]
        public virtual void SimpleGroupInheritanceTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "simpleGroupInheritance");
        }

        //Inheritance in use tags
        [NUnit.Framework.Test]
        public virtual void UseFillInheritanceTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "useFillInheritance");
        }

        //Inheritance and g-tags
        [NUnit.Framework.Test]
        public virtual void GroupInheritanceTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "groupInheritance");
        }

        [NUnit.Framework.Test]
        public virtual void UseInheritanceNotOverridingTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "useInheritanceNotOverriding");
        }

        [NUnit.Framework.Test]
        public virtual void UsePropertiesInheritanceTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "usePropertiesInheritance");
        }

        [NUnit.Framework.Test]
        public virtual void FillOpacityInheritanceTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "fillOpacityInheritance");
        }

        [NUnit.Framework.Test]
        public virtual void FillRuleInheritanceTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "fillRuleInheritance");
        }

        [NUnit.Framework.Test]
        public virtual void StrokeInheritanceTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "strokeInheritance");
        }

        [NUnit.Framework.Test]
        public virtual void StrokeWidthInheritanceTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "strokeWidthInheritance");
        }

        [NUnit.Framework.Test]
        public virtual void StrokeOpacityInheritanceTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "strokeOpacityInheritance");
        }

        [NUnit.Framework.Test]
        public virtual void StrokeLinecapInheritanceTest() {
            //TODO DEVSIX-4857 support stroke-linecap attribute
            ConvertAndCompare(sourceFolder, destinationFolder, "strokeLinecapInheritance");
        }

        [NUnit.Framework.Test]
        public virtual void StrokeLinejoinInheritanceTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "strokeLinejoinInheritance");
        }

        [NUnit.Framework.Test]
        public virtual void StrokeDasharrayInheritanceTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "strokeDasharrayInheritance");
        }

        [NUnit.Framework.Test]
        public virtual void StrokeDashoffsetInheritanceTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "strokeDashoffsetInheritance");
        }

        [NUnit.Framework.Test]
        public virtual void OpacityInheritanceTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "opacityInheritance");
        }

        [NUnit.Framework.Test]
        public virtual void TextPropertiesInheritanceTest() {
            //TODO DEVSIX-4114 support vertical text attribute
            ConvertAndCompare(sourceFolder, destinationFolder, "textPropertiesInheritance");
        }

        [NUnit.Framework.Test]
        public virtual void NestedInheritanceTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "nestedInheritance");
        }
    }
}
