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
using iText.Test;

namespace iText.Kernel.Numbering {
    [NUnit.Framework.Category("UnitTest")]
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
