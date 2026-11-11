/*
This file is part of jsoup, see NOTICE.txt in the root of the repository.
It may contain modifications beyond the original version.
*/
using System;
using iText.Test;

namespace iText.StyledXmlParser.Jsoup.Helper {
    [NUnit.Framework.Category("UnitTest")]
    public class ValidateTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void TestNotNull() {
            Validate.NotNull("foo");
            bool threw = false;
            try {
                Validate.NotNull(null);
            }
            catch (ArgumentException) {
                threw = true;
            }
            NUnit.Framework.Assert.IsTrue(threw);
        }
    }
}
