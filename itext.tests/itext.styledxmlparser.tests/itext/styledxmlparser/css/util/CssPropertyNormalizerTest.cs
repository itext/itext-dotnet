using System;

namespace iText.StyledXmlParser.Css.Util {
    public class CssPropertyNormalizerTest {
        [NUnit.Framework.Test]
        public virtual void TestUrlNormalizationSimple() {
            Test("url('data:image/png;base64,iVBORw0K')", "url('data:image/png;base64,iVBORw0K')");
        }

        [NUnit.Framework.Test]
        public virtual void TestUrlNormalizationLineTerminators() {
            // Test is initially added to ensure equal behavior between Java and C#.
            // The behavior itself might be reconsidered in the future. Browsers do not forgive newlines in base64 expressions
            Test("url(data:image/png;base64,iVBOR\nw0K)", "url(data:image/png;base64,iVBOR\nw0K)");
            Test("url(data:image/png;base64,iVBOR\rw0K)", "url(data:image/png;base64,iVBOR\rw0K)");
            Test("url(data:image/png;base64,iVBOR\r\nw0K)", "url(data:image/png;base64,iVBOR\r\nw0K)");
        }

        private void Test(String input, String expectedOutput) {
            String result = CssPropertyNormalizer.Normalize(input);
            NUnit.Framework.Assert.AreEqual(expectedOutput, result);
        }
    }
}
