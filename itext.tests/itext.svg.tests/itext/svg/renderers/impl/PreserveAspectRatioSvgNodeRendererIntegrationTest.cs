using System;
using iText.Test;

namespace iText.Svg.Renderers.Impl {
    public class PreserveAspectRatioSvgNodeRendererIntegrationTest {
        private static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/svg/renderers/impl/RootSvgNodeRendererTest/aspectratio/";

        private static readonly String DESTINATION_FOLDER = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/svg/renderers/impl/RootSvgNodeRendererTest/aspectratio/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            ITextTest.CreateDestinationFolder(DESTINATION_FOLDER);
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void XMinYMinTest() {
            SvgNodeRendererTestUtility.ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "xminymin");
        }
        // TODO RND-876
    }
}
