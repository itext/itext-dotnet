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
using iText.Svg.Renderers;
using iText.Test;

namespace iText.Svg.Renderers.Impl {
    [NUnit.Framework.Category("IntegrationTest")]
    public class CircleNodeRendererIntegrationTest : SvgIntegrationTest {
        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/svg/renderers/impl/CircleSvgNodeRendererTest/";

        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/svg/renderers/impl/CircleSvgNodeRendererTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            ITextTest.CreateDestinationFolder(destinationFolder);
        }

        [NUnit.Framework.Test]
        public virtual void BasicCircleTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "basicCircle");
        }

        [NUnit.Framework.Test]
        public virtual void CircleCxCyAbsentTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "circleCxCyAbsent");
        }

        [NUnit.Framework.Test]
        public virtual void CircleCxAbsentTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "circleCxAbsent");
        }

        [NUnit.Framework.Test]
        public virtual void CircleCxNegativeTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "circleCxNegative");
        }

        [NUnit.Framework.Test]
        public virtual void CircleCyAbsentTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "circleCyAbsent");
        }

        [NUnit.Framework.Test]
        public virtual void CircleCyNegativeTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "circleCyNegative");
        }

        [NUnit.Framework.Test]
        public virtual void CircleRAbsentTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "circleRAbsent");
        }

        [NUnit.Framework.Test]
        public virtual void CircleRNegativeTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "circleRNegative");
        }

        [NUnit.Framework.Test]
        public virtual void CircleTranslatedTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "circleTranslated");
        }

        [NUnit.Framework.Test]
        public virtual void CircleRotatedTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "circleRotated");
        }

        [NUnit.Framework.Test]
        public virtual void CircleScaledUpTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "circleScaledUp");
        }

        [NUnit.Framework.Test]
        public virtual void CircleScaledDownTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "circleScaledDown");
        }

        [NUnit.Framework.Test]
        public virtual void CircleScaledXYTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "circleScaledXY");
        }

        [NUnit.Framework.Test]
        public virtual void CircleSkewXTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "circleSkewX");
        }

        [NUnit.Framework.Test]
        public virtual void CircleSkewYTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "circleSkewY");
        }

        [NUnit.Framework.Test]
        public virtual void CircleWithBigStrokeWidthTest() {
            // TODO: DEVSIX-3932 update cmp_ after fix
            ConvertAndCompare(sourceFolder, destinationFolder, "circleWithBigStrokeWidth");
        }
    }
}
