using System;

namespace iText.StyledXmlParser.Util {
    public class WhiteSpaceUtilUnitTest {
        [NUnit.Framework.Test]
        public virtual void CollapseConsecutiveWhiteSpacesTest() {
            String toCollapse = "A   B";
            String actual = WhiteSpaceUtil.CollapseConsecutiveSpaces(toCollapse);
            String expected = "A B";
            NUnit.Framework.Assert.AreEqual(expected, actual);
        }

        [NUnit.Framework.Test]
        public virtual void CollapseConsecutiveWhiteSpacesTrailingWhiteSpaceTest() {
            String toCollapse = "A   B   ";
            String actual = WhiteSpaceUtil.CollapseConsecutiveSpaces(toCollapse);
            String expected = "A B ";
            NUnit.Framework.Assert.AreEqual(expected, actual);
        }

        [NUnit.Framework.Test]
        public virtual void CollapseConsecutiveWhiteSpacesPrecedingWhiteSpaceTest() {
            String toCollapse = "   A B";
            String actual = WhiteSpaceUtil.CollapseConsecutiveSpaces(toCollapse);
            String expected = " A B";
            NUnit.Framework.Assert.AreEqual(expected, actual);
        }

        [NUnit.Framework.Test]
        public virtual void CollapseConsecutiveWhiteSpacesPrecedingAndTrailingWhiteSpaceTest() {
            String toCollapse = "   A   B   ";
            String actual = WhiteSpaceUtil.CollapseConsecutiveSpaces(toCollapse);
            String expected = " A B ";
            NUnit.Framework.Assert.AreEqual(expected, actual);
        }

        [NUnit.Framework.Test]
        public virtual void CollapseConsecutiveWhiteSpacesNewLineWhiteSpaceTest() {
            String toCollapse = "\n   A B  \n";
            String actual = WhiteSpaceUtil.CollapseConsecutiveSpaces(toCollapse);
            String expected = " A B ";
            NUnit.Framework.Assert.AreEqual(expected, actual);
        }

        [NUnit.Framework.Test]
        public virtual void CollapseConsecutiveWhiteSpacesTabWhiteSpaceTest() {
            String toCollapse = "\t  A B  \t";
            String actual = WhiteSpaceUtil.CollapseConsecutiveSpaces(toCollapse);
            String expected = " A B ";
            NUnit.Framework.Assert.AreEqual(expected, actual);
        }
    }
}
