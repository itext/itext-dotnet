using iText.Layout.Properties;
using iText.Test;

namespace iText.Layout {
    public class BackgroundRepeatUnitTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void ConstructorTest() {
            BackgroundRepeat backgroundRepeat = new BackgroundRepeat(true, false);
            NUnit.Framework.Assert.IsTrue(backgroundRepeat.IsRepeatX());
            NUnit.Framework.Assert.IsFalse(backgroundRepeat.IsRepeatY());
        }
    }
}
