/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 Apryse Group NV
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
using System.Text.RegularExpressions;
using iText.Test;

namespace iText.IO.Util {
    /// <summary>At the moment there is no StringUtil class in Java, but there is one in C# and we are testing</summary>
    public class StringUtilTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void PatternSplitTest01() {
            // Pattern.split in Java works differently compared to Regex.Split in C#
            // In C#, empty strings are possible at the beginning of the resultant array for non-capturing groups in split regex
            // Thus, in C# we use a separate utility for splitting to align the implementation with Java
            // This test verifies that the resultant behavior is the same
            Regex pattern = iText.IO.Util.StringUtil.RegexCompile("(?=[ab])");
            String source = "a01aa78ab89b";
            String[] expected = new String[] { "a01", "a", "a78", "a", "b89", "b" };
            String[] result = iText.IO.Util.StringUtil.Split(pattern, source);
            NUnit.Framework.Assert.AreEqual(expected, result);
        }

        [NUnit.Framework.Test]
        public virtual void PatternSplitTest02() {
            Regex pattern = iText.IO.Util.StringUtil.RegexCompile("(?=[ab])");
            String source = "";
            String[] expected = new String[] { "" };
            String[] result = iText.IO.Util.StringUtil.Split(pattern, source);
            NUnit.Framework.Assert.AreEqual(expected, result);
        }

        [NUnit.Framework.Test]
        public virtual void StringSplitTest01() {
            String source = "a01aa78ab89b";
            String[] expected = new String[] { "a01", "a", "a78", "a", "b89", "b" };
            String[] result = iText.IO.Util.StringUtil.Split(source, "(?=[ab])");
            NUnit.Framework.Assert.AreEqual(expected, result);
        }

        [NUnit.Framework.Test]
        public virtual void StringSplitTest02() {
            String source = "";
            String[] expected = new String[] { "" };
            String[] result = iText.IO.Util.StringUtil.Split(source, "(?=[ab])");
            NUnit.Framework.Assert.AreEqual(expected, result);
        }
    }
}
