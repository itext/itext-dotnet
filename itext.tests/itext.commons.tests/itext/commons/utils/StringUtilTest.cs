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

namespace iText.Commons.Utils {
    /// <summary>At the moment there is no StringUtil class in Java, but there is one in C# and we are testing</summary>
    [NUnit.Framework.Category("UnitTest")]
    public class StringUtilTest : ExtendedITextTest {
        private const char SPLIT_PERIOD = '.';

        [NUnit.Framework.Test]
        public virtual void PatternSplitTest01() {
            // Android-Conversion-Ignore-Test (TODO DEVSIX-6457 fix different behavior of Pattern.split method)
            // Pattern.split in Java works differently compared to Regex.Split in C#
            // In C#, empty strings are possible at the beginning of the resultant array for non-capturing groups in
            // split regex
            // Thus, in C# we use a separate utility for splitting to align the implementation with Java
            // This test verifies that the resultant behavior is the same
            Regex pattern = iText.Commons.Utils.StringUtil.RegexCompile("(?=[ab])");
            String source = "a01aa78ab89b";
            String[] expected = new String[] { "a01", "a", "a78", "a", "b89", "b" };
            String[] result = iText.Commons.Utils.StringUtil.Split(pattern, source);
            NUnit.Framework.Assert.AreEqual(expected, result);
        }

        [NUnit.Framework.Test]
        public virtual void PatternSplitTest02() {
            Regex pattern = iText.Commons.Utils.StringUtil.RegexCompile("(?=[ab])");
            String source = "";
            String[] expected = new String[] { "" };
            String[] result = iText.Commons.Utils.StringUtil.Split(pattern, source);
            NUnit.Framework.Assert.AreEqual(expected, result);
        }

        [NUnit.Framework.Test]
        public virtual void StringSplitTest01() {
            // Android-Conversion-Ignore-Test (TODO DEVSIX-6457 fix different behavior of Pattern.split method)
            String source = "a01aa78ab89b";
            String[] expected = new String[] { "a01", "a", "a78", "a", "b89", "b" };
            String[] result = iText.Commons.Utils.StringUtil.Split(source, "(?=[ab])");
            NUnit.Framework.Assert.AreEqual(expected, result);
        }

        [NUnit.Framework.Test]
        public virtual void StringSplitTest02() {
            String source = "";
            String[] expected = new String[] { "" };
            String[] result = iText.Commons.Utils.StringUtil.Split(source, "(?=[ab])");
            NUnit.Framework.Assert.AreEqual(expected, result);
        }

        [NUnit.Framework.Test]
        public virtual void SplitKeepEmptyParts01() {
            String source = "";
            String[] expected = new String[] { "" };
            String[] result = StringSplitUtil.SplitKeepTrailingWhiteSpace(source, SPLIT_PERIOD);
            NUnit.Framework.Assert.AreEqual(iText.Commons.Utils.StringUtil.Split(source, SPLIT_PERIOD.ToString()), result
                );
            NUnit.Framework.Assert.AreEqual(expected, result);
        }

        [NUnit.Framework.Test]
        public virtual void SplitKeepEmptyParts02() {
            String source = null;
            NUnit.Framework.Assert.Catch(typeof(Exception), () => StringSplitUtil.SplitKeepTrailingWhiteSpace(source, 
                SPLIT_PERIOD));
        }

        [NUnit.Framework.Test]
        public virtual void SplitKeepEmptyParts03() {
            String source = "test.test1";
            String[] expected = new String[] { "test", "test1" };
            String[] result = StringSplitUtil.SplitKeepTrailingWhiteSpace(source, SPLIT_PERIOD);
            NUnit.Framework.Assert.AreEqual(expected, result);
        }

        [NUnit.Framework.Test]
        public virtual void SplitKeepEmptyParts04() {
            String source = "test..test1";
            String[] expected = new String[] { "test", "", "test1" };
            String[] result = StringSplitUtil.SplitKeepTrailingWhiteSpace(source, SPLIT_PERIOD);
            NUnit.Framework.Assert.AreEqual(expected, result);
        }

        [NUnit.Framework.Test]
        public virtual void SplitKeepEmptyParts05() {
            String source = "test...test1";
            String[] expected = new String[] { "test", "", "", "test1" };
            String[] result = StringSplitUtil.SplitKeepTrailingWhiteSpace(source, SPLIT_PERIOD);
            NUnit.Framework.Assert.AreEqual(expected, result);
        }

        [NUnit.Framework.Test]
        public virtual void SplitKeepEmptyParts06() {
            String source = ".test1";
            String[] expected = new String[] { "", "test1" };
            String[] result = StringSplitUtil.SplitKeepTrailingWhiteSpace(source, SPLIT_PERIOD);
            NUnit.Framework.Assert.AreEqual(expected, result);
        }

        [NUnit.Framework.Test]
        public virtual void SplitKeepEmptyPartsDifferentBehaviour01() {
            String source = "test.";
            String[] expected = new String[] { "test", "" };
            String[] result = StringSplitUtil.SplitKeepTrailingWhiteSpace(source, SPLIT_PERIOD);
            NUnit.Framework.Assert.AreEqual(expected, result);
        }

        [NUnit.Framework.Test]
        public virtual void SplitKeepEmptyPartsDifferentBehaviour02() {
            String source = "test..";
            String[] expected = new String[] { "test", "", "" };
            String[] result = StringSplitUtil.SplitKeepTrailingWhiteSpace(source, SPLIT_PERIOD);
            NUnit.Framework.Assert.AreEqual(expected, result);
        }
    }
}
