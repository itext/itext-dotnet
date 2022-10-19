/*
This file is part of the iText (R) project.
Copyright (c) 1998-2022 iText Group NV
Authors: iText Software.

This program is free software; you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License version 3
as published by the Free Software Foundation with the addition of the
following permission added to Section 15 as permitted in Section 7(a):
FOR ANY PART OF THE COVERED WORK IN WHICH THE COPYRIGHT IS OWNED BY
ITEXT GROUP. ITEXT GROUP DISCLAIMS THE WARRANTY OF NON INFRINGEMENT
OF THIRD PARTY RIGHTS

This program is distributed in the hope that it will be useful, but
WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
or FITNESS FOR A PARTICULAR PURPOSE.
See the GNU Affero General Public License for more details.
You should have received a copy of the GNU Affero General Public License
along with this program; if not, see http://www.gnu.org/licenses or write to
the Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor,
Boston, MA, 02110-1301 USA, or download the license from the following URL:
http://itextpdf.com/terms-of-use/

The interactive user interfaces in modified source and object code versions
of this program must display Appropriate Legal Notices, as required under
Section 5 of the GNU Affero General Public License.

In accordance with Section 7(b) of the GNU Affero General Public License,
a covered work must retain the producer line in every PDF that is created
or manipulated using iText.

You can be released from the requirements of the license by purchasing
a commercial license. Buying such a license is mandatory as soon as you
develop commercial activities involving the iText software without
disclosing the source code of your own applications.
These activities include: offering paid services to customers as an ASP,
serving PDFs on the fly in a web application, shipping iText with a closed
source product.

For more information, please contact iText Software Corp. at this
address: sales@itextpdf.com
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
            RunTest("func(param)", JavaUtil.ArraysAsList("func(param)"), JavaUtil.ArraysAsList(CssDeclarationValueTokenizer.TokenType
                .FUNCTION));
        }

        [NUnit.Framework.Test]
        public virtual void FunctionTest02() {
            RunTest("func(param1, param2)", JavaUtil.ArraysAsList("func(param1, param2)"), JavaUtil.ArraysAsList(CssDeclarationValueTokenizer.TokenType
                .FUNCTION));
        }

        [NUnit.Framework.Test]
        public virtual void FunctionTest03() {
            RunTest("func(param,'param)',\"param))\")", JavaUtil.ArraysAsList("func(param,'param)',\"param))\")"), JavaUtil.ArraysAsList
                (CssDeclarationValueTokenizer.TokenType.FUNCTION));
        }

        [NUnit.Framework.Test]
        public virtual void FunctionTest04() {
            RunTest("func(param, innerFunc())", JavaUtil.ArraysAsList("func(param, innerFunc())"), JavaUtil.ArraysAsList
                (CssDeclarationValueTokenizer.TokenType.FUNCTION));
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
