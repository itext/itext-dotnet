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
            NUnit.Framework.Assert.AreEqual(result, "Portable Document Format");
        }

        [NUnit.Framework.Test]
        public virtual void GetPlaceHolderByCharactersTextOverflow() {
            int amountOfCharacters = 31222 + 24;
            String result = PlaceHolderTextUtil.GetPlaceHolderText(PlaceHolderTextUtil.PlaceHolderTextBy.CHARACTERS, amountOfCharacters
                );
            NUnit.Framework.Assert.AreEqual(amountOfCharacters, result.Length);
            NUnit.Framework.Assert.IsTrue(result.EndsWith("Portable Document Format"));
        }

        [NUnit.Framework.Test]
        public virtual void GetPlaceHolderByWordsTextSimple() {
            int amountOfWords = 5;
            String result = PlaceHolderTextUtil.GetPlaceHolderText(PlaceHolderTextUtil.PlaceHolderTextBy.WORDS, amountOfWords
                );
            NUnit.Framework.Assert.AreEqual(44, result.Length);
        }

        [NUnit.Framework.Test]
        public virtual void GetPlaceHolderByWordsTextOverflow() {
            int amountOfCharacters = 4000;
            String result = PlaceHolderTextUtil.GetPlaceHolderText(PlaceHolderTextUtil.PlaceHolderTextBy.WORDS, amountOfCharacters
                );
            NUnit.Framework.Assert.AreEqual(25472, result.Length);
        }
    }
}
