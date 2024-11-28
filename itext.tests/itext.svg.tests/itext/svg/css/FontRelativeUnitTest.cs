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
using iText.Test.Attributes;

namespace iText.Svg.Css {
    [NUnit.Framework.Category("IntegrationTest")]
    public class FontRelativeUnitTest : SvgIntegrationTest {
        public static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/svg/css/FontRelativeUnitTest/";

        public static readonly String DESTINATION_FOLDER = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/svg/css/FontRelativeUnitTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            ITextTest.CreateDestinationFolder(DESTINATION_FOLDER);
        }

        // Text tests block
        [NUnit.Framework.Test]
        public virtual void TextFontSizeRemUnitTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "textFontSizeRemUnitTest");
        }

        [NUnit.Framework.Test]
        public virtual void TextFontSizeEmUnitTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "textFontSizeEmUnitTest");
        }

        [NUnit.Framework.Test]
        public virtual void TextNegativeFontSizeRemUnitTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "textNegativeFontSizeRemUnitTest");
        }

        [NUnit.Framework.Test]
        public virtual void TextNegativeFontSizeEmUnitTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "textNegativeFontSizeEmUnitTest");
        }

        [NUnit.Framework.Test]
        public virtual void TextFontSizeFromParentTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "textFontSizeFromParentTest");
        }

        [NUnit.Framework.Test]
        public virtual void TextFontSizeHierarchyEmAndRemUnitTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "textFontSizeHierarchyEmAndRemUnitTest");
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.StyledXmlParser.Logs.StyledXmlParserLogMessageConstant.UNKNOWN_ABSOLUTE_METRIC_LENGTH_PARSED
            , Count = 6)]
        public virtual void TextFontSizeInheritanceFromUseTest() {
            // TODO DEVSIX-2607 relative font-size value is not supported for tspan element
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "textFontSizeInheritanceFromUseTest");
        }

        [NUnit.Framework.Test]
        public virtual void TextFontSizeFromUseTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "textFontSizeFromUseTest");
        }

        // Linear gradient tests block
        [NUnit.Framework.Test]
        public virtual void LnrGrdntObjectBoundingBoxEmUnitTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "lnrGrdntObjectBoundingBoxEmUnitTest");
        }

        [NUnit.Framework.Test]
        public virtual void LnrGrdntUserSpaceOnUseEmUnitTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "lnrGrdntUserSpaceOnUseEmUnitTest");
        }

        [NUnit.Framework.Test]
        public virtual void LnrGrdntObjectBoundingBoxRemUnitTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "lnrGrdntObjectBoundingBoxRemUnitTest");
        }

        [NUnit.Framework.Test]
        public virtual void LnrGrdntUserSpaceOnUseRemUnitTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "lnrGrdntUserSpaceOnUseRemUnitTest");
        }

        [NUnit.Framework.Test]
        public virtual void LnrGrdntObjectBoundingBoxEmUnitFromDirectParentTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "lnrGrdntObjectBoundingBoxEmUnitFromDirectParentTest"
                );
        }

        [NUnit.Framework.Test]
        public virtual void LnrGrdntUserSpaceOnUseEmUnitFromDirectParentTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "lnrGrdntUserSpaceOnUseEmUnitFromDirectParentTest"
                );
        }

        [NUnit.Framework.Test]
        public virtual void LnrGrdntFontSizeFromDefsFillTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "lnrGrdntFontSizeFromDefsFillTest");
        }

        // Symbol tests block
        [NUnit.Framework.Test]
        public virtual void SymbolFontSizeInheritanceFromUseTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "symbolFontSizeInheritanceFromUseTest");
        }

        // Marker tests block
        [NUnit.Framework.Test]
        public virtual void MarkerFontSizeInheritanceFromDifsTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "markerFontSizeInheritanceFromDifsTest");
        }
    }
}
