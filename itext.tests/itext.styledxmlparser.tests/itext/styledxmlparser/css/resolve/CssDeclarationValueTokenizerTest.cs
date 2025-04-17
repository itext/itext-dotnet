/*
This file is part of the iText (R) project.
Copyright (c) 1998-2025 Apryse Group NV
Authors: Apryse Software.

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
using iText.Commons.Utils;
using iText.StyledXmlParser.Css.Parse;
using iText.Test;

namespace iText.StyledXmlParser.Css.Resolve {
    [NUnit.Framework.Category("UnitTest")]
    public class CssDeclarationValueTokenizerTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void FunctionTest01() {
            RunTest("func(param)", JavaCollectionsUtil.SingletonList("func(param)"), JavaCollectionsUtil.SingletonList
                (CssDeclarationValueTokenizer.TokenType.FUNCTION));
        }

        [NUnit.Framework.Test]
        public virtual void FunctionTest02() {
            RunTest("func(param1, param2)", JavaCollectionsUtil.SingletonList("func(param1, param2)"), JavaCollectionsUtil
                .SingletonList(CssDeclarationValueTokenizer.TokenType.FUNCTION));
        }

        [NUnit.Framework.Test]
        public virtual void FunctionTest03() {
            RunTest("func(param,'param)',\"param))\")", JavaCollectionsUtil.SingletonList("func(param,'param)',\"param))\")"
                ), JavaCollectionsUtil.SingletonList(CssDeclarationValueTokenizer.TokenType.FUNCTION));
        }

        [NUnit.Framework.Test]
        public virtual void FunctionTest04() {
            RunTest("func(param, innerFunc())", JavaCollectionsUtil.SingletonList("func(param, innerFunc())"), JavaCollectionsUtil
                .SingletonList(CssDeclarationValueTokenizer.TokenType.FUNCTION));
        }

        [NUnit.Framework.Test]
        public virtual void FunctionTest05() {
            RunTest(") )) function()", JavaUtil.ArraysAsList(")", "))", "function()"), JavaUtil.ArraysAsList(CssDeclarationValueTokenizer.TokenType
                .UNKNOWN, CssDeclarationValueTokenizer.TokenType.UNKNOWN, CssDeclarationValueTokenizer.TokenType.FUNCTION
                ));
        }

        [NUnit.Framework.Test]
        public virtual void FunctionTest06() {
            RunTest("a('x'), b('x')", JavaUtil.ArraysAsList("a('x')", ",", "b('x')"), JavaUtil.ArraysAsList(CssDeclarationValueTokenizer.TokenType
                .FUNCTION, CssDeclarationValueTokenizer.TokenType.COMMA, CssDeclarationValueTokenizer.TokenType.FUNCTION
                ));
        }

        [NUnit.Framework.Test]
        public virtual void StringTest01() {
            RunTest("'a b c'", JavaCollectionsUtil.SingletonList("a b c"), JavaCollectionsUtil.SingletonList(CssDeclarationValueTokenizer.TokenType
                .STRING));
        }

        [NUnit.Framework.Test]
        public virtual void StringTest02() {
            RunTest("\"a b c\"", JavaCollectionsUtil.SingletonList("a b c"), JavaCollectionsUtil.SingletonList(CssDeclarationValueTokenizer.TokenType
                .STRING));
        }

        [NUnit.Framework.Test]
        public virtual void StringTest03() {
            RunTest("[ aa  bb  cc ]", JavaCollectionsUtil.SingletonList("[ aa  bb  cc ]"), JavaCollectionsUtil.SingletonList
                (CssDeclarationValueTokenizer.TokenType.STRING));
        }

        [NUnit.Framework.Test]
        public virtual void StringTest04() {
            RunTest("[aa bb cc] [dd ee] 'ff ff'", JavaUtil.ArraysAsList("[aa bb cc]", "[dd ee]", "ff ff"), JavaUtil.ArraysAsList
                (CssDeclarationValueTokenizer.TokenType.STRING, CssDeclarationValueTokenizer.TokenType.STRING, CssDeclarationValueTokenizer.TokenType
                .STRING));
        }

        [NUnit.Framework.Test]
        public virtual void FunctionWithSquareBracketsTest04() {
            RunTest("'prefix' repeat(3, [aa bb cc] 2 [dd ee] 3) 'ff ff'", JavaUtil.ArraysAsList("prefix", "repeat(3, [aa bb cc] 2 [dd ee] 3)"
                , "ff ff"), JavaUtil.ArraysAsList(CssDeclarationValueTokenizer.TokenType.STRING, CssDeclarationValueTokenizer.TokenType
                .FUNCTION, CssDeclarationValueTokenizer.TokenType.STRING));
        }

        [NUnit.Framework.Test]
        public virtual void ClosingQuoteInsideStringTest() {
            RunTest("a(\"a12x\")\"", JavaCollectionsUtil.SingletonList("a(\"a12x\")"), JavaCollectionsUtil.SingletonList
                (CssDeclarationValueTokenizer.TokenType.FUNCTION));
        }

        [NUnit.Framework.Test]
        public virtual void SpaceAfterFunctionTest() {
            RunTest("a(\"a12x\") ,", JavaUtil.ArraysAsList("a(\"a12x\")", ","), JavaUtil.ArraysAsList(CssDeclarationValueTokenizer.TokenType
                .FUNCTION, CssDeclarationValueTokenizer.TokenType.COMMA));
        }

        [NUnit.Framework.Test]
        public virtual void SpaceAfterFunction123Test() {
            RunTest("a(\"a12x\") bold", JavaUtil.ArraysAsList("a(\"a12x\")", "bold"), JavaUtil.ArraysAsList(CssDeclarationValueTokenizer.TokenType
                .FUNCTION, CssDeclarationValueTokenizer.TokenType.FUNCTION));
        }

        [NUnit.Framework.Test]
        public virtual void ClosingSquareBracketOutsideStringTest() {
            RunTest("a[\"a12x\"] ,", JavaUtil.ArraysAsList("a[", "a12x", "]", ","), JavaUtil.ArraysAsList(CssDeclarationValueTokenizer.TokenType
                .FUNCTION, CssDeclarationValueTokenizer.TokenType.STRING, CssDeclarationValueTokenizer.TokenType.STRING
                , CssDeclarationValueTokenizer.TokenType.COMMA));
        }

        [NUnit.Framework.Test]
        public virtual void WhitespaceTest() {
            RunTest("a[\"a12x\"]    ", JavaUtil.ArraysAsList("a[", "a12x", "]"), JavaUtil.ArraysAsList(CssDeclarationValueTokenizer.TokenType
                .FUNCTION, CssDeclarationValueTokenizer.TokenType.STRING, CssDeclarationValueTokenizer.TokenType.STRING
                ));
        }

        [NUnit.Framework.Test]
        public virtual void QuoteInsideFunctionTest() {
            RunTest("a(\"a12x\"),", JavaUtil.ArraysAsList("a(\"a12x\")", ","), JavaUtil.ArraysAsList(CssDeclarationValueTokenizer.TokenType
                .FUNCTION, CssDeclarationValueTokenizer.TokenType.COMMA));
        }

        [NUnit.Framework.Test]
        public virtual void TriplingQuotesFunctionTest() {
            RunTest("p:not([class*=\"\"])", JavaCollectionsUtil.SingletonList("p:not([class*=\"\"])"), JavaCollectionsUtil
                .SingletonList(CssDeclarationValueTokenizer.TokenType.FUNCTION));
        }

        private void RunTest(String src, IList<String> tokenValues, IList<CssDeclarationValueTokenizer.TokenType> 
            tokenTypes) {
            CssDeclarationValueTokenizer tokenizer = new CssDeclarationValueTokenizer(src);
            CssDeclarationValueTokenizer.Token token = null;
            NUnit.Framework.Assert.IsTrue(tokenValues.Count == tokenTypes.Count, "Value and type arrays size should be equal"
                );
            int index = 0;
            while ((token = tokenizer.GetNextValidToken()) != null) {
                NUnit.Framework.Assert.AreEqual(tokenValues[index], token.GetValue());
                NUnit.Framework.Assert.AreEqual(tokenTypes[index], token.GetType());
                ++index;
            }
            NUnit.Framework.Assert.IsTrue(index == tokenValues.Count);
        }
    }
}
