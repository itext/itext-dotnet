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
using System.Collections.Generic;
using iText.Commons.Datastructures;
using iText.IO.Font.Otf;
using iText.Kernel.Font;
using iText.Test;

namespace iText.Layout.Font.Selectorstrategy {
    [NUnit.Framework.Category("UnitTest")]
    public class BestMatchFontSelectorStrategyTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void TwoDiacriticsInRowTest() {
            IFontSelectorStrategy strategy = FontSelectorTestsUtil.CreateStrategyWithFreeSansAndTNR(new BestMatchFontSelectorStrategy.BestMatchFontSelectorStrategyFactory
                ());
            IList<Tuple2<GlyphLine, PdfFont>> result = strategy.GetGlyphLines("L with accent: \u004f\u0301\u0302 abc");
            NUnit.Framework.Assert.AreEqual(3, result.Count);
            NUnit.Framework.Assert.AreEqual("L with accent: ", result[0].GetFirst().ToString());
            NUnit.Framework.Assert.AreEqual("\u004f\u0301\u0302 ", result[1].GetFirst().ToString());
            NUnit.Framework.Assert.AreEqual("abc", result[2].GetFirst().ToString());
            // Diacritics and symbol were separated, but the font is the same
            NUnit.Framework.Assert.AreEqual(result[0].GetSecond(), result[2].GetSecond());
        }

        [NUnit.Framework.Test]
        public virtual void OneDiacriticTest() {
            IFontSelectorStrategy strategy = FontSelectorTestsUtil.CreateStrategyWithFreeSansAndTNR(new BestMatchFontSelectorStrategy.BestMatchFontSelectorStrategyFactory
                ());
            IList<Tuple2<GlyphLine, PdfFont>> result = strategy.GetGlyphLines("L with accent: \u004f\u0302 abc");
            NUnit.Framework.Assert.AreEqual(3, result.Count);
            NUnit.Framework.Assert.AreEqual("L with accent: ", result[0].GetFirst().ToString());
            NUnit.Framework.Assert.AreEqual("\u004f\u0302 ", result[1].GetFirst().ToString());
            NUnit.Framework.Assert.AreEqual("abc", result[2].GetFirst().ToString());
            NUnit.Framework.Assert.AreNotEqual(result[0].GetSecond(), result[1].GetSecond());
        }

        [NUnit.Framework.Test]
        public virtual void OneDiacriticWithUnsupportedFontTest() {
            IFontSelectorStrategy strategy = FontSelectorTestsUtil.CreateStrategyWithTNR(new BestMatchFontSelectorStrategy.BestMatchFontSelectorStrategyFactory
                ());
            IList<Tuple2<GlyphLine, PdfFont>> result = strategy.GetGlyphLines("L with accent: \u004f\u0302 abc");
            NUnit.Framework.Assert.AreEqual(3, result.Count);
            NUnit.Framework.Assert.AreEqual("L with accent: \u004f", result[0].GetFirst().ToString());
            NUnit.Framework.Assert.AreEqual("", result[1].GetFirst().ToString());
            NUnit.Framework.Assert.AreEqual(" abc", result[2].GetFirst().ToString());
            NUnit.Framework.Assert.AreEqual(result[0].GetSecond(), result[2].GetSecond());
            NUnit.Framework.Assert.AreEqual(result[0].GetSecond(), result[1].GetSecond());
        }

        [NUnit.Framework.Test]
        public virtual void DiacriticFontDoesnotContainPreviousSymbolTest() {
            IFontSelectorStrategy strategy = FontSelectorTestsUtil.CreateStrategyWithNotoSans(new BestMatchFontSelectorStrategy.BestMatchFontSelectorStrategyFactory
                ());
            IList<Tuple2<GlyphLine, PdfFont>> result = strategy.GetGlyphLines("Ми\u0301ръ (mírə)");
            NUnit.Framework.Assert.AreEqual(6, result.Count);
            NUnit.Framework.Assert.AreEqual("Ми", result[0].GetFirst().ToString());
            NUnit.Framework.Assert.AreEqual("\u0301", result[1].GetFirst().ToString());
            NUnit.Framework.Assert.AreEqual("ръ (", result[2].GetFirst().ToString());
            NUnit.Framework.Assert.AreEqual("mír", result[3].GetFirst().ToString());
            NUnit.Framework.Assert.AreEqual("ə", result[4].GetFirst().ToString());
            NUnit.Framework.Assert.AreEqual(")", result[5].GetFirst().ToString());
            NUnit.Framework.Assert.AreEqual(result[0].GetSecond(), result[2].GetSecond());
            NUnit.Framework.Assert.AreEqual(result[2].GetSecond(), result[3].GetSecond());
        }

        [NUnit.Framework.Test]
        public virtual void OneDiacriticWithOneSupportedFontTest() {
            IFontSelectorStrategy strategy = FontSelectorTestsUtil.CreateStrategyWithFreeSans(new BestMatchFontSelectorStrategy.BestMatchFontSelectorStrategyFactory
                ());
            IList<Tuple2<GlyphLine, PdfFont>> result = strategy.GetGlyphLines("L with accent: \u004f\u0302 abc");
            NUnit.Framework.Assert.AreEqual(1, result.Count);
            NUnit.Framework.Assert.AreEqual("L with accent: \u004f\u0302 abc", result[0].GetFirst().ToString());
        }

        [NUnit.Framework.Test]
        public virtual void SurrogatePairsTest() {
            IFontSelectorStrategy strategy = FontSelectorTestsUtil.CreateStrategyWithOldItalic(new BestMatchFontSelectorStrategy.BestMatchFontSelectorStrategyFactory
                ());
            // this text contains three successive surrogate pairs
            IList<Tuple2<GlyphLine, PdfFont>> result = strategy.GetGlyphLines("text \uD800\uDF10\uD800\uDF00\uD800\uDF11 text"
                );
            NUnit.Framework.Assert.AreEqual(3, result.Count);
            NUnit.Framework.Assert.AreEqual("text ", result[0].GetFirst().ToString());
            NUnit.Framework.Assert.AreEqual("\uD800\uDF10\uD800\uDF00\uD800\uDF11 ", result[1].GetFirst().ToString());
            NUnit.Framework.Assert.AreEqual("text", result[2].GetFirst().ToString());
            NUnit.Framework.Assert.AreEqual(result[0].GetSecond(), result[2].GetSecond());
        }

        [NUnit.Framework.Test]
        public virtual void SimpleThreeFontTest() {
            IFontSelectorStrategy strategy = FontSelectorTestsUtil.CreateStrategyWithLimitedThreeFonts(new BestMatchFontSelectorStrategy.BestMatchFontSelectorStrategyFactory
                ());
            IList<Tuple2<GlyphLine, PdfFont>> result = strategy.GetGlyphLines("abcdefxyz");
            NUnit.Framework.Assert.AreEqual(3, result.Count);
            NUnit.Framework.Assert.AreEqual("abc", result[0].GetFirst().ToString());
            NUnit.Framework.Assert.AreEqual("def", result[1].GetFirst().ToString());
            NUnit.Framework.Assert.AreEqual("xyz", result[2].GetFirst().ToString());
        }

        [NUnit.Framework.Test]
        public virtual void ThreeFontWithSpacesTest() {
            IFontSelectorStrategy strategy = FontSelectorTestsUtil.CreateStrategyWithLimitedThreeFonts(new BestMatchFontSelectorStrategy.BestMatchFontSelectorStrategyFactory
                ());
            IList<Tuple2<GlyphLine, PdfFont>> result = strategy.GetGlyphLines(" axadefa ");
            NUnit.Framework.Assert.AreEqual(5, result.Count);
            NUnit.Framework.Assert.AreEqual(" a", result[0].GetFirst().ToString());
            NUnit.Framework.Assert.AreEqual("x", result[1].GetFirst().ToString());
            NUnit.Framework.Assert.AreEqual("a", result[2].GetFirst().ToString());
            NUnit.Framework.Assert.AreEqual("def", result[3].GetFirst().ToString());
            NUnit.Framework.Assert.AreEqual("a ", result[4].GetFirst().ToString());
        }

        [NUnit.Framework.Test]
        public virtual void WindowsLineEndingsTest() {
            IFontSelectorStrategy strategy = FontSelectorTestsUtil.CreateStrategyWithFreeSans(new BestMatchFontSelectorStrategy.BestMatchFontSelectorStrategyFactory
                ());
            IList<Tuple2<GlyphLine, PdfFont>> result = strategy.GetGlyphLines("Hello\r\n   World!\r\n ");
            NUnit.Framework.Assert.AreEqual(1, result.Count);
            NUnit.Framework.Assert.AreEqual("Hello\r\n   World!\r\n ", result[0].GetFirst().ToString());
        }
    }
}
