using System;
using iText.Test;

namespace iText.Svg.Renderers.Impl {
    public class NestedSvgSvgNodeRendererIntegrationTest {
        private static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/svg/renderers/impl/RootSvgNodeRendererTest/nested/";

        private static readonly String DESTINATION_FOLDER = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/svg/renderers/impl/RootSvgNodeRendererTest/nested/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            ITextTest.CreateDestinationFolder(DESTINATION_FOLDER);
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void SingleNestedSvgTest() {
            SvgNodeRendererTestUtility.ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "singleNested");
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void DoubleNestedSvgTest() {
            SvgNodeRendererTestUtility.ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "doubleNested");
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void TwoNestedSvgTest() {
            SvgNodeRendererTestUtility.ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "twoNested");
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void EmptySvgTest() {
            SvgNodeRendererTestUtility.ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "empty");
        }
    }
}
