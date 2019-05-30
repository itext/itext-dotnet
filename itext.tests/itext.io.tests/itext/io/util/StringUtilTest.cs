using System;
using System.Text.RegularExpressions;

namespace iText.IO.Util {
    /// <summary>At the moment there is no StringUtil class in Java, but there is one in C# and we are testing</summary>
    public class StringUtilTest {
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
