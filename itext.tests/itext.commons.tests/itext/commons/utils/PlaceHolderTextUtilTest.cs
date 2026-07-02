/*
This file is part of the iText (R) project.
Copyright (c) 1998-2026 Apryse Group NV
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
using System.Text;
using iText.Test;

namespace iText.Commons.Utils {
    [NUnit.Framework.Category("UnitTest")]
    public class PlaceHolderTextUtilTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void GetPlaceHolderByCharacterTextSimple() {
            int amountOfCharacters = 24;
            String result = PlaceHolderTextUtil.GetPlaceHolderText(PlaceHolderTextUtil.PlaceHolderTextBy.CHARACTERS, amountOfCharacters
                );
            NUnit.Framework.Assert.AreEqual(amountOfCharacters, result.Length);
            NUnit.Framework.Assert.AreEqual(GetExpectedPlaceHolderTextByCharacters(amountOfCharacters), result);
        }

        [NUnit.Framework.Test]
        public virtual void GetPlaceHolderByCharactersTextOverflow() {
            int amountOfCharacters = PlaceHolderTextUtil.TEMPLATE.Length + 24;
            String result = PlaceHolderTextUtil.GetPlaceHolderText(PlaceHolderTextUtil.PlaceHolderTextBy.CHARACTERS, amountOfCharacters
                );
            NUnit.Framework.Assert.AreEqual(amountOfCharacters, result.Length);
            NUnit.Framework.Assert.AreEqual(GetExpectedPlaceHolderTextByCharacters(amountOfCharacters), result);
            NUnit.Framework.Assert.IsTrue(result.EndsWith(GetExpectedPlaceHolderTextByCharacters(24)));
        }

        [NUnit.Framework.Test]
        public virtual void GetPlaceHolderByWordsTextSimple() {
            int amountOfWords = 5;
            String result = PlaceHolderTextUtil.GetPlaceHolderText(PlaceHolderTextUtil.PlaceHolderTextBy.WORDS, amountOfWords
                );
            NUnit.Framework.Assert.AreEqual(GetExpectedPlaceHolderTextByWords(amountOfWords), result);
        }

        [NUnit.Framework.Test]
        public virtual void GetPlaceHolderByWordsTextOverflow() {
            int amountOfWords = iText.Commons.Utils.StringUtil.Split(PlaceHolderTextUtil.TEMPLATE, " ").Length + 5;
            String result = PlaceHolderTextUtil.GetPlaceHolderText(PlaceHolderTextUtil.PlaceHolderTextBy.WORDS, amountOfWords
                );
            NUnit.Framework.Assert.AreEqual(GetExpectedPlaceHolderTextByWords(amountOfWords), result);
            NUnit.Framework.Assert.IsTrue(result.EndsWith(GetExpectedPlaceHolderTextByWords(5)));
        }

        private static String GetExpectedPlaceHolderTextByWords(int amount) {
            String[] words = iText.Commons.Utils.StringUtil.Split(PlaceHolderTextUtil.TEMPLATE, " ");
            StringBuilder sb = new StringBuilder(amount * 5);
            for (int i = 0; i < amount; i++) {
                sb.Append(words[i % words.Length]);
                if (i + 1 == amount) {
                    break;
                }
                sb.Append(' ');
            }
            return sb.ToString();
        }

        private static String GetExpectedPlaceHolderTextByCharacters(int amount) {
            String template = PlaceHolderTextUtil.TEMPLATE;
            StringBuilder sb = new StringBuilder(amount);
            for (int i = 0; i < amount; i++) {
                sb.Append(template[i % template.Length]);
            }
            return sb.ToString();
        }
    }
}
