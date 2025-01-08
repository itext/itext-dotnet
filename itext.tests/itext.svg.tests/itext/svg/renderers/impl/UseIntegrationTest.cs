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
using iText.Svg.Logs;
using iText.Svg.Processors;
using iText.Svg.Processors.Impl;
using iText.Svg.Renderers;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Svg.Renderers.Impl {
    [NUnit.Framework.Category("IntegrationTest")]
    public class UseIntegrationTest : SvgIntegrationTest {
        private static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/svg/renderers/impl/UseIntegrationTest/";

        private static readonly String DESTINATION_FOLDER = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/svg/renderers/impl/UseIntegrationTest/";

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
        public virtual void SingleUseTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "singleUse");
        }

        [NUnit.Framework.Test]
        public virtual void SingleUseFillTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "singleUseFill");
        }

        [NUnit.Framework.Test]
        public virtual void DoubleNestedUseFillTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "doubleNestedUseFill");
        }

        [NUnit.Framework.Test]
        public virtual void SingleUseStrokeTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "singleUseStroke");
        }

        [NUnit.Framework.Test]
        public virtual void DoubleNestedUseStrokeTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "doubleNestedUseStroke");
        }

        [NUnit.Framework.Test]
        public virtual void TranslateUseTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "translateUse");
        }

        [NUnit.Framework.Test]
        public virtual void MultipleTransformationsUseTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "multipleTransformationsUse");
        }

        [NUnit.Framework.Test]
        public virtual void CoordinatesUseTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "coordinatesUse");
        }

        [NUnit.Framework.Test]
        public virtual void ImageUseTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "imageUse", properties);
        }

        [NUnit.Framework.Test]
        public virtual void SvgUseTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "svgUse", properties);
        }

        [NUnit.Framework.Test]
        public virtual void ComplexUseTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "complexUse", properties);
        }

        [NUnit.Framework.Test]
        public virtual void UseWithoutDefsTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "useWithoutDefs", properties);
        }

        [NUnit.Framework.Test]
        public virtual void UseWithoutDefsUsedElementAfterUseTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "useWithoutDefsUsedElementAfterUse", properties
                );
        }

        [NUnit.Framework.Test]
        public virtual void SimpleRectReuseTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "simpleRectReuse", properties);
        }

        [NUnit.Framework.Test]
        public virtual void TransitiveTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "transitive", properties);
        }

        [NUnit.Framework.Test]
        public virtual void CircularTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "circular", properties);
        }

        [NUnit.Framework.Test]
        public virtual void ComplexReferencesTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "complexReferences", properties);
        }

        [NUnit.Framework.Test]
        public virtual void TransformationsOnTransformationsTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "transformationsOnTransformations", properties
                );
        }

        [NUnit.Framework.Test]
        public virtual void ReuseLinesTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "reuseLines", properties);
        }

        [NUnit.Framework.Test]
        public virtual void MissingHashtagTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "missingHashtag", properties);
        }

        [NUnit.Framework.Test]
        public virtual void UseInDifferentFilesExampleTest() {
            //TODO: update when DEVSIX-2252 fixed
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "useInDifferentFilesExampleTest");
        }

        [NUnit.Framework.Test]
        public virtual void WidthAndHeightResolvingTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "widthAndHeightResolving", properties);
        }

        [NUnit.Framework.Test]
        public virtual void WidthAndHeightOverridingTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "widthAndHeightOverriding", properties);
        }

        [NUnit.Framework.Test]
        [LogMessage(SvgLogMessageConstant.VIEWBOX_WIDTH_OR_HEIGHT_IS_ZERO, LogLevel = LogLevelConstants.INFO, Count
             = 2)]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.ATTEMPT_PROCESS_NAN, LogLevel = LogLevelConstants.ERROR, Count
             = 4)]
        public virtual void InvalidWidthAndHeightResolvingTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "invalidWidthAndHeightResolving", properties
                );
        }

        [NUnit.Framework.Test]
        public virtual void WidthAndHeightNestedResolvingTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "widthAndHeightNestedResolving", properties
                );
        }
    }
}
