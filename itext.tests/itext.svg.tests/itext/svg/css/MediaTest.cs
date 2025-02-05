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
    public class MediaTest : SvgIntegrationTest {
        // TODO DEVSIX-2263 SVG: Update cmp files
        private static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/svg/css/MediaTest/";

        private static readonly String DESTINATION_FOLDER = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/svg/css/MediaTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            ITextTest.CreateDestinationFolder(DESTINATION_FOLDER);
        }

        [NUnit.Framework.Test]
        public virtual void MediaQueryWidthTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "mediaQueryWidth");
        }

        [NUnit.Framework.Test]
        public virtual void MediaQueryMinWidthTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "mediaQueryMinWidth");
        }

        [NUnit.Framework.Test]
        public virtual void MediaQueryBigMinWidthTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "mediaQueryMinWidth2");
        }

        [NUnit.Framework.Test]
        public virtual void MediaQueryBigMaxWidthTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "mediaQueryMaxWidth");
        }

        [NUnit.Framework.Test]
        public virtual void MediaQueryHeightTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "mediaQueryHeight");
        }

        [NUnit.Framework.Test]
        public virtual void MediaQueryMinHeightTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "mediaQueryMinHeight");
        }

        [NUnit.Framework.Test]
        public virtual void MediaQueryMaxHeightTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "mediaQueryMaxHeight");
        }

        [NUnit.Framework.Test]
        public virtual void MediaQueryOrientationLandscapeTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "mediaQueryOrientationLandscape");
        }

        [NUnit.Framework.Test]
        public virtual void MediaQueryOrientationPortraitTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "mediaQueryOrientationPortrait");
        }

        [NUnit.Framework.Test]
        public virtual void MediaQueryAspectRatioTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "mediaQueryRatio");
        }

        [NUnit.Framework.Test]
        public virtual void MediaQueryAspectRatioSingleValTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "mediaQueryRatioSingleVal");
        }

        [NUnit.Framework.Test]
        public virtual void MediaQueryScreenTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "mediaQueryScreen");
        }

        [NUnit.Framework.Test]
        public virtual void MediaQueryPrintTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "mediaQueryPrint");
        }

        [NUnit.Framework.Test]
        public virtual void MediaQueryResolutionTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "mediaQueryResolution");
        }

        [NUnit.Framework.Test]
        public virtual void MediaQueryOverflowBlockTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "mediaQueryOverflowBlock");
        }

        [NUnit.Framework.Test]
        public virtual void MediaQueryOverflowInlineTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "mediaQueryOverflowInline");
        }

        [NUnit.Framework.Test]
        public virtual void MediaQueryNotTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "mediaQueryNot");
        }

        [NUnit.Framework.Test]
        public virtual void MediaQueryOnlyTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "mediaQueryOnly");
        }

        [NUnit.Framework.Test]
        public virtual void MediaQueryOnlyAndTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "mediaQueryOnlyAnd");
        }

        [NUnit.Framework.Test]
        public virtual void MediaQueryColorTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "mediaQueryColor");
        }

        [NUnit.Framework.Test]
        public virtual void MediaQueryMinColorTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "mediaQueryMinColor");
        }

        [NUnit.Framework.Test]
        public virtual void MediaQueryColorGamutTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "mediaQueryColorGamut");
        }

        [NUnit.Framework.Test]
        public virtual void MediaQueryDisplayBrowserTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "mediaQueryDisplayBrowser");
        }
    }
}
