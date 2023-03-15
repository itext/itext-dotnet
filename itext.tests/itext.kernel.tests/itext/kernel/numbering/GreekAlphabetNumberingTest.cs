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
using System.Text;
using iText.Test;

namespace iText.Kernel.Numbering {
    [NUnit.Framework.Category("UnitTest")]
    public class GreekAlphabetNumberingTest : ExtendedITextTest {
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

        [NUnit.Framework.Test]
        public virtual void IntIsNotEnoughForInternalCalculationsTest() {
            NUnit.Framework.Assert.AreEqual("ζλαββωσ", GreekAlphabetNumbering.ToGreekAlphabetNumberLowerCase(1234567890
                ));
        }
    }
}
