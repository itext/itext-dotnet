/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
Authors: iText Software.

This program is offered under a commercial and under the AGPL license.
For commercial licensing, contact us at https://itextpdf.com/sales.  For AGPL licensing, see below.

AGPL licensing:
This program is free software: you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU Affero General Public License for more details.

You should have received a copy of the GNU Affero General Public License
along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/
using System;
using System.Collections.Generic;
using iText.StyledXmlParser.Css.Parse;
using iText.StyledXmlParser.Css.Selector;
using iText.Test;

namespace iText.StyledXmlParser.Css {
    [NUnit.Framework.Category("UnitTest")]
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
