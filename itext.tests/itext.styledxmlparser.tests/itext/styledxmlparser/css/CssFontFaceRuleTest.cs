using System;
using System.Collections.Generic;
using iText.Test;

namespace iText.StyledXmlParser.Css {
    public class CssFontFaceRuleTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void VerifyThatToStringProducesValidCss() {
            CssFontFaceRule fontFaceRule = new CssFontFaceRule();
            IList<CssDeclaration> declarations = new List<CssDeclaration>();
            declarations.Add(new CssDeclaration(CommonCssConstants.FONT_FAMILY, "test-font-family"));
            declarations.Add(new CssDeclaration(CommonCssConstants.FONT_WEIGHT, CommonCssConstants.BOLD));
            fontFaceRule.AddBodyCssDeclarations(declarations);
            String expectedCss = "@font-face {\n" + "    font-family: test-font-family;\n" + "    font-weight: bold;\n"
                 + "}";
            NUnit.Framework.Assert.AreEqual(expectedCss, fontFaceRule.ToString());
        }
    }
}
