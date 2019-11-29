using System;
using iText.Svg;
using iText.Svg.Exceptions;
using iText.Svg.Processors;
using iText.Svg.Processors.Impl;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Svg.Renderers.Impl {
    public class OverflowAttributeTest : ExtendedITextTest {
        private static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/svg/renderers/impl/OverflowAttributeTest/";

        private static readonly String DESTINATION_FOLDER = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/svg/renderers/impl/OverflowAttributeTest/";

        private ISvgConverterProperties properties;

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            ITextTest.CreateDestinationFolder(DESTINATION_FOLDER);
        }

        [NUnit.Framework.SetUp]
        public virtual void Before() {
            properties = new SvgConverterProperties().SetBaseUri(SOURCE_FOLDER);
        }

        [NUnit.Framework.Test]
        public virtual void OverflowVisibleInMarkerElementTest() {
            //TODO: update when DEVSIX-3482, DEVSIX-2860 fixed
            SvgNodeRendererIntegrationTestUtil.ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "overflowVisibleInMarkerElement"
                );
        }

        [NUnit.Framework.Test]
        public virtual void OverflowHiddenInMarkerElementTest() {
            //TODO: update when DEVSIX-3482, DEVSIX-2860 fixed
            SvgNodeRendererIntegrationTestUtil.ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "overflowHiddenInMarkerElement"
                );
        }

        [NUnit.Framework.Test]
        public virtual void OverflowAutoInMarkerElementTest() {
            //TODO: update when DEVSIX-3482, DEVSIX-2860 fixed
            SvgNodeRendererIntegrationTestUtil.ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "overflowAutoInMarkerElement"
                );
        }

        [NUnit.Framework.Test]
        public virtual void OverflowScrollInMarkerElementTest() {
            //TODO: update when DEVSIX-3482, DEVSIX-2860 fixed
            SvgNodeRendererIntegrationTestUtil.ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "overflowScrollInMarkerElement"
                );
        }

        [NUnit.Framework.Test]
        public virtual void OverflowInitialInMarkerElementTest() {
            //TODO: update when DEVSIX-3482, DEVSIX-2860 fixed
            SvgNodeRendererIntegrationTestUtil.ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "overflowInitialInMarkerElement"
                );
        }

        [NUnit.Framework.Test]
        [LogMessage(SvgLogMessageConstant.UNMAPPEDTAG, Count = 2)]
        [LogMessage(iText.StyledXmlParser.LogMessageConstant.ERROR_RESOLVING_PARENT_STYLES, Count = 2)]
        public virtual void OverflowVisibleInSymbolElementTest() {
            //TODO: update when DEVSIX-3482, DEVSIX-2257 fixed
            SvgNodeRendererIntegrationTestUtil.ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "overflowVisibleInSymbol"
                );
        }

        [NUnit.Framework.Test]
        [LogMessage(SvgLogMessageConstant.UNMAPPEDTAG)]
        [LogMessage(iText.StyledXmlParser.LogMessageConstant.ERROR_RESOLVING_PARENT_STYLES)]
        public virtual void OverflowHiddenInSymbolElementTest() {
            //TODO: update when DEVSIX-3482, DEVSIX-2257 fixed
            SvgNodeRendererIntegrationTestUtil.ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "overflowHiddenInSymbol"
                );
        }

        [NUnit.Framework.Test]
        [LogMessage(SvgLogMessageConstant.UNMAPPEDTAG)]
        [LogMessage(iText.StyledXmlParser.LogMessageConstant.ERROR_RESOLVING_PARENT_STYLES)]
        public virtual void OverflowInitialInSymbolElementTest() {
            //TODO: update when DEVSIX-3482, DEVSIX-2257 fixed
            SvgNodeRendererIntegrationTestUtil.ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "overflowInitialInSymbol"
                );
        }

        [NUnit.Framework.Test]
        [LogMessage(SvgLogMessageConstant.UNMAPPEDTAG)]
        [LogMessage(iText.StyledXmlParser.LogMessageConstant.ERROR_RESOLVING_PARENT_STYLES)]
        public virtual void OverflowScrollInSymbolElementTest() {
            //TODO: update when DEVSIX-3482, DEVSIX-2257 fixed
            SvgNodeRendererIntegrationTestUtil.ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "overflowScrollInSymbol"
                );
        }

        [NUnit.Framework.Test]
        public virtual void OverflowInSvgElementTest() {
            //TODO: update when DEVSIX-3482 fixed
            SvgNodeRendererIntegrationTestUtil.ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "overflowInSvgElement"
                );
        }
    }
}
