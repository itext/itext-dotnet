using System;
using System.Collections.Generic;
using iText.StyledXmlParser.Css.Parse;
using iText.StyledXmlParser.Css.Selector;
using iText.Test;

namespace iText.StyledXmlParser.Css {
    public class CssRuleSetTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void AddCssRuleSetWithNormalImportantDeclarationsTest() {
            String src = "float:right; clear:right !important;width:22.0em!important; margin:0 0 1.0em 1.0em; " + "background:#f9f9f9; "
                 + "border:1px solid #aaa;padding:0.2em ! important;border-spacing:0.4em 0; text-align:center " + "!important; "
                 + "line-height:1.4em; font-size:88%!  important;";
            String[] expectedNormal = new String[] { "float: right", "margin: 0 0 1.0em 1.0em", "background: #f9f9f9", 
                "border: 1px solid #aaa", "border-spacing: 0.4em 0", "line-height: 1.4em" };
            String[] expectedImportant = new String[] { "clear: right", "width: 22.0em", "padding: 0.2em", "text-align: center"
                , "font-size: 88%" };
            IList<CssDeclaration> declarations = CssRuleSetParser.ParsePropertyDeclarations(src);
            CssSelector selector = new CssSelector("h1");
            CssRuleSet cssRuleSet = new CssRuleSet(selector, declarations);
            IList<CssDeclaration> normalDeclarations = cssRuleSet.GetNormalDeclarations();
            for (int i = 0; i < expectedNormal.Length; i++) {
                NUnit.Framework.Assert.AreEqual(expectedNormal[i], normalDeclarations[i].ToString());
            }
            IList<CssDeclaration> importantDeclarations = cssRuleSet.GetImportantDeclarations();
            for (int i = 0; i < expectedImportant.Length; i++) {
                NUnit.Framework.Assert.AreEqual(expectedImportant[i], importantDeclarations[i].ToString());
            }
        }
    }
}
