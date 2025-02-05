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
using iText.Test.Attributes;

namespace iText.Svg.Processors.Impl.Font {
    [NUnit.Framework.Category("IntegrationTest")]
    public class FontSizeTest : SvgIntegrationTest {
        public static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/svg/processors/impl/font/FontSizeTest/";

        public static readonly String DESTINATION_FOLDER = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/svg/processors/impl/font/FontSizeTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            ITextTest.CreateDestinationFolder(DESTINATION_FOLDER);
        }

        [NUnit.Framework.Test]
        public virtual void FontSize01Test() {
            String name = "fontSizeTest01";
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, name);
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.StyledXmlParser.Logs.StyledXmlParserLogMessageConstant.UNKNOWN_ABSOLUTE_METRIC_LENGTH_PARSED
            )]
        public virtual void FontSize02Test() {
            String name = "fontSizeTest02";
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, name);
        }

        [NUnit.Framework.Test]
        public virtual void FontSize03Test() {
            String name = "fontSizeTest03";
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, name);
        }

        [NUnit.Framework.Test]
        public virtual void FontAbsoluteKeywords() {
            String name = "fontAbsoluteKeywords";
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, name);
        }

        [NUnit.Framework.Test]
        public virtual void FontRelativeKeywords() {
            String name = "fontRelativeKeywords";
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, name);
        }

        [NUnit.Framework.Test]
        public virtual void DiffUnitsOfMeasure() {
            String name = "diff_units_of_measure";
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, name);
        }
    }
}
