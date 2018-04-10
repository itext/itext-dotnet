using System;

namespace iText.Svg.Utils {
    public class SvgTextUtilTest {
        //Trim leading tests
        [NUnit.Framework.Test]
        public virtual void TrimLeadingTest() {
            String toTrim = "\t \t   to trim  \t";
            String actual = SvgTextUtil.TrimLeadingWhitespace(toTrim);
            String expected = "to trim  \t";
            NUnit.Framework.Assert.AreEqual(expected, actual);
        }

        [NUnit.Framework.Test]
        public virtual void TrimLeadingEmptyTest() {
            String toTrim = "";
            String actual = SvgTextUtil.TrimLeadingWhitespace(toTrim);
            String expected = "";
            NUnit.Framework.Assert.AreEqual(expected, actual);
        }

        [NUnit.Framework.Test]
        public virtual void TrimLeadingNoLeadingTest() {
            String toTrim = "to Test  ";
            String actual = SvgTextUtil.TrimLeadingWhitespace(toTrim);
            String expected = "to Test  ";
            NUnit.Framework.Assert.AreEqual(expected, actual);
        }

        [NUnit.Framework.Test]
        public virtual void TrimLeadingSingleWhiteSpaceTest() {
            String toTrim = " to Test  ";
            String actual = SvgTextUtil.TrimLeadingWhitespace(toTrim);
            String expected = "to Test  ";
            NUnit.Framework.Assert.AreEqual(expected, actual);
        }

        [NUnit.Framework.Test]
        public virtual void TrimLeadingNonBreakingSpaceTest() {
            String toTrim = "\u00A0to Test  ";
            String actual = SvgTextUtil.TrimLeadingWhitespace(toTrim);
            String expected = "\u00A0to Test  ";
            NUnit.Framework.Assert.AreEqual(expected, actual);
        }

        [NUnit.Framework.Test]
        public virtual void TrimLeadingOnlyWhitespaceTest() {
            String toTrim = "\t\t\t   \t\t\t";
            String actual = SvgTextUtil.TrimLeadingWhitespace(toTrim);
            String expected = "";
            NUnit.Framework.Assert.AreEqual(expected, actual);
        }

        [NUnit.Framework.Test]
        public virtual void TrimLeadingLineBreakTest() {
            String toTrim = " \n Test ";
            String actual = SvgTextUtil.TrimLeadingWhitespace(toTrim);
            String expected = "\n Test ";
            NUnit.Framework.Assert.AreEqual(expected, actual);
        }

        //Trim trailing tests
        [NUnit.Framework.Test]
        public virtual void TrimTrailingTest() {
            String toTrim = "\t \t   to trim  \t";
            String actual = SvgTextUtil.TrimTrailingWhitespace(toTrim);
            String expected = "\t \t   to trim";
            NUnit.Framework.Assert.AreEqual(expected, actual);
        }

        [NUnit.Framework.Test]
        public virtual void TrimTrailingEmptyTest() {
            String toTrim = "";
            String actual = SvgTextUtil.TrimTrailingWhitespace(toTrim);
            String expected = "";
            NUnit.Framework.Assert.AreEqual(expected, actual);
        }

        [NUnit.Framework.Test]
        public virtual void TrimTrailingNoTrailingTest() {
            String toTrim = "   to Test";
            String actual = SvgTextUtil.TrimTrailingWhitespace(toTrim);
            String expected = "   to Test";
            NUnit.Framework.Assert.AreEqual(expected, actual);
        }

        [NUnit.Framework.Test]
        public virtual void TrimTrailingSingleWhiteSpaceTest() {
            String toTrim = " to Test ";
            String actual = SvgTextUtil.TrimTrailingWhitespace(toTrim);
            String expected = " to Test";
            NUnit.Framework.Assert.AreEqual(expected, actual);
        }

        [NUnit.Framework.Test]
        public virtual void TrimTrailingNonBreakingSpaceTest() {
            String toTrim = " to Test\u00A0";
            String actual = SvgTextUtil.TrimTrailingWhitespace(toTrim);
            String expected = " to Test\u00A0";
            NUnit.Framework.Assert.AreEqual(expected, actual);
        }

        [NUnit.Framework.Test]
        public virtual void TrimTrailingOnlyWhitespaceTest() {
            String toTrim = "\t\t\t   \t\t\t";
            String actual = SvgTextUtil.TrimTrailingWhitespace(toTrim);
            String expected = "";
            NUnit.Framework.Assert.AreEqual(expected, actual);
        }

        [NUnit.Framework.Test]
        public virtual void TrimTrailingLineBreakTest() {
            String toTrim = " to trim \n";
            String actual = SvgTextUtil.TrimTrailingWhitespace(toTrim);
            String expected = " to trim \n";
            NUnit.Framework.Assert.AreEqual(expected, actual);
        }
    }
}
