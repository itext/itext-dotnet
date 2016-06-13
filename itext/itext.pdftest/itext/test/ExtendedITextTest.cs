using NUnit.Framework;

namespace iText.Test {
    [LogListener]
    public abstract class ExtendedITextTest : ITextTest {
        [SetUp]
        public virtual void BeforeTestMethodAction() {
        }

        [TearDown]
        public virtual void AfterTestMethodAction() {
        }
    }
}
