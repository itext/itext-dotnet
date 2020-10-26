using System;
using iText.Svg.Renderers;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Svg.Css {
    public class FontRelativeUnitsTest : SvgIntegrationTest {
        public static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/svg/css/FontRelativeUnitsTest/";

        public static readonly String DESTINATION_FOLDER = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/svg/css/FontRelativeUnitsTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            ITextTest.CreateDestinationFolder(DESTINATION_FOLDER);
        }

        // Text tests block
        [NUnit.Framework.Test]
        public virtual void TextFontSizeRemUnitTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "textFontSizeRemUnitTest");
        }

        [NUnit.Framework.Test]
        public virtual void TextFontSizeEmUnitTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "textFontSizeEmUnitTest");
        }

        [NUnit.Framework.Test]
        public virtual void TextNegativeFontSizeRemUnitTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "textNegativeFontSizeRemUnitTest");
        }

        [NUnit.Framework.Test]
        public virtual void TextNegativeFontSizeEmUnitTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "textNegativeFontSizeEmUnitTest");
        }

        [NUnit.Framework.Test]
        public virtual void TextFontSizeFromParentTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "textFontSizeFromParentTest");
        }

        [NUnit.Framework.Test]
        public virtual void TextFontSizeHierarchyEmAndRemUnitTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "textFontSizeHierarchyEmAndRemUnitTest");
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.StyledXmlParser.LogMessageConstant.UNKNOWN_ABSOLUTE_METRIC_LENGTH_PARSED, Count = 4)]
        public virtual void TextFontSizeInheritanceFromUseTest() {
            // TODO DEVSIX-2607 relative font-size value is not supported for tspan element
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "textFontSizeInheritanceFromUseTest");
        }

        [NUnit.Framework.Test]
        public virtual void TextFontSizeFromUseTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "textFontSizeFromUseTest");
        }

        // Linear gradient tests block
        [NUnit.Framework.Test]
        public virtual void LnrGrdntObjectBoundingBoxEmUnitTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "lnrGrdntObjectBoundingBoxEmUnitTest");
        }

        [NUnit.Framework.Test]
        public virtual void LnrGrdntUserSpaceOnUseEmUnitTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "lnrGrdntUserSpaceOnUseEmUnitTest");
        }

        [NUnit.Framework.Test]
        public virtual void LnrGrdntObjectBoundingBoxRemUnitTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "lnrGrdntObjectBoundingBoxRemUnitTest");
        }

        [NUnit.Framework.Test]
        public virtual void LnrGrdntUserSpaceOnUseRemUnitTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "lnrGrdntUserSpaceOnUseRemUnitTest");
        }

        [NUnit.Framework.Test]
        public virtual void LnrGrdntObjectBoundingBoxEmUnitFromDirectParentTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "lnrGrdntObjectBoundingBoxEmUnitFromDirectParentTest"
                );
        }

        [NUnit.Framework.Test]
        public virtual void LnrGrdntUserSpaceOnUseEmUnitFromDirectParentTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "lnrGrdntUserSpaceOnUseEmUnitFromDirectParentTest"
                );
        }

        [NUnit.Framework.Test]
        public virtual void LnrGrdntFontSizeFromDefsFillTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "lnrGrdntFontSizeFromDefsFillTest");
        }

        // Symbol tests block
        [NUnit.Framework.Test]
        public virtual void SymbolFontSizeInheritanceFromUseTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "symbolFontSizeInheritanceFromUseTest");
        }

        // Marker tests block
        [NUnit.Framework.Test]
        public virtual void MarkerFontSizeInheritanceFromDifsTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "markerFontSizeInheritanceFromDifsTest");
        }
    }
}
