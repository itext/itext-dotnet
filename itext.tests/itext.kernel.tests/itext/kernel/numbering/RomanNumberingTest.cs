using System;
using iText.Test;

namespace iText.Kernel.Numbering {
    public class RomanNumberingTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void NegativeConvertTest() {
            NUnit.Framework.Assert.AreEqual("-vi", RomanNumbering.Convert(-6));
        }

        [NUnit.Framework.Test]
        public virtual void ZeroConvertTest() {
            NUnit.Framework.Assert.AreEqual("", RomanNumbering.Convert(0));
        }

        [NUnit.Framework.Test]
        public virtual void ConvertTest() {
            NUnit.Framework.Assert.AreEqual("mdclxvi", RomanNumbering.Convert(1666));
            NUnit.Framework.Assert.AreEqual("mcmlxxxiii", RomanNumbering.Convert(1983));
            NUnit.Framework.Assert.AreEqual("mmm", RomanNumbering.Convert(3000));
            NUnit.Framework.Assert.AreEqual("|vi|", RomanNumbering.Convert(6000));
            NUnit.Framework.Assert.AreEqual("|vi|dccxxxiii", RomanNumbering.Convert(6733));
        }

        [NUnit.Framework.Test]
        public virtual void ToRomanTest() {
            String expected = "dcclvi";
            NUnit.Framework.Assert.AreEqual(expected.ToUpperInvariant(), RomanNumbering.ToRoman(756, true));
            NUnit.Framework.Assert.AreEqual(expected.ToLowerInvariant(), RomanNumbering.ToRoman(756, false));
        }

        [NUnit.Framework.Test]
        public virtual void ToRomanUpperCaseTest() {
            NUnit.Framework.Assert.AreEqual("CCCLXXXVI", RomanNumbering.ToRomanUpperCase(386));
        }

        [NUnit.Framework.Test]
        public virtual void ToRomanLowerCaseTest() {
            NUnit.Framework.Assert.AreEqual("xxvi", RomanNumbering.ToRomanLowerCase(26));
        }
    }
}
