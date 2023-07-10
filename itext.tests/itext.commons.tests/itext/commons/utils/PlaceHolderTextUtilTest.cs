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
