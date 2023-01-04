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
using iText.Layout.Font;
using iText.Test;

namespace iText.StyledXmlParser.Css {
    [NUnit.Framework.Category("UnitTest")]
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

        [NUnit.Framework.Test]
        public virtual void ResolveUnicodeRangeTest() {
            CssFontFaceRule fontFaceRule = new CssFontFaceRule();
            IList<CssDeclaration> declarations = new List<CssDeclaration>();
            declarations.Add(new CssDeclaration("unicode-range", "U+75"));
            fontFaceRule.AddBodyCssDeclarations(declarations);
            Range range = fontFaceRule.ResolveUnicodeRange();
            NUnit.Framework.Assert.IsNotNull(range);
            NUnit.Framework.Assert.IsTrue(range.Contains(117));
        }
    }
}
