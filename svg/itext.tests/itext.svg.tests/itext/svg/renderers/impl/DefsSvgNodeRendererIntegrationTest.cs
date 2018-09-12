using System;
using iText.Svg.Renderers;
using iText.Test;

namespace iText.Svg.Renderers.Impl {
    public class DefsSvgNodeRendererIntegrationTest : SvgIntegrationTest {
        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/svg/renderers/impl/DefsSvgNodeRendererTest/";

        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/svg/renderers/impl/DefsSvgNodeRendererTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            ITextTest.CreateDestinationFolder(destinationFolder);
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void DefsWithNoChildrenTest() {
            ConvertAndCompareSinglePageVisually(sourceFolder, destinationFolder, "onlyDefsWithNoChildren");
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void DefsWithOneChildTest() {
            ConvertAndCompareSinglePageVisually(sourceFolder, destinationFolder, "onlyDefsWithOneChild");
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void DefsWithMultipleChildrenTest() {
            ConvertAndCompareSinglePageVisually(sourceFolder, destinationFolder, "onlyDefsWithMultipleChildren");
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void DefsWithOneChildAndNonDefsBeingDrawnTest() {
            ConvertAndCompareSinglePageVisually(sourceFolder, destinationFolder, "defsWithOneChildAndNonDefsBeingDrawn"
                );
        }
    }
}
