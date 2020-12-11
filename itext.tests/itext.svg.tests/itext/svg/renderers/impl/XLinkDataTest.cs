using System;
using iText.Svg.Renderers;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Svg.Renderers.Impl {
    public class XLinkDataTest : SvgIntegrationTest {
        private static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/svg/renderers/impl/XLinkDataTest/";

        private static readonly String DESTINATION_FOLDER = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/svg/renderers/impl/XLinkDataTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            ITextTest.CreateDestinationFolder(DESTINATION_FOLDER);
        }

        [NUnit.Framework.Test]
        public virtual void CorrectImageWithDataTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "correctImageWithData");
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.StyledXmlParser.LogMessageConstant.UNABLE_TO_RETRIEVE_IMAGE_WITH_GIVEN_DATA_URI)]
        public virtual void IncorrectImageWithDataTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "incorrectImageWithData");
        }
    }
}
