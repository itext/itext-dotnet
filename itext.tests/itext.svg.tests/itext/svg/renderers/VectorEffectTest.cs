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
using iText.Svg.Logs;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Svg.Renderers {
    [NUnit.Framework.Category("IntegrationTest")]
    public class VectorEffectTest : SvgIntegrationTest {
        private static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/svg/renderers/VectorEffectTest/";

        private static readonly String DESTINATION_FOLDER = TestUtil.GetOutputPath() + "/svg/renderers/VectorEffectTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            ITextTest.CreateDestinationFolder(DESTINATION_FOLDER);
        }

        [NUnit.Framework.Test]
        public virtual void NonScalingStrokeTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "nonScalingStroke");
        }

        [NUnit.Framework.Test]
        public virtual void NonScalingStrokePathTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "nonScalingStrokePath");
        }

        [NUnit.Framework.Test]
        public virtual void NonScalingStrokeFiguresTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "nonScalingStrokeFigures");
        }

        [NUnit.Framework.Test]
        public virtual void NonScalingStrokeTextTest() {
            // TODO DEVSIX-8850 support vector-effect for text and tspan
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "nonScalingStrokeText");
        }

        [NUnit.Framework.Test]
        public virtual void PreserveAspectRatioTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "preserveAspectRatio");
        }

        [NUnit.Framework.Test]
        public virtual void SvgWithSvgTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "svgWithSvg");
        }

        [NUnit.Framework.Test]
        public virtual void SeveralTransformationsTest() {
            // TODO DEVSIX-8850 support vector-effect for text and tspan
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "severalTransformations");
        }

        [NUnit.Framework.Test]
        public virtual void SeveralNestedSvgTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "severalNestedSvg");
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.StyledXmlParser.Logs.StyledXmlParserLogMessageConstant.UNABLE_TO_RETRIEVE_IMAGE_WITH_GIVEN_BASE_URI
            )]
        public virtual void ImageWithSvgTest() {
            // TODO DEVSIX-8884 Support svg format for image href attribute
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "imageWithSvg");
        }

        [NUnit.Framework.Test]
        public virtual void ClipPathTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "clipPath");
        }

        [NUnit.Framework.Test]
        public virtual void VectorEffectOnUseTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "vectorEffectOnUse");
        }

        [NUnit.Framework.Test]
        [LogMessage(SvgLogMessageConstant.NON_INVERTIBLE_TRANSFORMATION_MATRIX_FOR_NON_SCALING_STROKE)]
        public virtual void NonInvertibleMatrixTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "nonInvertibleMatrix");
        }
    }
}
