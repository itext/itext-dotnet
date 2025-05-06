/*
This file is part of the iText (R) project.
Copyright (c) 1998-2025 Apryse Group NV
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

namespace iText.IO.Font.Constants {
    [NUnit.Framework.Category("UnitTest")]
    public class FontWeightsTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void FromType1FontWeightTest() {
            NUnit.Framework.Assert.AreEqual(FontWeights.THIN, FontWeights.FromType1FontWeight("ultralight"));
            NUnit.Framework.Assert.AreEqual(FontWeights.EXTRA_LIGHT, FontWeights.FromType1FontWeight("thin"));
            NUnit.Framework.Assert.AreEqual(FontWeights.EXTRA_LIGHT, FontWeights.FromType1FontWeight("extralight"));
            NUnit.Framework.Assert.AreEqual(FontWeights.LIGHT, FontWeights.FromType1FontWeight("light"));
            NUnit.Framework.Assert.AreEqual(FontWeights.NORMAL, FontWeights.FromType1FontWeight("book"));
            NUnit.Framework.Assert.AreEqual(FontWeights.NORMAL, FontWeights.FromType1FontWeight("regular"));
            NUnit.Framework.Assert.AreEqual(FontWeights.NORMAL, FontWeights.FromType1FontWeight("normal"));
            NUnit.Framework.Assert.AreEqual(FontWeights.MEDIUM, FontWeights.FromType1FontWeight("medium"));
            NUnit.Framework.Assert.AreEqual(FontWeights.SEMI_BOLD, FontWeights.FromType1FontWeight("demibold"));
            NUnit.Framework.Assert.AreEqual(FontWeights.SEMI_BOLD, FontWeights.FromType1FontWeight("semibold"));
            NUnit.Framework.Assert.AreEqual(FontWeights.BOLD, FontWeights.FromType1FontWeight("bold"));
            NUnit.Framework.Assert.AreEqual(FontWeights.EXTRA_BOLD, FontWeights.FromType1FontWeight("extrabold"));
            NUnit.Framework.Assert.AreEqual(FontWeights.EXTRA_BOLD, FontWeights.FromType1FontWeight("ultrabold"));
            NUnit.Framework.Assert.AreEqual(FontWeights.BLACK, FontWeights.FromType1FontWeight("heavy"));
            NUnit.Framework.Assert.AreEqual(FontWeights.BLACK, FontWeights.FromType1FontWeight("black"));
            NUnit.Framework.Assert.AreEqual(FontWeights.BLACK, FontWeights.FromType1FontWeight("ultra"));
            NUnit.Framework.Assert.AreEqual(FontWeights.BLACK, FontWeights.FromType1FontWeight("ultrablack"));
            NUnit.Framework.Assert.AreEqual(FontWeights.BLACK, FontWeights.FromType1FontWeight("fat"));
            NUnit.Framework.Assert.AreEqual(FontWeights.BLACK, FontWeights.FromType1FontWeight("extrablack"));
        }

        [NUnit.Framework.Test]
        public virtual void NormalizeFontWeightTest() {
            NUnit.Framework.Assert.AreEqual(FontWeights.THIN, FontWeights.NormalizeFontWeight(99));
            NUnit.Framework.Assert.AreEqual(FontWeights.BLACK, FontWeights.NormalizeFontWeight(1000));
            NUnit.Framework.Assert.AreEqual(FontWeights.MEDIUM, FontWeights.NormalizeFontWeight(505));
        }
    }
}
