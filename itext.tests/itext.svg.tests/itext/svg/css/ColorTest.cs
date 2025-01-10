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
using iText.Svg.Processors.Impl;
using iText.Svg.Renderers;
using iText.Test;

namespace iText.Svg.Css {
    [NUnit.Framework.Category("IntegrationTest")]
    public class ColorTest : SvgIntegrationTest {
        public static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/svg/css/ColorTest/";

        public static readonly String DESTINATION_FOLDER = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/svg/css/ColorTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            ITextTest.CreateDestinationFolder(DESTINATION_FOLDER);
        }

        [NUnit.Framework.Test]
        public virtual void BackgroundColorTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "background-color");
        }

        [NUnit.Framework.Test]
        public virtual void CurrentColorTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "current-color");
        }

        [NUnit.Framework.Test]
        public virtual void InterpolationColorTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "color-interpolation");
        }

        [NUnit.Framework.Test]
        public virtual void InterpolationFilterColorTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "color-interpolation-filter");
        }

        [NUnit.Framework.Test]
        public virtual void ColorProfileTest() {
            //TODO DEVSIX-2259: update cmp after supporting
            SvgConverterProperties properties = new SvgConverterProperties().SetBaseUri(SOURCE_FOLDER);
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "color-profile", properties);
        }

        [NUnit.Framework.Test]
        public virtual void ColorTypesTest() {
            //TODO DEVSIX-8748: update cmp files after supporting
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "color-types");
        }
    }
}
