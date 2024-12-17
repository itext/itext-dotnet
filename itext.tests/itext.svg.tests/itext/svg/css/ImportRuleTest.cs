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
using iText.StyledXmlParser.Css.Media;
using iText.Svg.Processors;
using iText.Svg.Processors.Impl;
using iText.Svg.Renderers;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Svg.Css {
    [NUnit.Framework.Category("IntegrationTest")]
    public class ImportRuleTest : SvgIntegrationTest {
        private static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/svg/css/ImportRuleTest/";

        private static readonly String DESTINATION_FOLDER = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/svg/css/ImportRuleTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            ITextTest.CreateDestinationFolder(DESTINATION_FOLDER);
        }

        [NUnit.Framework.Test]
        public virtual void ExternalCssLoopTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "externalCssLoop");
        }

        [NUnit.Framework.Test]
        public virtual void RecursiveImports1Test() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "recursiveImports1");
        }

        [NUnit.Framework.Test]
        public virtual void RecursiveImports2Test() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "recursiveImports2");
        }

        [NUnit.Framework.Test]
        public virtual void RecursiveImports3Test() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "recursiveImports3");
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.StyledXmlParser.Logs.StyledXmlParserLogMessageConstant.IMPORT_MUST_COME_BEFORE, LogLevel
             = LogLevelConstants.WARN)]
        public virtual void StyleBeforeImportTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "styleBeforeImport");
        }

        [NUnit.Framework.Test]
        public virtual void TwoExternalCssTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "twoExternalCss");
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.StyledXmlParser.Logs.StyledXmlParserLogMessageConstant.UNABLE_TO_RETRIEVE_STREAM_WITH_GIVEN_BASE_URI
            , LogLevel = LogLevelConstants.ERROR)]
        public virtual void WrongNestedCssTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "wrongNestedCss");
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.StyledXmlParser.Logs.StyledXmlParserLogMessageConstant.UNABLE_TO_RETRIEVE_STREAM_WITH_GIVEN_BASE_URI
            , LogLevel = LogLevelConstants.ERROR)]
        public virtual void MediaQueryTest() {
            // TODO DEVSIX-2263 SVG: CSS: Media query processing
            ISvgConverterProperties properties = new SvgConverterProperties().SetMediaDeviceDescription(new MediaDeviceDescription
                (MediaType.SCREEN));
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "mediaQuery", properties);
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.StyledXmlParser.Logs.StyledXmlParserLogMessageConstant.UNABLE_TO_RETRIEVE_STREAM_WITH_GIVEN_BASE_URI
            , LogLevel = LogLevelConstants.ERROR)]
        public virtual void MediaQueryPrintTest() {
            // TODO DEVSIX-2263 SVG: CSS: Media query processing
            ISvgConverterProperties properties = new SvgConverterProperties().SetMediaDeviceDescription(new MediaDeviceDescription
                (MediaType.SCREEN));
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "mediaQueryPrint", properties);
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.StyledXmlParser.Logs.StyledXmlParserLogMessageConstant.UNABLE_TO_RETRIEVE_STREAM_WITH_GIVEN_BASE_URI
            , LogLevel = LogLevelConstants.ERROR)]
        public virtual void SrcInImportTest() {
            // Spec says that import can contain src, but no one browser doesn't support it as well as iText
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "srcInImport");
        }
    }
}
