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
using iText.Test;

namespace iText.Layout.Font {
    [NUnit.Framework.Category("UnitTest")]
    public class FontCharacteristicsUtilsTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void TestNormalizingThinFontWeight() {
            NUnit.Framework.Assert.AreEqual(100, FontCharacteristicsUtils.NormalizeFontWeight((short)-10000));
            NUnit.Framework.Assert.AreEqual(100, FontCharacteristicsUtils.NormalizeFontWeight((short)0));
            NUnit.Framework.Assert.AreEqual(100, FontCharacteristicsUtils.NormalizeFontWeight((short)50));
            NUnit.Framework.Assert.AreEqual(100, FontCharacteristicsUtils.NormalizeFontWeight((short)100));
        }

        [NUnit.Framework.Test]
        public virtual void TestNormalizingHeavyFontWeight() {
            NUnit.Framework.Assert.AreEqual(900, FontCharacteristicsUtils.NormalizeFontWeight((short)900));
            NUnit.Framework.Assert.AreEqual(900, FontCharacteristicsUtils.NormalizeFontWeight((short)1600));
            NUnit.Framework.Assert.AreEqual(900, FontCharacteristicsUtils.NormalizeFontWeight((short)23000));
        }

        [NUnit.Framework.Test]
        public virtual void TestNormalizingNormalFontWeight() {
            NUnit.Framework.Assert.AreEqual(200, FontCharacteristicsUtils.NormalizeFontWeight((short)220));
            NUnit.Framework.Assert.AreEqual(400, FontCharacteristicsUtils.NormalizeFontWeight((short)456));
            NUnit.Framework.Assert.AreEqual(500, FontCharacteristicsUtils.NormalizeFontWeight((short)550));
            NUnit.Framework.Assert.AreEqual(600, FontCharacteristicsUtils.NormalizeFontWeight((short)620));
            NUnit.Framework.Assert.AreEqual(700, FontCharacteristicsUtils.NormalizeFontWeight((short)780));
        }

        [NUnit.Framework.Test]
        public virtual void TestParsingIncorrectFontWeight() {
            NUnit.Framework.Assert.AreEqual((short)-1, FontCharacteristicsUtils.ParseFontWeight(""));
            NUnit.Framework.Assert.AreEqual((short)-1, FontCharacteristicsUtils.ParseFontWeight(null));
            NUnit.Framework.Assert.AreEqual((short)-1, FontCharacteristicsUtils.ParseFontWeight("dfgdgdfgdfgdf"));
            NUnit.Framework.Assert.AreEqual((short)-1, FontCharacteristicsUtils.ParseFontWeight("italic"));
        }

        [NUnit.Framework.Test]
        public virtual void TestParsingNumberFontWeight() {
            NUnit.Framework.Assert.AreEqual((short)100, FontCharacteristicsUtils.ParseFontWeight("-1"));
            NUnit.Framework.Assert.AreEqual((short)100, FontCharacteristicsUtils.ParseFontWeight("50"));
            NUnit.Framework.Assert.AreEqual((short)300, FontCharacteristicsUtils.ParseFontWeight("360"));
            NUnit.Framework.Assert.AreEqual((short)900, FontCharacteristicsUtils.ParseFontWeight("25000"));
        }

        [NUnit.Framework.Test]
        public virtual void TestParseAllowedFontWeight() {
            NUnit.Framework.Assert.AreEqual((short)400, FontCharacteristicsUtils.ParseFontWeight("normal"));
            NUnit.Framework.Assert.AreEqual((short)700, FontCharacteristicsUtils.ParseFontWeight("bold"));
        }
    }
}
