using System;
using System.Collections.Generic;
using iText.StyledXmlParser.Css;
using iText.Test;

namespace iText.StyledXmlParser.Css.Parse {
    public class CssRuleSetParserTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void ParsePropertyDeclarationsTest() {
            String src = "float:right; clear:right;width:22.0em; margin:0 0 1.0em 1.0em; background:#f9f9f9; " + "border:1px solid #aaa;padding:0.2em;border-spacing:0.4em 0; text-align:center; "
                 + "line-height:1.4em; font-size:88%;";
            String[] expected = new String[] { "float: right", "clear: right", "width: 22.0em", "margin: 0 0 1.0em 1.0em"
                , "background: #f9f9f9", "border: 1px solid #aaa", "padding: 0.2em", "border-spacing: 0.4em 0", "text-align: center"
                , "line-height: 1.4em", "font-size: 88%" };
            IList<CssDeclaration> declarations = CssRuleSetParser.ParsePropertyDeclarations(src);
            NUnit.Framework.Assert.AreEqual(expected.Length, declarations.Count);
            for (int i = 0; i < expected.Length; i++) {
                NUnit.Framework.Assert.AreEqual(expected[i], declarations[i].ToString());
            }
        }
    }
}
