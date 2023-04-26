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
using iText.Layout.Hyphenation;
using iText.Test;

namespace iText.Layout {
    [NUnit.Framework.Category("IntegrationTest")]
    public class HyphenateResultTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void UkraineHyphenTest() {
            //здравствуйте
            TestHyphenateResult("uk", "\u0437\u0434\u0440\u0430\u0432\u0441\u0442\u0432\u0443\u0439", new int[] { 5 });
        }

        [NUnit.Framework.Test]
        public virtual void UkraineNoneHyphenTest() {
            //день
            TestHyphenateResult("uk", "\u0434\u0435\u043D\u044C", null);
        }

        [NUnit.Framework.Test]
        public virtual void ParenthesisTest01() {
            //Annuitätendarlehen
            TestHyphenateResult("de", "((:::(\"|;Annuitätendarlehen|\")))", new int[] { 5, 7, 10, 13, 15 });
        }

        [NUnit.Framework.Test]
        public virtual void HindiHyphResult() {
            //लाभहानि
            TestHyphenateResult("hi", "लाभहानि", new int[] { 3 });
        }

        [NUnit.Framework.Test]
        public virtual void SpacesTest01() {
            //Annuitätendarlehen
            TestHyphenateResult("de", "    Annuitätendarlehen", new int[] { 5, 7, 10, 13, 15 });
        }

        [NUnit.Framework.Test]
        public virtual void SoftHyphenTest01() {
            //Ann\u00ADuit\u00ADätendarl\u00ADehen
            TestHyphenateResult("de", "Ann\u00ADuit\u00ADätendarl\u00ADehen", new int[] { 3, 7, 16 });
        }

        [NUnit.Framework.Test]
        public virtual void StackoverflowTestDe() {
            //https://stackoverflow.com/
            TestHyphenateResult("de", "https://stackoverflow.com/", new int[] { 3, 14, 17 });
        }

        [NUnit.Framework.Test]
        public virtual void StackoverflowTestEn() {
            //https://stackoverflow.com/
            TestHyphenateResult("en", "https://stackoverflow.com/", new int[] { 13, 17 });
        }

        [NUnit.Framework.Test]
        public virtual void NonBreakingHyphenTest01() {
            //99\u2011verheiratet
            TestHyphenateResult("de", "999\u2011verheiratet", new int[] { 3, 6, 8 });
        }

        [NUnit.Framework.Test]
        public virtual void NonBreakingHyphenTest02() {
            //honorificabilitudinitatibus
            TestHyphenateResult("en", "honorificabilitudinitatibus", new int[] { 3, 5, 6, 9, 11, 13, 15, 19, 21, 22, 24
                 });
        }

        [NUnit.Framework.Test]
        public virtual void NonBreakingHyphenTest02A() {
            //honorificabil\u2011itudinitatibus
            TestHyphenateResult("en", "honorificabil\u2011itudinitatibus", new int[] { 3, 5, 6, 9, 11, 20, 22, 23, 25 }
                );
        }

        [NUnit.Framework.Test]
        public virtual void NumberTest01() {
            //123456789
            TestHyphenateResult("en", "123456789", null);
        }

        private void TestHyphenateResult(String lang, String testWorld, int[] expectedHyphenatePoints) {
            String[] parts = iText.Commons.Utils.StringUtil.Split(lang, "_");
            lang = parts[0];
            String country = (parts.Length == 2) ? parts[1] : null;
            HyphenationConfig config = new HyphenationConfig(lang, country, 3, 3);
            iText.Layout.Hyphenation.Hyphenation result = config.Hyphenate(testWorld);
            if (result != null) {
                NUnit.Framework.Assert.AreEqual(expectedHyphenatePoints, result.GetHyphenationPoints());
            }
            else {
                NUnit.Framework.Assert.IsNull(expectedHyphenatePoints);
            }
        }
    }
}
