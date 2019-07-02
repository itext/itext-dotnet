using System;
using iText.Svg.Processors;
using iText.Svg.Processors.Impl;
using iText.Svg.Renderers;
using iText.Test;

namespace iText.Svg.Renderers.Impl {
    public class MarkerSvgTests : SvgIntegrationTest {
        private static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/svg/renderers/impl/MarkerSvgTests/";

        private static readonly String DESTINATION_FOLDER = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/svg/renderers/impl/MarkerSvgTests/";

        private ISvgConverterProperties properties;

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            ITextTest.CreateDestinationFolder(DESTINATION_FOLDER);
        }

        [NUnit.Framework.SetUp]
        public virtual void Before() {
            properties = new SvgConverterProperties().SetBaseUri(SOURCE_FOLDER);
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void MarkerTest() {
            //TODO: update when DEVSIX-2262, 2860 fixed
            ConvertAndCompareSinglePageVisually(SOURCE_FOLDER, DESTINATION_FOLDER, "marker");
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void Markers_in_different_elements() {
            //TODO: update when DEVSIX-2262, 2860 and DEVSIX-2719 fixed
            ConvertAndCompareSinglePageVisually(SOURCE_FOLDER, DESTINATION_FOLDER, "Markers_in_elements");
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void MarkerUnits() {
            //TODO: update when DEVSIX-2262, 2860 fixed
            ConvertAndCompareSinglePageVisually(SOURCE_FOLDER, DESTINATION_FOLDER, "marker_Units");
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void Marker_RefXY_Orient() {
            //TODO: update when DEVSIX-2262,2860 fixed
            ConvertAndCompareSinglePageVisually(SOURCE_FOLDER, DESTINATION_FOLDER, "marker_RefXY_orient");
        }
    }
}
