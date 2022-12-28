/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
Authors: iText Software.

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
using iText.StyledXmlParser.Css.Validate;
using iText.StyledXmlParser.Css.Validate.Impl;
using iText.Svg.Processors;
using iText.Svg.Processors.Impl;
using iText.Svg.Renderers;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Svg.Renderers.Impl {
    public class DeviceCmykSvgTest : SvgIntegrationTest {
        private static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/svg/renderers/impl/DeviceCmykTest/";

        private static readonly String DESTINATION_FOLDER = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/svg/renderers/impl/DeviceCmykTest/";

        private ISvgConverterProperties properties;

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            ITextTest.CreateDestinationFolder(DESTINATION_FOLDER);
            CssDeclarationValidationMaster.SetValidator(new CssDeviceCmykAwareValidator());
        }

        [NUnit.Framework.SetUp]
        public virtual void Before() {
            properties = new SvgConverterProperties().SetBaseUri(SOURCE_FOLDER);
        }

        [NUnit.Framework.OneTimeTearDown]
        public static void AfterClass() {
            CssDeclarationValidationMaster.SetValidator(new CssDefaultValidator());
        }

        [NUnit.Framework.Test]
        public virtual void SvgFillColorTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "svgFillColor");
        }

        [NUnit.Framework.Test]
        public virtual void SvgStrokeColorTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "svgStrokeColor");
        }

        [NUnit.Framework.Test]
        public virtual void SvgFillStrokeColorTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "svgFillStrokeColor");
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.StyledXmlParser.Logs.StyledXmlParserLogMessageConstant.UNKNOWN_ABSOLUTE_METRIC_LENGTH_PARSED
            )]
        public virtual void SvgSimpleShapesColorTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "svgSimpleShapesColor");
        }
    }
}
