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
using iText.Svg.Exceptions;
using iText.Svg.Logs;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Svg.Renderers {
    [NUnit.Framework.Category("IntegrationTest")]
    public class FillTest : SvgIntegrationTest {
        private static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/svg/renderers/impl/FillTest/";

        private static readonly String DESTINATION_FOLDER = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/svg/renderers/impl/FillTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            ITextTest.CreateDestinationFolder(DESTINATION_FOLDER);
        }

        [NUnit.Framework.Test]
        public virtual void NormalRectangleFillTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "normalRectangleFill");
        }

        [NUnit.Framework.Test]
        public virtual void MultipleNormalRectangleFillTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "multipleNormalRectangleFill");
        }

        [NUnit.Framework.Test]
        public virtual void NoRectangleFillColorTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "noRectangleFillColor");
        }

        [NUnit.Framework.Test]
        public virtual void EoFillTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "eofill");
        }

        [NUnit.Framework.Test]
        public virtual void EoFillTest01() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "eofill01");
        }

        [NUnit.Framework.Test]
        public virtual void EoFillTest02() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "eofill02");
        }

        [NUnit.Framework.Test]
        public virtual void EoFillTest03() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "eofill03");
        }

        [NUnit.Framework.Test]
        public virtual void MultipleObjectsTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "multipleObjectsTest");
        }

        [NUnit.Framework.Test]
        public virtual void EoFillStrokeTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "eofillstroke");
        }

        [NUnit.Framework.Test]
        public virtual void NonZeroFillTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "nonzerofill");
        }

        [NUnit.Framework.Test]
        public virtual void OpacityFillTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "opacityfill");
        }

        [NUnit.Framework.Test]
        public virtual void EofillUnsuportedAtributeTest() {
            NUnit.Framework.Assert.Catch(typeof(SvgProcessingException), () => ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER
                , "eofillUnsuportedAtributeTest"));
        }

        [NUnit.Framework.Test]
        public virtual void PathVerticalLineFillTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "pathVerticalLineFillTest");
        }

        [NUnit.Framework.Test]
        public virtual void PathHorizontalLineFillTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "pathHorizontalLineFillTest");
        }

        [NUnit.Framework.Test]
        public virtual void InvalidUrlFillTest() {
            //TODO update cmp file after DEVSIX-3365 will be fixed
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "invalidUrlFillTest");
        }

        [NUnit.Framework.Test]
        [LogMessage(SvgLogMessageConstant.UNMAPPED_TAG, Count = 4)]
        public virtual void TextFillFallbackTest() {
            //TODO update cmp file after DEVSIX-2915 will be fixed
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "textFillFallbackTest");
        }

        [NUnit.Framework.Test]
        public virtual void FillLinkToNonExistingGradientTest() {
            //TODO DEVSIX-8821: update after supported
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "fillLinkToNonExistingGradient");
        }
    }
}
