using System;
using iText.Svg.Renderers;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Svg.Css {
    public class AttributesRelativeUnitTest : SvgIntegrationTest {
        public static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/svg/css/AttributesRelativeUnitTest/";

        public static readonly String DESTINATION_FOLDER = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/svg/css/AttributesRelativeUnitTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            ITextTest.CreateDestinationFolder(DESTINATION_FOLDER);
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.StyledXmlParser.LogMessageConstant.UNKNOWN_ABSOLUTE_METRIC_LENGTH_PARSED, Count = 4)]
        public virtual void RectangleAttributesEmUnitsTest() {
            // TODO DEVSIX-4834 support relative units in attributes of svg elements. Remove log message at this test
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "rectangleAttributesEmUnits");
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.StyledXmlParser.LogMessageConstant.UNKNOWN_ABSOLUTE_METRIC_LENGTH_PARSED, Count = 4)]
        public virtual void RectangleAttributesExUnitsTest() {
            // TODO DEVSIX-4834 support relative units in attributes of svg elements. Remove log message at this test
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "rectangleAttributesExUnits");
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.StyledXmlParser.LogMessageConstant.UNKNOWN_ABSOLUTE_METRIC_LENGTH_PARSED, Count = 4)]
        public virtual void RectangleAttributesPercentUnitsTest() {
            // TODO DEVSIX-4834 support relative units in attributes of svg elements. Remove log message at this test
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "rectangleAttributesPercentUnits");
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.StyledXmlParser.LogMessageConstant.UNKNOWN_ABSOLUTE_METRIC_LENGTH_PARSED, Count = 4)]
        public virtual void ImageAttributesEmUnitsTest() {
            // TODO DEVSIX-4834 support relative units in attributes of svg elements. Remove log message at this test
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "imageAttributesEmUnits");
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.StyledXmlParser.LogMessageConstant.UNKNOWN_ABSOLUTE_METRIC_LENGTH_PARSED, Count = 4)]
        public virtual void ImageAttributesExUnitsTest() {
            // TODO DEVSIX-4834 support relative units in attributes of svg elements. Remove log message at this test
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "imageAttributesExUnits");
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.StyledXmlParser.LogMessageConstant.UNKNOWN_ABSOLUTE_METRIC_LENGTH_PARSED, Count = 4)]
        public virtual void ImageAttributesPercentUnitsTest() {
            // TODO DEVSIX-4834 support relative units in attributes of svg elements. Remove log message at this test
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "imageAttributesPercentUnits");
        }
    }
}
