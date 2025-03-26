using System;
using iText.Svg.Renderers;
using iText.Test;

namespace iText.Svg.Css {
    [NUnit.Framework.Category("IntegrationTest")]
    public class CssVariableTest : SvgIntegrationTest {
        private static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/svg/css/CssVariableTest/";

        private static readonly String DESTINATION_FOLDER = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/svg/css/CssVariableTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            ITextTest.CreateDestinationFolder(DESTINATION_FOLDER);
        }

        [NUnit.Framework.Test]
        public virtual void CircleWithVariablesTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "circleWithVariables");
        }

        [NUnit.Framework.Test]
        public virtual void CircleWithVariablesInDefsTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "circleWithVariablesInDefs");
        }

        [NUnit.Framework.Test]
        public virtual void CircleWithVariablesInDefsWithInnerSvgTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "circleWithVariablesInDefsWithInnerSvg");
        }

        [NUnit.Framework.Test]
        public virtual void SvgWithVariablesInShorthandTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "svgWithVariablesInShorthand");
        }

        [NUnit.Framework.Test]
        public virtual void SvgWithVariablesAsShorthandTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "svgWithVariablesAsShorthand");
        }

        [NUnit.Framework.Test]
        public virtual void RootSelectorVariablesTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "rootSelectorVariables");
        }

        [NUnit.Framework.Test]
        public virtual void VariablesInStyleAttributeTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "variablesInStyleAttribute");
        }

        [NUnit.Framework.Test]
        public virtual void SymbolInheritanceTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "symbolInheritance");
        }
    }
}
