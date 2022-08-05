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
