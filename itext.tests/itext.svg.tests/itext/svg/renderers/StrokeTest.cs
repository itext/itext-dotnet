using System;
using iText.Svg.Renderers.Impl;
using iText.Test;

namespace iText.Svg.Renderers {
    public class StrokeTest {
        private static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/svg/renderers/impl/StrokeTest/";

        private static readonly String DESTINATION_FOLDER = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/svg/renderers/impl/StrokeTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            ITextTest.CreateDestinationFolder(DESTINATION_FOLDER);
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void NormalLineStrokeTest() {
            SvgNodeRendererTestUtility.ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "normalLineStroke");
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void NoLineStrokeTest() {
            SvgNodeRendererTestUtility.ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "noLineStroke");
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void NoLineStrokeWidthTest() {
            SvgNodeRendererTestUtility.ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "noLineStrokeWidth");
        }
    }
}
