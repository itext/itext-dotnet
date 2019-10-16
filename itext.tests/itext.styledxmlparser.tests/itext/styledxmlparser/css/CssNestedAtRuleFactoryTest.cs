using iText.StyledXmlParser.Css.Page;
using iText.Test;

namespace iText.StyledXmlParser.Css {
    public class CssNestedAtRuleFactoryTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void TestCreatingNestedRule() {
            CssNestedAtRule pageRule = CssNestedAtRuleFactory.CreateNestedRule("page:first");
            NUnit.Framework.Assert.IsTrue(pageRule is CssPageRule);
            NUnit.Framework.Assert.AreEqual(CssRuleName.PAGE, pageRule.GetRuleName());
            NUnit.Framework.Assert.AreEqual(":first", pageRule.GetRuleParameters());
            CssNestedAtRule rightBottomMarginRule = CssNestedAtRuleFactory.CreateNestedRule("bottom-right");
            NUnit.Framework.Assert.IsTrue(rightBottomMarginRule is CssMarginRule);
            NUnit.Framework.Assert.AreEqual(CssRuleName.BOTTOM_RIGHT, rightBottomMarginRule.GetRuleName());
            CssNestedAtRule fontFaceRule = CssNestedAtRuleFactory.CreateNestedRule("font-face");
            NUnit.Framework.Assert.IsTrue(fontFaceRule is CssFontFaceRule);
            NUnit.Framework.Assert.AreEqual(CssRuleName.FONT_FACE, fontFaceRule.GetRuleName());
        }
    }
}
