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

namespace iText.Svg.Renderers.Impl {
    [NUnit.Framework.Category("IntegrationTest")]
    public class CircleNodeRendererIntegrationTest : SvgIntegrationTest {
        private static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/svg/renderers/impl/CircleSvgNodeRendererTest/";

        private static readonly String DESTINATION_FOLDER = TestUtil.GetOutputPath() + "/svg/renderers/impl/CircleSvgNodeRendererTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            ITextTest.CreateDestinationFolder(DESTINATION_FOLDER);
        }

        [NUnit.Framework.Test]
        public virtual void BasicCircleTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "basicCircle");
        }

        [NUnit.Framework.Test]
        public virtual void RelativeCircleTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "relativeCircle");
        }

        [NUnit.Framework.Test]
        public virtual void CircleCxCyAbsentTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "circleCxCyAbsent");
        }

        [NUnit.Framework.Test]
        public virtual void CircleCxAbsentTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "circleCxAbsent");
        }

        [NUnit.Framework.Test]
        public virtual void CircleCxNegativeTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "circleCxNegative");
        }

        [NUnit.Framework.Test]
        public virtual void CircleCyAbsentTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "circleCyAbsent");
        }

        [NUnit.Framework.Test]
        public virtual void CircleCyNegativeTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "circleCyNegative");
        }

        [NUnit.Framework.Test]
        public virtual void CircleRAbsentTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "circleRAbsent");
        }

        [NUnit.Framework.Test]
        public virtual void CircleRNegativeTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "circleRNegative");
        }

        [NUnit.Framework.Test]
        public virtual void CircleTranslatedTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "circleTranslated");
        }

        [NUnit.Framework.Test]
        public virtual void CircleRotatedTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "circleRotated");
        }

        [NUnit.Framework.Test]
        public virtual void CircleScaledUpTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "circleScaledUp");
        }

        [NUnit.Framework.Test]
        public virtual void CircleScaledDownTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "circleScaledDown");
        }

        [NUnit.Framework.Test]
        public virtual void CircleScaledXYTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "circleScaledXY");
        }

        [NUnit.Framework.Test]
        public virtual void CircleSkewXTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "circleSkewX");
        }

        [NUnit.Framework.Test]
        public virtual void CircleSkewYTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "circleSkewY");
        }

        [NUnit.Framework.Test]
        public virtual void CircleWithBigStrokeWidthTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "circleWithBigStrokeWidth");
        }

        [NUnit.Framework.Test]
        public virtual void CircleShapeRenderingTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "shape-rendering");
        }
    }
}
