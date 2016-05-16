using iTextSharp.Test;
using NUnit.Framework;

namespace iTextSharp.Test {
    public abstract class ExtendedITextTest : ITextTest {
        [SetUp]
        public virtual void BeforeTestMethodAction() {
        }

        [TearDown]
        public virtual void AfterTestMethodAction() {
        }
    }
}
