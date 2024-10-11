/*
This file is part of the iText (R) project.
Copyright (c) 1998-2024 Apryse Group NV
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
using iText.Test;

namespace iText.Commons.Utils {
    [NUnit.Framework.Category("UnitTest")]
    public class MessageFormatUtilTest : ExtendedITextTest {
        public static IEnumerable<Object[]> DataSource() {
            return JavaUtil.ArraysAsList(new Object[][] { new Object[] { "Plain message with params 1 test", "Plain message with params {0} {1}"
                , new Object[] { 1, "test" }, "test with simple params" }, new Object[] { "Message with 'single quotes'"
                , "Message with 'single quotes'", new Object[0], "test with single quotes" }, new Object[] { "Message with ''doubled single quotes''"
                , "Message with ''doubled single quotes''", new Object[0], "test with doubled single quotes" }, new Object
                [] { "Message with {curly braces} and a parameter {I'm between curly braces too}", "Message with {{curly braces}} and a parameter {{{0}}}"
                , new Object[] { "I'm between curly braces too" }, "Test with curly braces" }, new Object[] { "'{value}'"
                , "'{{{0}}}'", new Object[] { "value" }, "Mix om multiple brackets and quotes 1" }, new Object[] { "'value'"
                , "'{0}'", new Object[] { "value" }, "Mix of brackets and quotes" }, new Object[] { "{'0'}", "{{'0'}}"
                , new Object[0], "Mix of multiple brackets and quotes 2" }, new Object[] { "single opening brace {0 test"
                , "single opening brace {{0 test", new Object[0], "Test single opening brace" }, new Object[] { "single closing  brace 0} test"
                , "single closing  brace 0}} test", new Object[0], "Test single closing brace" }, new Object[] { "single opening + closing  brace {  test  }"
                , "single opening + closing  brace {{  {0}  }}", new Object[] { "test" }, "Test single opening and closing brace"
                 } });
        }

        [NUnit.Framework.TestCaseSource("DataSource")]
        public virtual void TestFormatting(String expectedResult, String pattern, Object[] arguments, String name) {
            NUnit.Framework.Assert.AreEqual(expectedResult, MessageFormatUtil.Format(pattern, arguments));
        }
    }
}
