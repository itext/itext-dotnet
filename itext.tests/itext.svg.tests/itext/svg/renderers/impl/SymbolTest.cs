using System;
using iText.Svg.Exceptions;
using iText.Svg.Processors;
using iText.Svg.Processors.Impl;
using iText.Svg.Renderers;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Svg.Renderers.Impl {
    public class SymbolTest : SvgIntegrationTest {
        public static readonly String DESTINATION_FOLDER = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/svg/renderers/impl/SymbolTest/";

        public static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/svg/renderers/impl/SymbolTest/";

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
        // TODO Check cmp after feature implementation. DEVSIX-2257
        [LogMessage(SvgLogMessageConstant.UNMAPPEDTAG)]
        [LogMessage(iText.StyledXmlParser.LogMessageConstant.ERROR_RESOLVING_PARENT_STYLES)]
        public virtual void SimpleSymbolTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "simpleSymbolTest");
        }

        [NUnit.Framework.Test]
        // TODO Check cmp after feature implementation. DEVSIX-2257
        [LogMessage(SvgLogMessageConstant.UNMAPPEDTAG)]
        [LogMessage(iText.StyledXmlParser.LogMessageConstant.ERROR_RESOLVING_PARENT_STYLES)]
        public virtual void UseTagFirstSymbolAfterTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "useTagFirstSymbolAfterTest");
        }

        [NUnit.Framework.Test]
        [LogMessage(SvgLogMessageConstant.UNMAPPEDTAG)]
        [LogMessage(iText.StyledXmlParser.LogMessageConstant.ERROR_RESOLVING_PARENT_STYLES)]
        public virtual void HeightPxAttrTest() {
            // TODO Check cmp after feature implementation. DEVSIX-2257
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "heightPxAttrTest");
        }

        [NUnit.Framework.Test]
        [LogMessage(SvgLogMessageConstant.UNMAPPEDTAG)]
        [LogMessage(iText.StyledXmlParser.LogMessageConstant.ERROR_RESOLVING_PARENT_STYLES)]
        public virtual void HeightPercentsAttrTest() {
            // TODO Check cmp after feature implementation. DEVSIX-2257
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "heightPercentsAttrTest");
        }

        [NUnit.Framework.Test]
        [LogMessage(SvgLogMessageConstant.UNMAPPEDTAG)]
        [LogMessage(iText.StyledXmlParser.LogMessageConstant.ERROR_RESOLVING_PARENT_STYLES)]
        public virtual void WidthPxAttrTest() {
            // TODO Check cmp after feature implementation. DEVSIX-2257
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "widthPxAttrTest");
        }

        [NUnit.Framework.Test]
        [LogMessage(SvgLogMessageConstant.UNMAPPEDTAG)]
        [LogMessage(iText.StyledXmlParser.LogMessageConstant.ERROR_RESOLVING_PARENT_STYLES)]
        public virtual void WidthPercentsAttrTest() {
            // TODO Check cmp after feature implementation. DEVSIX-2257
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "widthPercentsAttrTest");
        }

        [NUnit.Framework.Test]
        [LogMessage(SvgLogMessageConstant.UNMAPPEDTAG)]
        [LogMessage(iText.StyledXmlParser.LogMessageConstant.ERROR_RESOLVING_PARENT_STYLES)]
        public virtual void WidthHeightAttrPxTest() {
            // TODO Check cmp after feature implementation. DEVSIX-2257
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "widthHeightAttrPxTest");
        }

        [NUnit.Framework.Test]
        // TODO Check cmp after feature implementation. DEVSIX-2257
        [LogMessage(iText.StyledXmlParser.LogMessageConstant.UNKNOWN_ABSOLUTE_METRIC_LENGTH_PARSED, Count = 4)]
        [LogMessage(SvgLogMessageConstant.UNMAPPEDTAG, Count = 2)]
        [LogMessage(iText.StyledXmlParser.LogMessageConstant.ERROR_RESOLVING_PARENT_STYLES, Count = 2)]
        public virtual void WidthHeightAttrPercentsPxTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "widthHeightAttrPercentsPxTest");
        }

        [NUnit.Framework.Test]
        // TODO Check cmp after feature implementation. DEVSIX-2257
        [LogMessage(SvgLogMessageConstant.MISSING_HEIGHT)]
        [LogMessage(SvgLogMessageConstant.MISSING_WIDTH)]
        [LogMessage(SvgLogMessageConstant.UNMAPPEDTAG, Count = 3)]
        [LogMessage(iText.StyledXmlParser.LogMessageConstant.ERROR_RESOLVING_PARENT_STYLES, Count = 3)]
        public virtual void PreserveAspectRatioViewBoxTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "preserveAspectRatioViewBoxTest");
        }

        [NUnit.Framework.Test]
        [NUnit.Framework.Ignore("DEVSIX-2257")]
        public virtual void XYInUseWithDefsTest() {
            // TODO Check cmp after feature implementation. DEVSIX-2257
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "xYInUseWithDefsTest");
        }

        [NUnit.Framework.Test]
        // TODO Check cmp after feature implementation. DEVSIX-2257
        [LogMessage(SvgLogMessageConstant.UNMAPPEDTAG)]
        [LogMessage(iText.StyledXmlParser.LogMessageConstant.ERROR_RESOLVING_PARENT_STYLES)]
        public virtual void ClassAttributeTestWithCssTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "classAttrTest");
        }

        [NUnit.Framework.Test]
        // TODO Check cmp after feature implementation. DEVSIX-2257
        [LogMessage(SvgLogMessageConstant.UNMAPPEDTAG)]
        [LogMessage(iText.StyledXmlParser.LogMessageConstant.ERROR_RESOLVING_PARENT_STYLES)]
        public virtual void StyleAttributeTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "styleAttrTest");
        }

        [NUnit.Framework.Test]
        // TODO Check cmp after feature implementation. DEVSIX-2257
        [LogMessage(SvgLogMessageConstant.UNMAPPEDTAG)]
        [LogMessage(iText.StyledXmlParser.LogMessageConstant.ERROR_RESOLVING_PARENT_STYLES)]
        public virtual void StyleAttrInUseTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "styleAttrInUseTest");
        }

        [NUnit.Framework.Test]
        // TODO Check cmp after feature implementation. DEVSIX-2257
        [LogMessage(SvgLogMessageConstant.UNMAPPEDTAG)]
        [LogMessage(iText.StyledXmlParser.LogMessageConstant.ERROR_RESOLVING_PARENT_STYLES)]
        public virtual void BothStyleAttributeTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "bothStyleAttrTest");
        }

        [NUnit.Framework.Test]
        // TODO Check cmp after feature implementation. DEVSIX-2257
        [LogMessage(SvgLogMessageConstant.UNMAPPEDTAG)]
        [LogMessage(iText.StyledXmlParser.LogMessageConstant.ERROR_RESOLVING_PARENT_STYLES)]
        public virtual void OpacityAttributeTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "opacityAttrTest");
        }

        [NUnit.Framework.Test]
        // TODO Check cmp after feature implementation. DEVSIX-2257
        [LogMessage(SvgLogMessageConstant.UNMAPPEDTAG)]
        [LogMessage(iText.StyledXmlParser.LogMessageConstant.ERROR_RESOLVING_PARENT_STYLES)]
        public virtual void VisibilityAttributeTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "visibilityAttrTest");
        }

        [NUnit.Framework.Test]
        // TODO Check cmp after feature implementation. DEVSIX-2257
        [LogMessage(SvgLogMessageConstant.UNMAPPEDTAG)]
        [LogMessage(iText.StyledXmlParser.LogMessageConstant.ERROR_RESOLVING_PARENT_STYLES)]
        public virtual void DisplayNoneAttributeTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "displayNoneAttrTest");
        }

        [NUnit.Framework.Test]
        // TODO Check cmp after feature implementation. DEVSIX-2257
        [LogMessage(SvgLogMessageConstant.UNMAPPEDTAG)]
        [LogMessage(iText.StyledXmlParser.LogMessageConstant.ERROR_RESOLVING_PARENT_STYLES)]
        public virtual void DisplayAttributeWithNoUseTagTest() {
            //Expects that nothing will be displayed on the page as it's done in Chrome browser
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "displayAttrWithNoUseTagTest");
        }

        [NUnit.Framework.Test]
        // TODO Check cmp after feature implementation. DEVSIX-2257
        [LogMessage(SvgLogMessageConstant.UNMAPPEDTAG)]
        [LogMessage(iText.StyledXmlParser.LogMessageConstant.ERROR_RESOLVING_PARENT_STYLES)]
        public virtual void SimpleImageTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "simpleImageTest", properties);
        }

        [NUnit.Framework.Test]
        // TODO Check cmp after feature implementation. DEVSIX-2257
        [LogMessage(SvgLogMessageConstant.UNMAPPEDTAG)]
        [LogMessage(iText.StyledXmlParser.LogMessageConstant.ERROR_RESOLVING_PARENT_STYLES)]
        public virtual void LinearGradientSymbolTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "linearGradientSymbolTest", properties);
        }

        [NUnit.Framework.Test]
        // TODO Check cmp after feature implementation. DEVSIX-2257
        [LogMessage(SvgLogMessageConstant.UNMAPPEDTAG)]
        [LogMessage(iText.StyledXmlParser.LogMessageConstant.ERROR_RESOLVING_PARENT_STYLES)]
        public virtual void UseHeightWidthAllUnitsTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "useHeightWidthAllUnitsTest", properties);
        }

        [NUnit.Framework.Test]
        // TODO Check cmp after feature implementation. DEVSIX-2257
        [LogMessage(SvgLogMessageConstant.UNMAPPEDTAG)]
        [LogMessage(iText.StyledXmlParser.LogMessageConstant.ERROR_RESOLVING_PARENT_STYLES)]
        public virtual void UseSymbolHeightWidthAllUnitsTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "useSymbolHeightWidthAllUnitsTest", properties
                );
        }

        [NUnit.Framework.Test]
        [LogMessage(SvgLogMessageConstant.UNMAPPEDTAG)]
        [LogMessage(iText.StyledXmlParser.LogMessageConstant.ERROR_RESOLVING_PARENT_STYLES)]
        public virtual void UseSymbolXYContrudictionAllUnitsTest() {
            // TODO Check cmp after feature implementation. DEVSIX-2257
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "useSymbolXYContrudictionAllUnitsTest", properties
                );
        }
    }
}
