using System.Text;
using iText.Kernel.Numbering;

namespace iText.Kernel.Pdf.Numbering {
    public class GreekAlphabetNumberingTest {
        [NUnit.Framework.Test]
        public virtual void TestUpperCase() {
            StringBuilder builder = new StringBuilder();
            for (int i = 1; i <= 25; i++) {
                builder.Append(GreekAlphabetNumbering.ToGreekAlphabetNumber(i, true));
            }
            // 25th symbol is `AA`, i.e. alphabet has 24 letters.
            NUnit.Framework.Assert.AreEqual("ΑΒΓΔΕΖΗΘΙΚΛΜΝΞΟΠΡΣΤΥΦΧΨΩΑΑ", builder.ToString());
        }

        [NUnit.Framework.Test]
        public virtual void TestLowerCase() {
            StringBuilder builder = new StringBuilder();
            for (int i = 1; i <= 25; i++) {
                builder.Append(GreekAlphabetNumbering.ToGreekAlphabetNumber(i, false));
            }
            // 25th symbol is `αα`, i.e. alphabet has 24 letters.
            NUnit.Framework.Assert.AreEqual("αβγδεζηθικλμνξοπρστυφχψωαα", builder.ToString());
        }

        [NUnit.Framework.Test]
        public virtual void TestUpperCaseSymbol() {
            StringBuilder builder = new StringBuilder();
            for (int i = 1; i <= 25; i++) {
                builder.Append(GreekAlphabetNumbering.ToGreekAlphabetNumber(i, true, true));
            }
            // Symbol font use regular WinAnsi codes for greek letters.
            NUnit.Framework.Assert.AreEqual("ABGDEZHQIKLMNXOPRSTUFCYWAA", builder.ToString());
        }

        [NUnit.Framework.Test]
        public virtual void TestLowerCaseSymbol() {
            StringBuilder builder = new StringBuilder();
            for (int i = 1; i <= 25; i++) {
                builder.Append(GreekAlphabetNumbering.ToGreekAlphabetNumber(i, false, true));
            }
            // Symbol font use regular WinAnsi codes for greek letters.
            NUnit.Framework.Assert.AreEqual("abgdezhqiklmnxoprstufcywaa", builder.ToString());
        }
    }
}
